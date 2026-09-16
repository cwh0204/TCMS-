using System;
using System.Drawing;
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

        public double Ch1_mA => RawCh1 / 100.0;
        public double Ch2_mA => (RawCh2 / 1000.0) * 20.0;
        public double Ch3_V => RawCh3 / 100.0;
        public double Ch4_V => RawCh4 / 100.0;
        public double Ch5_V => RawCh5 / 100.0;

        /// <summary>
        /// 규격 3.1: Ch5 Mascon Feedback 15V 정상 여부 (기본 오차 ±0.5V 허용)
        /// </summary>
        public bool IsMasconValid(double tolerance = 0.5) => Math.Abs(Ch5_V - 15.0) <= tolerance;
    }

    public class VaioTestService : IDisposable
    {
        private readonly UdpService _udpService;

        // 규격서 1장: LAN 1(10.0.1.11)과 LAN 2(10.0.2.11) 절체 지원
        public string TargetIp { get; set; }
        public int TargetPort { get; set; }

        private const ushort CMD_VAIO_AI_READ = 0x0201;
        private const ushort CMD_VAIO_AO_WRITE = 0x0202;

        private TaskCompletionSource<ushort[]> _aiResponseTcs;
        private TaskCompletionSource<ushort[]> _aoResponseTcs;
        private readonly object _lockObj = new object();
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }

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

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            if (e == null) return;

            if (e.Command == CMD_VAIO_AI_READ)
            {
                lock (_lockObj)
                {
                    _aiResponseTcs?.TrySetResult(e.Payloads ?? new ushort[0]);
                }
            }
            else if (e.Command == CMD_VAIO_AO_WRITE)
            {
                lock (_lockObj)
                {
                    _aoResponseTcs?.TrySetResult(e.Payloads ?? new ushort[0]);
                }
            }
        }

        /// <summary>
        /// 아날로그 입력(AI) 5채널 읽기 (CMD: 0x0201)
        /// </summary>
        public async Task<VaioAiResult> ReadAnalogInputsAsync(int timeoutMs = 2000)
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
                bool sent = await _udpService.SendCommandAsync(TargetIp, TargetPort, CMD_VAIO_AI_READ);
                if (!sent)
                {
                    OnFailLog?.Invoke("[VAIO] AI_READ 송신 실패", Color.Red);
                    return null;
                }

                using (var cts = new CancellationTokenSource(timeoutMs))
                using (cts.Token.Register(() => tcs.TrySetResult(null)))
                {
                    ushort[] payloads = await tcs.Task;
                    if (payloads == null || payloads.Length < 5)
                    {
                        OnFailLog?.Invoke("[VAIO] AI_READ 수신 타임아웃 또는 데이터 부족", Color.Red);
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

                    OnLog?.Invoke($"[VAIO-AI] Ch1:{result.Ch1_mA:F2}mA | Ch2:{result.Ch2_mA:F2}mA | Ch3:{result.Ch3_V:F2}V | Ch4:{result.Ch4_V:F2}V | Ch5(Mascon):{result.Ch5_V:F2}V", Color.DarkBlue);
                    return result;
                }
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
                bool sent = await _udpService.SendCommandAsync(TargetIp, TargetPort, CMD_VAIO_AO_WRITE, payloads);
                if (!sent)
                {
                    OnFailLog?.Invoke("[VAIO] AO_WRITE 송신 실패", Color.Red);
                    return false;
                }

                using (var cts = new CancellationTokenSource(timeoutMs))
                using (cts.Token.Register(() => tcs.TrySetResult(null)))
                {
                    ushort[] resp = await tcs.Task;
                    if (resp == null)
                    {
                        OnFailLog?.Invoke("[VAIO] AO_WRITE 응답 타임아웃", Color.OrangeRed);
                        return false;
                    }

                    // 페이로드가 없는 순수 ACK(Length == 0)이거나 결과 코드가 0 또는 1(성공)인 경우 정상 인정
                    bool isSuccess = (resp.Length == 0) || (resp.Length >= 1 && (resp[0] == 0 || resp[0] == 1));

                    if (isSuccess)
                    {
                        OnLog?.Invoke($"[VAIO-AO] 출력 설정 완료 -> Ch1~4: {v1:F2}V, {v2:F2}V, {v3:F2}V, {v4:F2}V", Color.DarkGreen);
                        return true;
                    }

                    OnFailLog?.Invoke($"[VAIO-AO] 보드 오류 응답: 0x{(resp.Length > 0 ? resp[0] : 0):X4}", Color.Red);
                    return false;
                }
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