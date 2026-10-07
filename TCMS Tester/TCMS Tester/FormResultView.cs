using System;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = System.Drawing.Color;
using Control = System.Windows.Forms.Control;
using Font = System.Drawing.Font;

namespace CITester
{
    public partial class FormResultView : Form
    {
        private readonly TestResultJson m_objTestResult = null;

        // 원본 통신 카드의 위치 보관 변수
        private int _origCommTop = -1;

        public FormResultView()
        {
            InitializeComponent();
        }

        public FormResultView(TestResultJson objTestResult) : this()
        {
            m_objTestResult = objTestResult;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED 스타일 (깜빡임 방지)
                return cp;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            DisplayConfig();
        }

        private void DisplayConfig()
        {
            var objHeaderInfo = m_objTestResult?.Header;

            if (objHeaderInfo == null)
            {
                Label_Unit.Text = string.Empty;
                Label_Fleet.Text = string.Empty;
                Label_Train.Text = string.Empty;
                Label_Tester.Text = string.Empty;
                Label_Serial.Text = string.Empty;
                Label_Round.Text = "-";
                Label_FinalResult.Text = "-";
                TestResult_IO.Text = "-";
                TestResult_Comn.Text = "-";
                TestResult_Memory.Text = "-";
                Label_Date.Text = DateTime.Now.ToString("yyyy년 MM월 dd일");
                return;
            }

            string strUnitType = objHeaderInfo.TCMSUnit ?? string.Empty;
            bool isDu = strUnitType.Equals("DU", StringComparison.OrdinalIgnoreCase);
            bool isEr = strUnitType.Equals("ER", StringComparison.OrdinalIgnoreCase);

            Label_Unit.Text = strUnitType;
            Label_Fleet.Text = objHeaderInfo.FleetNo ?? string.Empty;
            Label_Train.Text = objHeaderInfo.TrainNo ?? string.Empty;
            Label_Tester.Text = objHeaderInfo.TesterName ?? string.Empty;
            Label_Serial.Text = objHeaderInfo.SerialNo ?? string.Empty;
            Label_Round.Text = $"{objHeaderInfo.TotalRound} 회";
            Label_FinalResult.Text = objHeaderInfo.FinalResult ?? string.Empty;

            // 최종 판정 라벨 색상 설정
            Label_FinalResult.ForeColor = (objHeaderInfo.FinalResult == "합격") ? Color.Blue : Color.Red;

            // DU는 입출력/메모리 시험이 없으므로 '해당없음' 처리 (ER은 입출력/통신/메모리 모두 포함)
            string strIoResult = isDu ? "해당없음" : GetIoCategoryResult(m_objTestResult);
            string strComnResult = GetGenericCategoryResult(m_objTestResult, "통신");
            string strMemResult = isDu ? "해당없음" : GetGenericCategoryResult(m_objTestResult, "메모리");

            SetResultLabelStyle(TestResult_IO, strIoResult);
            SetResultLabelStyle(TestResult_Comn, strComnResult);
            SetResultLabelStyle(TestResult_Memory, strMemResult);

            if (DateTime.TryParse(objHeaderInfo.TestDateTime, out DateTime dtTestDate))
            {
                Label_Date.Text = dtTestDate.ToString("yyyy년 MM월 dd일");
            }
            else
            {
                Label_Date.Text = objHeaderInfo.TestDateTime;
            }

            DisplayFailedLog(richTextBox_Err, m_objTestResult);

            // 좌측 패널 간섭 없이 우측 카드만 안전하게 재배치
            AdjustLayoutForUnit(isDu);

            InitTestDataGridViews();
        }

        private void AdjustLayoutForUnit(bool isDu)
        {
            Control cardDio = dataGridViewDIO?.Parent;
            Control cardComm = dataGridViewComm?.Parent;
            Control cardMem = dataGridViewMemory?.Parent;

            if (cardDio == null || cardComm == null || cardMem == null) return;

            if (_origCommTop == -1)
            {
                _origCommTop = cardComm.Top;
            }

            if (isDu)
            {
                cardDio.Visible = false;
                cardMem.Visible = false;
                cardComm.Top = cardDio.Top;
                cardComm.Visible = true;
            }
            else
            {
                // TC, CC 및 ER은 3개 카드(DIO, 통신, 메모리) 모두 표출
                cardDio.Visible = true;
                cardMem.Visible = true;
                cardComm.Top = _origCommTop;
                cardComm.Visible = true;
            }
        }

        private string GetIoCategoryResult(TestResultJson objTestResult)
        {
            if (objTestResult?.GridResults == null) return "-";

            bool bHasIoData = false;

            foreach (var objGrid in objTestResult.GridResults)
            {
                if (objGrid.GridTitle.Contains("디지털") || objGrid.GridTitle.Contains("아날로그") || objGrid.GridTitle.Contains("입출력"))
                {
                    if (objGrid.PinDetails != null && objGrid.PinDetails.Count > 0)
                    {
                        bHasIoData = true;
                        if (objGrid.PinDetails.Exists(p => p.Result == "불합격" || p.MeasuredValue == "ERR" || p.Result == "FAIL"))
                        {
                            return "불합격";
                        }
                    }

                    if (objGrid.RowData != null && objGrid.RowData.Count > 0)
                    {
                        bHasIoData = true;
                        foreach (var pair in objGrid.RowData)
                        {
                            if (pair.Value != null && pair.Value.Exists(v => v == "불합격" || v == "FAIL" || v == "ERR"))
                            {
                                return "불합격";
                            }
                        }
                    }
                }
            }

            return bHasIoData ? "합격" : "미시험";
        }

