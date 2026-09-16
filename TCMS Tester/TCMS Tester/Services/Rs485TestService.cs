using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TCMSTester.Protocol;

namespace TCMSTester.Services
{
    #region 결과 데이터 모델

    public class Rs485ChannelResult
    {
        public int ChannelIndex { get; set; }
        public string PortName { get; set; }
        public ushort SuccessCount { get; set; }
        public ushort FailCount { get; set; }
        public bool IsPass => (SuccessCount >= 10 && FailCount == 0);

        public Rs485ChannelResult()
        {
            PortName = string.Empty;
        }
    }

    public class Rs485TestResult
    {
        public bool TotalPass { get; set; }
        public List<Rs485ChannelResult> Channels { get; }
        public string Summary { get; set; }
        public ushort[] RawPayloads { get; set; }

        public Rs485TestResult()
        {
            Channels = new List<Rs485ChannelResult>();
            Summary = string.Empty;
            RawPayloads = new ushort[0];
        }
    }

    #endregion

    /// <summary>
    /// VCPUT RS485 3채널 루프백 시험 전용 독립 서비스 (C# 7.3 호환 / 포트 재사용 구조)
    /// </summary>
    public class Rs485TestService : IDisposable
    {
        private readonly UdpService _udpService;
        private readonly string _targetIp;
        private readonly int _targetPort;
        private const ushort CMD_RS485_TEST = 0x0103;

        private TaskCompletionSource<ushort[]> _rs485ResponseTcs;
        private readonly object _udpLock = new object();

        private class SerialSession
        {
            public SerialPort Port { get; set; }
            public List<byte> Buffer { get; }
            public object Lock { get; }
            public int ChannelIndex { get; set; }
            public int EchoCount { get; set; }

            public SerialSession()
            {
                Buffer = new List<byte>();
                Lock = new object();
                ChannelIndex = 0;
                EchoCount = 0;
            }
        }

        private readonly Dictionary<string, SerialSession> _sessions = new Dictionary<string, SerialSession>(StringComparer.OrdinalIgnoreCase);

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }

        public Rs485TestService(UdpService udpService, string targetIp = "10.0.1.11", int targetPort = 5060)
        {
            _udpService = udpService;
            _targetIp = targetIp;
            _targetPort = targetPort;

            if (_udpService != null)
            {
                _udpService.PacketReceived += OnUdpPacketReceived;
            }
        }

        private void OnUdpPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            if (e == null) return;

