using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CITester;
using Newtonsoft.Json;
using TCMSTester.Models;
using TCMSTester.Protocol;
using static CITester.FormMain;

namespace TCMSTester.Services
{
    public class TcmsTestService
    {
        private readonly MvbReceiver _mvbReceiver;

        // 파싱된 최신 패킷 보관 (스레드 세이프 보장)
        private TcmsPacket _latestPacket;
        private string _latestPortAddress = string.Empty;
        private readonly object _lockObj = new object();

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }
        public Action OnGridInvalidate { get; set; }

        public Func<string, EChannelState[], int, int, int, List<string>, List<TestResultJson.PinResultItem>, Func<byte[]>, Task> RunChannelSequenceFunc { get; set; }
        public Func<int, int, List<string>, List<TestResultJson.PinResultItem>, Task> RunAnalogSequenceFunc { get; set; }

        public TcmsTestService(MvbReceiver mvbReceiver)
        {
            _mvbReceiver = mvbReceiver;
        }

        #region MVB 수신기 라이프사이클 제어

        /// <summary>
        /// MVB 수신기를 초기화하고 패킷 수신 스레드를 시작합니다.
        /// </summary>
        public void StartMvbReceiver(string strUnitType)
        {
            if (_mvbReceiver == null)
            {
                OnLog?.Invoke("[통신 경고] _mvbReceiver 객체가 null입니다!", Color.Red);
                return;
            }

            _mvbReceiver.FrameSize = (strUnitType == "TC")
                ? MvbReceiver.PACKET_SIZE_TC
                : MvbReceiver.PACKET_SIZE_DEFAULT;

            // 중복 바인딩 방지
            _mvbReceiver.PacketReceived -= OnMvbPacketReceived;
            _mvbReceiver.PacketReceived += OnMvbPacketReceived;

            _mvbReceiver.ErrorOccurred -= OnMvbErrorOccurred;
            _mvbReceiver.ErrorOccurred += OnMvbErrorOccurred;

            if (!_mvbReceiver.IsRunning)
            {
                _mvbReceiver.Start();
                OnLog?.Invoke($"[통신] MVB 수신 스레드가 시작되었습니다. (유닛:{strUnitType}, FrameSize:{_mvbReceiver.FrameSize})", Color.DarkGreen);
            }
        }

        /// <summary>
        /// MVB 수신 이벤트 연결을 안전하게 해제합니다.
        /// </summary>
        public void StopMvbReceiver()
        {
            if (_mvbReceiver == null) return;

            _mvbReceiver.PacketReceived -= OnMvbPacketReceived;
            _mvbReceiver.ErrorOccurred -= OnMvbErrorOccurred;
        }

        #endregion

        #region MVB 통신 포트 검증

