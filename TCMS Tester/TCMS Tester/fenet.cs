using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CITester
{
	public class classFenet
	{
		byte[] btCompanyIdBuf = System.Text.Encoding.ASCII.GetBytes("LSIS-XGT");

		public UdpClient sckPlcUdp;
		public string strIpAddr = "0.0.0.0";
		public int nPortNo = 2005;
		public byte[] btRxBuf;

		public byte[] m_bDIBuffer;
		public bool[] m_bDOValue;
		public bool[] m_bDOFbValue;
        public byte[] m_bDOBuffer;
		public ushort[] m_nAIBuffer;
		public ushort[] m_nAOBuffer;

		public int m_nAOPin = 10;               // AO module당 입력 pin 개수

		public int m_nAIPin = 16;              // AI module당 입력 pin 개수

		private Thread threadRun = null;
		private ManualResetEvent threadEvent = new ManualResetEvent(true);
		private bool bThreadRunStop = false;


		public classFenet(UdpClient sckPlcUdp)
		{
			this.sckPlcUdp = sckPlcUdp;

			m_bDIBuffer = new byte[1024];
			m_bDOBuffer = new byte[1024];
            m_bDOValue = new bool[1024];
            m_bDOFbValue = new bool[1024];
            m_nAIBuffer = new ushort[50];
			m_nAOBuffer = new ushort[50];

			// Thread
			if (threadRun == null)
            {
				threadRun = new Thread(new ParameterizedThreadStart(ThreadRunFunc));
				threadRun.Priority = ThreadPriority.Highest;
				threadRun.Start(this);
				threadRun.IsBackground = true;
				bThreadRunStop = true;
			}
		}

		public classFenet(UdpClient sckPlcUdp,string strIpAddr)
		{
			this.sckPlcUdp = sckPlcUdp;
			this.strIpAddr = strIpAddr;

			m_bDIBuffer = new byte[1024];
			m_bDOBuffer = new byte[1024];
			m_bDOValue = new bool[1024];
            m_bDOFbValue = new bool[1024];
            m_nAIBuffer = new ushort[50];
			m_nAOBuffer = new ushort[50];

			// Thread
			if (threadRun == null)
			{
				threadRun = new Thread(new ParameterizedThreadStart(ThreadRunFunc));
				threadRun.Priority = ThreadPriority.Highest;
				threadRun.Start(this);
				threadRun.IsBackground = true;
				bThreadRunStop = true;
			}
		}

		public classFenet(UdpClient sckPlcUdp, string strIpAddr, int nPortNo)
		{
			this.sckPlcUdp = sckPlcUdp;
			this.strIpAddr = strIpAddr;
			this.nPortNo = nPortNo;

			m_bDIBuffer = new byte[1024];
			m_bDOBuffer = new byte[1024];
			m_bDOValue = new bool[1024];
            m_bDOFbValue = new bool[1024];
            m_nAIBuffer = new ushort[50];
			m_nAOBuffer = new ushort[50];

			// Thread
			if (threadRun == null)
			{
				threadRun = new Thread(new ParameterizedThreadStart(ThreadRunFunc));
				threadRun.Priority = ThreadPriority.Highest;
				threadRun.Start(this);
				threadRun.IsBackground = true;
				bThreadRunStop = true;
			}
		}
				
		~classFenet()
		{
			ThreadStop();
		}

		// Close
		public void Close()
		{
			ThreadStop();
		}

		void ThreadStop()
        {
			bThreadRunStop = false;

			if (threadRun != null)
			{
				threadRun.Interrupt();
				//threadCuMpuRxRun.Abort();
				//threadCuMpuRxRun.Join();
			}
		}

		// CheckSum
		public int GetChecksum(byte[] btBuff, int nStart, int nEnd)
		{
			int CheckSum = 0;

			for (int i = nStart; i < nEnd; i++)
			{
				CheckSum = CheckSum + btBuff[i];
				if (CheckSum > 255)
				{
					CheckSum = CheckSum - 256;
				}
			}

			return CheckSum;
		}
		public void ReadWrite(char chRw,ushort nBlockCnt,string strVar,ushort nDataLen,uint dwData)
		{
			int i;
			int nTxPos;
			byte[] btTxBuf = new byte[512];
			IPEndPoint epRemote = new IPEndPoint(IPAddress.Parse(strIpAddr), nPortNo);
            nTxPos = 0;

			for (i = 0; i < 8; i++)
			{
				btTxBuf[nTxPos++] = btCompanyIdBuf[i];
			}

			btTxBuf[nTxPos++] = 0x00;
			btTxBuf[nTxPos++] = 0x00;

			btTxBuf[nTxPos++] = 0x00;
			btTxBuf[nTxPos++] = 0x00;

			btTxBuf[nTxPos++] = 0xA0; // 예약영역, XGK,XGI,XGR

			btTxBuf[nTxPos++] = 0x33;

			btTxBuf[nTxPos++] = 0x00; // Invoke ID
			btTxBuf[nTxPos++] = 0x00;

			btTxBuf[nTxPos++] = 0x00; // Length(응용명령어 + 변수명 + 데이터길이 + 데이터)
			btTxBuf[nTxPos++] = 0x00;

			btTxBuf[nTxPos++] = 0x01; // FEnet Position

			btTxBuf[nTxPos++] = (byte)GetChecksum(btTxBuf, 0, nTxPos); // BCC(CheckSum)

			// 응용명령어
			btTxBuf[nTxPos++] = (byte)(chRw == 'W' ? 0x58 : 0x54); // 쓰기
			btTxBuf[nTxPos++] = 0x00;

			byte btDataType = 0x00;
			switch (strVar[2]) // 데이터
			{
				default:
				case 'X':
					btDataType = 0x00;
					break;
				case 'B':
					btDataType = 0x01;
					break;
				case 'W':
					btDataType = 0x02;
					break;

				case 'D':
					btDataType = 0x03;
					break;
			}

			btTxBuf[nTxPos++] = btDataType; // Bit(0), Byte(1), Word(2). Dword(3), LDword(4), Continue(0x14)
			btTxBuf[nTxPos++] = 0x00;

			btTxBuf[nTxPos++] = 0x00; // Reserved
			btTxBuf[nTxPos++] = 0x00;

			btTxBuf[nTxPos++] = (byte)nBlockCnt; // 블록갯수
			btTxBuf[nTxPos++] = (byte)(nBlockCnt>>8);

			btTxBuf[nTxPos++] = (byte)strVar.Length; // 변수길이
			btTxBuf[nTxPos++] = 0x00;

			// 어드레스
			for(i=0;i< strVar.Length;i++)
            {
				btTxBuf[nTxPos++] = (byte)strVar[i];
			}

			if(chRw == 'W')
            {
				btTxBuf[nTxPos++] = (byte)nDataLen; // 데이터 길이
				btTxBuf[nTxPos++] = (byte)(nDataLen >> 8);

				switch (strVar[2]) // 데이터
				{
					default:
					case 'B':
					case 'X': 
							btTxBuf[nTxPos++] = (byte)(dwData);
							break;
					case 'W':
							btTxBuf[nTxPos++] = (byte)(dwData);
							btTxBuf[nTxPos++] = (byte)(dwData>>8);
							break;

					case 'D':
							btTxBuf[nTxPos++] = (byte)(dwData);
							btTxBuf[nTxPos++] = (byte)(dwData >> 8);
							btTxBuf[nTxPos++] = (byte)(dwData >> 16);
							btTxBuf[nTxPos++] = (byte)(dwData >> 24);
						break;
				}
			}

			btTxBuf[16] = (byte)(nTxPos - 20);

            try
            {
				sckPlcUdp.BeginSend(btTxBuf, nTxPos, epRemote, new AsyncCallback(SendCallback), null);
				//sckPlcUdp.Send(btTxBuf, nTxPos, epRemote);
			}
            catch
            {

            }
		}

		public static void SendCallback(IAsyncResult iar)
		{
		}

		//           
		public void ReiceveMessage()
		{
			try
			{
				sckPlcUdp.BeginReceive(new AsyncCallback(ReceiveCallback), null);
			}
			catch
            {

            }
		}

		private void ReceiveCallback(IAsyncResult iar)
		{
			IPEndPoint epRemote = new IPEndPoint(IPAddress.Parse(strIpAddr), nPortNo);

			try
            {
				byte[] btBuf = sckPlcUdp.EndReceive(iar, ref epRemote);

				if (btBuf == null)
				{
					Array.Resize(ref btRxBuf, 0);
				}
				else
				{
					if (btBuf.Length == 0)
					{
						Array.Resize(ref btRxBuf, 0);
					}
					else
					{
						Array.Resize(ref btRxBuf, btBuf.Length);
						Array.Copy(btBuf, 0, btRxBuf, 0, btRxBuf.Length);
					}
				}
            }
            catch
            {

            }
		}

		//
		public int GetDI(int nModule, int nPin)
		{
			if (nPin >= 16) return 0;

			nPin = 15 - nPin;

            int nByteIndex = (nPin / 8)  + (nModule);
			int nBitIndex = 7 - (nPin % 8);

			byte cShift = 0x01;
			cShift <<= nBitIndex;
			int ret = ((m_bDIBuffer[nByteIndex] & cShift) > 0) ? 1 : 0;

			//Console.WriteLine("{0}", nByteIndex);

			return ret;
		}

		//
		public void SetDO(int nModule, int nPin, int nValue)
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
			/*
			int nByteIndex = nPin / 8 + (nModule * 8);
			int nBitIndex = nPin % 8;

			byte cShift = 0x01;
			cShift <<= nBitIndex;
			int ret = ((m_bDOBuffer[nByteIndex] & cShift) > 0) ? 1 : 0;
			*/

			int ret = m_bDOFbValue[nPin] == true ? 1 : 0;
            return ret;
		}

		//
		public void SetAO(int nModule, int nPin, int nValue)
        {
			nValue = nValue * 400;
            m_nAOBuffer[nModule * m_nAOPin + nPin] = (ushort)nValue;
		}
        public void SetAO_double(int nModule, int nPin, double dValue)
        {
            dValue = dValue * 400;
            m_nAOBuffer[nModule * m_nAOPin + nPin] = (ushort)dValue;
        }

        //
        public int GetAO(int nModule, int nPin)
		{
			return (int)m_nAOBuffer[nModule * m_nAIPin + nPin];
		}       
		
		//
		// 응답된 데이터 수신 후 처리
		// chDataType : 'X' : BIT, 'B' : BYTE, 'W' : WORD, 'D' : DBWORD
		// return
		//  데이터가 없으면 오류
		//  읽기 : 읽은 데이터를 uint 배열 형으로 리턴
		//  쓰기 : 정상적으로 쓰여졌으면 '1'
		public uint[] Answer(char chDataType)
        {
			int i;
            byte[] btBuf = new byte[64];
            byte[] btBuf2 = new byte[64];
            ushort nBlockCnt;
			ushort nDataLen;
			uint[] dwDataBuf = new uint[512];
			ushort nDataType;

			if (btRxBuf == null) return null;
			if (btRxBuf.Length < 10) return null;

			byte[] btCmpBuf = new byte[btCompanyIdBuf.Length];
			Buffer.BlockCopy(btRxBuf, 0, btCmpBuf, 0, btCompanyIdBuf.Length);

			if (!btCmpBuf.SequenceEqual(btCompanyIdBuf))
			{
				return null;
			}

			nDataType = 0;
			switch (chDataType)
            {
				default:
				case 'X':
				case 'B':
					nDataType = 0x0000;
					break;
				case 'W':
					nDataType = 0x0002;
					break;
				case 'D':
					nDataType = 0x0003;
					break;
				case 'L':
					nDataType = 0x0004;
					break;
			}

			Array.Copy(btRxBuf, 20, btBuf, 0, 2);
			if (BitConverter.ToUInt16(btBuf,0) == 0x0055) // 읽기 응답
            {
				Array.Copy(btRxBuf, 22, btBuf, 0, 2);
				if (BitConverter.ToUInt16(btBuf,0) == nDataType) // Data Type
                {
					Array.Copy(btRxBuf, 26, btBuf, 0, 2);
					if (BitConverter.ToUInt16(btBuf,0) == 0x0000) // 0 : 정상, 0 아니면 에러
                    {
						Array.Copy(btRxBuf, 28, btBuf, 0, 2);
						nBlockCnt = BitConverter.ToUInt16(btBuf,0);

						for (i = 0; i < (int)nBlockCnt; i++)
                        {
							try
							{
								Array.Copy(btRxBuf, 30 + (i * 4), btBuf, 0, 2);
								nDataLen = BitConverter.ToUInt16(btBuf, 0);
								Array.Copy(btRxBuf, 32 + (i * 4), btBuf2, 0, nDataLen);
								dwDataBuf[i] = BitConverter.ToUInt16(btBuf2,0);
							}
							catch
							{
								Console.WriteLine("PLC Error");
							}
						}

                        Array.Resize(ref dwDataBuf, i);

						return dwDataBuf;
					}
				}
			}
			else
			if (BitConverter.ToUInt16(btBuf,0) == 0x0059) // 쓰기 응답
            {
				Array.Copy(btRxBuf, 22, btBuf, 0, 2);
				if (BitConverter.ToUInt16(btBuf, 0) == nDataType) // Data Type
				{
					Array.Copy(btRxBuf, 26, btBuf, 0, 2);
					if (BitConverter.ToUInt16(btBuf, 0) == 0x0000) // 0 : 정상, 0 아니면 에러
					{
						Array.Resize(ref dwDataBuf, 1);
						dwDataBuf[0] = 0x0001;
						return dwDataBuf;
					}
				}
				return null;
			}
            else
            {
				return null;
			}

			return null;
		}

		// Receive thread
		private void ThreadRunFunc(object objClass)
		{
			int i, k, n;
			//int nRxLen;
			//string strBuf;
			//byte[] btRxBuf = new byte[1024];
			//byte[] btTxBuf = new byte[1024];
			//char[] szBuf = new char[256];
			//ushort nFwBufLength;
			//uint nFwBufOffset;
			//uint dwFlashAddr;
			ushort wTemp;
			//int nTxPos;
			//int nMaxTimer;
			//byte btTemp;

			k = 0;
			n = 0;
			wTemp = 0x0000;

			try
			{
				while (bThreadRunStop)
				{
					threadEvent.WaitOne(Timeout.Infinite);

					// P 영역 : 최종 입출력, M : 메모리 영역
					// PLC로 DI입력
					ReadWrite('R', 1, "%PW000", 0x00, 0x00);
					Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBuf0 = Answer('W');

					if (dwPlcRxBuf0 != null)
					{						
                        m_bDIBuffer[0] = (byte)(dwPlcRxBuf0[0] >> 8);
                        m_bDIBuffer[1] = (byte)(dwPlcRxBuf0[0] & 0xFF);
                        //Console.WriteLine("DI0:{0:X}", dwPlcRxBuf0[0]);
                    }
					else
					{
					}

                    // P 영역 : 최종 입출력, M : 메모리 영역
                    // PLC로 DI입력
                    ReadWrite('R', 1, "%PW008", 0x00, 0x00);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBuf8 = Answer('W');

                    if (dwPlcRxBuf8 != null)
                    {
                        m_bDIBuffer[2] = (byte)(dwPlcRxBuf8[0] >> 8);
                        m_bDIBuffer[3] = (byte)(dwPlcRxBuf8[0] & 0xFF);
                        //Console.WriteLine("DI1:{0:X}", dwPlcRxBuf8[0]);
                    }
                    else
                    {
                    }

                    // P 영역 : 최종 입출력, M : 메모리 영역
                    // PLC로 DODB입력
                    ReadWrite('R', 1, "%PW002", 0x00, 0x00);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcFbRxBuf2 = Answer('W');

                    if (dwPlcFbRxBuf2 != null)
                    {
						for (i = 0; i < 16; i++)
						{
							m_bDOFbValue[0] = (((uint)dwPlcFbRxBuf2[0] & (uint)(0x0001 << i)) > 0) ? true : false;
                        }
                        //Console.WriteLine("DOFB2:{0:X}", dwPlcFbRxBuf2[0]);
                    }
                    else
                    {
                    }

                    ReadWrite('R', 1, "%PW0012", 0x00, 0x00);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcFbRxBuf12 = Answer('W');

                    if (dwPlcFbRxBuf12 != null)
                    {
                        for (i = 0; i < 16; i++)
                        {
                            m_bDOFbValue[i+16] = (((uint)dwPlcFbRxBuf12[0] & (uint)(0x0001 << i)) > 0) ? true : false;
                        }
                        //Console.WriteLine("DOFB12:{0:X}", dwPlcFbRxBuf12[0]);
                    }
                    else
                    {
                    }

                    ReadWrite('R', 1, "%PW013", 0x00, 0x00);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcFbRxBuf13 = Answer('W');

                    if (dwPlcFbRxBuf13 != null)
                    {
                        for (i = 0; i < 16; i++)
                        {
                            m_bDOFbValue[i + 16*2] = (((uint)dwPlcFbRxBuf13[0] & (uint)(0x0001 << i)) > 0) ? true : false;
                        }
                        //Console.WriteLine("DOFB13:{0:X}", dwPlcFbRxBuf13[0]);
                    }
                    else
                    {
                    }

                    ReadWrite('R', 1, "%PW016", 0x00, 0x00);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcFbRxBuf16 = Answer('W');

                    if (dwPlcFbRxBuf16 != null)
                    {
                        for (i = 0; i < 16; i++)
                        {
                            m_bDOFbValue[i + 16 * 3] = (((uint)dwPlcFbRxBuf16[0] & (uint)(0x0001 << i)) > 0) ? true : false;
                        }
                        //Console.WriteLine("DOFB16:{0:X}", dwPlcFbRxBuf16[0]);
                    }
                    else
                    {
                    }

                    ReadWrite('R', 1, "%PW017", 0x00, 0x00);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcFbRxBuf17 = Answer('W');

                    if (dwPlcFbRxBuf17 != null)
                    {
                        for (i = 0; i < 16; i++)
                        {
                            m_bDOFbValue[i + 16 * 4] = (((uint)dwPlcFbRxBuf17[0] & (uint)(0x0001 << i)) > 0) ? true : false;
                        }
                        //Console.WriteLine("DOFB17:{0:X}", dwPlcFbRxBuf17[0]);
                    }
                    else
                    {
                    }

                    // PLC로 'D'영역 쓰고 읽기
                    /*
					wTemp++;
					ReadWrite('W', 1, "%DW100", 2, (uint)wTemp);
					Thread.Sleep(100);
					ReiceveMessage();
					Thread.Sleep(100);

					ReadWrite('R', 1, "%DW100", 0x00, 0x00);
					ReiceveMessage();
					Thread.Sleep(100);
					uint[] dwPlcRxBuf10 = Answer('W');

					if (dwPlcRxBuf10 != null)
					{
						Console.WriteLine("{0:D}", dwPlcRxBuf10[0]);
					}
					else
					{
					}
					*/

                    // PLC로 DO출력
                    //SetDO(0, 0, 1);
                    wTemp = 0x000;
					for (i = 0; i < 16; i++)
					{
						wTemp |= (ushort)(m_bDOValue[i+16*0] ? 0x0001 << i : 0x0000);
					};
					//wTemp = (ushort)(k++);
					ReadWrite('W', 1, "%PW002", 2, (uint)wTemp);
					Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBuf1 = Answer('W');
					if (dwPlcRxBuf1 != null)
					{
						if (dwPlcRxBuf1.Length > 0)
						{
						}
						else
						{
						}
					}
					else
					{
					}

					wTemp = 0x000;
					for (i = 0; i < 16; i++)
					{
						wTemp |= (ushort)(m_bDOValue[i+16*1] ? 0x0001 << i : 0x0000);
					}; 
					//wTemp = (ushort)(k++);
					ReadWrite('W', 1, "%PW012", 2, (uint)wTemp);
					Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBuf2 = Answer('W');
					if (dwPlcRxBuf2 != null)
					{
						if (dwPlcRxBuf2.Length > 0)
						{
						}
						else
						{
						}
					}
					else
					{
					}
					
					wTemp = 0x000;
					for (i = 0; i < 16; i++)
					{
						wTemp |= (ushort)(m_bDOValue[i+16*2] ? 0x0001 << i : 0x0000);
					}; 
					//wTemp = (ushort)(k++);
					ReadWrite('W', 1, "%PW013", 2, (uint)wTemp);
					Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBuf3 = Answer('W');
					if (dwPlcRxBuf3 != null)
					{
						if (dwPlcRxBuf3.Length > 0)
						{
						}
						else
						{
						}
					}
					else
					{
					}

                    wTemp = 0x000;
                    for (i = 0; i < 16; i++)
                    {
                        wTemp |= (ushort)(m_bDOValue[i + 16 * 3] ? 0x0001 << i : 0x0000);
                    }
                    ;
                    //wTemp = (ushort)(k++);
                    ReadWrite('W', 1, "%PW016", 2, (uint)wTemp);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBuf4 = Answer('W');
                    if (dwPlcRxBuf4 != null)
                    {
                        if (dwPlcRxBuf4.Length > 0)
                        {
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                    }

                    wTemp = 0x000;
                    for (i = 0; i < 16; i++)
                    {
                        wTemp |= (ushort)(m_bDOValue[i + 16 * 4] ? 0x0001 << i : 0x0000);
                    }
                    ;
                    //wTemp = (ushort)(k++);
                    ReadWrite('W', 1, "%PW017", 2, (uint)wTemp);
                    Thread.Sleep(5);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBuf5 = Answer('W');
                    if (dwPlcRxBuf5 != null)
                    {
                        if (dwPlcRxBuf5.Length > 0)
                        {
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                    }

                    // AO
                    for (i=0; i<4;i++)
					{
                        wTemp = (ushort)m_nAOBuffer[i];
						//wTemp = (ushort)n;
                        ReadWrite('W', 1, string.Format("%DW{0}",100+i), 2, (uint)wTemp);
                        //ReadWrite('W', 1, "%DW100", 2, (uint)2000);
                        Thread.Sleep(5);
                        ReiceveMessage();
                        ReiceveMessage();
                        uint[] dwPlcRxBufAO = Answer('W');
                        if (dwPlcRxBufAO != null)
                        {
                            if (dwPlcRxBufAO.Length > 0)
                            {
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                        }
                    }
					n += 10;
					/*
                    // AO OPEN
                    wTemp = 0x000F;
                    
                    //wTemp = (ushort)(k++);
                    ReadWrite('W', 1, "%UW0.4.2", 2, (uint)wTemp);
                    Thread.Sleep(100);
                    ReiceveMessage();
                    ReiceveMessage();
                    uint[] dwPlcRxBufAO_OPEN = Answer('W');
                    if (dwPlcRxBufAO_OPEN != null)
                    {
                        if (dwPlcRxBufAO_OPEN.Length > 0)
                        {
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                    }
					*/
                }
			}
			catch (ThreadInterruptedException)
			{

			}
		}
	}
}
