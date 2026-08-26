using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Net;

namespace main
{
    class classCnet
    {
        public SerialPort serialPort = null;

        public byte[] m_bDIBuffer;
        public bool[] m_bDOValue;
        public bool[] m_bDOFbValue;
        public byte[] m_bDOBuffer;
        public ushort[] m_nAIBuffer;
        public ushort[] m_nAOBuffer;
        public ushort[] nCnetAnswerValue = new ushort[16];

        private Thread threadRun = null;
        private ManualResetEvent threadEvent = new ManualResetEvent(true);
        private bool bThreadRunStop = false;


        public classCnet(SerialPort spHandle)
        {
            serialPort = spHandle;

            m_bDIBuffer = new byte[1024];
            m_bDOBuffer = new byte[1024];
            m_bDOValue = new bool[1024];
            m_bDOFbValue = new bool[1024];
            m_nAIBuffer = new ushort[50];
            m_nAOBuffer = new ushort[50];


            // Thread

            /*if (threadRun != null)
            {
                threadRun = new Thread(new ParameterizedThreadStart(ThreadRunFunc));
                threadRun.Priority = ThreadPriority.Highest;
                threadRun.Start(this);
                threadRun.IsBackground = true;
                bThreadRunStop = true;
            }*/

        }

        // Receive thread
        private void ThreadRunFunc(object objClass)
        {
            int i, k, n;
            ushort wTemp;
            string strBuf;
            string strTemp;
            int nCnetResult;
            byte[] btTxBuf = new byte[1024];
            byte[] btRxBuf = new byte[1024];

            k = 0;
            n = 0;
            wTemp = 0x0000;

            try
            {
                while (bThreadRunStop)
                {
                    threadEvent.WaitOne(Timeout.Infinite);

                    for (k = 0; k < 4; k++)
                    {
                        // PLC로 DI입력
                        strBuf = string.Format("{0:X2}{1}{2}{3}{4}{5}", 0, "r", "SS", "01", "06", string.Format("%PW00{0}", k));
                        Request(strBuf.ToCharArray());
                        Thread.Sleep(80);
                        nCnetResult = Answer("00".ToCharArray(), 'w', "SS".ToCharArray(), out nCnetAnswerValue);
                        if (nCnetResult >= 0)
                        {
                            m_bDIBuffer[k * 2] = (byte)(nCnetAnswerValue[0] >> 8);
                            m_bDIBuffer[(k * 2) + 1] = (byte)(nCnetAnswerValue[0] & 0xFF);

                            Console.WriteLine("DI{0}:{1:X}", k + 1, nCnetAnswerValue[0]);
                        }
                    }

                    for (k = 0; k < 10; k++)
                    {
                        // PLC로 DO입력
                        strBuf = string.Format("{0:X2}{1}{2}{3}{4}{5}", 0, "r", "SS", "01", "06", string.Format("%PW0{0:00}", k + 5));
                        Request(strBuf.ToCharArray());
                        Thread.Sleep(80);
                        nCnetResult = Answer("00".ToCharArray(), 'w', "SS".ToCharArray(), out nCnetAnswerValue);
                        if (nCnetResult >= 0)
                        {
                            if (nCnetAnswerValue[0] != 0)
                            {
                                for (i = 0; i < 16; i++)
                                {
                                    m_bDOFbValue[i + (k * 16)] = (((uint)nCnetAnswerValue[0] & (uint)(0x0001 << i)) > 0) ? true : false;
                                }
                                Console.WriteLine("DODB{0}:{1:X}", k + 1, nCnetAnswerValue[0]);
                            }
                        }
                    }

                    for (k = 0; k < 10; k++)
                    {
                        // PLC로 DO출력
                        wTemp = 0x0000;
                        for (i = 0; i < 16; i++)
                        {
                            wTemp |= (ushort)(m_bDOValue[i + (k * 16)] ? 0x0001 << i : 0x0000);
                        };

                        strTemp = Convert.ToString(wTemp, 16);
                        strTemp = strTemp.PadLeft(4, '0');

                        strBuf = string.Format("{0:X2}{1}{2}{3}{4}{5}{6}", 0, "w", "SS", "01", "06", string.Format("%PW0{0:00}", k + 5), strTemp);
                        Request(strBuf.ToCharArray());
                        Thread.Sleep(80);
                        nCnetResult = Answer("00".ToCharArray(), 'w', "SS".ToCharArray(), out nCnetAnswerValue);
                        if (nCnetResult >= 0)
                        {

                        }
                    }
                }
            }
            catch (ThreadInterruptedException)
            {

            }
        }

