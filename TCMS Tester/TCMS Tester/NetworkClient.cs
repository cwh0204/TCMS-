using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;

namespace NetworkService
{
    public class NetworkClient
    {
        const int DATA_BUFFER_SIZE = 4096*4;

        private string m_strIP = "127.0.0.1";
        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        private int m_nPort = 26950;
        public int PORT
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        private bool m_bConnected = false;
        public bool CONNECTED
        {
            get { return m_bConnected; }
            set { m_bConnected = value; }
        }

        private TcpClient m_socket;
        private NetworkStream m_stream;
        private byte[] m_receiveBuffer;
        public int m_nReceivedData = 0;
        public byte[] m_processBuffer;

        private string m_strDelimitor = "\r\n";
        public string DELIMITOR
        {
            get { return m_strDelimitor; }
            set { m_strDelimitor = value; }
        }

        private bool m_bIsReturn = false;
        public bool IS_RETURN
        {
            get { return m_bIsReturn; }
            set { m_bIsReturn = value; }
        }

        private string m_strReturnValue = "";
        public string RETURN_VALUE
        {
            get { return m_strReturnValue; }
            set { m_strReturnValue = value; }
        }

        private const int SLEEP_TIME = 50;
        private int m_nRetryCount = 200;
        private int m_nTimeOut = 1000;
        public int TIMEOUT
        {
            get { return m_nTimeOut; }
            set 
            { 
                m_nTimeOut = value; 
                m_nRetryCount = m_nTimeOut / SLEEP_TIME;  
            }
        }

        bool m_bRecvBinaryHeader = false;
        int m_nBinaryHeaderSize = 0;
        int m_nBinaryDataSize = 0;
        string m_strBinaryDataSize = "";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public NetworkClient()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Connect()
        {
            m_socket = new TcpClient
            {
                ReceiveBufferSize = DATA_BUFFER_SIZE,
                SendBufferSize = DATA_BUFFER_SIZE
            };

            m_nReceivedData = 0;
            m_receiveBuffer = new byte[DATA_BUFFER_SIZE];
            m_processBuffer = new byte[DATA_BUFFER_SIZE];
            try
            {
                m_socket.BeginConnect(m_strIP, m_nPort, ConnectCallback, m_socket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_result"></param>
        /// 
        private void ConnectCallback(IAsyncResult _result)
        {
            try
            {
                m_socket.EndConnect(_result);
            }
            catch
            {
            }

            if (!m_socket.Connected)
            {
                return;
            }

            m_bConnected = true;
            m_stream = m_socket.GetStream();
            m_stream.BeginRead(m_receiveBuffer, 0, DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool CheckConnect()
        {
            int nRetry = 0;
            while (true)
            {
                if (m_bConnected == true)
                {
                    break;
                }

                Thread.Sleep(SLEEP_TIME);
                ++nRetry;

                if (nRetry >= m_nRetryCount)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_result"></param>
        /// 
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = m_stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Disconnect();
                    return;
                }

                Array.Copy(m_receiveBuffer, 0, m_processBuffer, m_nReceivedData, _byteLength);
                m_nReceivedData += _byteLength;
                
                m_stream.BeginRead(m_receiveBuffer, 0, DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch //(Exception _ex)
            {
                //Console.WriteLine($"Error receiving TCP data : {_ex}");
                Disconnect();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strData"></param>
        /// 
        public void SendData(string strData)
        {
            m_bRecvBinaryHeader = false;
            m_nBinaryHeaderSize = 0;
            m_nBinaryDataSize = 0;
            m_strBinaryDataSize = "";

            m_nReceivedData = 0;
            if( m_processBuffer != null )
                Array.Clear(m_processBuffer, 0, m_processBuffer.Length-1);

            try
            {
                if(m_socket != null)
                {
                    m_stream.BeginWrite(Encoding.UTF8.GetBytes(strData), 0, strData.Length, null, null);
                }
            }
            catch //(Exception _ex)
            {
                //Debug.Log($"Error sending data to server via TCP: {_ex}");
                Disconnect();
            }
        }

        public void SetReceiveBufferSize(int nSize)
        {
            m_processBuffer = new byte[nSize];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool ReceiveData()
        {
            int nRetry = 0;
            while (true)
            {
                if (m_nReceivedData > 0)
                {
                    m_processBuffer[m_nReceivedData] = 0x00;
                    m_processBuffer[m_nReceivedData+1] = 0x00;

                    m_strReturnValue = "";
                    m_strReturnValue = Encoding.Default.GetString(m_processBuffer);
                    if (m_strReturnValue.IndexOf(m_strDelimitor) >= 0)
                    {
                        m_bIsReturn = true;
                        break;
                    }
                }

                Thread.Sleep(SLEEP_TIME);
                ++nRetry;

                if (nRetry >= m_nRetryCount)
                {
                    return false;
                }
            }
 
            return true;
        }


        public bool ReceiveBinaryData()
        {
            int nRetry = 0;
            while (true)
            {
                if (m_nBinaryHeaderSize == 0 )
                {
                    if( m_nReceivedData >= 2)
                        m_nBinaryHeaderSize = m_processBuffer[1] - 48;

                    m_strBinaryDataSize = "";
                }
                else
                {
                    if (m_nBinaryDataSize == 0)
                    {
                        if (m_nReceivedData >= (m_nBinaryHeaderSize + 2))
                        {
                            for (int i = 0; i < m_nBinaryHeaderSize; ++i)
                            {
                                m_strBinaryDataSize += (m_processBuffer[2 + i] - 48).ToString();
                            }
                            try
                            {
                                m_nBinaryDataSize = int.Parse(m_strBinaryDataSize);
                                m_bRecvBinaryHeader = true;
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if( m_bRecvBinaryHeader == true )
                {
                    if( m_nReceivedData >= (m_nBinaryDataSize + m_nBinaryHeaderSize + 2) )
                    {
                        m_bIsReturn = true;
                        break;
                    }
                }

                Thread.Sleep(SLEEP_TIME);
                ++nRetry;

                if (nRetry >= m_nRetryCount)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Disconnect()
        {
            try
            {
                m_socket.Close();
            }
            catch
            {
            }

            m_bConnected = false;

            m_stream = null;
            m_receiveBuffer = null;
            m_processBuffer = null;
            m_socket = null;
        }
    }
}
