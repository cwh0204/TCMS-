using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CITester.FormLoad;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CITester
{
    public partial class FormDB : Form
    {
        OleDbCommand m_OLECommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// 
        private DialogResult ShowMsg(string enMsg, string koMsg, string enTitle = "Notification", string koTitle = "알림", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            bool isEng = GlobalSettings.strLanguage.StartsWith("en");
            string msg = isEng ? enMsg : koMsg;
            string title = isEng ? enTitle : koTitle;
            return MessageBox.Show(msg, title, buttons, icon);
        }
        public FormDB(OleDbCommand command)
        {
            InitializeComponent();

            m_OLECommand = command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void FormDB_Load(object sender, EventArgs e)
        {
            ListAllTester();
            ListAllSerial();

        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        private void ListAllTester()
        {
            try
            {
                m_OLECommand.CommandText = "Select * From Tester";

                using (OleDbDataReader rows = m_OLECommand.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rows);
                    dataGridView_Tester.DataSource = dt;
                    if (dataGridView_Tester.Columns.Count >= 4)
                    {
                        dataGridView_Tester.Columns[0].HeaderText = "시험자명";
                        dataGridView_Tester.Columns[1].HeaderText = "부서";
                        dataGridView_Tester.Columns[2].HeaderText = "사원번호";
                        dataGridView_Tester.Columns[3].Visible = false;
                    }

                    DataGridViewSetup(dataGridView_Tester);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ListAllTester Error: {ex.Message}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        private void ListAllSerial()
        {
            try
            {
                m_OLECommand.CommandText = "Select * From Serial";

                using (OleDbDataReader rows = m_OLECommand.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rows);
                    dataGridView_Serial.DataSource = dt;
                    if (dataGridView_Serial.Columns.Count >= 3)
                    {
                        dataGridView_Serial.Columns[0].HeaderText = "편성번호";
                        dataGridView_Serial.Columns[1].HeaderText = "차량번호";
                        dataGridView_Serial.Columns[2].HeaderText = "일련번호";
                    }

                    DataGridViewSetup(dataGridView_Serial);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ListAllSerial Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strNo"></param>
        /// <returns></returns>
        /// 
        private void DataGridViewSetup(DataGridView dgv)
        {
            // 공통 동작 및 제한 설정
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToResizeRows = false;

            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 13F, FontStyle.Bold); // 머릿말 폰트 (10pt, 굵게)
            dgv.DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 11F, FontStyle.Regular);

            // ── 3. 헤더(Header) 크기 및 정렬 설정 ──
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 40; // 시원하게 보이도록 40으로 추천 (원하시면 35로 변경 가능)
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // ── 4. 테마 스타일 및 클릭(선택) 시 색상 유지 설정 ──
            dgv.EnableHeadersVisualStyles = false;

            // 헤더 셀 스타일 지정 (요청 사양 색상)
            System.Drawing.Color headerBg = System.Drawing.Color.FromArgb(240, 244, 253);
            System.Drawing.Color headerFg = System.Drawing.Color.FromArgb(8, 31, 78);

            dgv.ColumnHeadersDefaultCellStyle.BackColor = headerBg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = headerFg;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBg;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = headerFg;

            // ── 5. 정렬 기능 끄기 및 행 높이 설정 틀 적용 ──
            dgv.RowTemplate.Height = 40; // 앞으로 추가될 데이터 행 높이 고정 (35~40 추천)

            // 현재 생성된 열들의 정렬 기능 차단
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // 현재 이미 생성되어 있는 행들의 높이 강제 변경
            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Height = 40;
            }
        }
        private bool CheckRegistedTester(string strNo)
        {
            try
            {
                m_OLECommand.CommandText = "Select * From Tester Where TesterNo = '" + strNo + "'";

                using (OleDbDataReader rows = m_OLECommand.ExecuteReader())
                {
                    if (rows.HasRows)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CheckRegistedTester Error: {ex.Message}");
                return true;
            }

            return false; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strModel"></param>
        /// <param name="strGroupNo"></param>
        /// <param name="strTrainNo"></param>
        /// <param name="strSerialNo"></param>
        /// <returns></returns>
        /// 
        private bool CheckRegistedSerial(string strGroupNo, string strTrainNo, string strSerialNo)
        {
            try
            {
                m_OLECommand.CommandText = "Select * From Serial Where GroupNo = '" + strGroupNo + "' and TrainNo = '" + strTrainNo + "' and SerialNo = '" + strSerialNo + "'";

                OleDbDataReader rows = m_OLECommand.ExecuteReader();
                if (rows.HasRows)
                {
                    rows.Close();
                    return true;
                }
                rows.Close();

            }
            catch (Exception ex)
            {
                ShowMsg($"A database error has occurred.\n{ex.Message}", $"데이터베이스 오류가 발생했습니다.\n{ex.Message}", "Database Error", "DB 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        /*      private void BtnTesterAdd_Click(object sender, EventArgs e)
              {
                  if (TextBox_Tester.Text == "")
                  {
                      ShowMsg("Please enter the tester's name.", "시험자명을 입력해 주세요.");
                      return;
                  }
                  if (TextBox_ID.Text == "")
                  {
                      ShowMsg("Please enter the tester's ID number.", "시험자의 사원번호를 입력해 주세요.");
                      return;
                  }
                  if (TextBox_Department.Text == "")
                  {
                      ShowMsg("Please enter the tester's department.", "시험자의 부서를 입력해 주세요.");
                      return;
                  }

                  TextBox_Password.Text = "0";

                  if (TextBox_Password.Text == "")
                  {
                      ShowMsg("Please enter the tester's password.", "시험자의 비밀번호를 입력해 주세요.");
                      return;
                  }
                  try
                  {
                      if (CheckRegistedTester(TextBox_ID.Text.Trim()))
                      {
                          ShowMsg("This tester information is already registered in the database.", "이 시험자 정보는 이미 데이터베이스에 등록되어 있습니다.");
                          return;
                      }

                      //m_OLECommand.CommandText = "Insert Into Tester(Tester,Department) Values('" + TextBox_Tester.Text + "','" + TextBox_Department.Text + "')";
                      m_OLECommand.CommandText = "Insert Into Tester Values('" +
                                                         TextBox_Tester.Text.Trim() + "','" +
                                                         TextBox_Department.Text.Trim() + "','" +
                                                         TextBox_ID.Text.Trim() + "','" +
                                                         TextBox_Password.Text.Trim() + "')";
                      m_OLECommand.ExecuteNonQuery();

                      ListAllTester();

                      TextBox_Tester.Text = "";
                      TextBox_ID.Text = "";
                      TextBox_Department.Text = "";
                  }
                  catch (Exception ex)
                  {
                      ShowMsg($"A database error has occurred.\n{ex.Message}", $"데이터베이스 오류가 발생했습니다.\n{ex.Message}", "Database Error", "DB 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                  }
              }
        */
        private void BtnTesterAdd_Click(object sender, EventArgs e)
        {
            if (TextBox_Tester.Text.Trim() == "")
            {
                ShowMsg("Please enter the tester's name.", "시험자명을 입력해 주세요.");
                return;
            }
            if (TextBox_ID.Text.Trim() == "")
            {
                ShowMsg("Please enter the tester's ID number.", "시험자의 사원번호를 입력해 주세요.");
                return;
            }
            if (TextBox_Department.Text.Trim() == "")
            {
                ShowMsg("Please enter the tester's department.", "시험자의 부서를 입력해 주세요.");
                return;
            }

            TextBox_Password.Text = "0";

            try
            {
                string inputName = TextBox_Tester.Text.Trim();
                string inputID = TextBox_ID.Text.Trim();
                string inputDept = TextBox_Department.Text.Trim();
                string inputPw = TextBox_Password.Text.Trim();

                m_OLECommand.CommandText = "Select Count(*) From Tester Where Tester = '" + inputName + "'";

                int duplicateCount = Convert.ToInt32(m_OLECommand.ExecuteScalar());

                if (duplicateCount > 0)
                {
                    ShowMsg("The tester's name is already registered in the database.",
                            "해당 시험자명은 이미 데이터베이스에 등록되어 있습니다..");
                    return; 
                }

                m_OLECommand.CommandText = "Insert Into Tester Values('" +
                                           inputName + "','" +
                                           inputDept + "','" +
                                           inputID + "','" +
                                           inputPw + "')";

                m_OLECommand.ExecuteNonQuery();

                ListAllTester();

                TextBox_Tester.Text = "";
                TextBox_ID.Text = "";
                TextBox_Department.Text = "";
            }
            catch (Exception ex)
            {
                ShowMsg($"A database error has occurred.\n{ex.Message}",
                        $"데이터베이스 오류가 발생했습니다.\n{ex.Message}",
                        "Database Error", "DB 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnTesterDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView_Tester.CurrentRow == null)
            {
                ShowMsg("Please select a tester to delete from the list.", "목록에서 삭제할 시험자를 선택해 주세요.");
                return;
            }

            DataGridViewRow selectedRow = dataGridView_Tester.CurrentRow;

            string testerName = selectedRow.Cells[0].Value?.ToString().Trim() ?? "";
            string testerNo = selectedRow.Cells[2].Value?.ToString().Trim() ?? ""; 

            System.Diagnostics.Debug.WriteLine($"삭제 시도 데이터 -> 이름: [{testerName}], 사번: [{testerNo}]");

            try
            {
                m_OLECommand.CommandText = "Delete From Tester Where Tester = '" + testerName + "' and TesterNo = '" + testerNo + "'";

                int affectedRows = m_OLECommand.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    ShowMsg("No matching data found to delete.", "조건에 일치하는 데이터가 없어 삭제되지 않았습니다.\n데이터 매핑을 확인하세요.", "Warning", "삭제 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TextBox_Tester.Text = "";
                TextBox_ID.Text = "";
                TextBox_Department.Text = "";
                TextBox_Password.Text = "";
            }
            catch (Exception ex)
            {
                ShowMsg($"A database error has occurred.\n{ex.Message}",
                        $"데이터베이스 오류가 발생했습니다.\n{ex.Message}",
                        "Database Error", "DB 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 목록 새로고침
            ListAllTester();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnTesterPassword_Click(object sender, EventArgs e)
        {
            /*System.Windows.Forms.ListView.SelectedListViewItemCollection select = ListView_Tester.SelectedItems;
            if (select.Count == 0)
            {
                MessageBox.Show("시험자명 목록에서 비밀번호 변경할 시험자를 선택하십시오.");
                return;
            }

            FormPassword frmPassword = new FormPassword();
            frmPassword.TESTER = select[0].SubItems[0].Text;
            frmPassword.ID = select[0].SubItems[1].Text;
            frmPassword.CURRENT_PASSWORD = select[0].SubItems[3].Text;
            if (frmPassword.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_OLECommand.CommandText = "Update Tester Set PW = '" + frmPassword.NEW_PASSWORD +
                           "' Where Tester = '" + select[0].SubItems[0].Text + "' and TesterNo = '" + select[0].SubItems[1].Text + "'";
                m_OLECommand.ExecuteNonQuery();
                MessageBox.Show("사용자 [" + select[0].SubItems[0].Text + "]의 비밀번호를 수정했습니다.");

                ListAllTester();
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_Serial_Add_Click(object sender, EventArgs e)
        {
            if (TextBox_GroupNo.Text == "")
            {
                ShowMsg("Please enter the Trainset Number to register.", "등록할 편성번호를 입력해 주세요.");
                return;
            }
            if (TextBox_TrainNo.Text == "")
            {
                ShowMsg("Please enter the Car Number to register.", "등록할 차량번호를 입력해 주세요.");
                return;
            }
            if (TextBox_SerialNo.Text == "")
            {
                ShowMsg("Please enter the Serial Number to register.", "등록할 일련번호를 입력해 주세요.");
                return;
            }
            if (TextBox_GroupNo.Text.Length != 3)
            {
                ShowMsg("Please enter a 3-digit number for the Trainset Number.", "편성번호는 3자리 숫자로 입력해 주세요.");
                return;
            }
            if (TextBox_TrainNo.Text.Length != 4)
            {
                ShowMsg("Please enter a 4-digit number for the Car Number.", "차량번호는 4자리 숫자로 입력해 주세요.");
                return;
            }
            int nInput = 0;
            try
            {
                nInput = int.Parse(TextBox_GroupNo.Text);
            }
            catch
            {
                ShowMsg("Please enter a 3-digit number for the Trainset Number.", "편성번호는 숫자(3자리)만 입력 가능합니다.");
                return;
            }
            try
            {
                nInput = int.Parse(TextBox_TrainNo.Text);
            }
            catch
            {
                ShowMsg("Please enter a 4-digit number for the Car Number.", "차량번호는 숫자(4자리)만 입력 가능합니다.");
                return;
            }

            try
            {
                if (CheckRegistedSerial(TextBox_GroupNo.Text, TextBox_TrainNo.Text, TextBox_SerialNo.Text))
                {
                    ShowMsg("This C/I information is already registered in the database.", "해당 C/I 정보는 이미 데이터베이스에 등록되어 있습니다.");
                    return;
                }

                m_OLECommand.CommandText = "Insert Into Serial(GroupNo,TrainNo,SerialNo) Values('" + TextBox_GroupNo.Text +
                                             "','" + TextBox_TrainNo.Text + "','" + TextBox_SerialNo.Text + "')";
                m_OLECommand.ExecuteNonQuery();

                ListAllSerial();
            }
            catch (Exception ex)
            {
                ShowMsg($"A database error has occurred.\n{ex.Message}", $"데이터베이스 오류가 발생했습니다.\n{ex.Message}", "Database Error", "DB 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_Serial_Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView_Serial.CurrentRow == null)
            {
                ShowMsg("Please select an item to delete from the information list.", "목록에서 삭제할 항목을 선택해 주세요.");
                return;
            }

            DataGridViewRow selectedRow = dataGridView_Serial.CurrentRow;

            string groupNo = selectedRow.Cells[0].Value?.ToString() ?? "";
            string trainNo = selectedRow.Cells[1].Value?.ToString() ?? "";
            string serialNo = selectedRow.Cells[2].Value?.ToString() ?? "";

            // 2. 삭제 확인 메시지 출력 (일련번호 기준 안내)
            if (ShowMsg($"Are you sure you want to delete the information [{serialNo}]?",
                        $"정보 [{serialNo}]을(를) 삭제하시겠습니까?",
                        "Confirm Delete", "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            try
            {
                m_OLECommand.CommandText = "Delete From Serial Where GroupNo = '" + groupNo +
                                           "' and TrainNo = '" + trainNo +
                                           "' and SerialNo = '" + serialNo + "'";

                m_OLECommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowMsg($"A database error has occurred.\n{ex.Message}",
                        $"데이터베이스 오류가 발생했습니다.\n{ex.Message}",
                        "Database Error", "DB 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ListAllSerial();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        private void dataGridView_Serial_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView_Serial.Rows[e.RowIndex];

            TextBox_GroupNo.Text = row.Cells[0].Value?.ToString() ?? "";
            TextBox_TrainNo.Text = row.Cells[1].Value?.ToString() ?? "";
            TextBox_SerialNo.Text = row.Cells[2].Value?.ToString() ?? "";
        }
        private void dataGridView_Tester_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView_Tester.Rows[e.RowIndex];

            TextBox_Tester.Text = row.Cells[0].Value?.ToString() ?? "";
            TextBox_ID.Text = row.Cells[2].Value?.ToString() ?? "";
            TextBox_Department.Text = row.Cells[1].Value?.ToString() ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string strWhere = "";

            if (CheckBox_GroupNo.Checked == true && TextBox_Search_GroupNo.Text == "")
            {
                ShowMsg("Please enter the Trainset Number to search.", "조회할 편성번호를 입력해 주세요.");
                return;
            }
            if (CheckBox_TrainNo.Checked == true && TextBox_Search_TrainNo.Text == "")
            {
                ShowMsg("Please enter the Car Number to search.", "조회할 차량번호를 입력해 주세요.");
                return;
            }
            if (CheckBox_SerialNo.Checked == true && TextBox_Search_SerialNo.Text == "")
            {
                ShowMsg("Please enter the Serial Number to search.", "조회할 번호를 입력해 주세요.");
                return;
            }

            m_OLECommand.CommandText = "Select * From Serial";

            if (CheckBox_GroupNo.Checked == true)
            {
                if (strWhere == "")
                    strWhere = " where GroupNo LIKE '%" + TextBox_Search_GroupNo.Text + "%'";
                else
                    strWhere += " and GroupNo LIKE '%" + TextBox_Search_GroupNo.Text + "%'";
            }
            if (CheckBox_TrainNo.Checked == true)
            {
                if (strWhere == "")
                    strWhere = " where TrainNo LIKE '%" + TextBox_Search_TrainNo.Text + "%'";
                else
                    strWhere += " and TrainNo LIKE '%" + TextBox_Search_TrainNo.Text + "%'";
            }
            if (CheckBox_SerialNo.Checked == true)
            {
                if (strWhere == "")
                    strWhere = " where SerialNo LIKE '%" + TextBox_Search_SerialNo.Text + "%'";
                else
                    strWhere += " and SerialNo LIKE '%" + TextBox_Search_SerialNo.Text + "%'";
            }

            if (strWhere != "")
                m_OLECommand.CommandText += strWhere;

            try
            {
                using (OleDbDataReader rows = m_OLECommand.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rows);
                    dataGridView_Serial.DataSource = dt;

                    if (dataGridView_Serial.Columns.Count >= 3)
                    {
                        dataGridView_Serial.Columns[0].HeaderText = "편성번호";
                        dataGridView_Serial.Columns[1].HeaderText = "차량번호";
                        dataGridView_Serial.Columns[2].HeaderText = "일련번호";
                    }
                    DataGridViewSetup(dataGridView_Serial);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BtnSearch_Click Error: {ex.Message}");
            }
        }

        private void TextBox_Search_GroupNo_Click(object sender, EventArgs e)
        {
            TextBox_Search_GroupNo.Text = "";
        }

        private void TextBox_Search_TrainNo_Click(object sender, EventArgs e)
        {
            TextBox_Search_TrainNo.Text = "";
        }

        private void TextBox_Search_SerialNo_Click(object sender, EventArgs e)
        {
            TextBox_Search_SerialNo.Text = "";
        }

        private void TextBox_Search_GroupNo_Leave(object sender, EventArgs e)
        {
            TextBox_Search_GroupNo.Text = "편성번호 입력";
        }
    }
}
