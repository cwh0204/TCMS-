using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using static CITester.FormLoad;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace CITester
{
    public partial class FormConfig : Form
    {
        OleDbCommand m_OLECommand;
        FormMain frmMain; //제어편성 변수
        //언어설정
        //
        private string tempTrainClassInfo;
        XmlDocument m_xmlDoc = new XmlDocument();
        public ConfigData m_Config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="config"></param>
        /// 
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED (모든 자식 컨트롤 더블 버퍼링 일괄 적용)
                return cp;
            }
        }
        public FormConfig(OleDbCommand command, ConfigData config, FormMain parent)
        {
            this.SuspendLayout();
            InitializeComponent();
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint,
                true);

            m_OLECommand = command;
            m_Config = config;
            this.m_Config = config;
            this.frmMain = parent;
            this.ResumeLayout(false);
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            tempTrainClassInfo = ConfigJson.CurrentConfig.Operation.TCMSUnit;
            if (string.IsNullOrEmpty(tempTrainClassInfo))
            {
                tempTrainClassInfo = "1,2,3단계 전동차";
            }
            TextBox_Tester.Text = ConfigJson.CurrentConfig.Operation.TesterName;

            if (ConfigJson.CurrentConfig.Operation.ShowTCMSUnit == true)
            {
                chkDonotShowAgain.Checked = true;
            }
            RefreshUI();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnOK_Click(object sender, EventArgs e)
        {
            bool bIsEng = GlobalSettings.strLanguage.StartsWith("en");

            // 설명형 주석: 언어 설정에 따른 경고 메시지 창 출력을 일괄 처리하는 로컬 함수
            void ShowMsg(string strEnMsg, string strKoMsg)
            {
                string strMsg = bIsEng ? strEnMsg : strKoMsg;
                string strTitle = bIsEng ? "Warning" : "경고";
                MessageBox.Show(strMsg, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            try
            {
                // 1. 운영 및 작업자 정보 전역 객체에 다이렉트 반영
                ConfigJson.CurrentConfig.Operation.TesterName = TextBox_Tester.Text;
                ConfigJson.CurrentConfig.Operation.TCMSUnit = tempTrainClassInfo;
                ConfigJson.CurrentConfig.Operation.ShowTCMSUnit = chkDonotShowAgain.Checked;

                // 로컬 헬퍼 함수: 리스트 내부에 데이터가 존재할 경우 안전하게 수치를 동기화하는 기능
                void UpdateSpec(System.Collections.Generic.List<ConfigJson.SpecItem> list, string strName, double dStd, double dPmt)
                {
                    var item = list.Find(x => x.ItemName == strName);
                    if (item != null)
                    {
                        item.Standard = dStd;
                        item.Permissible = dPmt;
                    }
                }

                // 2. 제어편성 조건에 따른 세부 제원 리스트 업데이트 (기존 데이터 완벽 보존)
                if (tempTrainClassInfo == "1,2,3단계 전동차")
                {
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "컨버터/인버터(1,2,3) ON", TextBox_CI_ON_Standard.Value, TextBox_CI_ON_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "컨버터/인버터(1,2,3) OFF", TextBox_CI_OFF_Standard.Value, TextBox_CI_OFF_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "BPSF_123", TextBox_1_Standard.Value, TextBox_1_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "ACOV_123", TextBox_2_Standard.Value, TextBox_2_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "ACLV_123", TextBox_3_Standard.Value, TextBox_3_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "VDOV_123", TextBox_4_Standard.Value, TextBox_4_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "VDLV_123", TextBox_5_Standard.Value, TextBox_5_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "ISOC1_123", TextBox_6_Standard.Value, TextBox_6_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "ISOC2_123", TextBox_7_Standard.Value, TextBox_7_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "MOCD_123", TextBox_8_Standard.Value, TextBox_8_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "PUD_123", TextBox_9_Standard.Value, TextBox_9_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "FCDF_123", TextBox_10_Standard.Value, TextBox_10_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "IGOC_123", TextBox_11_Standard.Value, TextBox_11_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "BSD_123", TextBox_12_Standard.Value, TextBox_12_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "IDOC_123", TextBox_13_Standard.Value, TextBox_13_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "ZCDFP_123", TextBox_14_Standard.Value, TextBox_14_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs123, "ZCDFM_123", TextBox_15_Standard.Value, TextBox_15_Permit.Value);
                }
                else if (tempTrainClassInfo == "54칸 전동차")
                {
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "컨버터/인버터(54) ON", TextBox_CI_ON_Standard.Value, TextBox_CI_ON_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "컨버터/인버터(54) OFF", TextBox_CI_OFF_Standard.Value, TextBox_CI_OFF_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "BPSF_54", TextBox_1_Standard.Value, TextBox_1_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "ACOV_54", TextBox_2_Standard.Value, TextBox_2_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "ACLV_54", TextBox_3_Standard.Value, TextBox_3_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "ISOC_54", TextBox_4_Standard.Value, TextBox_4_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "MOCD_54", TextBox_5_Standard.Value, TextBox_5_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "FCOV_54", TextBox_6_Standard.Value, TextBox_6_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "FCLV_54", TextBox_7_Standard.Value, TextBox_7_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "LGD_54", TextBox_8_Standard.Value, TextBox_8_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "BOCD_54", TextBox_9_Standard.Value, TextBox_9_Permit.Value);
                    UpdateSpec(ConfigJson.CurrentConfig.ListSpecs54, "PUD_54", TextBox_10_Standard.Value, TextBox_10_Permit.Value);
                }

                // 3. 공통 전압 제원 항목군 업데이트
                UpdateSpec(ConfigJson.CurrentConfig.ListSpecsCommon, "P24_Unit", TextBox_P24_Unit_Standard.Value, TextBox_P24_Unit_Permit.Value);
                UpdateSpec(ConfigJson.CurrentConfig.ListSpecsCommon, "N24_Unit", TextBox_N24_Unit_Standard.Value, TextBox_N24_Unit_Permit.Value);
                UpdateSpec(ConfigJson.CurrentConfig.ListSpecsCommon, "P12_Unit", TextBox_P12_Unit_Standard.Value, TextBox_P12_Unit_Permit.Value);
                UpdateSpec(ConfigJson.CurrentConfig.ListSpecsCommon, "Main_ON", TextBox_Main_ON_Standard.Value, TextBox_Main_ON_Permit.Value);
                UpdateSpec(ConfigJson.CurrentConfig.ListSpecsCommon, "Main_OFF", TextBox_Main_OFF_Standard.Value, TextBox_Main_OFF_Permit.Value);

                // 4. 전역 객체 상태 그대로 config.json 파일 물리 저장 수행
                ConfigManager configManager = new ConfigManager();
                bool bSaveResult = configManager.SaveConfig(ConfigJson.CurrentConfig);

                if (bSaveResult)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    ShowMsg("Failed to write updated settings to config.json.", "설정 파일(config.json) 저장에 실패하여 화면을 닫을 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                // UI 대입 중 포맷 변환 실패나 컨트롤 탐색 오류 감지 시 강제 다운 방어
                ShowMsg($"UI data conversion error occurred. [{ex.Message}]", $"데이터 수집 및 매핑 중 오류가 발생했습니다. 입력값을 확인하세요. [{ex.Message}]");
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
            this.DialogResult = DialogResult.Cancel;
            Close();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void BtnSelectTester_Click(object sender, EventArgs e)
        {
            FormTester frmTester = new FormTester(m_OLECommand);
            if (frmTester.ShowDialog() == DialogResult.OK)
            {
                TextBox_Tester.Text = ConfigJson.CurrentConfig.Operation.TesterName;
            }
        }

        private void BtnSelectUnit_Click(object sender, EventArgs e)
        {
            using (UnitSelectForm selectForm = new UnitSelectForm())
            {
                if (selectForm.ShowDialog() == DialogResult.OK)
                {
                    // ★ 메인 폼이 아닌 임시 변수에 저장
                    if (selectForm.strTCMSUnit == "Unit1")
                        tempTrainClassInfo = "1,2,3단계 전동차";
                    else
                        tempTrainClassInfo = "54칸 전동차";

                    RefreshUI();
                    MessageBox.Show("열차 형식이 변경되었습니다.\n[저장 및 종료] 버튼을 눌러야 적용됩니다.", "알림");
                }
            }
        }
        private void RefreshUI()
        {
            // ★ 추가
            this.SuspendLayout();
            try
            {
                if (tempTrainClassInfo == "1,2,3단계 전동차")
                {
                    label_3.Text = "VDOV";
                    label_3.Location = new Point(7, 202);
                    label_4.Text = "VDLV";
                    label_4.Location = new Point(6, 245);
                    label_5.Text = "ISOC1";
                    label_5.Location = new Point(8, 286);
                    label_6.Text = "ISOC2";
                    label_6.Location = new Point(8, 328);
                    label_7.Text = "MOCD";
                    label_7.Location = new Point(8, 370);
                    label_8.Text = "PUD";
                    label_8.Location = new Point(2, 412);
                    label_9.Text = "FCDF";
                    label_9.Location = new Point(4, 454);

                    label_10.Visible = true;
                    label_11.Visible = true;
                    label_12.Visible = true;
                    label_13.Visible = true;
                    label_14.Visible = true;
                    label50.Visible = true;
                    label47.Visible = true;
                    label25.Visible = true;
                    label44.Visible = true;
                    label40.Visible = true;
                    label41.Visible = true;
                    label43.Visible = true;
                    label16.Visible = true;
                    label49.Visible = true;
                    label46.Visible = true;

                    TextBox_11_Standard.Visible = true;
                    TextBox_11_Permit.Visible = true;
                    TextBox_12_Standard.Visible = true;
                    TextBox_12_Permit.Visible = true;
                    TextBox_13_Standard.Visible = true;
                    TextBox_13_Permit.Visible = true;
                    TextBox_14_Standard.Visible = true;
                    TextBox_14_Permit.Visible = true;
                    TextBox_15_Standard.Visible = true;
                    TextBox_15_Permit.Visible = true;

                    TextBox_1_Standard.Text = string.Format("{0:0.0}", m_Config.dBPSF_123_Std);
                    TextBox_1_Permit.Text = string.Format("{0:0.0}", m_Config.dBPSF_123_Pmt);
                    TextBox_2_Standard.Text = string.Format("{0:0.0}", m_Config.dACOV_123_Std);
                    TextBox_2_Permit.Text = string.Format("{0:0.0}", m_Config.dACOV_123_Pmt);
                    TextBox_3_Standard.Text = string.Format("{0:0.0}", m_Config.dACLV_123_Std);
                    TextBox_3_Permit.Text = string.Format("{0:0.0}", m_Config.dACLV_123_Pmt);
                    TextBox_4_Standard.Text = string.Format("{0:0.0}", m_Config.dVDOV_123_Std);
                    TextBox_4_Permit.Text = string.Format("{0:0.0}", m_Config.dVDOV_123_Pmt);
                    TextBox_5_Standard.Text = string.Format("{0:0.0}", m_Config.dVDLV_123_Std);
                    TextBox_5_Permit.Text = string.Format("{0:0.0}", m_Config.dVDLV_123_Pmt);
                    TextBox_6_Standard.Text = string.Format("{0:0.0}", m_Config.dISOC1_123_Std);
                    TextBox_6_Permit.Text = string.Format("{0:0.0}", m_Config.dISOC1_123_Pmt);
                    TextBox_7_Standard.Text = string.Format("{0:0.0}", m_Config.dISOC2_123_Std);
                    TextBox_7_Permit.Text = string.Format("{0:0.0}", m_Config.dISOC2_123_Pmt);
                    TextBox_8_Standard.Text = string.Format("{0:0.0}", m_Config.dMOCD_123_Std);
                    TextBox_8_Permit.Text = string.Format("{0:0.0}", m_Config.dMOCD_123_Pmt);
                    TextBox_9_Standard.Text = string.Format("{0:0.0}", m_Config.dPUD_123_Std);
                    TextBox_9_Permit.Text = string.Format("{0:0.0}", m_Config.dPUD_123_Pmt);
                    TextBox_10_Standard.Text = string.Format("{0:0.0}", m_Config.dFCDF_123_Std);
                    TextBox_10_Permit.Text = string.Format("{0:0.0}", m_Config.dFCDF_123_Pmt);
                    TextBox_11_Standard.Text = string.Format("{0:0.0}", m_Config.dIGOC_123_Std);
                    TextBox_11_Permit.Text = string.Format("{0:0.0}", m_Config.dIGOC_123_Pmt);
                    TextBox_12_Standard.Text = string.Format("{0:0.0}", m_Config.dBSD_123_Std);
                    TextBox_12_Permit.Text = string.Format("{0:0.0}", m_Config.dBSD_123_Pmt);
                    TextBox_13_Standard.Text = string.Format("{0:0.0}", m_Config.dIDOC_123_Std);
                    TextBox_13_Permit.Text = string.Format("{0:0.0}", m_Config.dIDOC_123_Pmt);
                    TextBox_14_Standard.Text = string.Format("{0:0.0}", m_Config.dZCDFP_123_Std);
                    TextBox_14_Permit.Text = string.Format("{0:0.0}", m_Config.dZCDFP_123_Pmt);
                    TextBox_15_Standard.Text = string.Format("{0:0.0}", m_Config.dZCDFM_123_Std);
                    TextBox_15_Permit.Text = string.Format("{0:0.0}", m_Config.dZCDFP_123_Pmt);

                    TextBox_CI_ON_Standard.Text = string.Format("{0:0.0}", m_Config.dCI_123_ON_Std);
                    TextBox_CI_ON_Permit.Text = string.Format("{0:0.0}", m_Config.dCI_123_ON_Pmt);
                    TextBox_CI_OFF_Standard.Text = string.Format("{0:0.0}", m_Config.dCI_123_OFF_Std);
                    TextBox_CI_OFF_Permit.Text = string.Format("{0:0.0}", m_Config.dCI_123_OFF_Pmt);
                    TextBox_Main_ON_Standard.Text = string.Format("{0:0.0}", m_Config.dCI_123_ON_Std);
                    TextBox_Main_ON_Permit.Text = string.Format("{0:0.0}", m_Config.dCI_123_ON_Pmt);
                    TextBox_Main_OFF_Standard.Text = string.Format("{0:0.0}", m_Config.dCI_123_OFF_Std);
                    TextBox_Main_OFF_Permit.Text = string.Format("{0:0.0}", m_Config.dCI_123_OFF_Pmt);
                }
                else if (tempTrainClassInfo == "54칸 전동차")
                {
                    label_3.Text = "ISOC";
                    label_3.Location = new Point(5, 202);
                    label_4.Text = "MOCD";
                    label_4.Location = new Point(9, 245);
                    label_5.Text = "FCOV";
                    label_5.Location = new Point(7, 286);
                    label_6.Text = "FCLV";
                    label_6.Location = new Point(5, 328);
                    label_7.Text = "LGD";
                    label_7.Location = new Point(4, 370);
                    label_8.Text = "BOCD";
                    label_8.Location = new Point(10, 412);
                    label_9.Text = "PUD";
                    label_9.Location = new Point(4, 454);

                    label13.Text = "[A]";
                    label17.Text = "[A]";
                    label20.Text = "[V]";
                    label28.Text = "[V]";

                    label_10.Visible = false;
                    label_11.Visible = false;
                    label_12.Visible = false;
                    label_13.Visible = false;
                    label_14.Visible = false;
                    label50.Visible = false;
                    label47.Visible = false;
                    label25.Visible = false;
                    label44.Visible = false;
                    label40.Visible = false;
                    label41.Visible = false;
                    label43.Visible = false;
                    label16.Visible = false;
                    label49.Visible = false;
                    label46.Visible = false;

                    TextBox_11_Standard.Visible = false;
                    TextBox_11_Permit.Visible = false;
                    TextBox_12_Standard.Visible = false;
                    TextBox_12_Permit.Visible = false;
                    TextBox_13_Standard.Visible = false;
                    TextBox_13_Permit.Visible = false;
                    TextBox_14_Standard.Visible = false;
                    TextBox_14_Permit.Visible = false;
                    TextBox_15_Standard.Visible = false;
                    TextBox_15_Permit.Visible = false;

                    TextBox_1_Standard.Text = string.Format("{0:0.0}", m_Config.dBPSF_54_Std);
                    TextBox_1_Permit.Text = string.Format("{0:0.0}", m_Config.dBPSF_54_Pmt);
                    TextBox_2_Standard.Text = string.Format("{0:0.0}", m_Config.dACOV_54_Std);
                    TextBox_2_Permit.Text = string.Format("{0:0.0}", m_Config.dACOV_54_Pmt);
                    TextBox_3_Standard.Text = string.Format("{0:0.0}", m_Config.dACLV_54_Std);
                    TextBox_3_Permit.Text = string.Format("{0:0.0}", m_Config.dACLV_54_Pmt);
                    TextBox_4_Standard.Text = string.Format("{0:0.0}", m_Config.dISOC_54_Std);
                    TextBox_4_Permit.Text = string.Format("{0:0.0}", m_Config.dISOC_54_Pmt);
                    TextBox_5_Standard.Text = string.Format("{0:0.0}", m_Config.dMOCD_54_Std);
                    TextBox_5_Permit.Text = string.Format("{0:0.0}", m_Config.dMOCD_54_Pmt);
                    TextBox_6_Standard.Text = string.Format("{0:0.0}", m_Config.dFCOV_54_Std);
                    TextBox_6_Permit.Text = string.Format("{0:0.0}", m_Config.dFCOV_54_Pmt);
                    TextBox_7_Standard.Text = string.Format("{0:0.0}", m_Config.dFCLV_54_Std);
                    TextBox_7_Permit.Text = string.Format("{0:0.0}", m_Config.dFCLV_54_Pmt);
                    TextBox_8_Standard.Text = string.Format("{0:0.0}", m_Config.dLGD_54_Std);
                    TextBox_8_Permit.Text = string.Format("{0:0.0}", m_Config.dLGD_54_Pmt);
                    TextBox_9_Standard.Text = string.Format("{0:0.0}", m_Config.dBOCD_54_Std);
                    TextBox_9_Permit.Text = string.Format("{0:0.0}", m_Config.dBOCD_54_Pmt);
                    TextBox_10_Standard.Text = string.Format("{0:0.0}", m_Config.dPUD_54_Std);
                    TextBox_10_Permit.Text = string.Format("{0:0.0}", m_Config.dPUD_54_Pmt);

                    TextBox_CI_ON_Standard.Text = string.Format("{0:0.0}", m_Config.dCI_54_ON_Std);
                    TextBox_CI_ON_Permit.Text = string.Format("{0:0.0}", m_Config.dCI_54_ON_Pmt);
                    TextBox_CI_OFF_Standard.Text = string.Format("{0:0.0}", m_Config.dCI_54_OFF_Std);
                    TextBox_CI_OFF_Permit.Text = string.Format("{0:0.0}", m_Config.dCI_54_OFF_Pmt);
                }
                ciTesterLabel1.Text = tempTrainClassInfo;
            }
            finally
            {
                this.ResumeLayout(true); // ★ 추가
            }
        }
        private void CheckBox_Measure_Run_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FormConfig_Shown(object sender, EventArgs e)
        {
            // 윈도우 OS 표준 주기에 가장 안정적인 15ms 인터벌 설정
            var fadeTimer = new System.Windows.Forms.Timer { Interval = 15 };
            double targetOpacity = 1.0;

            fadeTimer.Tick += (s, _) =>
            {
                // 💡 핵심: 목표치(1.0)와의 차이를 계산해 부드러운 감속 효과(Easing) 연출
                double diff = targetOpacity - this.Opacity;

                // 차이가 아주 미미해지면 타이머를 끄고 완벽한 1.0(불투명)으로 고정
                if (diff < 0.05)
                {
                    this.Opacity = 1.0;
                    fadeTimer.Stop();
                    fadeTimer.Dispose(); // 메모리 누수 방지
                }
                else
                {
                    // 남은 거리의 35%씩 빠르게 좁혀감 (버벅임 없이 스르륵 열리는 마법의 수치)
                    this.Opacity += diff * 0.35;
                }
            };

            fadeTimer.Start();
        }
    }
}
public class UnitSelectForm : Form
{
    public string SelectedUnit { get; private set; } = "Unit1";

    public UnitSelectForm()
    {
        // 1. 폼 설정 (상하 배치를 위해 높이를 약간 키움)
        this.Text = "Control Group Selection";
        this.Size = new Size(350, 280); // Height를 220 -> 280으로 변경
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // 2. 상단 안내 문구
        System.Windows.Forms.Label lblNotice = new System.Windows.Forms.Label()
        {
            Text = "열차 형식을 선택하세요.",
            Location = new Point(20, 20),
            AutoSize = true,
            Font = new Font("맑은 고딕", 10, FontStyle.Bold)
        };

        // 3. 그룹박스 (높이를 키워 라디오 버튼을 상하로 배치)
        GroupBox group = new GroupBox
        {
            Text = "Unit Selection",
            Location = new Point(20, 50),
            Size = new Size(300, 120) // 높이 확장
        };

        // 라디오 버튼 1 (위쪽)
        RadioButton rb1 = new RadioButton
        {
            Text = "Unit 1 (1,2,3단계 전동차)",
            Location = new Point(20, 35),
            Checked = true,
            AutoSize = true,
            Font = new Font("맑은 고딕", 9)
        };

        // 라디오 버튼 2 (아래쪽)
        RadioButton rb2 = new RadioButton
        {
            Text = "Unit 2 (54칸 전동차)",
            Location = new Point(20, 75), // Y 좌표를 아래로 내림
            AutoSize = true,
            Font = new Font("맑은 고딕", 9)
        };

        // 4. 확인 버튼 (위치 조정)
        Button btnOk = new Button
        {
            Text = "확인 (OK)",
            Location = new Point(120, 190), // 전체 폼 높이에 맞춰 조정
            Size = new Size(100, 40),
            Cursor = Cursors.Hand
        };

        btnOk.Click += (s, e) =>
        {
            SelectedUnit = rb1.Checked ? "Unit1" : "Unit2";
            this.DialogResult = DialogResult.OK;
            this.Close();
        };

        // 컨트롤 조립
        group.Controls.Add(rb1);
        group.Controls.Add(rb2);
        this.Controls.Add(lblNotice);
        this.Controls.Add(group);
        this.Controls.Add(btnOk);
    }
}