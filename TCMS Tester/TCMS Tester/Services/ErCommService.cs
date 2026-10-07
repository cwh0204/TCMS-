using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    /// <summary>
    /// ER 제어기 저수준 UDP 통신 서비스 (포트: Target 57721, Tester 57888)
    /// </summary>
    public class ErCommService : IDisposable
    {
        public const ushort REQ_HEADER = 0x10A5;
        public const ushort RES_HEADER = 0x205A;
        public const ushort RESULT_OK = 0x0001;
        public const ushort RESULT_NG = 0x0002;

        // 기본 응답 대기 타임아웃: 2분 (120,000ms)
        public const int DEFAULT_TIMEOUT_MS = 120000;

        private UdpClient _targetUdp;
        private UdpClient _testerUdp;
        private CancellationTokenSource _cts;

        private readonly object _lock = new object();
        private TaskCompletionSource<byte[]> _currentTcs;
        private ushort _waitingCmd;

        public Action<string> OnLog { get; set; }

        /// <summary>
        /// UDP 송수신 소켓 초기화 및 수신 루프 기동
        /// </summary>
        public void InitSockets(int targetLocalPort = 57721, int testerLocalPort = 57888)
        {
            CloseSockets();
            _cts = new CancellationTokenSource();

            try
            {
                const int SIO_UDP_CONNRESET = -1744830452;

                // 1. Target 수신 소켓 (57721)
                _targetUdp = new UdpClient();
                _targetUdp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                try { _targetUdp.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null); } catch { }
                _targetUdp.Client.Bind(new IPEndPoint(IPAddress.Any, targetLocalPort));
                StartReceiverLoop(_targetUdp, "ER Target (57721)");

                // 2. Tester 수신 소켓 (57888)
                _testerUdp = new UdpClient();
                _testerUdp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                try { _testerUdp.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null); } catch { }
                _testerUdp.Client.Bind(new IPEndPoint(IPAddress.Any, testerLocalPort));
                StartReceiverLoop(_testerUdp, "Tester (57888)");

                OnLog?.Invoke($"[SYS] UDP 소켓 오픈 완료 (Target: {targetLocalPort}, Tester: {testerLocalPort})");
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"[SYS 에러] UDP 소켓 초기화 실패: {ex.Message}");
            }
        }

        private void StartReceiverLoop(UdpClient client, string name)
        {
            Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested && client != null)
                {
                    try
                    {
                        var res = await client.ReceiveAsync();
                        byte[] data = res.Buffer;

                        if (data != null && data.Length >= 4)
                        {
                            ushort header = (ushort)((data[0] << 8) | data[1]);
                            ushort cmd = (ushort)((data[2] << 8) | data[3]);
                            string hex = BitConverter.ToString(data);

                            OnLog?.Invoke($"[RX] <- {res.RemoteEndPoint} (CMD: 0x{cmd:X4}) [{hex}]");

                            if (header == RES_HEADER)
                            {
                                lock (_lock)
                                {
                                    if (_currentTcs != null && !_currentTcs.Task.IsCompleted && _waitingCmd == cmd)
                                    {
                                        _currentTcs.TrySetResult(data);
                                    }
                                }
                            }
                        }
                    }
                    catch (ObjectDisposedException) { break; }
                    catch (Exception ex)
                    {
                        if (_cts.IsCancellationRequested) break;
                        OnLog?.Invoke($"[{name} RX 에러] {ex.Message}");
                        await Task.Delay(50);
                    }
                }
            });
        }

        /// <summary>
        /// 기본 4바이트 커맨드 패킷 빌드 (Big-Endian)
        /// </summary>
        public byte[] BuildCommandPacket(ushort cmd)
        {
            return new byte[] {
                (byte)(REQ_HEADER >> 8), (byte)(REQ_HEADER & 0xFF),
                (byte)(cmd >> 8),        (byte)(cmd & 0xFF)
            };
        }

        /// <summary>
        /// UDP 송신 후 지정 시간(기본 2분) 동안 응답 대기
        /// </summary>
        public async Task<byte[]> SendAndReceiveAsync(byte[] reqData, ushort cmd, string ip, int port, bool isTester = false, int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            UdpClient client = isTester ? _testerUdp : _targetUdp;
            if (client == null)
            {
                OnLog?.Invoke("[에러] 해당 소켓이 열려있지 않습니다.");
                return null;
            }

            var tcs = new TaskCompletionSource<byte[]>();
            lock (_lock)
            {
                _waitingCmd = cmd;
                _currentTcs = tcs;
            }

            try
            {
                string hex = BitConverter.ToString(reqData);
                OnLog?.Invoke($"[TX] -> {ip}:{port} (CMD: 0x{cmd:X4}) [{hex}]");
                await client.SendAsync(reqData, reqData.Length, ip, port);

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
                        OnLog?.Invoke($"[CMD 0x{cmd:X4}] ❌ 수신 타임아웃 ({timeoutMs / 1000}초 경과, 무응답)");
                        return null;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                OnLog?.Invoke($"[CMD 0x{cmd:X4}] 작업 취소");
                return null;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"[송수신 에러] {ex.Message}");
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

        public void CloseSockets()
        {
            _cts?.Cancel();
            lock (_lock)
            {
                _currentTcs?.TrySetCanceled();
                _currentTcs = null;
            }
            try { _targetUdp?.Close(); _targetUdp?.Dispose(); } catch { }
            try { _testerUdp?.Close(); _testerUdp?.Dispose(); } catch { }
            _targetUdp = null;
            _testerUdp = null;
        }

        public void Dispose() => CloseSockets();
    }
}