        public void SetDO(int nModule, int nPin, int nValue)
        {
            m_bDOValue[nPin] = (nValue == 0) ? false : true;
        }
        public void SetDO(int nPin, int nValue)
        {
            m_bDOValue[nPin] = (nValue == 0) ? false : true;
        }

        //
        public void SetDO(int nModule, int nPin, bool bValue)
        {
            m_bDOValue[nPin] = bValue;
        }

        //
        public int GetDO(int nModule, int nPin)
        {
            int ret = m_bDOFbValue[nPin] == true ? 1 : 0;
            return ret;
        }

        public int GetDI(int nModule, int nPin)
        {
            if (nPin >= 16) return 0;

            nPin = 15 - nPin;

            int nByteIndex = (nPin / 8) + (nModule * 2);
            int nBitIndex = 7 - (nPin % 8);

            byte cShift = 0x01;
            cShift <<= nBitIndex;
            int ret = ((m_bDIBuffer[nByteIndex] & cShift) > 0) ? 1 : 0;
            return ret;
        }

        private Byte GetChecksum(Byte[] btDatBuf, int nDataLen)
        {
            int i;
            Byte btChecksum;

            i = 0;
            btChecksum = 0x00;
            while (nDataLen-- != 0) // pass through message buffer
            {
                btChecksum += btDatBuf[i++];
            }

            return btChecksum;
        }

