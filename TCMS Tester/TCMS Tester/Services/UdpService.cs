using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public byte[] RawBytes { get; }
        public ushort Header { get; }
        public ushort Command { get; }
        public ushort[] Payloads { get; }

        public PacketReceivedEventArgs(byte[] rawBytes, ushort header, ushort command, ushort[] payloads)
        {
            RawBytes = rawBytes;
            Header = header;
            Command = command;
            Payloads = payloads;
        }
    }

    public class UdpService : IDisposable
    {
        private UdpClient _udpClient;
        private bool _isRunning;

        public const ushort PACKET_HEADER = 0xA153;

        public event EventHandler<string> LogMessage;
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;
        public event EventHandler<bool> StateChanged;

        public bool IsRunning => _isRunning;

        public void Start(int localPort = 0)
        {
            if (_isRunning) return;

            try
            {
                _udpClient = new UdpClient(localPort);
                _isRunning = true;

                StateChanged?.Invoke(this, true);
                LogMessage?.Invoke(this, $"[SYS] UDP 소켓 오픈 완료 (Local Port: {((IPEndPoint)_udpClient.Client.LocalEndPoint).Port})");

                Task.Run((Func<Task>)ReceiveLoopAsync);
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"[ERR] 소켓 오픈 실패: {ex.Message}");
                Stop();
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            try
            {
                _udpClient?.Close();
            }
            catch { }
            _udpClient = null;

            StateChanged?.Invoke(this, false);
            LogMessage?.Invoke(this, "[SYS] UDP 통신이 종료되었습니다.");
        }

        public async Task<bool> SendCommandAsync(string targetIp, int targetPort, ushort cmd, params ushort[] payloads)
        {
            if (_udpClient == null || !_isRunning)
            {
                LogMessage?.Invoke(this, "[ERR] 통신 소켓이 열려있지 않습니다.");
                return false;
            }

            try
            {
                byte[] packet;
                using (var ms = new MemoryStream())
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(PACKET_HEADER);
                    writer.Write(cmd);

                    if (payloads != null)
                    {
                        foreach (var val in payloads)
                        {
                            writer.Write(val);
                        }
                    }
                    packet = ms.ToArray();
                }

                await _udpClient.SendAsync(packet, packet.Length, targetIp, targetPort);
                LogMessage?.Invoke(this, $"[TX] -> {targetIp}:{targetPort} (CMD: 0x{cmd:X4}) [{BitConverter.ToString(packet)}]");
                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"[ERR] 송신 실패: {ex.Message}");
                return false;
            }
        }

        private async Task ReceiveLoopAsync()
        {
            while (_isRunning && _udpClient != null)
            {
                try
                {
                    // .NET Framework 호환: 인자 없는 ReceiveAsync 호출
                    var result = await _udpClient.ReceiveAsync();
                    byte[] data = result.Buffer;

                    if (data.Length < 4) continue;

                    ushort header = BitConverter.ToUInt16(data, 0);
                    ushort cmd = BitConverter.ToUInt16(data, 2);

                    if (header != PACKET_HEADER) continue;

                    int payloadCount = (data.Length - 4) / 2;
                    ushort[] payloads = new ushort[payloadCount];
                    for (int i = 0; i < payloadCount; i++)
                    {
                        payloads[i] = BitConverter.ToUInt16(data, 4 + (i * 2));
                    }

                    LogMessage?.Invoke(this, $"[RX] <- {result.RemoteEndPoint} (CMD: 0x{cmd:X4}) [{BitConverter.ToString(data)}]");
                    PacketReceived?.Invoke(this, new PacketReceivedEventArgs(data, header, cmd, payloads));
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        LogMessage?.Invoke(this, $"[ERR] 수신 에러: {ex.Message}");
                    }
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}