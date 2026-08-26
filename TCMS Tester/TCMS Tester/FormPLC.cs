using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Spreadsheet;
using TCMSTester.Config;
using TCMSTester.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ListView = System.Windows.Forms.ListView;
using TextBox = System.Windows.Forms.TextBox;

namespace CITester
{
    public partial class FormPLC : Form
    {
        FormMain m_frmMain = null;
        private FormMain _mainForm;
        private classSerialCommPacket serialTester1;
        private classSerialCommPacket serialTester2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frmMain"></param>
        /// 
        public FormPLC(FormMain frmMain)
        {
            InitializeComponent();
            m_frmMain = frmMain;

            digitalOutputGridControl1.ItemClicked += DigitalOutputGridControl1_ItemClicked;
        }

        private void DigitalOutputGridControl1_ItemClicked(DigitalItemConfig item)
        {
            if (_mainForm?.m_PLCNetwork != null)
            {
                int currentOutputValue = item.IsChecked ? item.OnValue : item.OffValue;

                _mainForm?.m_PLCNetwork.SetDO(0, item.ChannelNo, currentOutputValue);
            }
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


        private void FormPLC_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. 시리얼 통신 초기화 (시리얼 포트 에러가 나더라도 UI 로드가 멈추지 않도록 예외 처리)
                try
                {
                    // TODO: 'FormMain.stCommonInfo' 부분의 stCommonInfo를 FormMain.cs에 선언된 실제 COMMON_INFO 변수명으로 변경하세요.
                    // (만약 static 변수가 아닌 m_frmMain 인스턴스 변수라면 m_frmMain.stCommonInfo 로 변경)
                    serialTester1 = new classSerialCommPacket(FormMain.COMMON_INFO.serialTester1Port0);
                    serialTester2 = new classSerialCommPacket(FormMain.COMMON_INFO.serialTester2Port0);
                }
                catch (Exception exSerial)
                {
                    Console.WriteLine($"[Warning] 시리얼 통신 초기화 실패: {exSerial.Message}");
                }

                // 2. Config/config.json 경로 조합 및 로그
                string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
                Console.WriteLine($"[Debug] Config 파일 경로: {configPath}");

                // 3. AppConfigManager를 이용해 데이터 로드
                AppConfig config = AppConfigManager.LoadConfig(configPath);

                if (config == null)
                {
                    MessageBox.Show("Config 파일 로드에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Console.WriteLine($"[Debug] DigitalInputs 개수: {config.DigitalInputs?.Count ?? 0}개");
                Console.WriteLine($"[Debug] DigitalOutputs 개수: {config.DigitalOutputs?.Count ?? 0}개");

                // 4. 채널 검색 테스트
                int chNo = config.GetChannelNo("DI100-2<48>");
                Console.WriteLine($"[Debug] DI100-2<48> 채널 검색 결과: {chNo}");

                // 5. Digital Output Grid 컨트롤 데이터 바인딩
                if (digitalOutputGridControl1 != null)
                {
                    if (config.DigitalOutputs != null && config.DigitalOutputs.Count > 0)
                    {
                        digitalOutputGridControl1.SetIoItems(config.DigitalOutputs);
                        Console.WriteLine("[Debug] Digital Output 그리드 바인딩 성공!");
                    }
                    else
                    {
                        Console.WriteLine("[Warning] config.DigitalOutputs 리스트가 비어 있습니다.");
                    }
                }
                else
                {
                    MessageBox.Show("digitalOutputGridControl1 이 디자이너에서 생성되지 않았습니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // 6. Digital Input ListView 데이터 바인딩
                if (config.DigitalInputs != null && config.DigitalInputs.Count > 0)
                {
                    PopulateDigitalInputListViews(config.DigitalInputs);
                    Console.WriteLine("[Debug] Digital Input 리스트뷰 바인딩 성공!");
                }

                // 7. 타이머 시작
                m_Timer.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"FormPLC_Load 실행 중 예외 발생!\n\n내용: {ex.Message}\n\n위치: {ex.StackTrace}",
                                "초기화 에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Digital Input 설정 데이터 리스트를 4개의 ListView(ListView_DI1 ~ DI4)에 16개씩 순서대로 분할하여 화면에 표사합니다.
        /// 세로 스크롤바를 활성화하고, 가로 스크롤바 생성을 방지하기 위해 사용 가능 영역에 맞춰 컬럼 폭 및 폰트 크기를 자동으로 조정합니다.
        /// </summary>
        /// <param name="items">ListView에 바인딩할 Digital Input 항목 설정 데이터 리스트</param>
        private void PopulateDigitalInputListViews(List<DigitalItemConfig> items)
        {
            if (items == null) return;

            // 1. 화면에 배치된 4개의 ListView를 배열로 묶음
            ListView[] listViews = new ListView[] { ListView_DI1, ListView_DI2, ListView_DI3, ListView_DI4 };

            int itemsPerListView = 16; // 리스트뷰당 16개 고정

            for (int lvIndex = 0; lvIndex < listViews.Length; lvIndex++)
            {
                ListView lv = listViews[lvIndex];
                if (lv == null) continue;

                lv.BeginUpdate();
                lv.Items.Clear();

                // 세로 스크롤바 활성화
                lv.Scrollable = true;

                float newFontSize = Math.Max(6.0f, lv.Font.SizeInPoints - 3.0f);
                lv.Font = new System.Drawing.Font(lv.Font.FontFamily, newFontSize, lv.Font.Style);

                if (lv.Columns.Count == 0)
                {
                    lv.View = View.Details;
                    lv.FullRowSelect = true;
                    lv.GridLines = true;

                    lv.Columns.Add("부품", 65, HorizontalAlignment.Left);
                    lv.Columns.Add("NO", 30, HorizontalAlignment.Center);
                    lv.Columns.Add("Value", 35, HorizontalAlignment.Center);
                }

                int startIndex = lvIndex * itemsPerListView;
                int endIndex = Math.Min(startIndex + itemsPerListView, items.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    var item = items[i];

                    // SubItem 구성: [0] 부품명, [1] NO(0~15 순번), [2] Value 값
                    ListViewItem lvi = new ListViewItem(item.Name);
                    lvi.SubItems.Add((i % itemsPerListView).ToString()); // 0~15 순번 표기
                    lvi.SubItems.Add(item.OffValue.ToString());          // 현재 설정값/상태값

                    lv.Items.Add(lvi);
                }

                lv.EndUpdate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void FormPLC_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (m_frmMain != null)
            //{
            //m_frmMain.m_PLCNetwork.START = false;
            //m_frmMain.m_PLCNetwork.WRITE = false;
            //m_frmMain.m_PLCNetwork.WRITE = false;m_Timer
            //
            m_Timer.Enabled = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void DigitalOutput_Select_Changed(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            int nTag = Int32.Parse(check.Tag.ToString());

            m_frmMain.m_PLCNetwork.SetDO(0, nTag, check.Checked ? 1 : 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void m_Timer_Tick(object sender, EventArgs e)
        {
            if (m_frmMain?.m_PLCNetwork == null) return;

            // 1. ListView_DI1 처리 (슬롯 0)
            for (int i = 0; i < ListView_DI1.Items.Count && i < 16; ++i)
            {
                var item = ListView_DI1.Items[i];

                // SubItems 개수 체크 및 파싱 예외 방지
                if (item.SubItems.Count > 2 && int.TryParse(item.SubItems[1].Text, out int bitIndex))
                {
                    int nValue = m_frmMain.m_PLCNetwork.GetDI(0, bitIndex);
                    string strValue = nValue.ToString();

                    if (item.SubItems[2].Text != strValue)
                    {
                        item.SubItems[2].Text = strValue;
                    }
                }
            }

            // 2. ListView_DI2 처리 (슬롯 2)
            for (int i = 0; i < ListView_DI2.Items.Count && i < 16; ++i)
            {
                var item = ListView_DI2.Items[i];

                // SubItems 개수 체크 및 파싱 예외 방지
                if (item.SubItems.Count > 2 && int.TryParse(item.SubItems[1].Text, out int bitIndex))
                {
                    int nValue = m_frmMain.m_PLCNetwork.GetDI(2, bitIndex);
                    string strValue = nValue.ToString();

                    if (item.SubItems[2].Text != strValue)
                    {
                        item.SubItems[2].Text = strValue;
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
        private void CheckBox_DCPower_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_DCPower.Checked == true)
            {
                if (TextBox_DCPower.Text == "")
                {
                    CheckBox_DCPower.Checked = false;
                    MessageBox.Show("전압 설정값을 입력하세요.");
                    return;
                }
                if (TextBox_DCPowerCurrent.Text == "")
                {
                    CheckBox_DCPower.Checked = false;
                    MessageBox.Show("전류 설정값을 입력하세요.");
                    return;
                }

                double dValue = 0;
                int nValueCurrent = 0;
                try
                {
                    dValue = double.Parse(TextBox_DCPower.Text);
                    nValueCurrent = int.Parse(TextBox_DCPowerCurrent.Text);
                    m_frmMain.SetDCPowerON(dValue, nValueCurrent);
                }
                catch
                {
                }
            }
            else
            {
                m_frmMain.SetDCPowerOFF();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_DCPower_Click(object sender, EventArgs e)
        {
            if (TextBox_DCPower.Text == "")
            {
                MessageBox.Show("전압 설정값을 입력하세요.");
                return;
            }

            double dValue = 0;
            int nValueCurrent = 0;
            try
            {
                dValue = double.Parse(TextBox_DCPower.Text);
                nValueCurrent = int.Parse(TextBox_DCPowerCurrent.Text);
                m_frmMain.ChangeDCPower(dValue, nValueCurrent);
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
        private void CheckBox_ACPower_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (CheckBox_ACPower.Checked == true)
            {
                if (TextBox_ACPower.Text == "")
                {
                    CheckBox_ACPower.Checked = false;
                    MessageBox.Show("전압 설정값을 입력하세요.");
                    return;
                }

                double dValue = 0;
                try
                {
                    dValue = double.Parse(TextBox_ACPower.Text);
                    m_frmMain.SetACPowerON(dValue);
                }
                catch
                {
                }
            }
            else
            {
                m_frmMain.SetACPowerOFF();
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_ACPower_Click(object sender, EventArgs e)
        {
            /*
            if (TextBox_ACPower.Text == "")
            {
                MessageBox.Show("전압 설정값을 입력하세요.");
                return;
            }

            double dValue = 0;
            try
            {
                dValue = double.Parse(TextBox_ACPower.Text);
                m_frmMain.ChangeACPowerVolt(dValue);
            }
            catch
            {
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void CheckBox_FG_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (CheckBox_FG.Checked == true)
            {
                if (TextBox_FG.Text == "")
                {
                    CheckBox_FG.Checked = false;
                    MessageBox.Show("주파수 설정값을 입력하세요.");
                    return;
                }
                if (TextBox_FGVolt.Text == "")
                {
                    CheckBox_FG.Checked = false;
                    MessageBox.Show("전압 설정값을 입력하세요.");
                    return;
                }

                int nValue = 0;
                int nValueVolt = 0;
                try
                {
                    nValue = int.Parse(TextBox_FG.Text);
                    nValueVolt = int.Parse(TextBox_FGVolt.Text);
                    //m_frmMain.SetFunctionGeneratorON(nValue, nValueVolt);
                }
                catch
                {
                }
            }
            else
            {
                //m_frmMain.SetFunctionGeneratorOFF();
            }
            */
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void CheckBox_PowerPWM_CheckedChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void CheckBox_RGPWM_CheckedChanged(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_Oscilloscope_Click(object sender, EventArgs e)
        {
            if (comboBoxOSC_Cmd.Text == "")
            {
                MessageBox.Show("오실로스코프 전송 명령을 입력하세요.");
                return;
            }

            try
            {
                if (comboBoxOSC_Cmd.Text.IndexOf("?") >= 0)
                {
                    m_frmMain.m_ethernetOscilloscope.WriteString(comboBoxOSC_Cmd.Text + "\r\n");
                    String strIDN = m_frmMain.m_ethernetOscilloscope.ReadString();

                    labelOSC_Receive.Text = strIDN;
                    //MessageBox.Show(strIDN);
                }
                else
                {
                    m_frmMain.m_ethernetOscilloscope.WriteString(comboBoxOSC_Cmd.Text + "\r\n");
                }
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
        private void CheckBox_Powering_CheckedChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void CheckBox_Braking_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Button_DCPT1_Click(object sender, EventArgs e)
        {
            double dValue;

            try
            {
                if (TextBox_DCPT1.Text != "")
                {
                    dValue = Int32.Parse(TextBox_DCPT1.Text);
                }
                else
                {
                    dValue = 0;
                }

                if (dValue > 10)
                {
                    MessageBox.Show("DCPT1 0 - 10 사이의 숫자를 입력하십시오.");
                    return;
                }

                m_frmMain.m_PLCNetwork.SetAO_double(0, 0, dValue);
            }
            catch
            {
                MessageBox.Show("DCPT1 0 - 10 사이의 숫자를 입력하십시오.");
            }



        }

        private void Button_DCPT2_Click(object sender, EventArgs e)
        {
            double dValue;

            try
            {
                if (TextBox_DCPT2.Text != "")
                {
                    dValue = Int32.Parse(TextBox_DCPT2.Text);
                }
                else
                {
                    dValue = 0;
                }

                if (dValue > 10)
                {
                    MessageBox.Show("DCPT1 0 - 10 사이의 숫자를 입력하십시오.");
                    return;
                }

                m_frmMain.m_PLCNetwork.SetAO_double(0, 1, dValue);

                labelFCOVValue.Text = string.Format("{0:0.00}", Double.Parse(TextBox_DCPT2.Text) * 330);
            }
            catch
            {
                MessageBox.Show("DCPT2 0 - 10 사이의 숫자를 입력하십시오.");
            }
        }

        private void Button_DCPT3_Click(object sender, EventArgs e)
        {
            if (Double.Parse(TextBox_DCPT3Volt.Text) > 15)
            {
                MessageBox.Show("DCPT3 전압과 전류를 확인해주세요");
                return;
            }

            try
            {
                if (TextBox_DCPT3Current.Text != "" && TextBox_DCPT3Volt.Text != "")
                {
                    m_frmMain.SetDSP1VoltCurrentChange(Double.Parse(TextBox_DCPT3Volt.Text), Double.Parse(TextBox_DCPT3Current.Text));
                    Thread.Sleep(200);
                    m_frmMain.SetDSP1PowerON();

                    labelDCOVValue.Text = string.Format("{0:0.00}", Double.Parse(TextBox_DCPT3Volt.Text) * 300);
                }
                else
                {
                    m_frmMain.SetDSP1VoltCurrentChange(Double.Parse("0"), Double.Parse("0"));
                    Thread.Sleep(200);
                    m_frmMain.SetDSP1PowerON();

                    labelDCOVValue.Text = string.Format("{0:0.00}", Double.Parse("0") * 300);
                }
            }
            catch
            {
                MessageBox.Show("DCPT3 전압과 전류를 확인해주세요");
            }
        }

        private void Button_SP_Click(object sender, EventArgs e)
        {
            double dValue;

            try
            {
                if (TextBox_SP.Text != "")
                {
                    dValue = Int32.Parse(TextBox_SP.Text);
                }
                else
                {
                    dValue = 0;
                }

                if (dValue > 10)
                {
                    MessageBox.Show("DCPT1 0 - 10 사이의 숫자를 입력하십시오.");
                    return;
                }

                m_frmMain.m_PLCNetwork.SetAO_double(0, 3, dValue);
            }
            catch
            {
                MessageBox.Show("SP 0 - 10 사이의 숫자를 입력하십시오.");
            }
        }

        ///**************************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCurrentOut_Send_Click(object sender, EventArgs e)
        {
            labelCurrentOut_Receive.Text = "";

            if (comboBoxCurrentOut_Cmd.Text == "")
            {
                labelCurrentOut_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelCurrentOut_Receive.Text = m_frmMain.CurrentOutCmd_Send(comboBoxCurrentOut_Cmd.Text);
        }

        private void buttonCurrentOut_Set_Send_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string strTag = button.Tag.ToString();
            int nTag = int.Parse(strTag);

            TextBox[] textBoxes = {textBoxCurrentOut_Value0, textBoxCurrentOut_Value1, textBoxCurrentOut_Value2, textBoxCurrentOut_Value3, textBoxCurrentOut_Value4
                    , textBoxCurrentOut_Value5, textBoxCurrentOut_Value6, textBoxCurrentOut_Value7};



            labelCurrentOut_Receive.Text = "";

            if (textBoxes[nTag].Text == "")
            {
                labelCurrentOut_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelCurrentOut_Receive.Text = m_frmMain.CurrentOutCmd_Set_Send("0", strTag, textBoxes[nTag].Text);
        }

        private void buttonCurrentOut_Reset_Send_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string strTag = button.Tag.ToString();
            int nTag = int.Parse(strTag);

            TextBox[] textBoxes = {textBoxCurrentOut_Value0, textBoxCurrentOut_Value1, textBoxCurrentOut_Value2, textBoxCurrentOut_Value3, textBoxCurrentOut_Value4
                    , textBoxCurrentOut_Value5, textBoxCurrentOut_Value6, textBoxCurrentOut_Value7};



            labelCurrentOut_Receive.Text = "";

            if (textBoxes[nTag].Text == "")
            {
                labelCurrentOut_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelCurrentOut_Receive.Text = m_frmMain.CurrentOutCmd_Set_Send("0", strTag, "0");
        }

        private void labelCurrentOut_Receive_Click(object sender, EventArgs e)
        {
            labelCurrentOut_Receive.Text = "";
        }


        ///**************************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSpeedOut_Send_Click(object sender, EventArgs e)
        {
            labelSpeedOut_Receive.Text = "";

            if (comboBoxSpeedOut_Cmd.Text == "")
            {
                labelSpeedOut_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelSpeedOut_Receive.Text = m_frmMain.SpeedOutCmd_Send(comboBoxSpeedOut_Cmd.Text);
        }

        private void buttonSpeedOut_Send_2sin_Click(object sender, EventArgs e)
        {
            labelSpeedOut_Receive.Text = "";

            if (textBoxSpeedOut_2sin_Hz.Text == "" || textBoxSpeedOut_2sin_Volt.Text == "")
            {
                labelSpeedOut_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelSpeedOut_Receive.Text = m_frmMain.SpeedOutCmd_2sin_Send(textBoxSpeedOut_2sin_Hz.Text, textBoxSpeedOut_2sin_Volt.Text);
        }

        private void buttonSpeedOut_Send_3sin_Click(object sender, EventArgs e)
        {
            labelSpeedOut_Receive.Text = "";

            if (textBoxSpeedOut_3sin_Hz.Text == "" || textBoxSpeedOut_3sin_Volt.Text == "")
            {
                labelSpeedOut_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelSpeedOut_Receive.Text = m_frmMain.SpeedOutCmd_3sin_Send(textBoxSpeedOut_3sin_Hz.Text, textBoxSpeedOut_3sin_Volt.Text);

            if (labelSpeedOut_Receive.Text != "")
            {
                labelACOVValue.Text = string.Format("{0:0.00}", Double.Parse(textBoxSpeedOut_3sin_Volt.Text) * 96);
                labelACUVValue.Text = string.Format("{0:0.00}", Double.Parse(textBoxSpeedOut_3sin_Volt.Text) * 96);
            }
        }

        private void labelSpeedOut_Receive_Click(object sender, EventArgs e)
        {
            labelSpeedOut_Receive.Text = "";
        }

        ///**************************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOptical_Send_Click(object sender, EventArgs e)
        {
            labelOptical1_Receive.Text = "";

            if (comboBoxOptical1_Cmd.Text == "")
            {
                labelOptical1_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelOptical1_Receive.Text = m_frmMain.OpticalCmd_Send(comboBoxOptical1_Cmd.Text);
        }

        private void buttonOptical_Hz_Send_Click(object sender, EventArgs e)
        {
            labelOptical1_Receive.Text = "";

            if (textBoxOptical1_Loc.Text == "" || textBoxOptical1_Hz.Text == "")
            {
                labelOptical1_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelOptical1_Receive.Text = m_frmMain.OpticalCmd_Hz_Send(textBoxOptical1_Loc.Text, textBoxOptical1_Hz.Text);
        }

        private void buttonOptical_Duty_Send_Click(object sender, EventArgs e)
        {
            labelOptical1_Receive.Text = "";

            if (textBoxOptical1_Loc.Text == "" || textBoxOptical1_Duty.Text == "")
            {
                labelOptical1_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelOptical1_Receive.Text = m_frmMain.OpticalCmd_Duty_Send(textBoxOptical1_Loc.Text, textBoxOptical1_Duty.Text);
        }

        private void labelOptical_Receive_Click(object sender, EventArgs e)
        {
            labelOptical1_Receive.Text = "";
        }

        private void buttonOptical2_Send_Click(object sender, EventArgs e)
        {
            labelOptical2_Receive.Text = "";

            if (comboBoxOptical2_Cmd.Text == "")
            {
                labelOptical2_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelOptical2_Receive.Text = m_frmMain.OpticalCmd2_Send(comboBoxOptical2_Cmd.Text);
        }

        private void buttonOptical2_Hz_Send_Click(object sender, EventArgs e)
        {
            labelOptical2_Receive.Text = "";

            if (textBoxOptical2_Loc.Text == "" || textBoxOptical2_Hz.Text == "")
            {
                labelOptical2_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelOptical2_Receive.Text = m_frmMain.OpticalCmd2_Hz_Send(textBoxOptical2_Loc.Text, textBoxOptical2_Hz.Text);
        }

        private void buttonOptical2_Duty_Send_Click(object sender, EventArgs e)
        {
            labelOptical2_Receive.Text = "";

            if (textBoxOptical2_Loc.Text == "" || textBoxOptical2_Duty.Text == "")
            {
                labelOptical2_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelOptical2_Receive.Text = m_frmMain.OpticalCmd2_Duty_Send(textBoxOptical2_Loc.Text, textBoxOptical2_Duty.Text);
        }
        private void labelOptical2_Receive_Click(object sender, EventArgs e)
        {
            labelOptical1_Receive.Text = "";
        }
        ///**************************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMVB_Send_Click(object sender, EventArgs e)
        {
            labelMVB_Receive.Text = "";

            if (comboBoxMVB_Cmd.Text == "")
            {
                labelMVB_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelMVB_Receive.Text = m_frmMain.MVBCmd_Send(comboBoxMVB_Cmd.Text);
        }
        private void labelMVB_Receive_Click(object sender, EventArgs e)
        {
            labelMVB_Receive.Text = "";
        }

        ///**************************************************************************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDMM_Send_Click(object sender, EventArgs e)
        {
            labelDMM_Receive.Text = "";

            if (comboBoxDMM_Cmd1.Text == "")
            {
                labelDMM_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelDMM_Receive.Text = m_frmMain.DMMCmd_Send(comboBoxDMM_Cmd1.Text, comboBoxDMM_Cmd2.Text, comboBoxDMM_Cmd3.Text, comboBoxDMM_Cmd4.Text, comboBoxDMM_Cmd5.Text);
        }

        private void labelDMM_Receive_Click(object sender, EventArgs e)
        {
            labelDMM_Receive.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button_DCPT1.PerformClick();
            Button_DCPT2.PerformClick();
            Button_DCPT3.PerformClick();
            Button_SP.PerformClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                double dValue1 = Double.Parse(TextBox_DCPT1.Text);
                int nValue1 = (int)(dValue1);
                if (nValue1 > 4000)
                    nValue1 = 4000;
                if (nValue1 < 0)
                    nValue1 = 0;
                m_frmMain.m_PLCNetwork.SetAO(0, 0, nValue1);



                double dValue2 = Double.Parse(TextBox_DCPT2.Text);
                int nValue2 = (int)(dValue2);
                if (nValue2 > 4000)
                    nValue2 = 4000;
                if (nValue2 < 0)
                    nValue2 = 0;
                m_frmMain.m_PLCNetwork.SetAO(0, 2, nValue2);
            }
            catch
            {
                MessageBox.Show("0 - 10 사이의 숫자를 입력하십시오.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                double dValue1 = Double.Parse(TextBox_DCPT1.Text);
                int nValue1 = (int)(dValue1);
                if (nValue1 > 4000)
                    nValue1 = 4000;
                if (nValue1 < 0)
                    nValue1 = 0;
                m_frmMain.m_PLCNetwork.SetAO(0, 0, nValue1);



                double dValue2 = Double.Parse(TextBox_DCPT2.Text);
                int nValue2 = (int)(dValue2);
                if (nValue2 > 4000)
                    nValue2 = 4000;
                if (nValue2 < 0)
                    nValue2 = 0;
                m_frmMain.m_PLCNetwork.SetAO(0, 1, nValue2);
            }
            catch
            {
                MessageBox.Show("0 - 10 사이의 숫자를 입력하십시오.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                double dValue1 = Double.Parse(TextBox_DCPT1.Text);
                int nValue1 = (int)(dValue1);
                if (nValue1 > 4000)
                    nValue1 = 4000;
                if (nValue1 < 0)
                    nValue1 = 0;
                m_frmMain.m_PLCNetwork.SetAO(0, 1, nValue1);



                double dValue2 = Double.Parse(TextBox_DCPT2.Text);
                int nValue2 = (int)(dValue2);
                if (nValue2 > 4000)
                    nValue2 = 4000;
                if (nValue2 < 0)
                    nValue2 = 0;
                m_frmMain.m_PLCNetwork.SetAO(0, 2, nValue2);
            }
            catch
            {
                MessageBox.Show("0 - 10 사이의 숫자를 입력하십시오.");
            }
        }

        public string CRC(string sTmpMsg)
        {
            byte[] convertArr = new byte[(sTmpMsg.Length) / 2];
            for (int i = 0; i < convertArr.Length; i++)
            {
                convertArr[i] = Convert.ToByte(sTmpMsg.Substring(i * 2, 2), 16);
            }
            //Variable with result of your calculation.
            int checksum = 0;
            //Step1: Add byte values.            
            foreach (byte value in convertArr)
            {
                checksum += value;
            }
            checksum = 256 - checksum;
            checksum &= 0xFF; // FFFFFF replace
            return checksum.ToString("X2");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox_DSP2Volt.Text == "" || textBox_DSP2Current.Text == "")
            {
                MessageBox.Show("전압과 전류를 확인해주세요");

                return;
            }

            m_frmMain.SetDSP2VoltCurrentChange(Double.Parse(textBox_DSP2Volt.Text), Double.Parse(textBox_DSP2Current.Text));
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            m_frmMain.SetDSP2PowerOFF();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            m_frmMain.SetDSP2PowerON();
        }

        private void TextBox_DCPT1Volt_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void buttonTrimmer_Send_Click(object sender, EventArgs e)
        {
            labelTrimmer_Receive.Text = "";

            if (comboBoxTrimmer_Cmd.Text == "")
            {
                labelTrimmer_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelTrimmer_Receive.Text = m_frmMain.Trimmer_Send(comboBoxTrimmer_Cmd.Text);
            Console.WriteLine(labelTrimmer_Receive.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            labelTrimmer_Receive.Text = "";

            if (textBoxTrimmer_No.Text == "" || textBoxTrimmer_Ch.Text == "" || textBoxTrimmer_value.Text == "")
            {
                labelTrimmer_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelTrimmer_Receive.Text = m_frmMain.Trimmer_No_Ch_Value_Send(textBoxTrimmer_No.Text, textBoxTrimmer_Ch.Text, textBoxTrimmer_value.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            /*
            CheckBox_DO_17.Checked = true;
            CheckBox_DO_22.Checked = true;
            CheckBox_DO_23.Checked = true;
            CheckBox_DO_24.Checked = true;
            CheckBox_DO_25.Checked = true;
            CheckBox_DO_27.Checked = true;
            CheckBox_DO_28.Checked = true;
            CheckBox_DO_33.Checked = true;
            CheckBox_DO_34.Checked = true;
            CheckBox_DO_39.Checked = true;
            CheckBox_DO_40.Checked = true;
            CheckBox_DO_41.Checked = true;
            CheckBox_DO_4.Checked = true;
            CheckBox_DO_2.Checked = true;
            CheckBox_DO_56.Checked = true;

            CheckBox_DO_18.Checked = false;
            CheckBox_DO_19.Checked = false;
            CheckBox_DO_20.Checked = false;
            CheckBox_DO_38.Checked = false;
            CheckBox_DO_50.Checked = false;
            CheckBox_DO_53.Checked = false;
            CheckBox_DO_42.Checked = false;
            CheckBox_DO_48.Checked = false;
            CheckBox_DO_46.Checked = false;
            CheckBox_DO_61.Checked = false;
            CheckBox_DO_62.Checked = false;
            */
            button9.PerformClick();
            Thread.Sleep(300);
            button10.PerformClick();
            Thread.Sleep(300);
            button11.PerformClick();
            Thread.Sleep(300);
            button12.PerformClick();
            Thread.Sleep(300);
            button13.PerformClick();
            Thread.Sleep(300);
            button14.PerformClick();
            Thread.Sleep(300);
            button15.PerformClick();
            Thread.Sleep(300);
            button16.PerformClick();
        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            //CheckBox_DO_50.Checked = true;
            Thread.Sleep(100);
            //CheckBox_DO_38.Checked = true;
            Thread.Sleep(100);
            //CheckBox_DO_18.Checked = true;
            //CheckBox_DO_61.Checked = true;
            //CheckBox_DO_62.Checked = true;
            Thread.Sleep(100);
            buttonCurrentOut_Set_Send7.PerformClick();
            Thread.Sleep(300);

            for (int i = 0; i < 1500; i++)
            {
                if (m_frmMain.m_PLCNetwork.GetDI(0, 0) == 1)
                {
                    //CheckBox_DO_53.Checked = true;

                    break;
                }
                Thread.Sleep(1);
            }

            for (int i = 0; i < 1500; i++)
            {
                if (m_frmMain.m_PLCNetwork.GetDI(0, 11) == 1)
                {
                    //CheckBox_DO_42.Checked = true;
                }
                if (m_frmMain.m_PLCNetwork.GetDI(0, 0) == 0)
                {
                    //CheckBox_DO_53.Checked = false;

                    break;
                }
                Thread.Sleep(1);
            }

            buttonCurrentOut_Set_Send0.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send1.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send2.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send3.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send4.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send5.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send6.PerformClick();
            Thread.Sleep(300);


            //CheckBox_DO_48.Checked = true;
            //CheckBox_DO_46.Checked = true;
        }

        private void buttonPwm_Send_Click(object sender, EventArgs e)
        {
            labelPwm_Receive.Text = "";

            if (comboBoxPwm_Cmd.Text == "")
            {
                labelPwm_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelPwm_Receive.Text = m_frmMain.SpeedOutCmd_Send(comboBoxPwm_Cmd.Text);
        }

        private void buttonPwm_Hz_Send_Click(object sender, EventArgs e)
        {
            labelPwm_Receive.Text = "";

            if (textBoxPwm_Loc.Text == "" || textBoxPwm_Hz.Text == "")
            {
                labelPwm_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelPwm_Receive.Text = m_frmMain.PwmCmd_Hz_Send(textBoxPwm_Loc.Text, textBoxPwm_Hz.Text);
        }

        private void buttonPwm_Duty_Send_Click(object sender, EventArgs e)
        {
            labelPwm_Receive.Text = "";

            if (textBoxPwm_Loc.Text == "" || textBoxPwm_Hz.Text == "")
            {
                labelPwm_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelPwm_Receive.Text = m_frmMain.PwmCmd_Duty_Send(textBoxPwm_Loc.Text, textBoxPwm_Duty.Text);
        }

        private void labelPwm_Receive_Click(object sender, EventArgs e)
        {
            labelPwm_Receive.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //CheckBox_DO_50.Checked = true;
            Thread.Sleep(100);
            //CheckBox_DO_38.Checked = true;
            Thread.Sleep(100);
            //CheckBox_DO_19.Checked = true;
            //CheckBox_DO_61.Checked = true;
            //CheckBox_DO_62.Checked = true;
            Thread.Sleep(100);
            m_frmMain.CurrentOutCmd_Set_Send("0", "7", "40");


            for (int i = 0; i < 1500; i++)
            {
                if (m_frmMain.m_PLCNetwork.GetDI(0, 0) == 1)
                {
                    //CheckBox_DO_53.Checked = true;

                    break;
                }
                Thread.Sleep(1);
            }

            for (int i = 0; i < 1500; i++)
            {
                if (m_frmMain.m_PLCNetwork.GetDI(0, 11) == 1)
                {
                    //CheckBox_DO_42.Checked = true;
                }
                if (m_frmMain.m_PLCNetwork.GetDI(0, 0) == 0)
                {
                    //CheckBox_DO_53.Checked = false;

                    break;
                }
                Thread.Sleep(1);
            }

            buttonCurrentOut_Set_Send0.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send1.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send2.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send3.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send4.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send5.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send6.PerformClick();
            Thread.Sleep(300);
            buttonCurrentOut_Set_Send7.PerformClick();
            Thread.Sleep(300);

            //CheckBox_DO_48.Checked = true;
            //CheckBox_DO_46.Checked = true;
        }

        private void buttonTrimmer2_Send_Click(object sender, EventArgs e)
        {
            labelTrimmer2_Receive.Text = "";

            if (comboBoxTrimmer2_Cmd.Text == "")
            {
                labelTrimmer2_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelTrimmer2_Receive.Text = m_frmMain.Trimmer2_Send(comboBoxTrimmer2_Cmd.Text);
            Console.WriteLine(labelTrimmer2_Receive.Text);
        }

        private void buttonTrimmer2_No_Ch_Send_Click(object sender, EventArgs e)
        {
            labelTrimmer2_Receive.Text = "";

            if (textBoxTrimmer2_No.Text == "" || textBoxTrimmer2_Ch.Text == "" || textBoxTrimmer2_value.Text == "")
            {
                labelTrimmer2_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelTrimmer2_Receive.Text = m_frmMain.Trimmer2_No_Ch_Value_Send(textBoxTrimmer2_No.Text, textBoxTrimmer2_Ch.Text, textBoxTrimmer2_value.Text);
        }

        private void buttonTest_Send_Click(object sender, EventArgs e)
        {
            labelLineVoltage_Receive.Text = "";

            if (comboBoxLineVoltage_Cmd.Text == "")
            {
                labelLineVoltage_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelLineVoltage_Receive.Text = m_frmMain.LineVoltage0Cmd_Send(comboBoxLineVoltage_Cmd.Text);
        }

        private void buttonTest_Hz_V_Send_Click(object sender, EventArgs e)
        {
            labelLineVoltage_Receive.Text = "";

            if (textBoxLineVoltage_Hz.Text == "" || textBoxLineVoltage_V.Text == "")
            {
                labelLineVoltage_Receive.Text = "명령어를 입력해주세요.";

                return;
            }

            labelLineVoltage_Receive.Text = m_frmMain.LineVoltage0Cmd_2sin_Send(textBoxLineVoltage_Hz.Text, textBoxLineVoltage_V.Text);
        }

        private void digitalOutputGridControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