        /// <summary>
        /// 타깃 MVB 포트들의 정상 수신 여부를 제한 시간 내에 확인합니다.
        /// </summary>
        public async Task<bool> CheckMvbPortsAsync(int timeoutMs, params string[] targetPorts)
        {
            if (_mvbReceiver == null)
            {
                OnLog?.Invoke("[통신 검사] MvbReceiver 객체가 null입니다.", Color.Red);
                return false;
            }

            if (targetPorts == null || targetPorts.Length == 0)
            {
                OnLog?.Invoke("[통신 검사] 검사할 대상 포트 목록이 없습니다.", Color.DarkOrange);
                return false;
            }

            var portCheckMap = targetPorts
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(p => p.Trim().ToUpper(), p => false, StringComparer.OrdinalIgnoreCase);

            if (portCheckMap.Count == 0) return false;

            if (!_mvbReceiver.IsRunning)
            {
                _mvbReceiver.Start();
            }

            var tcs = new TaskCompletionSource<bool>();
            using (var cts = new CancellationTokenSource(timeoutMs))
            {
                EventHandler<MvbPacketEventArgs> portCheckHandler = (s, e) =>
                {
                    if (string.IsNullOrEmpty(e.PortAddress)) return;

                    lock (portCheckMap)
                    {
                        if (portCheckMap.ContainsKey(e.PortAddress) && !portCheckMap[e.PortAddress])
                        {
                            portCheckMap[e.PortAddress] = true;
                            OnLog?.Invoke($"[통신 검사] 대상 포트 수신 확인: {e.PortAddress}", Color.DarkGreen);

                            if (portCheckMap.Values.All(v => v))
                            {
                                tcs.TrySetResult(true);
                            }
                        }
                    }
                };

                _mvbReceiver.PacketReceived += portCheckHandler;

                using (cts.Token.Register(() => tcs.TrySetResult(false)))
                {
                    bool isPassed = await tcs.Task;
                    _mvbReceiver.PacketReceived -= portCheckHandler;

                    if (isPassed)
                    {
                        OnLog?.Invoke($"[통신 검사] 합격 (모든 대상 포트 정상 수신: {string.Join(", ", portCheckMap.Keys)})", Color.Blue);
                    }
                    else
                    {
                        var missing = portCheckMap.Where(kv => !kv.Value).Select(kv => kv.Key).ToList();
                        OnLog?.Invoke($"[통신 검사] 불합격 (미수신 포트: {string.Join(", ", missing)})", Color.Red);
                        OnFailLog?.Invoke($"통신 불합격 - 미수신 포트: {string.Join(", ", missing)}", Color.Red);
                    }

                    return isPassed;
                }
            }
        }

        #endregion

        #region 단일 회차 입·출력 시험 시퀀스

        /// <summary>
        /// 지정된 단일 회차(nLoop)의 디지털/아날로그 입·출력 시험을 1회 수행합니다.
        /// </summary>
        public async Task<bool> ExecuteSingleRoundIoAsync(
            string strUnitType,
            int nLoop,
            Func<bool> checkIsTesting,
            ChannelContext context,
            TestResultJson.GridTestResult objDigitalGridResult,
            TestResultJson.GridTestResult objAnalogGridResult)
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

