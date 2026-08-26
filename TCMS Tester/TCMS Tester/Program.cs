using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CITester
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 환경 설정 로드 및 싱글톤 인스턴스 동기화
            ConfigManager clsConfigMgr = new ConfigManager();
            if (!clsConfigMgr.LoadConfig(out ConfigJson clsLoadedConfig))
            {
                clsLoadedConfig = new ConfigJson();
            }
            ConfigJson.CurrentConfig = clsLoadedConfig;

            FormLoad frmLoad = new FormLoad();
            FormMain frmMain = new FormMain();
            frmLoad.frmMain = frmMain;
            Application.Run(frmLoad);

            // 유닛 선택 창 실행 (로딩 화면 종료 후 실행)
            if (ConfigJson.CurrentConfig.Operation.ShowTCMSUnit)
            {
                using (UnitSelectForm clsSelectForm = new UnitSelectForm())
                {
                    if (clsSelectForm.ShowDialog() == DialogResult.OK)
                    {
                        // 선택된 정보를 CurrentConfig에 업데이트 후 파일 저장
                        ConfigJson.CurrentConfig.Operation.TCMSUnit = clsSelectForm.strTCMSUnit;
                        ConfigJson.CurrentConfig.Operation.SerialNo = clsSelectForm.strSerialNo;
                        ConfigJson.CurrentConfig.Operation.FleetNo = clsSelectForm.strFleetNo;
                        ConfigJson.CurrentConfig.Operation.TrainNo = clsSelectForm.strTrainNo;
                        ConfigJson.CurrentConfig.Operation.TesterName = clsSelectForm.strTester;

                        clsConfigMgr.SaveConfig(ConfigJson.CurrentConfig);
                    }
                    else
                    {
                        Application.Exit();
                        return;
                    }
                }
            }

            // 메인 폼 실행
            Application.Run(frmMain);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }

    public class UnitSelectForm : Form
    {
        public string strTCMSUnit { get; private set; } = "TC";
        public string strSerialNo { get; private set; } = string.Empty;
        public string strFleetNo { get; private set; } = string.Empty;
        public string strTrainNo { get; private set; } = string.Empty;
        public string strTester { get; private set; } = string.Empty;

        public UnitSelectForm() : this("TC", string.Empty, string.Empty, string.Empty, string.Empty, false)
        {
        }

        public UnitSelectForm(string strUnit, string strSerial, string strFleet, string strTrain, string strTesterName, bool bIsChangeMode = false)
        {
            Color clrNavyPoint = Color.FromArgb(20, 40, 80);
            Color clrWhiteBg = Color.White;

            this.Text = "TCMS";
            this.Size = bIsChangeMode ? new Size(440, 430) : new Size(440, 510);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(251, 251, 252);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblNotice = new Label
            {
                Text = bIsChangeMode ? "변경할 정보를 입력해주세요." : "시험 대상의 정보를 입력해주세요.",
                Location = new Point(25, 20),
                AutoSize = true,
                ForeColor = clrNavyPoint,
                Font = new Font("맑은 고딕", 12F, FontStyle.Bold)
            };

            RadioButton rdoExistInfo = new RadioButton
            {
                Text = "기존 정보",
                Location = new Point(25, 55),
                AutoSize = true,
                Checked = true,
                Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Visible = !bIsChangeMode
            };

            RadioButton rdoNewInfo = new RadioButton
            {
                Text = "새 정보",
                Location = new Point(130, 55),
                AutoSize = true,
                Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Visible = !bIsChangeMode
            };

            Label lblRecentHistory = new Label
            {
                Text = "최근 사용",
                Location = new Point(25, 90),
                AutoSize = true,
                Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Visible = !bIsChangeMode
            };

            ComboBox cmbRecentHistory = new ComboBox
            {
                Location = new Point(100, 86),
                Size = new Size(300, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("맑은 고딕", 9.5F, FontStyle.Regular),
                Visible = !bIsChangeMode
            };

            int nGrpLocationY = bIsChangeMode ? 55 : 125;

            GroupBox grpInputContainer = new GroupBox
            {
                Text = "입력 정보",
                Location = new Point(25, nGrpLocationY),
                Size = new Size(375, 250),
                BackColor = clrWhiteBg,
                Font = new Font("맑은 고딕", 9F, FontStyle.Regular)
            };

            Label lblClassification = new Label
            {
                Text = "유닛 구분",
                Location = new Point(20, 35),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            ComboBox cmbClassification = new ComboBox
            {
                Location = new Point(120, 31),
                Size = new Size(220, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("맑은 고딕", 11F, FontStyle.Regular)
            };
            cmbClassification.Items.AddRange(new object[] { "TC", "CC", "ER", "DU" });

            if (!string.IsNullOrEmpty(strUnit) && cmbClassification.Items.Contains(strUnit))
            {
                cmbClassification.SelectedItem = strUnit;
            }
            else
            {
                cmbClassification.SelectedIndex = 0;
            }

            Label lblSerial = new Label { Text = "일련 번호", Location = new Point(20, 75), AutoSize = true, Font = new Font("맑은 고딕", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60) };
            TextBox txtSerial = new TextBox { Location = new Point(120, 71), Size = new Size(220, 28), Font = new Font("맑은 고딕", 11F, FontStyle.Regular), MaxLength = 20, Text = strSerial ?? string.Empty };

            Label lblFleetNumber = new Label { Text = "편성 번호", Location = new Point(20, 115), AutoSize = true, Font = new Font("맑은 고딕", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60) };
            TextBox txtFleetNumber = new TextBox { Location = new Point(120, 111), Size = new Size(220, 28), Font = new Font("맑은 고딕", 11F, FontStyle.Regular), MaxLength = 20, Text = strFleet ?? string.Empty };

            Label lblTrainNumber = new Label { Text = "차량 번호", Location = new Point(20, 155), AutoSize = true, Font = new Font("맑은 고딕", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60) };
            TextBox txtTrainNumber = new TextBox { Location = new Point(120, 151), Size = new Size(220, 28), Font = new Font("맑은 고딕", 11F, FontStyle.Regular), MaxLength = 20, Text = strTrain ?? string.Empty };

            Label lblTesterName = new Label { Text = "시험자", Location = new Point(20, 195), AutoSize = true, Font = new Font("맑은 고딕", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60), TextAlign = ContentAlignment.MiddleCenter };
            TextBox txtTesterName = new TextBox { Location = new Point(120, 191), Size = new Size(220, 28), Font = new Font("맑은 고딕", 11F, FontStyle.Regular), MaxLength = 20, Text = strTesterName ?? string.Empty };

            // JSON 종합 이력 목록을 콤보박스에 바인딩하는 무명 함수
            Action loadRecentHistoryToCombo = () =>
            {
                cmbRecentHistory.Items.Clear();
                var clsOp = ConfigJson.CurrentConfig?.Operation;

                if (clsOp?.lstCombinedHistory != null && clsOp.lstCombinedHistory.Count > 0)
                {
                    foreach (var clsHist in clsOp.lstCombinedHistory)
                    {
                        cmbRecentHistory.Items.Add(clsHist);
                    }
                    cmbRecentHistory.SelectedIndex = 0;
                }
            };

            // 최근 이력 선택 변경 시 입력 컨트롤 자동 바인딩
            cmbRecentHistory.SelectedIndexChanged += (s, e) =>
            {
                if (cmbRecentHistory.SelectedItem is ConfigJson.OperationInfo.CombinedHistoryItem clsSelected)
                {
                    if (cmbClassification.Items.Contains(clsSelected.strUnit))
                    {
                        cmbClassification.SelectedItem = clsSelected.strUnit;
                    }
                    txtSerial.Text = clsSelected.strSerialNo;
                    txtFleetNumber.Text = clsSelected.strFleetNo;
                    txtTrainNumber.Text = clsSelected.strTrainNo;
                    txtTesterName.Text = clsSelected.strTesterName;
                }
            };

            // 입력 컨트롤 초기화 함수
            Action clearInputFields = () =>
            {
                cmbClassification.SelectedIndex = 0;
                txtSerial.Text = string.Empty;
                txtTrainNumber.Text = string.Empty;
                txtFleetNumber.Text = string.Empty;
                txtTesterName.Text = string.Empty;
            };

            if (!bIsChangeMode)
            {
                loadRecentHistoryToCombo();

                rdoExistInfo.CheckedChanged += (s, e) =>
                {
                    if (rdoExistInfo.Checked)
                    {
                        lblRecentHistory.Visible = true;
                        cmbRecentHistory.Visible = true;
                        loadRecentHistoryToCombo();
                    }
                };

                rdoNewInfo.CheckedChanged += (s, e) =>
                {
                    if (rdoNewInfo.Checked)
                    {
                        lblRecentHistory.Visible = false;
                        cmbRecentHistory.Visible = false;
                        clearInputFields();
                    }
                };
            }

            int nBtnLocationY = bIsChangeMode ? 320 : 395;

            Button btnOk = new Button
            {
                Text = "확 인",
                Location = new Point(150, nBtnLocationY),
                Size = new Size(130, 42),
                Cursor = Cursors.Hand,
                BackColor = clrNavyPoint,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("맑은 고딕", 10.5F, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;

            this.AcceptButton = btnOk;

            btnOk.Click += (s, e) =>
            {
                string strInputSerial = txtSerial.Text.Trim();
                string strInputFleet = txtFleetNumber.Text.Trim();
                string strInputTrain = txtTrainNumber.Text.Trim();
                string strInputTester = txtTesterName.Text.Trim();

                if (string.IsNullOrEmpty(strInputSerial)) strInputSerial = "0000";
                if (string.IsNullOrEmpty(strInputFleet)) strInputFleet = "0000";
                if (string.IsNullOrEmpty(strInputTrain)) strInputTrain = "0000";
                if (string.IsNullOrEmpty(strInputTester)) strInputTester = "홍길동";

                strTCMSUnit = cmbClassification.SelectedItem?.ToString() ?? "TC";
                strSerialNo = strInputSerial;
                strFleetNo = strInputFleet;
                strTrainNo = strInputTrain;
                strTester = strInputTester;

                // 신규 입력 정보를 JSON 종합 이력(최대 10개)에 갱신
                var clsOp = ConfigJson.CurrentConfig?.Operation;
                if (clsOp != null)
                {
                    var clsNewHistory = new ConfigJson.OperationInfo.CombinedHistoryItem
                    {
                        strUnit = strTCMSUnit,
                        strSerialNo = strSerialNo,
                        strFleetNo = strFleetNo,
                        strTrainNo = strTrainNo,
                        strTesterName = strTester
                    };
                    clsOp.AddCombinedHistory(clsNewHistory);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            grpInputContainer.Controls.Add(lblClassification);
            grpInputContainer.Controls.Add(cmbClassification);
            grpInputContainer.Controls.Add(lblSerial);
            grpInputContainer.Controls.Add(txtSerial);
            grpInputContainer.Controls.Add(lblTrainNumber);
            grpInputContainer.Controls.Add(txtTrainNumber);
            grpInputContainer.Controls.Add(lblFleetNumber);
            grpInputContainer.Controls.Add(txtFleetNumber);
            grpInputContainer.Controls.Add(lblTesterName);
            grpInputContainer.Controls.Add(txtTesterName);

            this.Controls.Add(lblNotice);
            if (!bIsChangeMode)
            {
                this.Controls.Add(rdoExistInfo);
                this.Controls.Add(rdoNewInfo);
                this.Controls.Add(lblRecentHistory);
                this.Controls.Add(cmbRecentHistory);
            }
            this.Controls.Add(grpInputContainer);
            this.Controls.Add(btnOk);
        }
    }
}