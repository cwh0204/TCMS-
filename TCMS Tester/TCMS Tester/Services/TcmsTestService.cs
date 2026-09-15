using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using CITester;
using TCMSTester.Models;
using TCMSTester.Protocol;
using static CITester.FormMain;

namespace TCMSTester.Services
{
    public class TcmsTestService
    {
        private readonly MvbReceiver _mvbReceiver;
        private readonly UdpService _udpService;
        private readonly string _targetIp;
        private readonly int _targetPort;
        private string _strUnitType = "TC";

        private TcmsPacket _latestPacket = new TcmsPacket();
        private readonly object _lockObj = new object();

        // 링크 상태 및 VDI 주기적 폴링 제어
        private volatile bool _isFirstPacketReceived = false;
        private volatile bool _isPollingPaused = false; // DO 시험 중 폴링 일시정지 플래그
        private CancellationTokenSource _pollingCts;
        private Task _pollingTask;

        /// <summary>
        /// 타깃 보드로부터 실제 유효 UDP 패킷이 1회 이상 도착했는지 여부
        /// </summary>
        public bool IsLinkReady => _isFirstPacketReceived;

        // VDO 비동기 응답 대기 및 동기화 락
        private TaskCompletionSource<bool> _vdoResponseTcs;
        private readonly object _vdoLock = new object();

        // Command 상수
        private const ushort CMD_DO_WRITE = 0x0101;
        private const ushort CMD_DI_READ = 0x0102;
        private const ushort CMD_VDI_READ = 0x0301;
        private const ushort CMD_VDO_WRITE = 0x0401;

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }
        public Action OnGridInvalidate { get; set; }

        public Func<string, EChannelState[], int, int, int, List<string>, List<CITester.TestResultJson.PinResultItem>, Func<byte[]>, Task> RunChannelSequenceFunc { get; set; }
        public Func<int, int, List<string>, List<CITester.TestResultJson.PinResultItem>, Task> RunAnalogSequenceFunc { get; set; }

        // 1. 기존 MVB 수신기 사용 코드 호환용 생성자
        public TcmsTestService(MvbReceiver mvbReceiver)
        {
            _mvbReceiver = mvbReceiver;
            _targetIp = "10.0.1.11";
            _targetPort = 5060;
        }

        // 2. 신규 이더넷 UDP 서비스용 생성자
        public TcmsTestService(UdpService udpService, string targetIp = "10.0.1.11", int targetPort = 5060)
        {
            _udpService = udpService;
            _targetIp = targetIp;
            _targetPort = targetPort;
        }

        #region 이더넷 통신 수명주기 및 VDI 주기적 폴링 제어

        public void StartUdpPolling(string strUnitType)
        {
            _strUnitType = strUnitType.ToUpper();
            _isPollingPaused = false;
            _isFirstPacketReceived = false;

            lock (_lockObj)
            {
                _latestPacket = new TcmsPacket();
            }

            if (_udpService != null && !_udpService.IsRunning)
            {
                _udpService.Start();
            }

            if (_udpService != null)
            {
                _udpService.PacketReceived -= OnUdpPacketReceived;
                _udpService.PacketReceived += OnUdpPacketReceived;
            }

            _pollingCts = new CancellationTokenSource();
            _pollingTask = Task.Run(() => PollingVdiLoopAsync(_pollingCts.Token));

            OnLog?.Invoke($"[통신] 이더넷 통신 및 VDI 폴링 루프 가동 ({_strUnitType} 모드, {_targetIp}:{_targetPort})", Color.DarkGreen);
        }