        public int GetRxBuf()
        {
            int nRxLen = 0;
            Byte[] btRxBuf = new byte[256];
            if (serialPort != null) nRxLen = serialPort.Read(btRxBuf, 0, btRxBuf.Length);

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

        public int Answer(char[] szAddr, char chCmd, char[] szCmdType, out ushort[] wReturnVal)
        {
            int nRxLen;
            bool bCheckSumOk;
            Byte[] btRxBuf = new byte[256];
            wReturnVal = new ushort[16];

            nRxLen = 0;

            try
            {
                if (serialPort != null) nRxLen = serialPort.Read(btRxBuf, 0, btRxBuf.Length);
                if (nRxLen > 0 && btRxBuf[0] == 0x05)
                {
                    Thread.Sleep(100); // PLC가 진짜 응답을 보낼 시간을 벌어줌
                    if (serialPort.BytesToRead > 0)
                    {
                        // 에코 백 데이터를 덮어쓰고 진짜 응답을 수신
                        nRxLen = serialPort.Read(btRxBuf, 0, btRxBuf.Length);
                    }
                }
            }
            catch
            {

            }

            for (int i = 0; i < wReturnVal.Length; i++)
            {
                wReturnVal[i] = 0;
            }

            // Checksum
            if (nRxLen > 4)
            {
                bCheckSumOk = true;
                if (btRxBuf[3] == (Byte)'r' || btRxBuf[3] == (Byte)'w')
                //if (btRxBuf[3] == (Byte)'r' || btRxBuf[3] == (Byte)'w' || btRxBuf[3] == (Byte)'R' || btRxBuf[3] == (Byte)'W')
                {
                    char[] szBcc = string.Format("{0:X2}", GetChecksum(btRxBuf, nRxLen - 2)).ToCharArray();
                    if (szBcc[0] != btRxBuf[nRxLen - 2] || szBcc[1] != btRxBuf[nRxLen - 1])
                    {
                        bCheckSumOk = false;
                    }

                    nRxLen -= 2;
                }

                if (bCheckSumOk == true)
                {
                    if (btRxBuf[0] == 0x06 && btRxBuf[nRxLen - 1] == 0x03)
                    {
                        if (btRxBuf[1] == szAddr[0] && btRxBuf[2] == szAddr[1])
                        {
                            switch (btRxBuf[3])
                            {
                                case (Byte)'r':
                                case (Byte)'R':
                                    int nPos;
                                    int nBlockCnt;
                                    int nDataCnt;
                                    Byte btTemp;

                                    nPos = 0;

                                    if (szCmdType[0] == btRxBuf[4] && szCmdType[1] == btRxBuf[5])
                                    {
                                        nBlockCnt = Convert.ToInt32(string.Format("{0}{1}", Char.ConvertFromUtf32(btRxBuf[6]), Char.ConvertFromUtf32(btRxBuf[7])));

                                        for (int i = 0; i < nBlockCnt; i++)
                                        {
                                            if (i >= wReturnVal.Length) break;

                                            wReturnVal[i] = 0x0000;

                                            nDataCnt = Convert.ToInt32(string.Format("{0}{1}", Char.ConvertFromUtf32(btRxBuf[8 + nPos]), Char.ConvertFromUtf32(btRxBuf[9 + nPos])));
                                            for (int k = 0; k < nDataCnt * 2; k++)
                                            {
                                                wReturnVal[i] <<= 4;

                                                //wReturnVal[i] |= (ushort)(btRxBuf[10 + nPos] - 0x30);

                                                if (btRxBuf[10 + nPos] >= 'A' && btRxBuf[10 + nPos] <= 'F')
                                                {
                                                    btTemp = (Byte)((char)btRxBuf[10 + nPos] - 'A' + 10);
                                                }
                                                else
                                                if (btRxBuf[10 + nPos] >= 'a' && btRxBuf[10 + nPos] <= 'f')
                                                {
                                                    btTemp = (Byte)((char)btRxBuf[10 + nPos] - 'a' + 10);
                                                }
                                                else
                                                if (btRxBuf[10 + nPos] >= '0' && btRxBuf[10 + nPos] <= '9')
                                                {
                                                    btTemp = (Byte)((char)btRxBuf[10 + nPos] - '0');
                                                }
                                                else
                                                {
                                                    btTemp = (Byte)((char)0x00);
                                                }

                                                wReturnVal[i] |= (ushort)btTemp;
                                                nPos++;
                                                //wReturnVal[i] = (ushort)(((btRxBuf[10] - 0x30) << 12) | ((btRxBuf[11] - 0x30) << 8) | ((btRxBuf[12] - 0x30) << 4) | ((btRxBuf[13] - 0x30)));
                                            }
                                            nPos += 2;
                                        }
                                    }
                                    else
                                    {
                                        return -6; // 'r' 또는 'R' Command type이 다르면..
                                    }
                                    break;

                                case (Byte)'w':
                                case (Byte)'W':
                                    if (szCmdType[0] == btRxBuf[4] && szCmdType[1] == btRxBuf[5])
                                    {
                                    }
                                    else
                                    {
                                        return -5; // 'w' 또는 'W' Command type이 다르면..
                                    }
                                    break;

                                default:
                                    return -4; // 'R', 'r', 'W', 'w'이 아닐때..
                                               //break;
                            }
                        }
                        else
                        {
                            return -3; // 국번(어드레스)가 맞지 않을때..
                        }
                    }
                    else
                    {
                        return -2; // ACK(0x06)와 ETX(0x03)이 아닐때..
                    }
                }
            }
            else
            {
                return -1; // 오류검출이 되었을때..
            }

            return 1; // 정상일때..
        }
    }
}
