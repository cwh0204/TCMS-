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

namespace CITester
{
    public partial class FormTester : Form
    {
        OleDbCommand m_OLECommand;

        public string m_strTester = "";
        public string m_strID = "";
        public string m_strDepartment = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// 
        public FormTester(OleDbCommand command)
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
        private void FormTester_Load(object sender, EventArgs e)
        {
            try
            {
                m_OLECommand.CommandText = "Select * From Tester";

                OleDbDataReader rows = m_OLECommand.ExecuteReader();

                ListView_Tester.Items.Clear();
                while (rows.Read())
                {
                    ListViewItem item = new ListViewItem(rows[0].ToString());
                    item.SubItems.Add(rows[2].ToString());
                    item.SubItems.Add(rows[1].ToString());
                    ListView_Tester.Items.Add(item);
                }
                rows.Close();
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnOK_Click(object sender, EventArgs e)
        {
            bool isEnglish = GlobalSettings.strLanguage.StartsWith("en");

            if (string.IsNullOrEmpty(TextBox_Tester.Text))
            {
                string msg = isEnglish
                    ? "No tester selected. Please select a tester first."
                    : "시험자가 선택되지 않았습니다. 먼저 시험자를 선택하십시오.";

                string title = isEnglish ? "Notification" : "알림";

                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ConfigJson.CurrentConfig.Operation.TesterName = TextBox_Tester.Text;

            this.DialogResult = DialogResult.OK;
            Close();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ListView_Tester_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection select = ListView_Tester.SelectedItems;
            if (select.Count > 0)
            {
                TextBox_Tester.Text = select[0].SubItems[0].Text;
                TextBox_ID.Text = select[0].SubItems[1].Text;
                TextBox_Department.Text = select[0].SubItems[2].Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ListView_Tester_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView.SelectedListViewItemCollection select = ListView_Tester.SelectedItems;
            if (select.Count > 0)
            {
                m_strTester = select[0].SubItems[0].Text;
                m_strID = select[0].SubItems[1].Text;
                m_strDepartment = select[0].SubItems[2].Text;

                this.DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