        private async Task PollingVdiLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!_isPollingPaused && _udpService != null)
                    {
                        await _udpService.SendCommandAsync(_targetIp, _targetPort, CMD_VDI_READ);
                    }
                    await Task.Delay(50, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnLog?.Invoke($"[폴링 에러] {ex.Message}", Color.Red);
                    await Task.Delay(200, token);
                }
            }
        }

        private void OnUdpPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            if (e == null) return;

            _isFirstPacketReceived = true;

            switch (e.Command)
            {
                case CMD_VDI_READ:
                    ParseVdiResponse(e.Payloads);
                    break;

                case CMD_VDO_WRITE:
                    // 타깃 응답 규격: [A1 53] [01 04] [01 00] (Result: 0x0001 성공)
                    bool isSuccess = (e.Payloads != null && e.Payloads.Length >= 1 && e.Payloads[0] == 0x0001);
                    lock (_vdoLock)
                    {
                        _vdoResponseTcs?.TrySetResult(isSuccess);
                    }
                    break;
            }
        }

        /// <summary>
        /// DO 시험 진입 전 대기 중인 잔여 VDO 응답 상태 초기화
        /// </summary>
        public void ResetVdoResponseState()
        {
            lock (_vdoLock)
            {
                _vdoResponseTcs?.TrySetResult(false);
                _vdoResponseTcs = null;
            }
        }

        private volatile bool _vdiLogPrintedOnce = false; // 최초 1회 정상 수신 포맷 확인용

        private void ParseVdiResponse(ushort[] payloads)
        {
            if (payloads == null) return;

            byte[] di1 = new byte[6];
            byte[] di2 = new byte[6];
            byte[] di3 = new byte[6];

            if (_strUnitType == "CC")
            {
                // CC 유닛은 12 words 중 6~8번 워드가 DI1, 9~11번 워드가 DI2
                if (payloads.Length >= 12)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[6]), 0, di1, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[7]), 0, di1, 2, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[8]), 0, di1, 4, 2);

                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[9]), 0, di2, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[10]), 0, di2, 2, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[11]), 0, di2, 4, 2);
                }
            }
            else // TC 유닛
            {
                // TC 유닛은 0~2번 DI1, 3~5번 DI2, 6~8번 DI3
                if (payloads.Length >= 6)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[0]), 0, di1, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[1]), 0, di1, 2, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[2]), 0, di1, 4, 2);

                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[3]), 0, di2, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[4]), 0, di2, 2, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[5]), 0, di2, 4, 2);
                }

                if (payloads.Length >= 9)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[6]), 0, di3, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[7]), 0, di3, 2, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(payloads[8]), 0, di3, 4, 2);
                }
            }

            lock (_lockObj)
            {
                _latestPacket.Di1Raw = di1;
                _latestPacket.Di2Raw = di2;
                _latestPacket.Di3Raw = di3;
            }
        }

        #endregion

        #region MVB 하위 호환 메서드 (기존 탭 빌드 보장)

        public void StartMvbReceiver(string strUnitType)
        {
            if (_mvbReceiver == null) return;

            _mvbReceiver.FrameSize = (strUnitType == "TC") ? MvbReceiver.PACKET_SIZE_TC : MvbReceiver.PACKET_SIZE_DEFAULT;
            if (!_mvbReceiver.IsRunning)
            {
                _mvbReceiver.Start();
                OnLog?.Invoke($"[통신] MVB 수신 스레드 시작", Color.DarkGreen);
            }
        }

        public void StopMvbReceiver()
        {
            if (_mvbReceiver == null) return;
            _mvbReceiver.Stop();
        }

        public async Task<bool> CheckMvbPortsAsync(int timeoutMs, params string[] targetPorts)
        {
            await Task.Delay(100);
            return true;
        }

        public async Task<bool> ExecuteSinglePinTestAsync(
            string strCategory,
            int nPinNo,
            int nBitIndex,
            Action<int, bool> setPlcDo,
            int nDelay = 200,
            CancellationToken cancellationToken = default)
        {
            await Task.Delay(nDelay, cancellationToken);
            return true;
        }

        #endregion

        #region I/O 조회 및 VDO 제어

        public byte[] GetCurrentRawData(string category)
        {
            lock (_lockObj)
            {
                string key = category != null ? category.ToUpper().Trim() : string.Empty;
                byte[] src = null;

                switch (key)
                {
                    case "DI1": src = _latestPacket.Di1Raw; break;
                    case "DI2": src = _latestPacket.Di2Raw; break;
                    case "DI3": src = _latestPacket.Di3Raw; break;
                    case "DO": src = _latestPacket.DoRaw; break;
                }

                return src != null ? (byte[])src.Clone() : null;
            }
        }

        /// <summary>
        /// VDO 출력 보드의 드라이버 및 릴레이 제어 준비 완료 여부를 핑(전체 OFF 0x0401)으로 확인합니다.
        /// </summary>
        public async Task<bool> CheckVdoReadyAsync(int timeoutMs = 500)
        {
            return await SetVdoPinAsync(0, false, timeoutMs);
        }

        /// <summary>
        /// VDO 32ch 제어 명령 전송 후 타깃 보드의 실제 ACK([RX] 0x0401)를 대기합니다. (응답 없거나 불일치 시 false)
        /// </summary>
        public async Task<bool> SetVdoPinAsync(int pinNo, bool isOn, int timeoutMs = 300)
        {
            if (_udpService == null || pinNo < 0 || pinNo > 32) return false;

            ushort ch1To16 = 0;
            ushort ch17To32 = 0;

            if (isOn && pinNo >= 1)
            {
                if (pinNo <= 16)
                    ch1To16 = (ushort)(1 << (pinNo - 1));
                else
                    ch17To32 = (ushort)(1 << (pinNo - 17));
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (_vdoLock)
            {
                _vdoResponseTcs?.TrySetResult(false);
                _vdoResponseTcs = tcs;
            }

            try
            {
                // 패킷 송신 실패 시 즉시 false
                bool sent = await _udpService.SendCommandAsync(_targetIp, _targetPort, CMD_VDO_WRITE, ch1To16, ch17To32);
                if (!sent) return false;

                // 타깃 보드로부터 [RX] ACK 응답이 올 때까지 timeoutMs 대기
                using (var cts = new CancellationTokenSource(timeoutMs))
                using (cts.Token.Register(() => tcs.TrySetResult(false)))
                {
                    return await tcs.Task;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                lock (_vdoLock)
                {
                    if (_vdoResponseTcs == tcs)
                    {
                        _vdoResponseTcs = null;
                    }
                }
            }
        }

        public void StopUdpPolling()
        {
            _pollingCts?.Cancel();
            if (_udpService != null)
            {
                _udpService.PacketReceived -= OnUdpPacketReceived;
            }
            _isFirstPacketReceived = false;
            OnLog?.Invoke("[통신] VDI 폴링 루프가 정지되었습니다.", Color.DarkGray);
        }

        /// <summary>
        /// DO 시험 시 타깃 보드 버스 병목을 막기 위해 VDI 폴링을 일시 정지합니다.
        /// </summary>
        public void PauseVdiPolling()
        {
            _isPollingPaused = true;
        }

        /// <summary>
        /// DO 시험 완료 후 VDI 폴링을 다시 재개합니다.
        /// </summary>
        public void ResumeVdiPolling()
        {
            _isPollingPaused = false;
        }

        public async Task<bool> ExecuteSingleRoundIoAsync(
    string strUnitType,
    int nLoop,
    Func<bool> checkIsTesting,
    ChannelContext context,
    CITester.TestResultJson.GridTestResult objDigitalGridResult,
    CITester.TestResultJson.GridTestResult objAnalogGridResult)
        {
            bool bRoundSuccess = true;
            int nAnimationDelay = 100;
            List<string> listFailedPins = new List<string>();

            if (RunChannelSequenceFunc != null) objDigitalGridResult.HeaderRounds.Add($"{nLoop}회차");
            if (RunAnalogSequenceFunc != null) objAnalogGridResult.HeaderRounds.Add($"{nLoop}회차");

            OnLog?.Invoke($"===== [{strUnitType} 유닛] 입·출력 시험 {nLoop}회차 시작 =====", Color.Purple);

            ClearChannelStates(context);
            OnGridInvalidate?.Invoke();

            await Task.Delay(300);

            // DI1 ~ DI3 입력 검사 (VDI 폴링 동작 상태)
            if (!checkIsTesting()) return false;
            if (context.ActiveDi1Count > 0 && RunChannelSequenceFunc != null)
                await RunChannelSequenceFunc("DI1", context.ActiveDi1, context.ActiveDi1Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI1"));

            if (!checkIsTesting()) return false;
            if (context.ActiveDi2Count > 0 && RunChannelSequenceFunc != null)
                await RunChannelSequenceFunc("DI2", context.ActiveDi2, context.ActiveDi2Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI2"));

            if (!checkIsTesting()) return false;
            if (strUnitType == "TC" && context.ActiveDi3Count > 0 && RunChannelSequenceFunc != null)
                await RunChannelSequenceFunc("DI3", context.ActiveDi3, context.ActiveDi3Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI3"));

            // DO 출력 검사 (VDI 폴링 일시 정지 후 VDO 드라이버 응답 대기)
            if (!checkIsTesting()) return false;
            if (context.ActiveDoCount > 0 && RunChannelSequenceFunc != null)
            {
                PauseVdiPolling();
                ResetVdoResponseState(); // 이전 잔여 응답 상태 초기화

                OnLog?.Invoke("[시스템] DO 시험 준비 중: VDO 드라이버 응답 대기 (최대 40초)...", Color.DarkGray);

                // VDO 보드가 실제로 깨어나서 0x0401 ACK를 돌려줄 때까지 동적 대기
                bool bVdoReady = false;
                DateTime dtVdoLimit = DateTime.Now.AddSeconds(40);
                while (DateTime.Now < dtVdoLimit)
                {
                    if (!checkIsTesting()) return false;

                    if (await CheckVdoReadyAsync(500))
                    {
                        bVdoReady = true;
                        OnLog?.Invoke("[시스템] VDO 드라이버 응답 확인 완료! DO 시험을 시작합니다.", Color.DarkGreen);
                        break;
                    }
                    await Task.Delay(500);
                }

                if (!bVdoReady)
                {
                    OnLog?.Invoke("[시스템] VDO 드라이버 응답 시간 초과 (40초 경과). 시퀀스를 진행합니다.", Color.Red);
                }

                await Task.Delay(300); // VME 버스 안정화

                try
                {
                    await RunChannelSequenceFunc("DO", context.ActiveDo, context.ActiveDoCount, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DO"));
                }
                finally
                {
                    ResumeVdiPolling();
                }
            }

            // 판정 기록
            if (listFailedPins.Count > 0)
            {
                bRoundSuccess = false;
                OnFailLog?.Invoke($"{nLoop}회차 입출력 - {string.Join(", ", listFailedPins)} 오류", Color.DarkRed);
            }
            else
            {
                OnFailLog?.Invoke($"{nLoop}회차 입출력 - 모든 채널 정상", Color.Green);
            }

            OnLog?.Invoke($"===== [{strUnitType} 유닛] 입·출력 시험 {nLoop}회차 종료 =====", Color.Purple);
            return bRoundSuccess;
        }

        private void ClearChannelStates(ChannelContext context)
        {
            if (context == null) return;
            if (context.ActiveDi1 != null) Array.Clear(context.ActiveDi1, 0, context.ActiveDi1.Length);
            if (context.ActiveDi2 != null) Array.Clear(context.ActiveDi2, 0, context.ActiveDi2.Length);
            if (context.ActiveDi3 != null) Array.Clear(context.ActiveDi3, 0, context.ActiveDi3.Length);
            if (context.ActiveDo != null) Array.Clear(context.ActiveDo, 0, context.ActiveDo.Length);
        }

        #endregion
    }
}