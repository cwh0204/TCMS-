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
    public partial class FormSerial : Form
    {
        OleDbCommand m_OLECommand;

        public string m_strGroupNo;
        public string m_strTrainNo;
        public string m_strSerialNo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// 
        public FormSerial(OleDbCommand command)
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
        private void FormSerial_Load(object sender, EventArgs e)
        {
            ListAllSerial();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        private void ListAllSerial()
        {
            m_OLECommand.CommandText = "Select * From Serial";

            try
            {
                OleDbDataReader rows = m_OLECommand.ExecuteReader();

                ListView_Serial.Items.Clear();
                while (rows.Read())
                {
                    ListViewItem item = new ListViewItem(rows[0].ToString());
                    item.SubItems.Add(rows[1].ToString());
                    item.SubItems.Add(rows[2].ToString());

                    ListView_Serial.Items.Add(item);
                }
                rows.Close();
            }
            catch
            {
            }
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
            bool isEnglish = GlobalSettings.strLanguage.StartsWith("en");
            string title = isEnglish ? "Notification" : "알림";

            if (CheckBox_GroupNo.Checked && string.IsNullOrEmpty(TextBox_Search_GroupNo.Text))
            {
                string msg = isEnglish
                    ? "Please enter the Trainset number of the battery to search."
                    : "조회할 배터리의 편성번호를 입력하십시오.";

                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CheckBox_TrainNo.Checked && string.IsNullOrEmpty(TextBox_Search_TrainNo.Text))
            {
                string msg = isEnglish
                    ? "Please enter the Car number of the battery to search."
                    : "조회할 배터리의 차량번호를 입력하십시오.";

                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CheckBox_SerialNo.Checked && string.IsNullOrEmpty(TextBox_Search_SerialNo.Text))
            {
                string msg = isEnglish
                    ? "Please enter the Serial number of the battery to search."
                    : "조회할 배터리의 일련번호를 입력하십시오.";

                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            m_OLECommand.CommandText = "Select * From Serial";
            if (CheckBox_GroupNo.Checked == true)
            {
                if (strWhere == "")
                    strWhere = " where GroupNo LIKE '%%" + TextBox_Search_GroupNo.Text + "%%'";
                else
                    strWhere += " and GroupNo LIKE '%%" + TextBox_Search_GroupNo.Text + "%%'";
            }
            if (CheckBox_TrainNo.Checked == true)
            {
                if (strWhere == "")
                    strWhere = " where TrainNo LIKE '%%" + TextBox_Search_TrainNo.Text + "%%'";
                else
                    strWhere += " and TrainNo LIKE '%%" + TextBox_Search_TrainNo.Text + "%%'";
            }
            if (CheckBox_SerialNo.Checked == true)
            {
                if (strWhere == "")
                    strWhere = " where SerialNo LIKE '%%" + TextBox_Search_SerialNo.Text + "%%'";
                else
                    strWhere += " and SerialNo LIKE '%%" + TextBox_Search_SerialNo.Text + "%%'";
            }
            if (strWhere != "")
                m_OLECommand.CommandText += strWhere;

            OleDbDataReader rows = m_OLECommand.ExecuteReader();

            ListView_Serial.Items.Clear();
            while (rows.Read())
            {
                ListViewItem item = new ListViewItem(rows[0].ToString());
                item.SubItems.Add(rows[1].ToString());
                item.SubItems.Add(rows[2].ToString());

                ListView_Serial.Items.Add(item);
            }
            rows.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ListView_Serial_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection select = ListView_Serial.SelectedItems;
            if (select.Count > 0)
            {
                TextBox_GroupNo.Text = select[0].SubItems[0].Text;
                TextBox_TrainNo.Text = select[0].SubItems[1].Text;
                TextBox_SerialNo.Text = select[0].SubItems[2].Text;

                m_strGroupNo = select[0].SubItems[0].Text;
                m_strTrainNo = select[0].SubItems[1].Text;
                m_strSerialNo = select[0].SubItems[2].Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ListView_Serial_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView.SelectedListViewItemCollection select = ListView_Serial.SelectedItems;
            if (select.Count > 0)
            {
                m_strGroupNo = select[0].SubItems[1].Text;
                m_strTrainNo = select[0].SubItems[2].Text;
                m_strSerialNo = select[0].SubItems[3].Text;

                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            ConfigJson.CurrentConfig.Operation.FleetNo = TextBox_GroupNo.Text;
            ConfigJson.CurrentConfig.Operation.TrainNo = TextBox_TrainNo.Text;
            ConfigJson.CurrentConfig.Operation.SerialNo = TextBox_SerialNo.Text;

            this.DialogResult = DialogResult.OK;
            Close();

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
