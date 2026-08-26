using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace CITester
{
	class classSerialCommPacket
	{
		public const byte STX = 0x02;
		public const byte ETX = 0x03;
		public const byte DLE = 0x10;
		public const byte GP_ESC = 0x1B;

        byte bNewSecond = 0;
        byte bOldSecond = 0;
		int nCounter = 0;

        public SerialPort serialPort = null;

		private ushort[] m_wCcittTable = {
			0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
			0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
			0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
			0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
			0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
			0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
			0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
			0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
			0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
			0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
			0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
			0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
			0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
			0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
			0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
			0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
			0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
			0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
			0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
			0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
			0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
			0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
			0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
			0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
			0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
			0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
			0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
			0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
			0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
			0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
			0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
			0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0,
		};

        private byte[] m_btModBufCrcHi =
		{
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
			0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
			0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
			0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
			0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
			0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40
		};

        private byte[] m_btModBufCrcLo =
        {
			0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
			0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
			0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
			0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
			0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
			0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
			0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
			0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
			0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
			0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
			0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
			0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
			0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
			0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
			0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
			0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
			0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
			0x40
		};

        bool IS_HEXASCCODE(char x)
		{
			return (((x) >= '0' && (x) <= '9') || ((x) >= 'A' && (x) <= 'F'));
		}

		public classSerialCommPacket(SerialPort spHandle)
		{
			serialPort = spHandle;
		}

		public ushort GetCcittCal(byte[] btBuf, int nLen)
		{
			int i;
			ushort wCrc;

			wCrc = 0xFFFF;
			for (i = 0; i < nLen; i++)
			{
				wCrc = (ushort)((ushort)(wCrc << 8) ^ m_wCcittTable[((wCrc >> 8) ^ btBuf[i]) & 0x00ff]);
			}

			return (ushort)((ushort)wCrc & (ushort)0xFFFF);
		}

        public ushort GetModebusCrc16(byte []pDatBuf, int nDataLen)
        {
			int i;
            byte btCrcHi = 0xFF; // high byte of CRC initialized
            byte btCrcLo = 0xFF; // low byte of CRC initialized
            ushort wIndex; // will index into CRC lookup table

            for (i=0;i<nDataLen;i++) // pass through message buffer
            {
                wIndex = (ushort)((btCrcHi ^ pDatBuf[i]) & 0xff); // calculate the CRC
                btCrcHi = (byte)(btCrcLo ^ m_btModBufCrcHi[(int)wIndex]);
                btCrcLo = (byte)(m_btModBufCrcLo[(int)wIndex]);
            }

            return (ushort)(((ushort)((ushort)btCrcHi << 8) | (ushort)btCrcLo) & 0xffff);
        }

        //	HEX to ASC
        char main_ConvHex2Asc(byte btCh)
		{
			char chBuf = '0';
			if ((short)btCh >= 0 && btCh <= 9) chBuf = (char)((char)btCh + '0');
			else if (btCh >= 10 && btCh <= 15) chBuf = (char)(((int)btCh - 10) + 'A');
			return chBuf;
		}

		//public fixed char szBuf[128];
		unsafe public short SendPacket(byte btCode, byte[] btTxBuf, int nTxLen)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			char[] szBuf = new char[1024];
			ushort wCrcBuf;

			btBuf[nTxPos++] = btCode;
			btBuf[nTxPos++] = (byte)nTxLen;

			for (int i = 0; i < nTxLen; i++)
			{
				btBuf[nTxPos++] = btTxBuf[i];
			}

			wCrcBuf = GetCcittCal(btBuf, nTxPos);

			btBuf[nTxPos++] = (byte)(wCrcBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wCrcBuf >> 8) & 0xFF);

			nTxLen = ControllerHexToAscPacket(szBuf, btBuf, nTxPos);
			SendData(szBuf, nTxLen);

			return 1;
		}

        unsafe public short SendPacketMODBUS(byte btCode, byte[] btTxBuf, int nTxLen)
        {
            int nTxPos = 0;
            byte[] btBuf = new byte[1024];
            char[] szBuf = new char[1024];
            ushort wCrcBuf;

            btBuf[nTxPos++] = btCode;
            //btBuf[nTxPos++] = (byte)nTxLen;

            for (int i = 0; i < nTxLen; i++)
            {
                btBuf[nTxPos++] = btTxBuf[i];
            }

            wCrcBuf = GetModebusCrc16(btBuf, nTxPos);

       
            btBuf[nTxPos++] = (byte)((wCrcBuf >> 8) & 0xFF);
            btBuf[nTxPos++] = (byte)(wCrcBuf & 0xFF);

            SendData(btBuf, nTxPos);

            return 1;
        }


        // Making packeting & unpacketing
        unsafe public short ControllerHexToAscPacket(char[] pDistBuf, byte[] pSrcBuf, int nLen)
		{
			int i;
			int nTxPos;

			nTxPos = 0;
			pDistBuf[nTxPos++] = (char)STX;

			for (i = 0; i < nLen; i++)
			{
				pDistBuf[nTxPos++] = main_ConvHex2Asc((byte)((pSrcBuf[i] >> 4) & 0x0F));
				pDistBuf[nTxPos++] = main_ConvHex2Asc((byte)(pSrcBuf[i] & 0x0F));
			}

			pDistBuf[nTxPos++] = (char)ETX;

			return (short)nTxPos;
		}

		unsafe public short ControllerAscToHexPacket(byte[] btDistBuf, char[] btSrcBuf, int nLen)
		{
			int i;
			int nTxPos;

			nTxPos = 0;

			for (i = 0; i < nLen; i += 2)
			{
				if (!IS_HEXASCCODE(btSrcBuf[i]) || !IS_HEXASCCODE(btSrcBuf[i + 1])) return -1;
				btDistBuf[nTxPos++] = (byte)((ConvAsc2Hex(btSrcBuf[i]) << 4) | ConvAsc2Hex(btSrcBuf[i + 1]));
			}

			return (short)nTxPos;
		}

		// DU 화면변경
		unsafe public int DuScreenChange(ushort nCode)
		{
			int nTxPos;
			byte[] btBuf = new byte[1024];

			nTxPos = 0;
			btBuf[nTxPos++] = GP_ESC;
			btBuf[nTxPos++] = (byte)'W';
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x0F;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x01;
			btBuf[nTxPos++] = (byte)((nCode >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(nCode & 0xFF);

			try
			{
				if (serialPort != null) serialPort.Write(btBuf, 0, nTxPos);
			}
			catch
			{

			}

			return 1;
		}

		// DU에서 n개 워드 읽기
		unsafe public int DuReadWords(ushort wAddr,ushort nLen)
		{
			int nTxPos;
			byte[] btBuf = new byte[1024];

			nTxPos = 0;
			btBuf[nTxPos++] = GP_ESC;
			btBuf[nTxPos++] = (byte)'R';
			btBuf[nTxPos++] = (byte)((wAddr >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(wAddr & 0xFF);
			btBuf[nTxPos++] = (byte)((nLen >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(nLen & 0xFF);

			try
			{
				if (serialPort != null) serialPort.Write(btBuf, 0, nTxPos);
			}
			catch
			{

			}

			return 1;
		}

		// DU에서 한 워드 쓰기
		unsafe public int DuWriteWord(ushort wAddr, ushort nData)
		{
			int nTxPos;
			byte[] btBuf = new byte[1024];

			nTxPos = 0;
			btBuf[nTxPos++] = GP_ESC;
			btBuf[nTxPos++] = (byte)'W';
			btBuf[nTxPos++] = (byte)((wAddr >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(wAddr & 0xFF);
			btBuf[nTxPos++] = (byte)(0x00);
			btBuf[nTxPos++] = (byte)(0x01);
			btBuf[nTxPos++] = (byte)((nData >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(nData & 0xFF);

			try
			{
				if (serialPort != null) serialPort.Write(btBuf, 0, nTxPos);
			}
			catch
			{

			}

			return 1;
		}

		// TGIS로 'I'(버튼) 쓰기
		unsafe public int DuButtonWriteWord(byte nLen,byte nData)
		{
			int nTxPos;
			byte[] btBuf = new byte[1024];

			nTxPos = 0;
			btBuf[nTxPos++] = GP_ESC;
			btBuf[nTxPos++] = (byte)'I';
			btBuf[nTxPos++] = (byte)nLen;
			btBuf[nTxPos++] = (byte)nData;

			try
			{
				if (serialPort != null) serialPort.Write(btBuf, 0, nTxPos);
			}
			catch
			{

			}

			return 1;
		}       
		
		// DU에서 문자열 워드 쓰기
		unsafe public int DuWriteStr(ushort wAddr, string strDat)
		{
			int i;
			int nTxPos;
			ushort wLen = (ushort)((strDat.Length / 2) + 1);
			byte[] btBuf = new byte[1024];

			nTxPos = 0;
			btBuf[nTxPos++] = GP_ESC;
			btBuf[nTxPos++] = (byte)'W';
			btBuf[nTxPos++] = (byte)((wAddr >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(wAddr & 0xFF);
			btBuf[nTxPos++] = (byte)((wLen >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(wLen & 0xFF);

			for(i=0;i<wLen * 2;i++)
            {
				if (i < strDat.Length) btBuf[nTxPos++] = (byte)strDat[i]; else btBuf[nTxPos++] = 0x00;
			}

			try
			{
				if (serialPort != null) serialPort.Write(btBuf, 0, nTxPos);
			}
			catch
			{

			}

			return 1;
		}

		//	HEX to ASC
		public char ConvHex2Asc(byte btCh)
		{
			char chBuf = '0';
			if ((int)btCh >= 0 && (int)btCh <= 9) chBuf = (char)((char)btCh + '0');
			else if ((int)btCh >= 10 && (int)btCh <= 15) chBuf = (char)((btCh - 10) + 'A');
			return chBuf;
		}

		//	ASC to HEX
		public byte ConvAsc2Hex(char chDat)
		{
			byte nBuf = 0;
			if (chDat >= '0' && chDat <= '9') nBuf = (byte)(chDat - '0');
			else
			if (chDat >= 'a' && chDat <= 'f') nBuf = (byte)((chDat - 'a') + 10);
			else
			if (chDat >= 'A' && chDat <= 'F') nBuf = (byte)((chDat - 'A') + 10);

			return nBuf;
		}

		public int SerialBufferClear()
		{
			try
			{
				if (serialPort != null) serialPort.DiscardInBuffer(); else return -1;
			}
			catch
			{
				return -2;
			}

			return 1;
		}

		unsafe public int RecieveData(byte[] btRxBuf,int nLength)
		{
			int nRxLen = 0;
			try
			{
				if (serialPort != null) nRxLen = serialPort.Read(btRxBuf, 0, nLength);
			}
            catch
			{
				nRxLen = 0;
			}

			return nRxLen;
		}

		unsafe public int SendData(byte[] btTxBuf,int nTxLen)
		{
			if (nTxLen <= 0) return -1;

			try
			{
				if (serialPort != null) serialPort.Write(btTxBuf, 0, nTxLen);
			}
			catch
			{

			}

			return 1;
		}
        public void SendData(string strData)
        {
            try
            {
                if (serialPort != null) serialPort.Write(strData);
            }
            catch
            {

            }
        }

        unsafe public int SendData(char[] szTxBuf, int nTxLen)
		{
			if (nTxLen <= 0) return -1;

			try
			{
				if (serialPort != null) serialPort.Write(szTxBuf, 0, nTxLen);
			}
			catch
			{

			}

			return 1;
		}

		unsafe public int SendData(byte[] btTxBuf)
		{
			if (btTxBuf.Length <= 0) return -1;

			try
			{
				if (serialPort != null) serialPort.Write(btTxBuf, 0, btTxBuf.Length);
			}
			catch
			{

			}

			return 1;
		}

        unsafe public int DSP_SendData(byte[] btTxBuf, int nTxLen)
        {

            return 1;
        }

        //	BCC값을 계산하여 리턴한다.
        unsafe ushort GetBcc(byte[] pDat, int nLen)
		{
			int i;
			byte[] btBcc = new byte[2];
			btBcc[0] = btBcc[1] = 0x00;
			for (i = 0; i < nLen; i++) btBcc[i & 0x01] ^= pDat[i];
			return BitConverter.ToUInt16(btBcc,0);
		}

		//	BCC가 정상적으로 되었는가 검사
		//	return : OK : TRUE, NG : FALSE
		unsafe bool IsBccOk(byte[] pDat, int nLen)
		{
			int i;
			byte[] nBCC = new byte [2];

			nBCC[0] = nBCC[1] = 0x00;

			for (i = 0; i < nLen; i++) nBCC[(i & 0x01)] ^= pDat[i];

			if (nBCC[0] != pDat[nLen] || nBCC[1] != pDat[nLen + 1])
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		// DECODER 응답(CU, 0CH, A2(RS485+),C2(RS485-), 9600,NONE)
		// EX) "02 F0 F3 20 0A 16 44 00 00 00 00 00 00 00 00 03 [BCC]"
		unsafe public short SendSdToDecoder(byte btPwm, byte btDuty)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			btBuf[nTxPos++] = STX;
			
			btBuf[nTxPos++] = 0xF0;
			btBuf[nTxPos++] = 0xF3;
			btBuf[nTxPos++] = 0x20;
			
			btBuf[nTxPos++] = 0x0A;
			
			btBuf[nTxPos++] = btPwm;
			btBuf[nTxPos++] = btDuty;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = ETX;

			//ArraySegment<byte> arySeg = new ArraySegment<byte>(btBuf, 1, nTxPos - 1);
			//wBccBuf = GetBcc(arySeg.Array, nTxPos - 1);
			Buffer.BlockCopy(btBuf, 1, btTempBuf, 0, nTxPos - 1);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 1);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

	
		// ATP SDR (CU, 4,5CH, 38400,EVEN)
		// EX) ""
		unsafe public short SendSdrToAtp(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wCrcBuf;

			nTxPos = 0;

			btBuf[nTxPos++] = DLE;
			btBuf[nTxPos++] = STX;

			btBuf[nTxPos++] = 0x30;

			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = btCheckDat;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = DLE;
			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 0, btTempBuf, 0, nTxPos);
			wCrcBuf = GetCcittCal(btTempBuf, nTxPos);

			btBuf[nTxPos++] = (byte)((wCrcBuf >> 8) & 0xFF);
			btBuf[nTxPos++] = (byte)(wCrcBuf & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// PA 응답(CU, 2CH, {A10(RX),C10(G24),A11(TX),C11(P24)}, 38400,NONE)
		// EX) "02 20 00 00 00 00 00 00 00 00 00 00 00 00 03 [BCC]"
		unsafe public short SendSdToPa(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			btBuf[nTxPos++] = STX;

			btBuf[nTxPos++] = 0x20;

			btBuf[nTxPos++] = btCheckDat;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 1, btTempBuf, 0, nTxPos - 1);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 1);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// DCU 응답(TU, 1CH, {A2(RTX+),C2(RTX-)}, 38400,NONE)
		// EX) "AA BB CC 20 XX YY 00 00 00 00 00 00 03 [BCC]"
		unsafe public short SendSdToDcu(byte btCnt, byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			btBuf[nTxPos++] = 0xAA;
			btBuf[nTxPos++] = 0xBB;
			btBuf[nTxPos++] = 0xCC;

			btBuf[nTxPos++] = 0x20;

			btBuf[nTxPos++] = btCnt;
			btBuf[nTxPos++] = btCheckDat;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 3, btTempBuf, 0, nTxPos - 3);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 3);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// HVAC 응답(TU, 2CH, {A5(RTX+),C5(RTX-)}, 9600,NONE)
		// EX) "AA BB CC CC 09 0X 00 00 00 00 00 00 00 03 [BCC] "
		unsafe public short SendSdToHvac(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			btBuf[nTxPos++] = 0xAA;
			btBuf[nTxPos++] = 0xBB;
			btBuf[nTxPos++] = 0xCC;

			btBuf[nTxPos++] = 0xCC;
			btBuf[nTxPos++] = 0x09;

			btBuf[nTxPos++] = btCheckDat;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 3, btTempBuf, 0, nTxPos - 3);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 3);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// BMS 응답(TU, 5CH, {A18(RTX+),C18(RTX-)}, 9600,NONE)
		// EX) "02 CC 20 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 [BCC] "
		unsafe public short SendSdToBms(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			btBuf[nTxPos++] = STX;

			btBuf[nTxPos++] = 0xCC;
			btBuf[nTxPos++] = 0x20;
			btBuf[nTxPos++] = 0x30;

			btBuf[nTxPos++] = btCheckDat;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 1, btTempBuf, 0, nTxPos - 1);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 1);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// LTE-R 응답(TU, 6CH, {A20(RTX+),C20(RTX-)}, 38400,NONE)
		// EX) "02 CC 50 30 0X 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 [BCC] "
		unsafe public short SendSdToLteR(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			btBuf[nTxPos++] = STX;

			btBuf[nTxPos++] = 0xCC;
			btBuf[nTxPos++] = 0x50;
			btBuf[nTxPos++] = 0x30;

			btBuf[nTxPos++] = btCheckDat;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;
			btBuf[nTxPos++] = 0x00;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 1, btTempBuf, 0, nTxPos - 1);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 1);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// BRAKE 응답(TU, 3CH, A10(TX+), C10(G24,TX-),A11(P24,RX+),C11(RX-)}, 9600,EVEN)
		// EX) "02 20 0X 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 [BCC]"
		unsafe public short SendSdToBrake(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			Array.Clear(btBuf, 0x00, btBuf.Length);

			btBuf[nTxPos++] = STX;

			btBuf[nTxPos++] = 0x20;

			btBuf[nTxPos++] = btCheckDat;
			nTxPos += 48;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 1, btTempBuf, 0, nTxPos - 1);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 1);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// SIV 응답(TU, 4CH, A15(TX+), C15(G24,TX-),A16(P24,RX+),C16(RX-)}, 9600,EVEN)
		// EX) "02 20 0X 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 [BCC]"
		unsafe public short SendSdToSiv(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			Array.Clear(btBuf, 0x00, btBuf.Length);

			btBuf[nTxPos++] = STX;

			btBuf[nTxPos++] = 0x20;

			btBuf[nTxPos++] = btCheckDat;
			nTxPos += 48;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 1, btTempBuf, 0, nTxPos - 1);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 1);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

			return 1;
		}

		// CI 응답(TU, 4CH, A15(TX+), C15(G24,TX-),A16(P24,RX+),C16(RX-)}, 9600,EVEN)
		// EX) "02 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0X 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 [BCC]"
		unsafe public short SendSdToCi(byte btCheckDat)
		{
			int nTxPos = 0;
			byte[] btBuf = new byte[1024];
			byte[] btTempBuf = new byte[1024];
			ushort wBccBuf;

			nTxPos = 0;

			Array.Clear(btBuf, 0x00, btBuf.Length);

			btBuf[nTxPos++] = STX;

			btBuf[nTxPos++] = 0x20;

			btBuf[nTxPos + 20] = btCheckDat;
			nTxPos += 49;

			btBuf[nTxPos++] = ETX;

			Buffer.BlockCopy(btBuf, 1, btTempBuf, 0, nTxPos - 1);
			wBccBuf = GetBcc(btTempBuf, nTxPos - 1);

			btBuf[nTxPos++] = (byte)(wBccBuf & 0xFF);
			btBuf[nTxPos++] = (byte)((wBccBuf >> 8) & 0xFF);

			SendData(btBuf, nTxPos);

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