            if (e.Command == CMD_RS485_TEST)
            {
                lock (_udpLock)
                {
                    if (_rs485ResponseTcs != null)
                    {
                        _rs485ResponseTcs.TrySetResult(e.Payloads ?? new ushort[0]);
                    }
                }
            }
        }

        /// <summary>
        /// RS485 3채널 루프백 시험을 실행합니다. (포트 재사용 방식)
        /// </summary>
        public async Task<Rs485TestResult> ExecuteTestAsync(
            string[] targetPorts = null,
            int timeoutMs = 4000,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new Rs485TestResult();
            string[] ports = (targetPorts != null && targetPorts.Length > 0)
                ? targetPorts
                : new[] { "COM49", "COM50", "COM52" };

            // 1. 포트 준비 (이미 열려 있으면 버퍼 리셋, 닫혀 있을 때만 오픈)
            if (!EnsurePortsReady(ports))
            {
                result.TotalPass = false;
                result.Summary = "포트 준비 실패 (오픈 불가)";
                return result;
            }

            var tcs = new TaskCompletionSource<ushort[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_udpLock)
            {
                if (_rs485ResponseTcs != null)
                {
                    _rs485ResponseTcs.TrySetResult(new ushort[0]);
                }
                _rs485ResponseTcs = tcs;
            }

            try
            {
                OnLog?.Invoke("================================================================================", Color.Purple);
                OnLog?.Invoke($"[RS485] 루프백 시험 시작 (CMD: 0x{CMD_RS485_TEST:X4}) | 대상: {string.Join(", ", ports)}", Color.Purple);

                // 2. UDP 트리거 송출
                bool sent = await _udpService.SendCommandAsync(_targetIp, _targetPort, CMD_RS485_TEST);
                if (!sent)
                {
                    result.TotalPass = false;
                    result.Summary = "UDP 트리거 송신 실패";
                    OnFailLog?.Invoke("[RS485] UDP 명령 전송 실패", Color.DarkRed);
                    return result;
                }

                // 3. VCPUT 결과 응답 대기
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    cts.CancelAfter(timeoutMs);
                    using (cts.Token.Register(() => tcs.TrySetResult(null)))
                    {
                        ushort[] payloads = await tcs.Task;
                        result.RawPayloads = payloads ?? new ushort[0];

                        if (payloads == null)
                        {
                            result.TotalPass = false;
                            result.Summary = "응답 시간 초과 (VCPUT 타임아웃)";
                            OnFailLog?.Invoke($"[RS485] {timeoutMs}ms 이내에 응답 패킷이 도착하지 않았습니다.", Color.DarkRed);
                            return result;
                        }

                        // 4. 결과 분석 (CH1~CH3)
                        if (payloads.Length >= 6)
                        {
                            ushort ch1Succ = payloads[0], ch1Fail = payloads[1];
                            ushort ch2Succ = payloads[2], ch2Fail = payloads[3];
                            ushort ch3Succ = payloads[4], ch3Fail = payloads[5];

                            result.Channels.Add(new Rs485ChannelResult { ChannelIndex = 1, PortName = ports[0], SuccessCount = ch1Succ, FailCount = ch1Fail });
                            result.Channels.Add(new Rs485ChannelResult { ChannelIndex = 2, PortName = ports[1], SuccessCount = ch2Succ, FailCount = ch2Fail });
                            result.Channels.Add(new Rs485ChannelResult { ChannelIndex = 3, PortName = ports[2], SuccessCount = ch3Succ, FailCount = ch3Fail });

                            foreach (var ch in result.Channels)
                            {
                                OnLog?.Invoke($"[CH{ch.ChannelIndex} ({ch.PortName})] 성공: {ch.SuccessCount}회 / 실패: {ch.FailCount}회", ch.IsPass ? Color.Green : Color.Red);
                            }

                            result.TotalPass = result.Channels.All(c => c.IsPass);
                            result.Summary = $"CH1:{ch1Succ}/{ch1Fail} | CH2:{ch2Succ}/{ch2Fail} | CH3:{ch3Succ}/{ch3Fail}";

                            if (result.TotalPass)
                                OnLog?.Invoke($" [RS485 판정] ALL PASS (3채널 10/10 통과) -> [{result.Summary}]", Color.Green);
                            else
                                OnFailLog?.Invoke($" [RS485 판정] FAIL -> [{result.Summary}]", Color.DarkRed);
                        }
                        else
                        {
                            result.TotalPass = false;
                            result.Summary = $"페이로드 부족 ({payloads.Length} words)";
                            OnFailLog?.Invoke("[RS485] 수신 페이로드 길이 부족", Color.DarkRed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.TotalPass = false;
                result.Summary = ex.Message;
                OnFailLog?.Invoke($"[RS485 에러] {ex.Message}", Color.Red);
            }
            finally
            {
                // 회차가 끝날 때 포트를 닫지 않고 유지하여 재사용
                lock (_udpLock)
                {
                    if (_rs485ResponseTcs == tcs) _rs485ResponseTcs = null;
                }
                OnLog?.Invoke("================================================================================", Color.Purple);
            }

            return result;
        }

        /// <summary>
        /// 포트 연결 상태를 점검하고, 이미 열려 있으면 버퍼만 청소하고 닫혀 있을 때만 오픈합니다.
        /// </summary>
        private bool EnsurePortsReady(string[] ports)
        {
            int readyCount = 0;

            for (int i = 0; i < ports.Length; i++)
            {
                string cleanPort = ports[i].Trim().ToUpper();

                try
                {
                    SerialSession session;
                    if (_sessions.TryGetValue(cleanPort, out session) && session.Port != null && session.Port.IsOpen)
                    {
                        // 이미 열려 있는 포트는 버퍼와 에코 카운트만 리셋
                        lock (session.Lock)
                        {
                            session.Buffer.Clear();
                            session.EchoCount = 0;
                            session.ChannelIndex = i;
                            try { session.Port.DiscardInBuffer(); } catch { }
                            try { session.Port.DiscardOutBuffer(); } catch { }
                        }
                        readyCount++;
                    }
                    else
                    {
                        // 포트가 없거나 닫혀 있을 때만 새로 생성 및 오픈
                        if (session != null && session.Port != null)
                        {
                            try
                            {
                                session.Port.DataReceived -= SerialPort_DataReceived;
                                if (session.Port.IsOpen) session.Port.Close();
                                session.Port.Dispose();
                            }
                            catch { }
                            _sessions.Remove(cleanPort);
                        }

                        var sp = new SerialPort(cleanPort, 38400, Parity.None, 8, StopBits.One)
                        {
                            ReadTimeout = 500,
                            WriteTimeout = 500,
                            Handshake = Handshake.None,
                            DtrEnable = false,
                            RtsEnable = false
                        };

                        var newSession = new SerialSession { Port = sp, ChannelIndex = i };
                        _sessions[cleanPort] = newSession;

                        sp.DataReceived += SerialPort_DataReceived;
                        sp.Open();
                        readyCount++;

                        OnLog?.Invoke($"[RS485] {cleanPort} (CH{i + 1}) 포트 오픈 완료", Color.DarkGreen);
                    }
                }
                catch (Exception ex)
                {
                    _sessions.Remove(cleanPort);
                    OnFailLog?.Invoke($"[RS485] {cleanPort} 준비 실패: {ex.Message}", Color.Red);
                }
            }

            return readyCount == ports.Length;
        }

        /// <summary>
        /// 전체 시험이 완전히 종료되었을 때 포트를 닫습니다.
        /// </summary>
        public void ClosePorts()
        {
            foreach (var session in _sessions.Values)
            {
                try
                {
                    session.Port.DataReceived -= SerialPort_DataReceived;
                    if (session.Port.IsOpen) session.Port.Close();
                    session.Port.Dispose();
                }
                catch { }
            }
            _sessions.Clear();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = sender as SerialPort;
            if (sp == null || !sp.IsOpen) return;

            SerialSession session;
            if (!_sessions.TryGetValue(sp.PortName, out session)) return;

            lock (session.Lock)
            {
                try
                {
                    int bytesToRead = sp.BytesToRead;
                    if (bytesToRead <= 0) return;

                    byte[] temp = new byte[bytesToRead];
                    sp.Read(temp, 0, bytesToRead);
                    session.Buffer.AddRange(temp);

                    var buffer = session.Buffer;

                    while (true)
                    {
                        // 1. 헤더 [0xBA, 0xB0] 동기화
                        while (buffer.Count >= 2)
                        {
                            if (buffer[0] == 0xBA && buffer[1] == 0xB0) break;
                            buffer.RemoveAt(0);
                        }

                        if (buffer.Count < 256) break;

                        // 2. 256B 패킷 인출
                        byte[] rxPacket = buffer.GetRange(0, 256).ToArray();
                        buffer.RemoveRange(0, 256);

                        session.EchoCount++;

                        // 3. 0xB0 -> 0xB1 치환
                        byte[] echoPacket = (byte[])rxPacket.Clone();
                        echoPacket[1] = 0xB1;

                        // 4. 안전 시차 딜레이 (CH1:12ms, CH2:15ms, CH3:18ms)
                        int delayMs = 12 + (session.ChannelIndex * 3);
                        Thread.Sleep(delayMs);

                        // 5. 회신
                        sp.Write(echoPacket, 0, 256);
                        sp.BaseStream.Flush();
                    }
                }
                catch { }
            }
        }

        public void Dispose()
        {
            ClosePorts();
            if (_udpService != null)
            {
                _udpService.PacketReceived -= OnUdpPacketReceived;
            }
        }
    }
}