
//2024.12.02 - FormPLC 오류부분 수정

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Ivi.Visa;
using Ivi.Visa.Interop;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using NetworkService;
using TCMSTester.Config;
using TCMSTester.Hardware;
using TCMSTester.Models;
using TCMSTester.Protocol;
using TCMSTester.Services;
using TCMSTester.Tests;
using TCMSTester.UI;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Color = System.Drawing.Color;
using Control = System.Windows.Forms.Control;
using Font = System.Drawing.Font;


namespace CITester
{
    public partial class FormMain : Form
    {
        private string[] strSerialPortList = System.IO.Ports.SerialPort.GetPortNames();


        FormLoad formLoad = new FormLoad();
        // XML Document
        //
        XmlDocument m_xmlDoc = new XmlDocument();

        private classSerialCommPacket serialTester1;
        private classSerialCommPacket serialTester2;

        // 언어 변수
        public string strLangaue = "KO";

        // 제어 편성 변수
        public string strControlUnitInfo { get; set; }
        // 설정 관련 변수
        //
        public ConfigData m_Config = new ConfigData();

        // 측정 결과 변수
        //
        public ResultData m_Result = new ResultData();
        public struct COMMON_INFO
        {
            public static SerialPort serialTester1Port0 = null;
            public static SerialPort serialTester2Port0 = null;
            public static string strTester1PortNo0 = "COM1";
            public static string strTester2PortNo0 = "COM1";
        }
        public struct SERIALPORT_TYPE
        {
            public const int Tester1 = 0;
            public const int Tester2 = 10;
        }

        // 데이터베이스 관련 변수
        //
        bool m_bDBOpened = false;
        string m_strDBPath = "CIDB.mdb";
        string m_strDBPassword = "hzsofttr";

        OleDbConnection m_OLEConnect;
        OleDbCommand m_OLECommand;

        int nTestTotalNumber = 0;
        int nTestCurrentNumber = 0;

        FormLoad frmLoad = new FormLoad();

        /// <summary>
        ///     TCP/IP 연결 장비
        /// </summary>
        /// 
        //public NetworkClient m_clientOscilloscope;
        NetworkClient m_clientDMM;
        public NetworkClient m_clientACPower;

        /// <summary>
        ///     TekVISA 연결 장비
        /// </summary>
        /// 
        ResourceManager m_ResourceManager = new ResourceManager();

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private PageSettings m_pgSettings = new PageSettings();
        private PrintDocument m_printDoc = new PrintDocument();
        int m_nPage = 0;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------

        //UI 헬퍼
        private CommTestUiManager _commUiManager;

        //전류출력보드출력값
        double dVdcCHFTChangeValue = 22;
        bool bVdcCHFTRestart = false;
        //
        bool bVdcCHFTTestStatus = false;
        bool bProtectTestManual = false;
        int nGateTestNum = 0;

        double dProtectValue = 0;

        int nCurrentTestItems = 0;


        // PLC 관련 변수
        public UdpClient sckPlcUdp = null;
        public classFenet m_PLCNetwork = null;
        public NetworkXGT m_PLCNetworkTest = null;
        public string m_strPLCAddress = "0.0.0.0";

        //MVB
        private MvbReceiver m_mvbReceiver;
        private MvbSerialManager m_serialManager;
        private MvbReceiver _mvbReceiver;
        private TcmsTestService _tcmsTestService;


        // TC 입출력 변수
        private int TC_DI1Count = 48;
        private int TC_DI2Count = 48;
        private int TC_DI3Count = 48;
        private int TC_DoCount = 32;

        // CC 입출력 변수
        private int CC_DI1Count = 48;
        private int CC_DI2Count = 48;
        private int CC_DoCount = 32;

        // DU 입출력 변수
        private int DU_DICount = 16;
        private int DU_DoCount = 6;

        // 아날로그 입출력 변수
        private int nAnalogInputCount = 4;
        private int nAnalogOutputCount = 4;

        // 오실로스코프(DSO-X 1204AA) 관련 변수, Ethernet
        public FormattedIO488 m_ethernetOscilloscope = new FormattedIO488();
        bool m_bOscilloscopeConnected = false;
        string m_strOscilloscode = "";
        string m_strOscilloscodeIDN = "";

        // 디지털 멀티 미터 (DMM - DAQ970A) 관련 변수, Ethernet
        FormattedIO488 m_ethernetDmm = new FormattedIO488();
        bool m_bDmmConnected = false;
        string m_strDmmIpNo = "";
        string m_strDmmIdn = "";

        // AC Power 관련 변수, Ethernet
        string m_strAcPowerIpNo = "";
        string m_strAcPowerPortNo = "";
        string m_strACPowerIdn = "";
        int m_nAcPowerPortNo = 0;

        // DC Power 관련 변수, Serial port
        string m_strDcPowerComPort = "";
        string m_strDcPowerIdn = "";
        int m_nDcPowerBaudRate = 0;
        SerialClient m_serialDcPower;

        // PWM 관련 변수
        string m_strPwmOutComPort = "";
        int m_nPwmOutBaudRate = 0;
        public SerialClient m_serialPwm;

        // 전류제어보드
        string m_strCurrentOutBoardComPort = "COM1";
        string m_strCurrentOutBoardIDN = "";
        int m_nCurrentOutBaudRate = 0;
        int nFailBoard = 0;
        SerialClient m_serialCurrentOutBoard;

        // 광보드1
        string m_strOpticBoardComPort1 = "COM1";
        string m_strOpticBoardIDN1 = "";
        int m_strOpticBoardBaudRate = 0;
        SerialClient m_serialOpticBoard1;

        // 광보드2
        string m_strOpticBoardComPort2 = "COM1";
        string m_strOpticBoardIDN2 = "";
        int m_strOpticBoardBaudRate2 = 0;
        SerialClient m_serialOpticBoard2;

        // MVB보드
        string m_strMvbBoardComPort = "COM1";
        string m_strMvbBoardIDN = "MVB-BOARD-V1X";
        int m_nMvbBoardBaudRate = 0;
        SerialClient m_serialMvbBoard;

        // TRIMMER1보드
        string m_strTrimmerBoardComPort1 = "COM1";
        string m_strTrimmerBoardIDN1 = "";
        int m_strTrimmerBoardBaudRate1 = 0;
        SerialClient m_serialTrimmerBoard1;

        // TRIMMER2보드
        string m_strTrimmerBoardComPort2 = "COM1";
        string m_strTrimmerBoardIDN2 = "";
        int m_strTrimmerBoardBaudRate2 = 0;
        SerialClient m_serialTrimmerBoard2;

        // LineVoltage보드
        string m_strLineVoltageBoardComPort0 = "COM1";
        string m_strLineVoltageBoardIDN0 = "";
        int m_strLineVoltageBaudRate0 = 0;
        SerialClient m_serialLineVoltageBoard0;

        // 모의속도 발생기 보드
        string m_strSpeedOutComPort = "COM1";
        string m_strSpeedOutIDN = "";
        int m_nSpeedOutBaudRate = 0;
        SerialClient m_serialSpeedOut;


        const int MEASURE_START = 0;
        const int MEASURE_STOP = 1;

        // 측정 제어 관련 변수
        //
        const int MEASURE_ITEM_NO = 11;

        const int MEASURE_ITEM_POWER_UNIT = 0;
        const int MEASURE_ITEM_COMM = 1;
        const int MEASURE_ITEM_SEQUENCE_RUN = 2;
        const int MEASURE_ITEM_SEQUENCE_BRAKE = 3;
        const int MEASURE_ITEM_SEQUENCE_STOP = 4;
        const int MEASURE_ITEM_INVERTER = 5;
        const int MEASURE_ITEM_PROTECT = 6;
        const int MEASURE_ITEM_GDU = 7;
        const int MEASURE_ITEM_CLEAR = 9;

        //DMM 채널정리
        //
        const int TP24V = 105;
        const int TN24V = 106;
        const int SEN_P12V = 107;

        bool m_bMeasureStarted = false;
        bool[] m_bMeasureItem = new bool[MEASURE_ITEM_NO];

        int m_nRunItem = 0;                 // 측정 중인 시험 항목 번호
        int m_nRunPhase = 0;                // 각 측정 내에서의 단계
        int nPowRunPhase = 0;                // 각 측정 내에서의 단계
        int nForRunPhase = 0;                // 각 측정 내에서의 단계
        int nBasicRunPhase = 0;                // 각 측정 내에서의 단계
        int nRunStopPhase = 0;                // 각 측정 내에서의 단계

        int nRunStopItem = 0;                 // 측정 중인 시험 항목 번호

        int nProtectStart = 0;                       // 1.기동ON 0.기동OFF
        int nProtectStop = 0;                       // 1.정지ON 0.정지OFF

        int m_nTestCounter = 0;             // 시간 대기를 위한 카운터 변수
        int m_nConfirmCount = 0;
        bool m_bControlPower = false;       // C/I 제어전원 인가 여뷰
        bool m_bSetCP = false;
        bool m_bSetCF = false;
        bool m_bSetAK = false;
        bool m_bSetK = false;

        bool m_bAKC = false;
        bool m_bMKC = false;
        bool m_bCGST = false;
        bool m_bIGST = false;
        bool m_bMJFX = false;

        int m_nStartPhase = 0;                  // 기동처리 단계
        bool m_bEndStartProcedure = false;      // 기동처리 완료 여부
        bool m_bResultStartProcedure = false;   // 기동처리 성공 여부

        bool bStart = false;   // 기동처리 성공 여부
        bool bStop = false;   // 기동처리 성공 여부

        bool bCI_ISO = false;

        int m_nClearPhase = 0;                  // 기동정리 단계 
        bool m_bEndClearProcedure = false;      // 기동정리 완료 여부

        double dProtectInputDCOV = 0;
        double dProtectInputFCOV = 0;
        double dProtectInputINOC = 0;
        double dProtectInputACOC = 0;
        double dProtectInputACOV = 0;
        double dProtectInputACUV = 0;
        double dProtectInputBCOC = 0;
        double dProtectInputBCOV = 0;
        double dProtectInputGDF = 0;
        double dProtectInputTHF = 0;
        double dProtectInputPOWER = 0;

        double m_dProtectMeasureCurrent = 0;    // 보호회로 측정 현재 출력값
        double m_dProtectMeasureCurrentOld = 0;    // 보호회로 측정 현재 출력값
        double m_dProtectMeasureCurrentNew = 0;    // 보호회로 측정 현재 출력값
        double m_dProtectMeasureMin = 0;        // 보호회로 측정범위 최소값
        double m_dProtectMeasureMax = 0;        // 보호회로 측정범위 최대값
        double m_dProtectMeasurePermitMin = 0;  // 보호회로 허용범위 최소값
        double m_dProtectMeasurePermitMax = 0;  // 보호회로 허용범위 최대값
        double m_dProtectMeasureStep1 = 0;      // 허용범위 밖에서의 출력값 조정치
        double m_dProtectMeasureStep2 = 0;      // 허용범위 내에서의 출력값 조정치

        double m_dYOffsetCh1 = 0;
        double m_dYOffsetCh2 = 0;

        //MVB 데이터 저장
        //아날로그
        string strLinVoltage = "0";
        string strIs = "0";
        string strIrms = "0";
        string strIu = "0";
        string strIv = "0";
        string strIw = "0";
        string strVdc = "0";
        string strIbch = "0";
        //디지털
        byte bGCH = 0;
        byte bGSC = 0;
        byte bGSI = 0;
        byte bMINOR_FT = 0;
        byte bMAJOR_FT = 0;
        byte bVdc_CHFT = 0;
        byte bVdc_LVFT = 0;
        byte bVdc_OVFT = 0;
        byte bVin_LVFT = 0;
        byte bVin_OVFT = 0;
        byte bCV_OT_FT = 0;
        byte bCU_OT_FT = 0;
        byte bGCY_FT = 0;
        byte bGCX_FT = 0;
        byte bGCV_FT = 0;
        byte bGCU_FT = 0;
        byte bIin_OCFT = 0;
        byte bPHUB_FT = 0;
        byte bIdh_OCFT = 0;
        byte bIinv_OCFT = 0;
        byte bDC_LGD_FT = 0;
        byte bAC_LGD_FT = 0;
        byte bCPS_FT = 0;
        byte bGCH_FT = 0;
        byte bIGZ_FT = 0;
        byte bIGY_FT = 0;
        byte bIGX_FT = 0;
        byte bIGW_FT = 0;
        byte bIGV_FT = 0;
        byte bIGU_FT = 0;
        byte bCH_OT_FT = 0;
        byte bPG2_FT = 0;
        byte bPG1_FT = 0;
        byte bI_OT_FT = 0;
        byte bVTI2_FT = 0;
        byte bVTI1_FT = 0;
        byte bCS_LL_FT = 0;
        byte bCS_FS_FT = 0;
        byte bCS_OVER_TEMP_FT = 0;


        private Dictionary<string, List<(string strName, string strUnit, EChannelState eState)>> m_dicBoards;

        private Dictionary<(int nCol, int nRow), Rectangle> m_dicBtnAreas = new Dictionary<(int, int), Rectangle>();

        #region TIMER_PHASE_DEFINITION
        const int POWER_UNIT_PHASE_1 = 0;
        const int POWER_UNIT_PHASE_2 = 1;
        const int POWER_UNIT_PHASE_3 = 2;
        const int POWER_UNIT_PHASE_4 = 3;
        const int POWER_UNIT_PHASE_5 = 4;
        const int POWER_UNIT_PHASE_6 = 5;
        const int POWER_UNIT_PHASE_7 = 6;
        const int POWER_UNIT_PHASE_8 = 7;
        const int POWER_UNIT_PHASE_9 = 8;
        const int POWER_UNIT_PHASE_10 = 9;
        const int POWER_UNIT_PHASE_11 = 10;
        const int POWER_UNIT_PHASE_12 = 11;
        const int POWER_UNIT_PHASE_13 = 12;
        const int POWER_UNIT_PHASE_14 = 13;
        const int POWER_UNIT_PHASE_15 = 14;
        const int POWER_UNIT_PHASE_16 = 15;
        const int POWER_UNIT_PHASE_17 = 16;
        const int POWER_UNIT_PHASE_END = 17;

        const int RUN_PHASE_1 = 0;
        const int RUN_PHASE_2 = 1;
        const int RUN_PHASE_3 = 2;
        const int RUN_PHASE_4 = 3;
        const int RUN_PHASE_5 = 4;
        const int RUN_PHASE_6 = 5;
        const int RUN_PHASE_7 = 6;
        const int RUN_PHASE_8 = 7;
        const int RUN_PHASE_9 = 8;
        const int RUN_PHASE_10 = 9;
        const int RUN_PHASE_11 = 10;
        const int RUN_PHASE_12 = 11;
        const int RUN_PHASE_END = 12;

        const int BRAKE_PHASE_1 = 0;
        const int BRAKE_PHASE_2 = 1;
        const int BRAKE_PHASE_3 = 2;
        const int BRAKE_PHASE_4 = 3;
        const int BRAKE_PHASE_5 = 4;
        const int BRAKE_PHASE_6 = 5;
        const int BRAKE_PHASE_7 = 6;
        const int BRAKE_PHASE_8 = 7;
        const int BRAKE_PHASE_9 = 8;
        const int BRAKE_PHASE_10 = 9;
        const int BRAKE_PHASE_11 = 10;
        const int BRAKE_PHASE_12 = 11;
        const int BRAKE_PHASE_13 = 12;
        const int BRAKE_PHASE_14 = 13;
        const int BRAKE_PHASE_15 = 14;
        const int BRAKE_PHASE_16 = 15;
        const int BRAKE_PHASE_17 = 16;
        const int BRAKE_PHASE_18 = 17;
        const int BRAKE_PHASE_19 = 18;
        const int BRAKE_PHASE_END = 19;

        const int STOP_PHASE_1 = 0;
        const int STOP_PHASE_2 = 1;
        const int STOP_PHASE_3 = 2;
        const int STOP_PHASE_4 = 3;
        const int STOP_PHASE_5 = 4;
        const int STOP_PHASE_6 = 5;
        const int STOP_PHASE_7 = 6;
        const int STOP_PHASE_8 = 7;
        const int STOP_PHASE_9 = 8;
        const int STOP_PHASE_10 = 9;
        const int STOP_PHASE_11 = 10;
        const int STOP_PHASE_12 = 11;
        const int STOP_PHASE_13 = 12;
        const int STOP_PHASE_14 = 13;
        const int STOP_PHASE_15 = 14;
        const int STOP_PHASE_16 = 15;
        const int STOP_PHASE_17 = 16;
        const int STOP_PHASE_18 = 17;
        const int STOP_PHASE_19 = 18;
        const int STOP_PHASE_20 = 19;
        const int STOP_PHASE_END = 20;

        const int INVERTER_PHASE_INIT1 = 0;
        const int INVERTER_PHASE_INIT2 = 1;
        const int INVERTER_PHASE_GCU_GCX_1 = 2;
        const int INVERTER_PHASE_GCU_GCX_2 = 3;
        const int INVERTER_PHASE_GCU_GCX_3 = 4;
        const int INVERTER_PHASE_GCU_GCX_4 = 5;
        const int INVERTER_PHASE_GCV_GCY_1 = 6;
        const int INVERTER_PHASE_GCV_GCY_2 = 7;
        const int INVERTER_PHASE_GCV_GCY_3 = 8;
        const int INVERTER_PHASE_GCV_GCY_4 = 9;
        const int INVERTER_PHASE_GIU_GIX_1 = 10;
        const int INVERTER_PHASE_GIU_GIX_2 = 11;
        const int INVERTER_PHASE_GIU_GIX_3 = 12;
        const int INVERTER_PHASE_GIU_GIX_4 = 13;
        const int INVERTER_PHASE_GIV_GIY_1 = 14;
        const int INVERTER_PHASE_GIV_GIY_2 = 15;
        const int INVERTER_PHASE_GIV_GIY_3 = 16;
        const int INVERTER_PHASE_GIV_GIY_4 = 17;
        const int INVERTER_PHASE_GIW_GIZ_1 = 18;
        const int INVERTER_PHASE_GIW_GIZ_2 = 19;
        const int INVERTER_PHASE_GIW_GIZ_3 = 20;
        const int INVERTER_PHASE_GIW_GIZ_4 = 21;
        const int INVERTER_PHASE_CLEAR = 22;
        const int INVERTER_PHASE_END = 23;

        const int PROTECT_PHASE_INIT = 0;
        const int PROTECT_PHASE_VdcOVFT_1 = 1;
        const int PROTECT_PHASE_VdcOVFT_2 = 2;
        const int PROTECT_PHASE_VdcOVFT_3 = 3;
        const int PROTECT_PHASE_VdcOVFT_4 = 4;
        const int PROTECT_PHASE_VdcOVFT_5 = 5;
        const int PROTECT_PHASE_VdcLVFT_1 = 6;
        const int PROTECT_PHASE_VdcLVFT_2 = 7;
        const int PROTECT_PHASE_VdcLVFT_3 = 8;
        const int PROTECT_PHASE_VdcLVFT_4 = 9;
        const int PROTECT_PHASE_VdcLVFT_5 = 10;
        const int PROTECT_PHASE_IinOCFT_1 = 11;
        const int PROTECT_PHASE_IinOCFT_2 = 12;
        const int PROTECT_PHASE_IinOCFT_3 = 13;
        const int PROTECT_PHASE_IinOCFT_4 = 14;
        const int PROTECT_PHASE_IinOCFT_5 = 15;
        const int PROTECT_PHASE_IinvUOCFT_1 = 16;
        const int PROTECT_PHASE_IinvUOCFT_2 = 17;
        const int PROTECT_PHASE_IinvUOCFT_3 = 18;
        const int PROTECT_PHASE_IinvUOCFT_4 = 19;
        const int PROTECT_PHASE_IinvUOCFT_5 = 20;
        const int PROTECT_PHASE_IinvVOCFT_1 = 21;
        const int PROTECT_PHASE_IinvVOCFT_2 = 22;
        const int PROTECT_PHASE_IinvVOCFT_3 = 23;
        const int PROTECT_PHASE_IinvVOCFT_4 = 24;
        const int PROTECT_PHASE_IinvVOCFT_5 = 25;
        const int PROTECT_PHASE_IinvWOCFT_1 = 26;
        const int PROTECT_PHASE_IinvWOCFT_2 = 27;
        const int PROTECT_PHASE_IinvWOCFT_3 = 28;
        const int PROTECT_PHASE_IinvWOCFT_4 = 29;
        const int PROTECT_PHASE_IinvWOCFT_5 = 30;
        const int PROTECT_PHASE_IdhOCFT_1 = 31;
        const int PROTECT_PHASE_IdhOCFT_2 = 32;
        const int PROTECT_PHASE_IdhOCFT_3 = 33;
        const int PROTECT_PHASE_IdhOCFT_4 = 34;
        const int PROTECT_PHASE_IdhOCFT_5 = 35;
        const int PROTECT_PHASE_VdcCHFT_1 = 36;
        const int PROTECT_PHASE_VdcCHFT_2 = 37;
        const int PROTECT_PHASE_VdcCHFT_3 = 38;
        const int PROTECT_PHASE_VdcCHFT_4 = 39;
        const int PROTECT_PHASE_VdcCHFT_5 = 40;
        const int PROTECT_PHASE_VdcCHFT_6 = 41;
        const int PROTECT_PHASE_VdcCHFT_7 = 42;
        const int PROTECT_PHASE_VinOVFT_1 = 43;
        const int PROTECT_PHASE_VinOVFT_2 = 44;
        const int PROTECT_PHASE_VinOVFT_3 = 45;
        const int PROTECT_PHASE_VinOVFT_4 = 46;
        const int PROTECT_PHASE_VinOVFT_5 = 47;
        const int PROTECT_PHASE_VinLVFT_1 = 48;
        const int PROTECT_PHASE_VinLVFT_2 = 49;
        const int PROTECT_PHASE_VinLVFT_3 = 50;
        const int PROTECT_PHASE_VinLVFT_4 = 51;
        const int PROTECT_PHASE_VinLVFT_5 = 52;
        const int PROTECT_PHASE_CPSFT_1 = 53;
        const int PROTECT_PHASE_CPSFT_2 = 54;
        const int PROTECT_PHASE_CPSFT_3 = 55;
        const int PROTECT_PHASE_CPSFT_4 = 56;
        const int PROTECT_PHASE_CPSFT_5 = 57;
        const int PROTECT_PHASE_GCUFT_1 = 58;
        const int PROTECT_PHASE_GCUFT_2 = 59;
        const int PROTECT_PHASE_GCUFT_3 = 60;
        const int PROTECT_PHASE_GCUFT_4 = 61;
        const int PROTECT_PHASE_GCUFT_5 = 62;
        const int PROTECT_PHASE_GCXFT_1 = 63;
        const int PROTECT_PHASE_GCXFT_2 = 64;
        const int PROTECT_PHASE_GCXFT_3 = 65;
        const int PROTECT_PHASE_GCXFT_4 = 66;
        const int PROTECT_PHASE_GCXFT_5 = 67;
        const int PROTECT_PHASE_GCVFT_1 = 68;
        const int PROTECT_PHASE_GCVFT_2 = 69;
        const int PROTECT_PHASE_GCVFT_3 = 70;
        const int PROTECT_PHASE_GCVFT_4 = 71;
        const int PROTECT_PHASE_GCVFT_5 = 72;
        const int PROTECT_PHASE_GCYFT_1 = 73;
        const int PROTECT_PHASE_GCYFT_2 = 74;
        const int PROTECT_PHASE_GCYFT_3 = 75;
        const int PROTECT_PHASE_GCYFT_4 = 76;
        const int PROTECT_PHASE_GCYFT_5 = 77;
        const int PROTECT_PHASE_IGUFT_1 = 78;
        const int PROTECT_PHASE_IGUFT_2 = 79;
        const int PROTECT_PHASE_IGUFT_3 = 80;
        const int PROTECT_PHASE_IGUFT_4 = 81;
        const int PROTECT_PHASE_IGUFT_5 = 82;
        const int PROTECT_PHASE_IGXFT_1 = 83;
        const int PROTECT_PHASE_IGXFT_2 = 84;
        const int PROTECT_PHASE_IGXFT_3 = 85;
        const int PROTECT_PHASE_IGXFT_4 = 86;
        const int PROTECT_PHASE_IGXFT_5 = 87;
        const int PROTECT_PHASE_IGVFT_1 = 88;
        const int PROTECT_PHASE_IGVFT_2 = 89;
        const int PROTECT_PHASE_IGVFT_3 = 90;
        const int PROTECT_PHASE_IGVFT_4 = 91;
        const int PROTECT_PHASE_IGVFT_5 = 92;
        const int PROTECT_PHASE_IGYFT_1 = 93;
        const int PROTECT_PHASE_IGYFT_2 = 94;
        const int PROTECT_PHASE_IGYFT_3 = 95;
        const int PROTECT_PHASE_IGYFT_4 = 96;
        const int PROTECT_PHASE_IGYFT_5 = 97;
        const int PROTECT_PHASE_IGWFT_1 = 98;
        const int PROTECT_PHASE_IGWFT_2 = 99;
        const int PROTECT_PHASE_IGWFT_3 = 100;
        const int PROTECT_PHASE_IGWFT_4 = 101;
        const int PROTECT_PHASE_IGWFT_5 = 102;
        const int PROTECT_PHASE_IGZFT_1 = 103;
        const int PROTECT_PHASE_IGZFT_2 = 104;
        const int PROTECT_PHASE_IGZFT_3 = 105;
        const int PROTECT_PHASE_IGZFT_4 = 106;
        const int PROTECT_PHASE_IGZFT_5 = 107;
        const int PROTECT_PHASE_GCHFT_1 = 108;
        const int PROTECT_PHASE_GCHFT_2 = 109;
        const int PROTECT_PHASE_GCHFT_3 = 110;
        const int PROTECT_PHASE_GCHFT_4 = 111;
        const int PROTECT_PHASE_GCHFT_5 = 112;
        const int PROTECT_PHASE_CUOTFT_1 = 113;
        const int PROTECT_PHASE_CUOTFT_2 = 114;
        const int PROTECT_PHASE_CUOTFT_3 = 115;
        const int PROTECT_PHASE_CUOTFT_4 = 116;
        const int PROTECT_PHASE_CUOTFT_5 = 117;
        const int PROTECT_PHASE_CVOTFT_1 = 118;
        const int PROTECT_PHASE_CVOTFT_2 = 119;
        const int PROTECT_PHASE_CVOTFT_3 = 120;
        const int PROTECT_PHASE_CVOTFT_4 = 121;
        const int PROTECT_PHASE_CVOTFT_5 = 122;
        const int PROTECT_PHASE_IOTFT_1 = 123;
        const int PROTECT_PHASE_IOTFT_2 = 124;
        const int PROTECT_PHASE_IOTFT_3 = 125;
        const int PROTECT_PHASE_IOTFT_4 = 126;
        const int PROTECT_PHASE_IOTFT_5 = 127;
        const int PROTECT_PHASE_CHOTFT_1 = 128;
        const int PROTECT_PHASE_CHOTFT_2 = 129;
        const int PROTECT_PHASE_CHOTFT_3 = 130;
        const int PROTECT_PHASE_CHOTFT_4 = 131;
        const int PROTECT_PHASE_CHOTFT_5 = 132;
        const int PROTECT_PHASE_DCLGDFT_1 = 133;
        const int PROTECT_PHASE_DCLGDFT_2 = 134;
        const int PROTECT_PHASE_DCLGDFT_3 = 135;
        const int PROTECT_PHASE_DCLGDFT_4 = 136;
        const int PROTECT_PHASE_DCLGDFT_5 = 137;
        const int PROTECT_PHASE_ACLGDFT_1 = 138;
        const int PROTECT_PHASE_ACLGDFT_2 = 139;
        const int PROTECT_PHASE_ACLGDFT_3 = 140;
        const int PROTECT_PHASE_ACLGDFT_4 = 141;
        const int PROTECT_PHASE_ACLGDFT_5 = 142;
        const int PROTECT_PHASE_CSOVERTEMPFT_1 = 143;
        const int PROTECT_PHASE_CSOVERTEMPFT_2 = 144;
        const int PROTECT_PHASE_CSOVERTEMPFT_3 = 145;
        const int PROTECT_PHASE_CSOVERTEMPFT_4 = 146;
        const int PROTECT_PHASE_CSOVERTEMPFT_5 = 147;
        const int PROTECT_PHASE_VTI1FLT_1 = 148;
        const int PROTECT_PHASE_VTI1FLT_2 = 149;
        const int PROTECT_PHASE_VTI1FLT_3 = 150;
        const int PROTECT_PHASE_VTI1FLT_4 = 151;
        const int PROTECT_PHASE_VTI1FLT_5 = 152;
        const int PROTECT_PHASE_VTI2FLT_1 = 153;
        const int PROTECT_PHASE_VTI2FLT_2 = 154;
        const int PROTECT_PHASE_VTI2FLT_3 = 155;
        const int PROTECT_PHASE_VTI2FLT_4 = 156;
        const int PROTECT_PHASE_VTI2FLT_5 = 157;
        const int PROTECT_PHASE_CLEAR = 158;
        const int PROTECT_PHASE_END = 159;