        private string GetGenericCategoryResult(TestResultJson objTestResult, string strKeyword)
        {
            if (objTestResult?.GridResults == null) return "-";

            foreach (var objGrid in objTestResult.GridResults)
            {
                if (objGrid.GridTitle.IndexOf(strKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (objGrid.RowData != null && objGrid.RowData.Count > 0)
                    {
                        foreach (var pairRow in objGrid.RowData)
                        {
                            if (pairRow.Value != null && pairRow.Value.Exists(strVal => strVal == "불합격" || strVal == "FAIL" || strVal == "ERR"))
                            {
                                return "불합격";
                            }
                        }
                        return "합격";
                    }

                    if (objGrid.PinDetails != null && objGrid.PinDetails.Count > 0)
                    {
                        if (objGrid.PinDetails.Exists(p => p.Result == "불합격" || p.MeasuredValue == "ERR" || p.Result == "FAIL"))
                        {
                            return "불합격";
                        }
                        return "합격";
                    }
                }
            }

            return "미시험";
        }

        private void SetResultLabelStyle(CustomIconButton lblTarget, string strResult)
        {
            if (lblTarget == null) return;

            lblTarget.Text = string.IsNullOrWhiteSpace(strResult) ? "-" : strResult;

            switch (lblTarget.Text)
            {
                case "합격":
                    lblTarget.BackColor = Color.FromArgb(229, 245, 230);
                    lblTarget.ForeColor = Color.FromArgb(14, 93, 24);
                    break;

                case "불합격":
                    lblTarget.BackColor = Color.FromArgb(255, 205, 205);
                    lblTarget.ForeColor = Color.FromArgb(180, 0, 0);
                    break;

                case "해당없음":
                case "미시험":
                default:
                    lblTarget.BackColor = Color.FromArgb(230, 230, 230);
                    lblTarget.ForeColor = Color.FromArgb(100, 100, 100);
                    break;
            }
        }

        private void DisplayFailedLog(RichTextBox rtbTarget, TestResultJson objTestResult)
        {
            if (rtbTarget == null) return;

            rtbTarget.SuspendLayout();
            rtbTarget.Clear();

            try
            {
                if (objTestResult?.GridResults == null || objTestResult.GridResults.Count == 0) return;

                var listFailLogs = new HashSet<string>();

                foreach (var objGrid in objTestResult.GridResults)
                {
                    string strGridTitle = objGrid.GridTitle;

                    if (objGrid.PinDetails != null && objGrid.PinDetails.Count > 0)
                    {
                        var listFails = objGrid.PinDetails.FindAll(p => p.Result == "불합격" || p.MeasuredValue == "ERR" || p.Result == "FAIL");
                        foreach (var fail in listFails)
                        {
                            listFailLogs.Add($"[{strGridTitle}] {fail.Round}회차 - {fail.PinName} - 불합격");
                        }
                    }

                    if (objGrid.RowData != null && objGrid.RowData.Count > 0)
                    {
                        foreach (var pair in objGrid.RowData)
                        {
                            string itemKey = pair.Key;
                            for (int i = 0; i < pair.Value.Count; i++)
                            {
                                string val = pair.Value[i];
                                if (val == "불합격" || val == "FAIL" || val == "ERR")
                                {
                                    listFailLogs.Add($"[{strGridTitle}] {i + 1}회차 - {itemKey} - 불합격");
                                }
                            }
                        }
                    }
                }

                if (listFailLogs.Count > 0)
                {
                    foreach (string log in listFailLogs)
                    {
                        rtbTarget.SelectionColor = Color.Red;
                        rtbTarget.AppendText(log + "\n");
                    }
                    imagebtn2.Visible = false;
                }
                else
                {
                    rtbTarget.SelectionColor = Color.Green;
                    rtbTarget.AppendText("모든 회차 시험 항목이 정상(합격)입니다.\n");
                    imagebtn2.Visible = true;
                }
            }
            finally
            {
                rtbTarget.ResumeLayout();
            }
        }

        // =========================================================================
        // 그리드 3종 초기화 (TC, CC, DU, ER 4대 유닛별 전용 행 동적 생성)
        // =========================================================================
        private void InitTestDataGridViews()
        {
            int nMaxRoundCount = GetMaxRoundCount(m_objTestResult);
            string strUnit = m_objTestResult?.Header?.TCMSUnit ?? "CC";
            bool isCc = strUnit.Equals("CC", StringComparison.OrdinalIgnoreCase);
            bool isDu = strUnit.Equals("DU", StringComparison.OrdinalIgnoreCase);
            bool isEr = strUnit.Equals("ER", StringComparison.OrdinalIgnoreCase);

            // 1. 입·출력 시험 그리드 (DU: 없음 / ER: DI1, DO / CC: DI1, DI2, DO, 아날로그 / TC: DI1, DI2, DI3, DO, 아날로그)
            string[] arrDioRows;
            if (isDu)
            {
                arrDioRows = Array.Empty<string>();
            }
            else if (isEr)
            {
                arrDioRows = new string[] { "DI1", "DO" };
            }
            else if (isCc)
            {
                arrDioRows = new string[] { "DI1", "DI2", "DO", "아날로그 입력", "아날로그 출력" };
            }
            else
            {
                arrDioRows = new string[] { "DI1", "DI2", "DI3", "DO", "아날로그 입력", "아날로그 출력" };
            }

            SetupTestGrid(dataGridViewDIO, arrDioRows, nMaxRoundCount, (strTitle, nRound) => GetDioRoundResult(m_objTestResult, strTitle, nRound));

            // 2. 통신 시험 그리드 ( DU: RS-485 제외하고 MVB만 단독 표출 / ER: LAN, PROBE, MVB, CPM, USB / TC, CC: 5종)
            string[] arrCommRows;
            if (isDu)
            {
                arrCommRows = new string[] { "MVB" }; //  RS-485 제거
            }
            else if (isEr)
            {
                arrCommRows = new string[] { "LAN", "PROBE", "MVB", "CPM", "USB" };
            }
            else
            {
                arrCommRows = new string[] { "WTB", "MVB", "RS485-1", "RS485-2", "RS485-3" };
            }

            SetupTestGrid(dataGridViewComm, arrCommRows, nMaxRoundCount, (strTitle, nRound) => GetGenericRoundResult(m_objTestResult, "통신", strTitle, nRound));

            // 3. 메모리 시험 그리드 (DU: 없음 / ER: FLASH, SDRAM, FRAM, RTC, HRS / TC, CC: VCPUT 8종)
            string[] arrMemoryRows;
            if (isDu)
            {
                arrMemoryRows = Array.Empty<string>();
            }
            else if (isEr)
            {
                arrMemoryRows = new string[] { "FLASH", "SDRAM", "FRAM", "RTC", "HRS" };
            }
            else
            {
                arrMemoryRows = new string[] { "DPRAM", "SDRAM", "MRAM", "FLASH", "EMMC", "USB", "ENET_1", "ENET_2" };
            }

            SetupTestGrid(dataGridViewMemory, arrMemoryRows, nMaxRoundCount, (strTitle, nRound) => GetGenericRoundResult(m_objTestResult, "메모리", strTitle, nRound));
        }

        private void SetupTestGrid(DataGridView dgvTarget, string[] arrRowHeaderTitles, int nRoundCount, Func<string, int, string> fnGetResultText)
        {
            if (dgvTarget == null) return;

            dgvTarget.SuspendLayout();

            try
            {
                dgvTarget.Columns.Clear();
                dgvTarget.Rows.Clear();

                dgvTarget.AllowUserToAddRows = false;
                dgvTarget.AllowUserToDeleteRows = false;
                dgvTarget.AllowUserToResizeColumns = false;
                dgvTarget.AllowUserToResizeRows = false;
                dgvTarget.ReadOnly = true;
                dgvTarget.EnableHeadersVisualStyles = false;

                Color clrSkyBlueBg = Color.FromArgb(248, 250, 254);
                Color clrDarkBlueText = Color.FromArgb(20, 50, 90);
                Color clrCellBg = Color.White;
                Color clrCellText = Color.Black;

                dgvTarget.DefaultCellStyle.BackColor = clrCellBg;
                dgvTarget.DefaultCellStyle.ForeColor = clrCellText;
                dgvTarget.DefaultCellStyle.SelectionBackColor = clrCellBg;
                dgvTarget.DefaultCellStyle.SelectionForeColor = clrCellText;

                dgvTarget.ColumnHeadersDefaultCellStyle.BackColor = clrSkyBlueBg;
                dgvTarget.ColumnHeadersDefaultCellStyle.ForeColor = clrDarkBlueText;
                dgvTarget.ColumnHeadersDefaultCellStyle.SelectionBackColor = clrSkyBlueBg;
                dgvTarget.ColumnHeadersDefaultCellStyle.SelectionForeColor = clrDarkBlueText;

                dgvTarget.RowHeadersDefaultCellStyle.BackColor = clrSkyBlueBg;
                dgvTarget.RowHeadersDefaultCellStyle.ForeColor = clrDarkBlueText;
                dgvTarget.RowHeadersDefaultCellStyle.SelectionBackColor = clrSkyBlueBg;
                dgvTarget.RowHeadersDefaultCellStyle.SelectionForeColor = clrDarkBlueText;

                dgvTarget.TopLeftHeaderCell.Value = "항목";
                dgvTarget.TopLeftHeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvTarget.TopLeftHeaderCell.Style.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);

                dgvTarget.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvTarget.MultiSelect = false;
                dgvTarget.RowHeadersVisible = true;
                dgvTarget.RowHeadersWidth = 140;
                dgvTarget.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

                dgvTarget.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvTarget.ColumnHeadersHeight = 35;

                Font fntRegular = new Font("맑은 고딕", 9.5F, FontStyle.Regular);
                Font fntBold = new Font("맑은 고딕", 10F, FontStyle.Bold);

                dgvTarget.DefaultCellStyle.Font = fntRegular;
                dgvTarget.ColumnHeadersDefaultCellStyle.Font = fntBold;
                dgvTarget.RowHeadersDefaultCellStyle.Font = fntBold;

                dgvTarget.CellPainting -= DgvTarget_CellPainting;
                dgvTarget.CellPainting += DgvTarget_CellPainting;

                for (int nIndex = 1; nIndex <= nRoundCount; nIndex++)
                {
                    int nColIndex = dgvTarget.Columns.Add($"colRound{nIndex}", $"{nIndex}회차");
                    dgvTarget.Columns[nColIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvTarget.Columns[nColIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (arrRowHeaderTitles != null && arrRowHeaderTitles.Length > 0)
                {
                    foreach (string strRowTitle in arrRowHeaderTitles)
                    {
                        int nRowIndex = dgvTarget.Rows.Add();
                        dgvTarget.Rows[nRowIndex].HeaderCell.Value = strRowTitle;

                        for (int nColIdx = 0; nColIdx < nRoundCount; nColIdx++)
                        {
                            int nRoundNum = nColIdx + 1;
                            string strResultValue = fnGetResultText != null ? fnGetResultText(strRowTitle, nRoundNum) : "-";
                            var cell = dgvTarget.Rows[nRowIndex].Cells[nColIdx];
                            cell.Value = strResultValue;

                            cell.Style.SelectionBackColor = clrCellBg;

                            if (strResultValue.Contains("불합격") || strResultValue.Contains("FAIL") || strResultValue.Contains("ERR"))
                            {
                                cell.Style.ForeColor = Color.Red;
                                cell.Style.SelectionForeColor = Color.Red;
                                cell.Style.Font = fntBold;
                            }
                            else if (strResultValue.Contains("합격") || strResultValue.Contains("PASS"))
                            {
                                cell.Style.ForeColor = Color.Blue;
                                cell.Style.SelectionForeColor = Color.Blue;
                                cell.Style.Font = fntBold;
                            }
                            else
                            {
                                cell.Style.ForeColor = Color.Gray;
                                cell.Style.SelectionForeColor = Color.Gray;
                                cell.Style.Font = fntRegular;
                            }
                        }
                    }

                    Action actAdjustRowHeights = () =>
                    {
                        dgvTarget.ScrollBars = ScrollBars.None;
                        int nAvailableHeight = dgvTarget.ClientSize.Height - dgvTarget.ColumnHeadersHeight;
                        if (nAvailableHeight > 0 && dgvTarget.Rows.Count > 0)
                        {
                            int nCalculatedHeight = nAvailableHeight / dgvTarget.Rows.Count;
                            foreach (DataGridViewRow objRow in dgvTarget.Rows)
                            {
                                objRow.Height = Math.Max(nCalculatedHeight, 22);
                            }
                        }
                    };

                    actAdjustRowHeights();

                    EventHandler actResizeHandler = (s, e) => { actAdjustRowHeights(); };
                    if (dgvTarget.Tag is EventHandler objOldHandler)
                    {
                        dgvTarget.Resize -= objOldHandler;
                    }
                    dgvTarget.Resize += actResizeHandler;
                    dgvTarget.Tag = actResizeHandler;
                }

                dgvTarget.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvTarget.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvTarget.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvTarget.ClearSelection();
                dgvTarget.CurrentCell = null;
            }
            finally
            {
                dgvTarget.ResumeLayout();
            }
        }

        private void DgvTarget_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == -1)
            {
                var dgv = sender as DataGridView;
                if (dgv == null) return;

                e.PaintBackground(e.CellBounds, true);

                string strHeaderText = dgv.Rows[e.RowIndex].HeaderCell.Value?.ToString() ?? string.Empty;
                TextRenderer.DrawText(
                    e.Graphics,
                    strHeaderText,
                    dgv.RowHeadersDefaultCellStyle.Font,
                    e.CellBounds,
                    dgv.RowHeadersDefaultCellStyle.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
                );

                e.Handled = true;
            }
        }

        private int GetMaxRoundCount(TestResultJson objTestResult)
        {
            if (objTestResult?.Header != null && objTestResult.Header.TotalRound > 0)
            {
                return objTestResult.Header.TotalRound;
            }
            return 1;
        }

        private string GetDioRoundResult(TestResultJson objTestResult, string strTargetGroup, int nRound)
        {
            if (objTestResult?.GridResults == null) return "-";

            string unit = objTestResult.Header?.TCMSUnit ?? "CC";
            if (unit.Equals("CC", StringComparison.OrdinalIgnoreCase) && strTargetGroup == "DI3")
            {
                return "해당없음";
            }

            foreach (var objGrid in objTestResult.GridResults)
            {
                if (objGrid.PinDetails == null) continue;

                List<TestResultJson.PinResultItem> targetPins = null;

                if (strTargetGroup == "아날로그 입력" || strTargetGroup == "AI")
                {
                    targetPins = objGrid.PinDetails.FindAll(p => p.Round == nRound && p.ChannelGroup == "ANALOG" && p.PinName.Contains("입력"));
                }
                else if (strTargetGroup == "아날로그 출력" || strTargetGroup == "AO")
                {
                    targetPins = objGrid.PinDetails.FindAll(p => p.Round == nRound && p.ChannelGroup == "ANALOG" && p.PinName.Contains("출력"));
                }
                else
                {
                    targetPins = objGrid.PinDetails.FindAll(p => p.Round == nRound && p.ChannelGroup.Equals(strTargetGroup, StringComparison.OrdinalIgnoreCase));
                }

                if (targetPins != null && targetPins.Count > 0)
                {
                    int nTotalCount = targetPins.Count;
                    int nPassCount = targetPins.Count(p => p.Result == "합격" || p.MeasuredValue == "PASS");

                    if (nPassCount == nTotalCount)
                    {
                        return $"{nPassCount}/{nTotalCount} (합격)";
                    }
                    else
                    {
                        return $"{nPassCount}/{nTotalCount} (불합격)";
                    }
                }
            }

            return "-";
        }

        private string GetGenericRoundResult(TestResultJson objTestResult, string strGridKeyword, string strRowKey, int nRound)
        {
            if (objTestResult?.GridResults == null) return "-";

            foreach (var objGrid in objTestResult.GridResults)
            {
                if (objGrid.GridTitle.IndexOf(strGridKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (objGrid.RowData != null && objGrid.RowData.Count > 0)
                    {
                        string targetNorm = strRowKey.Replace("-", "").Replace("_", "").Replace(" ", "").ToUpper();

                        var matched = objGrid.RowData.FirstOrDefault(k =>
                        {
                            string keyNorm = k.Key.Replace("-", "").Replace("_", "").Replace(" ", "").ToUpper();
                            return keyNorm == targetNorm || keyNorm.StartsWith(targetNorm) || targetNorm.StartsWith(keyNorm);
                        });

                        if (matched.Key != null && matched.Value != null)
                        {
                            int idx = nRound - 1;
                            if (idx >= 0 && idx < matched.Value.Count)
                            {
                                return matched.Value[idx];
                            }
                        }
                    }

                    if (objGrid.PinDetails != null && objGrid.PinDetails.Count > 0)
                    {
                        var pin = objGrid.PinDetails.FirstOrDefault(p => p.Round == nRound &&
                            (p.PinName.IndexOf(strRowKey, StringComparison.OrdinalIgnoreCase) >= 0 ||
                             strRowKey.IndexOf(p.PinName, StringComparison.OrdinalIgnoreCase) >= 0));

                        if (pin != null) return pin.Result;
                    }
                }
            }

            return "-";
        }

        // =========================================================================
        // PDF 결과 보고서 생성 및 인쇄 (TC, CC, DU, ER 4대 유닛별 전용 섹션 자동 분기)
        // =========================================================================
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            DialogResult drSelect = MessageBox.Show(
                "시험 결과를 PDF 보고서로 인쇄하시겠습니까?",
                "인쇄 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (drSelect != DialogResult.Yes) return;

            string strDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string strTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string strFilePath = Path.Combine(strDesktopPath, $"TCMS_시험결과보고서_{strTimeStamp}.pdf");

            if (File.Exists(strFilePath))
            {
                try { File.Delete(strFilePath); }
                catch (IOException)
                {
                    MessageBox.Show("기존 보고서 PDF 파일이 열려 있습니다. 닫은 후 다시 시도해 주세요.", "파일 잠김", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string strUnitType = m_objTestResult?.Header?.TCMSUnit ?? "CC";
            bool isDu = strUnitType.Equals("DU", StringComparison.OrdinalIgnoreCase);
            bool isEr = strUnitType.Equals("ER", StringComparison.OrdinalIgnoreCase);

            string strSerialNo = m_objTestResult?.Header?.SerialNo ?? "0000";
            string strCarNo = m_objTestResult?.Header?.FleetNo ?? "0000";
            string strTrainNo = m_objTestResult?.Header?.TrainNo ?? "0000";
            string strTester = m_objTestResult?.Header?.TesterName ?? "Tester";
            string strFinalDecision = m_objTestResult?.Header?.FinalResult ?? "미시험";

            int nMaxRoundCount = GetMaxRoundCount(m_objTestResult);

            //  가변 회차 테이블 헤더 행 구성 {"Header", "시험 항목", "1회차", "2회차", ...}
            List<string> listHeaderTokens = new List<string> { "Header", "시험 항목" };
            for (int r = 1; r <= nMaxRoundCount; r++)
            {
                listHeaderTokens.Add($"{r}회차");
            }
            string[] arrDynamicHeader = listHeaderTokens.ToArray();

            List<string[]> listItems = new List<string[]>();

            // -------------------------------------------------------------
            //  유닛 종류(DU / ER / TC·CC)에 따른 전용 PDF 보고서 구조 생성
            // -------------------------------------------------------------
            if (isDu)
            {
                //  DU 보고서 제목 및 항목 수정: RS-485 행을 제거하고 MVB만 단독 출력
                listItems.Add(new string[] { "Section", "1. DU 통신 시험 (MVB)" });
                listItems.Add(arrDynamicHeader);

                // MVB 행
                List<string> rowMvb = new List<string> { "Row", "MVB 통신" };
                for (int r = 1; r <= nMaxRoundCount; r++)
                {
                    rowMvb.Add(GetGenericRoundResult(m_objTestResult, "통신", "MVB", r));
                }
                listItems.Add(rowMvb.ToArray());

                listItems.Add(new string[] { "Spacing", "15" });
            }
            else if (isEr)
            {
                //  [ER 전용 보고서] 8DI/2DO, 통신 5종, 메모리 5종
                // 1. ER 디지털 입·출력 시험
                listItems.Add(new string[] { "Section", "1. ER 디지털 입출력 시험 (8DI / 2DO)" });
                listItems.Add(arrDynamicHeader);

                string[] arrErDiDoKeys = new string[] { "DI1", "DO" };
                string[] arrErDiDoTitles = new string[] { "디지털 입력 (DI 8핀)", "디지털 출력 (DO 2핀)" };

                for (int i = 0; i < arrErDiDoKeys.Length; i++)
                {
                    List<string> row = new List<string> { "Row", arrErDiDoTitles[i] };
                    for (int r = 1; r <= nMaxRoundCount; r++)
                    {
                        row.Add(GetDioRoundResult(m_objTestResult, arrErDiDoKeys[i], r));
                    }
                    listItems.Add(row.ToArray());
                }
                listItems.Add(new string[] { "Spacing", "15" });

                // 2. ER 통신 시험 (5종)
                listItems.Add(new string[] { "Section", "2. ER 통신 시험 (5종)" });
                listItems.Add(arrDynamicHeader);

                string[] arrErCommItems = new string[] { "Ethernet LAN", "Target Probe", "MVB 통신", "CPM 통신", "USB 인터페이스" };
                string[] arrErCommKeys = new string[] { "LAN", "PROBE", "MVB", "CPM", "USB" };
                for (int i = 0; i < arrErCommItems.Length; i++)
                {
                    List<string> row = new List<string> { "Row", arrErCommItems[i] };
                    for (int r = 1; r <= nMaxRoundCount; r++)
                    {
                        row.Add(GetGenericRoundResult(m_objTestResult, "통신", arrErCommKeys[i], r));
                    }
                    listItems.Add(row.ToArray());
                }
                listItems.Add(new string[] { "Spacing", "15" });

                // 3. ER 메모리 및 주변장치 시험 (5종)
                listItems.Add(new string[] { "Section", "3. ER 메모리 시험 (5종)" });
                listItems.Add(arrDynamicHeader);

                string[] arrErMemItems = new string[] { "Flash Memory", "SDRAM", "FRAM", "RTC 시각 레지스터", "HRS 로터리 스위치" };
                string[] arrErMemKeys = new string[] { "FLASH", "SDRAM", "FRAM", "RTC", "HRS" };
                for (int i = 0; i < arrErMemItems.Length; i++)
                {
                    List<string> row = new List<string> { "Row", arrErMemItems[i] };
                    for (int r = 1; r <= nMaxRoundCount; r++)
                    {
                        row.Add(GetGenericRoundResult(m_objTestResult, "메모리", arrErMemKeys[i], r));
                    }
                    listItems.Add(row.ToArray());
                }
                listItems.Add(new string[] { "Spacing", "15" });
            }
            else
            {
                // TC / CC 기존 3대 대분류 보고서 체계 유지
                // 1. 입·출력 시험
                listItems.Add(new string[] { "Section", "1. 입·출력 시험" });
                listItems.Add(new string[] { "SubSection", "1.1 디지털 입출력 (DI / DO)" });
                listItems.Add(arrDynamicHeader);

                string[] arrDiDoKeys = strUnitType.Equals("TC", StringComparison.OrdinalIgnoreCase)
                    ? new string[] { "DI1", "DI2", "DI3", "DO" }
                    : new string[] { "DI1", "DI2", "DO" };

                string[] arrDiDoTitles = strUnitType.Equals("TC", StringComparison.OrdinalIgnoreCase)
                    ? new string[] { "디지털 입력 1 (DI 1)", "디지털 입력 2 (DI 2)", "디지털 입력 3 (DI 3)", "디지털 출력 (DO)" }
                    : new string[] { "디지털 입력 1 (DI 1)", "디지털 입력 2 (DI 2)", "디지털 출력 (DO)" };

                for (int i = 0; i < arrDiDoKeys.Length; i++)
                {
                    List<string> row = new List<string> { "Row", arrDiDoTitles[i] };
                    for (int r = 1; r <= nMaxRoundCount; r++)
                    {
                        row.Add(GetDioRoundResult(m_objTestResult, arrDiDoKeys[i], r));
                    }
                    listItems.Add(row.ToArray());
                }
                listItems.Add(new string[] { "Spacing", "10" });

                // 1.2 아날로그
                listItems.Add(new string[] { "SubSection", "1.2 아날로그 입출력 (AI / AO)" });
                listItems.Add(arrDynamicHeader);

                string[] arrAiAoKeys = new string[] { "아날로그 입력", "아날로그 출력" };
                string[] arrAiAoTitles = new string[] { "아날로그 입력 (AI)", "아날로그 출력 (AO)" };
                for (int i = 0; i < arrAiAoKeys.Length; i++)
                {
                    List<string> row = new List<string> { "Row", arrAiAoTitles[i] };
                    for (int r = 1; r <= nMaxRoundCount; r++)
                    {
                        row.Add(GetDioRoundResult(m_objTestResult, arrAiAoKeys[i], r));
                    }
                    listItems.Add(row.ToArray());
                }
                listItems.Add(new string[] { "Spacing", "15" });

                // 2. 통신 시험
                listItems.Add(new string[] { "Section", "2. 통신 시험" });
                listItems.Add(arrDynamicHeader);

                string[] arrCommItems = new string[] { "WTB 통신", "MVB 통신", "RS-485 #1", "RS-485 #2", "RS-485 #3" };
                string[] arrCommKeys = new string[] { "WTB", "MVB", "RS485-1", "RS485-2", "RS485-3" };
                for (int i = 0; i < arrCommItems.Length; i++)
                {
                    List<string> row = new List<string> { "Row", arrCommItems[i] };
                    for (int r = 1; r <= nMaxRoundCount; r++)
                    {
                        row.Add(GetGenericRoundResult(m_objTestResult, "통신", arrCommKeys[i], r));
                    }
                    listItems.Add(row.ToArray());
                }
                listItems.Add(new string[] { "Spacing", "15" });

                // 3. 메모리 및 이더넷 시험
                listItems.Add(new string[] { "Section", "3. 메모리 및 이더넷 시험" });
                listItems.Add(arrDynamicHeader);

                string[] arrMemItems = new string[] { "DPRAM", "SDRAM", "MRAM", "FLASH", "eMMC", "USB 메모리", "이더넷 포트 1 (ENET_1)", "이더넷 포트 2 (ENET_2)" };
                string[] arrMemKeys = new string[] { "DPRAM", "SDRAM", "MRAM", "FLASH", "EMMC", "USB", "ENET_1", "ENET_2" };
                for (int i = 0; i < arrMemItems.Length; i++)
                {
                    List<string> row = new List<string> { "Row", arrMemItems[i] };
                    for (int r = 1; r <= nMaxRoundCount; r++)
                    {
                        row.Add(GetGenericRoundResult(m_objTestResult, "메모리", arrMemKeys[i], r));
                    }
                    listItems.Add(row.ToArray());
                }
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
                TopMost = true,
                BackColor = Color.White
            };

            Label lblStatusMessage = new Label
            {
                Text = "PDF 문서를 생성하고 있습니다...",
                Location = new Point(25, 20),
                Size = new Size(300, 23),
                Font = new Font("맑은 고딕", 9, FontStyle.Regular)
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
                    prtDoc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(40, 40, 40, 40);
                    prtDoc.PrintController = new System.Drawing.Printing.StandardPrintController();

                    prtDoc.PrintPage += (object prtSender, System.Drawing.Printing.PrintPageEventArgs ePage) =>
                    {
                        Graphics gtxCanvas = ePage.Graphics;
                        gtxCanvas.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        Font fntTitle = new Font("맑은 고딕", 18, FontStyle.Bold);
                        Font fntSection = new Font("맑은 고딕", 10.5f, FontStyle.Bold);
                        Font fntSubSection = new Font("맑은 고딕", 9.5f, FontStyle.Bold);
                        Font fntHeader = new Font("맑은 고딕", 9, FontStyle.Bold);
                        Font fntBody = new Font("맑은 고딕", 8.5f, FontStyle.Regular);
                        Font fntBodyBold = new Font("맑은 고딕", 8.5f, FontStyle.Bold);

                        float fStartX = ePage.MarginBounds.Left;
                        float fCurrentY = ePage.MarginBounds.Top;
                        float fPageWidth = ePage.MarginBounds.Width;

                        if (nPageIndex == 1)
                        {
                            string strTitleText = isDu ? "DU 표시기 시험 결과 보고서" : (isEr ? "ER 기록기 시험 결과 보고서" : "TCMS 시험기 결과 보고서");
                            SizeF szTitle = gtxCanvas.MeasureString(strTitleText, fntTitle);
                            gtxCanvas.DrawString(strTitleText, fntTitle, Brushes.Black, fStartX + (fPageWidth - szTitle.Width) / 2, fCurrentY);
                            fCurrentY += szTitle.Height + 15f;

                            string[,] arrInfoMatrix = new string[4, 3] {
                                { "시험일자", "시험자명", "최종 판정 결과" },
                                { DateTime.Now.ToString("yyyy-MM-dd"), strTester, strFinalDecision },
                                { "편성번호", "차량번호", "유닛종류 (일련번호)" },
                                { strTrainNo, strCarNo, $"{strUnitType} ({strSerialNo})" }
                            };

                            int nInfoRowHeight = 24;
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
                                            gtxCanvas.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), rectTarget);
                                        }
                                        gtxCanvas.DrawRectangle(Pens.DarkGray, rectTarget);

                                        Brush brshText = Brushes.Black;
                                        Font fntSelect = (nRow == 0 || nRow == 2) ? fntHeader : fntBody;

                                        if (nRow == 1 && nCol == 2)
                                        {
                                            fntSelect = fntBodyBold;
                                            brshText = (strFinalDecision == "합격") ? Brushes.Blue : (strFinalDecision == "불합격") ? Brushes.Red : Brushes.Gray;
                                        }

                                        gtxCanvas.DrawString(arrInfoMatrix[nRow, nCol], fntSelect, brshText, rectTarget, sfCenter);
                                        nGridX += arrColWidths[nCol];
                                    }
                                    nGridY += nInfoRowHeight;
                                }
                            }
                            fCurrentY = nGridY + 16f;
                        }
                        else
                        {
                            gtxCanvas.DrawString($"시험 결과 보고서 (페이지 {nPageIndex})", fntSubSection, Brushes.Gray, fStartX, fCurrentY);
                            fCurrentY += 25f;
                        }

                        // ---------------------------------------------------------
                        //  가변 회차 수에 맞춘 열 너비 동적 계산
                        // ---------------------------------------------------------
                        float fTitleRatio = (nMaxRoundCount <= 1) ? 0.60f : (nMaxRoundCount <= 3) ? 0.38f : (nMaxRoundCount <= 5) ? 0.28f : 0.24f;
                        int nTitleColW = (int)(fPageWidth * fTitleRatio);
                        int nRemainW = (int)fPageWidth - nTitleColW;
                        int nRoundColW = nRemainW / nMaxRoundCount;

                        int[] colWidths = new int[nMaxRoundCount + 1];
                        colWidths[0] = nTitleColW;
                        for (int c = 1; c < nMaxRoundCount; c++)
                        {
                            colWidths[c] = nRoundColW;
                        }
                        colWidths[nMaxRoundCount] = nRemainW - (nRoundColW * (nMaxRoundCount - 1));

                        float fRowH = 21f;

                        Font fntResult = (nMaxRoundCount >= 5) ? new Font("맑은 고딕", 7.5f, FontStyle.Regular) : fntBody;
                        Font fntResultBold = (nMaxRoundCount >= 5) ? new Font("맑은 고딕", 7.5f, FontStyle.Bold) : fntBodyBold;

                        using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        using (StringFormat sfLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                        {
                            while (nItemIndex < listItems.Count)
                            {
                                string[] arrCurrentItem = listItems[nItemIndex];
                                string strType = arrCurrentItem[0];

                                if (strType == "Spacing")
                                {
                                    fCurrentY += float.Parse(arrCurrentItem[1]);
                                    nItemIndex++;
                                    continue;
                                }

                                float fItemHeight = (strType == "Section") ? 25f : (strType == "SubSection" ? 22f : fRowH);

                                if (fCurrentY + fItemHeight > ePage.MarginBounds.Bottom)
                                {
                                    ePage.HasMorePages = true;
                                    nPageIndex++;
                                    return;
                                }

                                if (strType == "Section")
                                {
                                    gtxCanvas.DrawString(arrCurrentItem[1], fntSection, Brushes.Black, fStartX, fCurrentY + 3f);
                                    fCurrentY += fItemHeight;
                                }
                                else if (strType == "SubSection")
                                {
                                    gtxCanvas.DrawString(arrCurrentItem[1], fntSubSection, Brushes.DarkSlateGray, fStartX + 5f, fCurrentY + 2f);
                                    fCurrentY += fItemHeight;
                                }
                                else if (strType == "Header")
                                {
                                    int curX = (int)fStartX;
                                    for (int col = 0; col < colWidths.Length && (col + 1) < arrCurrentItem.Length; col++)
                                    {
                                        int w = colWidths[col];
                                        Rectangle rectH = new Rectangle(curX, (int)fCurrentY, w, (int)fRowH);
                                        gtxCanvas.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), rectH);
                                        gtxCanvas.DrawRectangle(Pens.DarkGray, rectH);

                                        gtxCanvas.DrawString(arrCurrentItem[col + 1], fntHeader, Brushes.Black, rectH, sfCenter);
                                        curX += w;
                                    }
                                    fCurrentY += fRowH;
                                }
                                else if (strType == "Row")
                                {
                                    int curX = (int)fStartX;

                                    // 0번째 열: 항목명
                                    int titleW = colWidths[0];
                                    Rectangle rectTitle = new Rectangle(curX, (int)fCurrentY, titleW, (int)fRowH);
                                    gtxCanvas.DrawRectangle(Pens.DarkGray, rectTitle);

                                    Rectangle rectTextPadding = rectTitle;
                                    rectTextPadding.X += 8;
                                    rectTextPadding.Width -= 8;
                                    gtxCanvas.DrawString(arrCurrentItem[1], fntBody, Brushes.Black, rectTextPadding, sfLeft);
                                    curX += titleW;

                                    // 1 ~ N번째 열: 각 회차별 판정 데이터
                                    for (int c = 1; c < colWidths.Length && (c + 1) < arrCurrentItem.Length; c++)
                                    {
                                        int w = colWidths[c];
                                        Rectangle rectCell = new Rectangle(curX, (int)fCurrentY, w, (int)fRowH);
                                        gtxCanvas.DrawRectangle(Pens.DarkGray, rectCell);

                                        string strVal = arrCurrentItem[c + 1];

                                        Brush brshText;
                                        if (strVal.Contains("불합격") || strVal.Contains("FAIL") || strVal.Contains("ERR"))
                                        {
                                            brshText = Brushes.Red;
                                        }
                                        else if (strVal.Contains("합격") || strVal.Contains("PASS"))
                                        {
                                            brshText = Brushes.Blue;
                                        }
                                        else
                                        {
                                            brshText = Brushes.Gray;
                                        }

                                        bool isDecided = strVal.Contains("불합격") || strVal.Contains("합격") || strVal.Contains("FAIL") || strVal.Contains("PASS");
                                        gtxCanvas.DrawString(strVal, isDecided ? fntResultBold : fntResult, brshText, rectCell, sfCenter);

                                        curX += w;
                                    }
                                    fCurrentY += fRowH;
                                }

                                nItemIndex++;
                                pgbStatus.Value = Math.Min(nItemIndex, pgbStatus.Maximum);
                                pgbStatus.Update();
                            }
                        }

                        ePage.HasMorePages = false;
                    };

