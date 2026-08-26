using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITester
{


    //#############################################################################################
    //#############################################################################################
    //#############################################################################################
    //
    //  ConfigData class
    //      - 시험 설정 정보 
    //
    public class ConfigData
    {
        public string strTester;                // 시험자명
        public string strID;                    // 사원번호
        public string strDepartment;            // 부서명

        public string strGroupNo;
        public string strTrainNo;
        public string strSerialNo;
        public string strControlUnit;

        public bool bMeasurePowerUnit;      // 파워유니트시험
        public bool bMeasureSequenceRun;       // 시퀀스시험기동
        public bool bMeasureSequenceBrake;   // 시퀀스시험고장정지
        public bool bMeasureSequenceStop;  // 시퀀스시험중고장
        public bool bMeasureProtect;     // 보호동작시험
        public bool bMeasureConvInverter;   // 컨버터인버터시험
        public bool bMeasureSeqRunStop;     // 기동정지시퀀스시험
        public bool bMeasureMainCircuitOut; // 주회로출력시험
        public bool bMeasureGDU;            // GDU시험
        public bool bMeasureVI;            // 전압전류센서시험
        public bool bMeasureComm;           // 통신시험
        public bool bControlUnit; // 제어편성

        public double dConverter_ON_Std;
        public double dConverter_ON_Pmt;
        public double dConverter_OFF_Std;
        public double dConverter_OFF_Pmt;

        public double dInverter_ON_Std;
        public double dInverter_ON_Pmt;
        public double dInverter_OFF_Std;
        public double dInverter_OFF_Pmt;

        public double dCI_123_ON_Std;
        public double dCI_123_ON_Pmt;
        public double dCI_123_OFF_Std;
        public double dCI_123_OFF_Pmt;

        public double dCI_54_ON_Std;
        public double dCI_54_ON_Pmt;
        public double dCI_54_OFF_Std;
        public double dCI_54_OFF_Pmt;

        public double dIDU_ON_Std;
        public double dIDU_ON_Pmt;
        public double dIDU_OFF_Std;
        public double dIDU_OFF_Pmt;

        public double dP24_Unit_Std;
        public double dP24_Unit_Pmt;
        public double dN24_Unit_Std;
        public double dN24_Unit_Pmt;
        public double dP12_Unit_Std;
        public double dP12_Unit_Pmt;

        public double dBPSF_123_Std;
        public double dBPSF_123_Pmt;
        public double dACOV_123_Std;
        public double dACOV_123_Pmt;
        public double dACLV_123_Std;
        public double dACLV_123_Pmt;
        public double dVDOV_123_Std;
        public double dVDOV_123_Pmt;
        public double dVDLV_123_Std;
        public double dVDLV_123_Pmt;
        public double dISOC1_123_Std;
        public double dISOC1_123_Pmt;
        public double dISOC2_123_Std;
        public double dISOC2_123_Pmt;
        public double dMOCD_123_Std;
        public double dMOCD_123_Pmt;
        public double dPUD_123_Std;
        public double dPUD_123_Pmt;
        public double dFCDF_123_Std;
        public double dFCDF_123_Pmt;
        public double dIGOC_123_Std;
        public double dIGOC_123_Pmt;
        public double dBSD_123_Std;
        public double dBSD_123_Pmt;
        public double dIDOC_123_Std;
        public double dIDOC_123_Pmt;
        public double dZCDFP_123_Std;
        public double dZCDFP_123_Pmt;
        public double dZCDFM_123_Std;
        public double dZCDFM_123_Pmt;

        public double dBPSF_54_Std;
        public double dBPSF_54_Pmt;
        public double dACOV_54_Std;
        public double dACOV_54_Pmt;
        public double dACLV_54_Std;
        public double dACLV_54_Pmt;
        public double dISOC_54_Std;
        public double dISOC_54_Pmt;
        public double dMOCD_54_Std;
        public double dMOCD_54_Pmt;
        public double dFCOV_54_Std;
        public double dFCOV_54_Pmt;
        public double dFCLV_54_Std;
        public double dFCLV_54_Pmt;
        public double dLGD_54_Std;
        public double dLGD_54_Pmt;
        public double dBOCD_54_Std;
        public double dBOCD_54_Pmt;
        public double dPUD_54_Std;
        public double dPUD_54_Pmt;

        public ConfigData()
        {
            strTester = "";
            strID = "";
            strDepartment = "";

            strGroupNo = "";
            strTrainNo = "";
            strSerialNo = "";
            strControlUnit = "";

            bMeasurePowerUnit = true;
            bMeasureSequenceRun = true;
            bMeasureSequenceBrake = true;
            bMeasureSequenceStop = true;
            bMeasureProtect = true;
            bMeasureConvInverter = true;
            bMeasureSeqRunStop = true;
            bMeasureMainCircuitOut = true;
            bMeasureGDU = true;
            bMeasureComm = true;

            dConverter_ON_Std = 3;
            dConverter_ON_Pmt = 0.15;
            dConverter_OFF_Std = 3;
            dConverter_OFF_Pmt = 0.15;

            dInverter_ON_Std = 3;
            dInverter_ON_Pmt = 0.15;
            dInverter_OFF_Std = 3;
            dInverter_OFF_Pmt = 0.15;

            dCI_123_ON_Std = 5;
            dCI_123_ON_Pmt = 0.5;
            dCI_123_OFF_Std = 0;
            dCI_123_OFF_Pmt = 0.5;

            dCI_54_ON_Std = 15;
            dCI_54_ON_Pmt = 2.2;
            dCI_54_OFF_Std = -15;
            dCI_54_OFF_Pmt = 2.2;

            dIDU_ON_Std = 15;
            dIDU_ON_Pmt = 1.5;
            dIDU_OFF_Std = -15;
            dIDU_OFF_Pmt = -1.5;

            dP24_Unit_Std = 24;
            dP24_Unit_Pmt = 2.4;
            dN24_Unit_Std = -24;
            dN24_Unit_Pmt = -2.4;
            dP12_Unit_Std = 12;
            dP12_Unit_Pmt = 1.2;

            dBPSF_123_Std = 70.0;
            dBPSF_123_Pmt = 7.0;
            dACOV_123_Std = 30000;
            dACOV_123_Pmt = 3000;
            dACLV_123_Std = 19500;
            dACLV_123_Pmt = 1950;
            dVDOV_123_Std = 2200;
            dVDOV_123_Pmt = 220;
            dVDLV_123_Std = 1600;
            dVDLV_123_Pmt = 160;
            dISOC1_123_Std = 2300;
            dISOC1_123_Pmt = 230;
            dISOC2_123_Std = 2300;
            dISOC2_123_Pmt = 230;
            dMOCD_123_Std = 1850;
            dMOCD_123_Pmt = 185;
            dPUD_123_Std = 300;
            dPUD_123_Pmt = 30;
            dFCDF_123_Std = 350;
            dFCDF_123_Pmt = 35;
            dIGOC_123_Std = 300;
            dIGOC_123_Pmt = 30;
            dBSD_123_Std = 5.0;
            dBSD_123_Pmt = 0.5;
            dIDOC_123_Std = 3000;
            dIDOC_123_Pmt = 150;
            dZCDFP_123_Std = 61.0;
            dZCDFP_123_Pmt = 1.0;
            dZCDFM_123_Std = 59.0;
            dZCDFM_123_Pmt = 1.0;

            dBPSF_54_Std = 70.0;
            dBPSF_54_Pmt = 7.0;
            dACOV_54_Std = 30000;
            dACOV_54_Pmt = 3000;
            dACLV_54_Std = 17300;
            dACLV_54_Pmt = 1730;
            dISOC_54_Std = 1950;
            dISOC_54_Pmt = 195;
            dMOCD_54_Std = 1150;
            dMOCD_54_Pmt = 115;
            dFCOV_54_Std = 2150;
            dFCOV_54_Pmt = 215;
            dFCLV_54_Std = 1650;
            dFCLV_54_Pmt = 165;
            dLGD_54_Std = 290;
            dLGD_54_Pmt = 29;
            dBOCD_54_Std = 240;
            dBOCD_54_Pmt = 24;
            dPUD_54_Std = 300;
            dPUD_54_Pmt = 30;
        }
    }


    //#############################################################################################
    //#############################################################################################
    //#############################################################################################
    //
    //  ResultData class
    //      - 시험 결과 정보 
    //
    public class ResultData
    {
        public string strDate1;     // 화면 표시용
        public string strDate2;     // DB 등록용
        public string strDate3;     // 폴더 생성용
        public string strEndDate;   // 측정 종료시간

        public DateTime dtStart;
        public DateTime dtEnd;

        public bool bHasResult = false;
        public bool[] bMeasured = new bool[10];
        public bool[] bValue = new bool[100];
        public double[] dValue = new double[100];
        public bool[] bMeasuredDetail = new bool[100];
  

        public const int RESULT_POWER_UNIT_P24 = 0;
        public const int RESULT_POWER_UNIT_N24 = 1;
        public const int RESULT_POWER_UNIT_P12 = 2;
        public const int RESULT_COMM_MVB = 3;

        public const int RESULT_SEQUENCE_RUN = 4;
        public const int RESULT_SEQUENCE_BRAKE = 5;
        public const int RESULT_SEQUENCE_STOP = 6;

        public const int RESULT_INVERTER_GCU_H = 7;
        public const int RESULT_INVERTER_GCX_H = 8;
        public const int RESULT_INVERTER_GCV_H = 9;
        public const int RESULT_INVERTER_GCY_H = 10;
        public const int RESULT_INVERTER_GIU_H = 11;
        public const int RESULT_INVERTER_GIX_H = 12;
        public const int RESULT_INVERTER_GIV_H = 13;
        public const int RESULT_INVERTER_GIY_H = 14;
        public const int RESULT_INVERTER_GIW_H = 15;
        public const int RESULT_INVERTER_GIZ_H = 16;

        public const int RESULT_IDU_H = 17;
        public const int RESULT_IDU_L = 18;

        public const int RESULT_VDC_OVFT = 19;
        public const int RESULT_VDC_LVFT = 20;
        public const int RESULT_IIN_OCFT = 21;
        public const int RESULT_IINV_U_OCFT = 22;
        public const int RESULT_IINV_V_OCFT = 23;
        public const int RESULT_IINV_W_OCFT = 24;
        public const int RESULT_IDH_OCFT = 25;
        public const int RESULT_VDC_CHFT = 26;
        public const int RESULT_VIN_OVFT = 27;
        public const int RESULT_VIN_LVFT = 28;
        public const int RESULT_CPS_FT = 29;
        public const int RESULT_GCU_FT = 30;
        public const int RESULT_GCX_FT = 31;
        public const int RESULT_GCV_FT = 32;
        public const int RESULT_GCY_FT = 33;
        public const int RESULT_IGU_FT = 34;
        public const int RESULT_IGX_FT = 35;
        public const int RESULT_IGV_FT = 36;
        public const int RESULT_IGY_FT = 37;
        public const int RESULT_IGW_FT = 38;
        public const int RESULT_IGZ_FT = 39;
        public const int RESULT_GCH_FT = 40;
        public const int RESULT_CU_OT_FT = 41;
        public const int RESULT_CV_OT_FT = 42;
        public const int RESULT_I_OT_FT = 43;
        public const int RESULT_CH_OT_FT = 44;
        public const int RESULT_DC_LGD_FCT = 45;
        public const int RESULT_AC_LGD_FCT = 46;
        public const int RESULT_CS_OVER_FT = 47;
        public const int RESULT_VTI1_FLT = 48;
        public const int RESULT_VTI2_FLT = 49;
        public const int RESULT_PG1_FT = 50;
        public const int RESULT_PG2_FT = 51;
        public const int RESULT_CS_FS_FT = 52;
        public const int RESULT_CS_LL_FT = 53;


        public const int RESULTDATA_POWER_UNIT_P24_70V = 0;
        public const int RESULTDATA_POWER_UNIT_P24_100V = 1;
        public const int RESULTDATA_POWER_UNIT_P24_110V = 2;

        public const int RESULTDATA_POWER_UNIT_N24_70V = 3;
        public const int RESULTDATA_POWER_UNIT_N24_100V = 4;
        public const int RESULTDATA_POWER_UNIT_N24_110V = 5;

        public const int RESULTDATA_POWER_UNIT_P12_70V = 6;
        public const int RESULTDATA_POWER_UNIT_P12_100V = 7;
        public const int RESULTDATA_POWER_UNIT_P12_110V = 8;

        public const int RESULTDATA_INVERTER_GCU_H = 9;
        public const int RESULTDATA_INVERTER_GCX_H = 10;
        public const int RESULTDATA_INVERTER_GCV_H = 11;
        public const int RESULTDATA_INVERTER_GCY_H = 12;
        public const int RESULTDATA_INVERTER_GIU_H = 13;
        public const int RESULTDATA_INVERTER_GIX_H = 14;
        public const int RESULTDATA_INVERTER_GIV_H = 15;
        public const int RESULTDATA_INVERTER_GIY_H = 16;
        public const int RESULTDATA_INVERTER_GIW_H = 17;
        public const int RESULTDATA_INVERTER_GIZ_H = 18;

        public const int RESULTDATA_IDU_H = 19;
        public const int RESULTDATA_IDU_L = 20;

        public const int RESULTDATA_PROTECT_VDC_OVFT = 21;
        public const int RESULTDATA_PROTECT_VDC_LVFT = 22;
        public const int RESULTDATA_PROTECT_IIN_OCFT = 23;
        public const int RESULTDATA_PROTECT_IINV_U_OCFT = 24;
        public const int RESULTDATA_PROTECT_IINV_V_OCFT = 25;
        public const int RESULTDATA_PROTECT_IINV_W_OCFT = 26;
        public const int RESULTDATA_PROTECT_IDH_OCFT = 27;
        public const int RESULTDATA_PROTECT_VDC_CHFT = 28;
        public const int RESULTDATA_PROTECT_VIN_OVFT = 29;
        public const int RESULTDATA_PROTECT_VIN_LVFT = 30;
        public const int RESULTDATA_PROTECT_CPS_FT = 31;
        public const int RESULTDATA_PROTECT_GCU_FT = 32;
        public const int RESULTDATA_PROTECT_GCX_FT = 33;
        public const int RESULTDATA_PROTECT_GCV_FT = 34;
        public const int RESULTDATA_PROTECT_GCY_FT = 35;
        public const int RESULTDATA_PROTECT_IGU_FT = 36;
        public const int RESULTDATA_PROTECT_IGX_FT = 37;
        public const int RESULTDATA_PROTECT_IGV_FT = 38;
        public const int RESULTDATA_PROTECT_IGY_FT = 39;
        public const int RESULTDATA_PROTECT_IGW_FT = 40;
        public const int RESULTDATA_PROTECT_IGZ_FT = 41;
        public const int RESULTDATA_PROTECT_GCH_FT = 42;
        public const int RESULTDATA_PROTECT_CU_OT_FT = 43;
        public const int RESULTDATA_PROTECT_CV_OT_FT = 44;
        public const int RESULTDATA_PROTECT_I_OT_FT = 45;
        public const int RESULTDATA_PROTECT_CH_OT_FT = 46;
        public const int RESULTDATA_PROTECT_DC_LGD_FCT = 47;
        public const int RESULTDATA_PROTECT_AC_LGD_FCT = 48;
        public const int RESULTDATA_PROTECT_CS_OVER_FT = 49;
        public const int RESULTDATA_PROTECT_VTI1_FLT = 50;
        public const int RESULTDATA_PROTECT_VTI2_FLT = 51;
        public const int RESULTDATA_PROTECT_PG1_FT = 52;
        public const int RESULTDATA_PROTECT_PG2_FT = 53;
        public const int RESULTDATA_PROTECT_CS_FS_FT = 54;
        public const int RESULTDATA_PROTECT_CS_LL_FT = 55;

        public ResultData()
        {
            Reset();
        }

        public void Reset()
        {
            strDate1 = "";
            strDate2 = "";
            strDate3 = "";
            strEndDate = "";

            bHasResult = false;
            for (int i = 0; i < bMeasured.Length; ++i)
                bMeasured[i] = false;
            for (int i = 0; i < bValue.Length; ++i)
                bValue[i] = true ;
            for (int i = 0; i < dValue.Length; ++i)
                dValue[i] = 0;
        }

        public void SetStartTime()
        {
            strDate1 = DateTime.Now.ToString("yyyy년 MM월 dd일 hh시 mm분 ss초");
            strDate2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            strDate3 = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            dtStart = DateTime.Now;
        }

        public void SetEndTime()
        {
            strEndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dtEnd = DateTime.Now;
        }
    }


    //#############################################################################################
    //#############################################################################################
    //#############################################################################################
    //
    //  SerialInfo class
    //      - 시리얼 통신 정보 
    //
    public class SerialInfo
    {
        private string strPort;
        public string PORT
        {
            get { return strPort; }
            set { strPort = value; }
        }

        private int nBaudRate;
        public int BAUDRATE
        {
            get { return nBaudRate; }
            set { nBaudRate = value; }
        }

        private bool bConnected;
        public bool CONNECTED
        {
            get { return bConnected; }
            set { bConnected = value; }
        }

        private int nPacketCount;
        public int COUNT
        {
            get { return nPacketCount; }
            set { nPacketCount = value; }
        }

        public SerialInfo()
        {
            strPort = "";
            nBaudRate = 19200;
            bConnected = false;
            nPacketCount = 0;
        }

        public void ResetCount()
        {
            nPacketCount = 0;
        }

        public void IncreaseCount()
        {
            ++nPacketCount;
        }
    }
}