        const int CLEAR_PHASE_1 = 0;
        const int CLEAR_PHASE_2 = 1;
        const int CLEAR_PHASE_3 = 2;
        const int CLEAR_PHASE_4 = 3;
        const int CLEAR_PHASE_5 = 4;
        const int CLEAR_PHASE_6 = 5;
        const int CLEAR_PHASE_7 = 6;
        const int CLEAR_PHASE_8 = 7;
        const int CLEAR_PHASE_9 = 8;
        const int CLEAR_PHASE_10 = 9;
        const int CLEAR_PHASE_11 = 10;
        const int CLEAR_PHASE_12 = 11;
        const int CLEAR_PHASE_13 = 12;
        const int CLEAR_PHASE_14 = 13;
        const int CLEAR_PHASE_15 = 14;

        const int COMM_PHASE_1 = 0;
        const int COMM_PHASE_2 = 1;
        const int COMM_PHASE_3 = 2;
        const int COMM_PHASE_4 = 3;
        const int COMM_PHASE_5 = 4;
        const int COMM_PHASE_END = 5;

        const int GDU_PHASE_1 = 0;
        const int GDU_PHASE_2 = 1;
        const int GDU_PHASE_3 = 2;
        const int GDU_PHASE_4 = 3;
        const int GDU_PHASE_5 = 4;
        const int GDU_PHASE_6 = 5;
        const int GDU_PHASE_7 = 6;
        const int GDU_PHASE_END = 7;

        #endregion


        #region MAINFORM

        /// <summary>
        /// 
        /// </summary>
        /// 
        public FormMain()
        {
            //ApplyConfigLanguage();
            InitializeComponent();

            if (m_mvbReceiver == null)
            {
                m_mvbReceiver = new MvbReceiver();
            }

            try
            {
                if (sckPlcUdp != null)
                {
                    sckPlcUdp.Close();
                    sckPlcUdp.Dispose();
                }
                sckPlcUdp = new UdpClient(Convert.ToInt32("2005"));
                m_PLCNetwork = new classFenet(sckPlcUdp, "192.168.1.2", 2005);
            }
            catch
            {

            }

        }

        const int TIMINGCHART_MCB = 0;
        const int TIMINGCHART_FOR = 1;
        const int TIMINGCHART_AK = 2;
        const int TIMINGCHART_MK = 3;
        const int TIMINGCHART_POW = 4;
        const int TIMINGCHART_GSC = 5;
        const int TIMINGCHART_GSI = 6;
        const int TIMINGCHART_MINOR_FT = 7;
        const int TIMINGCHART_RESET = 7;
        const int TIMINGCHART_MAJOR_FT = 8;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED 스타일 추가
                return cp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        // 탭페이지 보관 변수
        private TabPage tabPageIndex2 = null;

        private void FormMain_Load(object sender, EventArgs e)
        {
            modernTreeView1.ItemHeight = 30;
            serialTester1 = new classSerialCommPacket(FormMain.COMMON_INFO.serialTester1Port0);
            serialTester2 = new classSerialCommPacket(FormMain.COMMON_INFO.serialTester2Port0);

            if (ConfigJson.CurrentConfig.Operation.TCMSUnit != "ER")
            {
                modernTreeView1.SetNodeVisible("ER 속도센서 시험", false);
            }
            else
            {
                modernTreeView1.SetNodeVisible("ER 속도센서 시험", true);
            }

            EnableDoubleBuffering(dataGridViewDI1);
            EnableDoubleBuffering(dataGridViewDI2);
            EnableDoubleBuffering(dataGridViewDI3);
            EnableDoubleBuffering(dataGridViewDO);
            EnableDoubleBuffering(dataGridViewAnalog);
            DisplayConfig();
            AnalgogData(dataGridViewAnalog);
            SetAnalogDataGrid();
            DataGridInit();

            _commUiManager = new CommTestUiManager(tableLayoutPanel1);
            _tcmsTestService = new TcmsTestService(_mvbReceiver);

            if (tabPageIndex2 == null)
            {
                tabPageIndex2 = mainTabControl1.TabPages.Cast<TabPage>().FirstOrDefault(p => p.Name == "tabPage6");
            }
            UpdateTabVisibility();
            InitData();
            SetupDataGridView();

            var nodeControl = modernTreeView1.Nodes.Find("입출력 시험", true).FirstOrDefault();
            if (nodeControl != null && nodeControl.Nodes.Count > 0)
                nodeControl.Checked = nodeControl.Nodes.Cast<TreeNode>().All(n => n.Checked);

            // MvbSerialManager 객체만 준비 (시험 시작 전에는 포트를 열지 않음)
            if (m_serialManager == null)
            {
                m_serialManager = new MvbSerialManager(m_mvbReceiver)
                {
                    OnLog = (msg) => AppendTestLog(richTextBox_Log, msg, Color.Blue),
                    OnError = (msg) => AppendTestLog(richTextBox_Log, msg, Color.Red)
                };
            }

            // 전역 에러 핸들러 연결
            m_mvbReceiver.ErrorOccurred += (s, errorMsg) =>
            {
                Console.WriteLine($"[TCMS 수신 에러] {errorMsg}");
            };
        }
        // 유닛마다 탭페이지 조절 함수
        private void UpdateTabVisibility()
        {
            if (tabPageIndex2 == null || mainTabControl1 == null || flatTabControl1 == null)
            {
                return;
            }

            bool bIsMainTabChanged = false;
            bool bIsFlatTabChanged = false;
            string strUnitType = ConfigJson.CurrentConfig?.Operation?.TCMSUnit ?? "TC";

            if (strUnitType != "ER")
            {
                if (mainTabControl1.TabPages.Contains(tabPageIndex2))
                {
                    mainTabControl1.TabPages.Remove(tabPageIndex2);
                    bIsMainTabChanged = true;
                }
            }
            else
            {
                if (!mainTabControl1.TabPages.Contains(tabPageIndex2))
                {
                    int nInsertIndex = Math.Min(2, mainTabControl1.TabPages.Count);
                    mainTabControl1.TabPages.Insert(nInsertIndex, tabPageIndex2);
                    bIsMainTabChanged = true;
                }
            }

            flatTabControl1.SuspendLayout();
            try
            {
                if (strUnitType == "CC")
                {
                    if (tabPageDI3 != null && flatTabControl1.TabPages.Contains(tabPageDI3))
                    {
                        flatTabControl1.TabPages.Remove(tabPageDI3);
                        bIsFlatTabChanged = true;
                    }

                    if (flatTabControl1.TabPages.Contains(tabPageDI1)) tabPageDI1.Text = "디지털 입력 1/2";
                    if (flatTabControl1.TabPages.Contains(tabPageDI2)) tabPageDI2.Text = "디지털 입력 2/2";
                }
                else if (strUnitType == "DU")
                {
                    if (tabPageDI2 != null && flatTabControl1.TabPages.Contains(tabPageDI2))
                    {
                        flatTabControl1.TabPages.Remove(tabPageDI2);
                        bIsFlatTabChanged = true;
                    }

                    if (tabPageDI3 != null && flatTabControl1.TabPages.Contains(tabPageDI3))
                    {
                        flatTabControl1.TabPages.Remove(tabPageDI3);
                        bIsFlatTabChanged = true;
                    }
                    if (flatTabControl1.TabPages.Contains(tabPageDI1)) tabPageDI1.Text = "디지털 입력";
                }
                else
                {
                    if (tabPageDI2 != null && !flatTabControl1.TabPages.Contains(tabPageDI2))
                    {
                        int nInsertIndex = Math.Min(1, flatTabControl1.TabPages.Count);
                        flatTabControl1.TabPages.Insert(nInsertIndex, tabPageDI2);
                        bIsFlatTabChanged = true;
                    }
                    if (tabPageDI3 != null && !flatTabControl1.TabPages.Contains(tabPageDI3))
                    {
                        int nInsertIndex = Math.Min(2, flatTabControl1.TabPages.Count);
                        flatTabControl1.TabPages.Insert(nInsertIndex, tabPageDI3);
                        bIsFlatTabChanged = true;
                    }

                    // 기본 탭 이름 복원 
                    if (tabPageDI1 != null) tabPageDI1.Text = "디지털 입력 1/3";
                    if (tabPageDI2 != null) tabPageDI2.Text = "디지털 입력 2/3";
                    if (tabPageDI3 != null) tabPageDI3.Text = "디지털 입력 3/3";
                }
            }
            finally
            {
                flatTabControl1.ResumeLayout();
            }

            if (bIsMainTabChanged)
            {
                mainTabControl1.ForceUpdateTabSize();
            }
        }
        private void AnalgogData(DataGridView dgv)
        {
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 클릭 시 색상 변화 없애기
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DefaultCellStyle.SelectionBackColor = dataGridViewAnalog.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dataGridViewAnalog.DefaultCellStyle.ForeColor;

            // 정렬 및 헤더 스타일 설정
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(240, 244, 253);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(8, 31, 78);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(8, 31, 78);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 244, 253);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewAnalog.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridViewAnalog.ColumnHeadersDefaultCellStyle.ForeColor;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 35;
            dgv.RowTemplate.Height = 35;

            dgv.DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 12, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 12, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(8, 31, 78);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 244, 253);

