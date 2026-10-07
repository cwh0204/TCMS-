using System;
using System.Drawing;
using System.Windows.Forms;

namespace CITester
{
    public partial class FormPlcControl : Form
    {
        private readonly FormMain _mainForm;
        private FlowLayoutPanel panelPins;
        private ComboBox cboPlcSelect;
        private NumericUpDown numStartWord;
        private Button[] btnPins = new Button[32]; // 기본 32개 핀 (필요 시 확장 가능)

        public FormPlcControl(FormMain mainForm)
        {
            _mainForm = mainForm;
            InitializeComponentDynamic();
        }

        private void InitializeComponentDynamic()
        {
            this.Text = "PLC 수동 입출력 제어기 (PLC 1 / PLC 2)";
            this.Size = new Size(680, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 상단 설정 패널
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            Label lblPlc = new Label { Text = "대상 PLC:", Location = new Point(15, 20), AutoSize = true };
            cboPlcSelect = new ComboBox { Location = new Point(80, 16), Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPlcSelect.Items.AddRange(new object[] { "PLC 1", "PLC 2" });
            cboPlcSelect.SelectedIndex = 1; // 기본값 PLC 2

            Label lblWord = new Label { Text = "시작 워드(%PW):", Location = new Point(210, 20), AutoSize = true };
            numStartWord = new NumericUpDown { Location = new Point(320, 17), Width = 60, Minimum = 0, Maximum = 100, Value = 2 };

            Button btnAllOff = new Button { Text = "전체 OFF", Location = new Point(410, 14), Width = 90, Height = 30, BackColor = Color.LightCoral };
            btnAllOff.Click += BtnAllOff_Click;

            Button btnRefresh = new Button { Text = "상태 갱신", Location = new Point(510, 14), Width = 90, Height = 30 };
            btnRefresh.Click += (s, e) => UpdatePinButtonStates();

            topPanel.Controls.AddRange(new Control[] { lblPlc, cboPlcSelect, lblWord, numStartWord, btnAllOff, btnRefresh });

            // 중앙 핀 버튼 영역
            panelPins = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            // 0번부터 31번 핀 버튼 생성
            for (int i = 0; i < btnPins.Length; i++)
            {
                int pinNo = i;
                Button btn = new Button
                {
                    Text = $"Pin {pinNo}\n[OFF]",
                    Size = new Size(72, 60),
                    Margin = new Padding(4),
                    BackColor = Color.LightGray,
                    Tag = pinNo
                };
                btn.Click += PinButton_Click;
                btnPins[i] = btn;
                panelPins.Controls.Add(btn);
            }

            this.Controls.Add(panelPins);
            this.Controls.Add(topPanel);

            cboPlcSelect.SelectedIndexChanged += (s, e) => UpdatePinButtonStates();
            UpdatePinButtonStates();
        }

        private classCnet GetCurrentPlc()
        {
            return cboPlcSelect.SelectedIndex == 0 ? _mainForm?.c_PLCNetwork : _mainForm?.c_PLCNetwork2;
        }

        private void PinButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int pinNo)
            {
                var plc = GetCurrentPlc();
                if (plc == null || plc.serialPort == null || !plc.serialPort.IsOpen)
                {
                    MessageBox.Show("선택된 PLC 포트가 연결되어 있지 않습니다.", "통신 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int startWord = (int)numStartWord.Value;
                bool currentOn = plc.m_bDOValue != null && plc.m_bDOValue[pinNo];
                bool nextState = !currentOn;

                // PLC DO 전송
                plc.SetDo(pinNo, nextState, startWord, 0);

                // 버튼 색상 및 텍스트 즉시 반영
                btn.BackColor = nextState ? Color.LimeGreen : Color.LightGray;
                btn.Text = $"Pin {pinNo}\n[{(nextState ? "ON" : "OFF")}]";
            }
        }

        private void BtnAllOff_Click(object sender, EventArgs e)
        {
            var plc = GetCurrentPlc();
            if (plc == null || plc.serialPort == null || !plc.serialPort.IsOpen)
            {
                MessageBox.Show("선택된 PLC 포트가 연결되어 있지 않습니다.", "통신 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int startWord = (int)numStartWord.Value;

            for (int pin = 0; pin < btnPins.Length; pin++)
            {
                plc.SetDo(pin, false, startWord, 0);
            }

            UpdatePinButtonStates();
            MessageBox.Show("전체 핀을 OFF로 초기화했습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdatePinButtonStates()
        {
            var plc = GetCurrentPlc();
            for (int i = 0; i < btnPins.Length; i++)
            {
                bool isOn = plc?.m_bDOValue != null && plc.m_bDOValue[i];
                btnPins[i].BackColor = isOn ? Color.LimeGreen : Color.LightGray;
                btnPins[i].Text = $"Pin {i}\n[{(isOn ? "ON" : "OFF")}]";
            }
        }
    }
}