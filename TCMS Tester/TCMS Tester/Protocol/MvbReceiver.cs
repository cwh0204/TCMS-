using System;
using System.Text;
using System.Text.RegularExpressions;
using TCMSTester.Models;

namespace TCMSTester.Protocol
{
    public class MvbPacketEventArgs : EventArgs
    {
        public TcmsPacket Packet { get; }
        public byte[] RawData { get; }
        public MvbPacketEventArgs(TcmsPacket packet, byte[] rawData)
        {
            Packet = packet;
            RawData = rawData;
        }
    }

    /// <summary>
    /// 실제 수신 스트림은 순수 바이너리가 아니라 ASCII 텍스트 프로토콜입니다.
    /// 예) "41A01826083118264011310100002000000000000001000080000000000000C1receiveddata 41A0..."
    /// </summary>
    public class MvbReceiver : IDisposable
    {
        public const int PACKET_SIZE_TC = 40;
        public const int PACKET_SIZE_DEFAULT = 34; // CC 유닛 기본 크기 34바이트로 변경

        // 기존 {68} 고정에서 {64,80} 범위로 수정 (32바이트~40바이트 대응)
        private static readonly Regex FrameRegex = new Regex(@"(?<hex>[0-9A-Fa-f]{64,80})", RegexOptions.Compiled);

        private const int MAX_BUFFER_CHARS = 20000;

        private readonly StringBuilder _textBuffer = new StringBuilder();
        private readonly object _lockObj = new object();
        private byte[] _latestRawData;

        public int FrameSize { get; set; } = PACKET_SIZE_DEFAULT;

        public event EventHandler<MvbPacketEventArgs> PacketReceived;
        public event EventHandler<string> ErrorOccurred;

        public bool IsRunning { get; private set; }

        /// <summary>
        /// 가장 최근에 파싱 성공한 실시간 Raw 바이트 배열을 스레드 안전하게 반환합니다.
        /// </summary>
        public byte[] GetLatestData()
        {
            lock (_lockObj)
            {
                if (_latestRawData == null) return null;
                return (byte[])_latestRawData.Clone();
            }
        }

        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;
            lock (_lockObj)
            {
                _textBuffer.Clear();
                _latestRawData = null;
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void PushRawData(byte[] data, int length)
        {
            if (!IsRunning) return;
            if (data == null || length <= 0) return;

            string chunk = Encoding.ASCII.GetString(data, 0, length);

            lock (_lockObj)
            {
                _textBuffer.Append(chunk);

                if (_textBuffer.Length > MAX_BUFFER_CHARS)
                {
                    int trimCount = _textBuffer.Length - MAX_BUFFER_CHARS;
                    _textBuffer.Remove(0, trimCount);
                    ErrorOccurred?.Invoke(this, $"[MVB 텍스트 버퍼] 프레임 구분자를 찾지 못해 버퍼가 과도하게 커져 앞부분 {trimCount}자를 폐기했습니다.");
                }

                ExtractCompleteFrames();
            }
        }

        private void ExtractCompleteFrames()
        {
            string bufferContent = _textBuffer.ToString();
            int lastConsumedIndex = 0;

            foreach (Match m in FrameRegex.Matches(bufferContent))
            {
                string hex = m.Groups["hex"].Value;
                lastConsumedIndex = m.Index + m.Length;

                // [수정] 41A0 헤더 검사 및 제거 로직 완전 삭제
                // Regex를 통해 추출된 Hex 문자열의 길이 유효성만 검사합니다.

                if (hex.Length % 2 != 0)
                {
                    ErrorOccurred?.Invoke(this, $"[MVB 텍스트 파싱] hex 문자열 길이가 홀수({hex.Length})라 프레임을 버립니다: {hex}");
                    continue;
                }

                byte[] frameBytes;
                try
                {
                    frameBytes = HexStringToBytes(hex);
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"[MVB 텍스트 파싱] hex 디코딩 실패: {ex.Message}");
                    continue;
                }

                // TcmsParser 호출 (TcmsParser 내부 오프셋이 Byte 7부터 IO를 읽도록 되어있는지 확인 필요)
                if (TcmsParser.TryParse(frameBytes, out TcmsPacket packet))
                {
                    // 최신 Raw 데이터 스레드 안전하게 저장
                    _latestRawData = (byte[])frameBytes.Clone();

                    PacketReceived?.Invoke(this, new MvbPacketEventArgs(packet, frameBytes));
                }
                else
                {
                    ErrorOccurred?.Invoke(this, $"MVB 패킷 검증(Tail/RTC) 실패 (길이={frameBytes.Length}바이트, HEX={hex})");
                }
            }

            if (lastConsumedIndex > 0)
            {
                _textBuffer.Remove(0, lastConsumedIndex);
            }
        }

        private static byte[] HexStringToBytes(string hex)
        {
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}