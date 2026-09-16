using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public class MemTestResult
    {
        public bool IsSuccess { get; set; }
        public string StatusMessage { get; set; }
        public string DetailMessage { get; set; }
        public long ElapsedMs { get; set; }
    }

    public class MemEthTestService
    {
        private readonly UdpService _udpService;
        private static readonly Random _random = new Random();
        private static readonly object _randomLock = new object();

        // VCPUT IP 및 포트 규격
        private const string IP_ENET1 = "10.0.1.11";
        private const string IP_ENET2 = "10.0.2.11";
        private const int PORT_VCPUT = 5060;

        // 커맨드 규격 (tester.h 매핑)
        private const ushort CMD_USB_TEST = 0x0001;
        private const ushort CMD_EMMC_TEST = 0x0002;
        private const ushort CMD_ETHERNET_TEST = 0x0003;
        private const ushort CMD_DPRAM_TEST = 0x0107;
        private const ushort CMD_SDRAM_TEST = 0x010A;
        private const ushort CMD_MRAM_TEST = 0x010B;
        private const ushort CMD_FLASH_TEST = 0x010C;

        private TaskCompletionSource<ushort[]> _waitingTcs;
        private ushort _waitingCmd = 0;
        private readonly object _syncLock = new object();

        public MemEthTestService(UdpService udpService)
        {
            _udpService = udpService ?? throw new ArgumentNullException(nameof(udpService));
            _udpService.PacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            lock (_syncLock)
            {
                if (_waitingTcs != null && e.Command == _waitingCmd)
                {
                    _waitingTcs.TrySetResult(e.Payloads ?? new ushort[0]);
                }
            }
        }

        /// <summary>
        /// 항목별 특성에 맞춘 가변 타임아웃 부여 (C# 7.3 호환)
        /// </summary>
        public async Task<MemTestResult> RunTestAsync(string key)
        {
            switch (key)
            {
                case "DPRAM":
                    // DPRAM은 1~2초 내 완료
                    return await ExecuteMemoryTestAsync(CMD_DPRAM_TEST, "DPRAM", 60000);

                case "MRAM":
                    // MRAM 검사는 약 15초 소요 -> 22초 대기
                    return await ExecuteMemoryTestAsync(CMD_MRAM_TEST, "MRAM", 60000);

                case "FLASH":
                    // FLASH 체크섬 검사는 약 18초 소요 -> 25초 대기
                    return await ExecuteMemoryTestAsync(CMD_FLASH_TEST, "FLASH", 60000);

                case "EMMC":
                    // eMMC 검사는 약 22초 소요 -> 30초 대기
                    return await ExecuteMemoryTestAsync(CMD_EMMC_TEST, "eMMC", 60000);

                case "USB":
                    // USB는 15초 대기 (사전에 USB 메모리 꽂기 필수)
                    return await ExecuteMemoryTestAsync(CMD_USB_TEST, "USB", 60000);

                case "ENET_1":
                    // 앞선 메모리 검사가 끝난 상태면 Echo는 200ms 내 완료
                    return await ExecuteEthernetEchoTestAsync(IP_ENET1, 1);

                case "ENET_2":
                    return await ExecuteEthernetEchoTestAsync(IP_ENET2, 2);

                case "SDRAM":
                    // SDRAM은 약 17.5초 소요 -> 25초 대기
                    return await ExecuteMemoryTestAsync(CMD_SDRAM_TEST, "SDRAM", 60000);

                default:
                    return new MemTestResult
                    {
                        IsSuccess = false,
                        StatusMessage = "미정의 항목",
                        DetailMessage = "지원하지 않는 시험 키입니다."
                    };
            }
        }

        private async Task<MemTestResult> ExecuteMemoryTestAsync(ushort cmd, string memName, int timeoutMs)
        {
            Stopwatch sw = Stopwatch.StartNew();

            lock (_syncLock)
            {
                _waitingCmd = cmd;
                _waitingTcs = new TaskCompletionSource<ushort[]>();
            }

            try
            {
                // 메모리 시험 커맨드 송신 (페이로드 없음)
                await _udpService.SendCommandAsync(IP_ENET1, PORT_VCPUT, cmd, new ushort[0]);

                Task completedTask = await Task.WhenAny(_waitingTcs.Task, Task.Delay(timeoutMs));
                sw.Stop();

                if (completedTask == _waitingTcs.Task)
                {
                    ushort[] payloads = await _waitingTcs.Task;
                    ushort status = (payloads != null && payloads.Length > 0) ? payloads[0] : (ushort)0;

                    if (status == 1)
                    {
                        return new MemTestResult
                        {
                            IsSuccess = true,
                            StatusMessage = "시험 정상 (PASS)",
                            DetailMessage = string.Format("코드: 0x{0:X4} | 정상 회신 (1) [{1}ms]", cmd, sw.ElapsedMilliseconds),
                            ElapsedMs = sw.ElapsedMilliseconds
                        };
                    }
                    else
                    {
                        return new MemTestResult
                        {
                            IsSuccess = false,
                            StatusMessage = string.Format("장치 오류 (코드: {0})", status),
                            DetailMessage = (status == 2) ? "자체 진단 결과 에러(2) 리턴" : string.Format("알 수 없는 에러 코드: {0}", status),
                            ElapsedMs = sw.ElapsedMilliseconds
                        };
                    }
                }
                else
                {
                    return new MemTestResult
                    {
                        IsSuccess = false,
                        StatusMessage = "응답 타임아웃",
                        DetailMessage = string.Format("{0}초 내 VCPUT 회신 없음", timeoutMs / 1000)
                    };
                }
            }
            catch (Exception ex)
            {
                return new MemTestResult
                {
                    IsSuccess = false,
                    StatusMessage = "송수신 예외",
                    DetailMessage = ex.Message
                };
            }
            finally
            {
                lock (_syncLock)
                {
                    _waitingCmd = 0;
                    _waitingTcs = null;
                }
            }
        }

        private async Task<MemTestResult> ExecuteEthernetEchoTestAsync(string targetIp, int ch)
        {
            Stopwatch sw = Stopwatch.StartNew();

            lock (_syncLock)
            {
                _waitingCmd = CMD_ETHERNET_TEST;
                _waitingTcs = new TaskCompletionSource<ushort[]>();
            }

            try
            {
                ushort tag;
                lock (_randomLock)
                {
                    tag = (ushort)_random.Next(0x1000, 0x7FFF);
                }

                ushort[] txPattern = new ushort[] { tag, 0xA5A5, 0x5A5A, (ushort)(tag ^ 0xFFFF) };

                await _udpService.SendCommandAsync(targetIp, PORT_VCPUT, CMD_ETHERNET_TEST, txPattern);

                Task completedTask = await Task.WhenAny(_waitingTcs.Task, Task.Delay(2500));
                sw.Stop();

                if (completedTask == _waitingTcs.Task)
                {
                    ushort[] rxPayloads = await _waitingTcs.Task;

                    bool isMatched = rxPayloads != null &&
                                     rxPayloads.Length == txPattern.Length &&
                                     rxPayloads.SequenceEqual(txPattern);

                    if (isMatched)
                    {
                        return new MemTestResult
                        {
                            IsSuccess = true,
                            StatusMessage = "에코 정상 (PASS)",
                            DetailMessage = string.Format("{0} | 왕복 시간: {1}ms", targetIp, sw.ElapsedMilliseconds),
                            ElapsedMs = sw.ElapsedMilliseconds
                        };
                    }
                    else
                    {
                        return new MemTestResult
                        {
                            IsSuccess = false,
                            StatusMessage = "데이터 불일치",
                            DetailMessage = "수신된 에코 패킷이 송신 데이터와 다름"
                        };
                    }
                }
                else
                {
                    return new MemTestResult
                    {
                        IsSuccess = false,
                        StatusMessage = "무응답 (타임아웃)",
                        DetailMessage = string.Format("{0} 물리 링크/배선 연결 확인 필요", targetIp)
                    };
                }
            }
            catch (Exception ex)
            {
                return new MemTestResult
                {
                    IsSuccess = false,
                    StatusMessage = "송수신 예외",
                    DetailMessage = ex.Message
                };
            }
            finally
            {
                lock (_syncLock)
                {
                    _waitingCmd = 0;
                    _waitingTcs = null;
                }
            }
        }
    }
}