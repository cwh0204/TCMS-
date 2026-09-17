using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using TCMSTester.Protocol;

namespace TCMSTester.Services
{
    public class DuTestService : IDuTestService
    {
        private readonly UdpService _udpService;

        public string TargetIp { get; set; }
        public int TargetPort { get; set; }

        // 규격서에 정의된 DU 통신 검증 커맨드 예시
        private const ushort CMD_DU_MVB_TEST = 0x0401;
        private const ushort CMD_DU_RS485_TEST = 0x0402;

        private TaskCompletionSource<ushort[]>? _responseTcs;
        private readonly object _lockObj = new object();
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public Action<string, Color>? OnLog { get; set; }
        public Action<string, Color>? OnFailLog { get; set; }

        public DuTestService(UdpService udpService, string targetIp = "10.0.1.21", int targetPort = 5060)
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
            if (baseCmd == CMD_DU_MVB_TEST || baseCmd == CMD_DU_RS485_TEST)
            {
                lock (_lockObj)
                {
                    _responseTcs?.TrySetResult(e.Payloads ?? Array.Empty<ushort>());
                }
            }
        }

        public async Task<bool> TestMvbAsync(int timeoutMs = 2000)
        {
            return await ExecuteCommTestAsync(CMD_DU_MVB_TEST, "MVB", timeoutMs);
        }

        public async Task<bool> TestRs485Async(int timeoutMs = 2000)
        {
            return await ExecuteCommTestAsync(CMD_DU_RS485_TEST, "RS-485", timeoutMs);
        }

        public async Task<DuTestResult> RunDuCommTestAsync(int timeoutMs = 3000)
        {
            bool mvb = await TestMvbAsync(timeoutMs);
            bool rs485 = await TestRs485Async(timeoutMs);

            return new DuTestResult
            {
                IsMvbPass = mvb,
                IsRs485Pass = rs485,
                MvbStatusMessage = mvb ? "합격" : "응답 없음",
                Rs485StatusMessage = rs485 ? "합격" : "수신 실패"
            };
        }

        private async Task<bool> ExecuteCommTestAsync(ushort cmd, string commName, int timeoutMs)
        {
            if (_udpService == null || !_udpService.IsRunning)
            {
                OnFailLog?.Invoke($"[DU-{commName}] UDP 통신 소켓이 비활성화 상태입니다.", Color.Red);
                return false;
            }

            await _sendLock.WaitAsync();
            var tcs = new TaskCompletionSource<ushort[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (_lockObj)
            {
                _responseTcs = tcs;
            }

            try
            {
                OnLog?.Invoke($"[DU-TX] {commName} 시험 요청 송신 (CMD: 0x{cmd:X4}) -> {TargetIp}:{TargetPort}", Color.DarkBlue);

                bool sent = await _udpService.SendCommandAsync(TargetIp, TargetPort, cmd, Array.Empty<ushort>());
                if (!sent)
                {
                    OnFailLog?.Invoke($"[DU-{commName}] 패킷 송신 실패", Color.Red);
                    return false;
                }

                var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
                if (completed != tcs.Task)
                {
                    OnFailLog?.Invoke($"[DU-{commName}] 수신 타임아웃 ({timeoutMs}ms 초과)", Color.OrangeRed);
                    return false;
                }

                ushort[] resp = await tcs.Task;
                bool isSuccess = resp != null && resp.Length > 0 && resp[0] == 0; // 0x0000: 정상 수신 ACK

                if (isSuccess)
                {
                    OnLog?.Invoke($"[DU-RX] {commName} 통신 정상 응답 확인 (합격)", Color.DarkGreen);
                    return true;
                }

                OnFailLog?.Invoke($"[DU-RX] {commName} 오류 응답 (ERR Code: 0x{(resp?.Length > 0 ? resp[0] : 0):X4})", Color.Red);
                return false;
            }
            catch (Exception ex)
            {
                OnFailLog?.Invoke($"[DU-{commName} 예외] {ex.Message}", Color.Red);
                return false;
            }
            finally
            {
                lock (_lockObj)
                {
                    if (_responseTcs == tcs) _responseTcs = null;
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