
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
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED 스타일 추가
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
                TestResult_IO.Text = "-";
                TestResult_Comn.Text = "-";
                TestResult_Memory.Text = "-";
                Label_Date.Text = DateTime.Now.ToString("yyyy년 MM월 dd일");
                return;
            }

            Label_Unit.Text = objHeaderInfo.TCMSUnit ?? string.Empty;
            Label_Fleet.Text = objHeaderInfo.FleetNo ?? string.Empty;
            Label_Train.Text = objHeaderInfo.TrainNo ?? string.Empty;
            Label_Tester.Text = objHeaderInfo.TesterName ?? string.Empty;
            Label_Serial.Text = objHeaderInfo.SerialNo ?? string.Empty;
            Label_Round.Text = $"{objHeaderInfo.TotalRound} 회";
            Label_FinalResult.Text = objHeaderInfo.FinalResult ?? string.Empty;

            string strIoResult = GetIoCategoryResult(m_objTestResult);
            string strComnResult = GetGenericCategoryResult(m_objTestResult, "통신 시험");
            string strMemResult = GetGenericCategoryResult(m_objTestResult, "메모리 시험");

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
            InitTestDataGridViews();
        }

        // 입출력(디지털/아날로그) 시험 항목 판정 산출 함수
        private string GetIoCategoryResult(TestResultJson objTestResult)
        {
            if (objTestResult?.GridResults == null) return "-";

            bool bHasIoData = false;

            foreach (var objGrid in objTestResult.GridResults)
            {
                if (objGrid.GridTitle == "디지털 입출력 시험" || objGrid.GridTitle == "아날로그 입출력 시험")
                {
                    if (objGrid.PinDetails != null && objGrid.PinDetails.Count > 0)
                    {
                        bHasIoData = true;

                        // 세부 핀 항목 중 하나라도 불합격이 존재하면 불합격 처리
                        if (objGrid.PinDetails.Exists(objPin => objPin.Result == "불합격" || objPin.MeasuredValue == "ERR"))
                        {
                            return "불합격";
                        }
                    }
                }
            }

            return bHasIoData ? "합격" : "미시험";
        }

        // 통신, 메모리 시험 항목 판정 산출 함수
        private string GetGenericCategoryResult(TestResultJson objTestResult, string strTargetGridTitle)
        {
            if (objTestResult?.GridResults == null) return "-";

            foreach (var objGrid in objTestResult.GridResults)
            {
                if (objGrid.GridTitle == strTargetGridTitle)
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
                }
            }

            return "미시험";
        }

        // 시험 결과 상태에 따른 라벨 컨트롤 스타일(글자색/배경색) 변경 함수
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

                case "미시험":
                default:
                    lblTarget.BackColor = Color.FromArgb(230, 230, 230); 
                    lblTarget.ForeColor = Color.FromArgb(100, 100, 100); 
                    break;
            }
        }

        // 시험 실패 목록을 수집하여 RichTextBox에 표시하는 함수
        private void DisplayFailedLog(RichTextBox rtbTarget, TestResultJson objTestResult)
        {
            if (rtbTarget == null) return;

            rtbTarget.SuspendLayout();
            rtbTarget.Clear();

            try
            {
                if (objTestResult?.GridResults == null || objTestResult.GridResults.Count == 0)
                {
                    return;
                }

                bool bHasError = false;

                foreach (var objGrid in objTestResult.GridResults)
                {
                    string strGridTitle = objGrid.GridTitle;

                    // 핀 상세 데이터(디지털/아날로그 입출력) 실패 항목 탐색
                    if (objGrid.PinDetails != null && objGrid.PinDetails.Count > 0)
                    {
                        var listFails = objGrid.PinDetails.FindAll(objPin => objPin.Result == "불합격" || objPin.MeasuredValue == "ERR");

                        foreach (var objFailPin in listFails)
                        {
                            bHasError = true;
                            string strErrText = $"[{strGridTitle}] {objFailPin.Round}회차 - {objFailPin.PinName} - 불합격\n";

                            rtbTarget.SelectionColor = Color.Red;
                            rtbTarget.AppendText(strErrText);
                        }
                    }

                    // 행 데이터(통신/메모리 시험 등) 실패 항목 탐색
                    if (objGrid.RowData != null && objGrid.RowData.Count > 0)
                    {
                        foreach (var pairRow in objGrid.RowData)
                        {
                            string strRowTitle = pairRow.Key;
                            List<string> listResults = pairRow.Value;

                            if (listResults == null) continue;

                            for (int nIdx = 0; nIdx < listResults.Count; nIdx++)
                            {
                                string strVal = listResults[nIdx];
                                if (strVal == "불합격" || strVal == "FAIL" || strVal == "ERR")
                                {
                                    bHasError = true;
                                    int nRound = nIdx + 1;
                                    string strErrText = $"[{strGridTitle}] {nRound}회차 - {strRowTitle} - 불합격\n";

                                    rtbTarget.SelectionColor = Color.Red;
                                    rtbTarget.AppendText(strErrText);
                                }
                            }
                        }
                    }
                }

                if (bHasError)
                {
                    imagebtn2.Visible = false;
                }
                else
                {
                    imagebtn2.Visible = true;
                }
            }
            finally
            {
                rtbTarget.ResumeLayout();
            }
        }
        private void InitTestDataGridViews()
        {
            int nMaxRoundCount = GetMaxRoundCount(m_objTestResult);

            // 디지털 및 아날로그 입출력 그리드 구성
            string[] arrDioRows = new string[] { "DI1", "DI2", "DI3", "DO", "아날로그" };
            SetupTestGrid(dataGridViewDIO, arrDioRows, nMaxRoundCount, (strTitle, nRound) => GetDioRoundResult(m_objTestResult, strTitle, nRound));

            // 통신 시험 그리드 구성
            string[] arrCommRows = new string[] { "WTB", "MVB", "RS-485" };
            SetupTestGrid(dataGridViewComm, arrCommRows, nMaxRoundCount, (strTitle, nRound) => GetGenericRoundResult(m_objTestResult, "통신 시험", strTitle, nRound));

            // 메모리 시험 그리드 구성
            string[] arrMemoryRows = new string[] { "임시1", "임시2", "임시3" };
            SetupTestGrid(dataGridViewMemory, arrMemoryRows, nMaxRoundCount, (strTitle, nRound) => GetGenericRoundResult(m_objTestResult, "메모리 시험", strTitle, nRound));
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
                dgvTarget.TopLeftHeaderCell.Style.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
                dgvTarget.TopLeftHeaderCell.Style.ForeColor = clrDarkBlueText;
                dgvTarget.TopLeftHeaderCell.Style.BackColor = clrSkyBlueBg;

                dgvTarget.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvTarget.MultiSelect = false;

                dgvTarget.RowHeadersVisible = true;
                dgvTarget.RowHeadersWidth = 120;
                dgvTarget.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

                dgvTarget.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvTarget.ColumnHeadersHeight = 40;

                Font fntRegular = new Font("맑은 고딕", 10F, FontStyle.Regular);
                Font fntBold = new Font("맑은 고딕", 11F, FontStyle.Bold);

                dgvTarget.DefaultCellStyle.Font = fntRegular;
                dgvTarget.ColumnHeadersDefaultCellStyle.Font = fntBold;
                dgvTarget.RowHeadersDefaultCellStyle.Font = fntBold;

                for (int nIndex = 1; nIndex <= nRoundCount; nIndex++)
                {
                    string strColName = $"colRound{nIndex}";
                    string strHeaderText = $"{nIndex}회차";

                    int nColIndex = dgvTarget.Columns.Add(strColName, strHeaderText);
                    dgvTarget.Columns[nColIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvTarget.Columns[nColIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // 시험 결과 바인딩
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
                            dgvTarget.Rows[nRowIndex].Cells[nColIdx].Value = strResultValue;
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
                                objRow.Height = nCalculatedHeight;
                            }
                        }
                    };

                    actAdjustRowHeights();

                    // 중복 이벤트 핸들러 등록 방지 처리 (핸들러 누수 차단)
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

                dgvTarget.CellPainting += (s, e) =>
                {
                    if (e.ColumnIndex == -1 && e.RowIndex >= 0)
                    {
                        e.Paint(e.ClipBounds, DataGridViewPaintParts.Background
                                            | DataGridViewPaintParts.Border
                                            | DataGridViewPaintParts.ContentForeground);
                        e.Handled = true;
                    }
                };
            }
            finally
            {
                dgvTarget.ResumeLayout();
            }
        }

        // 최대 시험 회차 계산 함수 (기본값: 5회)
        private int GetMaxRoundCount(TestResultJson objTestResult)
        {
            int nMaxRound = 5;

            if (objTestResult?.GridResults == null) return nMaxRound;

            foreach (TestResultJson.GridTestResult objGrid in objTestResult.GridResults)
            {
                if (objGrid.HeaderRounds != null && objGrid.HeaderRounds.Count > nMaxRound)
                {
                    nMaxRound = objGrid.HeaderRounds.Count;
                }

                if (objGrid.PinDetails != null)
                {
                    foreach (TestResultJson.PinResultItem objPin in objGrid.PinDetails)
                    {
                        if (objPin.Round > nMaxRound)
                        {
                            nMaxRound = objPin.Round;
                        }
                    }
                }
            }

            return nMaxRound;
        }

        // 입출력 시험(DI/DO/아날로그) 회차별 결과 산출 함수
        private string GetDioRoundResult(TestResultJson objTestResult, string strTargetGroup, int nRound)
        {
            if (objTestResult?.GridResults == null) return "-";

            List<TestResultJson.PinResultItem> listMatchedPins = new List<TestResultJson.PinResultItem>();

            foreach (TestResultJson.GridTestResult objGrid in objTestResult.GridResults)
            {
                if (objGrid.PinDetails == null) continue;

                foreach (TestResultJson.PinResultItem objPin in objGrid.PinDetails)
                {
                    if (objPin.Round == nRound && IsChannelGroupMatch(objPin.ChannelGroup, strTargetGroup))
                    {
                        listMatchedPins.Add(objPin);
                    }
                }
            }

            // 해당 회차 및 그룹에 측정 데이터가 없는 경우
            if (listMatchedPins.Count == 0) return "-";

            // 핀 중 하나라도 불합격 또는 ERR이 존재하면 불합격 처리
            bool bHasFail = listMatchedPins.Exists(objPin => objPin.Result == "불합격" || objPin.MeasuredValue == "ERR");
            return bHasFail ? "불합격" : "합격";
        }

        // 통신/메모리 등 일반 RowData 형태의 회차별 결과 산출 함수
        private string GetGenericRoundResult(TestResultJson objTestResult, string strGridTitle, string strRowTitle, int nRound)
        {
            if (objTestResult?.GridResults == null) return "-";

            foreach (TestResultJson.GridTestResult objGrid in objTestResult.GridResults)
            {
                if (objGrid.GridTitle == strGridTitle && objGrid.RowData != null)
                {
                    if (objGrid.RowData.TryGetValue(strRowTitle, out List<string> listRoundResults))
                    {
                        int nRoundIndex = nRound - 1;
                        if (nRoundIndex >= 0 && nRoundIndex < listRoundResults.Count)
                        {
                            return listRoundResults[nRoundIndex];
                        }
                    }
                }
            }

            return "-";
        }

        // 채널 그룹 명칭 매칭 보조 함수 (아날로그 / ANALOG 호환)
        private bool IsChannelGroupMatch(string strChannelGroup, string strTargetGroup)
        {
            if (string.IsNullOrEmpty(strChannelGroup) || string.IsNullOrEmpty(strTargetGroup)) return false;

            if (strTargetGroup == "아날로그")
            {
                return strChannelGroup.Equals("ANALOG", StringComparison.OrdinalIgnoreCase) ||
                       strChannelGroup.Equals("아날로그", StringComparison.OrdinalIgnoreCase);
            }

            return strChannelGroup.Equals(strTargetGroup, StringComparison.OrdinalIgnoreCase);
        }

        private void DgvTarget_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            throw new NotImplementedException();
        }

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
            for (int nIdx = 1; nIdx <= 5; nIdx++)
            {
                listItems.Add(new string[] { "Row", $"디지털 입력 (DI {nIdx})", "미시험" });
            }


            listItems.Add(new string[] { "ForcePageBreak" });

            listItems.Add(new string[] { "Section", "  1.2 디지털 출력 (DO)" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            for (int nIdx = 1; nIdx <= 5; nIdx++)
            {
                listItems.Add(new string[] { "Row", $"디지털 출력 (DO {nIdx})", "미시험" });
            }

            listItems.Add(new string[] { "ForcePageBreak" });

            listItems.Add(new string[] { "Section", "  1.3 아날로그 입력 (AI)" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            for (int nIdx = 1; nIdx <= 5; nIdx++)
            {
                listItems.Add(new string[] { "Row", $"아날로그 입력 (AI {nIdx})", "미시험" });
            }

            listItems.Add(new string[] { "Section", "  1.4 아날로그 출력 (AO)" });
            listItems.Add(new string[] { "Header", "시험 항목", "판정" });
            for (int nIdx = 1; nIdx <= 5; nIdx++)
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

        private void BtnClose_Click_2(object sender, EventArgs e)
        {
            Close();
        }
    }
}