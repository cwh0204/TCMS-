using System;
using System.Drawing; // WinForms 표준 Color 네임스페이스
using System.Threading;
using System.Threading.Tasks;
using TCMSTester.Protocol;

namespace TCMSTester.Services
{
    public class VaioAiResult
    {
        public ushort RawCh1 { get; set; } // 0~10mA  (규격: 0~1000 count -> 0~10.00mA)
        public ushort RawCh2 { get; set; } // 0~20mA  (규격: 0~1000 count -> 0~20.00mA)
        public ushort RawCh3 { get; set; } // 0~10V   (규격: 0~1000 count -> 0~10.00V)
        public ushort RawCh4 { get; set; } // 0~10V   (규격: 0~1000 count -> 0~10.00V)
        public ushort RawCh5 { get; set; } // Mascon FB (규격: 1500 count -> 15.00V)

        // 단위 스케일링 변환 (1000 count 기준)
        public double Ch1_mA => RawCh1 / 100.0;       // 1000 -> 10.00mA
        public double Ch2_mA => RawCh2 / 50.0;        // 1000 -> 20.00mA (RawCh2 * 20.0 / 1000.0)
        public double Ch3_V => RawCh3 / 100.0;        // 1000 -> 10.00V
        public double Ch4_V => RawCh4 / 100.0;        // 1000 -> 10.00V
        public double Ch5_V => RawCh5 / 100.0;        // 1500 -> 15.00V

        /// <summary>
        /// 규격 3.1: Ch5 Mascon Feedback 15V 정상 여부 (기본 오차 ±0.5V 허용)
        /// </summary>
        public bool IsMasconValid(double tolerance = 0.5) => Math.Abs(Ch5_V - 15.0) <= tolerance;
    }

    public class VaioTestService : IDisposable
    {
        private readonly UdpService _udpService;

        public string TargetIp { get; set; }
        public int TargetPort { get; set; }

        private const ushort CMD_VAIO_AI_READ = 0x0201;
        private const ushort CMD_VAIO_AO_WRITE = 0x0202;

        private TaskCompletionSource<ushort[]>? _aiResponseTcs;
        private TaskCompletionSource<ushort[]>? _aoResponseTcs;
        private readonly object _lockObj = new object();
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public Action<string, Color>? OnLog { get; set; }
        public Action<string, Color>? OnFailLog { get; set; }

        public VaioTestService(UdpService udpService, string targetIp = "10.0.1.11", int targetPort = 5060)
        {
            _udpService = udpService;
            TargetIp = targetIp;
            TargetPort = targetPort;

            if (_udpService != null)
            {
                _udpService.PacketReceived += OnPacketReceived;
            }
        }

        private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            if (e == null) return;

            // VCPUT 응답 시 최상위 ACK 비트(0x8000)가 세팅되어 오는 경우(0x8201, 0x8202)도 수신 허용
            ushort baseCmd = (ushort)(e.Command & 0x0FFF);

            if (baseCmd == CMD_VAIO_AI_READ)
            {
                lock (_lockObj)
                {
                    _aiResponseTcs?.TrySetResult(e.Payloads ?? Array.Empty<ushort>());
                }
            }
            else if (baseCmd == CMD_VAIO_AO_WRITE)
            {
                lock (_lockObj)
                {
                    _aoResponseTcs?.TrySetResult(e.Payloads ?? Array.Empty<ushort>());
                }
            }
        }

