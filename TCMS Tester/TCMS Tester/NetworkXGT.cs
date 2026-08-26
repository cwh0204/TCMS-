using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CITester
{
    // Delegate 선언
    //
    public delegate void NetworkXGTAlarm();
    public delegate void NetworkXGTEvent(byte[] packet);
    public delegate void NetworkXGTDisplay(string msg);

    public class NetworkXGT
    {
        //===============================================================================
        //
        // 일반적인 네트웍 관련 변수
        //
        public Thread m_Thread = null;
        public bool m_bRunFlag = false;
        public bool m_bStart = false;
        public bool m_bWrite = false;
        public bool m_bWriteOnce = false;
        public Socket m_Socket = null;

        private int m_nOnceCount = 0;

        string m_strIP = "127.0.0.1";
        int m_nPort = 2004;

        int m_nInvokeID = 1;

        public bool m_bConnected = false;

        public bool CONNECTED
        {
            get { return m_bConnected; }
        }
        public int PORT
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }
        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }
        public bool START
        {
            get { return m_bStart; }
            set { m_bStart = value; }
        }
        public bool WRITE
        {
            get { return m_bWrite; }
            set { m_bWrite = value; }
        }
        public bool WRITE_ONCE
        {
            get { return m_bWriteOnce; }
            set { m_nOnceCount = 0; m_bWriteOnce = value; }
        }
        private bool m_bFastMode = false;
        public bool FAST_MODE
        {
            get { return m_bFastMode; }
            set { m_bFastMode = value; }
        }

        private int m_bFastReadIndex = -1;
        public int FAST_READ_INDEX
        {
            get { return m_bFastReadIndex; }
            set { m_bFastReadIndex = value; }
        }

        public event NetworkXGTAlarm OnConnect;
        public event NetworkXGTAlarm OnNotConnect;
        public event NetworkXGTAlarm OnDisconnect;
        //public event NetworkEvent OnReceive;
        public event NetworkXGTDisplay OnDisplay;

        //===============================================================================
        //
        // Modbus 관련 선언
        //
        enum ModbusCommand : byte
        {
            ReadDO = 0x01,
            ReadDI = 0x02,
            ReadAO = 0x03,
            ReadAI = 0x04,
            WriteSingleDO = 0x05,
            WriteSingleAO = 0x06,
            WriteDO = 0x0F,
            WriteAO = 0x10
        }

        struct XGTHeader
        {
            public byte bCompanyID1;    // Company ID
            public byte bCompanyID2;
            public byte bCompanyID3;
            public byte bCompanyID4;
            public byte bCompanyID5;
            public byte bCompanyID6;
            public byte bCompanyID7;
            public byte bCompanyID8;
            public ushort nReserved;   
            public ushort nPLCInfo;     // PLC Info
            public byte bCPUInfo;       // PLC CPU Info
            public byte bSource;        // Source of frame
            public ushort nInvokeID;
            public ushort nLength;      // Application frame length
            public byte bPositionl;     // FEnet position;
            public byte bBCC;
        }

        struct XGTSendData
        {
            public ushort nCommand;
            public ushort nDatatype;
            public ushort nReserved;
            public ushort nBlock;
            public ushort nLength;
            public byte bAddress1;
            public byte bAddress2;
            public byte bAddress3;
            public byte bAddress4;
            public byte bAddress5;
            public byte bAddress6;
            public byte bAddress7;
            public byte bAddress8;
            public ushort nCount;
        }

        struct XGTRecvData
        {
            public byte cPLCID;
            public byte cFunction;
            public byte cCount;
        }


        public byte[] m_bDIBuffer;
        public bool[] m_bDOValue;
        public byte[] m_bDOBuffer;
        public ushort[] m_nAIBuffer;
        public ushort[] m_nAOBuffer;

        public int m_nDIModule = 1;            // Digital Input module의 개수
        public int m_nDIPin = 16;              // DI module당 입력 pin 개수
        public int m_nDOModule = 1;            // Digital Output module의 개수
        public int m_nDOPin = 48;              // DO module당 입력 pin 개수
        public int m_nAIModule = 1;            // Analog Input module의 개수
        public int m_nAIPin = 16;              // AI module당 입력 pin 개수
        public int m_nAOModule = 1;            // Analog Output module의 개수
        public int m_nAOPin = 10;               // AO module당 입력 pin 개수

        //#########################################################################################
        //
        /// <summary>
        ///     NetworkModbus 생성자
        /// </summary>
        /// 
        public NetworkXGT()
        {
            m_bDIBuffer = new byte[100];
            m_bDOBuffer = new byte[100];
            m_bDOValue = new bool[512];
            m_nAIBuffer = new ushort[50];
            m_nAOBuffer = new ushort[50];
        }

        //#########################################################################################
        //
        public int GetDI(int nModule, int nPin)
        {
            int nByteIndex = nPin / 8 + (nModule * 8);
            int nBitIndex = nPin % 8;

            byte cShift = 0x01;
            cShift <<= nBitIndex;
            int ret = ((m_bDIBuffer[nByteIndex] & cShift) > 0) ? 1 : 0;
            return ret;
        }

        //#########################################################################################
        //
        public void SetDO(int nModule, int nPin, int nValue)
        {
            m_bDOValue[nPin] = (nValue == 0) ? false : true;
            /*
            int nByteIndex = nPin / 8 + ((nModule - 1) * 8);
            int nBitIndex = nPin % 8;

            byte cShift = 0x01;
            cShift <<= nBitIndex;
            m_bDOBuffer[nByteIndex] |= cShift;
            */
        }

        //#########################################################################################
        //
        public void SetDO(int nModule, int nPin, bool bValue)
        {
            m_bDOValue[nPin] = bValue;
        }

        //#########################################################################################
        //
        public int GetDO(int nModule, int nPin)
        {
            int nByteIndex = nPin / 8 + (nModule * 8);
            int nBitIndex = nPin % 8;

            byte cShift = 0x01;
            cShift <<= nBitIndex;
            int ret = ((m_bDOBuffer[nByteIndex] & cShift) > 0) ? 1 : 0;
            return ret;
        }

        //#########################################################################################
        //
        public int GetAI(int nModule, int nPin)
        {
            return (int)m_nAIBuffer[nModule * m_nAIPin + nPin];
        }

        //#########################################################################################
        //
        public void SetAO(int nModule, int nPin, int nValue)
        {
            m_nAOBuffer[nModule * m_nAOPin + nPin] = (ushort)nValue;
        }

        //#########################################################################################
        //
        public int GetAO(int nModule, int nPin)
        {
            return (int)m_nAOBuffer[nModule * m_nAIPin + nPin];
        }

        //#########################################################################################
        //
        /// <summary>
        ///     서버와 연결 시도(클라이언트용)
        /// </summary>
        /// <param name="ip">연결할 서버의 IP주소</param>
        /// <returns>연결 유무</returns>
        /// 
        public bool Connect(string ip)
        {
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), this.m_nPort);
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.Connect(ipep);

                this.m_strIP = ip;
                m_Thread = new Thread(new ThreadStart(Control));
                m_bRunFlag = true;
                m_Thread.Start();

                if (OnConnect != null)
                    OnConnect();

                m_bConnected = true;
                return true;
            }
            catch
            {
                if (OnNotConnect != null)
                    OnNotConnect();
                return false;
            }
        }

        //#########################################################################################
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// 
        public bool Connect(string ip, string port)
        {
            m_nPort = int.Parse(port);
            return this.Connect(ip);
        }


        //#########################################################################################
        //
        public bool Connect()
        {
            return this.Connect(m_strIP);
        }


        //#########################################################################################
        //
        /// <summary>
        ///     서버와의 연결 종료(클라이언트용)
        /// </summary>
        /// 
        public void Disconnect()
        {
            try
            {
                if (m_Socket != null)
                {
                    if (m_Socket.Connected)
                    {
                        m_Socket.Close();
                    }
                }
                m_bConnected = false;

                m_bRunFlag = false;
                if (m_Thread != null && m_Thread.IsAlive)
                    m_Thread.Abort();
            }
            catch (Exception ex)
            {
                if (OnDisplay != null)
                    OnDisplay(ex.Message);
            }
            finally
            {
                if (OnDisconnect != null)
                    OnDisconnect();
            }
        }

        //#########################################################################################
        //
        /// <summary>
        ///     사용자가 입력한 문장을 상대방에게 전송
        /// </summary>
        /// <param name="msg">전송할 문자열</param>
        /// 
        public void Send(string msg)
        {
            try
            {
                if (m_Socket != null && m_Socket.Connected)
                {
                    byte[] data = Encoding.Default.GetBytes(msg);
                    this.SendData(data, data.Length);
                }
                else
                {
                    if (OnDisplay != null)
                        OnDisplay("네트웍 미연결..메시지 전송 실패!");
                }
            }
            catch (Exception ex)
            {
                if (OnDisplay != null)
                    OnDisplay(ex.Message);
            }
        }

        //#########################################################################################
        //
        /// <summary>
        ///     전송 포맷에 맞게 데이터 전송
        /// </summary>
        /// <param name="data">전송할 바이트 배열</param>
        /// 
        public void SendData(byte[] data, int count)
        {
            if( count > data.Length )
                return;

            try
            {
                int nTotal = 0;
                int nSize = count;
                int nLeftData = nSize;
                int nSendData = 0;

                while (nTotal < nSize)
                {
                    nSendData = this.m_Socket.Send(data, nTotal, nLeftData, SocketFlags.None);
                    nTotal += nSendData;
                    nLeftData -= nSendData;
                }
            }
            catch (Exception ex)
            {
                if (OnDisplay != null)
                    OnDisplay(ex.Message);
            }
        }

        //#########################################################################################
        //
        /// <summary>
        ///     상대방이 보낸 데이터 수신하기
        /// </summary>
        /// <returns>수신한 데이터의 바이트 배열</returns>
        /// 
        public byte[] ReceiveData(int nSize)
        {
            int startTickCount = Environment.TickCount;

            int nTotal = 0;
            //int nSize = 0;
            int nLeftData = nSize;
            int nRecvData = 0;

            byte[] data = new byte[nSize];
            while (nTotal < nSize)
            {
                if (Environment.TickCount > startTickCount + 1000)
                    return null;

                try
                {
                    nRecvData = this.m_Socket.Receive(data, nTotal, nLeftData, SocketFlags.None);

                    //if (nRecvData == 0)
                    //    return null;
                    nTotal += nRecvData;
                    nLeftData -= nRecvData;
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return data;
        }




        //#########################################################################################
        //
        public void Control()
        {
            byte[] buffer = new byte[10];
            //int i;

            try
            {
                while(m_bRunFlag)
                {
                    while (m_Socket != null && m_Socket.Connected )
                    {
                        if (m_bStart == false)
                        {
                            Thread.Sleep(500);
                            continue;
                        }

                        if (m_Socket.Poll(0, SelectMode.SelectRead))
                        {
                            if (m_Socket.Receive(buffer, SocketFlags.Peek) == 0)
                            {
                                m_Socket.Shutdown(SocketShutdown.Both);
                                m_Socket.Close();
                                m_Socket = null;
                                break;
                            }
                        }

                        // Read digital input
                        //
                        ReadDigitalInput(0, (ushort)(m_nDIModule * m_nDIPin));
                        ReceivePacket(0);

                        // Read digital input
                        //
                        ReadDigitalInput(16, (ushort)(m_nDIModule * m_nDIPin));
                        ReceivePacket(4);

                        if (m_bWrite == true || m_bWriteOnce == true)
                        {
                            // Write digital output
                            //
                            WriteDigitalOutput(0, (ushort)(m_nDOModule * m_nDOPin));

                            if (m_bWriteOnce == true)
                            {
                                m_bWrite = false;
                                m_bWriteOnce = false;
                            }
                        }

                        // Write analog output
                        //
                        for (int i = 0; i < (m_nAOModule * m_nAOPin); ++i)
                        {
                            WriteSingleAnalogOutput((ushort)i, m_nAOBuffer[i]);
                            ReceivePacket(1);
                        }
                        
                        // Read analog input
                        //
                        //ReadAnalogInput(0, (ushort)(m_nAIModule * m_nAIPin));

                        if (m_bFastMode == true)
                            Thread.Sleep(5);
                        else
                            Thread.Sleep(50);
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                if (OnDisplay != null)
                    OnDisplay(ex.Message);
            }
            finally
            {
                m_bConnected = false;
                if (OnDisconnect != null)
                    OnDisconnect();
            }
        }

        //#########################################################################################
        //
        /// <summary>
        ///     MODBUS 패킷 수신
        /// </summary>
        /// 
        public void ReceivePacket(int nType)
        {
            //XGTHeader header;
            //XGTRecvData data;
            //int i;

            try
            {
                // header 수신
                //
                byte[] recv = ReceiveData(20);
                if (recv == null)
                    return;
                
                // 명령어, 데이터타입, 예약영역, 에러상태 읽기
                recv = ReceiveData(8);
                if (recv == null)
                    return;

                // 에러 확인
                if (recv[6] == 0xff || recv[7] == 0xff)
                {
                    recv = ReceiveData(1);
                    return;
                }

                if (nType == 0)
                {
                    // 블럭수, 데이터 개수 
                    recv = ReceiveData(4);
                    if (recv == null)
                        return;

                    recv = ReceiveData(recv[2]);
                    m_bDIBuffer[0] = recv[0];
                    m_bDIBuffer[1] = recv[1];
                }
                else if (nType == 4)
                {
                    // 블럭수, 데이터 개수 
                    recv = ReceiveData(4);
                    if (recv == null)
                        return;

                    recv = ReceiveData(recv[2]);
                    m_bDIBuffer[2] = recv[0];
                    m_bDIBuffer[3] = recv[1];
                }
                else if (nType == 1)
                {
                    // 블럭수
                    recv = ReceiveData(2);
                }
                else if (nType == 2)
                {
                    // 블럭수, 데이터 개수 
                    recv = ReceiveData(4);
                    if (recv == null)
                        return;

                    recv = ReceiveData(recv[2]);
                }
            }
            catch (Exception ex)
            {
                if (OnDisplay != null)
                    OnDisplay(ex.Message);

            }
        }


        //#########################################################################################
        //
        /// <summary>
        ///     MODBUS 패킷 수신
        /// </summary>
        /// 
        public void ReceivePacket(int nType, int index)
        {
            //XGTHeader header;
            //XGTRecvData data;
            //int i;

            try
            {
                // header 수신
                //
                byte[] recv = ReceiveData(20);
                if (recv == null)
                    return;
                
                // 명령어, 데이터타입, 예약영역, 에러상태 읽기
                recv = ReceiveData(8);
                if (recv == null)
                    return;

                // 에러 확인
                if (recv[6] == 0xff || recv[7] == 0xff)
                {
                    recv = ReceiveData(1);
                    return;
                }

                if (nType == 0)
                {
                    // 블럭수, 데이터 개수 
                    recv = ReceiveData(4);
                    if (recv == null)
                        return;

                    recv = ReceiveData(recv[2]);
                    m_bDIBuffer[0] = recv[0];
                    m_bDIBuffer[1] = recv[1];
                }
                else if (nType == 4)
                {
                    // 블럭수, 데이터 개수 
                    recv = ReceiveData(4);
                    if (recv == null)
                        return;

                    recv = ReceiveData(recv[2]);
                    m_bDIBuffer[2] = recv[0];
                    m_bDIBuffer[3] = recv[1];
                }
                else if (nType == 1)
                {
                    // 블럭수
                    recv = ReceiveData(2);
                }
                else if (nType == 2)
                {
                    // 블럭수, 데이터 개수 
                    recv = ReceiveData(4);
                    if (recv == null)
                        return;

                    recv = ReceiveData(recv[2]);
                    m_nAIBuffer[index] = (ushort)(recv[1] * 256 + recv[0]);
                }
            }
            catch (Exception ex)
            {
                if (OnDisplay != null)
                    OnDisplay(ex.Message);

            }
        }


        //#########################################################################################
        //
        /// <summary>
        ///     PLC에 Digital Input 상태 읽어오기 명령 전송
        /// </summary>
        /// <param name="nStart">시작위치</param>
        /// <param name="nCount">읽을 개수</param>
        /// 
        public void ReadDigitalInput(ushort nStart, ushort nCount)
        {
            if (CONNECTED == false)
                return;

            byte[] bytedata = new byte[100];
            byte[] convert = new byte[2];
            byte checksum = 0;

            // 헤더
            //
            bytedata[0] = Convert.ToByte('L');
            bytedata[1] = Convert.ToByte('S');
            bytedata[2] = Convert.ToByte('I');
            bytedata[3] = Convert.ToByte('S');
            bytedata[4] = Convert.ToByte('-');
            bytedata[5] = Convert.ToByte('X');
            bytedata[6] = Convert.ToByte('G');
            bytedata[7] = Convert.ToByte('T');
            bytedata[8] = Convert.ToByte(0);
            bytedata[9] = Convert.ToByte(0);
            bytedata[10] = Convert.ToByte(0);
            bytedata[11] = Convert.ToByte(0);
            bytedata[12] = Convert.ToByte(0x00);
            bytedata[13] = Convert.ToByte(0x22);
            bytedata[14] = Convert.ToByte(1);
            bytedata[15] = Convert.ToByte(0);
            bytedata[16] = Convert.ToByte(16);
            bytedata[17] = Convert.ToByte(0);
            bytedata[18] = Convert.ToByte(0);
            for (int i = 0; i <= 18; ++i)
                checksum += bytedata[i];
            bytedata[19] = Convert.ToByte(checksum);

            // 명령어
            bytedata[20] = Convert.ToByte(0x54);
            bytedata[21] = Convert.ToByte(0x00);
            // 데이터 형식
            bytedata[22] = Convert.ToByte(0x02);
            bytedata[23] = Convert.ToByte(0x00);           
            // 예약영역
            bytedata[24] = Convert.ToByte(0x00);
            bytedata[25] = Convert.ToByte(0x00);            
            // 블럭수
            bytedata[26] = Convert.ToByte(0x01);
            bytedata[27] = Convert.ToByte(0x00);            
            // 주소
            bytedata[28] = Convert.ToByte(0x06);
            bytedata[29] = Convert.ToByte(0x00);
            bytedata[30] = Convert.ToByte('%');
            bytedata[31] = Convert.ToByte('P');
            bytedata[32] = Convert.ToByte('W');
            bytedata[33] = Convert.ToByte('0');
            bytedata[34] = Convert.ToByte('0');
            if( nStart == 0 )
                bytedata[35] = Convert.ToByte('1');
            else
                bytedata[35] = Convert.ToByte('2');

            m_nInvokeID++;
            m_nInvokeID = m_nInvokeID % 20000;

            SendData(bytedata, 36);
        }

        //#########################################################################################
        //
        /// <summary>
        ///     PLC에 Analog Input 읽기 명령을 전송
        /// </summary>
        /// <param name="nStart">시작위치</param>
        /// <param name="nCount">읽을 개수</param>
        /// 
        public void ReadAnalogInput(ushort nStart, ushort nCount)
        {
            if (CONNECTED == false)
                return;

            byte[] bytedata = new byte[100];
            byte[] convert = new byte[2];
            byte checksum = 0;

            // 헤더
            //
            bytedata[0] = Convert.ToByte('L');
            bytedata[1] = Convert.ToByte('S');
            bytedata[2] = Convert.ToByte('I');
            bytedata[3] = Convert.ToByte('S');
            bytedata[4] = Convert.ToByte('-');
            bytedata[5] = Convert.ToByte('X');
            bytedata[6] = Convert.ToByte('G');
            bytedata[7] = Convert.ToByte('T');
            bytedata[8] = Convert.ToByte(0);
            bytedata[9] = Convert.ToByte(0);
            bytedata[10] = Convert.ToByte(0);
            bytedata[11] = Convert.ToByte(0);
            bytedata[12] = Convert.ToByte(0x00);
            bytedata[13] = Convert.ToByte(0x22);
            bytedata[14] = Convert.ToByte(1);
            bytedata[15] = Convert.ToByte(0);
            bytedata[16] = Convert.ToByte(16);
            bytedata[17] = Convert.ToByte(0);
            bytedata[18] = Convert.ToByte(0);
            for (int i = 0; i <= 18; ++i)
                checksum += bytedata[i];
            bytedata[19] = Convert.ToByte(checksum);

            // 명령어
            bytedata[20] = Convert.ToByte(0x54);
            bytedata[21] = Convert.ToByte(0x00);
            // 데이터 형식
            bytedata[22] = Convert.ToByte(0x02);
            bytedata[23] = Convert.ToByte(0x00);
            // 예약영역
            bytedata[24] = Convert.ToByte(0x00);
            bytedata[25] = Convert.ToByte(0x00);
            // 블럭수
            bytedata[26] = Convert.ToByte(0x01);
            bytedata[27] = Convert.ToByte(0x00);
            // 주소
            bytedata[28] = Convert.ToByte(0x06);
            bytedata[29] = Convert.ToByte(0x00);
            bytedata[30] = Convert.ToByte('%');
            bytedata[31] = Convert.ToByte('D');
            bytedata[32] = Convert.ToByte('W');
            bytedata[33] = Convert.ToByte('0');
            for (int i = 0; i < nCount; ++i)
            {
                bytedata[34] = Convert.ToByte('0');
                if (i >= 10 && i <= 19)
                    bytedata[34] += 1;
                if (i >= 20 && i <= 29)
                    bytedata[34] += 2;
                bytedata[35] = Convert.ToByte('0');
                bytedata[35] += (byte)(i % 10);

                SendData(bytedata, 36);
                ReceivePacket(2, i);
            }
            m_nInvokeID++;
            m_nInvokeID = m_nInvokeID % 20000;

        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteDO()
        {
            WriteDigitalOutput(0, (ushort)(m_nDOModule * m_nDOPin));
        }

        //#########################################################################################
        //
        /// <summary>
        ///     PLC에 Digital Output 쓰기 명령 전송
        /// </summary>
        /// <param name="nStart"></param>
        /// <param name="nCount"></param>
        /// 
        public void WriteDigitalOutput(ushort nStart, ushort nCount)
        {
            //if (CONNECTED == false)
            //    return;

            byte[] bytedata = new byte[100];
            byte[] convert = new byte[2];
            byte checksum = 0;

            int nByteIndex = 0;
            int nBitIndex = 0;
            byte cShift = 0x01;
            for (int i = 0; i < 16; ++i)
                m_bDOBuffer[i] = 0;
            for (int i = 0; i < 128; ++i)
            {
                if (m_bDOValue[i] == true)
                {
                    nByteIndex = i / 8;
                    nBitIndex = i % 8;

                    cShift = 0x01;
                    cShift <<= nBitIndex;
                    m_bDOBuffer[nByteIndex] |= cShift;
                }
            }

            // 헤더
            //

            //company id
            bytedata[0] = Convert.ToByte('L');
            bytedata[1] = Convert.ToByte('S');
            bytedata[2] = Convert.ToByte('I');
            bytedata[3] = Convert.ToByte('S');
            bytedata[4] = Convert.ToByte('-');
            bytedata[5] = Convert.ToByte('X');
            bytedata[6] = Convert.ToByte('G');
            bytedata[7] = Convert.ToByte('T');
            bytedata[8] = Convert.ToByte(0);
            bytedata[9] = Convert.ToByte(0);
            // PLC info
            bytedata[10] = Convert.ToByte(0);
            bytedata[11] = Convert.ToByte(0);
            // CPU info
            bytedata[12] = Convert.ToByte(0x00);
            // source of frame
            bytedata[13] = Convert.ToByte(0x22);
            // Invoke ID
            bytedata[14] = Convert.ToByte(2);
            bytedata[15] = Convert.ToByte(0);
            // length
            bytedata[16] = Convert.ToByte(20);
            bytedata[17] = Convert.ToByte(0);
            // FEnet position
            bytedata[18] = Convert.ToByte(0);
            // checksum 
            for (int i = 0; i <= 18; ++i)
                checksum += bytedata[i];
            bytedata[19] = Convert.ToByte(checksum);

            // 명령어
            bytedata[20] = Convert.ToByte(0x58);
            bytedata[21] = Convert.ToByte(0x00);
            // 데이터 타입
            bytedata[22] = Convert.ToByte(0x02);
            bytedata[23] = Convert.ToByte(0x00);
            // 예약 영역
            bytedata[24] = Convert.ToByte(0x00);
            bytedata[25] = Convert.ToByte(0x00);
            // 변수 개수
            bytedata[26] = Convert.ToByte(0x01);
            bytedata[27] = Convert.ToByte(0x00);
            // 변수명 길이
            bytedata[28] = Convert.ToByte(0x06);
            bytedata[29] = Convert.ToByte(0x00);

            bytedata[30] = Convert.ToByte('%');
            bytedata[31] = Convert.ToByte('P');
            bytedata[32] = Convert.ToByte('W');
            bytedata[33] = Convert.ToByte('0');

            for (byte i = 0; i < 4; ++i)
            {
                if (i == 0)
                {
                    bytedata[34] = Convert.ToByte('0');
                    bytedata[35] = Convert.ToByte('2');
                    //bytedata[35] = Convert.ToByte('3');
                }
                else if (i == 1)
                {
                    bytedata[34] = Convert.ToByte('0');
                    bytedata[35] = Convert.ToByte('3');
                }
                else if (i == 2)
                {
                    bytedata[34] = Convert.ToByte('0');
                    bytedata[35] = Convert.ToByte('4');
                }
                else if (i == 3)
                {
                    bytedata[34] = Convert.ToByte('0');
                    bytedata[35] = Convert.ToByte('5');
                }
                
                // 데이터 개수
                bytedata[36] = Convert.ToByte(0x02);
                bytedata[37] = Convert.ToByte(0x00);
                bytedata[38] = m_bDOBuffer[i*2];
                bytedata[39] = m_bDOBuffer[i * 2 + 1];

                SendData(bytedata, 40);
                ReceivePacket(1);
            }
            m_nInvokeID++;
            m_nInvokeID = m_nInvokeID % 20000;
        }


        //#########################################################################################
        //
        /// <summary>
        ///     PLC에 Analog Output 쓰기 명령 전송
        /// </summary>
        /// <param name="nStart"></param>
        /// <param name="nData"></param>
        /// 
        public void WriteSingleAnalogOutput(ushort nStart, ushort nData)
        {
            if (CONNECTED == false)
                return;

            byte[] bytedata = new byte[100];
            byte[] convert = new byte[2];
            byte checksum = 0;

            // 헤더
            //
            //company id
            bytedata[0] = Convert.ToByte('L');
            bytedata[1] = Convert.ToByte('S');
            bytedata[2] = Convert.ToByte('I');
            bytedata[3] = Convert.ToByte('S');
            bytedata[4] = Convert.ToByte('-');
            bytedata[5] = Convert.ToByte('X');
            bytedata[6] = Convert.ToByte('G');
            bytedata[7] = Convert.ToByte('T');
            bytedata[8] = Convert.ToByte(0);
            bytedata[9] = Convert.ToByte(0);
            // PLC info
            bytedata[10] = Convert.ToByte(0);
            bytedata[11] = Convert.ToByte(0);
            // CPU info
            bytedata[12] = Convert.ToByte(0x00);
            // source of frame
            bytedata[13] = Convert.ToByte(0x22);
            // Invoke ID
            bytedata[14] = Convert.ToByte(2);
            bytedata[15] = Convert.ToByte(0);
            // length
            bytedata[16] = Convert.ToByte(20);
            bytedata[17] = Convert.ToByte(0);
            // FEnet position
            bytedata[18] = Convert.ToByte(0);
            // checksum 
            for (int i = 0; i <= 18; ++i)
                checksum += bytedata[i];
            bytedata[19] = Convert.ToByte(checksum);

            // 명령어
            bytedata[20] = Convert.ToByte(0x58);
            bytedata[21] = Convert.ToByte(0x00);
            // 데이터 타입
            bytedata[22] = Convert.ToByte(0x02);
            bytedata[23] = Convert.ToByte(0x00);
            // 예약 영역
            bytedata[24] = Convert.ToByte(0x00);
            bytedata[25] = Convert.ToByte(0x00);
            // 변수 개수
            bytedata[26] = Convert.ToByte(0x01);
            bytedata[27] = Convert.ToByte(0x00);
            // 변수명 길이
            bytedata[28] = Convert.ToByte(0x06);
            bytedata[29] = Convert.ToByte(0x00);

            bytedata[30] = Convert.ToByte('%');
            bytedata[31] = Convert.ToByte('D');
            bytedata[32] = Convert.ToByte('W');
            bytedata[33] = Convert.ToByte('1');
            bytedata[34] = Convert.ToByte('0');
            bytedata[35] = Convert.ToByte('0');
            bytedata[35] += (byte)nStart;

            // 데이터 개수
            bytedata[36] = Convert.ToByte(0x02);
            bytedata[37] = Convert.ToByte(0x00);
            convert = BitConverter.GetBytes(nData);
            bytedata[38] = convert[0];
            bytedata[39] = convert[1];

            SendData(bytedata, 40);
            
            m_nInvokeID++;
            m_nInvokeID = m_nInvokeID % 20000;
        }

        
    }
}