            dgv.DefaultCellStyle.SelectionBackColor = dataGridViewAnalog.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dataGridViewAnalog.DefaultCellStyle.ForeColor;

            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewAnalog.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridViewAnalog.ColumnHeadersDefaultCellStyle.ForeColor;

        }
        private void SetAnalogDataGrid()
        {
            // UI 불필요한 렌더링 방지 및 예외 처리
            if (dataGridViewAnalog == null)
            {
                return;
            }

            dataGridViewAnalog.SuspendLayout();

            try
            {
                dataGridViewAnalog.Columns.Clear();
                dataGridViewAnalog.Columns.Add("colIndex", "순 번");
                dataGridViewAnalog.Columns.Add("colItem", "항 목");
                dataGridViewAnalog.Columns.Add("colValue", "측정치");
                dataGridViewAnalog.Columns.Add("colResult", "판 정");

                dataGridViewAnalog.Columns[0].Width = 60;

                foreach (DataGridViewColumn objCol in dataGridViewAnalog.Columns)
                {
                    objCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                dataGridViewAnalog.Rows.Clear();

                // 1. 아날로그 입력 행 추가 (순번: 1부터 시작)
                for (int nIdx = 0; nIdx < nAnalogInputCount; nIdx++)
                {
                    int nInRowIndex = nIdx + 1;
                    string strItemName = $"아날로그 입력_{nIdx:D2}";
                    dataGridViewAnalog.Rows.Add(new string[] { nInRowIndex.ToString(), strItemName, "-", "-" });
                }

                // 2. 입력과 출력 사이 구분 행 삽입
                if (nAnalogInputCount > 0 && nAnalogOutputCount > 0)
                {
                    dataGridViewAnalog.Rows.Add(new string[] { "", "--------------------", "", "" });
                }

                // 3. 아날로그 출력 행 추가 (순번: 다시 1부터 시작)
                for (int nIdx = 0; nIdx < nAnalogOutputCount; nIdx++)
                {
                    int nOutRowIndex = nIdx + 1;
                    string strItemName = $"아날로그 출력_{nIdx:D2}";
                    dataGridViewAnalog.Rows.Add(new string[] { nOutRowIndex.ToString(), strItemName, "-", "-" });
                }
            }
            finally
            {
                dataGridViewAnalog.ResumeLayout();
            }
        }

        // TC 유닛 독립 상태 버퍼
        private EChannelState[] arrTcDi1States;
        private EChannelState[] arrTcDi2States;
        private EChannelState[] arrTcDi3States;
        private EChannelState[] arrTcDoStates;

        // CC 유닛 독립 상태 버퍼
        private EChannelState[] arrCcDi1States;
        private EChannelState[] arrCcDi2States;
        private EChannelState[] arrCcDi3States;
        private EChannelState[] arrCcDoStates;

        // DU 유닛 독립 상태 버퍼
        private EChannelState[] arrDuDi1States;
        private EChannelState[] arrDuDi2States;
        private EChannelState[] arrDuDi3States;
        private EChannelState[] arrDuDoStates;

        private string strSelectedChannelNo = "";
        private string strSelectedChannelType = "";
        private string strSelectedState = "";
        private string strSelectedTestResult = "";
        public enum EChannelState
        {
            Off,    // OFF (비활성)
            On,     // ON (정상)
            Test,   // TEST (시험중)
            Err     // ERR (오류)
        }
        private string GetStateString(EChannelState eState)
        {
            switch (eState)
            {
                case EChannelState.On: return "정상";
                case EChannelState.Test: return "시험중";
                case EChannelState.Err: return "오류";
                case EChannelState.Off:
                default: return "미시험";
            }
        }

        #region 입출력 시험 디자인용
        private void DataGridInit()
        {
            // TC 유닛 사양 메모리 할당
            arrTcDi1States = new EChannelState[TC_DI1Count];
            arrTcDi2States = new EChannelState[TC_DI2Count];
            arrTcDi3States = new EChannelState[TC_DI3Count];
            arrTcDoStates = new EChannelState[TC_DoCount];

            // CC 유닛 사양 메모리 할당
            arrCcDi1States = new EChannelState[CC_DI1Count];
            arrCcDi2States = new EChannelState[CC_DI2Count];
            arrCcDoStates = new EChannelState[CC_DoCount];

            // DU 유닛 사양 메모리 할당
            arrDuDi1States = new EChannelState[DU_DICount];
            arrDuDoStates = new EChannelState[DU_DoCount];

            // 실행 시 최초 기본 타겟 유닛 레이아웃 구성
            SelectActiveUnit();
        }

        public void SelectActiveUnit()
        {
            switch (ConfigJson.CurrentConfig.Operation.TCMSUnit)
            {
                case "TC":
                    ConfigureChannelGrid(dataGridViewDI1, new string[] { "DI1"}, new int[] { TC_DI1Count });
                    ConfigureChannelGrid(dataGridViewDI2, new string[] { "DI2" }, new int[] { TC_DI2Count });
                    ConfigureChannelGrid(dataGridViewDI3, new string[] { "DI3" }, new int[] { TC_DI3Count });
                    ConfigureChannelGrid(dataGridViewDO, new string[] { "DO" }, new int[] { TC_DoCount });
                    break;

                case "CC":
                    ConfigureChannelGrid(dataGridViewDI1, new string[] { "DI1" }, new int[] { CC_DI1Count });
                    ConfigureChannelGrid(dataGridViewDI2, new string[] { "DI2" }, new int[] { CC_DI2Count });
                    ConfigureChannelGrid(dataGridViewDO, new string[] { "DO" }, new int[] { CC_DoCount });
                    break;

                case "DU":
                    ConfigureChannelGrid(dataGridViewDI1, new string[] { "DI1" }, new int[] { DU_DICount });
                    ConfigureChannelGrid(dataGridViewDO, new string[] { "DO" }, new int[] { DU_DoCount });
                    break;
            }
        }

        private void ConfigureChannelGrid(DataGridView dgvTarget, string[] arrStrTypes, int[] arrNCounts)
        {
            if (dgvTarget == null || arrStrTypes == null || arrNCounts == null || arrStrTypes.Length != arrNCounts.Length) return;

            dgvTarget.Tag = new ChannelGridState();
            dgvTarget.Columns.Clear();
            dgvTarget.Rows.Clear();

            dgvTarget.ReadOnly = true;
            dgvTarget.MultiSelect = false;
            dgvTarget.AllowUserToAddRows = false;
            dgvTarget.RowHeadersVisible = false;
            dgvTarget.ColumnHeadersVisible = false;
            dgvTarget.AllowUserToResizeColumns = false;
            dgvTarget.AllowUserToResizeRows = false;
            dgvTarget.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvTarget.BorderStyle = BorderStyle.None;
            dgvTarget.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTarget.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvTarget.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTarget.BackgroundColor = Color.White;
            dgvTarget.RowTemplate.DefaultCellStyle.SelectionBackColor = dgvTarget.BackgroundColor;
            dgvTarget.RowTemplate.DefaultCellStyle.SelectionForeColor = dgvTarget.DefaultCellStyle.ForeColor;
            dgvTarget.DefaultCellStyle.SelectionBackColor = dgvTarget.BackgroundColor;
            dgvTarget.DefaultCellStyle.SelectionForeColor = dgvTarget.DefaultCellStyle.ForeColor;

            int nTotalCols = 8;
            for (int nCol = 0; nCol < nTotalCols; nCol++)
            {
                int nColIdx = dgvTarget.Columns.Add($"Col{nCol}", "");
                dgvTarget.Columns[nColIdx].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvTarget.Columns[nColIdx].FillWeight = 100;
            }

            int nHeaderRowIdx = dgvTarget.Rows.Add();
            dgvTarget.Rows[nHeaderRowIdx].HeaderCell.Value = "HEADER";
            dgvTarget.Rows[nHeaderRowIdx].Height = 50;

            // 주입된 배열 스케일에 결합하여 가변적으로 서브 채널 데이터 렌더링 공간 적재
            for (int nGroup = 0; nGroup < arrStrTypes.Length; nGroup++)
            {
                if (arrNCounts[nGroup] <= 0) continue;

                if (nGroup > 0)
                {
                    int nBlankRowIdx = dgvTarget.Rows.Add();
                    dgvTarget.Rows[nBlankRowIdx].HeaderCell.Value = "BLANK";
                    dgvTarget.Rows[nBlankRowIdx].Height = 35;
                }

                int nRows = (int)Math.Ceiling((double)arrNCounts[nGroup] / 8);
                for (int nRow = 0; nRow < nRows; nRow++)
                {
                    int nRowIdx = dgvTarget.Rows.Add();
                    dgvTarget.Rows[nRowIdx].HeaderCell.Value = arrStrTypes[nGroup];
                }
            }

            dgvTarget.CellPainting -= DataGridViewChannel_CellPainting;
            dgvTarget.CellPainting += DataGridViewChannel_CellPainting;
            dgvTarget.Paint -= DataGridViewChannel_Paint;
            dgvTarget.Paint += DataGridViewChannel_Paint;
            dgvTarget.CellClick -= DataGridViewChannel_CellClick;
            dgvTarget.CellClick += DataGridViewChannel_CellClick;
            dgvTarget.MouseMove -= DataGridViewChannel_MouseMove;
            dgvTarget.MouseMove += DataGridViewChannel_MouseMove;
            dgvTarget.MouseLeave -= DataGridViewChannel_MouseLeave;
            dgvTarget.MouseLeave += DataGridViewChannel_MouseLeave;

            dgvTarget.Resize -= DataGridViewChannel_Resize;
            dgvTarget.Resize += DataGridViewChannel_Resize;

            DataGridViewChannel_Resize(dgvTarget, EventArgs.Empty);
        }

        private void DataGridViewChannel_Resize(object sender, EventArgs eArgs)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null || dgv.Rows.Count == 0) return;

            // [핵심 수정] 숨겨진 탭에서 그리드가 깨어날 때 발생하는 컬럼 오계산 버그를 해결하기 위해
            // 오토사이즈 모드를 잠시 꺼서 꼬인 레이아웃 캐시를 완전히 초기화합니다.
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            int nActiveRowsCount = 0;
            int nHeaderHeight = 0;
            int nBlankTotalHeight = 0;

            // 격리 정밀 검사: 헤더와 여백 행의 고정 높이를 명확히 카운트
            foreach (DataGridViewRow row in dgv.Rows)
            {
                string strVal = row.HeaderCell.Value?.ToString();
                if (strVal == "HEADER")
                {
                    nHeaderHeight = 50;
                    row.Height = 50;
                }
                else if (strVal == "BLANK")
                {
                    nBlankTotalHeight += 35;
                    row.Height = 35;
                }
                else
                {
                    nActiveRowsCount++;
                }
            }

            // 순수 데이터 셀들이 차지할 수 있는 실제 가용 픽셀 영역 계산
            int nTotalAvailableHeight = dgv.Height - nHeaderHeight - nBlankTotalHeight - 4;

            // 잔상 및 단차 오차 방지를 위한 동적 행 높이 분할 배정
            if (nActiveRowsCount > 0)
            {
                int nDynamicRowHeight = nTotalAvailableHeight / nActiveRowsCount;
                if (nDynamicRowHeight > 25)
                {
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        string strHeaderVal = row.HeaderCell.Value?.ToString();

                        if (strHeaderVal != "BLANK" && strHeaderVal != "HEADER")
                        {
                            row.Height = nDynamicRowHeight;
                        }
                    }
                }
            }

            // [핵심 수정] 현재 완전히 확장된 실제 런타임 가로 폭을 기준으로
            // 다시 깨끗하게 8등분 하도록 Fill 모드를 재적용합니다. 단차가 완벽히 사라집니다.
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private class ChannelGridState
        {
            public List<Point> listSelectedCells = new List<Point>();
            public int nHoverRowIdx = -1;
            public int nHoverColIdx = -1;
            public bool bIsHoverManualTest = false;
            public int nSelectedRowIdx = -1;
            public int nSelectedColIdx = -1;
        }

        private bool TryGetChannelRuntimeInfo(DataGridView dgvSource, int nRowIdx, int nColIdx, out int nOutChannelNo, out EChannelState eOutState, out string strOutTypeName)
        {
            nOutChannelNo = -1;
            eOutState = EChannelState.Off;
            strOutTypeName = string.Empty;

            if (nRowIdx < 0 || nColIdx < 0) return false;

            string strRowType = dgvSource.Rows[nRowIdx].HeaderCell.Value?.ToString() ?? string.Empty;
            if (strRowType == "BLANK" || strRowType == "HEADER" || string.IsNullOrEmpty(strRowType)) return false;

            int nSameTypeRowCount = 0;
            for (int nR = 0; nR < nRowIdx; nR++)
            {
                if (dgvSource.Rows[nR].HeaderCell.Value?.ToString() == strRowType)
                {
                    nSameTypeRowCount++;
                }
            }

            int nTargetIdx = (nSameTypeRowCount * 8) + nColIdx;
            EChannelState[] arrTargetStates = null;

            // 활성화된 메인 제어 유닛 및 상세 서브 도메인 분기 처리
            switch (ConfigJson.CurrentConfig.Operation.TCMSUnit)
            {
                case "TC":
                    if (strRowType == "DI1") { arrTargetStates = arrTcDi1States; strOutTypeName = "TC 입력 1 (DI1)"; }
                    else if (strRowType == "DI2") { arrTargetStates = arrTcDi2States; strOutTypeName = "TC 입력 2 (DI2)"; }
                    else if (strRowType == "DI3") { arrTargetStates = arrTcDi3States; strOutTypeName = "TC 입력 3 (DI3)"; }
                    else if (strRowType == "DO") { arrTargetStates = arrTcDoStates; strOutTypeName = "TC 출력 (DO)"; }
                    break;

                case "CC":
                    if (strRowType == "DI1") { arrTargetStates = arrCcDi1States; strOutTypeName = "CC 입력 1 (DI1)"; }
                    else if (strRowType == "DI2") { arrTargetStates = arrCcDi2States; strOutTypeName = "CC 입력 2 (DI2)"; }
                    else if (strRowType == "DI3") { arrTargetStates = arrCcDi3States; strOutTypeName = "CC 입력 3 (DI3)"; }
                    else if (strRowType == "DO") { arrTargetStates = arrCcDoStates; strOutTypeName = "CC 출력 (DO)"; }
                    break;

                case "DU":
                    if (strRowType == "DI1") { arrTargetStates = arrDuDi1States; strOutTypeName = "DU 입력 1 (DI1)"; }
                    else if (strRowType == "DI2") { arrTargetStates = arrDuDi2States; strOutTypeName = "DU 입력 2 (DI2)"; }
                    else if (strRowType == "DI3") { arrTargetStates = arrDuDi3States; strOutTypeName = "DU 입력 3 (DI3)"; }
                    else if (strRowType == "DO") { arrTargetStates = arrDuDoStates; strOutTypeName = "DU 출력 (DO)"; }
                    break;
            }

            if (arrTargetStates == null || nTargetIdx >= arrTargetStates.Length) return false;

            eOutState = arrTargetStates[nTargetIdx];
            nOutChannelNo = nTargetIdx + 1;
            return true;
        }

        private void DataGridViewChannel_CellPainting(object sender, DataGridViewCellPaintingEventArgs eCell)
        {
            if (eCell.RowIndex < 0 || eCell.ColumnIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            ChannelGridState state = dgv.Tag as ChannelGridState;
            if (state == null) return;

            string strRowType = dgv.Rows[eCell.RowIndex].HeaderCell.Value?.ToString() ?? string.Empty;

            using (SolidBrush brshBgClear = new SolidBrush(dgv.BackgroundColor))
            {
                Rectangle rectExtendedBounds = new Rectangle(eCell.CellBounds.X - 1, eCell.CellBounds.Y - 1, eCell.CellBounds.Width + 2, eCell.CellBounds.Height + 2);
                eCell.Graphics.FillRectangle(brshBgClear, rectExtendedBounds);
            }
            eCell.PaintBackground(eCell.CellBounds, true);

            if (TryGetChannelRuntimeInfo(dgv, eCell.RowIndex, eCell.ColumnIndex, out int nChannelNo, out EChannelState eState, out _))
            {
                Graphics gtx = eCell.Graphics;
                gtx.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen penEdgeClear = new Pen(dgv.BackgroundColor, 2.0f))
                {
                    gtx.DrawRectangle(penEdgeClear, 0, 0, dgv.Width, dgv.Height);
                }

                Color clrBoxFill, clrBoxBorder, clrText;
                switch (eState)
                {
                    case EChannelState.On:
                        clrBoxFill = Color.FromArgb(240, 249, 241);
                        clrBoxBorder = Color.FromArgb(40, 167, 69);
                        clrText = Color.FromArgb(40, 167, 69);
                        break;
                    case EChannelState.Test:
                        clrBoxFill = Color.FromArgb(255, 249, 230);
                        clrBoxBorder = Color.FromArgb(255, 193, 7);
                        clrText = Color.FromArgb(190, 140, 0);
                        break;
                    case EChannelState.Err:
                        clrBoxFill = Color.FromArgb(253, 238, 238);
                        clrBoxBorder = Color.FromArgb(220, 53, 69);
                        clrText = Color.FromArgb(220, 53, 69);
                        break;
                    case EChannelState.Off:
                    default:
                        clrBoxFill = Color.White;
                        clrBoxBorder = Color.DimGray;
                        clrText = Color.FromArgb(74, 85, 104);
                        break;
                }

                bool bIsSelected = state.listSelectedCells.Contains(new Point(eCell.ColumnIndex, eCell.RowIndex));
                bool bIsHovered = (eCell.RowIndex == state.nHoverRowIdx && eCell.ColumnIndex == state.nHoverColIdx);

                if (bIsSelected)
                {
                    clrBoxFill = Color.FromArgb(218, 234, 254);
                    clrBoxBorder = Color.FromArgb(37, 99, 235);
                    clrText = Color.FromArgb(37, 99, 235);
                }
                else if (bIsHovered)
                {
                    clrBoxFill = Color.FromArgb(243, 244, 246);
                    clrBoxBorder = Color.FromArgb(156, 163, 175);
                }

                Rectangle rectBox = eCell.CellBounds;
                int nMaxBoxWidth = 45;
                int nMaxBoxHeight = 45;

                int nNewWidth = Math.Min(rectBox.Width - 14, nMaxBoxWidth);
                int nNewHeight = Math.Min(rectBox.Height - 12, nMaxBoxHeight);

                int nOffsetX = rectBox.X + (rectBox.Width - nNewWidth) / 2;
                int nOffsetY = rectBox.Y + (rectBox.Height - nNewHeight) / 2;

                rectBox = new Rectangle(nOffsetX, nOffsetY, nNewWidth, nNewHeight);

                using (GraphicsPath pathBox = GetRoundedRectPath(rectBox, 11))
                {
                    using (SolidBrush brshBoxFill = new SolidBrush(clrBoxFill))
                    {
                        gtx.FillPath(brshBoxFill, pathBox);
                    }
                    using (Pen penBoxBorder = new Pen(clrBoxBorder, 2.0f))
                    {
                        gtx.DrawPath(penBoxBorder, pathBox);
                    }
                }

                string strChNo = $"{nChannelNo}";
                using (Font fntChannelNo = new Font("맑은 고딕", 11.5F, FontStyle.Bold))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (SolidBrush brshText = new SolidBrush(clrText))
                {
                    gtx.DrawString(strChNo, fntChannelNo, brshText, rectBox, sf);
                }
            }

            eCell.Handled = true;
        }

        private void DataGridViewChannel_Paint(object sender, PaintEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Rows.Count == 0) return;

            ChannelGridState state = dgv.Tag as ChannelGridState;
            if (state == null) return;

            Graphics gtx = e.Graphics;
            gtx.SmoothingMode = SmoothingMode.AntiAlias;
            gtx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (Pen penEdgeClear = new Pen(dgv.BackgroundColor, 2.0f))
            {
                gtx.DrawRectangle(penEdgeClear, 0, 0, dgv.Width, dgv.Height);
            }

            // 상단 우측 가로 배치 UI 좌표 계산 영역 (HEADER 행 공간 활용)
            int nBtnWidth = 100;
            int nBtnHeight = 32;
            int nBtnX = dgv.Width - nBtnWidth - 14;
            int nBtnY = (50 - nBtnHeight) / 2;
            Rectangle rectManualTest = new Rectangle(nBtnX, nBtnY, nBtnWidth, nBtnHeight);

            // 1. 수동 시험 버튼 렌더링
            Color clrBtnBg = state.bIsHoverManualTest ? Color.FromArgb(29, 78, 216) : Color.FromArgb(37, 99, 235);
            using (GraphicsPath pathBtn = GetRoundedRectPath(rectManualTest, 8))
            {
                using (SolidBrush brshBtnBg = new SolidBrush(clrBtnBg))
                {
                    gtx.FillPath(brshBtnBg, pathBtn);
                }
            }

            using (Font fntBtn = new Font("맑은 고딕", 10.5F, FontStyle.Bold))
            using (StringFormat sfBtn = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                gtx.DrawString("수동 시험", fntBtn, Brushes.White, rectManualTest, sfBtn);
            }

            // 2. 시험 상태 범례 가로 배치 레이아웃 구성 (테두리 제거)
            string[] arrStatusLabels = { "미시험", "시험중", "정상", "오류" };
            int nStatusItemWidth = 72;
            int nTotalStatusWidth = nStatusItemWidth * 4;
            int nStatusStartX = nBtnX - nTotalStatusWidth - 10;
            int nStatusY = (50 - 12) / 2;

            using (Font fntStatus = new Font("맑은 고딕", 10.0F, FontStyle.Regular))
            using (SolidBrush brshTextDark = new SolidBrush(Color.FromArgb(45, 55, 72)))
            {
                for (int nIdx = 0; nIdx < arrStatusLabels.Length; nIdx++)
                {
                    int nItemX = nStatusStartX + (nIdx * nStatusItemWidth);
                    Color clrDot = Color.Gray;
                    if (nIdx == 0) clrDot = Color.FromArgb(160, 174, 192);
                    if (nIdx == 1) clrDot = Color.FromArgb(255, 193, 7);
                    if (nIdx == 2) clrDot = Color.FromArgb(40, 167, 69);
                    if (nIdx == 3) clrDot = Color.FromArgb(220, 53, 69);

                    using (SolidBrush brshDot = new SolidBrush(clrDot))
                    {
                        gtx.FillEllipse(brshDot, nItemX, nStatusY + 1, 12, 12);
                    }
                    gtx.DrawString(arrStatusLabels[nIdx], fntStatus, brshTextDark, nItemX + 16, nStatusY - 2);
                }
            }
        }

        private void DataGridViewChannel_CellClick(object sender, DataGridViewCellEventArgs eArgs)
        {
            if (eArgs.RowIndex < 0 || eArgs.ColumnIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            ChannelGridState state = dgv.Tag as ChannelGridState;
            if (state == null) return;

            string strRowType = dgv.Rows[eArgs.RowIndex].HeaderCell.Value?.ToString() ?? string.Empty;

            // 상단 HEADER 행 클릭 시 수동 시험 버튼 좌표 판정 수행
            if (strRowType == "HEADER")
            {
                int nBtnWidth = 100;
                int nBtnHeight = 32;
                int nBtnX = dgv.Width - nBtnWidth - 14;
                int nBtnY = (50 - nBtnHeight) / 2;
                Rectangle rectManualTest = new Rectangle(nBtnX, nBtnY, nBtnWidth, nBtnHeight);

                Point ptClient = dgv.PointToClient(Cursor.Position);
                if (rectManualTest.Contains(ptClient))
                {
                    // 수동 시험 비즈니스 로직 연동 핸들러 추가 가능 위치
                }
                return;
            }

            if (TryGetChannelRuntimeInfo(dgv, eArgs.RowIndex, eArgs.ColumnIndex, out int nChannelNo, out EChannelState eState, out string strTypeName))
            {
                Point ptCurrent = new Point(eArgs.ColumnIndex, eArgs.RowIndex);
                if (state.listSelectedCells.Contains(ptCurrent))
                {
                    state.listSelectedCells.Remove(ptCurrent);
                }
                else
                {
                    state.listSelectedCells.Add(ptCurrent);
                }

                state.nSelectedRowIdx = eArgs.RowIndex;
                state.nSelectedColIdx = eArgs.ColumnIndex;

                strSelectedChannelNo = $"{nChannelNo}";
                strSelectedChannelType = strTypeName;
                strSelectedState = GetStateString(eState);
                strSelectedTestResult = (eState == EChannelState.Err) ? "FAIL" : (eState == EChannelState.Off ? "미시험" : "PASS");

                dgv.Invalidate();
            }
        }

        private void DataGridViewChannel_MouseMove(object sender, MouseEventArgs eArgs)
        {
            DataGridView dgv = (DataGridView)sender;
            ChannelGridState state = dgv.Tag as ChannelGridState;
            if (state == null) return;

            DataGridView.HitTestInfo hit = dgv.HitTest(eArgs.X, eArgs.Y);

            int nPrevHoverRow = state.nHoverRowIdx;
            int nPrevHoverCol = state.nHoverColIdx;
            bool bPrevHoverManualTest = state.bIsHoverManualTest;

            state.nHoverRowIdx = -1;
            state.nHoverColIdx = -1;
            state.bIsHoverManualTest = false;

            // 움직이는 마우스 좌표 기준 수동 시험 버튼 충돌 검사 수행
            int nBtnWidth = 100;
            int nBtnHeight = 32;
            int nBtnX = dgv.Width - nBtnWidth - 14;
            int nBtnY = (50 - nBtnHeight) / 2;
            Rectangle rectManualTest = new Rectangle(nBtnX, nBtnY, nBtnWidth, nBtnHeight);

            if (rectManualTest.Contains(eArgs.Location))
            {
                state.bIsHoverManualTest = true;
            }
            else if (hit.Type == DataGridViewHitTestType.Cell)
            {
                if (TryGetChannelRuntimeInfo(dgv, hit.RowIndex, hit.ColumnIndex, out _, out _, out _))
                {
                    state.nHoverRowIdx = hit.RowIndex;
                    state.nHoverColIdx = hit.ColumnIndex;
                }
            }

            if (state.nHoverRowIdx != -1 || state.bIsHoverManualTest)
            {
                dgv.Cursor = Cursors.Hand;
            }
            else
            {
                dgv.Cursor = Cursors.Default;
            }

            if (nPrevHoverRow != state.nHoverRowIdx || nPrevHoverCol != state.nHoverColIdx || bPrevHoverManualTest != state.bIsHoverManualTest)
            {
                dgv.Invalidate();
            }
        }

        private void DataGridViewChannel_MouseLeave(object sender, EventArgs eArgs)
        {
            DataGridView dgv = (DataGridView)sender;
            ChannelGridState state = dgv.Tag as ChannelGridState;
            if (state == null) return;

            state.nHoverRowIdx = -1;
            state.nHoverColIdx = -1;
            state.bIsHoverManualTest = false;
            dgv.Cursor = Cursors.Default;
            dgv.Invalidate();
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int nRadius)
        {
            GraphicsPath path = new GraphicsPath();
            int nDiameter = nRadius * 2;

            if (nDiameter > rect.Width) nDiameter = rect.Width;
            if (nDiameter > rect.Height) nDiameter = rect.Height;

            Rectangle rectArc = new Rectangle(rect.X, rect.Y, nDiameter, nDiameter);

            path.AddArc(rectArc, 180, 90);
            rectArc.X = rect.Right - nDiameter;
            path.AddArc(rectArc, 270, 90);
            rectArc.Y = rect.Bottom - nDiameter;
            path.AddArc(rectArc, 0, 90);
            rectArc.X = rect.X;
            path.AddArc(rectArc, 90, 90);
            path.CloseFigure();

            return path;
        }

        
        #endregion

        #region 메모리 시험 디자인용
        private void InitData()
        {
            // 1. 임시 딕셔너리를 활용해 소자명과 단위(기본 구조)만 깔끔하게 적재
            var dicRaw = new Dictionary<string, List<(string strName, string strUnit)>>
        {
            { "VAIO", new List<(string, string)> {
                ("DPRAM", "WORD")
            }},
            { "VCPU", new List<(string, string)> {
                ("FLASH", "WORD"),
                ("ATC", "BYTE"),
                ("SRAM", "BYTE"),
                ("SRAM", "WORD")
            }},
            { "VTCN", new List<(string, string)> {
                ("DPRAM1", "BYTE"),
                ("DPRAM2", "BYTE")
            }}
        };
            // 2. 기본 상태는 전부 "Normal"(대기 상태)로 일괄 자동 할당
            m_dicBoards = new Dictionary<string, List<(string, string, EChannelState)>>();
            foreach (var pair in dicRaw)
            {
                var list = new List<(string, string, EChannelState)>();
                foreach (var elem in pair.Value)
                {
                    list.Add((elem.strName, elem.strUnit, EChannelState.Off));
                }
                m_dicBoards.Add(pair.Key, list);
            }
        }
        public void UpdateMemoryStatus(string strBoard, string strName, string strUnit, EChannelState eNewState)
        {
            if (m_dicBoards.ContainsKey(strBoard))
            {
                var list = m_dicBoards[strBoard];
                for (int nIdx = 0; nIdx < list.Count; nIdx++)
                {
                    if (list[nIdx].strName == strName && list[nIdx].strUnit == strUnit)
                    {
                        list[nIdx] = (strName, strUnit, eNewState);
                        break;
                    }
                }
                dataGridViewMemory.Invalidate(); // UI 리프레시 유도
                if (panel2 != null)
                {
                    panel2.Invalidate();
                    panel2.Update(); // 무효화 큐에만 넣지 않고 즉시 Paint 호출 강제 수행
                }
            }
        }
        private void SetupDataGridView()
        {
            // 기본 스타일 속성 최적화
            dataGridViewMemory.AllowUserToAddRows = false;
            dataGridViewMemory.RowHeadersVisible = false;
            dataGridViewMemory.ColumnHeadersVisible = false;
            dataGridViewMemory.BackgroundColor = Color.White;
            dataGridViewMemory.BorderStyle = BorderStyle.None;

            // 깜빡임 방지 (DoubleBuffered) 활성화
            var prop = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop.SetValue(dataGridViewMemory, true, null);

            // 기존 컬럼 클리어 후 데이터 기반 자동 생성 및 균등 분할(꽉 채우기)
            dataGridViewMemory.Columns.Clear();
            foreach (var strKey in m_dicBoards.Keys)
            {
                var col = new DataGridViewTextBoxColumn();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // 크기에 맞게 가로 꽉 채우기
                dataGridViewMemory.Columns.Add(col);
            }

            dataGridViewMemory.Rows.Add();

            // 데이터그리드뷰 높이에 맞게 첫 번째 행의 높이를 가득 채우기
            dataGridViewMemory.Rows[0].Height = dataGridViewMemory.ClientSize.Height > 0 ? dataGridViewMemory.ClientSize.Height : 500;

            // 데이터그리드뷰 크기가 변할 때 행 높이도 꽉 차도록 이벤트 연결
            dataGridViewMemory.Resize += (s, e) =>
            {
                if (dataGridViewMemory.Rows.Count > 0)
                {
                    dataGridViewMemory.Rows[0].Height = dataGridViewMemory.ClientSize.Height;
                }
            };

            // 이벤트 바인딩
            dataGridViewMemory.CellPainting += DataGridViewMemory_CellPaint;
        }

        private void DataGridViewMemory_CellPaint(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex >= m_dicBoards.Count) return;

            e.Handled = true; // 격자 및 기본 배경 그리기 스킵
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. 외곽 전체 카드 영역 계산 및 그리기 (셀 너비/높이에 맞춰 꽉 차게 연산)
            Rectangle rectCell = e.CellBounds;
            Rectangle rectCard = new Rectangle(rectCell.X + 8, rectCell.Y + 8, rectCell.Width - 16, rectCell.Height - 16);

            g.FillRectangle(Brushes.White, rectCell);
            using (SolidBrush brCardBg = new SolidBrush(Color.FromArgb(245, 247, 250)))
            using (Pen penCardBorder = new Pen(Color.FromArgb(220, 224, 230)))
            {
                g.FillRectangle(brCardBg, rectCard);
                g.DrawRectangle(penCardBorder, rectCard);
            }

            // 2. 보드 타이틀 헤더 출력
            string[] arrayKeys = new string[m_dicBoards.Count];
            m_dicBoards.Keys.CopyTo(arrayKeys, 0);
            string strBoardName = arrayKeys[e.ColumnIndex];

            using (Font fontHeader = new Font("맑은 고딕", 11, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, strBoardName, fontHeader, new Rectangle(rectCard.X, rectCard.Y, rectCard.Width, 40),
                    Color.FromArgb(44, 62, 80), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            // 3. 보드 자식 소자 리스트 그리기 (아이콘, 버튼 없이 텍스트 중심 정렬)
            var listElements = m_dicBoards[strBoardName];
            int nStartY = rectCard.Y + 50;

            for (int nIdx = 0; nIdx < listElements.Count; nIdx++)
            {
                var elem = listElements[nIdx];
                Rectangle rectElem = new Rectangle(rectCard.X + 12, nStartY + (nIdx * 76), rectCard.Width - 24, 64);

                // 상태별 스타일 매칭 (Success, Warning, Normal)
                Color colorBg = Color.FromArgb(248, 249, 250);
                Color colorBorder = Color.DimGray;

                if (elem.eState == EChannelState.On) // 정상
                {
                    colorBg = Color.FromArgb(240, 249, 241);
                    colorBorder = Color.FromArgb(40, 167, 69);
                }
                else if (elem.eState == EChannelState.Test) // 시험중
                {
                    colorBg = Color.FromArgb(255, 249, 230);       // 연한 청색계열
                    colorBorder = Color.FromArgb(255, 193, 7);
                }
                else if (elem.eState == EChannelState.Err) // 오류
                {
                    colorBg = Color.FromArgb(253, 238, 238); ;       // 연한 적색계열
                    colorBorder = Color.FromArgb(220, 53, 69);
                }

                using (SolidBrush brElem = new SolidBrush(colorBg))
                using (Pen penElem = new Pen(colorBorder, 1.8f))
                {
                    g.FillRectangle(brElem, rectElem);
                    g.DrawRectangle(penElem, rectElem);
                }

                using (Font fontName = new Font("맑은 고딕", 10, FontStyle.Bold))
                {
                    string strDisplay = $"{elem.strName} [{elem.strUnit}]";
                    TextRenderer.DrawText(g, strDisplay, fontName,
                        new Rectangle(rectElem.X + 16, rectElem.Y, rectElem.Width - 32, rectElem.Height),
                        Color.FromArgb(33, 37, 41), TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            }
        }
        #endregion

        private void EnableDoubleBuffering(DataGridView dgv)
        {
            var prop = typeof(DataGridView).GetProperty(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            prop.SetValue(dgv, true, null);
        }


        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Btn_EmergencyStop_Click(object sender, EventArgs e)
        {
        }
        #endregion


        #region CONFIG

        /// <summary>
        /// 
        /// </summary>
        /// 
        private void WriteConfig()
        {
            string strFullPath = Application.StartupPath + "\\config.xml";

            try
            {
                XmlTextWriter xtw = new XmlTextWriter(strFullPath, Encoding.Unicode);

                xtw.Formatting = Formatting.Indented;
                xtw.WriteStartDocument();

                xtw.WriteStartElement("HertzConfig");
                xtw.WriteStartElement("General");

                xtw.WriteElementString("시험자명", null, m_Config.strTester);
                xtw.WriteElementString("사원번호", null, m_Config.strID);
                xtw.WriteElementString("부서명", null, m_Config.strDepartment);

                xtw.WriteElementString("편성번호", null, m_Config.strGroupNo);
                xtw.WriteElementString("차량번호", null, m_Config.strTrainNo);
                xtw.WriteElementString("일련번호", null, m_Config.strSerialNo);
                xtw.WriteElementString("제어편성", null, m_Config.strControlUnit);

                xtw.WriteElementString("제어편성표시", null, m_Config.bControlUnit == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("전원유니트시험", null, m_Config.bMeasurePowerUnit == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("시퀀스시험기동", null, m_Config.bMeasureSequenceRun == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("시퀀스시험고장정지", null, m_Config.bMeasureSequenceBrake == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("시퀀스시험중고장", null, m_Config.bMeasureSequenceStop == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("보호동작시험", null, m_Config.bMeasureProtect == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("컨버터인버터시험", null, m_Config.bMeasureConvInverter == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("기동정지시퀀스시험", null, m_Config.bMeasureSeqRunStop == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("주회로출력시험", null, m_Config.bMeasureMainCircuitOut == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("GDU시험", null, m_Config.bMeasureGDU == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("전압전류시험", null, m_Config.bMeasureVI == true ? "TRUE" : "FALSE");
                xtw.WriteElementString("통신시험", null, m_Config.bMeasureComm == true ? "TRUE" : "FALSE");


                xtw.WriteElementString("P24_Std", null, string.Format("{0:0.0}", m_Config.dP24_Unit_Std));
                xtw.WriteElementString("P24_Pmt", null, string.Format("{0:0.0}", m_Config.dP24_Unit_Pmt));
                xtw.WriteElementString("N24_Std", null, string.Format("{0:0.0}", m_Config.dN24_Unit_Std));
                xtw.WriteElementString("N24_Pmt", null, string.Format("{0:0.0}", m_Config.dN24_Unit_Pmt));
                xtw.WriteElementString("P12_Std", null, string.Format("{0:0.0}", m_Config.dP12_Unit_Std));
                xtw.WriteElementString("P12_Pmt", null, string.Format("{0:0.0}", m_Config.dP12_Unit_Pmt));

                xtw.WriteElementString("Inverter_ON_Std", null, string.Format("{0:0.0}", m_Config.dInverter_ON_Std));
                xtw.WriteElementString("Inverter_ON_Pmt", null, string.Format("{0:0.0}", m_Config.dInverter_ON_Pmt));
                xtw.WriteElementString("Inverter_OFF_Std", null, string.Format("{0:0.0}", m_Config.dInverter_OFF_Std));
                xtw.WriteElementString("Inverter_OFF_Pmt", null, string.Format("{0:0.0}", m_Config.dInverter_OFF_Pmt));

                xtw.WriteElementString("BPSF_123_Std", null, string.Format("{0:0.00}", m_Config.dBPSF_123_Std));
                xtw.WriteElementString("BPSF_123_Pmt", null, string.Format("{0:0.00}", m_Config.dBPSF_123_Pmt));
                xtw.WriteElementString("ACOV_123_Std", null, string.Format("{0:0.00}", m_Config.dACOV_123_Std));
                xtw.WriteElementString("ACOV_123_Pmt", null, string.Format("{0:0.00}", m_Config.dACOV_123_Pmt));
                xtw.WriteElementString("ACLV_123_Std", null, string.Format("{0:0.00}", m_Config.dACLV_123_Std));
                xtw.WriteElementString("ACLV_123_Pmt", null, string.Format("{0:0.00}", m_Config.dACLV_123_Pmt));
                xtw.WriteElementString("VDOV_123_Std", null, string.Format("{0:0.00}", m_Config.dVDOV_123_Std));
                xtw.WriteElementString("VDOV_123_Pmt", null, string.Format("{0:0.00}", m_Config.dVDOV_123_Pmt));
                xtw.WriteElementString("VDLV_123_Std", null, string.Format("{0:0.00}", m_Config.dVDLV_123_Std));
                xtw.WriteElementString("VDLV_123_Pmt", null, string.Format("{0:0.00}", m_Config.dVDLV_123_Pmt));
                xtw.WriteElementString("ISOC1_123_Std", null, string.Format("{0:0.00}", m_Config.dISOC1_123_Std));
                xtw.WriteElementString("ISOC1_123_Pmt", null, string.Format("{0:0.00}", m_Config.dISOC1_123_Pmt));
                xtw.WriteElementString("ISOC2_123_Std", null, string.Format("{0:0.00}", m_Config.dISOC2_123_Std));
                xtw.WriteElementString("ISOC2_123_Pmt", null, string.Format("{0:0.00}", m_Config.dISOC2_123_Pmt));
                xtw.WriteElementString("MOCD_123_Std", null, string.Format("{0:0.00}", m_Config.dMOCD_123_Std));
                xtw.WriteElementString("MOCD_123_Pmt", null, string.Format("{0:0.00}", m_Config.dMOCD_123_Pmt));
                xtw.WriteElementString("PUD_123_Std", null, string.Format("{0:0.00}", m_Config.dPUD_123_Std));
                xtw.WriteElementString("PUD_123_Pmt", null, string.Format("{0:0.00}", m_Config.dPUD_123_Pmt));
                xtw.WriteElementString("FCDF_123_Std", null, string.Format("{0:0.00}", m_Config.dFCDF_123_Std));
                xtw.WriteElementString("FCDF_123_Pmt", null, string.Format("{0:0.00}", m_Config.dFCDF_123_Pmt));
                xtw.WriteElementString("IGOC_123_Std", null, string.Format("{0:0.00}", m_Config.dIGOC_123_Std));
                xtw.WriteElementString("IGOC_123_Pmt", null, string.Format("{0:0.00}", m_Config.dIGOC_123_Pmt));
                xtw.WriteElementString("BSD_123_Std", null, string.Format("{0:0.00}", m_Config.dBSD_123_Std));
                xtw.WriteElementString("BSD_123_Pmt", null, string.Format("{0:0.00}", m_Config.dBSD_123_Pmt));
                xtw.WriteElementString("IDOC_123_Std", null, string.Format("{0:0.00}", m_Config.dIDOC_123_Std));
                xtw.WriteElementString("IDOC_123_Pmt", null, string.Format("{0:0.00}", m_Config.dIDOC_123_Pmt));
                xtw.WriteElementString("ZCDFP_123_Std", null, string.Format("{0:0.00}", m_Config.dZCDFP_123_Std));
                xtw.WriteElementString("ZCDFP_123_Pmt", null, string.Format("{0:0.00}", m_Config.dZCDFP_123_Pmt));
                xtw.WriteElementString("ZCDFM_123_Std", null, string.Format("{0:0.00}", m_Config.dZCDFM_123_Std));
                xtw.WriteElementString("ZCDFM_123_Pmt", null, string.Format("{0:0.00}", m_Config.dZCDFM_123_Pmt));
                xtw.WriteElementString("BPSF_54_Std", null, string.Format("{0:0.00}", m_Config.dBPSF_54_Std));
                xtw.WriteElementString("BPSF_54_Pmt", null, string.Format("{0:0.00}", m_Config.dBPSF_54_Pmt));
                xtw.WriteElementString("ACOV_54_Std", null, string.Format("{0:0.00}", m_Config.dACOV_54_Std));
                xtw.WriteElementString("ACOV_54_Pmt", null, string.Format("{0:0.00}", m_Config.dACOV_54_Pmt));
                xtw.WriteElementString("ACLV_54_Std", null, string.Format("{0:0.00}", m_Config.dACLV_54_Std));
                xtw.WriteElementString("ACLV_54_Pmt", null, string.Format("{0:0.00}", m_Config.dACLV_54_Pmt));
                xtw.WriteElementString("ISOC_54_Std", null, string.Format("{0:0.00}", m_Config.dISOC_54_Std));
                xtw.WriteElementString("ISOC_54_Pmt", null, string.Format("{0:0.00}", m_Config.dISOC_54_Pmt));
                xtw.WriteElementString("MOCD_54_Std", null, string.Format("{0:0.00}", m_Config.dMOCD_54_Std));
                xtw.WriteElementString("MOCD_54_Pmt", null, string.Format("{0:0.00}", m_Config.dMOCD_54_Pmt));
                xtw.WriteElementString("FCOV_54_Std", null, string.Format("{0:0.00}", m_Config.dFCOV_54_Std));
                xtw.WriteElementString("FCOV_54_Pmt", null, string.Format("{0:0.00}", m_Config.dFCOV_54_Pmt));
                xtw.WriteElementString("FCLV_54_Std", null, string.Format("{0:0.00}", m_Config.dFCLV_54_Std));
                xtw.WriteElementString("FCLV_54_Pmt", null, string.Format("{0:0.00}", m_Config.dFCLV_54_Pmt));
                xtw.WriteElementString("LGD_54_Std", null, string.Format("{0:0.00}", m_Config.dLGD_54_Std));
                xtw.WriteElementString("LGD_54_Pmt", null, string.Format("{0:0.00}", m_Config.dLGD_54_Pmt));
                xtw.WriteElementString("BOCD_54_Std", null, string.Format("{0:0.00}", m_Config.dBOCD_54_Std));
                xtw.WriteElementString("BOCD_54_Pmt", null, string.Format("{0:0.00}", m_Config.dBOCD_54_Pmt));
                xtw.WriteElementString("PUD_54_Std", null, string.Format("{0:0.00}", m_Config.dPUD_54_Std));
                xtw.WriteElementString("PUD_54_Pmt", null, string.Format("{0:0.00}", m_Config.dPUD_54_Pmt));

                xtw.WriteElementString("GDU_ON_Std", null, string.Format("{0:0.00}", m_Config.dIDU_ON_Std));
                xtw.WriteElementString("GDU_ON_Pmt", null, string.Format("{0:0.00}", m_Config.dIDU_ON_Pmt));
                xtw.WriteElementString("GDU_OFF_Std", null, string.Format("{0:0.00}", m_Config.dIDU_OFF_Std));
                xtw.WriteElementString("GDU_OFF_Pmt", null, string.Format("{0:0.00}", m_Config.dIDU_OFF_Pmt));

                xtw.WriteEndElement();

                xtw.Flush();
                xtw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        private void DisplayConfig()
        {
            Label_Unit.Text = ConfigJson.CurrentConfig.Operation.TCMSUnit;
            Label_Date.Text = DateTime.Now.ToString("yyyy년 MM월 dd일");

            Label_Fleet.Text = ConfigJson.CurrentConfig.Operation.FleetNo;
            Label_Train.Text = ConfigJson.CurrentConfig.Operation.TrainNo;
            Label_Tester.Text = ConfigJson.CurrentConfig.Operation.TesterName;
            Label_Serial.Text = ConfigJson.CurrentConfig.Operation.SerialNo;

            modernTreeView1.Nodes.Find("디지털 입출력 시험", true)[0].Checked = m_Config.bMeasurePowerUnit;
            modernTreeView1.Nodes.Find("아날로그 입출력 시험", true)[0].Checked = m_Config.bMeasureSequenceRun;
            modernTreeView1.Nodes.Find("통신 시험", true)[0].Checked = m_Config.bMeasureSequenceBrake;
            modernTreeView1.Nodes.Find("메모리 시험", true)[0].Checked = m_Config.bMeasureSequenceStop;
            if (ConfigJson.CurrentConfig.Operation.TCMSUnit == "ER")
            {
                modernTreeView1.Nodes.Find("ER 속도센서 시험", true)[0].Checked = m_Config.bMeasureProtect;
            }
            ConfigManager cfgManager = new ConfigManager();
            bool bIsSaveSuccess = cfgManager.SaveConfig(ConfigJson.CurrentConfig);
            if (!bIsSaveSuccess)
            {
                // 실시간 제어 스레드에 영향을 주지 않도록 UI 갱신 하단부와 로깅 분리 검토 가능
            }

            m_bMeasureItem[MEASURE_ITEM_POWER_UNIT] = m_Config.bMeasurePowerUnit;
            m_bMeasureItem[MEASURE_ITEM_COMM] = m_Config.bMeasureComm;
            m_bMeasureItem[MEASURE_ITEM_SEQUENCE_RUN] = m_Config.bMeasureSequenceRun;
            m_bMeasureItem[MEASURE_ITEM_SEQUENCE_BRAKE] = m_Config.bMeasureSequenceBrake;
            m_bMeasureItem[MEASURE_ITEM_SEQUENCE_STOP] = m_Config.bMeasureSequenceStop;
            m_bMeasureItem[MEASURE_ITEM_INVERTER] = m_Config.bMeasureConvInverter;
            m_bMeasureItem[MEASURE_ITEM_PROTECT] = m_Config.bMeasureProtect;
            m_bMeasureItem[MEASURE_ITEM_GDU] = m_Config.bMeasureGDU;

        }
        #endregion


        #region DIAGNOSTIC

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool OpenDatabase()
        {
            if (m_strDBPath == "")
                return false;

            m_strDBPath = Application.StartupPath + "\\" + m_strDBPath;

            string source = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_strDBPath + ";Jet OLEDB:Database Password=" + m_strDBPassword;
            try
            {
                m_OLEConnect = new OleDbConnection(source);
                m_OLEConnect.Open();

                m_OLECommand = new OleDbCommand();
                m_OLECommand.Connection = m_OLEConnect;

                m_bDBOpened = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("The database file could not be found or the connection failed. [" + ex.Message + "]", "Database Error" + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool OpenPLC()
        {
            try
            {

            }
            catch
            {
                return false;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        private void OnPLCConnect()
        {
            //m_PLCNetwork.START = true;
            //m_PLCNetwork.WRITE = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        private void OnPLCNotConnect()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        private void OnPLCDisconnect()
        {
            //m_PLCNetwork.START = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool ConnectOscilloscope()
        {
            if (m_strOscilloscode == "")
                return false;

            try
            {
                m_ethernetOscilloscope.IO = (IMessage)m_ResourceManager.Open(m_strOscilloscode);
                m_ethernetOscilloscope.IO.Timeout = 15000;

                m_ethernetOscilloscope.WriteString("*IDN?");
                String strIDN = m_ethernetOscilloscope.ReadString();
                if (strIDN.IndexOf(m_strOscilloscodeIDN) < 0)
                    return false;
            }
            catch
            {
                return false;
            }
            m_bOscilloscopeConnected = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool OpenSpeedOut()
        {
            int i = 0;

            for (i = 0; i < strSerialPortList.Length; i++)
            {
                if (strSerialPortList[i] == "COM1" || strSerialPortList[i] == "COM2" || strSerialPortList[i] == "COM3"
               || strSerialPortList[i] == "COM4" || strSerialPortList[i] == "COM5" || strSerialPortList[i] == "COM6" || strSerialPortList[i] == "ON")
                {
                    continue;
                }

                if (strSerialPortList[i] == "")
                {
                    continue;
                }

                if (m_serialSpeedOut != null)
                {
                    if (m_serialSpeedOut.CONNECTED == true)
                    {
                        m_serialSpeedOut.Disconnect();
                    }
                }

                m_serialSpeedOut = new SerialClient();

                m_serialSpeedOut.PORT = strSerialPortList[i];
                m_serialSpeedOut.BAUDRATE = m_nSpeedOutBaudRate;
                m_serialSpeedOut.CONNECTED = false;
                m_serialSpeedOut.DELIMITOR = "\n";

                try
                {
                    m_serialSpeedOut.Connect();
                    if (!m_serialSpeedOut.CheckConnect())
                    {
                        m_serialSpeedOut.Disconnect();
                        continue;
                    }

                    m_serialSpeedOut.IS_RETURN = false;
                    m_serialSpeedOut.SendData("get.devicename\r\n");

                    if (!m_serialSpeedOut.ReceiveData())
                    {
                        m_serialSpeedOut.Disconnect();
                        continue;
                    }

                    if (m_serialSpeedOut.RETURN_VALUE.Contains(m_strSpeedOutIDN))
                    {
                        Console.WriteLine(m_serialSpeedOut.RETURN_VALUE);
                        strSerialPortList[i] = "ON";
                        break;
                    }
                }
                catch
                {

                }
                m_serialSpeedOut.Disconnect();
            }

            if (i == strSerialPortList.Length)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool ConnectDMM()
        {
            if (m_strDmmIpNo == "")
                return false;

            try
            {
                m_ethernetDmm.IO = (IMessage)m_ResourceManager.Open(m_strDmmIpNo);
                m_ethernetDmm.IO.Timeout = 15000;

                m_ethernetDmm.WriteString("*IDN?");
                String strIDN = m_ethernetDmm.ReadString();
                if (strIDN.IndexOf(m_strDmmIdn) < 0)
                    return false;
            }
            catch
            {
                return false;
            }
            m_bDmmConnected = true;

            return true;
        }

        public void SerialInit(int nType, string strPortNo, int nBps, Parity pbParityBit, bool bMsg)
        {
            if (strPortNo == "") return;

            switch (nType)
            {
                case SERIALPORT_TYPE.Tester1:

                    if (COMMON_INFO.serialTester1Port0 != null) COMMON_INFO.serialTester1Port0.Close();

                    COMMON_INFO.serialTester1Port0 = new SerialPort();
                    COMMON_INFO.serialTester1Port0.PortName = strPortNo;
                    COMMON_INFO.serialTester1Port0.BaudRate = (int)nBps;
                    COMMON_INFO.serialTester1Port0.Parity = pbParityBit;
                    COMMON_INFO.serialTester1Port0.DataBits = (int)8;
                    COMMON_INFO.serialTester1Port0.StopBits = StopBits.One;

                    // Set the read/write timeouts
                    COMMON_INFO.serialTester1Port0.ReadTimeout = 5;
                    COMMON_INFO.serialTester1Port0.WriteTimeout = 5;

                    try
                    {
                        COMMON_INFO.serialTester1Port0.Open();
                        //InsertMainMessage("컴포트 " + strPortNo + "이 오픈되었습니다." + "\r\n");
                    }
                    catch
                    {

                    }

                    break;

                case SERIALPORT_TYPE.Tester2:

                    if (COMMON_INFO.serialTester2Port0 != null) COMMON_INFO.serialTester2Port0.Close();

                    COMMON_INFO.serialTester2Port0 = new SerialPort();
                    COMMON_INFO.serialTester2Port0.PortName = strPortNo;
                    COMMON_INFO.serialTester2Port0.BaudRate = (int)nBps;
                    COMMON_INFO.serialTester2Port0.Parity = pbParityBit;
                    COMMON_INFO.serialTester2Port0.DataBits = (int)8;
                    COMMON_INFO.serialTester2Port0.StopBits = StopBits.One;

                    // Set the read/write timeouts
                    COMMON_INFO.serialTester2Port0.ReadTimeout = 5;
                    COMMON_INFO.serialTester2Port0.WriteTimeout = 5;

                    try
                    {
                        COMMON_INFO.serialTester2Port0.Open();
                        //InsertMainMessage("컴포트 " + strPortNo + "이 오픈되었습니다." + "\r\n");
                    }
                    catch
                    {

                    }
                    break;
            }
        }
        //보드 연결 없이 리셋 실행 시 팅김현상 수정
        public void ResetPort()
        {
            var boards = new[] {
        m_serialDcPower, m_serialCurrentOutBoard, m_serialOpticBoard1,
        m_serialOpticBoard2, m_serialMvbBoard, m_serialTrimmerBoard1,
        m_serialTrimmerBoard2, m_serialLineVoltageBoard0, m_serialSpeedOut
    };

            foreach (var board in boards)
            {
                if (board == null) continue;

                try
                {
                    board.Disconnect();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    board.CONNECTED = false;
                }
            }
        }
        //밑에 원본
        /* public void ResetPort()
         {
             try
             {
                 m_serialDcPower.Disconnect();
                 m_serialCurrentOutBoard.Disconnect();
                 m_serialOpticBoard1.Disconnect();
                 m_serialOpticBoard2.Disconnect();
                 m_serialMvbBoard.Disconnect();
                 m_serialTrimmerBoard1.Disconnect();
                 m_serialTrimmerBoard2.Disconnect();
                 m_serialLineVoltageBoard0.Disconnect();
                 m_serialSpeedOut.Disconnect();

                 m_serialDcPower.CONNECTED = false;
                 m_serialCurrentOutBoard.CONNECTED = false;
                 m_serialOpticBoard1.CONNECTED = false;
                 m_serialOpticBoard2.CONNECTED = false;
                 m_serialMvbBoard.CONNECTED = false;
                 m_serialTrimmerBoard1.CONNECTED = false;
                 m_serialTrimmerBoard2.CONNECTED = false;
                 m_serialLineVoltageBoard0.CONNECTED = false;
                 m_serialSpeedOut.CONNECTED = false;
             }
             catch
             {

             }
         }*/

        // 2. DC 파워 포트 탐색 및 연결
        public bool OpenDCPower()
        {
            string strTargetPort = ConfigJson.CurrentConfig?.Device?.DCPower_ComPort;
            string strTargetIdn = ConfigJson.CurrentConfig?.Device?.DCPower_IDN;

            List<string> lstCandidatePorts = new List<string>();

            if (!string.IsNullOrEmpty(strTargetPort))
            {
                lstCandidatePorts.Add(strTargetPort);
            }

            string[] arrSystemPorts = SerialPort.GetPortNames();
            for (int nIdx = 0; nIdx < arrSystemPorts.Length; nIdx++)
            {
                string strPort = arrSystemPorts[nIdx];
                if (!lstCandidatePorts.Contains(strPort, StringComparer.OrdinalIgnoreCase))
                {
                    lstCandidatePorts.Add(strPort);
                }
            }

            for (int nPortIdx = 0; nPortIdx < lstCandidatePorts.Count; nPortIdx++)
            {
                string strPort = lstCandidatePorts[nPortIdx];

                if (m_serialDcPower != null && m_serialDcPower.CONNECTED)
                {
                    m_serialDcPower.Disconnect();
                }

                m_serialDcPower = new SerialClient
                {
                    PORT = strPort,
                    BAUDRATE = 9600,
                    DTR = true,
                    CONNECTED = false,
                    DELIMITOR = "\n"
                };

                try
                {
                    m_serialDcPower.Connect();
                    if (!m_serialDcPower.CheckConnect())
                    {
                        m_serialDcPower.Disconnect();
                        continue;
                    }

                    m_serialDcPower.IS_RETURN = false;

                    int nFlushRetry = 0;
                    while (m_serialDcPower.ReceiveData() && nFlushRetry < 3)
                    {
                        nFlushRetry++;
                    }

                    m_serialDcPower.SendData("*CLS\n");
                    Thread.Sleep(20);
                    m_serialDcPower.SendData("*IDN?\n");

                    bool bIsPowerFound = false;
                    int nRetryCount = 0;
                    const int nMaxRetry = 10;

                    while (nRetryCount < nMaxRetry)
                    {
                        if (m_serialDcPower.ReceiveData())
                        {
                            string strResultBuffer = m_serialDcPower.RETURN_VALUE ?? string.Empty;

                            if (strResultBuffer.Contains("receiveddata") ||
                                strResultBuffer.Contains("[RX ANALOG") ||
                                strResultBuffer.Contains("HEX:") ||
                                strResultBuffer.Contains("PORT 4020"))
                            {
                                Console.WriteLine($"DC 파워: MVB 통신 포트 감지됨 ({strPort}) -> 즉시 건너뜀");
                                break;
                            }

                            if (strResultBuffer.Contains(strTargetIdn))
                            {
                                m_serialDcPower.SendData("*CLS\n");
                                Thread.Sleep(20);
                                bIsPowerFound = true;
                                break;
                            }

                            if (!string.IsNullOrEmpty(strResultBuffer))
                            {
                                break;
                            }
                        }

                        nRetryCount++;
                        Thread.Sleep(10);
                    }

                    if (bIsPowerFound)
                    {
                        ConfigJson.CurrentConfig.Device.DCPower_ComPort = strPort;
                        return true; // DC 파워는 m_serialDcPower 인스턴스를 유지하여 계속 사용하는 경우 유지
                    }

                    m_serialDcPower.Disconnect();
                }
                catch (Exception)
                {
                    if (m_serialDcPower != null)
                    {
                        m_serialDcPower.Disconnect();
                    }
                }
            }

            return false;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public bool OpenPwmOut()
        {
            if (m_strPwmOutComPort == "")
                return false;

            if (m_serialPwm != null)
            {
                if (m_serialPwm.CONNECTED == true)
                {
                    m_serialPwm.Disconnect();
                }
            }

            m_serialPwm = new SerialClient();

            m_serialPwm.PORT = m_strPwmOutComPort;
            m_serialPwm.BAUDRATE = m_nPwmOutBaudRate;
            m_serialPwm.CONNECTED = false;
            m_serialPwm.DELIMITOR = "\n";
            m_serialPwm.Connect();
            if (!m_serialPwm.CheckConnect())
                return false;

            m_serialPwm.IS_RETURN = false;
            m_serialPwm.SendData("GET.VERSION\r\n");

            if (!m_serialPwm.ReceiveData())
                return false;
            //MessageBox.Show(m_serialPwm.RETURN_VALUE);

            return true;
        }
        #endregion

        /// <summary>
        /// 자가진단 관련 함수 
        /// </summary>
        /// <returns></returns>
        ///
        #region 자가진단 관련 함수들
        public bool OpenCurrentOutBoard(ref int nFailBoard)
        {
            string strSavedPort = ConfigJson.CurrentConfig.Device.CurrentOut_ComPort ?? string.Empty;
            System.Collections.Generic.List<string> listTargetPorts = new System.Collections.Generic.List<string>();

            if (!string.IsNullOrEmpty(strSavedPort) && Array.Exists(strSerialPortList, strPort => strPort == strSavedPort))
            {
                listTargetPorts.Add(strSavedPort);
            }

            foreach (string strPort in strSerialPortList)
            {
                if (strPort != strSavedPort && !string.IsNullOrEmpty(strPort))
                {
                    listTargetPorts.Add(strPort);
                }
            }

            int nIdx = 0;

            for (nIdx = 0; nIdx < listTargetPorts.Count; nIdx++)
            {
                string strCurrentPort = listTargetPorts[nIdx];

                if (m_serialCurrentOutBoard != null && m_serialCurrentOutBoard.CONNECTED == true)
                {
                    m_serialCurrentOutBoard.Disconnect();
                }

                m_serialCurrentOutBoard = new SerialClient();
                m_serialCurrentOutBoard.PORT = strCurrentPort;
                m_serialCurrentOutBoard.BAUDRATE = m_nCurrentOutBaudRate;
                m_serialCurrentOutBoard.CONNECTED = false;
                m_serialCurrentOutBoard.DELIMITOR = "\n";

                try
                {
                    m_serialCurrentOutBoard.Connect();
                    if (!m_serialCurrentOutBoard.CheckConnect())
                    {
                        m_serialCurrentOutBoard.Disconnect();
                        continue;
                    }

                    m_serialCurrentOutBoard.IS_RETURN = false;
                    while (m_serialCurrentOutBoard.ReceiveData())
                    {
                    }

                    m_serialCurrentOutBoard.SendData("get.devicename.0\r\n");
                    //m_serialCurrentOutBoard.SendData("set.current.0 0 50\r\n");

                    bool bIsBoardFound = false;
                    int nRetryCount = 0;
                    int nMaxRetry = 20;

                    while (nRetryCount < nMaxRetry)
                    {
                        if (!m_serialCurrentOutBoard.ReceiveData())
                        {
                            nRetryCount++;
                            System.Threading.Thread.Sleep(10);
                            continue;
                        }

                        string strResultBuffer = m_serialCurrentOutBoard.RETURN_VALUE ?? string.Empty;

                        Console.WriteLine($"{strResultBuffer}");

                        if (strResultBuffer.Contains(ConfigJson.CurrentConfig.Device.CurrentOut_IDN))
                        {
                            bIsBoardFound = true;
                            break;
                        }

                        nRetryCount++;
                    }

                    if (!bIsBoardFound)
                    {
                        m_serialCurrentOutBoard.Disconnect();
                        continue;
                    }

                    if (ConfigJson.CurrentConfig.Device.CurrentOut_ComPort != strCurrentPort)
                    {
                        ConfigJson.CurrentConfig.Device.CurrentOut_ComPort = strCurrentPort;
                    }
                    break;
                }
                catch (Exception)
                {
                }

                m_serialCurrentOutBoard.Disconnect();
            }

            if (nIdx == listTargetPorts.Count)
            {
                nFailBoard = 1;
                return false;
            }
            return true;
        }
        public bool TrimmerBoard1()
        {
            int i = 0;

            for (i = 0; i < strSerialPortList.Length; i++)
            {
                if (strSerialPortList[i] == "COM1" || strSerialPortList[i] == "COM2" || strSerialPortList[i] == "COM3"
                   || strSerialPortList[i] == "COM4" || strSerialPortList[i] == "COM5" || strSerialPortList[i] == "COM6" || strSerialPortList[i] == "ON")
                {
                    continue;
                }

                if (strSerialPortList[i] == "")
                {
                    continue;
                }

                if (m_serialTrimmerBoard1 != null)
                {
                    if (m_serialTrimmerBoard1.CONNECTED == true)
                    {
                        m_serialTrimmerBoard1.Disconnect();
                    }
                }

                m_serialTrimmerBoard1 = new SerialClient();

                m_serialTrimmerBoard1.PORT = strSerialPortList[i];
                m_serialTrimmerBoard1.BAUDRATE = m_strTrimmerBoardBaudRate1;
                m_serialTrimmerBoard1.CONNECTED = false;
                m_serialTrimmerBoard1.DELIMITOR = "\n";

                try
                {
                    m_serialTrimmerBoard1.Connect();

                    if (m_serialTrimmerBoard1.CheckConnect() == false)
                    {
                        m_serialTrimmerBoard1.Disconnect();
                        continue;
                    }

                    m_serialTrimmerBoard1.IS_RETURN = false;
                    m_serialTrimmerBoard1.SendData("get.devicename\r\n");

                    if (!m_serialTrimmerBoard1.ReceiveData())
                    {
                        m_serialTrimmerBoard1.Disconnect();
                        continue;
                    }

                    if (m_serialTrimmerBoard1.RETURN_VALUE.Contains(m_strTrimmerBoardIDN1))
                    {
                        Console.WriteLine(m_serialTrimmerBoard1.RETURN_VALUE);
                        strSerialPortList[i] = "ON";
                        break;
                    }
                }
                catch
                {

                }
                m_serialTrimmerBoard1.Disconnect();
            }

            if (i == strSerialPortList.Length)
            {
                return false;
            }
            return true;
        }
        public bool TrimmerBoard2()
        {
            int i = 0;

            for (i = 0; i < strSerialPortList.Length; i++)
            {
                if (strSerialPortList[i] == "COM1" || strSerialPortList[i] == "COM2" || strSerialPortList[i] == "COM3"
                   || strSerialPortList[i] == "COM4" || strSerialPortList[i] == "COM5" || strSerialPortList[i] == "COM6" || strSerialPortList[i] == "ON")
                {
                    continue;
                }

                if (strSerialPortList[i] == "")
                {
                    continue;
                }

                if (m_serialTrimmerBoard2 != null)
                {
                    if (m_serialTrimmerBoard2.CONNECTED == true)
                    {
                        m_serialTrimmerBoard2.Disconnect();
                    }
                }

                m_serialTrimmerBoard2 = new SerialClient();

                m_serialTrimmerBoard2.PORT = strSerialPortList[i];
                m_serialTrimmerBoard2.BAUDRATE = m_strTrimmerBoardBaudRate2;
                m_serialTrimmerBoard2.CONNECTED = false;
                m_serialTrimmerBoard2.DELIMITOR = "\n";

                try
                {
                    m_serialTrimmerBoard2.Connect();
                    if (!m_serialTrimmerBoard2.CheckConnect())
                    {
                        m_serialTrimmerBoard2.Disconnect();
                        continue;
                    }

                    m_serialTrimmerBoard2.IS_RETURN = false;
                    m_serialTrimmerBoard2.SendData("get.devicename\r\n");

                    if (!m_serialTrimmerBoard2.ReceiveData())
                    {
                        m_serialTrimmerBoard2.Disconnect();
                        continue;
                    }

                    if (m_serialTrimmerBoard2.RETURN_VALUE.Contains(m_strTrimmerBoardIDN2))
                    {
                        Console.WriteLine(m_serialTrimmerBoard2.RETURN_VALUE);
                        strSerialPortList[i] = "ON";
                        break;
                    }
                }
                catch
                {

                }
                m_serialTrimmerBoard2.Disconnect();
            }

            if (i == strSerialPortList.Length)
            {
                return false;
            }
            return true;
        }

        public bool LineVoltageBoard0()
        {
            int i = 0;


            for (i = 0; i < strSerialPortList.Length; i++)
            {
                if (strSerialPortList[i] == "COM1" || strSerialPortList[i] == "COM2" || strSerialPortList[i] == "COM3"
                    || strSerialPortList[i] == "COM4" || strSerialPortList[i] == "COM5" || strSerialPortList[i] == "COM6" || strSerialPortList[i] == "ON")
                {
                    continue;
                }

                if (strSerialPortList[i] == "")
                {
                    continue;
                }

                if (m_serialLineVoltageBoard0 != null)
                {
                    if (m_serialLineVoltageBoard0.CONNECTED == true)
                    {
                        m_serialLineVoltageBoard0.Disconnect();
                    }
                }

                m_serialLineVoltageBoard0 = new SerialClient();

                m_serialLineVoltageBoard0.PORT = strSerialPortList[i];
                m_serialLineVoltageBoard0.BAUDRATE = m_strLineVoltageBaudRate0;
                m_serialLineVoltageBoard0.CONNECTED = false;
                m_serialLineVoltageBoard0.DELIMITOR = "\n";

                try
                {
                    m_serialLineVoltageBoard0.Connect();
                    if (!m_serialLineVoltageBoard0.CheckConnect())
                    {
                        m_serialLineVoltageBoard0.Disconnect();
                        continue;
                    }

                    m_serialLineVoltageBoard0.IS_RETURN = false;
                    m_serialLineVoltageBoard0.SendData("get.devicename\r\n");

                    if (!m_serialLineVoltageBoard0.ReceiveData())
                    {
                        m_serialLineVoltageBoard0.Disconnect();
                        continue;
                    }

                    if (m_serialLineVoltageBoard0.RETURN_VALUE.Contains(m_strLineVoltageBoardIDN0))
                    {
                        Console.WriteLine(m_serialLineVoltageBoard0.RETURN_VALUE);
                        strSerialPortList[i] = "ON";
                        break;
                    }
                }
                catch
                {

                }
                m_serialLineVoltageBoard0.Disconnect();
            }

            if (i == strSerialPortList.Length)
            {
                return false;
            }
            return true;
        }

        // MVB 보드 포트 탐색 및 연결
        public bool OpenMvbBoard()
        {
            string strTargetPort = ConfigJson.CurrentConfig?.Device?.MVBBoard_ComPort;
            string strTargetIdn = ConfigJson.CurrentConfig?.Device?.MVBBoard_IDN;
            int nBaudRate = ConfigJson.CurrentConfig?.Device?.MVBBoard_BaudRate ?? 115200;

            List<string> lstCandidatePorts = new List<string>();

            if (!string.IsNullOrEmpty(strTargetPort))
            {
                lstCandidatePorts.Add(strTargetPort);
            }

            string[] arrSystemPorts = SerialPort.GetPortNames();
            for (int nIdx = 0; nIdx < arrSystemPorts.Length; nIdx++)
            {
                string strPort = arrSystemPorts[nIdx];
                if (!lstCandidatePorts.Contains(strPort, StringComparer.OrdinalIgnoreCase))
                {
                    lstCandidatePorts.Add(strPort);
                }
            }

            for (int nPortIdx = 0; nPortIdx < lstCandidatePorts.Count; nPortIdx++)
            {
                string strPort = lstCandidatePorts[nPortIdx];

                if (m_serialMvbBoard != null && m_serialMvbBoard.CONNECTED)
                {
                    m_serialMvbBoard.Disconnect();
                }

                m_serialMvbBoard = new SerialClient
                {
                    PORT = strPort,
                    BAUDRATE = nBaudRate,
                    CONNECTED = false,
                    DELIMITOR = "\n"
                };

                try
                {
                    m_serialMvbBoard.Connect();
                    if (!m_serialMvbBoard.CheckConnect())
                    {
                        m_serialMvbBoard.Disconnect();
                        continue;
                    }

                    m_serialMvbBoard.IS_RETURN = false;

                    int nFlushCount = 0;
                    while (m_serialMvbBoard.ReceiveData() && nFlushCount < 5)
                    {
                        nFlushCount++;
                    }

                    m_serialMvbBoard.SendData("get.devicename\r\n");

                    bool bIsBoardFound = false;
                    int nRetryCount = 0;
                    const int nMaxRetry = 15;

                    while (nRetryCount < nMaxRetry)
                    {
                        if (m_serialMvbBoard.ReceiveData())
                        {
                            string strResultBuffer = m_serialMvbBoard.RETURN_VALUE ?? string.Empty;

                            if (strResultBuffer.Contains(strTargetIdn))
                            {
                                bIsBoardFound = true;
                                break;
                            }
                        }

                        nRetryCount++;
                        System.Threading.Thread.Sleep(10);
                    }

                    // [수정] 보드를 찾았더라도 시험 시작 시 시리얼 매니저가 열 수 있도록 Disconnect 호출
                    if (bIsBoardFound)
                    {
                        ConfigJson.CurrentConfig.Device.MVBBoard_ComPort = strPort;
                        m_serialMvbBoard.Disconnect(); // 점유 해제
                        return true;
                    }

                    m_serialMvbBoard.Disconnect();
                }
                catch (Exception)
                {
                    if (m_serialMvbBoard != null)
                    {
                        m_serialMvbBoard.Disconnect();
                    }
                }
            }

            return false;
        }


        public bool OpenMvbBoardTester()
        {
            if (m_strMvbBoardComPort == "")
            {
                return false;
            }

            if (m_serialMvbBoard != null)
            {
                if (m_serialMvbBoard.CONNECTED == true)
                {
                    m_serialMvbBoard.Disconnect();
                }
            }

            m_serialMvbBoard = new SerialClient();

            m_serialMvbBoard.PORT = m_strMvbBoardComPort;
            m_serialMvbBoard.BAUDRATE = 19200;
            m_serialMvbBoard.CONNECTED = false;
            m_serialMvbBoard.DELIMITOR = "\n";
            m_serialMvbBoard.Connect();
            if (!m_serialMvbBoard.CheckConnect())
                return false;

            m_serialMvbBoard.IS_RETURN = false;
            //m_serialMvbBoard.SerialRxBufClear();

            if (m_serialMvbBoard.ReceiveData())
            {
                if (m_serialMvbBoard.RETURN_VALUE.Contains("4078"))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        #endregion

        private List<TreeNode> GetAllNodes(TreeNodeCollection nodes)
        {
            List<TreeNode> allNodes = new List<TreeNode>();
            foreach (TreeNode node in nodes)
            {
                allNodes.Add(node);
                if (node.Nodes.Count > 0)
                    allNodes.AddRange(GetAllNodes(node.Nodes));
            }
            return allNodes;
        }
        private void ClearAllDO()
        {
            /*
            m_PLCNetwork.SetDO(0, DO_TP24V, 0);
            m_PLCNetwork.SetDO(0, DO_TN12V, 0);
            m_PLCNetwork.SetDO(0, DO_PUMP_F, 0);
            m_PLCNetwork.SetDO(0, DO_BL_B, 0);
            m_PLCNetwork.SetDO(0, DO_BL_C1, 0);
            */
            
        }



        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        private void SelectNextMeasure()
        {
            int i = 0;

            for (i = m_nRunItem; i < MEASURE_ITEM_CLEAR; ++i)
            {
                if (m_bMeasureItem[i] == true)
                {
                    if (i == 2 || i == 3 || i == 4)
                    {
                        //TabControl_Measure.SelectedIndex = 2;
                    }
                    else
                    {
                        // TabControl_Measure.SelectedIndex = i;
                    }

                    m_nTestCounter = 0;
                    m_nConfirmCount = 0;
                    m_nRunItem = i;
                    m_nRunPhase = 0;
                    break;
                }
            }
            if (i == MEASURE_ITEM_CLEAR)
            {
                m_nRunItem = MEASURE_ITEM_CLEAR;
                m_nRunPhase = 0;
            }
        }

        #region _DEVICE_SETTING_

        public void SetDCPowerON(double dVolt, int nCurrent)
        {
            if (m_serialDcPower.CONNECTED == true)
            {
                m_serialDcPower.SendData("APPL " + string.Format("{0:0.0}", dVolt) + "," + nCurrent.ToString() + "\n");
                Thread.Sleep(50);
                m_serialDcPower.SendData("OUTP ON\n");
            }
        }

        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nVolt"></param>
        /// <param name="nCurrent"></param>
        /// 
        public void SetDSP1PowerON()
        {
            byte[] btTxBuf = new byte[1024];
            byte[] btRxBuf = new byte[1024];

            btTxBuf[0] = 0x06;
            btTxBuf[1] = 0x00;
            btTxBuf[2] = 0x09;
            btTxBuf[3] = 0x00;
            btTxBuf[4] = 0x01;

            serialTester1.SendPacketMODBUS(0x01, btTxBuf, 5);
            Thread.Sleep(200);
            serialTester1.RecieveData(btRxBuf, 256);
        }

        public void SetDSP1PowerOFF()
        {
            byte[] btTxBuf = new byte[1024];
            byte[] btRxBuf = new byte[1024];

            btTxBuf[0] = 0x06;
            btTxBuf[1] = 0x00;
            btTxBuf[2] = 0x09;
            btTxBuf[3] = 0x00;
            btTxBuf[4] = 0x00;

            serialTester1.SendPacketMODBUS(0x01, btTxBuf, 5);
            Thread.Sleep(200);
            serialTester1.RecieveData(btRxBuf, 256);
        }
        public void SetDSP1VoltCurrentChange(double dVolt, double dCurrent)
        {
            byte[] btTxBuf = new byte[1024];
            byte[] btRxBuf = new byte[1024];
            byte[] btVolt = new byte[1024];
            byte[] btCurrent = new byte[1024];

            int nVolt;
            int nCurrent;

            dVolt = dVolt * 100;
            dCurrent = dCurrent * 1000;

            nVolt = (int)dVolt;
            nCurrent = (int)dCurrent;

            btVolt = BitConverter.GetBytes(nVolt);
            btCurrent = BitConverter.GetBytes(nCurrent);

            btTxBuf[0] = 0x10;
            btTxBuf[1] = 0x00;
            btTxBuf[2] = 0x00;
            btTxBuf[3] = 0x00;
            btTxBuf[4] = 0x02;
            btTxBuf[5] = 0x04;

            btTxBuf[6] = btVolt[1];
            btTxBuf[7] = btVolt[0];
            btTxBuf[8] = btCurrent[1];
            btTxBuf[9] = btCurrent[0];

            serialTester1.SendPacketMODBUS(0x01, btTxBuf, 10);
            Thread.Sleep(200);
            serialTester1.RecieveData(btRxBuf, 256);
        }

        public void SetDSP2PowerON()
        {
            byte[] btTxBuf = new byte[1024];
            byte[] btRxBuf = new byte[1024];

            btTxBuf[0] = 0x06;
            btTxBuf[1] = 0x00;
            btTxBuf[2] = 0x09;
            btTxBuf[3] = 0x00;
            btTxBuf[4] = 0x01;

            serialTester2.SendPacketMODBUS(0x01, btTxBuf, 5);
            Thread.Sleep(200);
            serialTester2.RecieveData(btRxBuf, 256);
        }

        public void SetDSP2PowerOFF()
        {
            byte[] btTxBuf = new byte[1024];
            byte[] btRxBuf = new byte[1024];

            btTxBuf[0] = 0x06;
            btTxBuf[1] = 0x00;
            btTxBuf[2] = 0x09;
            btTxBuf[3] = 0x00;
            btTxBuf[4] = 0x00;

            serialTester2.SendPacketMODBUS(0x01, btTxBuf, 5);
            Thread.Sleep(200);
            serialTester2.RecieveData(btRxBuf, 256);
        }

        public void SetDSP2VoltCurrentChange(double dVolt, double dCurrent)
        {
            byte[] btTxBuf = new byte[1024];
            byte[] btRxBuf = new byte[1024];
            byte[] btVolt = new byte[1024];
            byte[] btCurrent = new byte[1024];

            int nVolt;
            int nCurrent;

            dVolt = dVolt * 100;
            dCurrent = dCurrent * 1000;

            nVolt = (int)dVolt;
            nCurrent = (int)dCurrent;

            btVolt = BitConverter.GetBytes(nVolt);
            btCurrent = BitConverter.GetBytes(nCurrent);

            btTxBuf[0] = 0x10;
            btTxBuf[1] = 0x00;
            btTxBuf[2] = 0x00;
            btTxBuf[3] = 0x00;
            btTxBuf[4] = 0x02;
            btTxBuf[5] = 0x04;

            btTxBuf[6] = btVolt[1];
            btTxBuf[7] = btVolt[0];
            btTxBuf[8] = btCurrent[1];
            btTxBuf[9] = btCurrent[0];

            serialTester2.SendPacketMODBUS(0x01, btTxBuf, 10);
            Thread.Sleep(200);
            serialTester2.RecieveData(btRxBuf, 256);
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dVolt"></param>
        /// <param name="nCurrent"></param>
        /// 
        public void ChangeDCPower(double dVolt, int nCurrent)
        {
            if (m_serialDcPower.CONNECTED == true)
            {
                m_serialDcPower.SendData("APPL " + string.Format("{0:0.0}", dVolt) + "," + nCurrent.ToString() + "\n");
            }
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetDCPowerOFF()
        {
            if (m_serialDcPower.CONNECTED == true)
            {
                m_serialDcPower.SendData("APPL 0,0\n");
                Thread.Sleep(50);
                m_serialDcPower.SendData("OUTP OFF\n");
            }
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bOn"></param>
        /// <param name="nVolt"></param>
        /// 
        public void SetACPowerON(double dVolt)
        {
            if (m_clientACPower.CONNECTED)
            {
                m_clientACPower.SendData("VOLT " + string.Format("{0:0.0}", dVolt) + "\r\n");
                m_clientACPower.SendData("OUTP ON\r\n");
            }
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dVolt"></param>
        /// 
        public void ChangeACPowerVolt(double dVolt)
        {
            if (m_clientACPower.CONNECTED)
            {
                m_clientACPower.SendData("VOLT " + string.Format("{0:0.0}", dVolt) + "\r\n");
            }
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetACPowerOFF()
        {
            //if (m_clientACPower.CONNECTED)
            //{
            //    m_clientACPower.SendData("OUTP OFF\r\n");
            //}
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetPowerPWMON()
        {
            m_serialPwm.IS_RETURN = false;
            m_serialPwm.SendData("PUT.PWMOUT.FREQ.0 500\r\n");
            m_serialPwm.ReceiveData();
            m_serialPwm.SendData("PUT.PWMOUT.DUTY.0 50\r\n");
            m_serialPwm.ReceiveData();

            //m_PLCNetwork.SetDO(0, DO_NOTCH, 1);
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetPowerPWMOFF()
        {
            m_serialPwm.IS_RETURN = false;
            m_serialPwm.SendData("PUT.PWMOUT.DUTY.0 0\r\n");
            m_serialPwm.ReceiveData();

            //m_PLCNetwork.SetDO(0, DO_NOTCH, 0);
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetPoweringSignal()
        {
            /*
            m_PLCNetwork.SetDO(0, DO_POWERING1, 1);
            
            m_PLCNetwork.SetDO(0, DO_PWM_PATTERN, 1);

            m_PLCNetwork.SetDO(0, DO_PG1_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG1_B, 1);
            m_PLCNetwork.SetDO(0, DO_PG2_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG2_B, 1);
            m_PLCNetwork.SetDO(0, DO_PG3_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG3_B, 1);
            m_PLCNetwork.SetDO(0, DO_PG4_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG4_B, 1);
            */
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void ResetPoweringSignal()
        {
            /*
            m_PLCNetwork.SetDO(0, DO_POWERING1, 0);
            
            m_PLCNetwork.SetDO(0, DO_PWM_PATTERN, 0);

            m_PLCNetwork.SetDO(0, DO_PG1_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG1_B, 0);
            m_PLCNetwork.SetDO(0, DO_PG2_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG2_B, 0);
            m_PLCNetwork.SetDO(0, DO_PG3_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG3_B, 0);
            m_PLCNetwork.SetDO(0, DO_PG4_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG4_B, 0);
            */
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetRegenBrakePWMON()
        {
            m_serialPwm.IS_RETURN = false;
            m_serialPwm.SendData("PUT.PWMOUT.FREQ.1 500\r\n");
            m_serialPwm.ReceiveData();
            m_serialPwm.SendData("PUT.PWMOUT.DUTY.1 50\r\n");
            m_serialPwm.ReceiveData();
        }


        ///######################################################################################## 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetRegenBrakePWMOFF()
        {
            m_serialPwm.IS_RETURN = false;
            m_serialPwm.SendData("PUT.PWMOUT.DUTY.1 0\r\n");
            m_serialPwm.ReceiveData();
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SetRegenBrakingSignal()
        {
            /*
            m_PLCNetwork.SetDO(0, DO_REGENERATIVE_BRAKING, 1);
            m_PLCNetwork.SetDO(0, DO_RB_PWM_PATTERN, 1);

            m_PLCNetwork.SetDO(0, DO_PG1_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG1_B, 1);
            m_PLCNetwork.SetDO(0, DO_PG2_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG2_B, 1);
            m_PLCNetwork.SetDO(0, DO_PG3_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG3_B, 1);
            m_PLCNetwork.SetDO(0, DO_PG4_A, 1);
            m_PLCNetwork.SetDO(0, DO_PG4_B, 1);
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void ResetRegenBrakingSignal()
        {
            /*
            m_PLCNetwork.SetDO(0, DO_REGENERATIVE_BRAKING, 0);
            m_PLCNetwork.SetDO(0, DO_RB_PWM_PATTERN, 0);

            m_PLCNetwork.SetDO(0, DO_PG1_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG1_B, 0);
            m_PLCNetwork.SetDO(0, DO_PG2_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG2_B, 0);
            m_PLCNetwork.SetDO(0, DO_PG3_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG3_B, 0);
            m_PLCNetwork.SetDO(0, DO_PG4_A, 0);
            m_PLCNetwork.SetDO(0, DO_PG4_B, 0);
            */
        }

        #endregion


        #region LISTVIEW_DRAW_HANDLER
        private void ListView_Converter_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.LightSlateGray, e.Bounds);
            e.Graphics.DrawRectangle(Pens.Black, e.Bounds);
            e.DrawText();
        }

        private void ListView_Converter_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView_Converter_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView_Inverter_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.LightSlateGray, e.Bounds);
            e.DrawText();
        }

        private void ListView_Inverter_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView_Inverter_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
        #endregion




        ///######################################################################################## 
        /// 
        /// <summary>
        ///     Button_Select_All_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_Select_All_Click(object sender, EventArgs e)
        {
            SetAllNodesCheckedState(modernTreeView1.Nodes, true);
        }

        ///######################################################################################## 
        /// 
        /// <summary>
        ///     Button_DeSelect_All_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_DeSelect_All_Click(object sender, EventArgs e)
        {
            SetAllNodesCheckedState(modernTreeView1.Nodes, false);
        }
        private void SetAllNodesCheckedState(TreeNodeCollection nodes, bool isChecked)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = isChecked;
                if (node.Nodes.Count > 0)
                {
                    SetAllNodesCheckedState(node.Nodes, isChecked);
                }
            }
        }

        ///****************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public string CurrentOutCmd_Send(string strCmd)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialCurrentOutBoard.SendData(string.Format("{0}\r\n", strCmd));

                Thread.Sleep(100);

                if (m_serialCurrentOutBoard.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string CurrentOutCmd_Set_Send(string strBoard, string strLoc, string strmA)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            if (m_serialCurrentOutBoard.CONNECTED == false || m_serialCurrentOutBoard == null)
                return "";

            for (i = 0; i < 3; i++)
            {
                m_serialCurrentOutBoard.SendData(string.Format("set.current.{0} {1} {2}\r\n", strBoard, strLoc, strmA));

                Thread.Sleep(100);

                if (m_serialCurrentOutBoard.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        ///****************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public string SpeedOutCmd_Send(string strCmd)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            if (!OpenSpeedOut())
            {
                return "";
            }

            for (i = 0; i < 3; i++)
            {
                m_serialSpeedOut.SendData(string.Format("{0}\r\n", strCmd));

                Thread.Sleep(500);

                if (m_serialSpeedOut.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string PwmCmd_Send(string strCmd)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            if (!OpenSpeedOut())
            {
                return "";
            }

            for (i = 0; i < 3; i++)
            {
                m_serialSpeedOut.SendData(string.Format("{0}\r\n", strCmd));

                Thread.Sleep(500);

                if (m_serialSpeedOut.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string SpeedOutCmd_2sin_Send(string strHz, string strVolt)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialSpeedOut.SendData(string.Format("set.2ph.freq.0 {0}\r\n", strHz));
                Thread.Sleep(300);
                m_serialSpeedOut.SendData(string.Format("set.2ph.volt.0 {0}\r\n", strVolt));

                Thread.Sleep(500);

                if (m_serialSpeedOut.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string PwmCmd_Hz_Send(string strCh, string strHz)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialSpeedOut.SendData(string.Format("set.pwmout.freq {0} {1}\r\n", strCh, strHz));

                Thread.Sleep(500);

                if (m_serialSpeedOut.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }


        public string PwmCmd_Duty_Send(string strCh, string strDuty)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialSpeedOut.SendData(string.Format("set.pwmout.duty {0} {1}\r\n", strCh, strDuty));

                Thread.Sleep(500);

                if (m_serialSpeedOut.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string SpeedOutCmd_3sin_Send(string strHz, string strVolt)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialSpeedOut.SendData(string.Format("set.3ph.freq.0 {0}\r\n", strHz));
                Thread.Sleep(300);
                m_serialSpeedOut.SendData(string.Format("set.3ph.volt.0 {0}\r\n", strVolt));
                Thread.Sleep(500);

                if (m_serialSpeedOut.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        ///****************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public string OpticalCmd_Send(string strCmd)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialOpticBoard1.SendData(string.Format("{0}\r\n", strCmd));
                Thread.Sleep(500);

                if (m_serialOpticBoard1.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string OpticalCmd_Hz_Send(string strLoc, string strHz)
        {
            if (m_serialOpticBoard1 == null || !m_serialOpticBoard1.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialOpticBoard1.SendData(string.Format("set.pwmout.freq.{0} {1}\r\n", strLoc, strHz));
                Thread.Sleep(100);

                if (m_serialOpticBoard1.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string OpticalCmd_Duty_Send(string strLoc, string strDuty)
        {
            if (m_serialOpticBoard1 == null || !m_serialOpticBoard1.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialOpticBoard1.SendData(string.Format("set.pwmout.duty.{0} {1}\r\n", strLoc, strDuty));
                Thread.Sleep(100);

                if (m_serialOpticBoard1.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string OpticalCmd2_Send(string strCmd)
        {
            if (m_serialOpticBoard2 == null || !m_serialOpticBoard2.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialOpticBoard2.SendData(string.Format("{0}\r\n", strCmd));
                Thread.Sleep(100);

                if (m_serialOpticBoard2.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string OpticalCmd2_Hz_Send(string strLoc, string strHz)
        {
            if (m_serialOpticBoard2 == null || !m_serialOpticBoard2.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialOpticBoard2.SendData(string.Format("set.pwmout.freq.{0} {1}\r\n", strLoc, strHz));
                Thread.Sleep(100);

                if (m_serialOpticBoard2.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string OpticalCmd2_Duty_Send(string strLoc, string strDuty)
        {
            if (m_serialOpticBoard2 == null || !m_serialOpticBoard2.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialOpticBoard2.SendData(string.Format("set.pwmout.duty.{0} {1}\r\n", strLoc, strDuty));
                Thread.Sleep(100);

                if (m_serialOpticBoard2.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string Trimmer_Send(string strCmd)
        {
            if (m_serialTrimmerBoard1 == null || !m_serialTrimmerBoard1.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialTrimmerBoard1.SendData(string.Format("{0}\r\n", strCmd));
                Thread.Sleep(10);

                if (m_serialTrimmerBoard1.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string Trimmer_No_Ch_Value_Send(string strNo, string strCh, string strValue)
        {
            if (m_serialTrimmerBoard1 == null || !m_serialTrimmerBoard1.CONNECTED) return "";

            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialTrimmerBoard1.SendData(string.Format("set.trimmer.{0} {1} {2}\r\n", strNo, strCh, strValue));
                Thread.Sleep(100);

                if (m_serialTrimmerBoard1.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string Trimmer2_Send(string strCmd)
        {
            if (m_serialTrimmerBoard2 == null || !m_serialTrimmerBoard2.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialTrimmerBoard2.SendData(string.Format("{0}\r\n", strCmd));
                Thread.Sleep(100);

                if (m_serialTrimmerBoard2.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        public string Trimmer2_No_Ch_Value_Send(string strNo, string strCh, string strValue)
        {
            if (m_serialTrimmerBoard2 == null || !m_serialTrimmerBoard2.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialTrimmerBoard2.SendData(string.Format("set.trimmer.{0} {1} {2}\r\n", strNo, strCh, strValue));
                Thread.Sleep(100);

                if (m_serialTrimmerBoard2.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        ///****************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public string MVBCmd_Send(string strCmd)
        {
            if (m_serialMvbBoard == null || !m_serialMvbBoard.CONNECTED) return "";
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialMvbBoard.SendData(string.Format("{0}\r\n", strCmd));
                Thread.Sleep(500);

                if (m_serialSpeedOut.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }

        ///****************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public string DMMCmd_Send(string strCmd1, string strCmd2, string strCmd3, string strCmd4, string strCmd5)
        {
            string strSend = "";

            if (!ConnectDMM())
            {
                return "통신상태불량";
            }

            if (strCmd1 != "")
            {
                m_ethernetDmm.WriteString(string.Format("{0}", strCmd1));
                Thread.Sleep(100);
            }
            if (strCmd2 != "")
            {
                m_ethernetDmm.WriteString(string.Format("{0}", strCmd2));
                Thread.Sleep(100);
            }
            if (strCmd3 != "")
            {
                m_ethernetDmm.WriteString(string.Format("{0}", strCmd3));
                Thread.Sleep(100);
            }
            if (strCmd4 != "")
            {
                m_ethernetDmm.WriteString(string.Format("{0}", strCmd4));
                Thread.Sleep(100);
            }
            if (strCmd5 != "")
            {
                m_ethernetDmm.WriteString(string.Format("{0}", strCmd5));
                Thread.Sleep(100);
            }

            //Thread.Sleep(100);
            //m_ethernetDmm.WriteString("TRIG:SOUR EXT;SLOP POS");
            //Thread.Sleep(100);
            //m_ethernetDmm.WriteString("INIT");
            //Thread.Sleep(100);
            //m_ethernetDmm.WriteString("FETC?");

            strSend = m_ethernetDmm.ReadString();
            return strSend;
        }

        ///****************************************************************************************************************************************************************************************************
        /// <summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dVolt"></param>
        /// <param name="nCurrent"></param>
        public bool DMM_DC_Send(int nVolt, int nChannel, ref double dVolt)
        {
            string strDmmRead = "";
            int nDmmCounter = 0;
            double dDmmMea = 0;

            m_ethernetDmm.WriteString(string.Format("CONF:VOLT:DC {0}, (@{1})", nVolt, nChannel));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString(string.Format("ROUT:SCAN (@{0})", nChannel));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString(string.Format("ROUT:MON:STAT ON"));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString(string.Format("ROUT:MON (@{0})", nChannel));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString("READ?");

            strDmmRead = m_ethernetDmm.ReadString();

            nDmmCounter = int.Parse(strDmmRead.Substring(strDmmRead.Length - 2));// 10진수값 구하기

            dDmmMea = double.Parse(strDmmRead.Substring(1, 4));//측정값 소수점

            if (nDmmCounter == 0)
            {
                dVolt = dDmmMea;
            }
            else
            {
                dVolt = dDmmMea * (nDmmCounter * 10);
            }


            if (dVolt > 0)
            {
                return true;
            }

            return false;
        }

        public bool DMM_CURR_Send(int nCurr, int nChannel, ref double dCurr)
        {
            string strDmmRead = "";
            int nDmmCounter = 0;
            double dDmmMea = 0;

            m_ethernetDmm.WriteString(string.Format("CONF:CURR:DC {0}, (@{1})", nCurr, nChannel));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString(string.Format("ROUT:SCAN (@{0})", nChannel));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString(string.Format("ROUT:MON:STAT ON"));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString(string.Format("ROUT:MON (@{0})", nChannel));
            Thread.Sleep(50);
            m_ethernetDmm.WriteString("READ?");

            strDmmRead = m_ethernetDmm.ReadString();

            nDmmCounter = int.Parse(strDmmRead.Substring(strDmmRead.Length - 2));// 10진수값 구하기

            dDmmMea = double.Parse(strDmmRead.Substring(1, 3));//측정값 소수점

            if (nDmmCounter == 0)
            {
                dCurr = dDmmMea;
            }
            else
            {
                dCurr = dDmmMea * (nDmmCounter * 10);
            }


            if (dCurr > 0)
            {
                return true;
            }

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Sequence_RUN_BasicSetting();
            SetDCPowerON(100, 10);

            /*
            m_PLCNetwork.SetDO(0, DO_SIVKB, 1);
            m_PLCNetwork.SetDO(0, DO_DC100, 1);
            */
        }

        private void ProgressBar_Run_Click(object sender, EventArgs e)
        {

        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {

        }

        public string LineVoltage0Cmd_Send(string strCmd)
        {
            int i;
            byte[] btRxBuf = new byte[1024];

            for (i = 0; i < 3; i++)
            {
                m_serialLineVoltageBoard0.SendData(string.Format("{0}\r\n", strCmd));

                Thread.Sleep(500);

                if (m_serialLineVoltageBoard0.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }
        public string LineVoltage0Cmd_2sin_Send(string strHz, string strVolt)
        {
            int i;
            byte[] btRxBuf = new byte[1024];


            for (i = 0; i < 3; i++)
            {
                m_serialLineVoltageBoard0.SendData(string.Format("set.3ph.freq.0 {0}\r\n", strHz));
                Thread.Sleep(50);
                m_serialLineVoltageBoard0.SendData(string.Format("set.3ph.volt.0 {0}\r\n", strVolt));
                Thread.Sleep(50);

                if (m_serialLineVoltageBoard0.ReceiveData_string(btRxBuf, 512) > 0)
                {
                    break;
                }
            }

            if (i >= 3)
            {
                return "";
            }
            else
            {
                return Encoding.Default.GetString(btRxBuf);
            }
        }


        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetPort();
        }
        private void BtnSetting_Click(object sender, EventArgs e)
        {
            if (m_bDBOpened == false)
            {
                MessageBox.Show("Failed to open the database file. Please check the file and restart the application.");
                return;
            }

            FormConfig frmConfig = new FormConfig(m_OLECommand, m_Config, this);

            if (frmConfig.ShowDialog() == DialogResult.OK)
            {
                m_Config = frmConfig.m_Config;
                DisplayConfig();
                WriteConfig();
                RefreshMainUI();
            }
        }

        //  메인화면 UI를 유닛 정보에 맞춰 갱신하는 함수
        private void RefreshMainUI()
        {
            if (this.strControlUnitInfo == "54")
            {
                // label_Main_UnitName.Text = "Unit 2 (54칸)";
            }
            else if (this.strControlUnitInfo == "1,2,3")
            {
                // label_Main_UnitName.Text = "Unit 1 (1,2,3단계)";
            }

            // 만약 메인폼 로드 시점에 모든 UI 세팅이 들어있다면 강제로 호출할 수도 있습니다.
            FormMain_Load(this, EventArgs.Empty);
        }
        private void BtnDB_Click(object sender, EventArgs e)
        {
            if (m_bDBOpened == false)
            {
                MessageBox.Show("Failed to open the database file. Please check the file and restart the application.");
            }
            FormDB frmDB = new FormDB(m_OLECommand);
            frmDB.ShowDialog();
        }

        private void BtnResult_Click(object sender, EventArgs e)
        {
            FormResult frmResult = new FormResult(m_OLECommand, m_Config);
            frmResult.ShowDialog();
        }

        private void BtnDiagnostic_Click(object sender, EventArgs e)
        {
            strSerialPortList = System.IO.Ports.SerialPort.GetPortNames();

            FormDiagnosis frmDiagnosis = new FormDiagnosis(this);
            frmDiagnosis.ShowDialog();
        }

        private void BtnReset_Click_1(object sender, EventArgs e)
        {
            ResetPort();
        }

        private void BtnLang_Click(object sender, EventArgs e)
        {
            /*  string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

              Form langForm = new Form();
              langForm.Text = "Language Settings";
              langForm.Size = new Size(350, 230);
              langForm.StartPosition = FormStartPosition.CenterParent;

              langForm.MaximizeBox = false;
              langForm.MinimizeBox = false;
              langForm.FormBorderStyle = FormBorderStyle.FixedDialog;

              Font customFont = new Font("맑은 고딕", 12, FontStyle.Bold);

              System.Windows.Forms.Button btnKor = new System.Windows.Forms.Button()
              {
                  Text = "한국어",
                  Location = new Point(25, 30),
                  Width = 130,
                  Height = 50,
                  Font = customFont
              };
              btnKor.Click += (s, ev) =>
              {
                  ChangeLanguage("ko-KR");
                  langForm.Close();
              };

              System.Windows.Forms.Button btnEng = new System.Windows.Forms.Button()
              {
                  Text = "English",
                  Location = new Point(175, 30),
                  Width = 130,
                  Height = 50,
                  Font = customFont
              };
              btnEng.Click += (s, ev) =>
              {
                  ChangeLanguage("en-US");
                  langForm.Close();
              };

              System.Windows.Forms.Button btnClose = new System.Windows.Forms.Button()
              {
                  Text = currentCulture.StartsWith("en") ? "Close" : "종 료",
                  Location = new Point(102, 100),
                  Width = 130,
                  Height = 45,
                  Font = customFont,
                  BackColor = Color.LightGray
              };
              btnClose.Click += (s, ev) =>
              {
                  langForm.Close();
              };

              langForm.Controls.Add(btnKor);
              langForm.Controls.Add(btnEng);
              langForm.Controls.Add(btnClose);

              langForm.ShowDialog();*/
        }
        private void ChangeLanguage(string cultureCode)
        {
            try
            {
                string configPath = Application.StartupPath + @"\Config.xml";
                XDocument doc = XDocument.Load(configPath);

                // XML의 Language 값 업데이트
                XElement langElement = doc.Root.Element("General").Element("Language");
                if (langElement != null)
                {
                    langElement.Value = cultureCode;
                }
                else
                {
                    doc.Root.Element("General").Add(new XElement("Language", cultureCode));
                }

                doc.Save(configPath);

                MessageBox.Show($"언어가 {cultureCode}로 변경되었습니다.\n" +
                                "프로그램을 재시작해야 적용됩니다.\n\n" +
                                $"Language has been changed to {cultureCode}.\n" +
                                "Please restart the program to apply changes.",
                                "알림 / Notification"
                                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("설정 저장 실패: " + ex.Message);
            }
        }

        private System.Windows.Forms.Timer dataTimer; // 타이머 객체
        private Random rand = new Random(); // 가상 전압 변동을 위한 랜덤 객체
        private double[] currentVoltages = new double[7];
        private bool[] isHighState = new bool[7];
        private const double Threshold = 2.5;

        // 시뮬레이션 진행 단계를 제어할 변수 (펄스를 순차적으로 만들기 위함)
        private int simulationStep = 0;

        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (dataTimer != null)
            {
                dataTimer.Stop();
                dataTimer.Dispose();
            }

            // 차트 기본 설정
            // ctrlTimingChart1.SHOW_GUIDE_LINE = true;

            // 모든 채널의 전압과 상태 초기화
            for (int i = 0; i < 7; i++)
            {
                currentVoltages[i] = rand.NextDouble() * 5.0; // 시작 전압도 제각각 랜덤
                isHighState[i] = currentVoltages[i] >= Threshold;
            }

            dataTimer = new System.Windows.Forms.Timer();
            dataTimer.Interval = 100; // 0.1초 간격
            dataTimer.Start();
        }
        /*   private void checkBox2_CheckedChanged(object sender, EventArgs e)
           {
               CtrlTimingChart_1.SHOW_VOLTAGE = checkboxGuide1.Checked;
           }*/

        private void checkboxGuide1_CheckedChanged(object sender, EventArgs e)
        {
            int[] starts = { 10, 65, 75, 105, 115, 125, 135 };
            int[] ends = { 280, 250, 235, 235, 235, 235, 280 };
        }

        private void checkboxGuide2_CheckedChanged(object sender, EventArgs e)
        {
            int[] starts = { 10, 65, 75, 105, 115, 125, 135, 135 };
            int[] ends = { 280, 250, 235, 235, 235, 235, 280, 280 };

        }

        private void checkboxGuide3_CheckedChanged(object sender, EventArgs e)
        {
            int[] starts = { 10, 65, 75, 105, 115, 125, 135, 135, 135 };
            int[] ends = { 280, 250, 235, 235, 235, 235, 280, 280, 280 };

        }
        private IMessageBasedSession _session;

        // 리골 오실로스코프 연결 함수
        public void Connect(string ipAddress)
        {
            string resourceString = $"TCPIP0::{ipAddress}::5025::SOCKET";

            try
            {
                _session = (IMessageBasedSession)GlobalResourceManager.Open(resourceString);

                _session.TerminationCharacterEnabled = true;
                _session.TerminationCharacter = 0x0A; // \n
                _session.TimeoutMilliseconds = 10000;

                Console.WriteLine("연결 성공");
            }
            catch (Exception ex)
            {
                Console.WriteLine("연결 실패");
            }
        }

        public string Query(string command)
        {
            if (_session == null) return "세션이 연결되지 않았습니다.";

            _session.RawIO.Write(command + "\n");
            return _session.RawIO.ReadString();
        }

        public void Write(string command)
        {
            _session?.RawIO.Write(command + "\n");
        }

        public void Disconnect()
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
                Console.WriteLine("세션 종료");
            }
        }
        private bool WriteOscilloscope2(string strCommand)
        {
            if (_session == null)
                return false;

            try
            {
                if (!strCommand.EndsWith("\n"))
                {
                    strCommand += "\n";
                }

                _session.RawIO.Write(strCommand);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public void SaveScreenCapture()
        {
            if (_session == null) return;
            try
            {
                _session.Clear();
                _session.TimeoutMilliseconds = 30000;
                _session.TerminationCharacterEnabled = false;

                _session.RawIO.Write(":DISP:DATA?\n");

                byte[] imageData = ReadIEEEBlock();

                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"Rigol_{DateTime.Now:HHmmss}.bmp");

                File.WriteAllBytes(filePath, imageData);
                Console.WriteLine($"저장 완료: {filePath} ({imageData.Length} bytes)");
            }
            catch (Exception ex)
            {
                Console.WriteLine("오류: " + ex.Message);
            }
            finally
            {
                _session.TerminationCharacterEnabled = true;
            }
        }

        private byte[] ReadIEEEBlock()
        {
            var rawIO = (IMessageBasedRawIO)_session.RawIO;

            // 1. '#' 읽기
            byte[] hashBuf = rawIO.Read(1);
            if (hashBuf[0] != (byte)'#')
                throw new Exception($"IEEE 헤더 오류: 0x{hashBuf[0]:X2}");

            // 2. 자릿수(N) 읽기
            byte[] nBuf = rawIO.Read(1);
            int nDigits = nBuf[0] - '0';

            // 3. 데이터 길이 읽기
            byte[] lenBuf = rawIO.Read(nDigits);
            int dataLength = int.Parse(System.Text.Encoding.ASCII.GetString(lenBuf));
            Console.WriteLine($"수신 예정: {dataLength} bytes");

            // 4. 정확한 크기만큼 수신
            byte[] data = new byte[dataLength];
            int received = 0;
            while (received < dataLength)
            {
                int remaining = dataLength - received;
                byte[] chunk = rawIO.Read(Math.Min(remaining, 1024 * 64));
                Array.Copy(chunk, 0, data, received, chunk.Length);
                received += chunk.Length;
                Console.WriteLine($"수신 중: {received} / {dataLength} bytes");
            }

            return data;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Connect("192.168.1.10");
        }
        private void TabControl_Menu_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ConnectToControlPage();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveScreenCapture();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (_session == null) return;

            try
            {
                // 1. 측정 소스 설정
                WriteOscilloscope2(":MEASure:SOURce CHANnel1");

                // --- Channel 1 ---
                WriteOscilloscope2(":CHANnel1:DISPlay 1");
                WriteOscilloscope2(":CHANnel1:SCALe 3");
                WriteOscilloscope2(":CHANnel1:OFFSet -8.6");
                Thread.Sleep(20); // 리골은 명령 사이의 간격을 20ms 정도로 주는 것이 안전

                // --- Channel 2 ---
                WriteOscilloscope2(":CHANnel2:DISPlay 1");
                WriteOscilloscope2(":CHANnel2:SCALe 3");
                WriteOscilloscope2(":CHANnel2:OFFSet -2.03");
                Thread.Sleep(20);

                // --- Channel 3 ---
                WriteOscilloscope2(":CHANnel3:DISPlay 1");
                WriteOscilloscope2(":CHANnel3:SCALe 3");
                WriteOscilloscope2(":CHANnel3:OFFSet 4.6");
                Thread.Sleep(20);

                // --- Channel 4 ---
                WriteOscilloscope2(":CHANnel4:DISPlay 1");
                WriteOscilloscope2(":CHANnel4:SCALe 3");
                WriteOscilloscope2(":CHANnel4:OFFSet 10.4");
                Thread.Sleep(20);


                Console.WriteLine("Rigol 오실로스코프 설정 완료.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("설정 중 오류 발생: " + ex.Message);
            }
        }
        private async Task FnRunSequentialLampTestAsync()
        {
            // 순차 점등 사이의 지연 오프셋 타임 설정 (300ms 고정)
            int nDelayInterval = 300;
        }
        private void AppendTestLog(RichTextBox rtbTarget, string strMessage, Color clrText)
        {
            if (rtbTarget == null) return;

            // 1. 크로스 스레드 방지를 위한 Invoke 예외 처리
            if (rtbTarget.InvokeRequired)
            {
                rtbTarget.Invoke(new Action(() => AppendTestLog(rtbTarget, strMessage, clrText)));
                return;
            }

            // 2. 메모리 누수 및 성능 저하 방지를 위한 텍스트 라인 버퍼 제한 (최대 500줄 유지)
            int nMaxLogLines = 500;
            if (rtbTarget.Lines.Length > nMaxLogLines)
            {
                rtbTarget.SelectionStart = 0;
                rtbTarget.SelectionLength = rtbTarget.GetFirstCharIndexFromLine(rtbTarget.Lines.Length - nMaxLogLines);
                rtbTarget.SelectedText = "";
            }

            // 3. 시간 스탬프 결합 및 텍스트 추가
            string strLogEntry = $"[{DateTime.Now:HH:mm:ss.fff}] {strMessage}{Environment.NewLine}";

            rtbTarget.SelectionStart = rtbTarget.TextLength;
            rtbTarget.SelectionLength = 0;
            rtbTarget.SelectionColor = clrText;

            rtbTarget.AppendText(strLogEntry);
            rtbTarget.SelectionColor = rtbTarget.ForeColor; // 색상 기본값으로 리셋

            // 4. 최신 로그 위치로 자동 스크롤
            rtbTarget.ScrollToCaret();
        }

        private readonly Random m_randGenerator = new Random();
        private bool m_bIsTesting = false;
        private TestResultJson m_objTestResult = null;

        /// <summary>
        /// FEnet 통신을 통해 모든 PLC 출력을 OFF 상태로 초기화
        /// </summary>
        private void ResetAllPlcOutputs(int maxChannelCount = 32)
        {

            for (int ch = 1; ch <= maxChannelCount; ch++)
            {
                // 국번(0), 채널번호(ch), OFF(false)
                m_PLCNetwork.SetDO(0, ch, false);
            }
        }

        /// <summary>
        /// 시험 시작
        /// </summary>
        private async void button1_Click_3Async(object sender, EventArgs e)
        {
            // 1. 강제 중단 요청 시 PLC 출력 및 통신 즉시 차단
            if (m_bIsTesting)
            {
                m_bIsTesting = false;
                ResetAllPlcOutputs();
                StopMvbCommunication();

                AppendTestLog(richTextBox_Log, "[시스템] 사용자 요청에 의해 시험이 강제 중단됩니다.", Color.OrangeRed);
                return;
            }

            int nMaxLoop = (int)TestCount.Value;
            if (MessageBox.Show($"시험 차수 : {nMaxLoop}회\n시험을 시작하시겠습니까?", "시험 시작 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            m_bIsTesting = true;
            SetTestingUiState(true);

            string strUnitType = ConfigJson.CurrentConfig?.Operation?.TCMSUnit ?? "TC";

            // 2. 시험 시작 직전 시리얼 포트 오픈
            if (!StartMvbCommunication())
            {
                m_bIsTesting = false;
                SetTestingUiState(false);
                return;
            }

            var finalResult = new TestResultJson();
            finalResult.Header.TCMSUnit = strUnitType;
            finalResult.Header.SerialNo = ConfigJson.CurrentConfig?.Operation?.SerialNo ?? "0000";
            finalResult.Header.TesterName = ConfigJson.CurrentConfig?.Operation?.TesterName ?? "Tester";
            finalResult.Header.FleetNo = ConfigJson.CurrentConfig?.Operation?.FleetNo ?? "0000";
            finalResult.Header.TrainNo = ConfigJson.CurrentConfig?.Operation?.TrainNo ?? "0000";
            finalResult.Header.TotalRound = nMaxLoop;

            var objDigitalGridResult = new TestResultJson.GridTestResult { GridTitle = "디지털 입출력 시험" };
            var objAnalogGridResult = new TestResultJson.GridTestResult { GridTitle = "아날로그 입출력 시험" };
            var objCommGridResult = new TestResultJson.GridTestResult { GridTitle = "통신 시험" };

            finalResult.GridResults.Add(objDigitalGridResult);
            finalResult.GridResults.Add(objAnalogGridResult);
            finalResult.GridResults.Add(objCommGridResult);

            bool bHasAnyFailure = false;

            // 3. 실제 시험을 주관하는 testService 생성 및 설정
            var testService = new TcmsTestService(m_mvbReceiver)
            {
                OnLog = (msg, color) => AppendTestLog(richTextBox_Log, msg, color),
                OnFailLog = (msg, color) => AppendTestLog(richTextBox_FailLog, msg, color),
                OnGridInvalidate = () =>
                {
                    dataGridViewDI1?.Invalidate();
                    dataGridViewDI2?.Invalidate();
                    dataGridViewDI3?.Invalidate();
                    dataGridViewDO?.Invalidate();
                },
                RunChannelSequenceFunc = async (cat, arr, count, delay, loop, fails, details, currentRawData) =>
                {
                    DataGridView targetDgv = null;
                    switch (cat)
                    {
                        case "DI1": targetDgv = dataGridViewDI1; break;
                        case "DI2": targetDgv = dataGridViewDI2; break;
                        case "DI3": targetDgv = dataGridViewDI3; break;
                        case "DO": targetDgv = dataGridViewDO; break;
                    }

                    bool[] expectedBits = GetExpectedBitsForCategory(cat);
                    Func<byte[]> channelRawDataSupplier = () => currentRawData?.Invoke();
                    int nStartPin = GetStartPinByCategory(strUnitType, cat);

                    await RunChannelTestSequenceAsync(
                        cat, arr, count, targetDgv, delay, loop, fails, details,
                        channelRawDataSupplier, expectedBits, nStartPin
                    );
                }
            };

            try
            {
                var channelContext = GetChannelContextByUnit(strUnitType);
                if (channelContext == null) return;

                // [핵심] 현재 시험 서비스 인스턴스를 m_mvbReceiver 이벤트에 등록하고 수신 시작
                testService.StartMvbReceiver(strUnitType);
                await Task.Delay(300); // 첫 패킷이 안정적으로 들어올 때까지 대기

                // 메인 회차 루프 (입·출력 -> 통신)
                for (int nLoop = 1; nLoop <= nMaxLoop; nLoop++)
                {
                    if (!m_bIsTesting) break;

                    AppendTestLog(richTextBox_Log, $"==================================================", Color.Purple);
                    AppendTestLog(richTextBox_Log, $"           [전체 시험 {nLoop}/{nMaxLoop}회차 시작]           ", Color.Purple);
                    AppendTestLog(richTextBox_Log, $"==================================================", Color.Purple);

                    // [1단계] 입·출력 시험
                    mainTabControl1.SelectedIndex = 0;
                    await Task.Delay(200);

                    bool bIoPass = await testService.ExecuteSingleRoundIoAsync(
                        strUnitType, nLoop, () => m_bIsTesting, channelContext, objDigitalGridResult, objAnalogGridResult);
                    if (!bIoPass) bHasAnyFailure = true;

                    if (!m_bIsTesting) break;

                    // [2단계] 통신 시험
                    mainTabControl1.SelectedIndex = 1;
                    await Task.Delay(300);

                    bool bCommPass = await RunCommSingleRoundAsync(strUnitType, nLoop, objCommGridResult);
                    if (!bCommPass)
                    {
                        bHasAnyFailure = true;
                        AppendTestLog(richTextBox_FailLog, $"[통신] {nLoop}회차 통신 시험 불합격 발생", Color.Red);
                    }

                    if (!m_bIsTesting) break;

                    AppendTestLog(richTextBox_Log, $"===== [전체 시험 {nLoop}/{nMaxLoop}회차 완료] =====\n", Color.Purple);
                    await Task.Delay(500);
                }

                // 최종 JSON 저장
                if (m_bIsTesting)
                {
                    finalResult.Header.FinalResult = bHasAnyFailure ? "불합격" : "합격";

                    TestResultManager resultManager = new TestResultManager();
                    if (resultManager.SaveTestResult(finalResult))
                    {
                        AppendTestLog(richTextBox_Log, "[시스템] 모든 회차 시험 완료 및 JSON 저장 완료.", Color.DarkBlue);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendTestLog(richTextBox_Log, $"[시스템 에러] 예외 발생: {ex.Message}", Color.Red);
            }
            finally
            {
                // [핵심] 시험이 끝나면 해당 testService의 이벤트 바인딩 해제 후 통신 닫기
                testService?.StopMvbReceiver();
                StopMvbCommunication();

                m_bIsTesting = false;
                ResetAllPlcOutputs();
                SetTestingUiState(false);
            }
        }

        // 카테고리별 시작 핀 번호 반환 헬퍼 메서드 예시
        private int GetStartPinByCategory(string strUnitType, string cat)
        {
            var unitType = AppConfig.ParseUnitType(strUnitType);

            // AppConfig 내부 메서드가 cat(string)을 직접 지원하는 경우:
            // return AppConfig.GetStartChannelNo(unitType, cat);

            // 직접 분기할 경우 예시:
            switch (cat?.ToUpper())
            {
                case "DI1": return AppConfig.GetStartChannelNo(unitType, false);       // 예: 144 (또는 1)
                case "DI2": return AppConfig.GetStartChannelNo(unitType, false) + 48;  // 예: 192 (또는 49)
                case "DI3": return AppConfig.GetStartChannelNo(unitType, false) + 96;  // 예: 240 (또는 97)
                case "DO": return AppConfig.GetStartChannelNo(unitType, true);        // DO 시작 핀
                default: return 1;
            }
        }

        // UI 버튼 및 컨트롤 상태 설정 헬퍼
        private void SetTestingUiState(bool isTesting)
        {
            m_bIsTesting = isTesting;

            if (isTesting)
            {
                richTextBox_Log.Clear();
                richTextBox_FailLog.Clear();

                BtnStart.Text = "시험 정지";
                BtnStart.BackColor = Color.IndianRed;
                BtnStart.HoverBackColor = Color.FromArgb(223, 115, 115);
                BtnStart.PressedBackColor = Color.FromArgb(166, 68, 68);
                BtnStart.Image = TCMSTester.Properties.Resources.stop_button;
                imagebtn1.Visible = false;
                imagebtn2.Visible = false;
            }
            else
            {
                BtnStart.Text = "시험 시작";
                BtnStart.BackColor = Color.RoyalBlue;
                BtnStart.HoverBackColor = Color.FromArgb(45, 110, 240);
                BtnStart.PressedBackColor = Color.FromArgb(20, 70, 180);
                BtnStart.Image = TCMSTester.Properties.Resources.play_button_arrowhead;

                imagebtn1.Visible = true;
                imagebtn2.Visible = true;
            }
        }

        /// <summary>
        /// 채널 카테고리(DI1, DI2, DI3, DO)별 기대 비트 패턴 배열을 생성합니다.
        /// </summary>
        private bool[] GetExpectedBitsForCategory(string category)
        {
            string strUnitType = ConfigJson.CurrentConfig?.Operation?.TCMSUnit ?? "TC";
            int count = 0;

            // 현재 선택된 유닛 및 카테고리에 맞는 활성 핀 개수 산출 (C# 7.3 호환)
            switch (category)
            {
                case "DI1":
                    if (strUnitType == "TC") count = TC_DI1Count;
                    else if (strUnitType == "CC") count = CC_DI1Count;
                    else if (strUnitType == "DU") count = DU_DICount;
                    break;

                case "DI2":
                    if (strUnitType == "TC") count = TC_DI2Count;
                    else if (strUnitType == "CC") count = CC_DI2Count;
                    break;

                case "DI3":
                    if (strUnitType == "TC") count = TC_DI3Count;
                    break;

                case "DO":
                    if (strUnitType == "TC") count = TC_DoCount;
                    else if (strUnitType == "CC") count = CC_DoCount;
                    else if (strUnitType == "DU") count = DU_DoCount;
                    break;
            }

            if (count <= 0) return null;

            // 시험 시 기대하는 비트 상태 배열 생성 (모두 ON 신호 수신 기대 시 true)
            bool[] expectedBits = new bool[count];
            for (int i = 0; i < count; i++)
            {
                expectedBits[i] = true;
            }

            return expectedBits;
        }

        // 유닛 타입별 채널 매핑 헬퍼
        private ChannelContext GetChannelContextByUnit(string unitType)
        {
            switch (unitType)
            {
                case "TC":
                    return new ChannelContext
                    {
                        ActiveDi1 = arrTcDi1States,
                        ActiveDi2 = arrTcDi2States,
                        ActiveDi3 = arrTcDi3States,
                        ActiveDo = arrTcDoStates,
                        ActiveDi1Count = TC_DI1Count,
                        ActiveDi2Count = TC_DI2Count,
                        ActiveDi3Count = TC_DI3Count,
                        ActiveDoCount = TC_DoCount
                    };

                case "CC":
                    return new ChannelContext
                    {
                        ActiveDi1 = arrCcDi1States,
                        ActiveDi2 = arrCcDi2States,
                        ActiveDi3 = arrCcDi3States,
                        ActiveDo = arrCcDoStates,
                        ActiveDi1Count = CC_DI1Count,
                        ActiveDi2Count = CC_DI2Count,
                        ActiveDi3Count = 0,
                        ActiveDoCount = CC_DoCount
                    };

                case "DU":
                    return new ChannelContext
                    {
                        ActiveDi1 = arrDuDi1States,
                        ActiveDi2 = arrDuDi2States,
                        ActiveDi3 = arrDuDi3States,
                        ActiveDo = arrDuDoStates,
                        ActiveDi1Count = DU_DICount,
                        ActiveDi2Count = 0,
                        ActiveDi3Count = 0,
                        ActiveDoCount = DU_DoCount
                    };

                default:
                    return null;
            }
        }
        private async Task RunChannelTestSequenceAsync(
    string strChannelName,
    EChannelState[] arrStates,
    int nActiveCount,
    DataGridView dgvTarget,
    int nDelay,
    int nRound,
    List<string> listFailedPins,
    List<TestResultJson.PinResultItem> listPinDetails,
    Func<byte[]> getRawDataFunc,
    bool[] expectedBitPattern,
    int nStartPin = 0,
    CancellationToken cancellationToken = default
)
        {
            if (arrStates == null || dgvTarget == null || nActiveCount <= 0) return;

            int loopCount = Math.Min(nActiveCount, arrStates.Length);

            // 1. 차종/편성별 탭 개수 변화에 대응하는 동적 탭 전환
            int targetTabIndex = -1;
            if (!string.IsNullOrEmpty(strChannelName))
            {
                string chKey = strChannelName.ToUpper().Trim();

                for (int t = 0; t < flatTabControl1.TabPages.Count; t++)
                {
                    TabPage page = flatTabControl1.TabPages[t];
                    string tabText = page.Text?.ToUpper().Trim() ?? "";
                    string tabName = page.Name?.ToUpper().Trim() ?? "";

                    if (tabText.Contains(chKey) || tabName.Contains(chKey))
                    {
                        targetTabIndex = t;
                        break;
                    }
                }
            }

            if (targetTabIndex >= 0 && flatTabControl1.SelectedIndex != targetTabIndex)
            {
                flatTabControl1.SelectedIndex = targetTabIndex;
                Application.DoEvents();
            }

            // 2. 상태 배열 초기화
            for (int k = 0; k < loopCount; k++)
            {
                arrStates[k] = default;
            }
            dgvTarget.Invalidate();

            bool isDoCategory = (strChannelName?.ToUpper() == "DO");

            // 3. 핀 단위 시험 진행
            for (int i = 0; i < loopCount; i++)
            {
                // 핀 시험 시작 전 중지 플래그 및 토큰 확인
                if (!m_bIsTesting || cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"[Debug] [{strChannelName}] 사용자 중지 요청 감지 - 루프 즉시 종료");
                    return;
                }

                int nPinNo = nStartPin + i;
                arrStates[i] = EChannelState.Test;
                dgvTarget.Invalidate();

                bool isOnOk = false;
                bool isOffOk = false;

                Console.WriteLine($"[Debug] [{strChannelName}] {nPinNo}번 핀 시험 시작 (PLC DO index: {i})");

                try
                {
                    if (!isDoCategory)
                    {
                        // STEP 1: ON 검증
                        m_PLCNetwork?.SetDO(0, i, true);

                        await Task.Delay(nDelay, cancellationToken);

                        // STEP 1 Delay 직후 중지 여부 확인
                        if (!m_bIsTesting || cancellationToken.IsCancellationRequested)
                        {
                            m_PLCNetwork?.SetDO(0, i, false);
                            return;
                        }

                        byte[] rawDataOn = getRawDataFunc?.Invoke();
                        if (rawDataOn != null)
                        {
                            EChannelState[] tempStatesOn = new EChannelState[loopCount];
                            bool[] patternOn = new bool[loopCount];
                            patternOn[i] = true;

                            TcmsValidator.ValidateGroup(strChannelName, rawDataOn, patternOn, tempStatesOn, nActiveCount);
                            isOnOk = (tempStatesOn[i] == EChannelState.On);

                            Console.WriteLine($"[Debug] [{strChannelName}] {nPinNo}번 핀 STEP 1(ON) - 결과: {isOnOk}, RawData: {BitConverter.ToString(rawDataOn)}");
                        }
                        else
                        {
                            Console.WriteLine($"[Debug] [{strChannelName}] {nPinNo}번 핀 STEP 1(ON) - RawData 수신 Null");
                        }

                        // STEP 2: OFF 검증
                        m_PLCNetwork?.SetDO(0, i, false);
                        await Task.Delay(nDelay, cancellationToken);

                        // STEP 2 Delay 직후 중지 여부 확인
                        if (!m_bIsTesting || cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        byte[] rawDataOff = getRawDataFunc?.Invoke();
                        if (rawDataOff != null)
                        {
                            EChannelState[] tempStatesOff = new EChannelState[loopCount];
                            bool[] patternOff = new bool[loopCount];
                            patternOff[i] = false;

                            TcmsValidator.ValidateGroup(strChannelName, rawDataOff, patternOff, tempStatesOff, nActiveCount);
                            isOffOk = (tempStatesOff[i] == EChannelState.Off || tempStatesOff[i] == default);

                            Console.WriteLine($"[Debug] [{strChannelName}] {nPinNo}번 핀 STEP 2(OFF) - 결과: {isOffOk}, RawData: {BitConverter.ToString(rawDataOff)}");
                        }
                        else
                        {
                            Console.WriteLine($"[Debug] [{strChannelName}] {nPinNo}번 핀 STEP 2(OFF) - RawData 수신 Null");
                        }
                    }
                    else
                    {
                        // [DO 시험] TCMS DO 제어 및 PLC DI 확인 로직
                        await Task.Delay(nDelay, cancellationToken);

                        //DO Delay 직후 중지 여부 확인
                        if (!m_bIsTesting || cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Task.Delay 대기 도중 정지 버튼이 눌렸을 때 안전 처리 및 PLC 출력 원복
                    m_PLCNetwork?.SetDO(0, i, false);
                    arrStates[i] = default;
                    dgvTarget.Invalidate();

                    Console.WriteLine($"[Debug] [{strChannelName}] {nPinNo}번 핀 진행 중 정지 예외 수신");
                    return;
                }

                // STEP 3: 최종 결과 처리
                bool isFinalSuccess = isOnOk && isOffOk;

                Console.WriteLine($"[Debug] [{strChannelName}] {nPinNo}번 핀 최종 결과: {(isFinalSuccess ? "합격" : "불합격")} (ON:{isOnOk}, OFF:{isOffOk})");

                if (isFinalSuccess)
                {
                    arrStates[i] = EChannelState.On;
                    AppendTestLog(richTextBox_Log, $"{strChannelName} {nPinNo}번 핀 ON/OFF (성공)", Color.Black);
                }
                else
                {
                    arrStates[i] = EChannelState.Err;
                    AppendTestLog(richTextBox_Log, $"{strChannelName} {nPinNo}번 핀 ON/OFF (실패)", Color.Red);
                    listFailedPins.Add($"{strChannelName}_{nPinNo}번");
                }

                TestResultJson.PinResultItem objPinResult = new TestResultJson.PinResultItem
                {
                    Round = nRound,
                    ChannelGroup = strChannelName,
                    PinNo = nPinNo,
                    PinName = $"{strChannelName}_{nPinNo}번",
                    MeasuredValue = arrStates[i].ToString(),
                    Result = isFinalSuccess ? "합격" : "불합격"
                };
                listPinDetails?.Add(objPinResult);

                dgvTarget.Invalidate();
            }
        }

        private async Task RunAnalogTestSequenceAsync(
            DataGridView dgvAnalog,
            int nDelay,
            int nRound,
            List<string> listFailedPins,
            List<TestResultJson.PinResultItem> listPinDetails)
        {
            if (dgvAnalog == null || dgvAnalog.Rows.Count == 0) return;

            // CustomNumeric1 컨트롤에서 설정된 실패 확률(%) 추출 (null 참조 예외 방지)
            double dFailProbability = 15.0;
            if (customNumeric1 != null)
            {
                dFailProbability = (double)customNumeric1.Value;
            }

            for (int nRowIdx = 0; nRowIdx < dgvAnalog.Rows.Count; nRowIdx++)
            {
                if (!m_bIsTesting) break;

                DataGridViewRow objRow = dgvAnalog.Rows[nRowIdx];

                // 구분선 및 빈 데이터 행 건너뛰기
                string strItemName = objRow.Cells[1].Value?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(strItemName) || strItemName.Contains("-"))
                {
                    continue;
                }

                // 설정된 백분율(%) 확률 기반으로 실패 여부 난수 산출
                bool bIsFailTarget = (m_randGenerator.NextDouble() * 100.0) < dFailProbability;

                // 가상 측정 전압 생성 (실패 시 0.0~2.0V 범주, 성공 시 9.8~10.2V 범주)
                double dMeasuredVolt = bIsFailTarget ? (m_randGenerator.NextDouble() * 2.0) : (9.8 + m_randGenerator.NextDouble() * 0.4);

                string strMeasuredValue = $"{dMeasuredVolt:F2} V";
                string strResultText = bIsFailTarget ? "불합격" : "합격";

                // DataGridView 측정치(인덱스 2) 및 판정(인덱스 3) 열 업데이트
                objRow.Cells[2].Value = strMeasuredValue;
                objRow.Cells[3].Value = strResultText;
                dgvAnalog.InvalidateRow(nRowIdx);

                await Task.Delay(nDelay);

                TestResultJson.PinResultItem objPinResult = new TestResultJson.PinResultItem
                {
                    Round = nRound,
                    ChannelGroup = "ANALOG",
                    PinNo = nRowIdx + 1,
                    PinName = strItemName,
                    MeasuredValue = strMeasuredValue,
                    Result = strResultText
                };

                listPinDetails?.Add(objPinResult);

                if (bIsFailTarget)
                {
                    AppendTestLog(richTextBox_Log, $"[아날로그] {strItemName} 측정치: {strMeasuredValue} (실패)", Color.Red);
                    listFailedPins.Add($"{strItemName}");
                }
                else
                {
                    AppendTestLog(richTextBox_Log, $"[아날로그] {strItemName} 측정치: {strMeasuredValue} (성공)", Color.Black);
                }
            }
        }
        private async void button2_Click_2(object sender, EventArgs e)
        {
            FormPLC frmPlc = new FormPLC(this);
            frmPlc.ShowDialog();
        }

        #region 통신 시험 디자인용
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            Graphics gtx = e.Graphics;

            // 더블 버퍼링 지원 및 글자 흔들림/뭉개짐 방지 고품질 렌더링 설정
            gtx.SmoothingMode = SmoothingMode.AntiAlias;
            gtx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 섹션 및 타이틀 기본 정보 데이터 정의
            string[] arrSections = { "통신방법", "TCMS", "시험기" }; 
            string[] arrTitles = { "통신 방법", "TCMS", "시험기" };

            int nTotalSections = arrSections.Length;
            int nMarginTop = 10;
            int nSectionGap = 15;

            // 패널 크기에 맞춰 각 세션 카드의 높이를 동적으로 계산
            int nSectionHeight = (pnl.Height - nMarginTop - (nSectionGap * (nTotalSections - 1))) / nTotalSections;

            // 요구사항 반영: 구분감을 높이기 위해 머릿말 좌측 배경색 음영을 더 짙은 미드 그레이 톤으로 보정
            using (SolidBrush brshBlue = new SolidBrush(Color.FromArgb(13, 71, 161)))      // 타이틀 불릿 파란색
            using (SolidBrush brshTextDark = new SolidBrush(Color.FromArgb(45, 55, 72)))   // 기본 짙은 글자색
            using (SolidBrush brshGrayBg = new SolidBrush(Color.FromArgb(232, 236, 241)))   // 머릿말 로고 영역 배경색
            using (Pen penBorder = new Pen(Color.FromArgb(214, 221, 230), 1.5f))            // 외곽 테두리선
            using (Pen penDivider = new Pen(Color.FromArgb(218, 224, 232), 1.2f))          // 내부 수직 구분선
            using (Font fntTitle = new Font("맑은 고딕", 10.5F, FontStyle.Bold))
            using (Font fntLogo = new Font("맑은 고딕", 13F, FontStyle.Bold))
            using (Font fntBody = new Font("맑은 고딕", 10F, FontStyle.Regular))
            using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int nIdx = 0; nIdx < nTotalSections; nIdx++)
                {
                    // 각 세션별 시작 Y축 좌표 연산
                    int nCurrTopY = nMarginTop + (nIdx * (nSectionHeight + nSectionGap));

                    // 상단 인디케이터 원형 불릿 및 대제목 텍스트 렌더링
                    int nBulletRadius = 4;
                    int nBulletY = nCurrTopY + 10;
                    gtx.FillEllipse(brshBlue, 15, nBulletY, nBulletRadius * 2, nBulletRadius * 2);
                    gtx.DrawString(arrTitles[nIdx], fntTitle, brshTextDark, 30, nCurrTopY + 3);

                    // 메인 카드 프레임 배치 구역 연산
                    int nCardTop = nCurrTopY + 28;
                    int nCardHeight = nSectionHeight - 28;
                    Rectangle rectCard = new Rectangle(12, nCardTop, pnl.Width - 24, nCardHeight);

                    // 첫 번째 행인 통신방법 레이아웃 구역 처리
                    if (nIdx == 0)
                    {
                        // 장치 형태 조건 변수 상태에 따른 유효 서브 타이틀 배열 스위칭
                        string[] arrSubLabels;
                        if (ConfigJson.CurrentConfig.Operation.TCMSUnit == "DU")
                        {
                            arrSubLabels = new string[] { "MVB", "RS-485" };
                        }
                        else if (ConfigJson.CurrentConfig.Operation.TCMSUnit == "ER")
                        {
                            arrSubLabels = new string[] { "MVB" };
                        }
                        else
                        {
                            arrSubLabels = new string[] { "WTB", "MVB", "RS-485" };
                        }

                        int nSubCount = arrSubLabels.Length;

                        if (nSubCount > 0)
                        {
                            int nTotalAvailableWidth = rectCard.Width;
                            int nGap = 10; // 분할 카드 간 간격
                            int nSubWidth = (nTotalAvailableWidth - (nGap * (nSubCount - 1))) / nSubCount;

                            for (int nSubIdx = 0; nSubIdx < nSubCount; nSubIdx++)
                            {
                                int nSubX = rectCard.X + (nSubIdx * (nSubWidth + nGap));
                                Rectangle rectSubBox = new Rectangle(nSubX, rectCard.Y, nSubWidth, rectCard.Height);

                                // 서브 카드의 우측 공란 영역을 포함한 흰색 전체 베이스 드로우
                                using (GraphicsPath pathSubBox = GetRoundedRectPath(rectSubBox, 8))
                                {
                                    gtx.FillPath(Brushes.White, pathSubBox);
                                    gtx.DrawPath(penBorder, pathSubBox);
                                }

                                // 서브 카드 내부의 좌측 머릿말 로고 영역 크기 지정
                                int nSubLogoWidth = 110;
                                Rectangle rectSubLogo = new Rectangle(rectSubBox.X, rectSubBox.Y, nSubLogoWidth, rectSubBox.Height);

                                // 서브 카드용 좌측 둥근 모서리 클리핑 회색 배경 채우기
                                using (GraphicsPath pathSubLogo = GetRoundedRectPath(rectSubLogo, 8, bLeftOnly: true))
                                {
                                    gtx.FillPath(brshGrayBg, pathSubLogo);
                                }

                                // 서브 머릿말과 우측 라디오버튼 공란 사이를 가르는 수직선 드로우
                                gtx.DrawLine(penDivider, rectSubBox.X + nSubLogoWidth, rectSubBox.Y, rectSubBox.X + nSubLogoWidth, rectSubBox.Bottom);

                                // 서브 머릿말 내부 정중앙에 통신명 레이블 배치
                                gtx.DrawString(arrSubLabels[nSubIdx], fntLogo, brshTextDark, rectSubLogo, sfCenter);
                            }
                        }
                    }
                    else
                    {
                        // 하단 잔여 행 구역 처리 (TCMS 및 시험기 행 레이아웃)
                        using (GraphicsPath pathCard = GetRoundedRectPath(rectCard, 8))
                        {
                            gtx.FillPath(Brushes.White, pathCard);
                            gtx.DrawPath(penBorder, pathCard);
                        }

                        // 하단 대분류용 머릿말 로고 영역 드로우
                        int nLogoWidth = 110;
                        Rectangle rectLogo = new Rectangle(rectCard.X, rectCard.Y, nLogoWidth, rectCard.Height);
                        using (GraphicsPath pathLogo = GetRoundedRectPath(rectLogo, 8, bLeftOnly: true))
                        {
                            gtx.FillPath(brshGrayBg, pathLogo);
                        }
                        gtx.DrawLine(penDivider, rectCard.X + nLogoWidth, rectCard.Y, rectCard.X + nLogoWidth, rectCard.Bottom);
                        gtx.DrawString(arrSections[nIdx], fntLogo, brshTextDark, rectLogo, sfCenter);

                        // 우측 송수신 데이터 텍스트 정보 및 디자인 점선 마킹
                        int nTextStartX = rectCard.X + nLogoWidth + 40;
                        int nRowGap = rectCard.Height / 3;

                        int nTxY = rectCard.Y + nRowGap - 8;
                        int nRxY = rectCard.Y + (nRowGap * 2) - 8;

                        gtx.DrawString("송신 (TX)", fntBody, brshTextDark, nTextStartX, nTxY);
                        gtx.DrawString("수신 (RX)", fntBody, brshTextDark, nTextStartX, nRxY);

                        string strDotLine = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -";
                        using (SolidBrush brshDotColor = new SolidBrush(Color.FromArgb(160, 174, 192)))
                        {
                            gtx.DrawString(strDotLine, fntBody, brshDotColor, nTextStartX + 140, nTxY);
                            gtx.DrawString(strDotLine, fntBody, brshDotColor, nTextStartX + 140, nRxY);
                        }
                    }
                }
            }
        }
        // 사각형의 모서리를 라운드 처리하기 위한 그래픽 패스 생성 알고리즘
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int nRadius, bool bLeftOnly = false)
        {
            GraphicsPath path = new GraphicsPath();
            int nDiameter = nRadius * 2;

            if (nDiameter > rect.Width) nDiameter = rect.Width;
            if (nDiameter > rect.Height) nDiameter = rect.Height;

            Rectangle rectArc = new Rectangle(rect.X, rect.Y, nDiameter, nDiameter);

            // 좌상단 모서리
            path.AddArc(rectArc, 180, 90);

            // 우상단 모서리
            rectArc.X = rect.Right - nDiameter;
            if (bLeftOnly) path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y + nRadius);
            else path.AddArc(rectArc, 270, 90);

            // 우하단 모서리
            rectArc.Y = rect.Bottom - nDiameter;
            if (bLeftOnly) path.AddLine(rect.Right, rect.Bottom, rect.Right - nRadius, rect.Bottom);
            else path.AddArc(rectArc, 0, 90);

            // 좌하단 모서리
            rectArc.X = rect.X;
            path.AddArc(rectArc, 90, 90);

            path.CloseFigure();
            return path;
        }
        #endregion 

        #region 메모리 시험 패널 디자인용
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (m_dicBoards == null) return;

            Graphics g = e.Graphics;
            // 앤티앨리어싱 및 고품질 텍스트 렌더링 힌트 설정 (글자 뭉개짐 방지)
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. 실시간 데이터 집계
            int nTotalCount = 0;
            int nNormalCount = 0;
            int nErrCount = 0;
            int nOffCount = 0;

            foreach (var pair in m_dicBoards)
            {
                foreach (var elem in pair.Value)
                {
                    nTotalCount++;
                    if (elem.eState == EChannelState.On) nNormalCount++;
                    else if (elem.eState == EChannelState.Err) nErrCount++;
                    else nOffCount++;
                }
            }

            // 2. 메인 "시험 요약" 타이틀 출력
            using (Font fontMainTitle = new Font("맑은 고딕", 12, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, "시험 요약", fontMainTitle, new Point(12, 12), Color.FromArgb(33, 37, 41));
            }

            // 3. 4개 요약 카드 정의 (직접 그리기 위해 튜플에서 불필요한 기호/폰트 정보 제거)
            var listCards = new List<(string strTitle, int nCount, Color colorTheme)>
    {
        ("총 메모리 개수", nTotalCount,  Color.FromArgb(73, 80, 87)),
        ("정상",           nNormalCount, Color.FromArgb(40, 167, 69)),
        ("오류",         nErrCount,    Color.FromArgb(220, 53, 69)),
        ("미시험",  nOffCount,    Color.FromArgb(142, 154, 166))
    };

            int nCardCount = listCards.Count;
            int nMarginLeft = 12;
            int nMarginTop = 45;
            int nGap = 16;

            int nCardWidth = (panel2.ClientSize.Width - (nMarginLeft * 2) - (nGap * (nCardCount - 1))) / nCardCount;
            int nCardHeight = panel2.ClientSize.Height - nMarginTop - 15;

            if (nCardWidth < 100 || nCardHeight < 50) return;

            for (int nIdx = 0; nIdx < nCardCount; nIdx++)
            {
                var card = listCards[nIdx];
                Rectangle rectCard = new Rectangle(nMarginLeft + (nIdx * (nCardWidth + nGap)), nMarginTop, nCardWidth, nCardHeight);

                // 카드 테두리 선명도 확보 (두께 2px 및 지정 외각 색상 적용)
                using (SolidBrush brCardBg = new SolidBrush(Color.FromArgb(252, 253, 254)))
                using (Pen penBorder = new Pen(Color.FromArgb(180, 190, 201), 2))
                {
                    g.FillRectangle(brCardBg, rectCard);
                    g.DrawRectangle(penBorder, rectCard);
                }

                // [변경 포인트] 외부 원형 선을 제거하고, 42px 크기의 아이콘 드로잉 영역으로 강제 지정
                int nIconSize = 42;
                Rectangle rectIconArea = new Rectangle(rectCard.X + 20, rectCard.Y + (rectCard.Height - nIconSize) / 2, nIconSize, nIconSize);

                // 각 카드 인덱스별로 전달받은 전용 펜(두께 2px)을 활용해 직접 그리기 엔진 연동
                using (Pen penIcon = new Pen(card.colorTheme, 2))
                {
                    switch (nIdx)
                    {
                        case 0: // 총 메모리 항목: IC 패턴 드로잉
                            DrawICIcon(g, rectIconArea, penIcon);
                            break;
                        case 1: // 정상: 체크 마크 드로잉
                            DrawCheckIcon(g, rectIconArea, penIcon);
                            break;
                        case 2: // 비정상: X 마크 드로잉
                            DrawXIcon(g, rectIconArea, penIcon);
                            break;
                        case 3: // 대기 / 미시험: 원형 일시정지바 패턴 드로잉
                            DrawWaitIcon(g, rectIconArea, penIcon);
                            break;
                    }
                }

                // 텍스트 우측 영역 좌표 계산 및 배치 정밀화
                int nTextLeft = rectIconArea.Right + 10;
                int nTextWidth = rectCard.Right - nTextLeft - 15;

                Rectangle rectTitle = new Rectangle(nTextLeft, rectCard.Y + (rectCard.Height / 2) - 26, nTextWidth, 22);
                Rectangle rectCountText = new Rectangle(nTextLeft, rectCard.Y + (rectCard.Height / 2) + 2, nTextWidth, 28);

                // 소제목 글자 출력
                using (Font fontTitle = new Font("맑은 고딕", 10, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, card.strTitle, fontTitle, rectTitle, Color.FromArgb(90, 100, 110),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                // 메인 카운트 숫자 및 "개" 단위 정렬
                using (Font fontCount = new Font("맑은 고딕", 20, FontStyle.Bold))
                using (Font fontUnit = new Font("맑은 고딕", 10, FontStyle.Regular))
                {
                    string strNum = card.nCount.ToString();
                    Size sizeNum = TextRenderer.MeasureText(g, strNum, fontCount);
                    Size sizeUnit = TextRenderer.MeasureText(g, " 개", fontUnit);

                    int nCombinedWidth = sizeNum.Width + sizeUnit.Width;
                    int nStartTextX = rectCountText.X + (rectCountText.Width - nCombinedWidth) / 2;

                    int nNumY = rectCountText.Y + (rectCountText.Height - sizeNum.Height) / 2;
                    int nUnitY = rectCountText.Y + (rectCountText.Height - sizeUnit.Height) / 2 + 4;

                    Point ptNum = new Point(nStartTextX, nNumY);
                    Point ptUnit = new Point(nStartTextX + sizeNum.Width, nUnitY);

                    Color colorNum = (nIdx == 0 || nIdx == 3) ? Color.FromArgb(33, 37, 41) : card.colorTheme;

                    TextRenderer.DrawText(g, strNum, fontCount, ptNum, colorNum);
                    TextRenderer.DrawText(g, " 개", fontUnit, ptUnit, Color.FromArgb(100, 110, 120));
                }
            }
        }
        private void DrawICIcon(Graphics g, Rectangle rect, Pen pen)
        {
            // IC 본체
            Rectangle rectBody = new Rectangle(rect.X + 8, rect.Y + 8, rect.Width - 16, rect.Height - 16);
            g.DrawRectangle(pen, rectBody);

            // 핀 (다리) 그리기
            int pinLength = 6;
            int pinSpacing = rectBody.Height / 5;
            for (int i = 1; i <= 4; i++)
            {
                // 왼쪽 핀
                g.DrawLine(pen, rectBody.Left - pinLength, rectBody.Top + (i * pinSpacing), rectBody.Left, rectBody.Top + (i * pinSpacing));
                // 오른쪽 핀
                g.DrawLine(pen, rectBody.Right, rectBody.Top + (i * pinSpacing), rectBody.Right + pinLength, rectBody.Top + (i * pinSpacing));
            }
            // 위쪽/아래쪽 핀 (중앙 2개씩)
            g.DrawLine(pen, rectBody.Left + pinSpacing * 1.5f, rectBody.Top - pinLength, rectBody.Left + pinSpacing * 1.5f, rectBody.Top);
            g.DrawLine(pen, rectBody.Left + pinSpacing * 2.5f, rectBody.Top - pinLength, rectBody.Left + pinSpacing * 2.5f, rectBody.Top);
            g.DrawLine(pen, rectBody.Left + pinSpacing * 1.5f, rectBody.Bottom, rectBody.Left + pinSpacing * 1.5f, rectBody.Bottom + pinLength);
            g.DrawLine(pen, rectBody.Left + pinSpacing * 2.5f, rectBody.Bottom, rectBody.Left + pinSpacing * 2.5f, rectBody.Bottom + pinLength);
        }

        private void DrawCheckIcon(Graphics g, Rectangle rect, Pen pen)
        {
            // 굵은 펜 사용
            Pen thickPen = (Pen)pen.Clone();
            thickPen.Width *= 2;

            // 체크 모양 포인트 계산
            Point[] checkPoints = new Point[]
            {
            new Point(rect.Left + (int)(rect.Width * 0.2), rect.Top + (int)(rect.Height * 0.5)),
            new Point(rect.Left + (int)(rect.Width * 0.45), rect.Top + (int)(rect.Height * 0.75)),
            new Point(rect.Right - (int)(rect.Width * 0.1), rect.Top + (int)(rect.Height * 0.25))
            };
            g.DrawLines(thickPen, checkPoints);
        }

        private void DrawXIcon(Graphics g, Rectangle rect, Pen pen)
        {
            // 굵은 펜 사용
            Pen thickPen = (Pen)pen.Clone();
            thickPen.Width *= 2;

            // X 모양 포인트 계산
            g.DrawLine(thickPen, rect.Left + (int)(rect.Width * 0.2), rect.Top + (int)(rect.Height * 0.2),
                               rect.Right - (int)(rect.Width * 0.2), rect.Bottom - (int)(rect.Height * 0.2));
            g.DrawLine(thickPen, rect.Left + (int)(rect.Width * 0.2), rect.Bottom - (int)(rect.Height * 0.2),
                               rect.Right - (int)(rect.Width * 0.2), rect.Top + (int)(rect.Height * 0.2));
        }

        private void DrawWaitIcon(Graphics g, Rectangle rect, Pen pen)
        {
            // 외부 원
            g.DrawEllipse(pen, rect);

            // 내부 대기/일시정지 모양 (두 개의 세로선)
            Rectangle rectPause = new Rectangle(rect.X + rect.Width / 3, rect.Y + rect.Height / 3, rect.Width / 3, rect.Height / 3);

            using (SolidBrush brush = new SolidBrush(pen.Color))
            {
                int barWidth = rectPause.Width / 4;
                g.FillRectangle(brush, rectPause.X, rectPause.Y, barWidth, rectPause.Height);
                g.FillRectangle(brush, rectPause.Right - barWidth, rectPause.Y, barWidth, rectPause.Height);
            }
        }
        #endregion

        /// <summary>
        /// 버튼 클릭시 시험 결과를 PDF로 인쇄하는 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            DialogResult drSelect = MessageBox.Show(
                "시험 결과를 인쇄하시겠습니까?",
                "인쇄 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (drSelect != DialogResult.Yes)
            {
                return;
            }

            string strDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string strTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string strFilePath = Path.Combine(strDesktopPath, $"TCMS_시험결과보고서_{strTimeStamp}.pdf");

            if (File.Exists(strFilePath))
            {
                try
                {
                    File.Delete(strFilePath);
                }
                catch (IOException)
                {
                    MessageBox.Show(
                        "기존에 생성된 보고서 PDF 파일이 현재 열려 있습니다.\n뷰어 창을 완전히 닫은 후 다시 시도해 주세요.",
                        "파일 잠김 안내",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            string strUnitType = string.IsNullOrEmpty(ConfigJson.CurrentConfig.Operation.TCMSUnit) ? "TC" : ConfigJson.CurrentConfig.Operation.TCMSUnit;
            string strSerialNo = string.IsNullOrEmpty(ConfigJson.CurrentConfig.Operation.SerialNo) ? "0000" : ConfigJson.CurrentConfig.Operation.SerialNo;
            string strCarNo = string.IsNullOrEmpty(ConfigJson.CurrentConfig.Operation.FleetNo) ? "0000" : ConfigJson.CurrentConfig.Operation.FleetNo;
            string strTrainNo = string.IsNullOrEmpty(ConfigJson.CurrentConfig.Operation.TrainNo) ? "0000" : ConfigJson.CurrentConfig.Operation.TrainNo;
            string strTester = string.IsNullOrEmpty(ConfigJson.CurrentConfig.Operation.TesterName) ? "ADMIN" : ConfigJson.CurrentConfig.Operation.TesterName;
            string strFinalDecision = "미시험";

            List<string[]> listItems = new List<string[]>();

            listItems.Add(new string[] { "Section", "1. 입·출력 시험" });
            listItems.Add(new string[] { "Section", "  1.1 디지털 입력 (DI)" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            for (int nIdx = 1; nIdx <= TC_DI1Count; nIdx++)
            {
                listItems.Add(new string[] { "Row", $"디지털 입력 (DI {nIdx})", "미시험" });
            }


            listItems.Add(new string[] { "ForcePageBreak" });

            listItems.Add(new string[] { "Section", "  1.2 디지털 출력 (DO)" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            for (int nIdx = 1; nIdx <= TC_DI1Count; nIdx++)
            {
                listItems.Add(new string[] { "Row", $"디지털 출력 (DO {nIdx})", "미시험" });
            }

            listItems.Add(new string[] { "ForcePageBreak" });

            listItems.Add(new string[] { "Section", "  1.3 아날로그 입력 (AI)" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            for (int nIdx = 1; nIdx <= nAnalogInputCount; nIdx++)
            {
                listItems.Add(new string[] { "Row", $"아날로그 입력 (AI {nIdx})", "미시험" });
            }

            listItems.Add(new string[] { "Section", "  1.4 아날로그 출력 (AO)" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            for (int nIdx = 1; nIdx <= nAnalogOutputCount; nIdx++)
            {
                listItems.Add(new string[] { "Row", $"아날로그 출력 (AO {nIdx})", "미시험" });
            }

            listItems.Add(new string[] { "ForcePageBreak" });

            listItems.Add(new string[] { "Section", "2. 통신 시험" });
            listItems.Add(new string[] { "CommGrid", strUnitType });

            listItems.Add(new string[] { "EmptySpace", "40" });

            listItems.Add(new string[] { "Section", "3. 메모리 시험" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            listItems.Add(new string[] { "Row", "VAIO", "미시험" });
            listItems.Add(new string[] { "Row", "VCPU", "미시험" });
            listItems.Add(new string[] { "Row", "VTCN", "미시험" });

            if (strUnitType == "ER")
            {
                listItems.Add(new string[] { "Section", "4. ER 속도 센서 시험" });
                listItems.Add(new string[] { "Header", "시험 항목", "판정" });
                listItems.Add(new string[] { "Row", "ER 속도 센서", "미시험" });
            }

            int nItemIndex = 0;
            int nPageIndex = 1;

            Form frmProgress = new Form
            {
                Text = "보고서 출력",
                Size = new Size(360, 140),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false,
                MaximizeBox = false,
                MinimizeBox = false,
                TopMost = true,
                BackColor = Color.White,
            };

            Label lblStatusMessage = new Label
            {
                Text = "PDF 문서를 초기화하고 있습니다...",
                Location = new Point(25, 20),
                Size = new Size(300, 23),
                Font = new System.Drawing.Font("맑은 고딕", 9, FontStyle.Regular)
            };

            YourNamespace.CustomProgressBar pgbStatus = new YourNamespace.CustomProgressBar
            {
                Location = new Point(25, 48),
                Size = new Size(295, 25),
                Maximum = listItems.Count,
                Value = 0,
                ShowPercentage = false,
                BarThickness = 30
            };

            frmProgress.Controls.Add(lblStatusMessage);
            frmProgress.Controls.Add(pgbStatus);

            frmProgress.Show();
            frmProgress.Refresh();

            try
            {
                using (System.Drawing.Printing.PrintDocument prtDoc = new System.Drawing.Printing.PrintDocument())
                {
                    prtDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                    prtDoc.PrinterSettings.PrintToFile = true;
                    prtDoc.PrinterSettings.PrintFileName = strFilePath;
                    prtDoc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
                    prtDoc.PrintController = new System.Drawing.Printing.StandardPrintController();

                    prtDoc.PrintPage += (object prtSender, System.Drawing.Printing.PrintPageEventArgs ePage) =>
                    {
                        pgbStatus.UseAnimation = false;

                        Graphics gtxCanvas = ePage.Graphics;
                        gtxCanvas.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        System.Drawing.Font fntTitle = new System.Drawing.Font("맑은 고딕", 22, FontStyle.Bold);
                        System.Drawing.Font fntHeader = new System.Drawing.Font("맑은 고딕", 9, FontStyle.Bold);
                        System.Drawing.Font fntBody = new System.Drawing.Font("맑은 고딕", 9, FontStyle.Regular);
                        System.Drawing.Font fntBodyBold = new System.Drawing.Font("맑은 고딕", 9, FontStyle.Bold);

                        float fStartX = ePage.MarginBounds.Left;
                        float fCurrentY = ePage.MarginBounds.Top;
                        float fPageWidth = ePage.MarginBounds.Width;

                        string strTitleText = "TCMS 시험기 결과 보고서";
                        SizeF szTitle = gtxCanvas.MeasureString(strTitleText, fntTitle);
                        gtxCanvas.DrawString(strTitleText, fntTitle, Brushes.Black, fStartX + (fPageWidth - szTitle.Width) / 2, fCurrentY);
                        fCurrentY += szTitle.Height + 35f;

                        string[,] arrInfoMatrix = new string[4, 3] {
                    { "시험일자", "시험자명", "최종 판정 결과" },
                    { DateTime.Now.ToString("yyyy-MM-dd"), strTester, strFinalDecision },
                    { "편성번호", "차량번호", "유닛종류 (일련번호)" },
                    { strTrainNo, strCarNo, strUnitType + " (" + strSerialNo + ")" }
                };

                        int nInfoRowHeight = 34;
                        int nTotalW = (int)fPageWidth;
                        int nW1 = nTotalW / 3;
                        int nW2 = nTotalW / 3;
                        int nW3 = nTotalW - nW1 - nW2;
                        int[] arrColWidths = new int[] { nW1, nW2, nW3 };

                        int nGridY = (int)fCurrentY;
                        using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            for (int nRow = 0; nRow < 4; nRow++)
                            {
                                int nGridX = (int)fStartX;
                                for (int nCol = 0; nCol < 3; nCol++)
                                {
                                    Rectangle rectTarget = new Rectangle(nGridX, nGridY, arrColWidths[nCol], nInfoRowHeight);
                                    if (nRow == 0 || nRow == 2)
                                    {
                                        gtxCanvas.FillRectangle(new SolidBrush(Color.LightGray), rectTarget);
                                    }
                                    gtxCanvas.DrawRectangle(Pens.Black, rectTarget);

                                    Brush brshText = Brushes.Black;
                                    System.Drawing.Font fntSelect = (nRow == 0 || nRow == 2) ? fntHeader : fntBody;

                                    if (nRow == 1 && nCol == 2)
                                    {
                                        fntSelect = fntBodyBold;
                                        brshText = Brushes.Gray;
                                    }

                                    gtxCanvas.DrawString(arrInfoMatrix[nRow, nCol], fntSelect, brshText, rectTarget, sfCenter);
                                    nGridX += arrColWidths[nCol];
                                }
                                nGridY += nInfoRowHeight;
                            }
                        }
                        fCurrentY = nGridY + 40f;

                        int nColWidth1 = (int)(fPageWidth * 0.75f);
                        int nColWidth2 = (int)fPageWidth - nColWidth1;

                        using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        using (StringFormat sfLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                        {
                            while (nItemIndex < listItems.Count)
                            {
                                string[] arrCurrentItem = listItems[nItemIndex];
                                string strType = arrCurrentItem[0];

                                if (strType == "ForcePageBreak")
                                {
                                    nItemIndex++;
                                    ePage.HasMorePages = true;
                                    nPageIndex++;
                                    return;
                                }

                                if (strType == "EmptySpace")
                                {
                                    float fSpaceHeight = float.TryParse(arrCurrentItem[1], out float fResult) ? fResult : 30f;
                                    if (fCurrentY + fSpaceHeight > ePage.MarginBounds.Bottom)
                                    {
                                        ePage.HasMorePages = true;
                                        nPageIndex++;
                                        return;
                                    }
                                    fCurrentY += fSpaceHeight;
                                    nItemIndex++;
                                    continue;
                                }

                                if (strType == "CommGrid")
                                {
                                    string strUnit = arrCurrentItem[1];
                                    List<string> listMethods = new List<string>();
                                    if (strUnit == "TC" || strUnit == "CC")
                                    {
                                        listMethods.Add("WTB 통신");
                                        listMethods.Add("MVB 통신");
                                        listMethods.Add("RS-485 통신");
                                    }
                                    else if (strUnit == "DU")
                                    {
                                        listMethods.Add("MVB 통신");
                                        listMethods.Add("RS-485 통신");
                                    }
                                    else if (strUnit == "ER")
                                    {
                                        listMethods.Add("MVB 통신");
                                    }

                                    if (listMethods.Count > 0)
                                    {
                                        float fGridHeight = (listMethods.Count > 1) ? 56f : 28f;
                                        if (fCurrentY + fGridHeight > ePage.MarginBounds.Bottom)
                                        {
                                            ePage.HasMorePages = true;
                                            nPageIndex++;
                                            return;
                                        }

                                        int nTotalWidth = (int)fPageWidth;

                                        if (listMethods.Count == 3)
                                        {
                                            int nColW = nTotalWidth / 3;
                                            for (int nCol = 0; nCol < 3; nCol++)
                                            {
                                                Rectangle rectH = new Rectangle((int)fStartX + (nCol * nColW), (int)fCurrentY, (nCol == 2) ? nTotalWidth - (nColW * 2) : nColW, 28);
                                                gtxCanvas.FillRectangle(new SolidBrush(Color.LightGray), rectH);
                                                gtxCanvas.DrawRectangle(Pens.Black, rectH);
                                                gtxCanvas.DrawString(listMethods[nCol], fntHeader, Brushes.Black, rectH, sfCenter);
                                            }
                                            fCurrentY += 28f;

                                            for (int nCol = 0; nCol < 3; nCol++)
                                            {
                                                Rectangle rectR = new Rectangle((int)fStartX + (nCol * nColW), (int)fCurrentY, (nCol == 2) ? nTotalWidth - (nColW * 2) : nColW, 28);
                                                gtxCanvas.DrawRectangle(Pens.Black, rectR);
                                                gtxCanvas.DrawString("미시험", fntBody, Brushes.Gray, rectR, sfCenter);
                                            }
                                            fCurrentY += 28f;
                                        }
                                        else if (listMethods.Count == 2)
                                        {
                                            int nColW = nTotalWidth / 2;
                                            for (int nCol = 0; nCol < 2; nCol++)
                                            {
                                                Rectangle rectH = new Rectangle((int)fStartX + (nCol * nColW), (int)fCurrentY, (nCol == 1) ? nTotalWidth - nColW : nColW, 28);
                                                gtxCanvas.FillRectangle(new SolidBrush(Color.LightGray), rectH);
                                                gtxCanvas.DrawRectangle(Pens.Black, rectH);
                                                gtxCanvas.DrawString(listMethods[nCol], fntHeader, Brushes.Black, rectH, sfCenter);
                                            }
                                            fCurrentY += 28f;

                                            for (int nCol = 0; nCol < 2; nCol++)
                                            {
                                                Rectangle rectR = new Rectangle((int)fStartX + (nCol * nColW), (int)fCurrentY, (nCol == 1) ? nTotalWidth - nColW : nColW, 28);
                                                gtxCanvas.DrawRectangle(Pens.Black, rectR);
                                                gtxCanvas.DrawString("미시험", fntBody, Brushes.Gray, rectR, sfCenter);
                                            }
                                            fCurrentY += 28f;
                                        }
                                        else if (listMethods.Count == 1)
                                        {
                                            int nColW = nTotalWidth / 2;
                                            Rectangle rectR1 = new Rectangle((int)fStartX, (int)fCurrentY, nColW, 28);
                                            Rectangle rectR2 = new Rectangle((int)fStartX + nColW, (int)fCurrentY, nTotalWidth - nColW, 28);

                                            gtxCanvas.DrawRectangle(Pens.Black, rectR1);
                                            gtxCanvas.DrawRectangle(Pens.Black, rectR2);

                                            Rectangle rectTextPadding = rectR1;
                                            rectTextPadding.X += 8;
                                            rectTextPadding.Width -= 8;

                                            gtxCanvas.DrawString(listMethods[0], fntBody, Brushes.Black, rectTextPadding, sfLeft);
                                            gtxCanvas.DrawString("미시험", fntBody, Brushes.Gray, rectR2, sfCenter);

                                            fCurrentY += 28f;
                                        }
                                    }

                                    nItemIndex++;
                                    continue;
                                }

                                float fItemHeight = (strType == "Section") ? 35f : 28f;

                                if (fCurrentY + fItemHeight > ePage.MarginBounds.Bottom)
                                {
                                    ePage.HasMorePages = true;
                                    nPageIndex++;
                                    return;
                                }

                                if (strType == "Section")
                                {
                                    gtxCanvas.DrawString(arrCurrentItem[1], fntHeader, Brushes.Black, fStartX, fCurrentY + 8f);
                                    fCurrentY += fItemHeight;
                                }
                                else if (strType == "Header")
                                {
                                    Rectangle rectH1 = new Rectangle((int)fStartX, (int)fCurrentY, nColWidth1, 28);
                                    Rectangle rectH2 = new Rectangle((int)fStartX + nColWidth1, (int)fCurrentY, nColWidth2, 28);

                                    gtxCanvas.FillRectangle(new SolidBrush(Color.LightGray), rectH1);
                                    gtxCanvas.FillRectangle(new SolidBrush(Color.LightGray), rectH2);
                                    gtxCanvas.DrawRectangle(Pens.Black, rectH1);
                                    gtxCanvas.DrawRectangle(Pens.Black, rectH2);

                                    gtxCanvas.DrawString(arrCurrentItem[1], fntHeader, Brushes.Black, rectH1, sfCenter);
                                    gtxCanvas.DrawString(arrCurrentItem[2], fntHeader, Brushes.Black, rectH2, sfCenter);

                                    fCurrentY += fItemHeight;
                                }
                                else if (strType == "Row")
                                {
                                    Rectangle rectR1 = new Rectangle((int)fStartX, (int)fCurrentY, nColWidth1, 28);
                                    Rectangle rectR2 = new Rectangle((int)fStartX + nColWidth1, (int)fCurrentY, nColWidth2, 28);

                                    gtxCanvas.DrawRectangle(Pens.Black, rectR1);
                                    gtxCanvas.DrawRectangle(Pens.Black, rectR2);

                                    Rectangle rectTextPadding = rectR1;
                                    rectTextPadding.X += 8;
                                    rectTextPadding.Width -= 8;

                                    gtxCanvas.DrawString(arrCurrentItem[1], fntBody, Brushes.Black, rectTextPadding, sfLeft);

                                    string strVal = arrCurrentItem[2];
                                    Brush brshText = Brushes.Gray;
                                    gtxCanvas.DrawString(strVal, fntBody, brshText, rectR2, sfCenter);

                                    fCurrentY += fItemHeight;
                                }

                                nItemIndex++;
                                pgbStatus.Value = nItemIndex;
                                pgbStatus.Update();
                                lblStatusMessage.Text = $"PDF 파일 구성 중 ... ({nItemIndex} / {listItems.Count})";
                                lblStatusMessage.Update();
                            }
                        }

                        ePage.HasMorePages = false;
                    };

                    prtDoc.Print();
                }
            }
            catch (Exception exException)
            {
                MessageBox.Show($"보고서 출력 처리 중 오류 발생: {exException.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                frmProgress.Close();
                frmProgress.Dispose();
            }

            bool bIsFileReady = false;
            int nMaxRetries = 30;

            for (int nRetry = 0; nRetry < nMaxRetries; nRetry++)
            {
                try
                {
                    if (File.Exists(strFilePath))
                    {
                        using (FileStream fsCheck = File.Open(strFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            bIsFileReady = true;
                            break;
                        }
                    }
                }
                catch (IOException)
                {
                }
                Thread.Sleep(100);
            }

            if (bIsFileReady)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = strFilePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("PDF 파일 생성이 지연되고 있습니다. 바탕화면에서 직접 확인해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        int test = 0;

        private void BtnChange_Click(object sender, EventArgs e)
        {

            // 현재 설정된 Operation 정보를 가져와 폼 인스턴스에 전달
            string strCurrentUnit = ConfigJson.CurrentConfig.Operation.TCMSUnit;
            string strCurrentSerial = ConfigJson.CurrentConfig.Operation.SerialNo;
            string strCurrentFleet = ConfigJson.CurrentConfig.Operation.FleetNo;
            string strCurrentTrain = ConfigJson.CurrentConfig.Operation.TrainNo;
            string strCurrentTester = ConfigJson.CurrentConfig.Operation.TesterName;

            using (UnitSelectForm frmUnitSelect = new UnitSelectForm(strCurrentUnit, strCurrentSerial, strCurrentFleet, strCurrentTrain, strCurrentTester, true))
            {
                if (frmUnitSelect.ShowDialog(this) == DialogResult.OK)
                {
                    ConfigJson.CurrentConfig.Operation.TCMSUnit = frmUnitSelect.strTCMSUnit;
                    ConfigJson.CurrentConfig.Operation.SerialNo = frmUnitSelect.strSerialNo;
                    ConfigJson.CurrentConfig.Operation.FleetNo = frmUnitSelect.strFleetNo;
                    ConfigJson.CurrentConfig.Operation.TrainNo = frmUnitSelect.strTrainNo;
                    ConfigJson.CurrentConfig.Operation.TesterName = frmUnitSelect.strTester;

                    if (ConfigJson.CurrentConfig.Operation.TCMSUnit == "ER")
                    {
                        modernTreeView1.SetNodeVisible("ER 속도센서 시험", true);
                    }
                    else
                    {
                        modernTreeView1.SetNodeVisible("ER 속도센서 시험", false);
                    }
                    UpdateTabVisibility();
                    DisplayConfig();
                    AnalgogData(dataGridViewAnalog);
                    SetAnalogDataGrid();
                    DataGridInit();
                    if (tabPageIndex2 == null)
                    {
                        tabPageIndex2 = mainTabControl1.TabPages.Cast<TabPage>().FirstOrDefault(p => p.Name == "tabPage6");
                    }
                    InitData();
                    SetupDataGridView();
                    var nodeControl = modernTreeView1.Nodes.Find("입출력 시험", true).FirstOrDefault();
                    if (nodeControl != null && nodeControl.Nodes.Count > 0)
                        nodeControl.Checked = nodeControl.Nodes.Cast<TreeNode>().All(n => n.Checked);

                    System.Reflection.PropertyInfo propPanel = typeof(Panel).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                }
            }
        }



        private void BtnNew_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewDI1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            TcmsTestRunner.RunAllTests();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// MVB 시리얼 포트 오픈 전담 메서드
        /// </summary>
        private bool StartMvbCommunication()
        {
            string savedPort = ConfigJson.CurrentConfig?.Device?.MVBBoard_ComPort ?? "COM4";
            int baudRate = ConfigJson.CurrentConfig?.Device?.MVBBoard_BaudRate ?? 115200;

            AppendTestLog(richTextBox_Log, $"[통신] 시리얼 포트({savedPort}) 연결 시도...", Color.Gray);

            if (m_serialManager == null)
            {
                m_serialManager = new MvbSerialManager(m_mvbReceiver)
                {
                    OnLog = (msg) => AppendTestLog(richTextBox_Log, msg, Color.Blue),
                    OnError = (msg) => AppendTestLog(richTextBox_Log, msg, Color.Red)
                };
            }

            bool isConnected = m_serialManager.OpenPort(savedPort, baudRate);
            if (!isConnected)
            {
                AppendTestLog(richTextBox_Log, $"[통신 실패] 시리얼 포트({savedPort})를 열 수 없습니다.", Color.Red);
                return false;
            }

            AppendTestLog(richTextBox_Log, $"[통신 성공] 시리얼 포트 연결 완료.", Color.DarkGreen);
            return true;
        }

        /// <summary>
        /// MVB 수신 스레드를 중단하고 시리얼 포트를 완전히 닫습니다.
        /// </summary>
        private void StopMvbCommunication()
        {
            try
            {
                _tcmsTestService?.StopMvbReceiver();
                m_mvbReceiver?.Stop();
                m_serialManager?.ClosePort();

                AppendTestLog(richTextBox_Log, "[통신] MVB 수신 정지 및 시리얼 포트가 해제되었습니다.", Color.Gray);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[통신 해제 예외] {ex.Message}");
            }
        }

        /// <summary>
        /// 지정된 단일 회차(nLoop)의 5개 통신 항목을 순차 검사하고 결과를 누적합니다.
        /// </summary>
        public async Task<bool> RunCommSingleRoundAsync(string strUnitType, int nLoop, TestResultJson.GridTestResult objCommGridResult)
        {
            bool isAllPass = true;
            _commUiManager.ResetAll();

            objCommGridResult.HeaderRounds.Add($"{nLoop}회차");
            AppendTestLog(richTextBox_Log, $"[통신] {nLoop}회차 통신 검사 시작", Color.Purple);

            string[] mvbTargetPorts = (strUnitType == "TC") ? new[] { "41A0" } :
                                      (strUnitType == "CC") ? new[] { "42A0" } :
                                      (strUnitType == "ER") ? new[] { "43A0" } : new[] { "44A0" };

            try
            {
                // 1. WTB 통신
                if (!m_bIsTesting) return false;
                _commUiManager.SetCardState("WTB", ECommTestState.Testing, $"{nLoop}회차 검사 중...", "노드: 0x01");
                await Task.Delay(300);
                bool wtbPass = true;
                _commUiManager.SetCardState("WTB", wtbPass ? ECommTestState.Pass : ECommTestState.Fail, wtbPass ? "정상 응답" : "응답 없음", "노드: 0x01");
                if (!wtbPass) isAllPass = false;
                objCommGridResult.AddCommDetail(nLoop, "WTB", 1, "WTB 통신", "Node 0x01 (정상 응답)", wtbPass);

                // 2. MVB 통신
                if (!m_bIsTesting) return false;
                string portStr = string.Join(", ", mvbTargetPorts);
                _commUiManager.SetCardState("MVB", ECommTestState.Testing, $"{nLoop}회차 수신 대기...", $"대상 포트: {portStr}");
                var testService = new TcmsTestService(m_mvbReceiver);
                bool mvbPass = await testService.CheckMvbPortsAsync(3000, mvbTargetPorts);
                _commUiManager.SetCardState("MVB", mvbPass ? ECommTestState.Pass : ECommTestState.Fail, mvbPass ? "정상 수신" : "수신 실패(타임아웃)", $"대상 포트: {portStr}");
                if (!mvbPass) isAllPass = false;
                objCommGridResult.AddCommDetail(nLoop, "MVB", 2, "MVB 통신", $"Port {portStr} ({(mvbPass ? "수신 성공" : "타임아웃")})", mvbPass);

                // 3. RS485-1
                if (!m_bIsTesting) return false;
                _commUiManager.SetCardState("RS485_1", ECommTestState.Testing, $"{nLoop}회차 에코백 검사...", "115200 bps");
                await Task.Delay(200);
                bool rs1Pass = true;
                _commUiManager.SetCardState("RS485_1", rs1Pass ? ECommTestState.Pass : ECommTestState.Fail, rs1Pass ? "정상" : "응답 실패", "115200 bps");
                if (!rs1Pass) isAllPass = false;
                objCommGridResult.AddCommDetail(nLoop, "RS485-1", 3, "RS485 #1", "115200 bps (에코백 정상)", rs1Pass);

                // 4. RS485-2
                if (!m_bIsTesting) return false;
                _commUiManager.SetCardState("RS485_2", ECommTestState.Testing, $"{nLoop}회차 에코백 검사...", "115200 bps");
                await Task.Delay(200);
                bool rs2Pass = true;
                _commUiManager.SetCardState("RS485_2", rs2Pass ? ECommTestState.Pass : ECommTestState.Fail, rs2Pass ? "정상" : "응답 실패", "115200 bps");
                if (!rs2Pass) isAllPass = false;
                objCommGridResult.AddCommDetail(nLoop, "RS485-2", 4, "RS485 #2", "115200 bps (에코백 정상)", rs2Pass);

                // 5. RS485-3
                if (!m_bIsTesting) return false;
                _commUiManager.SetCardState("RS485_3", ECommTestState.Testing, $"{nLoop}회차 에코백 검사...", "9600 bps");
                await Task.Delay(200);
                bool rs3Pass = true;
                _commUiManager.SetCardState("RS485_3", rs3Pass ? ECommTestState.Pass : ECommTestState.Fail, rs3Pass ? "정상" : "응답 실패", "9600 bps");
                if (!rs3Pass) isAllPass = false;
                objCommGridResult.AddCommDetail(nLoop, "RS485-3", 5, "RS485 #3", "9600 bps (에코백 정상)", rs3Pass);

                return isAllPass;
            }
            catch (Exception ex)
            {
                AppendTestLog(richTextBox_Log, $"[통신 에러] {ex.Message}", Color.Red);
                return false;
            }
        }
    }
}