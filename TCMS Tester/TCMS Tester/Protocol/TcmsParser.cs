using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCMSTester.Models;

namespace TCMSTester.Protocol
{
    public static class TcmsParser
    {
        private const int MIN_PACKET_SIZE = 32; // CC 유닛 최소 패킷 크기 (헤더 2바이트 제거 반영)

        public static bool TryParse(byte[] buffer, out TcmsPacket parsedPacket)
        {
            parsedPacket = null;

            // 1. 패킷 최소 길이 검증 (헤더 제거 기준: CC 32바이트, TC 38바이트)
            if (buffer == null || buffer.Length < MIN_PACKET_SIZE) return false;

            // 2. Header 검증 제거 (상위 수신부에서 41A0 2바이트 제거됨)

            // Tail 검증 (마지막 바이트 0xC1 검증)
            int tailIdx = buffer.Length - 1;
            if (buffer[tailIdx] != 0xC1) return false;

            var packet = new TcmsPacket
            {
                NodeId = buffer[7],    // (기존 9 -> 7)
                Command = buffer[8],   // (기존 10 -> 8)
                Status = buffer[9]     // (기존 11 -> 9)
            };

            // 3. RTC 타임스탬프 파싱 (BCD 형태 buffer[1] ~ buffer[6], 기존 3~8에서 2바이트 당김)
            try
            {
                int year = 2000 + Convert.ToInt32(buffer[1].ToString("X2"));
                int month = Convert.ToInt32(buffer[2].ToString("X2"));
                int day = Convert.ToInt32(buffer[3].ToString("X2"));
                int hour = Convert.ToInt32(buffer[4].ToString("X2"));
                int minute = Convert.ToInt32(buffer[5].ToString("X2"));
                int second = Convert.ToInt32(buffer[6].ToString("X2"));
                packet.Timestamp = new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                packet.Timestamp = DateTime.Now;
            }

            // 4. I/O 데이터 슬라이싱 및 Reverse (모든 오프셋 -2바이트 적용)
            // DI1 (6Bytes: 10~15)
            Array.Copy(buffer, 10, packet.Di1Raw, 0, 6);
            Array.Reverse(packet.Di1Raw);

            // DI2 (6Bytes: 16~21)
            Array.Copy(buffer, 16, packet.Di2Raw, 0, 6);
            Array.Reverse(packet.Di2Raw);

            if (buffer.Length >= 38)
            {
                // TC 유닛 (DI3 존재: 38Byte 패킷)
                // DI3 (6Bytes: 22~27)
                Array.Copy(buffer, 22, packet.Di3Raw, 0, 6);
                Array.Reverse(packet.Di3Raw);

                // DO (4Bytes: 28~31)
                Array.Copy(buffer, 28, packet.DoRaw, 0, 4);
                Array.Reverse(packet.DoRaw);

                // AIO (4Bytes: 32~35)
                Array.Copy(buffer, 32, packet.AioRaw, 0, 4);
                Array.Reverse(packet.AioRaw);
            }
            else
            {
                // CC 유닛 (DI3 없음: 32Byte 패킷)
                // DO (4Bytes: 22~25)
                Array.Copy(buffer, 22, packet.DoRaw, 0, 4);
                Array.Reverse(packet.DoRaw);

                // AIO (4Bytes: 26~29)
                Array.Copy(buffer, 26, packet.AioRaw, 0, 4);
                Array.Reverse(packet.AioRaw);
            }

            parsedPacket = packet;
            return true;
        }
    }
}