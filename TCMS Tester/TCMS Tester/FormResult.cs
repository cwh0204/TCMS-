using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static CITester.FormLoad;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CITester
{
    public partial class FormResult : Form
    {
        private bool IsEng => GlobalSettings.strLanguage.StartsWith("en");
        public OleDbCommand m_OLECommand;

        int m_nPage = 0;
        int m_nTotalPage = 0;
        int m_nPrintItemCount = 0;

        bool m_bOnlyStandardResult = true;

        private PrintDocument m_printDoc = new PrintDocument();
        private PageSettings m_pgSettings = new PageSettings();
        private PrinterSettings m_prtSettings = new PrinterSettings();

        ResultData m_Result = new ResultData();
        ConfigData m_Config = null;

        int m_nLinePerItem = 0;
        int m_nItemPerPage = 0;


        /// <summary>
        /// 
        /// </summary>
        /// 
        public FormResult()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="config"></param>
        /// 
        public FormResult(OleDbCommand command, ConfigData config)
        {
            InitializeComponent();

            m_OLECommand = command;
            m_Config = config;

            m_printDoc.PrintPage += new PrintPageEventHandler(PrintDoc_PrintPage);

        }

        private void FormResult_Load(object sender, EventArgs e)
        {
            DateTime_From.Value = DateTime.Now.AddMonths(-1);
            DateTime_From.MaxDate = DateTime.Now;
            DateTime_From.Format = DateTimePickerFormat.Short;
            EnableDoubleBuffered(dataGridView_Search);
            DataGridViewSetup(dataGridView_Search);
            InitializeCustomScrollBars();
        }
        // 리스트뷰 정렬을 담당하는 클래스
        public class ListViewItemComparer : System.Collections.IComparer
        {
            private int col;
            private SortOrder order;

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                int returnVal = -1;
                // 텍스트 비교
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                         ((ListViewItem)y).SubItems[col].Text);

                // 내림차순일 경우 결과 반전
                if (order == SortOrder.Descending)
                    returnVal *= -1;

                return returnVal;
            }
        }

        private void DataGridView_Search_Paint(object sender, PaintEventArgs e)
        {
            if (sender is DataGridView dgv)
            {
                bool bIsEmpty = true;
                if (dgv.Rows.Count > 0 && dgv.Rows[0].Cells[0].Value != null)
                {
                    string strValue = dgv.Rows[0].Cells[0].Value.ToString();
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        bIsEmpty = false;
                    }
                }

                if (bIsEmpty)
                {
                    Graphics gGraphics = e.Graphics;

                    Image imgIcon = TCMSTester.Properties.Resources.inbox;
                    if (imgIcon != null) 
                    {
                        gGraphics.DrawImage(imgIcon, 454, 150, 48, 48);
                        // }

                        // ── [B. 안내 글자(Text) 그리기 영역] ──
                        string strNotice = IsEng
                            ? "Please select search criteria and click the Search button."
                            : "검색 조건을 선택 후 검색 버튼을 클릭하세요.";

                        Color colorText = Color.FromArgb(156, 159, 166);

                        using (Font fontNotice = new Font("맑은 고딕", 12F, FontStyle.Bold))
                        using (SolidBrush brushText = new SolidBrush(colorText))
                        {
                            gGraphics.DrawString(strNotice, fontNotice, brushText, new PointF(321, 209));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void DataGridViewSetup(DataGridView dgv)
        {
            // 공통 동작 및 제한 설정
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToResizeRows = false;

            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 13F, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 11F, FontStyle.Regular);

            // 헤더(Header) 크기 및 정렬 설정
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 35;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 테마 스타일 및 클릭(선택) 시 색상 유지 설정
            dgv.EnableHeadersVisualStyles = false;
           　
            System.Drawing.Color headerBg = System.Drawing.Color.FromArgb(240, 244, 253);
            System.Drawing.Color headerFg = System.Drawing.Color.FromArgb(8, 31, 78);

            dgv.ColumnHeadersDefaultCellStyle.BackColor = headerBg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = headerFg;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBg;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = headerFg;

            if (dgv.Columns.Count == 0)
            {
                // 고성능 바인딩을 위한 빈 데이터 테이블 객체 생성
                DataTable dtDummy = new DataTable();
                dtDummy.Columns.Add("TestDate");
                dtDummy.Columns.Add("Tester");
                dtDummy.Columns.Add("Unit");
                dtDummy.Columns.Add("SerialNo");
                dtDummy.Columns.Add("GroupNo");
                dtDummy.Columns.Add("TrainNo");

                // 데이터를 불러오기 전에 화면에 그리드라인 행을 미리 추가합니다.
                int nMaxEmptyLines = 13;
                for (int i = 0; i < nMaxEmptyLines; i++)
                {
                    dtDummy.Rows.Add(dtDummy.NewRow());
                }

                // 데이터그리드뷰에 빈 규격 테이블을 선 바인딩
                dgv.DataSource = dtDummy;

                // 화면 표기용 한글 머릿말 매핑 고정
                dgv.Columns["TestDate"].HeaderText = "시험일자";
                dgv.Columns["Tester"].HeaderText = "시험자";
                dgv.Columns["Unit"].HeaderText = "유닛구분";
                dgv.Columns["SerialNo"].HeaderText = "일련번호";
                dgv.Columns["GroupNo"].HeaderText = "편성번호";
                dgv.Columns["TrainNo"].HeaderText = "차량번호";

                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.DataPropertyName = col.Name;
                }

                // 정렬 기능 끄기 및 행 높이 설정 틀 적용
                dgv.RowTemplate.Height = 29;
                dgv.AllowUserToResizeColumns = false;
                dgv.Columns[0].Width = 200;
                // 현재 생성된 열들의 정렬 기능 차단
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                // 현재 이미 생성되어 있는 행들의 높이 강제 변경
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    row.Height = 29;
                }
            }
            dgv.ClearSelection();
            // ★ [추가] 이벤트 중복 등록을 방지하기 위해 해제 후 다시 연결합니다.
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }
        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is DataGridView dgv && dgv.CurrentRow != null)
            {
                if (dgv.Columns.Contains("TestDate"))
                {
                    string strDate = dgv.CurrentRow.Cells["TestDate"].Value?.ToString();

                    // 측정 일자 데이터가 비어있다면 가짜 행이므로 선택 상태를 해제합니다.
                    if (string.IsNullOrEmpty(strDate))
                    {
                        dgv.ClearSelection();
                    }
                }
            }
        }
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            SearchAndDisplayResult();
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        private void SearchAndDisplayResult()
        {
            // 입력 유효성 검사 (필수값 체크)
            if (CheckBox_Tester.Checked && string.IsNullOrEmpty(TextBox_Search_Tester.Text.Trim()))
            {
                MessageBox.Show(IsEng ? "Please enter the Tester Name." : "시험자명을 입력하세요.");
                return;
            }
            if (CheckBox_GroupNo.Checked && string.IsNullOrEmpty(TextBox_Search_GroupNo.Text.Trim()))
            {
                MessageBox.Show(IsEng ? "Please enter the Trainset No." : "편성번호를 입력하세요.");
                return;
            }
            if (CheckBox_TrainNo.Checked && string.IsNullOrEmpty(TextBox_Search_TrainNo.Text.Trim()))
            {
                MessageBox.Show(IsEng ? "Please enter the Car No." : "차량번호를 입력하세요.");
                return;
            }
            if (CheckBox_SerialNo.Checked && string.IsNullOrEmpty(TextBox_Search_SerialNo.Text.Trim()))
            {
                MessageBox.Show(IsEng ? "Please enter the Serial No." : "일련번호를 입력하세요.");
                return;
            }

            // 데이터그리드뷰 초기화
            dataGridView_Search.DataSource = null;

            try
            {
                // JSON 매니저를 통한 전체 헤더 리스트 로드
                TestResultManager objResultManager = new TestResultManager();
                List<TestResultJson.ResultHeaderInfo> listAllHeaders = objResultManager.LoadAllResultHeaders();
                List<TestResultJson.ResultHeaderInfo> listFilteredHeaders = new List<TestResultJson.ResultHeaderInfo>();

                // 검색 조건 필터링 (메모리 내 LINQ / 조건 검색 인터프리터 구현)
                foreach (TestResultJson.ResultHeaderInfo objHeader in listAllHeaders)
                {
                    if (objHeader == null) continue;

                    // 기간 검색 필터링
                    if (CheckBox_Period.Checked)
                    {
                        if (DateTime.TryParse(objHeader.TestDateTime, out DateTime dtTestTime))
                        {
                            if (DateTime.TryParse(DateTime_From.Text, out DateTime dtFrom) &&
                                DateTime.TryParse(DateTime_To.Text, out DateTime dtTo))
                            {
                                // 시작일 00:00:00 ~ 종료일 23:59:59 범위 검증
                                DateTime dtFromLimit = dtFrom.Date;
                                DateTime dtToLimit = dtTo.Date.AddDays(1).AddTicks(-1);

                                if (dtTestTime < dtFromLimit || dtTestTime > dtToLimit) continue;
                            }
                        }
                    }

                    // 시험자명 정확도 필터링
                    if (CheckBox_Tester.Checked)
                    {
                        if (objHeader.TesterName.Trim() != TextBox_Search_Tester.Text.Trim()) continue;
                    }

                    // 편성번호 부분 일치 필터링 (LIKE 구현)
                    if (CheckBox_GroupNo.Checked)
                    {
                        if (!objHeader.FleetNo.Contains(TextBox_Search_GroupNo.Text.Trim())) continue;
                    }

                    // 차량번호 부분 일치 필터링
                    if (CheckBox_TrainNo.Checked)
                    {
                        if (!objHeader.TrainNo.Contains(TextBox_Search_TrainNo.Text.Trim())) continue;
                    }

                    // 일련번호 부분 일치 필터링
                    if (CheckBox_SerialNo.Checked)
                    {
                        if (!objHeader.SerialNo.Contains(TextBox_Search_SerialNo.Text.Trim())) continue;
                    }

                    listFilteredHeaders.Add(objHeader);
                }

                // Grid 바인딩용 DataTable 스키마 생성 및 데이터 적재
                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("TestDate");
                dtResult.Columns.Add("Tester");
                dtResult.Columns.Add("Unit");
                dtResult.Columns.Add("SerialNo");
                dtResult.Columns.Add("GroupNo");
                dtResult.Columns.Add("TrainNo");

                int nRealDataCount = listFilteredHeaders.Count;
                int nMaxEmptyLines = 13;

                // 정렬: 날짜 기준 오름차순 정렬
                listFilteredHeaders.Sort((x, y) => string.Compare(x.TestDateTime, y.TestDateTime));

                // 데이터 채우기 및 다국어 시간 포맷 인터프리터 반영
                foreach (TestResultJson.ResultHeaderInfo objHeader in listFilteredHeaders)
                {
                    DataRow objRow = dtResult.NewRow();

                    string strDisplayDate = objHeader.TestDateTime;
                    if (IsEng && DateTime.TryParse(strDisplayDate, out DateTime dtParsed))
                    {
                        // 영문 상태일 경우 AM/PM 규격의 포맷팅 강제 적용
                        strDisplayDate = dtParsed.ToString("yyyy-MM-dd tt hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    objRow["TestDate"] = strDisplayDate;
                    objRow["Tester"] = objHeader.TesterName;
                    objRow["Unit"] = objHeader.TCMSUnit;
                    objRow["SerialNo"] = objHeader.SerialNo;
                    objRow["GroupNo"] = objHeader.FleetNo;   // 맵핑 데이터 가시화 변경
                    objRow["TrainNo"] = objHeader.TrainNo;

                    dtResult.Rows.Add(objRow);
                }

                // 6. 빈 그리드 데이터 채우기 (포맷 유지 조건부 루프)
                if (nRealDataCount == 0)
                {
                    for (int nIdx = 0; nIdx < nMaxEmptyLines; nIdx++)
                    {
                        dtResult.Rows.Add(dtResult.NewRow());
                    }
                }
                else if (nRealDataCount < nMaxEmptyLines)
                {
                    int nDeficitCount = nMaxEmptyLines - nRealDataCount;
                    for (int nIdx = 0; nIdx < nDeficitCount; nIdx++)
                    {
                        dtResult.Rows.Add(dtResult.NewRow());
                    }
                }

                dataGridView_Search.DataSource = dtResult;

                // 7. 컬럼 머릿말 텍스트 제어 및 정렬 차단
                if (dataGridView_Search.Columns.Contains("TestDate")) dataGridView_Search.Columns["TestDate"].HeaderText = "시험일자";
                if (dataGridView_Search.Columns.Contains("Tester")) dataGridView_Search.Columns["Tester"].HeaderText = "시험자";
                if (dataGridView_Search.Columns.Contains("Unit")) dataGridView_Search.Columns["Unit"].HeaderText = "유닛구분";
                if (dataGridView_Search.Columns.Contains("SerialNo")) dataGridView_Search.Columns["SerialNo"].HeaderText = "일련번호";
                if (dataGridView_Search.Columns.Contains("GroupNo")) dataGridView_Search.Columns["GroupNo"].HeaderText = "편성번호";
                if (dataGridView_Search.Columns.Contains("TrainNo")) dataGridView_Search.Columns["TrainNo"].HeaderText = "차량번호";

                if (dataGridView_Search.Columns.Count > 0)
                {
                    dataGridView_Search.Columns[0].Width = 200;
                }

                foreach (DataGridViewColumn objCol in dataGridView_Search.Columns)
                {
                    objCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                foreach (DataGridViewRow objGridRow in dataGridView_Search.Rows)
                {
                    objGridRow.Height = 29;
                }

                // 8. 커스텀 VScrollBar 제어
                if (nRealDataCount <= nMaxEmptyLines)
                {
                    vscrollbarCustom.Visible = false;
                }
                else
                {
                    vscrollbarCustom.Visible = true;
                    vscrollbarCustom.Minimum = 0;
                    vscrollbarCustom.Maximum = nRealDataCount;
                    vscrollbarCustom.LargeChange = nMaxEmptyLines;
                    vscrollbarCustom.SmallChange = 1;
                    vscrollbarCustom.Value = 0;
                }

                if (nRealDataCount == 0)
                {
                    MessageBox.Show(IsEng ? "No test results found for the specified search criteria." : "검색 결과가 없습니다.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, IsEng ? "A file system reading error has occurred." : "파일 시스템 읽기 오류가 발생했습니다.");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Btn_View_Click(object sender, EventArgs e)
        {
            // DataGridView 선택 행 유효성 검사
            if (dataGridView_Search.CurrentRow == null || dataGridView_Search.CurrentRow.Index < 0)
            {
                MessageBox.Show(IsEng ? "Please select a test result to view." : "조회할 시험 결과 항목을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow objSelectedRow = dataGridView_Search.CurrentRow;

            // 빈 행(Dummy Line) 선택 방지를 위한 키값 검증
            string strUnit = objSelectedRow.Cells["Unit"].Value?.ToString() ?? string.Empty;
            string strSerialNo = objSelectedRow.Cells["SerialNo"].Value?.ToString() ?? string.Empty;
            string strTestDateTime = objSelectedRow.Cells["TestDate"].Value?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(strUnit) || string.IsNullOrWhiteSpace(strSerialNo) || string.IsNullOrWhiteSpace(strTestDateTime))
            {
                MessageBox.Show(IsEng ? "Invalid row selected." : "올바른 시험 결과 행을 선택해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 선택된 키정보를 바탕으로 상세 JSON 데이터 파일 로드
            TestResultManager objResultManager = new TestResultManager();
            TestResultJson objDetailResult = objResultManager.LoadDetailResultByHeader(strUnit, strSerialNo, strTestDateTime);

            if (objDetailResult == null)
            {
                MessageBox.Show(IsEng ? "Failed to load detailed test result file." : "상세 시험 결과 파일을 로드하지 못했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 생성자를 통해 상세 결과 객체를 전달하며 모달 폼 생성
            using (FormResultView frmResultView = new FormResultView(objDetailResult))
            {
                frmResultView.ShowDialog();
            }
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> listSelectedRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow objRow in dataGridView_Search.SelectedRows)
            {
                string strUnit = objRow.Cells["Unit"]?.Value?.ToString() ?? string.Empty;
                string strSerialNo = objRow.Cells["SerialNo"]?.Value?.ToString() ?? string.Empty;
                string strTestDateTime = objRow.Cells["TestDate"]?.Value?.ToString() ?? string.Empty;

                // 필수 키 식별 정보가 존재하는 유효 데이터 행만 삭제 리스트에 등록
                if (!string.IsNullOrWhiteSpace(strUnit) &&
                    !string.IsNullOrWhiteSpace(strSerialNo) &&
                    !string.IsNullOrWhiteSpace(strTestDateTime))
                {
                    listSelectedRows.Add(objRow);
                }
            }

            // 선택된 유효 데이터 항목이 없는 경우 경고 알림
            if (listSelectedRows.Count == 0)
            {
                string strMsg = IsEng ? "Please select item(s) from the list to delete." : "삭제할 항목을 선택하세요.";
                string strTitle = IsEng ? "Selection Required" : "선택 필요";

                MessageBox.Show(strMsg, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 삭제 실행 여부 사용자 최종 확인
            int nSelectCount = listSelectedRows.Count;
            string strConfirmMsg = IsEng
                ? $"{nSelectCount} item(s) selected. Are you sure you want to delete them?"
                : $"{nSelectCount}개의 항목이 선택되었습니다. 정말로 삭제하시겠습니까?";
            string strConfirmTitle = IsEng ? "Confirm Deletion" : "삭제 확인";

            if (MessageBox.Show(strConfirmMsg, strConfirmTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            // JSON 파일 저장 디렉토리 경로 정의
            string strResultDirPath = Path.Combine(Application.StartupPath, "TestResults");
            bool bHasError = false;

            // 선택된 행을 순회하며 해당하는 JSON 파일 탐색 및 물리 삭제
            foreach (DataGridViewRow objRow in listSelectedRows)
            {
                string strUnit = objRow.Cells["Unit"]?.Value?.ToString() ?? string.Empty;
                string strSerialNo = objRow.Cells["SerialNo"]?.Value?.ToString() ?? string.Empty;
                string strTestDateTime = objRow.Cells["TestDate"]?.Value?.ToString() ?? string.Empty;

                try
                {
                    if (Directory.Exists(strResultDirPath))
                    {
                        string[] arrFiles = Directory.GetFiles(strResultDirPath, "*.json");

                        foreach (string strFilePath in arrFiles)
                        {
                            string strJsonContent = File.ReadAllText(strFilePath);
                            TestResultJson objResult = JsonConvert.DeserializeObject<TestResultJson>(strJsonContent);

                            if (objResult?.Header != null)
                            {
                                // 선택된 행의 헤더 메타데이터(유닛, 일련번호, 시험일시)와 동일한 파일 식별
                                if (objResult.Header.TCMSUnit == strUnit &&
                                    objResult.Header.SerialNo == strSerialNo &&
                                    objResult.Header.TestDateTime == strTestDateTime)
                                {
                                    if (File.Exists(strFilePath))
                                    {
                                        File.Delete(strFilePath);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    bHasError = true;
                    string strErrorTitle = IsEng ? "Deletion Error" : "삭제 오류";
                    string strErrorMsg = IsEng
                        ? $"An error occurred while deleting item [{strTestDateTime}]: {ex.Message}"
                        : $"항목 [{strTestDateTime}] 삭제 중 오류가 발생했습니다: {ex.Message}";

                    MessageBox.Show(strErrorMsg, strErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!bHasError)
            {
                ShowMsg("Selected items have been deleted.", "선택한 항목이 삭제되었습니다.",
                        "Success", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // 데이터그리드뷰 재조회 및 결과 반영
            SearchAndDisplayResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private DialogResult ShowMsg(string enMsg, string koMsg, string enTitle = "Notification", string koTitle = "알림", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            bool isEng = GlobalSettings.strLanguage.StartsWith("en");
            return MessageBox.Show(isEng ? enMsg : koMsg, isEng ? enTitle : koTitle, buttons, icon);
        }
        private void Button_Print_Click(object sender, EventArgs e)
        {
            m_nPage = 0;
            m_printDoc.DefaultPageSettings = m_pgSettings;

            m_nPrintItemCount = 0;
            foreach (DataGridViewRow objRow in dataGridView_Search.Rows)
            {
                bool bIsChecked = Convert.ToBoolean(objRow.Cells[0].Value);
                if (bIsChecked)
                {
                    m_nPrintItemCount++;
                }
            }

            if (m_nPrintItemCount == 0)
            {
                MessageBox.Show("프린트할 측정 결과를 먼저 검색하십시오.");
                return;
            }

            /*
            m_nLinePerItem = 0;
            if (m_Config.bPrintAV == true) ++m_nLinePerItem;
            if (m_Config.bPrintRV == true) ++m_nLinePerItem;
            if (m_Config.bPrintAT == true) ++m_nLinePerItem; 
            if (m_Config.bPrintRT == true) ++m_nLinePerItem;
            if (m_Config.bPrintCR == true) ++m_nLinePerItem;
            m_nItemPerPage = 30 / m_nLinePerItem;
            */

            m_nTotalPage = m_nPrintItemCount / m_nItemPerPage;
            if ((m_nPrintItemCount % m_nItemPerPage) > 0)
            {
                ++m_nTotalPage;
            }

            /*
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.Document = m_printDoc;
            printDialog1.UseEXDialog = false;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                m_printDoc.Print();
            }
            */

            PrintPreviewDialog dlg = new PrintPreviewDialog();
            dlg.Document = m_printDoc;
            dlg.ClientSize = new System.Drawing.Size(1280, 1080);
            dlg.DesktopLocation = new Point(0, 0);
            ((Form)dlg).WindowState = FormWindowState.Maximized;
            dlg.ShowDialog();
        }
        private void PrintPage(PrintPageEventArgs e, bool bIsFirstPage)
        {
        }

        private void PrintDoc_PrintPage(Object sender, PrintPageEventArgs e)
        {
            ++m_nPage;
            if (m_nPage == 1)
                PrintPage(e, true);
            else
                PrintPage(e, false);


            if (m_nPage >= m_nTotalPage)
            {
                m_nPage = 0;
                e.HasMorePages = false;
            }
            else
            {
                e.HasMorePages = true;
            }
        }

        private void Btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void EnableDoubleBuffered(Control control)
        {
            PropertyInfo propertyInfo = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo.SetValue(control, true, null);
        }
        // 데이터그리드뷰와 커스텀 스크롤바 동기화 설정 함수
        private void InitializeCustomScrollBars()
        {
            try
            {
                dataGridView_Search.ScrollBars = ScrollBars.None;

                if (dataGridView_Search.RowCount > 0)
                {
                    // 설명형 주석: WinForms 스크롤바 공식(Max = 최대행 + 페이지크기 - 1)을 적용하여 범위 초과 오류 방지
                    vscrollbarCustom.LargeChange = dataGridView_Search.DisplayedRowCount(false);
                    vscrollbarCustom.Maximum = dataGridView_Search.RowCount + vscrollbarCustom.LargeChange - 1;
                }

                // 커스텀 수직 스크롤바 이벤트 연결
                vscrollbarCustom.Scroll += (sender, e) =>
                {
                    int nTargetIndex = e.NewValue;
                    if (nTargetIndex >= 0 && nTargetIndex < dataGridView_Search.RowCount)
                    {
                        dataGridView_Search.FirstDisplayedScrollingRowIndex = nTargetIndex;
                    }
                };

                // 데이터그리드뷰 마우스 휠 이벤트 연결 (최적화 버전)
                dataGridView_Search.MouseWheel += (sender, e) =>
                {
                    int nDeltaIndex = e.Delta > 0 ? -1 : 1;
                    int nNewIndex = dataGridView_Search.FirstDisplayedScrollingRowIndex + nDeltaIndex;

                    // 1차 검증: 데이터그리드뷰 행 범위 내에 있는지 확인
                    if (nNewIndex >= 0 && nNewIndex < dataGridView_Search.RowCount)
                    {
                        dataGridView_Search.FirstDisplayedScrollingRowIndex = nNewIndex;

                        // 2차 검증: 스크롤바가 가질 수 있는 실제 최대값 계산 (Maximum - LargeChange + 1)
                        int nScrollMaxLimit = vscrollbarCustom.Maximum - vscrollbarCustom.LargeChange + 1;

                        // 상하한 경계 예외 처리 후 안전하게 Value 대입
                        if (nNewIndex <= nScrollMaxLimit)
                        {
                            vscrollbarCustom.Value = nNewIndex;
                        }
                        else
                        {
                            vscrollbarCustom.Value = nScrollMaxLimit;
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Scrollbar Sync Error: {ex.Message}");
            }
        }

        private void dataGridView_Search_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow objSelectedRow = dataGridView_Search.CurrentRow;

            // 빈 행(Dummy Line) 선택 방지를 위한 키값 검증
            string strUnit = objSelectedRow.Cells["Unit"].Value?.ToString() ?? string.Empty;
            string strSerialNo = objSelectedRow.Cells["SerialNo"].Value?.ToString() ?? string.Empty;
            string strTestDateTime = objSelectedRow.Cells["TestDate"].Value?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(strUnit) || string.IsNullOrWhiteSpace(strSerialNo) || string.IsNullOrWhiteSpace(strTestDateTime))
            {
                MessageBox.Show(IsEng ? "Invalid row selected." : "올바른 시험 결과 행을 선택해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 선택된 키정보를 바탕으로 상세 JSON 데이터 파일 로드
            TestResultManager objResultManager = new TestResultManager();
            TestResultJson objDetailResult = objResultManager.LoadDetailResultByHeader(strUnit, strSerialNo, strTestDateTime);

            if (objDetailResult == null)
            {
                MessageBox.Show(IsEng ? "Failed to load detailed test result file." : "상세 시험 결과 파일을 로드하지 못했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 생성자를 통해 상세 결과 객체를 전달하며 모달 폼 생성
            using (FormResultView frmResultView = new FormResultView(objDetailResult))
            {
                frmResultView.ShowDialog();
            }
        }
    }
}
