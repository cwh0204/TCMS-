using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCMSTester.Models
{
    public class TcmsPacket
    {
        public DateTime Timestamp { get; set; }
        public byte NodeId { get; set; }
        public byte Command { get; set; }
        public byte Status { get; set; }

        // Raw 바이트 버퍼 (TC 유닛 대응을 위해 Di3Raw 추가)
        public byte[] Di1Raw { get; set; } = new byte[6];
        public byte[] Di2Raw { get; set; } = new byte[6];
        public byte[] Di3Raw { get; set; } = new byte[6]; // TC 유닛용 DI3 추가
        public byte[] DoRaw { get; set; } = new byte[4];
        public byte[] AioRaw { get; set; } = new byte[4];

        /// <summary>
        /// 포트 번호(1부터 시작)와 비트 번호(1=LSB(0x01) ~ 8=MSB(0x80))로 ON/OFF 조회
        /// </summary>
        public bool IsBitOn(byte[] rawBuffer, int portNo, int bitNo)
        {
            if (rawBuffer == null || portNo < 1 || portNo > rawBuffer.Length) return false;
            if (bitNo < 1 || bitNo > 8) return false;

            byte portByte = rawBuffer[portNo - 1];

            // LSB First 수정: bit 1 -> shift 0 (0x01), bit 8 -> shift 7 (0x80)
            int shift = bitNo - 1;
            return ((portByte >> shift) & 0x01) == 1;
        }
        
        /// <summary>
        /// 0부터 시작하는 전체 채널 순차 인덱스로 ON/OFF 조회
        /// </summary>
        public bool IsFlatBitOn(byte[] rawBuffer, int flatIndex)
        {
            int portNo = (flatIndex / 8) + 1;
            int bitNo = (flatIndex % 8) + 1;
            return IsBitOn(rawBuffer, portNo, bitNo);
        }
    }
}