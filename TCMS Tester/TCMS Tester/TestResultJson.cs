using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CITester
{
    public class TestResultJson
    {
        // 1. 기본 정보
        public class ResultHeaderInfo
        {
            public string ResultID { get; set; } = Guid.NewGuid().ToString("N");
            public string TestDateTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            public string TesterName { get; set; } = string.Empty;
            public string FleetNo { get; set; } = string.Empty;       // 편성번호
            public string TrainNo { get; set; } = string.Empty;       // 차량번호
            public string SerialNo { get; set; } = string.Empty;      // 일련번호
            public string TCMSUnit { get; set; } = string.Empty;      // 유닛구분 (TC, CC, DU, ER)
            public int TotalRound { get; set; } = 1;                  // 총 시험 회수
            public string FinalResult { get; set; } = "미시험";       // 최종 판정 (합격 / 불합격)
        }

        // 2. 단일 핀/채널 세부 결과 모델
        public class PinResultItem
        {
            public int Round { get; set; } = 1;
            public string ChannelGroup { get; set; } = string.Empty;  // DI1, DI2, COMM 등
            public int PinNo { get; set; }                            // 핀 번호 (통신은 1~5)
            public string PinName { get; set; } = string.Empty;       // 핀 이름 또는 통신명(WTB, MVB...)
            public string MeasuredValue { get; set; } = string.Empty; // 측정 상태값
            public string Result { get; set; } = "합격";              // 합격 / 불합격
        }

        // 3. 단일 시험 항목 데이터 모델
        public class GridTestResult
        {
            public string GridTitle { get; set; } = string.Empty;
            public List<string> HeaderRounds { get; set; } = new List<string>();
            public Dictionary<string, List<string>> RowData { get; set; } = new Dictionary<string, List<string>>();
            public List<PinResultItem> PinDetails { get; set; } = new List<PinResultItem>();

            /// <summary>
            /// 통신 시험 결과를 RowData 및 PinDetails에 동시 등록하는 헬퍼 메서드
            /// </summary>
            public void AddCommDetail(int round, string commKey, int index, string commName, string measuredVal, bool isPass)
            {
                string resultStr = isPass ? "합격" : "불합격";

                // RowData에 회차 요약 등록
                if (!RowData.ContainsKey(commKey))
                {
                    RowData[commKey] = new List<string>();
                }
                RowData[commKey].Add(resultStr);

                // PinDetails에 상세 등록
                PinDetails.Add(new PinResultItem
                {
                    Round = round,
                    ChannelGroup = "COMM",
                    PinNo = index,
                    PinName = commName,
                    MeasuredValue = measuredVal,
                    Result = resultStr
                });
            }
        }

        // 상위 프로퍼티
        public ResultHeaderInfo Header { get; set; } = new ResultHeaderInfo();
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
                    if (objResult?.Header != null)
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
                    if (objResult?.Header != null)
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