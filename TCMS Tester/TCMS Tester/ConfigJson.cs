using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CITester
{
    public class ConfigJson
    {
        public static ConfigJson CurrentConfig { get; set; } = new ConfigJson();

        // 운용 기본 정보 및 히스토리 관리
        public class OperationInfo
        {
            public class CombinedHistoryItem
            {
                public string strUnit { get; set; } = "TC";
                public string strSerialNo { get; set; } = string.Empty;
                public string strFleetNo { get; set; } = string.Empty;
                public string strTrainNo { get; set; } = string.Empty;
                public string strTesterName { get; set; } = string.Empty;

                // ComboBox에 자동 출력될 포맷 재정의
                public override string ToString()
                {
                    return $"{strUnit} - {strSerialNo} - {strFleetNo} - {strTrainNo} - {strTesterName}";
                }
            }
            // 최근 사용 히스토리 리스트 (최대 10개)
            public List<CombinedHistoryItem> lstCombinedHistory { get; set; } = new List<CombinedHistoryItem>();

            // 현재 선택 또는 마지막 사용 값
            public string TesterName { get; set; } = string.Empty;
            public string FleetNo { get; set; } = string.Empty;
            public string TrainNo { get; set; } = string.Empty;
            public string SerialNo { get; set; } = string.Empty;

            public string TCMSUnit { get; set; } = string.Empty;
            public bool ShowTCMSUnit { get; set; } = true;

            // 히스토리 항목 추가 및 최대 개수(기본 10개) 제한 캡슐화 메소드
            public void AddCombinedHistory(CombinedHistoryItem clsNewItem, int nMaxCount = 10)
            {
                if (clsNewItem == null || lstCombinedHistory == null) return;

                // 기존 동일 내역 제거 후 최상단 삽입 (LRU 방식)
                lstCombinedHistory.RemoveAll(clsItem =>
                    clsItem.strUnit == clsNewItem.strUnit &&
                    clsItem.strSerialNo == clsNewItem.strSerialNo &&
                    clsItem.strFleetNo == clsNewItem.strFleetNo &&
                    clsItem.strTrainNo == clsNewItem.strTrainNo &&
                    clsItem.strTesterName == clsNewItem.strTesterName);

                lstCombinedHistory.Insert(0, clsNewItem);

                while (lstCombinedHistory.Count > nMaxCount)
                {
                    lstCombinedHistory.RemoveAt(lstCombinedHistory.Count - 1);
                }
            }
        }

        // 시험 항목 설정
        public class TestEnableFlags
        {
            public bool PowerUnitTest { get; set; } = true;              // 전원유니트시험
            public bool SequenceTest1 { get; set; } = true;                // 시퀀스시험기동
            public bool SequenceTest2 { get; set; } = true;                // 시퀀스시험고장정지
            public bool SequenceTest3 { get; set; } = true;              // 시퀀스시험중고장
            public bool ProtectionTest { get; set; } = true;           // 보호동작시험
            public bool ConverterInverterTest { get; set; } = true;      // 컨버터인버터시험
            public bool StartStopSequenceTest { get; set; } = true;      // 기동정지시퀀스시험
            public bool MainCircuitTest { get; set; } = true;          // 주회로출력시험
            public bool GduTest { get; set; } = true;                    // GDU시험
            public bool VoltageCurrentTest { get; set; } = true;         // 전압전류시험
            public bool CommunicationTest { get; set; } = true;          // 통신시험
        }

        // 시험 세부항목 설정
        public class SpecItem
        {
            public string ItemName { get; set; } = string.Empty;         // 시험 세부항목
            public double Standard { get; set; } = 0.0;                    // 기준값 (Std)
            public double Permissible { get; set; } = 0.0;                 // 오차범위 (Pmt)
            public string Unit { get; set; } = string.Empty;             // 단위
        }

        // 데이터베이스 설정
        public class DatabaseConfig
        {
            public string Path { get; set; } = "CIDB.mdb";
            public string Password { get; set; } = "hzsofttr";
        }

        // 하드웨어 설정
        public class DeviceConfig
        {
            public string Plc_IPAddress { get; set; } = "192.168.1.20";

            public string Plc_COM { get; set; } = "COM1";
            public string Oscilloscope_IPAddress { get; set; } = "TCPIP0::192.168.1.10::5025::SOCKET";
            public string Oscilloscope_IDN { get; set; } = "DSOX1204A";

            public string SpeedOut_ComPort { get; set; } = "COM1";
            public int SpeedOut_BaudRate { get; set; } = 9600;
            public string SpeedOut_IDN { get; set; } = "speed";

            public string DMM_IPAddress { get; set; } = "TCPIP0::192.168.1.30::inst0::INSTR";
            public string DMM_IDN { get; set; } = "DAQ970A";

            public string ACPower_IPAddress { get; set; } = "192.168.1.23";
            public int ACPower_PortNo { get; set; } = 2268;
            public string ACPower_IDN { get; set; } = "APS-7100";

            public string DCPower_ComPort { get; set; } = "COM1";
            public int DCPower_BaudRate { get; set; } = 9600;
            public string DCPower_IDN { get; set; } = "EX-Series";

            public string PwmOut_ComPort { get; set; } = "COM102";
            public int PwmOut_BaudRate { get; set; } = 9600;

            public string CurrentOut_ComPort { get; set; } = "COM1";
            public int CurrentOut_BaudRate { get; set; } = 9600;
            public string CurrentOut_IDN { get; set; } = "current";

            public string OpticalBoard_ComPort { get; set; } = "COM1";
            public int OpticalBoard_BaudRate { get; set; } = 19200;
            public string OpticalBoard_IDN { get; set; } = "optboard-tester.0";

            public string OpticalBoard2_ComPort { get; set; } = "COM1";
            public int OpticalBoard2_BaudRate { get; set; } = 19200;
            public string OpticalBoard2_IDN { get; set; } = "optboard-tester.1";

            public string TrimmerBoard1_ComPort { get; set; } = "COM1";
            public int TrimmerBoard1_BaudRate { get; set; } = 9600;
            public string TrimmerBoard1_IDN { get; set; } = "trimmerx1 board.0";

            public string TrimmerBoard2_ComPort { get; set; } = "COM1";
            public int TrimmerBoard2_BaudRate { get; set; } = 9600;
            public string TrimmerBoard2_IDN { get; set; } = "trimmerx1 board.1";

            public string LineVoltageBoard0_ComPort { get; set; } = "COM1";
            public int LineVoltageBoard0_BaudRate { get; set; } = 9600;
            public string LineVoltageBoard0_IDN { get; set; } = "3sin.phase";

            public string MVBBoard_ComPort { get; set; } = "COM1";
            public int MVBBoard_BaudRate { get; set; } = 19200;
            public string MVBBoard_IDN { get; set; } = "MVB";

            public string DPS1_ComPort { get; set; } = "COM100";
            public string DPS2_ComPort { get; set; } = "COM101";
        }

        public OperationInfo Operation { get; set; } = new OperationInfo();
        public TestEnableFlags TestEnables { get; set; } = new TestEnableFlags();

        public DatabaseConfig Database { get; set; } = new DatabaseConfig();
        public DeviceConfig Device { get; set; } = new DeviceConfig();

        public List<SpecItem> ListSpecsCommon { get; set; } = new List<SpecItem>();
        public List<SpecItem> ListSpecs123 { get; set; } = new List<SpecItem>();
        public List<SpecItem> ListSpecs54 { get; set; } = new List<SpecItem>();
    }

    public class ConfigManager
    {
        private readonly string strJsonPath = Path.Combine(Application.StartupPath, "Config.json");

        // 환경 설정 로드 및 미존재 시 기본값 생성
        public bool LoadConfig(out ConfigJson clsConfigData)
        {
            clsConfigData = null;

            if (!File.Exists(strJsonPath))
            {
                MessageBox.Show("설정 파일이 존재하지 않아 기본 설정으로 파일을 생성합니다.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clsConfigData = CreateDefaultConfig();
                return SaveConfig(clsConfigData);
            }

            try
            {
                string strJsonContent = File.ReadAllText(strJsonPath);
                clsConfigData = JsonConvert.DeserializeObject<ConfigJson>(strJsonContent);

                if (clsConfigData != null)
                {
                    // 역직렬화 시 내장 리스트 객체 null 예외 방지 안전 조치
                    if (clsConfigData.Operation == null) clsConfigData.Operation = new ConfigJson.OperationInfo();
                    if (clsConfigData.Operation.lstCombinedHistory == null) clsConfigData.Operation.lstCombinedHistory = new List<ConfigJson.OperationInfo.CombinedHistoryItem>();

                    return true;
                }

                MessageBox.Show("설정 파일 데이터 파싱에 실패했습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception clsEx)
            {
                MessageBox.Show($"설정 로드 중 오류 발생: {clsEx.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // 설정 데이터 파일 저장
        public bool SaveConfig(ConfigJson clsConfigData)
        {
            if (clsConfigData == null) return false;

            try
            {
                string strDirectory = Path.GetDirectoryName(strJsonPath);
                if (!string.IsNullOrEmpty(strDirectory) && !Directory.Exists(strDirectory))
                {
                    Directory.CreateDirectory(strDirectory);
                }

                string strJsonContent = JsonConvert.SerializeObject(clsConfigData, Formatting.Indented);
                File.WriteAllText(strJsonPath, strJsonContent, System.Text.Encoding.UTF8);
                return true;
            }
            catch (Exception clsEx)
            {
                MessageBox.Show($"설정 저장 중 오류 발생: {clsEx.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // 초기 기본 환경설정 생성
        private ConfigJson CreateDefaultConfig()
        {
            ConfigJson clsDefaultConfig = new ConfigJson();

            // 공통 변수 초기화
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "Converter_ON", Standard = 3.0, Permissible = 0.15, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "Converter_OFF", Standard = 3.0, Permissible = 0.15, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "Inverter_ON", Standard = 3.0, Permissible = 0.15, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "Inverter_OFF", Standard = 3.0, Permissible = 0.15, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "P24_Unit", Standard = 24.0, Permissible = 2.4, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "N24_Unit", Standard = -24.0, Permissible = -2.4, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "P12_Unit", Standard = 12.0, Permissible = 1.2, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "Main_ON", Standard = 100.0, Permissible = 5.0, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "Main_OFF", Standard = -100.0, Permissible = 5.0, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "주파수", Standard = 60.0, Permissible = 0.0, Unit = "Hz" });

            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "DCPT_TEST1", Standard = 0.0, Permissible = 0.1, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "DCPT_TEST2", Standard = 1000.0, Permissible = 50.0, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "DCPT_TEST3", Standard = 2000.0, Permissible = 100.0, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "ACPT_TEST1", Standard = 0.0, Permissible = 0.1, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "ACPT_TEST2", Standard = 50.0, Permissible = 2.5, Unit = "V" });
            clsDefaultConfig.ListSpecsCommon.Add(new ConfigJson.SpecItem { ItemName = "ACPT_TEST3", Standard = 100.0, Permissible = 5.0, Unit = "V" });

            // 1,2,3단계 전동차 변수 초기화
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "컨버터/인버터(1,2,3) ON", Standard = 5.0, Permissible = 0.5, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "컨버터/인버터(1,2,3) OFF", Standard = 0.0, Permissible = 0.5, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "BPSF_123", Standard = 70.0, Permissible = 7.0, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "ACOV_123", Standard = 30000.0, Permissible = 3000.0, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "ACLV_123", Standard = 19500.0, Permissible = 1950.0, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "VDOV_123", Standard = 2200.0, Permissible = 220.0, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "VDLV_123", Standard = 1600.0, Permissible = 160.0, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "ISOC1_123", Standard = 2300.0, Permissible = 230.0, Unit = "A" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "ISOC2_123", Standard = 2300.0, Permissible = 230.0, Unit = "A" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "MOCD_123", Standard = 1850.0, Permissible = 185.0, Unit = "A" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "PUD_123", Standard = 300.0, Permissible = 30.0, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "FCDF_123", Standard = 350.0, Permissible = 35.0, Unit = "V" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "IGOC_123", Standard = 300.0, Permissible = 30.0, Unit = "A" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "BSD_123", Standard = 5.0, Permissible = 0.5, Unit = "km" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "IDOC_123", Standard = 3000.0, Permissible = 150.0, Unit = "A" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "ZCDFP_123", Standard = 61.0, Permissible = 1.0, Unit = "Hz" });
            clsDefaultConfig.ListSpecs123.Add(new ConfigJson.SpecItem { ItemName = "ZCDFM_123", Standard = 59.0, Permissible = 1.0, Unit = "Hz" });

            // 54칸 전동차 변수 초기화
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "컨버터/인버터(54) ON", Standard = 15.0, Permissible = 2.2, Unit = "V" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "컨버터/인버터(54) OFF", Standard = -15.0, Permissible = 2.2, Unit = "V" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "BPSF_54", Standard = 70.0, Permissible = 7.0, Unit = "V" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "ACOV_54", Standard = 30000.0, Permissible = 3000.0, Unit = "V" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "ACLV_54", Standard = 17300.0, Permissible = 1730.0, Unit = "V" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "ISOC_54", Standard = 1950.0, Permissible = 195.0, Unit = "A" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "MOCD_54", Standard = 1150.0, Permissible = 115.0, Unit = "A" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "FCOV_54", Standard = 2150.0, Permissible = 215.0, Unit = "V" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "FCLV_54", Standard = 1650.0, Permissible = 165.0, Unit = "V" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "LGD_54", Standard = 290.0, Permissible = 29.0, Unit = "A" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "BOCD_54", Standard = 240.0, Permissible = 24.0, Unit = "A" });
            clsDefaultConfig.ListSpecs54.Add(new ConfigJson.SpecItem { ItemName = "PUD_54", Standard = 300.0, Permissible = 30.0, Unit = "V" });

            return clsDefaultConfig;
        }
    }
}