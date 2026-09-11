using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace CITester
{
    public class classCnet
    {
        public SerialPort serialPort = null;
        public bool[] m_bDOValue = new bool[1024];

        public classCnet(SerialPort spHandle)
        {
            serialPort = spHandle;
        }

        private Byte GetChecksum(Byte[] btDatBuf, int nDataLen)
        {
            int i;
            Byte btChecksum;

            i = 0;
            btChecksum = 0x00;
            while (nDataLen-- != 0) // 원본 체크섬 계산 루프
            {
                btChecksum += btDatBuf[i++];
            }

            return btChecksum;
        }

        public int GetRxBuf()
        {
            int nRxLen = 0;
            Byte[] btRxBuf = new byte[256];
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    nRxLen = serialPort.Read(btRxBuf, 0, btRxBuf.Length);
                }
                catch { }
            }

            return nRxLen;
        }

        public void Request(char[] szBuf)
        {
            int nTxPos;
            Byte[] btTxBuf = new byte[128];
            Byte btBcc;

            nTxPos = 0;

            // ENQ
            btTxBuf[nTxPos++] = 0x05;

            // String
            for (int i = 0; i < szBuf.Length; i++) btTxBuf[nTxPos++] = (Byte)szBuf[i];

            // EOT
            btTxBuf[nTxPos++] = 0x04;

            // Checksum
            if (btTxBuf[3] == 'r' || btTxBuf[3] == 'w')
            {
                btBcc = GetChecksum(btTxBuf, nTxPos);
                btTxBuf[nTxPos++] = (Byte)string.Format("{0:X1}", (btBcc >> 4) & 0xF).ToCharArray()[0];
                btTxBuf[nTxPos++] = (Byte)string.Format("{0:X1}", btBcc & 0xF).ToCharArray()[0];
            }

            try
            {
                if (serialPort != null) serialPort.Write(btTxBuf, 0, nTxPos);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 핀 번호와 ON/OFF 상태를 받아 워드(%PW) 단위 패킷을 조합한 뒤 즉시 PLC로 전송합니다.
        /// (bOn이 false이면 해당 핀 1개만 OFF됩니다)
        /// </summary>
        public void SetDo(int pinNo, bool bOn, int startWord = 5, int stationNo = 0)
        {
            if (m_bDOValue == null) m_bDOValue = new bool[1024];

            // 1. 해당 핀 상태 업데이트 (false면 0으로 꺼짐)
            m_bDOValue[pinNo] = bOn;

            // 2. 워드 위치 및 해당 워드의 시작 핀 번호 계산
            int wordOffset = pinNo / 16;
            int wordIndex = startWord + wordOffset;
            int basePin = wordOffset * 16;

            // 3. 해당 워드(16개 핀) 상태를 조합하여 16비트 워드 데이터 구성
            ushort wordVal = 0x0000;
            for (int i = 0; i < 16; i++)
            {
                if (m_bDOValue[basePin + i])
                {
                    wordVal |= (ushort)(1 << i);
                }
            }

            string strBitVal = wordVal.ToString("X4");
            string strDevice = string.Format("%PW{0:D3}", wordIndex);

            // 4. 개별 쓰기(wSS) 패킷 생성 (기존 정상 패킷 규격 유지)
            string strReqPacket = string.Format("{0:X2}{1}{2}{3}{4:D2}{5}{6}",
                stationNo,          // 국번 (00)
                "w",                // 쓰기
                "SS",               // 개별 쓰기
                "01",               // 블록 수 1개
                strDevice.Length,   // 디바이스 길이 (06)
                strDevice,          // 디바이스명 (%PW005)
                strBitVal           // 16진수 4자리 데이터
            );

            // 5. 전송
            Request(strReqPacket.ToCharArray());

            // 6. 시리얼 하드웨어 안정화 딜레이
            Thread.Sleep(50);
        }

        /// <summary>
        /// 전체 DO 출력을 모두 0(OFF)으로 초기화하고 PLC로 전송합니다.
        /// </summary>
        public void SetAllOff(int totalPins = 256, int startWord = 5, int stationNo = 0)
        {
            if (m_bDOValue == null) m_bDOValue = new bool[1024];
            Array.Clear(m_bDOValue, 0, m_bDOValue.Length);

            int wordCount = totalPins / 16;

            for (int i = 0; i < wordCount; i++)
            {
                int wordIndex = startWord + i;
                string strDevice = string.Format("%PW{0:D3}", wordIndex);

                string strReqPacket = string.Format("{0:X2}{1}{2}{3}{4:D2}{5}{6}",
                    stationNo,
                    "w",
                    "SS",
                    "01",
                    strDevice.Length,
                    strDevice,
                    "0000"
                );

                Request(strReqPacket.ToCharArray());
                Thread.Sleep(30);
            }
        }

        public int Answer(char[] szAddr, char chCmd, char[] szCmdType, out ushort[] wReturnVal)
        {
            int nRxLen = 0;
            bool bCheckSumOk;
            Byte[] btRxBuf = new byte[256];
            wReturnVal = new ushort[16];

            try
            {
                if (serialPort != null && serialPort.IsOpen)
                    nRxLen = serialPort.Read(btRxBuf, 0, btRxBuf.Length);
            }
            catch { }

            for (int i = 0; i < wReturnVal.Length; i++)
            {
                wReturnVal[i] = 0;
            }

            if (nRxLen > 4)
            {
                bCheckSumOk = true;
                if (btRxBuf[3] == (Byte)'r' || btRxBuf[3] == (Byte)'w')
                {
                    char[] szBcc = string.Format("{0:X2}", GetChecksum(btRxBuf, nRxLen - 2)).ToCharArray();
                    if (szBcc[0] != btRxBuf[nRxLen - 2] || szBcc[1] != btRxBuf[nRxLen - 1])
                    {
                        bCheckSumOk = false;
                    }
                    nRxLen -= 2;
                }

                if (bCheckSumOk)
                {
                    if (btRxBuf[0] == 0x06 && btRxBuf[nRxLen - 1] == 0x03)
                    {
                        if (btRxBuf[1] == szAddr[0] && btRxBuf[2] == szAddr[1])
                        {
                            switch (btRxBuf[3])
                            {
                                case (Byte)'r':
                                case (Byte)'R':
                                    int nPos = 0;
                                    Byte btTemp;
                                    if (szCmdType[0] == btRxBuf[4] && szCmdType[1] == btRxBuf[5])
                                    {
                                        int nBlockCnt = Convert.ToInt32(string.Format("{0}{1}", (char)btRxBuf[6], (char)btRxBuf[7]));
                                        for (int i = 0; i < nBlockCnt; i++)
                                        {
                                            if (i >= wReturnVal.Length) break;
                                            wReturnVal[i] = 0x0000;

                                            int nDataCnt = Convert.ToInt32(string.Format("{0}{1}", (char)btRxBuf[8 + nPos], (char)btRxBuf[9 + nPos]));
                                            for (int k = 0; k < nDataCnt * 2; k++)
                                            {
                                                wReturnVal[i] <<= 4;
                                                char c = (char)btRxBuf[10 + nPos];
                                                if (c >= 'A' && c <= 'F') btTemp = (Byte)(c - 'A' + 10);
                                                else if (c >= 'a' && c <= 'f') btTemp = (Byte)(c - 'a' + 10);
                                                else if (c >= '0' && c <= '9') btTemp = (Byte)(c - '0');
                                                else btTemp = 0x00;

                                                wReturnVal[i] |= (ushort)btTemp;
                                                nPos++;
                                            }
                                            nPos += 2;
                                        }
                                    }
                                    else return -6;
                                    break;

                                case (Byte)'w':
                                case (Byte)'W':
                                    if (szCmdType[0] != btRxBuf[4] || szCmdType[1] != btRxBuf[5])
                                        return -5;
                                    break;

                                default:
                                    return -4;
                            }
                        }
                        else return -3;
                    }
                    else return -2;
                }
            }
            else return -1;

            return 1;
        }
    }
}