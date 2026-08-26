using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace NetworkService
{
    public class SerialClient
    {
        byte bNewSecond = 0;
        byte bOldSecond = 0;
        int nCounter = 0;

        const int DATA_BUFFER_SIZE = 4096;

        SerialPort m_Serial;

        private string m_strPort = "COM1";
        public string PORT
        {
            get { return m_strPort; }
            set { m_strPort = value; m_Serial.PortName = value; }
        }

        private int m_nBaudrate = 9600;
        public int BAUDRATE
        {
            get { return m_nBaudrate; }
            set { m_nBaudrate = value; m_Serial.BaudRate = 9600; }
        }

        private bool m_bDTREnable = false;
        public bool DTR
        {
            get { return m_bDTREnable; }
            set { m_bDTREnable = value; m_Serial.DtrEnable = value; }
        }

        private bool m_bConnected = false;
        public bool CONNECTED
        {
            get { return m_bConnected; }
            set { m_bConnected = value; }
        }

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

        private const int SLEEP_TIME = 10;
        private int m_nRetryCount = 10;
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


                /// <summary>
        /// 
        /// </summary>
        /// 
        public SerialClient()
        {
            m_Serial = new SerialPort();
        }

        public void SerialRxBufClear()
        {
            try
            {
                m_Serial.DiscardInBuffer();
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Connect()
        {
            try
            {
                m_Serial.Open();
            }
            catch { }
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
                if (m_Serial.IsOpen == true)
                {
                    m_bConnected = m_Serial.IsOpen;
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
        /// <param name="strData"></param>
        /// 
        public void SendData(string strData)
        {
            m_strReturnValue = "";

            try
            {
                if(m_Serial.IsOpen)
                {
                    m_Serial.Write(strData);
                }
            }
            catch 
            {
                Disconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        unsafe public int ReceiveData_string(byte[] btRxBuf, int nLength)
        {
            int nRxLen = 0;
            try
            {
                if (m_Serial != null) nRxLen = m_Serial.Read(btRxBuf, 0, nLength);
            }
            catch
            {
                nRxLen = 0;
            }

            return nRxLen;
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
                if (m_Serial.BytesToRead > 0)
                {
                    m_strReturnValue += m_Serial.ReadExisting();
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

        public int RecieveData(byte[] btRxBuf, int nLength)
        {
            int nRxLen = 0;
            try
            {
                if (m_Serial != null) nRxLen = m_Serial.Read(btRxBuf, 0, nLength);
            }
            catch
            {
                nRxLen = 0;
            }

            return nRxLen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Disconnect()
        {
            m_bConnected = false;
            try
            {
                m_Serial.Close();
            } 
            catch {}
        }

        public short SendSMVB(string strWordData)
        {
            int nTxPos = 0;
            byte[] btBuf = new byte[1024];
            byte[] btTempBuf = new byte[1024];
            string[] strSendData = new string[3];
            ushort wBccBuf;

            SendData(string.Format("set.port 4010 0000000000000000{0}\n\r", strWordData));

            

            return 1;
        }

        public short SendSdrToDcu(string strWordData)
        {
            int nTxPos = 0;
            byte[] btBuf = new byte[1024];
            byte[] btTempBuf = new byte[1024];
            string[] strSendData = new string[3];
            ushort wBccBuf;

            nTxPos = 0;

            Array.Clear(btBuf, 0x00, btBuf.Length);

            //word1
            btBuf[nTxPos++] = to_BCD(DateTime.Now.Month);//
            btBuf[nTxPos++] = to_BCD(DateTime.Now.Year % 100);
            //word2
            btBuf[nTxPos++] = to_BCD(DateTime.Now.Hour);
            btBuf[nTxPos++] = to_BCD(DateTime.Now.Day);
            //word3
            btBuf[nTxPos++] = to_BCD(DateTime.Now.Second);
            bNewSecond = to_BCD(DateTime.Now.Second);

            btBuf[nTxPos++] = to_BCD(DateTime.Now.Minute);

            for (int i = 0; i < strSendData.Length; i++)
            {
                for (int p = 0; p < 2; p++)
                {
                    strSendData[i] += btBuf[(i * 2) + p].ToString("X2");
                }
            }

            if (bNewSecond != bOldSecond)
            {
                SendData(string.Format("set.port 4010 0000{0}{1}{2}{3}\n\r", strSendData[0], strSendData[1], strSendData[2], strWordData));
                Console.WriteLine(string.Format("[{0}]set.port 4010 0000{1}{2}{3}{4}", nCounter, strSendData[0], strSendData[1], strSendData[2], strWordData));

                nCounter++;
                bOldSecond = bNewSecond;
            }
            else
            {
                nCounter = 0;
            }

            if (false)
            {
                SendData(string.Format("set.port 30A8 00004646464646464646000000000064\n\r"));
            }





            return 1;
        }

        byte to_BCD(int n)
        {
            // extract each digit from the input number n
            byte d1 = (byte)(n / 10);
            byte d2 = (byte)(n % 10);
            // combine the decimal digits into a BCD number
            return (byte)((d1 << 4) | d2);
        }
    }
}
