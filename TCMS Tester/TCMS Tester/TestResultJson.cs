using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CITester
{
    public class TestResultJson
    {
        // 1. 기본 정보 (ConfigJson.OperationInfo 데이터 구조 유지 및 시험 일시/결과 ID 추가)
        public class ResultHeaderInfo
        {
            public string ResultID { get; set; } = Guid.NewGuid().ToString("N"); // 결과 고유 식별자
            public string TestDateTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // 시험 수행 일시
            public string TesterName { get; set; } = string.Empty;
            public string FleetNo { get; set; } = string.Empty;       // 편성번호
            public string TrainNo { get; set; } = string.Empty;       // 차량번호
            public string SerialNo { get; set; } = string.Empty;      // 일련번호
            public string TCMSUnit { get; set; } = string.Empty;      // 유닛구분 (TC, CC, DU, ER)
            public int TotalRound { get; set; } = 1; // 총 시험 회수
            public string FinalResult { get; set; } = "미시험";         // 최종 시험 결과 (PASS / FAIL)
        }

        // 2. 단일 핀/채널 세부 결과 모델 (예: TC-DI1채널-48핀 정보)
        public class PinResultItem
        {
            public int Round { get; set; } = 1;                         // 시험 회차 (1회차, 2회차...)
            public string ChannelGroup { get; set; } = string.Empty;    // 채널 구분 (DI1, DI2, DO 등)
            public int PinNo { get; set; }                              // 핀 번호 (1~48)
            public string PinName { get; set; } = string.Empty;         // 핀 이름 또는 기능명
            public string MeasuredValue { get; set; } = string.Empty;   // 측정값/상태 (ON, ERR 등)
            public string Result { get; set; } = "합격";                 // 판정 (합격/불합격)
        }

        // 3. 단일 시험 항목 데이터 모델 (1개 DataGridView에 대응)
        public class GridTestResult
        {
            public string GridTitle { get; set; } = string.Empty; // 데이터그리드뷰 구분 (예: 입출력시험, 통신시험 등)
            public List<string> HeaderRounds { get; set; } = new List<string>(); // 가로 머리글 (1회차, 2회차...)
            public Dictionary<string, List<string>> RowData { get; set; } = new Dictionary<string, List<string>>(); // 세로 머리글(DI1, WTB 등)별 회차 측정 결과
            public List<PinResultItem> PinDetails { get; set; } = new List<PinResultItem>(); // 48핀 세부 정보 목록
        }

        // --- 상위 매핑 프로퍼티 ---
        public ResultHeaderInfo Header { get; set; } = new ResultHeaderInfo();

        // TC/CC(7개), DU(5개), ER(4개) 등 유닛별 DataGridView 결과 목록
        public List<GridTestResult> GridResults { get; set; } = new List<GridTestResult>();
    }

    public class TestResultManager
    {
        private readonly string strResultDirPath = Path.Combine(Application.StartupPath, "TestResults");

        public TestResultManager()
        {
            if (!Directory.Exists(strResultDirPath))
            {
                Directory.CreateDirectory(strResultDirPath);
            }
        }

        // DataGridView 및 핀 정보들을 수집하여 시험 결과 JSON 파일로 저장
        public bool SaveTestResult(TestResultJson resultData)
        {
            if (resultData == null || resultData.Header == null) return false;

            try
            {
                string strSafeDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string strFileName = $"Result_{strSafeDateTime}.json";
                string strFilePath = Path.Combine(strResultDirPath, strFileName);

                string strJsonContent = JsonConvert.SerializeObject(resultData, Formatting.Indented);
                File.WriteAllText(strFilePath, strJsonContent, System.Text.Encoding.UTF8);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"시험 결과 저장 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // 검색 화면(FormSearch)용: 저장된 모든 시험 결과의 기본 Header 목록만 빠르게 조회
        public List<TestResultJson.ResultHeaderInfo> LoadAllResultHeaders()
        {
            List<TestResultJson.ResultHeaderInfo> listHeaders = new List<TestResultJson.ResultHeaderInfo>();

            if (!Directory.Exists(strResultDirPath)) return listHeaders;

            try
            {
                string[] arrFiles = Directory.GetFiles(strResultDirPath, "*.json");

                foreach (string strFilePath in arrFiles)
                {
                    string strJsonContent = File.ReadAllText(strFilePath);
                    TestResultJson objResult = JsonConvert.DeserializeObject<TestResultJson>(strJsonContent);

                    if (objResult != null && objResult.Header != null)
                    {
                        listHeaders.Add(objResult.Header);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"시험 결과 목록 로드 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return listHeaders;
        }

        // 검색 데이터그리드뷰에서 선택된 특정 결과의 전체 파일(상세 핀 정보 포함)을 로드
        public TestResultJson LoadDetailResultByHeader(string strUnit, string strSerialNo, string strTestDateTime)
        {
            if (!Directory.Exists(strResultDirPath)) return null;

            try
            {
                string[] arrFiles = Directory.GetFiles(strResultDirPath, "*.json");

                foreach (string strFilePath in arrFiles)
                {
                    string strJsonContent = File.ReadAllText(strFilePath);
                    TestResultJson objResult = JsonConvert.DeserializeObject<TestResultJson>(strJsonContent);

                    if (objResult != null && objResult.Header != null)
                    {
                        if (objResult.Header.TCMSUnit == strUnit &&
                            objResult.Header.SerialNo == strSerialNo &&
                            objResult.Header.TestDateTime == strTestDateTime)
                        {
                            return objResult;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"상세 결과 파일 로드 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }
    }
}