        /// <summary>
        /// 아날로그 입력(AI) 5채널 읽기 (CMD: 0x0201)
        /// </summary>
        public async Task<VaioAiResult?> ReadAnalogInputsAsync(int timeoutMs = 2000)
        {
            if (_udpService == null || !_udpService.IsRunning)
            {
                OnFailLog?.Invoke("[VAIO] UDP 소켓이 열려있지 않습니다.", Color.Red);
                return null;
            }

            await _sendLock.WaitAsync();
            var tcs = new TaskCompletionSource<ushort[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (_lockObj)
            {
                _aiResponseTcs = tcs;
            }

            try
            {
                // 송신 직전 로그 출력
                OnLog?.Invoke($"[VAIO-TX] AI_READ(0x{CMD_VAIO_AI_READ:X4}) 요청 송신 -> {TargetIp}:{TargetPort}", Color.DarkBlue);

                bool sent = await _udpService.SendCommandAsync(TargetIp, TargetPort, CMD_VAIO_AI_READ, Array.Empty<ushort>());
                if (!sent)
                {
                    OnFailLog?.Invoke("[VAIO] AI_READ 송신 실패", Color.Red);
                    return null;
                }

                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
                if (completedTask != tcs.Task)
                {
                    OnFailLog?.Invoke($"[VAIO] AI_READ 수신 타임아웃 ({timeoutMs}ms 초과) - 응답 없음", Color.OrangeRed);
                    return null;
                }

                // VaioTestService.cs 내부

                ushort[] payloads = await tcs.Task;
                if (payloads == null || payloads.Length < 5)
                {
                    OnFailLog?.Invoke($"[VAIO] AI_READ 응답 데이터 부족 (수신: {payloads?.Length ?? 0} words)", Color.Red);
                    return null;
                }

                // ★ 오프셋 없이 0번부터 순서대로 매핑
                var result = new VaioAiResult
                {
                    RawCh1 = payloads[0], // Word 0: AIN1 (317 -> 3.17mA)
                    RawCh2 = payloads[1], // Word 1: AIN2 (157 -> 3.14mA)
                    RawCh3 = payloads[2], // Word 2: AIN3 (193 -> 1.93V)
                    RawCh4 = payloads[3], // Word 3: AIN4 (298 -> 2.98V)
                    RawCh5 = payloads[4]  // Word 4: PMASCON 15V (1499 -> 14.99V)
                };

                OnLog?.Invoke($"[VAIO-RX] Ch1(AIN1):{result.Ch1_mA:F2}mA | Ch2(AIN2):{result.Ch2_mA:F2}mA | Ch3(AIN3):{result.Ch3_V:F2}V | Ch4(AIN4):{result.Ch4_V:F2}V | Ch5(15V):{result.Ch5_V:F2}V", Color.DarkGreen);
                return result;
            }
            catch (Exception ex)
            {
                OnFailLog?.Invoke($"[VAIO-AI 에러] {ex.Message}", Color.Red);
                return null;
            }
            finally
            {
                lock (_lockObj)
                {
                    if (_aiResponseTcs == tcs) _aiResponseTcs = null;
                }
                _sendLock.Release();
            }
        }

        /// <summary>
        /// 아날로그 출력(AO) 4채널 전압 인가 (CMD: 0x0202 / 0.00 ~ 10.00V)
        /// </summary>
        public async Task<bool> WriteAnalogOutputsAsync(double v1, double v2, double v3, double v4, int timeoutMs = 1500)
        {
            if (_udpService == null || !_udpService.IsRunning)
            {
                OnFailLog?.Invoke("[VAIO] UDP 소켓이 열려있지 않습니다.", Color.Red);
                return false;
            }

            await _sendLock.WaitAsync();

            ushort r1 = (ushort)Math.Round(Math.Max(0.0, Math.Min(10.0, v1)) * 100.0);
            ushort r2 = (ushort)Math.Round(Math.Max(0.0, Math.Min(10.0, v2)) * 100.0);
            ushort r3 = (ushort)Math.Round(Math.Max(0.0, Math.Min(10.0, v3)) * 100.0);
            ushort r4 = (ushort)Math.Round(Math.Max(0.0, Math.Min(10.0, v4)) * 100.0);

            var tcs = new TaskCompletionSource<ushort[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (_lockObj)
            {
                _aoResponseTcs = tcs;
            }

            try
            {
                ushort[] payloads = new ushort[] { r1, r2, r3, r4 };
                OnLog?.Invoke($"[VAIO-TX] AO_WRITE(0x{CMD_VAIO_AO_WRITE:X4}) 송신 -> {v1:F1}V, {v2:F1}V, {v3:F1}V, {v4:F1}V", Color.DarkBlue);

                bool sent = await _udpService.SendCommandAsync(TargetIp, TargetPort, CMD_VAIO_AO_WRITE, payloads);
                if (!sent)
                {
                    OnFailLog?.Invoke("[VAIO] AO_WRITE 송신 실패", Color.Red);
                    return false;
                }

                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
                if (completedTask != tcs.Task)
                {
                    OnFailLog?.Invoke($"[VAIO] AO_WRITE 응답 타임아웃 ({timeoutMs}ms 초과)", Color.OrangeRed);
                    return false;
                }

                ushort[] resp = await tcs.Task;
                bool isSuccess = (resp == null || resp.Length == 0) || (resp.Length >= 1 && (resp[0] == 0 || resp[0] == 1));

                if (isSuccess)
                {
                    OnLog?.Invoke($"[VAIO-AO] 출력 설정 완료 -> Ch1~4: {v1:F2}V, {v2:F2}V, {v3:F2}V, {v4:F2}V", Color.DarkGreen);
                    return true;
                }

                OnFailLog?.Invoke($"[VAIO-AO] 보드 오류 응답: 0x{(resp?.Length > 0 ? resp[0] : 0):X4}", Color.Red);
                return false;
            }
            catch (Exception ex)
            {
                OnFailLog?.Invoke($"[VAIO-AO 에러] {ex.Message}", Color.Red);
                return false;
            }
            finally
            {
                lock (_lockObj)
                {
                    if (_aoResponseTcs == tcs) _aoResponseTcs = null;
                }
                _sendLock.Release();
            }
        }

        public void Dispose()
        {
            if (_udpService != null)
            {
                _udpService.PacketReceived -= OnPacketReceived;
            }
            _sendLock?.Dispose();
        }
    }
}