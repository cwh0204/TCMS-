using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using TCMSTester.Protocol;

namespace TCMSTester.Services
{
    public class VaioTestService : IVaioTestService
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

        // ★ IVaioTestService 인터페이스 구현: AO 4채널 인가 전압 보관
        public double[] LastAoVoltages { get; private set; } = new double[] { 0.0, 0.0, 0.0, 0.0 };

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

                ushort[] payloads = await tcs.Task;
                if (payloads == null || payloads.Length < 5)
                {
                    OnFailLog?.Invoke($"[VAIO] AI_READ 응답 데이터 부족 (수신: {payloads?.Length ?? 0} words)", Color.Red);
                    return null;
                }

                var result = new VaioAiResult
                {
                    RawCh1 = payloads[0],
                    RawCh2 = payloads[1],
                    RawCh3 = payloads[2],
                    RawCh4 = payloads[3],
                    RawCh5 = payloads[4]
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
                    // ★ 보드가 정상 ACK를 반환하면 설정 전압을 실측 배열에 반영
                    LastAoVoltages = new double[] { v1, v2, v3, v4 };

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