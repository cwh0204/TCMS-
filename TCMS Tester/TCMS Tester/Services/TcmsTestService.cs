using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
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

        // ★ [핵심] VDI 패킷 도착 시점 판별을 위한 수신 시퀀스 카운터
        private long _vdiUpdateCount = 0;

        // 링크 상태 및 VDI 주기적 폴링 제어
        private volatile bool _isFirstPacketReceived = false;
        private volatile bool _isPollingPaused = false; // DO 시험 중 폴링 일시정지 플래그
        private CancellationTokenSource _pollingCts;
        private Task _pollingTask;

        /// <summary>
        /// 타깃 보드로부터 실제 유효 UDP 패킷이 1회 이상 도착했는지 여부
        /// </summary>
        public bool IsLinkReady => _isFirstPacketReceived;

        /// <summary>
        /// VDI 패킷 갱신 번호 (PLC 조작 후 신규 패킷 여부 검증용)
        /// </summary>
        public long VdiUpdateCount
        {
            get { lock (_lockObj) return _vdiUpdateCount; }
        }

        // VDO 비동기 응답 대기 및 동기화 락
        private TaskCompletionSource<bool> _vdoResponseTcs;
        private readonly object _vdoLock = new object();

        // Command 상수
        private const ushort CMD_DO_WRITE = 0x0101;
        private const ushort CMD_DI_READ = 0x0102;
        private const ushort CMD_VDI_READ = 0x0301;
        private const ushort CMD_VDO_WRITE = 0x0401;

        // MVB 직결 시험용 커맨드 및 매크로 상수
        private const ushort CMD_MVB_TEST = 0x0501;
        private const ushort CHANNEL_LEFT = 0x0001;   // Direction: Left
        private const ushort CHANNEL_RIGHT = 0x0002;  // Direction: Right
        private const ushort CHANNEL_LINE_A = 0x000A; // Line: Line A
        private const ushort CHANNEL_LINE_B = 0x000B; // Line: Line B

        private const ushort CMD_WTB_TEST = 0x0502;

        private TaskCompletionSource<bool> _busTestCompleteTcs;
        private readonly object _busLock = new object();

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }
        public Action OnGridInvalidate { get; set; }

        public Func<string, EChannelState[], int, int, int, List<string>, List<CITester.TestResultJson.PinResultItem>, Func<byte[]>, Task> RunChannelSequenceFunc { get; set; }
        public Func<int, int, List<string>, List<CITester.TestResultJson.PinResultItem>, Task> RunAnalogSequenceFunc { get; set; }

        // 1. 기존 MVB 수신기 단독 사용 코드 호환용 생성자
        public TcmsTestService(MvbReceiver mvbReceiver)
        {
            _mvbReceiver = mvbReceiver;
            _targetIp = "10.0.1.11";
            _targetPort = 5060;
        }

        // 2. 신규 이더넷 UDP + MVB 수신기 통합 생성자
        public TcmsTestService(UdpService udpService, string targetIp = "10.0.1.11", int targetPort = 5060, MvbReceiver mvbReceiver = null)
        {
            _udpService = udpService;
            _targetIp = targetIp;
            _targetPort = targetPort;
            _mvbReceiver = mvbReceiver;
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
                _vdiUpdateCount = 0;
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

            // 기존 폴링 루프 정리 후 새로 시작
            _pollingCts?.Cancel();
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

                    // 배터리 절전 모드의 타이머 늘어짐을 감안하여 35ms로 폴링 주기 단축
                    await Task.Delay(35, token);
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
                    bool isSuccess = (e.Payloads != null && e.Payloads.Length >= 1 && e.Payloads[0] == 0x0001);
                    lock (_vdoLock)
                    {
                        _vdoResponseTcs?.TrySetResult(isSuccess);
                    }
                    break;

                case CMD_MVB_TEST:
                case CMD_WTB_TEST:
                    lock (_busLock)
                    {
                        OnLog?.Invoke($"[UDP RX] 보드 시험 완료 패킷 수신 (CMD: 0x{e.Command:X4})", Color.DarkGreen);
                        _busTestCompleteTcs?.TrySetResult(true);
                    }
                    break;
            }
        }

        public async Task<bool> RunBusTestSequenceAsync(string busName, string comPort, ushort cmd, int baudRate = 9600, int waitTimeoutMs = 15000)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_busLock)
            {
                _busTestCompleteTcs?.TrySetResult(false);
                _busTestCompleteTcs = tcs;
            }

            SerialPort serial = null;
            bool isSerialDataReceived = false;

            try
            {
                serial = new SerialPort(comPort, baudRate, Parity.None, 8, StopBits.One);
                serial.Open();
                serial.DiscardInBuffer();

                OnLog?.Invoke($"[{busName}] {comPort} 시리얼 포트 오픈 완료. 버스 데이터 대기 중...", Color.DarkBlue);

                bool sent = await _udpService.SendCommandAsync(_targetIp, _targetPort, cmd, CHANNEL_LEFT, CHANNEL_LINE_A);
                if (!sent)
                {
                    OnFailLog?.Invoke($"[{busName}] UDP 트리거 송신 실패 -> {_targetIp}:{_targetPort}", Color.Red);
                    return false;
                }

                OnLog?.Invoke($"[{busName}] UDP 트리거 송신 완료 (0x{cmd:X4}). 보드 완료 응답 대기...", Color.DarkBlue);

                DateTime limit = DateTime.Now.AddMilliseconds(waitTimeoutMs);
                while (DateTime.Now < limit)
                {
                    if (serial.IsOpen && serial.BytesToRead > 0)
                    {
                        int len = serial.BytesToRead;
                        byte[] buf = new byte[len];
                        serial.Read(buf, 0, len);
                        isSerialDataReceived = true;
                    }

                    if (tcs.Task.IsCompleted)
                    {
                        break;
                    }

                    await Task.Delay(50);
                }

                bool isUdpComplete = tcs.Task.IsCompleted && await tcs.Task;

                if (isSerialDataReceived && isUdpComplete)
                {
                    OnLog?.Invoke($"[{busName}] 시험 성공! (시리얼 데이터 확인 및 보드 완료 신호 수신)", Color.DarkGreen);
                    return true;
                }
                else
                {
                    string failReason = !isSerialDataReceived ? "시리얼 데이터 미유입" : "보드 완료 응답 타임아웃";
                    OnFailLog?.Invoke($"[{busName}] 시험 실패: {failReason}", Color.Red);
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnFailLog?.Invoke($"[{busName} 예외] {ex.Message}", Color.Red);
                return false;
            }
            finally
            {
                if (serial != null && serial.IsOpen)
                {
                    serial.Close();
                    serial.Dispose();
                }

                lock (_busLock)
                {
                    if (_busTestCompleteTcs == tcs) _busTestCompleteTcs = null;
                }
            }
        }

        public void ResetVdoResponseState()
        {
            lock (_vdoLock)
            {
                _vdoResponseTcs?.TrySetResult(false);
                _vdoResponseTcs = null;
            }
        }

        private void ParseVdiResponse(ushort[] payloads)
        {
            if (payloads == null) return;

            byte[] di1 = new byte[6];
            byte[] di2 = new byte[6];
            byte[] di3 = new byte[6];

            if (_strUnitType == "CC")
            {
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
                _vdiUpdateCount++; // ★ 신규 패킷 카운트 증가
            }
        }

        #endregion

        #region MVB 송수신 검증 및 하위 호환 메서드

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
        /// 특정 카운트 이후 새로 들어온 최신 VDI 패킷을 대기하여 반환합니다. (과거 패킷 읽기 방지)
        /// </summary>
        public async Task<byte[]> WaitForFreshRawDataAsync(string category, long previousUpdateCount, int maxWaitMs = 600)
        {
            DateTime limit = DateTime.Now.AddMilliseconds(maxWaitMs);
            while (DateTime.Now < limit)
            {
                lock (_lockObj)
                {
                    if (_vdiUpdateCount > previousUpdateCount)
                    {
                        return GetCurrentRawData(category);
                    }
                }
                await Task.Delay(10);
            }
            return GetCurrentRawData(category);
        }

        /// <summary>
        /// VDO 출력 보드의 드라이버 및 릴레이 제어 준비 완료 여부를 핑(전체 OFF 0x0401)으로 확인합니다.
        /// </summary>
        public async Task<bool> CheckVdoReadyAsync(int timeoutMs = 800)
        {
            return await SetVdoPinAsync(0, false, timeoutMs);
        }

        /// <summary>
        /// VDO 32ch 제어 명령 전송 후 타깃 보드의 실제 ACK([RX] 0x0401)를 대기합니다.
        /// </summary>
        public async Task<bool> SetVdoPinAsync(int pinNo, bool isOn, int timeoutMs = 800)
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
                bool sent = await _udpService.SendCommandAsync(_targetIp, _targetPort, CMD_VDO_WRITE, ch1To16, ch17To32);
                if (!sent) return false;

                // CancellationToken registration 대신 Task.WhenAny로 깔끔하고 확실한 타임아웃 제어
                var delayTask = Task.Delay(timeoutMs);
                var completedTask = await Task.WhenAny(tcs.Task, delayTask);

                if (completedTask == tcs.Task)
                {
                    return await tcs.Task;
                }
                else
                {
                    return false; // 타임아웃
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
            _isPollingPaused = false;
            OnLog?.Invoke("[통신] VDI 폴링 루프가 정지되었습니다.", Color.DarkGray);
        }

        public void PauseVdiPolling()
        {
            _isPollingPaused = true;
        }

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

                // ★ [핵심] 일시 정지 직전까지 네트워크 선로에 흐르던 잔여 VDI 패킷들이 완전히 수신/소진될 시간 확보
                await Task.Delay(200);
                ResetVdoResponseState();

                OnLog?.Invoke("[시스템] DO 시험 준비 중: VDO 드라이버 응답 대기 (최대 40초)...", Color.DarkGray);

                bool bVdoReady = false;
                DateTime dtVdoLimit = DateTime.Now.AddSeconds(40);
                while (DateTime.Now < dtVdoLimit)
                {
                    if (!checkIsTesting()) return false;

                    // 기본 타임아웃 800ms 적용
                    if (await CheckVdoReadyAsync(800))
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
                    ResetVdoResponseState();
                    await Task.Delay(100);
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