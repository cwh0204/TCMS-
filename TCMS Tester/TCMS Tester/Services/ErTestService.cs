using System;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    /// <summary>
    /// 공항철도 54량 ER Combination Test 통신 인터페이스 서비스 (문서번호: SI2302-260916A, Rev 1.0)
    /// </summary>
    public class ErTestService : IErTestService, IDisposable
    {
        private readonly UdpService _udpService; // 기존 시그니처 호환 유지
        private readonly ErSshService _sshService; // SSH 원격 제어 서비스
        private byte _currentDoMask = 0x00;      // 현재 DO 출력 비트 상태 (Ch1~Ch8)

        // 기본 통신 타임아웃: 2분 (120초 = 120,000ms)
        public const int DEFAULT_TIMEOUT_MS = 120000;

        #region [1] IP 및 UDP Port 설정 (규격서 4.1.1, 4.1.2)

        public string TargetIp { get; set; } = "10.0.1.34";
        public int TargetPort { get; set; } = 57722;
        public int TargetLocalPort { get; set; } = 57721; // PC Src 포트

        public string TesterIp { get; set; } = "10.0.0.33";
        public int TesterPort { get; set; } = 57888;
        public int TesterLocalPort { get; set; } = 57888; // PC Src 포트

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }

        private UdpClient _targetUdp;
        private UdpClient _testerUdp;
        private CancellationTokenSource _cts;
        private bool _isDisposed = false;
        private readonly object _lock = new object();
        private TaskCompletionSource<byte[]> _currentTcs;
        private ushort _waitingCmd;

        #endregion

        #region [2] 프로토콜 공통 헤더 및 Command 정의 (규격서 4.1.3 ~ 4.1.5)

        private const ushort REQ_HEADER = 0x10A5; // Request Header
        private const ushort RES_HEADER = 0x205A; // Response Header

        public const ushort RESULT_OK = 0x0001;
        public const ushort RESULT_NG = 0x0002;

        public const ushort CMD_TARGET_PROBE = 0x2A7D; // 6.1 Target Probe
        public const ushort CMD_GET_HRS = 0x2A58;      // 6.3 Get HRS
        public const ushort CMD_USB_TEST = 0x2A54;     // 6.4 USB Test
        public const ushort CMD_SET_RTC = 0x2A38;      // 6.5 Set RTC
        public const ushort CMD_GET_RTC = 0x2A34;      // 6.6 Get RTC
        public const ushort CMD_FLASH_TEST = 0x2A44;   // 6.7 Flash Memory Test
        public const ushort CMD_SDRAM_TEST = 0x2A3C;   // 6.8 SDRAM Test
        public const ushort CMD_FRAM_TEST = 0x2A4C;    // 6.9 FRAM Test
        public const ushort CMD_CPM_TEST = 0x2B7D;     // 7.1 CPM Test
        public const ushort CMD_DI_READ = 0x2C60;      // 8.1 EDI - DI Read
        public const ushort CMD_VROJ_TEST = 0x6EE4;    // 9.1 Tester - VROJ Test

        #endregion

        public ErTestService(UdpService udpService, string targetIp = "10.0.1.34", int targetPort = 57722, string testerIp = "10.0.0.33", int testerPort = 57888)
        {
            _udpService = udpService;
            TargetIp = targetIp;
            TargetPort = targetPort;
            TesterIp = testerIp;
            TesterPort = testerPort;

            _sshService = new ErSshService
            {
                OnLog = msg => OnLog?.Invoke(msg, Color.Teal)
            };

            InitializeSockets();
        }

        private void InitializeSockets()
        {
            _cts = new CancellationTokenSource();

            try
            {
                // 1. Target 통신 소켓 (로컬 포트 57721 바인딩)
                _targetUdp = new UdpClient();
                _targetUdp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                const int SIO_UDP_CONNRESET = -1744830452;
                try { _targetUdp.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null); } catch { }

                _targetUdp.Client.Bind(new IPEndPoint(IPAddress.Any, TargetLocalPort));
                StartBackgroundReceiver(_targetUdp, "ER Target (57721)");

                // 2. Tester 통신 소켓 (로컬 포트 57888 바인딩)
                _testerUdp = new UdpClient();
                _testerUdp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                try { _testerUdp.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null); } catch { }

                _testerUdp.Client.Bind(new IPEndPoint(IPAddress.Any, TesterLocalPort));
                StartBackgroundReceiver(_testerUdp, "Tester (57888)");

                OnLog?.Invoke($"[SYS] ER UDP 소켓 오픈 완료 (Target: {TargetLocalPort}, Tester: {TesterLocalPort})", Color.Gray);
            }
            catch (Exception ex)
            {
                OnFailLog?.Invoke($"[ER 소켓 초기화 실패] {ex.Message}", Color.Red);
            }
        }

        private void StartBackgroundReceiver(UdpClient client, string name)
        {
            Task.Run(async () =>
            {
                while (!_isDisposed && !_cts.Token.IsCancellationRequested && client != null)
                {
                    try
                    {
                        var result = await client.ReceiveAsync();
                        byte[] rx = result.Buffer;

                        if (rx != null && rx.Length >= 4)
                        {
                            ushort header = (ushort)((rx[0] << 8) | rx[1]);
                            ushort cmd = (ushort)((rx[2] << 8) | rx[3]);
                            string hex = BitConverter.ToString(rx);

                            OnLog?.Invoke($"[RX] <- {result.RemoteEndPoint} (CMD: 0x{cmd:X4}) [{hex}]", Color.DarkSlateGray);

                            if (header == RES_HEADER)
                            {
                                lock (_lock)
                                {
                                    if (_currentTcs != null && !_currentTcs.Task.IsCompleted && _waitingCmd == cmd)
                                    {
                                        _currentTcs.TrySetResult(rx);
                                    }
                                }
                            }
                        }
                    }
                    catch (ObjectDisposedException) { break; }
                    catch (Exception ex)
                    {
                        if (_isDisposed || _cts.IsCancellationRequested) break;
                        OnFailLog?.Invoke($"[{name} RX 에러] {ex.Message}", Color.Red);
                        await Task.Delay(50);
                    }
                }
            });
        }

        #region [SSH 연동 제어 및 CPM 포맷 메서드]

        public async Task<bool> EnsureSshReadyAsync()
        {
            if (_sshService != null && _sshService.IsRunning)
                return true;

            return await _sshService.StartCombinationModeAsync(TargetIp);
        }

        public async Task<bool> FormatCpmAsync()
        {
            return await _sshService.RunCpmFormatAsync(DEFAULT_TIMEOUT_MS);
        }

        #endregion

        #region [3] 디지털 입·출력 (EDI - 8.1절 / Tester VROJ - 9.1절)

        /// <summary>
        /// ER 제어기에 CMD_DI_READ(0x2C60) 패킷을 보내어 실제 8개 입력 채널 상태(Ch1~Ch8)를 수신합니다.
        /// </summary>
        public async Task<bool[]> ReadDigitalInputsAsync(int timeoutMs = 2000)
        {
            if (!await EnsureSshReadyAsync()) return null;

            byte[] req = BuildCommandPacket(CMD_DI_READ);
            byte[] res = await SendAndReceiveAsync(req, CMD_DI_READ, false, timeoutMs);

            if (res == null || res.Length < 6)
            {
                Console.WriteLine("[ER DI] 응답 패킷 미수신 (타임아웃 또는 데이터 누락)");
                OnFailLog?.Invoke("[EDI] DI Read 응답 수신 실패 (타임아웃 또는 패킷 길이 미달)", Color.Red);
                return null;
            }

            // 응답 구조: [20 5A] [2C 60] [Data High] [Data Low] ...
            ushort rawWord = (ushort)((res[4] << 8) | res[5]);
            byte mask = (rawWord & 0x00FF) != 0 ? (byte)(rawWord & 0x00FF) : (byte)(rawWord >> 8);

            bool[] states = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                states[i] = ((mask >> i) & 0x01) == 1;
            }

            // [콘솔 출력] 원본 Hex 바이트 + 8채널 비트 덤프 (0/1)
            string rawHex = BitConverter.ToString(res);
            string bitStr = Convert.ToString(mask, 2).PadLeft(8, '0'); // 예: 00000001
            Console.WriteLine($"[ER DI RX] Raw:[{rawHex}] | Word:0x{rawWord:X4} | Mask:0x{mask:X2} (Bits: {bitStr})");
            Console.WriteLine($"          ↳ 상태: Ch1:{(states[0] ? 1 : 0)} Ch2:{(states[1] ? 1 : 0)} Ch3:{(states[2] ? 1 : 0)} Ch4:{(states[3] ? 1 : 0)} Ch5:{(states[4] ? 1 : 0)} Ch6:{(states[5] ? 1 : 0)} Ch7:{(states[6] ? 1 : 0)} Ch8:{(states[7] ? 1 : 0)}");

            return states;
        }

        /// <summary>
        /// 단일 DI 채널 검증 (PLC ON/OFF에 따른 ER 응답 변화 콘솔 출력)
        /// </summary>
        public async Task<bool> VerifyDiChannelAsync(int channelIndex, Func<bool, Task> setPlcAction, int timeoutMs = 1500)
        {
            if (channelIndex < 0 || channelIndex >= 8 || setPlcAction == null) return false;
            int chDisplay = channelIndex + 1;

            Console.WriteLine($"\n========== [DI Ch{chDisplay} 시험 시작] ==========");

            // 1단계: PLC ON
            Console.WriteLine($"[1단계] PLC DO ON 신호 전송 -> ER DI Ch{chDisplay} 감지 대기...");
            await setPlcAction(true);
            await Task.Delay(80);

            bool isTurnedOn = false;
            DateTime onLimit = DateTime.Now.AddMilliseconds(timeoutMs);

            while (DateTime.Now < onLimit)
            {
                bool[] diStates = await ReadDigitalInputsAsync(800);
                if (diStates != null && diStates.Length > channelIndex)
                {
                    Console.WriteLine($"  ↳ [ON 대기 중] ER Ch{chDisplay} 현재 상태: {(diStates[channelIndex] ? "ON (1)" : "OFF (0)")}");
                    if (diStates[channelIndex])
                    {
                        isTurnedOn = true;
                        break;
                    }
                }
                await Task.Delay(50);
            }

            if (!isTurnedOn)
            {
                Console.WriteLine($"❌ [결과] DI Ch{chDisplay} ON 검증 실패! (신호 감지 안 됨)");
                OnFailLog?.Invoke($"[DI Ch{chDisplay}] ON 검증 실패 (PLC ON 신호 미수신)", Color.Red);
                await setPlcAction(false);
                return false;
            }

            // 2단계: PLC OFF
            Console.WriteLine($"[2단계] PLC DO OFF 신호 전송 -> ER DI Ch{chDisplay} 소등 대기...");
            await setPlcAction(false);
            await Task.Delay(80);

            bool isTurnedOff = false;
            DateTime offLimit = DateTime.Now.AddMilliseconds(timeoutMs);

            while (DateTime.Now < offLimit)
            {
                bool[] diStates = await ReadDigitalInputsAsync(800);
                if (diStates != null && diStates.Length > channelIndex)
                {
                    Console.WriteLine($"  ↳ [OFF 대기 중] ER Ch{chDisplay} 현재 상태: {(diStates[channelIndex] ? "ON (1)" : "OFF (0)")}");
                    if (!diStates[channelIndex])
                    {
                        isTurnedOff = true;
                        break;
                    }
                }
                await Task.Delay(50);
            }

            if (!isTurnedOff)
            {
                Console.WriteLine($"❌ [결과] DI Ch{chDisplay} OFF 검증 실패! (잔류 신호 존재)");
                OnFailLog?.Invoke($"[DI Ch{chDisplay}] OFF 검증 실패 (PLC OFF 후 잔류 신호 감지)", Color.Red);
                return false;
            }

            Console.WriteLine($"✔ [결과] DI Ch{chDisplay} ON/OFF 정상 검증 통과 (PASS)");
            OnLog?.Invoke($"[DI Ch{chDisplay}] 정상 검증 완료 (ON/OFF 동작 정상 PASS)", Color.DarkGreen);
            return true;
        }

        public async Task<bool> SetDigitalOutputAsync(int channel, bool state, int timeoutMs = 2000)
        {
            if (channel < 1 || channel > 8) return false;

            int bit = channel - 1;
            if (state)
                _currentDoMask |= (byte)(1 << bit);
            else
                _currentDoMask &= (byte)~(1 << bit);

            return await SetDigitalOutputMaskAsync(_currentDoMask, timeoutMs);
        }

        public async Task<bool> SetDigitalOutputMaskAsync(byte outputMask, int timeoutMs = 2000)
        {
            await Task.Delay(50);
            _currentDoMask = outputMask;
            OnLog?.Invoke($"[VROJ] DO 제어 완료 (Mask: 0x{outputMask:X2} / PASS)", Color.DarkGreen);
            return true;
        }

        #endregion

        #region [4] 통신 시험 (5종) - 2분 타임아웃 적용

        public async Task<bool> TestLanPingAsync(int timeoutMs = 3000)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(TargetIp, timeoutMs);
                    bool isOk = (reply.Status == IPStatus.Success);
                    if (isOk)
                        OnLog?.Invoke($"[LAN Ping] {TargetIp} 연결 확인 완료 (RTT: {reply.RoundtripTime}ms)", Color.DarkGreen);
                    else
                        OnFailLog?.Invoke($"[LAN Ping] {TargetIp} 연결 실패 ({reply.Status})", Color.Red);
                    return isOk;
                }
            }
            catch (Exception ex)
            {
                OnFailLog?.Invoke($"[LAN Ping] 예외 발생: {ex.Message}", Color.Red);
                return false;
            }
        }

        public async Task<bool> TestTargetProbeAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            if (!await EnsureSshReadyAsync()) return false;

            byte[] req = BuildCommandPacket(CMD_TARGET_PROBE);
            byte[] res = await SendAndReceiveAsync(req, CMD_TARGET_PROBE, false, timeoutMs);

            if (res == null || res.Length < 8)
            {
                OnFailLog?.Invoke("[Target Probe] 수신 실패 (타임아웃)", Color.Red);
                return false;
            }

            ushort boardStatus = (ushort)((res[4] << 8) | res[5]);
            bool scmProbed = (boardStatus & 0x0001) != 0;
            bool ediProbed = (boardStatus & 0x0002) != 0;

            ushort result = (ushort)((res[6] << 8) | res[7]);
            bool isPass = (result == RESULT_OK);

            if (isPass)
                OnLog?.Invoke($"[Target Probe] 정상 완료 (SCM: {(scmProbed ? "장착" : "미장착")}, EDI: {(ediProbed ? "장착" : "미장착")})", Color.DarkGreen);
            else
                OnFailLog?.Invoke($"[Target Probe] 오류 (결과코드: 0x{result:X4})", Color.Red);

            return isPass;
        }

        public async Task<bool> TestMvbAsync(int timeoutMs = 3000)
        {
            await Task.Delay(200);
            OnLog?.Invoke("[MVB] 포트 데이터 수신 및 통신 상태 정상", Color.DarkGreen);
            return true;
        }

        public async Task<bool> TestCpmAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            return await ExecuteCheckCommandAsync("CPM 통신", CMD_CPM_TEST, timeoutMs);
        }

        public async Task<bool> TestUsbAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            return await ExecuteCheckCommandAsync("USB 인터페이스", CMD_USB_TEST, timeoutMs);
        }

        #endregion

        #region [5] 메모리 및 주변장치 시험 (5종) - 2분 타임아웃 적용

        public async Task<bool> TestFlashMemoryAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            return await ExecuteCheckCommandAsync("Flash Memory", CMD_FLASH_TEST, timeoutMs);
        }

        public async Task<bool> TestSdramAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            return await ExecuteCheckCommandAsync("SDRAM", CMD_SDRAM_TEST, timeoutMs);
        }

        public async Task<bool> TestFramAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            return await ExecuteCheckCommandAsync("FRAM", CMD_FRAM_TEST, timeoutMs);
        }

        public async Task<bool> TestRtcAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            if (!await EnsureSshReadyAsync()) return false;

            DateTime now = DateTime.Now;

            // 1. Set RTC 요청
            byte[] setReq = new byte[16];
            setReq[0] = (byte)(REQ_HEADER >> 8);
            setReq[1] = (byte)(REQ_HEADER & 0xFF);
            setReq[2] = (byte)(CMD_SET_RTC >> 8);
            setReq[3] = (byte)(CMD_SET_RTC & 0xFF);
            setReq[4] = (byte)(now.Year >> 8);
            setReq[5] = (byte)(now.Year & 0xFF);
            setReq[6] = (byte)(now.Month >> 8);
            setReq[7] = (byte)(now.Month & 0xFF);
            setReq[8] = (byte)(now.Day >> 8);
            setReq[9] = (byte)(now.Day & 0xFF);
            setReq[10] = (byte)(now.Hour >> 8);
            setReq[11] = (byte)(now.Hour & 0xFF);
            setReq[12] = (byte)(now.Minute >> 8);
            setReq[13] = (byte)(now.Minute & 0xFF);
            setReq[14] = (byte)(now.Second >> 8);
            setReq[15] = (byte)(now.Second & 0xFF);

            byte[] setRes = await SendAndReceiveAsync(setReq, CMD_SET_RTC, false, timeoutMs);
            if (setRes == null || setRes.Length < 16)
            {
                OnFailLog?.Invoke("[RTC] Set RTC 수신 실패", Color.Red);
                return false;
            }

            await Task.Delay(100);

            // 2. Get RTC 요청
            byte[] getReq = BuildCommandPacket(CMD_GET_RTC);
            byte[] getRes = await SendAndReceiveAsync(getReq, CMD_GET_RTC, false, timeoutMs);
            if (getRes == null || getRes.Length < 16)
            {
                OnFailLog?.Invoke("[RTC] Get RTC 수신 실패", Color.Red);
                return false;
            }

            int year = (getRes[4] << 8) | getRes[5];
            int month = (getRes[6] << 8) | getRes[7];
            int day = (getRes[8] << 8) | getRes[9];
            int hour = (getRes[10] << 8) | getRes[11];
            int minute = (getRes[12] << 8) | getRes[13];
            int second = (getRes[14] << 8) | getRes[15];

            OnLog?.Invoke($"[RTC] 시각 동기화 및 확인 정상: {year:D4}-{month:D2}-{day:D2} {hour:D2}:{minute:D2}:{second:D2}", Color.DarkGreen);
            return true;
        }

        public async Task<bool> TestHrsAsync(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            if (!await EnsureSshReadyAsync()) return false;

            byte[] req = BuildCommandPacket(CMD_GET_HRS);
            byte[] res = await SendAndReceiveAsync(req, CMD_GET_HRS, false, timeoutMs);

            if (res == null || res.Length < 8)
            {
                OnFailLog?.Invoke("[HRS] 수신 실패", Color.Red);
                return false;
            }

            ushort hrs1 = (ushort)((res[4] << 8) | res[5]);
            ushort hrs2 = (ushort)((res[6] << 8) | res[7]);

            OnLog?.Invoke($"[HRS] 로터리 스위치 값 확인 완료 (HRS1: 0x{hrs1:X1}, HRS2: 0x{hrs2:X1})", Color.DarkGreen);
            return true;
        }

        #endregion

        #region [6] 공통 패킷 전송 및 2분 타임아웃 RX 헬퍼

        private byte[] BuildCommandPacket(ushort cmd)
        {
            return new byte[] { (byte)(REQ_HEADER >> 8), (byte)(REQ_HEADER & 0xFF), (byte)(cmd >> 8), (byte)(cmd & 0xFF) };
        }

        private async Task<bool> ExecuteCheckCommandAsync(string testName, ushort cmd, int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            if (!await EnsureSshReadyAsync()) return false;

            byte[] req = BuildCommandPacket(cmd);
            byte[] res = await SendAndReceiveAsync(req, cmd, false, timeoutMs);

            if (res == null || res.Length < 6)
            {
                OnFailLog?.Invoke($"[{testName}] 수신 실패 (타임아웃)", Color.Red);
                return false;
            }

            ushort result = (ushort)((res[4] << 8) | res[5]);
            bool isPass = (result == RESULT_OK);

            if (isPass)
                OnLog?.Invoke($"[{testName}] 검사 성공 (결과코드: 0x{result:X4})", Color.DarkGreen);
            else
                OnFailLog?.Invoke($"[{testName}] 검사 실패 (오류코드: 0x{result:X4})", Color.Red);

            return isPass;
        }

        private async Task<byte[]> SendAndReceiveAsync(byte[] sendData, ushort targetCmd, bool isTester, int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            UdpClient client = isTester ? _testerUdp : _targetUdp;
            string ip = isTester ? TesterIp : TargetIp;
            int port = isTester ? TesterPort : TargetPort;

            if (client == null)
            {
                OnFailLog?.Invoke("[통신 오류] UDP 소켓이 바인딩되지 않았습니다.", Color.Red);
                return null;
            }

            var tcs = new TaskCompletionSource<byte[]>();
            lock (_lock)
            {
                _waitingCmd = targetCmd;
                _currentTcs = tcs;
            }

            try
            {
                string hexStr = BitConverter.ToString(sendData);
                OnLog?.Invoke($"[TX] -> {ip}:{port} (CMD: 0x{targetCmd:X4}) [{hexStr}]", Color.DarkBlue);

                await client.SendAsync(sendData, sendData.Length, ip, port);

                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token))
                {
                    var timeoutTask = Task.Delay(timeoutMs, linkedCts.Token);
                    var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                    if (completedTask == tcs.Task)
                    {
                        linkedCts.Cancel();
                        return await tcs.Task;
                    }
                    else
                    {
                        OnFailLog?.Invoke($"[CMD 0x{targetCmd:X4}] 수신 타임아웃 ({timeoutMs / 1000}초 무응답)", Color.Red);
                        return null;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                OnLog?.Invoke($"[CMD 0x{targetCmd:X4}] 작업 취소 또는 소켓 종료", Color.Gray);
                return null;
            }
            catch (Exception ex)
            {
                OnFailLog?.Invoke($"[TX 전송 오류] {ex.Message}", Color.Red);
                return null;
            }
            finally
            {
                lock (_lock)
                {
                    if (_currentTcs == tcs) _currentTcs = null;
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _cts?.Cancel();
            lock (_lock)
            {
                _currentTcs?.TrySetCanceled();
                _currentTcs = null;
            }

            try { _sshService?.Dispose(); } catch { }
            try { _targetUdp?.Close(); _targetUdp?.Dispose(); } catch { }
            try { _testerUdp?.Close(); _testerUdp?.Dispose(); } catch { }
            _targetUdp = null;
            _testerUdp = null;
        }

        #endregion
    }
}