                    prtDoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"보고서 출력 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                frmProgress.Close();
                frmProgress.Dispose();
            }

            bool bIsFileReady = false;
            for (int nRetry = 0; nRetry < 30; nRetry++)
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
                catch (IOException) { }
                Thread.Sleep(100);
            }

            if (bIsFileReady)
            {
                Process.Start(new ProcessStartInfo { FileName = strFilePath, UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("PDF 파일 생성이 지연되고 있습니다. 바탕화면에서 확인하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnClose_Click_2(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnExcel_Click(object sender, EventArgs e)
    {
        DialogResult drSelect = MessageBox.Show(
            "1CC.xls 양식과 동일한 형태로 엑셀 보고서를 저장하시겠습니까?",
            "엑셀 저장 확인",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (drSelect != DialogResult.Yes) return;

        string strDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string strTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string strUnitType = m_objTestResult?.Header?.TCMSUnit ?? "TC";
        string strFilePath = Path.Combine(strDesktopPath, $"{strUnitType}_시험결과보고서_{strTimeStamp}.xlsx");

        if (File.Exists(strFilePath))
        {
            try { File.Delete(strFilePath); }
            catch (IOException)
            {
                MessageBox.Show("기존 엑셀 파일이 열려 있습니다. 닫은 후 다시 시도해 주세요.", "파일 잠김", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        try
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Sheet1");
                ws.Style.Font.FontName = "맑은 고딕";

                // 1. 열 너비 정밀 설정 (A열 글씨 잘림 방지를 위해 5.8로 확장, B~O열은 4.75 유지)
                ws.Column(1).Width = 5.8;
                for (int c = 2; c <= 15; c++)
                {
                    ws.Column(c).Width = 4.75;
                }

                void ApplyBorder(IXLRange range)
                {
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                int nMaxRoundCount = GetMaxRoundCount(m_objTestResult);
                if (nMaxRoundCount <= 0) nMaxRoundCount = 1;
                string countFormat = $"{nMaxRoundCount}[0]";

                // 2. 상단 타이틀 (Row 2, A:O 병합, 굵게, 밑줄)
                var titleRange = ws.Range("A2:O2");
                titleRange.Merge();
                titleRange.Value = "AREX TCMS 시험기 보고서";
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.FontSize = 15;
                titleRange.Style.Font.Underline = XLFontUnderlineValues.Single;
                titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Row(2).Height = 28;

                // 3. 메타데이터 (Row 4, Row 5 - 테두리 없음, 원본 양식 좌표 1:1 일치)
                ws.Range("A4:C4").Merge().Value = " [시험 시각]";
                ws.Range("A4:C4").Style.Font.Bold = true;
                ws.Range("A4:C4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                string testDate = m_objTestResult?.Header?.TestDateTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Range("E4:H4").Merge().Value = testDate;
                ws.Range("E4:H4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("I4").Value = "~";
                ws.Cell("I4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Range("J4:M4").Merge().Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Range("J4:M4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(4).Height = 22;

                ws.Range("A5:C5").Merge().Value = " [제품 번호]";
                ws.Range("A5:C5").Style.Font.Bold = true;
                ws.Range("A5:C5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Range("E5:M5").Merge().Value = $"AREX-{strUnitType}-{m_objTestResult?.Header?.SerialNo ?? "0000"}";
                ws.Range("E5:M5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(5).Height = 22;

                int curRow = 7;

                // =========================================================================
                // 4. 통신 시험 영역 (1CC.xls 스타일: 좌측 세로 배너 + 시험횟수/성공/실패/비고)
                // =========================================================================
                List<string> commItems = new List<string>();
                if (strUnitType == "ER") commItems.AddRange(new[] { "LAN", "PROBE", "MVB", "CPM", "USB" });
                else if (strUnitType == "DU") commItems.AddRange(new[] { "MVB" });
                else commItems.AddRange(new[] { "WTB", "MVB", "RS485-1", "RS485-2", "RS485-3" });

                // 섹션 배너
                var commSec = ws.Range(curRow, 1, curRow, 4);
                commSec.Merge().Value = $"{strUnitType} - 통신";
                commSec.Style.Font.Bold = true;
                commSec.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ApplyBorder(commSec);
                ws.Row(curRow).Height = 22;
                curRow++;

                // 테이블 헤더
                ws.Range(curRow, 1, curRow, 4).Merge().Value = "시험 항목";
                ws.Range(curRow, 5, curRow, 7).Merge().Value = "시험 횟수";
                ws.Range(curRow, 8, curRow, 10).Merge().Value = "성공 횟수";
                ws.Range(curRow, 11, curRow, 13).Merge().Value = "실패 횟수";
                ws.Range(curRow, 14, curRow, 15).Merge().Value = "비 고";

                var commHdr = ws.Range(curRow, 1, curRow, 15);
                commHdr.Style.Font.Bold = true;
                commHdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                commHdr.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ApplyBorder(commHdr);
                ws.Row(curRow).Height = 22;
                curRow++;

                int commStartRow = curRow;
                foreach (var item in commItems)
                {
                    ws.Range(curRow, 2, curRow, 4).Merge().Value = item;
                    ws.Range(curRow, 5, curRow, 7).Merge().Value = nMaxRoundCount;
                    ws.Range(curRow, 8, curRow, 10).Merge().Value = nMaxRoundCount;
                    ws.Range(curRow, 11, curRow, 13).Merge().Value = 0;
                    ws.Range(curRow, 14, curRow, 15).Merge(); // 비고 빈칸

                    ws.Range(curRow, 1, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(curRow, 1, curRow, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(curRow).Height = 20;
                    curRow++;
                }

                // 좌측 세로 병합 레이블 (A열: 글씨 잘림 방지를 위해 폰트 9pt, 자동 줄바꿈)
                var commSide = ws.Range(commStartRow, 1, curRow - 1, 1);
                commSide.Merge().Value = "통\n신";
                commSide.Style.Font.Bold = true;
                commSide.Style.Font.FontSize = 9.5;
                commSide.Style.Alignment.WrapText = true;
                commSide.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                commSide.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ApplyBorder(ws.Range(commStartRow, 1, curRow - 1, 15));
                curRow += 2; // 섹션 간 간격

                // =========================================================================
                // 5. 디지털 입력 (DI) 영역 (1CC.xls 스타일: 1~8 열 헤더 및 8채널 묶음 행 요약)
                // =========================================================================
                if (strUnitType != "DU")
                {
                    var diSec = ws.Range(curRow, 1, curRow, 4);
                    diSec.Merge().Value = $"{strUnitType} - DI";
                    diSec.Style.Font.Bold = true;
                    diSec.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ApplyBorder(diSec);

                    ws.Range(curRow, 13, curRow, 15).Merge().Value = "(전체횟수[실패횟수])";
                    ws.Range(curRow, 13, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Row(curRow).Height = 22;
                    curRow++;

                    // DI 테이블 헤더 (A~D: 시험 항목, E~L: 1~8, M~O: 비고)
                    ws.Range(curRow, 1, curRow, 4).Merge().Value = "시험 항목";
                    for (int r = 1; r <= 8; r++)
                    {
                        ws.Cell(curRow, 4 + r).Value = r; // E=1, F=2, G=3, H=4, I=5, J=6, K=7, L=8
                    }
                    ws.Range(curRow, 13, curRow, 15).Merge().Value = "비 고";

                    var diHdr = ws.Range(curRow, 1, curRow, 15);
                    diHdr.Style.Font.Bold = true;
                    diHdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    diHdr.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ApplyBorder(diHdr);
                    ws.Row(curRow).Height = 22;
                    curRow++;

                    int diStartRow = curRow;

                    // 유닛별 DI 블록 정의 (TC: DI1~DI3 각 48채널, CC: DI1~DI2 각 48채널, ER: DI1 8채널)
                    List<Tuple<string, int>> diConfigs = new List<Tuple<string, int>>();
                    if (strUnitType == "TC")
                    {
                        diConfigs.Add(Tuple.Create("DI1", 48));
                        diConfigs.Add(Tuple.Create("DI2", 48));
                        diConfigs.Add(Tuple.Create("DI3", 48));
                    }
                    else if (strUnitType == "CC")
                    {
                        diConfigs.Add(Tuple.Create("DI1", 48));
                        diConfigs.Add(Tuple.Create("DI2", 48));
                    }
                    else if (strUnitType == "ER")
                    {
                        diConfigs.Add(Tuple.Create("DI1", 8));
                    }

                    foreach (var cfg in diConfigs)
                    {
                        string prefix = cfg.Item1;
                        int totalCh = cfg.Item2;

                        for (int startCh = 1; startCh <= totalCh; startCh += 8)
                        {
                            int endCh = Math.Min(startCh + 7, totalCh);
                            ws.Range(curRow, 1, curRow, 4).Merge().Value = $"{prefix} CH.{startCh}-CH.{endCh}";

                            // 1~8열에 1[0] 값 배치
                            for (int colIdx = 0; colIdx < 8; colIdx++)
                            {
                                ws.Cell(curRow, 5 + colIdx).Value = countFormat;
                            }
                            ws.Range(curRow, 13, curRow, 15).Merge(); // 비고

                            ws.Range(curRow, 1, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Range(curRow, 1, curRow, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Row(curRow).Height = 20;
                            curRow++;
                        }
                    }

                    ApplyBorder(ws.Range(diStartRow, 1, curRow - 1, 15));
                    curRow += 2;

                    // =========================================================================
                    // 6. 디지털 출력 (DO) 및 아날로그 영역 (8채널 요약 그리드 동일 적용)
                    // =========================================================================
                    var doSec = ws.Range(curRow, 1, curRow, 4);
                    doSec.Merge().Value = $"{strUnitType} - DO";
                    doSec.Style.Font.Bold = true;
                    doSec.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ApplyBorder(doSec);

                    ws.Range(curRow, 13, curRow, 15).Merge().Value = "(전체횟수[실패횟수])";
                    ws.Range(curRow, 13, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Row(curRow).Height = 22;
                    curRow++;

                    ws.Range(curRow, 1, curRow, 4).Merge().Value = "시험 항목";
                    for (int r = 1; r <= 8; r++)
                    {
                        ws.Cell(curRow, 4 + r).Value = r;
                    }
                    ws.Range(curRow, 13, curRow, 15).Merge().Value = "비 고";

                    var doHdr = ws.Range(curRow, 1, curRow, 15);
                    doHdr.Style.Font.Bold = true;
                    doHdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    doHdr.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ApplyBorder(doHdr);
                    ws.Row(curRow).Height = 22;
                    curRow++;

                    int doStartRow = curRow;
                    int totalDo = (strUnitType == "ER") ? 2 : 32;

                    for (int startCh = 1; startCh <= totalDo; startCh += 8)
                    {
                        int endCh = Math.Min(startCh + 7, totalDo);
                        ws.Range(curRow, 1, curRow, 4).Merge().Value = $"DO CH.{startCh}-CH.{endCh}";

                        for (int colIdx = 0; colIdx < 8; colIdx++)
                        {
                            ws.Cell(curRow, 5 + colIdx).Value = (startCh + colIdx <= totalDo) ? countFormat : "-";
                        }
                        ws.Range(curRow, 13, curRow, 15).Merge();

                        ws.Range(curRow, 1, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(curRow, 1, curRow, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Row(curRow).Height = 20;
                        curRow++;
                    }

                    // TC, CC 유닛의 경우 아날로그 입출력(4채널) 추가 요약
                    if (strUnitType == "TC" || strUnitType == "CC")
                    {
                        string[] analogItems = { "아날로그 입력 (4CH)", "아날로그 출력 (4CH)" };
                        foreach (var aItem in analogItems)
                        {
                            ws.Range(curRow, 1, curRow, 4).Merge().Value = aItem;
                            for (int colIdx = 0; colIdx < 4; colIdx++)
                            {
                                ws.Cell(curRow, 5 + colIdx).Value = countFormat;
                            }
                            for (int colIdx = 4; colIdx < 8; colIdx++)
                            {
                                ws.Cell(curRow, 5 + colIdx).Value = "-";
                            }
                            ws.Range(curRow, 13, curRow, 15).Merge();

                            ws.Range(curRow, 1, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Range(curRow, 1, curRow, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Row(curRow).Height = 20;
                            curRow++;
                        }
                    }

                    ApplyBorder(ws.Range(doStartRow, 1, curRow - 1, 15));
                    curRow += 2;
                }

                // =========================================================================
                // 7. 메모리 시험 (MEM) 영역 (1CC.xls 스타일: 6개 상세 컬럼 구조 복원)
                // =========================================================================
                if (strUnitType != "DU")
                {
                    var memSec = ws.Range(curRow, 1, curRow, 4);
                    memSec.Merge().Value = $"{strUnitType} - MEM";
                    memSec.Style.Font.Bold = true;
                    memSec.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ApplyBorder(memSec);

                    ws.Range(curRow, 13, curRow, 15).Merge().Value = "(전체횟수[실패횟수])";
                    ws.Range(curRow, 13, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Row(curRow).Height = 22;
                    curRow++;

                    // 메모리 테이블 헤더 (1CC.xls 원본 컬럼 인덱스 1:1 일치)
                    ws.Range(curRow, 1, curRow, 5).Merge().Value = "메모리 종류";
                    ws.Range(curRow, 6, curRow, 7).Merge().Value = "결과";
                    ws.Range(curRow, 8, curRow, 9).Merge().Value = "오류값";
                    ws.Range(curRow, 10, curRow, 11).Merge().Value = "WriteErr";
                    ws.Range(curRow, 12, curRow, 13).Merge().Value = "ReadErr";
                    ws.Range(curRow, 14, curRow, 15).Merge().Value = "횟수";

                    var memHdr = ws.Range(curRow, 1, curRow, 15);
                    memHdr.Style.Font.Bold = true;
                    memHdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    memHdr.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ApplyBorder(memHdr);
                    ws.Row(curRow).Height = 22;
                    curRow++;

                    int memStartRow = curRow;

                    // 유닛별 메모리 시험 항목 목록
                    List<string> memList = new List<string>();
                    if (strUnitType == "ER") memList.AddRange(new[] { "FLASH", "SDRAM", "FRAM", "RTC", "HRS" });
                    else memList.AddRange(new[] { "DPRAM", "SDRAM", "MRAM", "FLASH", "EMMC", "USB", "ENET_1", "ENET_2" });

                    foreach (var mItem in memList)
                    {
                        ws.Range(curRow, 1, curRow, 3).Merge().Value = $"{strUnitType}-{mItem}";
                        ws.Range(curRow, 4, curRow, 5).Merge().Value = "0x0000";
                        ws.Range(curRow, 6, curRow, 7).Merge().Value = "성공";
                        ws.Range(curRow, 8, curRow, 9).Merge().Value = "없음";
                        ws.Range(curRow, 10, curRow, 11).Merge().Value = "해당없음";
                        ws.Range(curRow, 12, curRow, 13).Merge().Value = "해당없음";
                        ws.Range(curRow, 14, curRow, 15).Merge().Value = countFormat;

                        ws.Range(curRow, 1, curRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(curRow, 1, curRow, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Row(curRow).Height = 20;
                        curRow++;
                    }

                    ApplyBorder(ws.Range(memStartRow, 1, curRow - 1, 15));
                }

                workbook.SaveAs(strFilePath);
            }

            MessageBox.Show("1CC.xls 양식과 완벽히 일치하는 엑셀 파일이 저장되었습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(new ProcessStartInfo { FileName = strFilePath, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"엑셀 파일 저장 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
}