            // 디지털 입력 및 출력 검사
            if (!checkIsTesting()) return false;
            if (context.ActiveDi1Count > 0 && RunChannelSequenceFunc != null)
                await RunChannelSequenceFunc("DI1", context.ActiveDi1, context.ActiveDi1Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI1"));

            if (!checkIsTesting()) return false;
            if (context.ActiveDi2Count > 0 && RunChannelSequenceFunc != null)
                await RunChannelSequenceFunc("DI2", context.ActiveDi2, context.ActiveDi2Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI2"));

            if (!checkIsTesting()) return false;
            if (context.ActiveDi3Count > 0 && RunChannelSequenceFunc != null)
                await RunChannelSequenceFunc("DI3", context.ActiveDi3, context.ActiveDi3Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI3"));

            if (!checkIsTesting()) return false;
            if (context.ActiveDoCount > 0 && RunChannelSequenceFunc != null)
                await RunChannelSequenceFunc("DO", context.ActiveDo, context.ActiveDoCount, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DO"));

            // 아날로그 시험
            if (!checkIsTesting()) return false;
            if (RunAnalogSequenceFunc != null)
                await RunAnalogSequenceFunc(nAnimationDelay, nLoop, listFailedPins, objAnalogGridResult.PinDetails);

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

        #endregion

        #region MVB 수신 데이터 파싱 및 안전 추출

        private void OnMvbPacketReceived(object sender, MvbPacketEventArgs e)
        {
            if (e == null) return;

            lock (_lockObj)
            {
                _latestPortAddress = e.PortAddress;

                if (e.Packet != null)
                {
                    _latestPacket = e.Packet;
                    return;
                }
            }

            // 바이트 배열 직접 파싱 (불필요한 Hex 문자열 인코딩/디코딩 방지)
            if (e.RawData != null && e.RawData.Length >= 31)
            {
                ParsePacketBytes(e.RawData);
            }
            else if (e.RawData != null && e.RawData.Length > 0)
            {
                string rawHex = BitConverter.ToString(e.RawData).Replace("-", "");
                OnDataReceived(rawHex);
            }
        }

        private void OnMvbErrorOccurred(object sender, string errorMessage)
        {
            OnLog?.Invoke($"[MVB 수신 에러] {errorMessage}", Color.Red);
        }

        /// <summary>
        /// Hex 문자열 기반 패킷 수신 처리 (하위 호환 지원)
        /// </summary>
        public void OnDataReceived(string rawHex)
        {
            if (string.IsNullOrEmpty(rawHex)) return;

            string cleanHex = rawHex.Replace(" ", "").Trim().ToUpper();

            // 31바이트 = 62자리 (기존 68자리 제한 완화)
            if (cleanHex.Length < 62)
            {
                Debug.WriteLine($"[MVB Parser] 패킷 길이 부족 (Length={cleanHex.Length}): {cleanHex}");
                return;
            }

            try
            {
                byte[] bytes = ConvertHexToBytes(cleanHex);
                ParsePacketBytes(bytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MVB Parser] 데이터 변환 중 예외 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 바이트 배열에서 DI1~3, DO 데이터를 추출하여 최신 패킷 객체를 생성합니다.
        /// </summary>
        private void ParsePacketBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 31) return;

            try
            {
                byte[] di1 = new byte[6];
                byte[] di2 = new byte[6];
                byte[] di3 = new byte[6];
                byte[] doData = new byte[6];

                Array.Copy(bytes, 7, di1, 0, 6);
                Array.Copy(bytes, 13, di2, 0, 6);
                Array.Copy(bytes, 19, di3, 0, 6);
                Array.Copy(bytes, 25, doData, 0, 6);

                lock (_lockObj)
                {
                    _latestPacket = new TcmsPacket
                    {
                        Di1Raw = di1,
                        Di2Raw = di2,
                        Di3Raw = di3,
                        DoRaw = doData
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MVB Parser] 패킷 바이트 분해 에러: {ex.Message}");
            }
        }

        /// <summary>
        /// 현재 카테고리(DI1, DI2, DI3, DO)에 해당하는 최신 Raw 바이트를 반환합니다.
        /// (시험 시작 직후 첫 패킷 수신 지연 시 최대 500ms 동안 대기합니다.)
        /// </summary>
        private byte[] GetCurrentRawData(string category)
        {
            // 첫 패킷 미도착 시 최대 500ms 동기 폴링 대기
            int waitLimit = 50; // 10ms * 50 = 500ms
            while (_latestPacket == null && waitLimit > 0)
            {
                Thread.Sleep(10);
                waitLimit--;
            }

            lock (_lockObj)
            {
                if (_latestPacket == null)
                {
                    Debug.WriteLine($"[MVB Getter] _latestPacket 수신 대기 실패 (category={category})");
                    OnLog?.Invoke($"[디버그] GetCurrentRawData({category}) 호출됨 -> 패킷 미수신(NULL)", Color.Orange);
                    return null;
                }

                byte[] source = null;
                string key = category != null ? category.ToUpper() : string.Empty;

                switch (key)
                {
                    case "DI1": source = _latestPacket.Di1Raw; break;
                    case "DI2": source = _latestPacket.Di2Raw; break;
                    case "DI3": source = _latestPacket.Di3Raw; break;
                    case "DO": source = _latestPacket.DoRaw; break;
                    default: source = null; break;
                }

                if (source == null) return null;

                return (byte[])source.Clone();
            }
        }

        private byte[] ConvertHexToBytes(string hex)
        {
            if (hex.Length % 2 != 0) return null;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int hi = GetHexVal(hex[i * 2]);
                int lo = GetHexVal(hex[i * 2 + 1]);
                if (hi < 0 || lo < 0) return null;
                bytes[i] = (byte)((hi << 4) | lo);
            }
            return bytes;
        }

        private int GetHexVal(char hex)
        {
            int val = (int)hex;
            if (val >= '0' && val <= '9') return val - '0';
            if (val >= 'A' && val <= 'F') return val - 'A' + 10;
            if (val >= 'a' && val <= 'f') return val - 'a' + 10;
            return -1;
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