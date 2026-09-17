using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
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
using System.Threading;
using System.Windows.Forms;
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

            Label_Unit.Text = strUnitType;
            Label_Fleet.Text = objHeaderInfo.FleetNo ?? string.Empty;
            Label_Train.Text = objHeaderInfo.TrainNo ?? string.Empty;
            Label_Tester.Text = objHeaderInfo.TesterName ?? string.Empty;
            Label_Serial.Text = objHeaderInfo.SerialNo ?? string.Empty;
            Label_Round.Text = $"{objHeaderInfo.TotalRound} 회";
            Label_FinalResult.Text = objHeaderInfo.FinalResult ?? string.Empty;

            // 최종 판정 라벨 색상 설정
            Label_FinalResult.ForeColor = (objHeaderInfo.FinalResult == "합격") ? Color.Blue : Color.Red;

            // DU는 입출력/메모리 시험이 없으므로 '해당없음' 처리
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

            // ★ 좌측 패널 간섭 없이 우측 카드만 안전하게 재배치
            AdjustLayoutForUnit(isDu);

            InitTestDataGridViews();
        }

        /// <summary>
        /// 좌측 패널(시험정보/결과판정/오류목록)은 100% 보존하고,
        /// 우측의 불필요한 카드만 숨긴 뒤 통신 카드를 상단으로 끌어올립니다.
        /// </summary>
        private void AdjustLayoutForUnit(bool isDu)
        {
            Control cardDio = dataGridViewDIO?.Parent;
            Control cardComm = dataGridViewComm?.Parent;
            Control cardMem = dataGridViewMemory?.Parent;

            if (cardDio == null || cardComm == null || cardMem == null) return;

            // 최초 1회 원래 통신 카드 Y좌표 기억
            if (_origCommTop == -1)
            {
                _origCommTop = cardComm.Top;
            }

            if (isDu)
            {
                // 1. 불필요한 입출력 및 메모리 카드 숨김
                cardDio.Visible = false;
                cardMem.Visible = false;

                // 2. 통신 카드를 최상단(입출력 카드가 있던 위치)으로 이동 (Dock은 절대 건드리지 않음)
                cardComm.Top = cardDio.Top;
                cardComm.Visible = true;
            }
            else
            {
                // TC / CC 모드: 정상 위치 및 표시 상태 복구
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
                if (objGrid.GridTitle.Contains("디지털") || objGrid.GridTitle.Contains("아날로그"))
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
        // 그리드 3종 초기화 (DU 유닛은 MVB/RS-485 2개만 단독 표출)
        // =========================================================================
        private void InitTestDataGridViews()
        {
            int nMaxRoundCount = GetMaxRoundCount(m_objTestResult);
            string strUnit = m_objTestResult?.Header?.TCMSUnit ?? "CC";
            bool isCc = strUnit.Equals("CC", StringComparison.OrdinalIgnoreCase);
            bool isDu = strUnit.Equals("DU", StringComparison.OrdinalIgnoreCase);

            // 1. 입·출력 시험 그리드 (DU는 보드가 없으므로 행 미생성)
            string[] arrDioRows = isDu
                ? Array.Empty<string>()
                : (isCc
                    ? new string[] { "DI1", "DI2", "DO", "아날로그 입력", "아날로그 출력" }
                    : new string[] { "DI1", "DI2", "DI3", "DO", "아날로그 입력", "아날로그 출력" });

            SetupTestGrid(dataGridViewDIO, arrDioRows, nMaxRoundCount, (strTitle, nRound) => GetDioRoundResult(m_objTestResult, strTitle, nRound));

            // 2. 통신 시험 그리드 (DU는 MVB와 RS-485 2개만 표출)
            string[] arrCommRows = isDu
                ? new string[] { "MVB", "RS-485" }
                : new string[] { "WTB", "MVB", "RS485-1", "RS485-2", "RS485-3" };

            SetupTestGrid(dataGridViewComm, arrCommRows, nMaxRoundCount, (strTitle, nRound) => GetGenericRoundResult(m_objTestResult, "통신", strTitle, nRound));

            // 3. 메모리 및 이더넷 시험 그리드 (DU는 VCPUT 메모리 진단이 없으므로 행 미생성)
            string[] arrMemoryRows = isDu
                ? Array.Empty<string>()
                : new string[] { "DPRAM", "SDRAM", "MRAM", "FLASH", "EMMC", "USB", "ENET_1", "ENET_2" };

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
            return 3;
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
        // PDF 결과 보고서 생성 및 인쇄
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

            string strSerialNo = m_objTestResult?.Header?.SerialNo ?? "0000";
            string strCarNo = m_objTestResult?.Header?.FleetNo ?? "0000";
            string strTrainNo = m_objTestResult?.Header?.TrainNo ?? "0000";
            string strTester = m_objTestResult?.Header?.TesterName ?? "Tester";
            string strFinalDecision = m_objTestResult?.Header?.FinalResult ?? "미시험";

            List<string[]> listItems = new List<string[]>();

            if (isDu)
            {
                listItems.Add(new string[] { "Section", "1. DU 통신 시험 (MVB / RS-485)" });
                listItems.Add(new string[] { "Header", "통신 시험 항목", "판정" });
                listItems.Add(new string[] { "Row", "MVB 통신", GetGenericRoundResult(m_objTestResult, "통신", "MVB", 1) });
                listItems.Add(new string[] { "Row", "RS-485 통신", GetGenericRoundResult(m_objTestResult, "통신", "RS-485", 1) });
                listItems.Add(new string[] { "Spacing", "15" });
            }
            else
            {
                listItems.Add(new string[] { "Section", "1. 입·출력 시험" });
                listItems.Add(new string[] { "SubSection", "1.1 디지털 입출력 (DI / DO)" });
                listItems.Add(new string[] { "Header", "시험 항목", "수량 및 판정" });
                listItems.Add(new string[] { "Row", "디지털 입력 1 (DI 1)", GetDioRoundResult(m_objTestResult, "DI1", 1) });
                listItems.Add(new string[] { "Row", "디지털 입력 2 (DI 2)", GetDioRoundResult(m_objTestResult, "DI2", 1) });

                if (strUnitType.Equals("TC", StringComparison.OrdinalIgnoreCase))
                {
                    listItems.Add(new string[] { "Row", "디지털 입력 3 (DI 3)", GetDioRoundResult(m_objTestResult, "DI3", 1) });
                }

                listItems.Add(new string[] { "Row", "디지털 출력 (DO)", GetDioRoundResult(m_objTestResult, "DO", 1) });
                listItems.Add(new string[] { "Spacing", "10" });

                listItems.Add(new string[] { "SubSection", "1.2 아날로그 입출력 (AI / AO)" });
                listItems.Add(new string[] { "Header", "시험 항목", "수량 및 판정" });
                listItems.Add(new string[] { "Row", "아날로그 입력 (AI)", GetDioRoundResult(m_objTestResult, "아날로그 입력", 1) });
                listItems.Add(new string[] { "Row", "아날로그 출력 (AO)", GetDioRoundResult(m_objTestResult, "아날로그 출력", 1) });
                listItems.Add(new string[] { "Spacing", "15" });

                listItems.Add(new string[] { "Section", "2. 통신 시험" });
                listItems.Add(new string[] { "CommGrid", strUnitType });
                listItems.Add(new string[] { "Spacing", "15" });

                listItems.Add(new string[] { "Section", "3. 메모리 및 이더넷 시험" });
                listItems.Add(new string[] { "Header", "시험 항목", "판정" });
                listItems.Add(new string[] { "Row", "DPRAM", GetGenericRoundResult(m_objTestResult, "메모리", "DPRAM", 1) });
                listItems.Add(new string[] { "Row", "SDRAM", GetGenericRoundResult(m_objTestResult, "메모리", "SDRAM", 1) });
                listItems.Add(new string[] { "Row", "MRAM", GetGenericRoundResult(m_objTestResult, "메모리", "MRAM", 1) });
                listItems.Add(new string[] { "Row", "FLASH", GetGenericRoundResult(m_objTestResult, "메모리", "FLASH", 1) });
                listItems.Add(new string[] { "Row", "eMMC", GetGenericRoundResult(m_objTestResult, "메모리", "EMMC", 1) });
                listItems.Add(new string[] { "Row", "USB 메모리", GetGenericRoundResult(m_objTestResult, "메모리", "USB", 1) });
                listItems.Add(new string[] { "Row", "이더넷 포트 1 (ENET_1)", GetGenericRoundResult(m_objTestResult, "메모리", "ENET_1", 1) });
                listItems.Add(new string[] { "Row", "이더넷 포트 2 (ENET_2)", GetGenericRoundResult(m_objTestResult, "메모리", "ENET_2", 1) });
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
                            string strTitleText = isDu ? "DU 표시기 시험 결과 보고서" : "TCMS 시험기 결과 보고서";
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

                        int nColWidth1 = (int)(fPageWidth * 0.75f);
                        int nColWidth2 = (int)fPageWidth - nColWidth1;
                        float fRowH = 21f;

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

                                if (strType == "CommGrid")
                                {
                                    string[] arrCommItems = new string[] { "WTB 통신", "MVB 통신", "RS-485 #1", "RS-485 #2", "RS-485 #3" };
                                    string[] arrCommKeys = new string[] { "WTB", "MVB", "RS485-1", "RS485-2", "RS485-3" };
                                    float fTotalGridH = fRowH * (arrCommItems.Length + 1);

                                    if (fCurrentY + fTotalGridH > ePage.MarginBounds.Bottom)
                                    {
                                        ePage.HasMorePages = true;
                                        nPageIndex++;
                                        return;
                                    }

                                    Rectangle rectH1 = new Rectangle((int)fStartX, (int)fCurrentY, nColWidth1, (int)fRowH);
                                    Rectangle rectH2 = new Rectangle((int)fStartX + nColWidth1, (int)fCurrentY, nColWidth2, (int)fRowH);
                                    gtxCanvas.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), rectH1);
                                    gtxCanvas.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), rectH2);
                                    gtxCanvas.DrawRectangle(Pens.DarkGray, rectH1);
                                    gtxCanvas.DrawRectangle(Pens.DarkGray, rectH2);
                                    gtxCanvas.DrawString("통신 시험 항목", fntHeader, Brushes.Black, rectH1, sfCenter);
                                    gtxCanvas.DrawString("판정", fntHeader, Brushes.Black, rectH2, sfCenter);
                                    fCurrentY += fRowH;

                                    for (int i = 0; i < arrCommItems.Length; i++)
                                    {
                                        Rectangle rectR1 = new Rectangle((int)fStartX, (int)fCurrentY, nColWidth1, (int)fRowH);
                                        Rectangle rectR2 = new Rectangle((int)fStartX + nColWidth1, (int)fCurrentY, nColWidth2, (int)fRowH);
                                        gtxCanvas.DrawRectangle(Pens.DarkGray, rectR1);
                                        gtxCanvas.DrawRectangle(Pens.DarkGray, rectR2);

                                        Rectangle rectTextPadding = rectR1;
                                        rectTextPadding.X += 8;
                                        rectTextPadding.Width -= 8;

                                        string strVal = GetGenericRoundResult(m_objTestResult, "통신", arrCommKeys[i], 1);

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

                                        gtxCanvas.DrawString(arrCommItems[i], fntBody, Brushes.Black, rectTextPadding, sfLeft);
                                        gtxCanvas.DrawString(strVal, (strVal.Contains("합격") || strVal.Contains("불합격")) ? fntBodyBold : fntBody, brshText, rectR2, sfCenter);

                                        fCurrentY += fRowH;
                                    }

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
                                    Rectangle rectH1 = new Rectangle((int)fStartX, (int)fCurrentY, nColWidth1, (int)fRowH);
                                    Rectangle rectH2 = new Rectangle((int)fStartX + nColWidth1, (int)fCurrentY, nColWidth2, (int)fRowH);

                                    gtxCanvas.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), rectH1);
                                    gtxCanvas.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), rectH2);
                                    gtxCanvas.DrawRectangle(Pens.DarkGray, rectH1);
                                    gtxCanvas.DrawRectangle(Pens.DarkGray, rectH2);

                                    gtxCanvas.DrawString(arrCurrentItem[1], fntHeader, Brushes.Black, rectH1, sfCenter);
                                    gtxCanvas.DrawString(arrCurrentItem[2], fntHeader, Brushes.Black, rectH2, sfCenter);
                                    fCurrentY += fRowH;
                                }
                                else if (strType == "Row")
                                {
                                    Rectangle rectR1 = new Rectangle((int)fStartX, (int)fCurrentY, nColWidth1, (int)fRowH);
                                    Rectangle rectR2 = new Rectangle((int)fStartX + nColWidth1, (int)fCurrentY, nColWidth2, (int)fRowH);

                                    gtxCanvas.DrawRectangle(Pens.DarkGray, rectR1);
                                    gtxCanvas.DrawRectangle(Pens.DarkGray, rectR2);

                                    Rectangle rectTextPadding = rectR1;
                                    rectTextPadding.X += 8;
                                    rectTextPadding.Width -= 8;

                                    gtxCanvas.DrawString(arrCurrentItem[1], fntBody, Brushes.Black, rectTextPadding, sfLeft);

                                    string strVal = arrCurrentItem[2];

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
                                    gtxCanvas.DrawString(strVal, isDecided ? fntBodyBold : fntBody, brshText, rectR2, sfCenter);
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
    }
}