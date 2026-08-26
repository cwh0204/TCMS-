namespace CITester
{
    partial class FormLoad
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_Timer = new System.Windows.Forms.Timer(this.components);
            this.label80 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.m_lbResult7 = new System.Windows.Forms.Label();
            this.m_chkStep7 = new System.Windows.Forms.CheckBox();
            this.customProgressBar1 = new YourNamespace.CustomProgressBar();
            this.roundedLabel1 = new CITester.RoundedLabel();
            this.roundedPanel1 = new CITester.RoundedPanel();
            this.m_lbResult14 = new System.Windows.Forms.Label();
            this.m_chkStep6 = new System.Windows.Forms.CheckBox();
            this.m_chkStep14 = new System.Windows.Forms.CheckBox();
            this.m_chkStep2 = new System.Windows.Forms.CheckBox();
            this.m_lbResult13 = new System.Windows.Forms.Label();
            this.m_chkStep1 = new System.Windows.Forms.CheckBox();
            this.m_chkStep13 = new System.Windows.Forms.CheckBox();
            this.m_lbResult1 = new System.Windows.Forms.Label();
            this.m_lbResult12 = new System.Windows.Forms.Label();
            this.m_lbResult2 = new System.Windows.Forms.Label();
            this.m_chkStep12 = new System.Windows.Forms.CheckBox();
            this.m_chkStep3 = new System.Windows.Forms.CheckBox();
            this.m_lbResult11 = new System.Windows.Forms.Label();
            this.m_lbResult3 = new System.Windows.Forms.Label();
            this.m_chkStep11 = new System.Windows.Forms.CheckBox();
            this.m_chkStep4 = new System.Windows.Forms.CheckBox();
            this.m_lbResult10 = new System.Windows.Forms.Label();
            this.m_lbResult4 = new System.Windows.Forms.Label();
            this.m_lbResult9 = new System.Windows.Forms.Label();
            this.m_lbResult5 = new System.Windows.Forms.Label();
            this.m_chkStep10 = new System.Windows.Forms.CheckBox();
            this.m_lbResult6 = new System.Windows.Forms.Label();
            this.m_chkStep9 = new System.Windows.Forms.CheckBox();
            this.m_chkStep5 = new System.Windows.Forms.CheckBox();
            this.m_chkStep8 = new System.Windows.Forms.CheckBox();
            this.m_lbResult8 = new System.Windows.Forms.Label();
            this.roundedPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_Timer
            // 
            this.m_Timer.Interval = 500;
            this.m_Timer.Tick += new System.EventHandler(this.M_Timer_Tick);
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold);
            this.label80.ForeColor = System.Drawing.Color.Black;
            this.label80.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label80.Location = new System.Drawing.Point(138, 143);
            this.label80.Margin = new System.Windows.Forms.Padding(3);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(352, 32);
            this.label80.TabIndex = 62;
            this.label80.Text = "TCMS 시험기 프로그램 시작 중";
            this.label80.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.label1.Location = new System.Drawing.Point(177, 264);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 12);
            this.label1.TabIndex = 64;
            this.label1.Text = "프로그램을 시작하는 동안 잠시만 기다려주세요.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lbResult7
            // 
            this.m_lbResult7.BackColor = System.Drawing.Color.White;
            this.m_lbResult7.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult7.ForeColor = System.Drawing.Color.RoyalBlue;
            this.m_lbResult7.Location = new System.Drawing.Point(341, 796);
            this.m_lbResult7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult7.Name = "m_lbResult7";
            this.m_lbResult7.Size = new System.Drawing.Size(49, 18);
            this.m_lbResult7.TabIndex = 32;
            this.m_lbResult7.Text = "OK";
            this.m_lbResult7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_lbResult7.Visible = false;
            // 
            // m_chkStep7
            // 
            this.m_chkStep7.BackColor = System.Drawing.Color.White;
            this.m_chkStep7.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep7.Location = new System.Drawing.Point(26, 788);
            this.m_chkStep7.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep7.Name = "m_chkStep7";
            this.m_chkStep7.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep7.TabIndex = 34;
            this.m_chkStep7.Text = "PWM 신호 생성기 연결상태 확인";
            this.m_chkStep7.UseVisualStyleBackColor = false;
            this.m_chkStep7.Visible = false;
            // 
            // customProgressBar1
            // 
            this.customProgressBar1.BarThickness = 10;
            this.customProgressBar1.CornerRadius = 30;
            this.customProgressBar1.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.customProgressBar1.ForeColor = System.Drawing.Color.Black;
            this.customProgressBar1.Location = new System.Drawing.Point(152, 187);
            this.customProgressBar1.Maximum = 100;
            this.customProgressBar1.Name = "customProgressBar1";
            this.customProgressBar1.ProgressColor = System.Drawing.Color.DodgerBlue;
            this.customProgressBar1.Size = new System.Drawing.Size(331, 37);
            this.customProgressBar1.TabIndex = 65;
            this.customProgressBar1.Text = "customProgressBar1";
            this.customProgressBar1.TextMargin = 10;
            this.customProgressBar1.TrackColor = System.Drawing.Color.LightGray;
            this.customProgressBar1.UseAnimation = true;
            this.customProgressBar1.Value = 100;
            // 
            // roundedLabel1
            // 
            this.roundedLabel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel1.CustomImage = global::TCMSTester.Properties.Resources.ChatGPT_Image_2026년_5월_19일_오전_09_14_23;
            this.roundedLabel1.FillColor = System.Drawing.Color.Transparent;
            this.roundedLabel1.ImageSize = new System.Drawing.Size(250, 270);
            this.roundedLabel1.Location = new System.Drawing.Point(84, 31);
            this.roundedLabel1.Margin = new System.Windows.Forms.Padding(3);
            this.roundedLabel1.Name = "roundedLabel1";
            this.roundedLabel1.Size = new System.Drawing.Size(439, 113);
            this.roundedLabel1.TabIndex = 63;
            this.roundedLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedPanel1
            // 
            this.roundedPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.roundedPanel1.Controls.Add(this.m_lbResult14);
            this.roundedPanel1.Controls.Add(this.m_chkStep6);
            this.roundedPanel1.Controls.Add(this.m_chkStep14);
            this.roundedPanel1.Controls.Add(this.m_chkStep2);
            this.roundedPanel1.Controls.Add(this.m_lbResult13);
            this.roundedPanel1.Controls.Add(this.m_chkStep1);
            this.roundedPanel1.Controls.Add(this.m_chkStep13);
            this.roundedPanel1.Controls.Add(this.m_lbResult1);
            this.roundedPanel1.Controls.Add(this.m_lbResult12);
            this.roundedPanel1.Controls.Add(this.m_lbResult2);
            this.roundedPanel1.Controls.Add(this.m_chkStep12);
            this.roundedPanel1.Controls.Add(this.m_chkStep3);
            this.roundedPanel1.Controls.Add(this.m_lbResult11);
            this.roundedPanel1.Controls.Add(this.m_lbResult3);
            this.roundedPanel1.Controls.Add(this.m_chkStep11);
            this.roundedPanel1.Controls.Add(this.m_chkStep4);
            this.roundedPanel1.Controls.Add(this.m_lbResult10);
            this.roundedPanel1.Controls.Add(this.m_lbResult4);
            this.roundedPanel1.Controls.Add(this.m_lbResult9);
            this.roundedPanel1.Controls.Add(this.m_lbResult5);
            this.roundedPanel1.Controls.Add(this.m_chkStep10);
            this.roundedPanel1.Controls.Add(this.m_lbResult6);
            this.roundedPanel1.Controls.Add(this.m_chkStep9);
            this.roundedPanel1.Controls.Add(this.m_chkStep5);
            this.roundedPanel1.Controls.Add(this.m_chkStep8);
            this.roundedPanel1.Controls.Add(this.m_lbResult8);
            this.roundedPanel1.Location = new System.Drawing.Point(86, 198);
            this.roundedPanel1.Name = "roundedPanel1";
            this.roundedPanel1.Size = new System.Drawing.Size(529, 560);
            this.roundedPanel1.TabIndex = 66;
            this.roundedPanel1.Visible = false;
            // 
            // m_lbResult14
            // 
            this.m_lbResult14.BackColor = System.Drawing.Color.White;
            this.m_lbResult14.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult14.Location = new System.Drawing.Point(442, 510);
            this.m_lbResult14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult14.Name = "m_lbResult14";
            this.m_lbResult14.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult14.TabIndex = 59;
            this.m_lbResult14.Text = "대기중";
            this.m_lbResult14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_chkStep6
            // 
            this.m_chkStep6.BackColor = System.Drawing.Color.White;
            this.m_chkStep6.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep6.Location = new System.Drawing.Point(27, 228);
            this.m_chkStep6.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep6.Name = "m_chkStep6";
            this.m_chkStep6.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep6.TabIndex = 28;
            this.m_chkStep6.Text = "PG, PWM 출력보드 연결상태 확인 ";
            this.m_chkStep6.UseVisualStyleBackColor = false;
            // 
            // m_chkStep14
            // 
            this.m_chkStep14.BackColor = System.Drawing.Color.White;
            this.m_chkStep14.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep14.Location = new System.Drawing.Point(27, 508);
            this.m_chkStep14.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep14.Name = "m_chkStep14";
            this.m_chkStep14.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep14.TabIndex = 58;
            this.m_chkStep14.Text = "Line_Voltage 출력보드 연결상태 확인";
            this.m_chkStep14.UseVisualStyleBackColor = false;
            // 
            // m_chkStep2
            // 
            this.m_chkStep2.BackColor = System.Drawing.Color.White;
            this.m_chkStep2.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep2.Location = new System.Drawing.Point(27, 68);
            this.m_chkStep2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep2.Name = "m_chkStep2";
            this.m_chkStep2.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep2.TabIndex = 2;
            this.m_chkStep2.Text = "PLC 연결상태 확인";
            this.m_chkStep2.UseVisualStyleBackColor = false;
            // 
            // m_lbResult13
            // 
            this.m_lbResult13.BackColor = System.Drawing.Color.White;
            this.m_lbResult13.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult13.Location = new System.Drawing.Point(442, 470);
            this.m_lbResult13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult13.Name = "m_lbResult13";
            this.m_lbResult13.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult13.TabIndex = 57;
            this.m_lbResult13.Text = "대기중";
            this.m_lbResult13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_chkStep1
            // 
            this.m_chkStep1.BackColor = System.Drawing.Color.White;
            this.m_chkStep1.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep1.Location = new System.Drawing.Point(27, 28);
            this.m_chkStep1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep1.Name = "m_chkStep1";
            this.m_chkStep1.Size = new System.Drawing.Size(349, 26);
            this.m_chkStep1.TabIndex = 1;
            this.m_chkStep1.Text = "환경설정 및 데이터베이스 초기화";
            this.m_chkStep1.UseVisualStyleBackColor = false;
            // 
            // m_chkStep13
            // 
            this.m_chkStep13.BackColor = System.Drawing.Color.White;
            this.m_chkStep13.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep13.Location = new System.Drawing.Point(27, 468);
            this.m_chkStep13.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep13.Name = "m_chkStep13";
            this.m_chkStep13.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep13.TabIndex = 56;
            this.m_chkStep13.Text = "TRIMER2 보드 연결상태 확인";
            this.m_chkStep13.UseVisualStyleBackColor = false;
            // 
            // m_lbResult1
            // 
            this.m_lbResult1.BackColor = System.Drawing.Color.White;
            this.m_lbResult1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult1.Location = new System.Drawing.Point(442, 30);
            this.m_lbResult1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult1.Name = "m_lbResult1";
            this.m_lbResult1.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult1.TabIndex = 11;
            this.m_lbResult1.Text = "대기중";
            this.m_lbResult1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lbResult12
            // 
            this.m_lbResult12.BackColor = System.Drawing.Color.White;
            this.m_lbResult12.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult12.Location = new System.Drawing.Point(442, 430);
            this.m_lbResult12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult12.Name = "m_lbResult12";
            this.m_lbResult12.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult12.TabIndex = 55;
            this.m_lbResult12.Text = "대기중";
            this.m_lbResult12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lbResult2
            // 
            this.m_lbResult2.BackColor = System.Drawing.Color.White;
            this.m_lbResult2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult2.Location = new System.Drawing.Point(442, 70);
            this.m_lbResult2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult2.Name = "m_lbResult2";
            this.m_lbResult2.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult2.TabIndex = 12;
            this.m_lbResult2.Text = "대기중";
            this.m_lbResult2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_chkStep12
            // 
            this.m_chkStep12.BackColor = System.Drawing.Color.White;
            this.m_chkStep12.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep12.Location = new System.Drawing.Point(27, 428);
            this.m_chkStep12.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep12.Name = "m_chkStep12";
            this.m_chkStep12.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep12.TabIndex = 54;
            this.m_chkStep12.Text = "TRIMER1 보드 연결상태 확인";
            this.m_chkStep12.UseVisualStyleBackColor = false;
            // 
            // m_chkStep3
            // 
            this.m_chkStep3.BackColor = System.Drawing.Color.White;
            this.m_chkStep3.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep3.Location = new System.Drawing.Point(27, 108);
            this.m_chkStep3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep3.Name = "m_chkStep3";
            this.m_chkStep3.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep3.TabIndex = 19;
            this.m_chkStep3.Text = "오실로스코프 연결상태 확인";
            this.m_chkStep3.UseVisualStyleBackColor = false;
            // 
            // m_lbResult11
            // 
            this.m_lbResult11.BackColor = System.Drawing.Color.White;
            this.m_lbResult11.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult11.Location = new System.Drawing.Point(442, 350);
            this.m_lbResult11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult11.Name = "m_lbResult11";
            this.m_lbResult11.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult11.TabIndex = 53;
            this.m_lbResult11.Text = "대기중";
            this.m_lbResult11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lbResult3
            // 
            this.m_lbResult3.BackColor = System.Drawing.Color.White;
            this.m_lbResult3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult3.Location = new System.Drawing.Point(442, 110);
            this.m_lbResult3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult3.Name = "m_lbResult3";
            this.m_lbResult3.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult3.TabIndex = 20;
            this.m_lbResult3.Text = "대기중";
            this.m_lbResult3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_chkStep11
            // 
            this.m_chkStep11.BackColor = System.Drawing.Color.White;
            this.m_chkStep11.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep11.Location = new System.Drawing.Point(27, 348);
            this.m_chkStep11.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep11.Name = "m_chkStep11";
            this.m_chkStep11.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep11.TabIndex = 52;
            this.m_chkStep11.Text = "광보드2 연결상태 확인";
            this.m_chkStep11.UseVisualStyleBackColor = false;
            // 
            // m_chkStep4
            // 
            this.m_chkStep4.BackColor = System.Drawing.Color.White;
            this.m_chkStep4.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep4.Location = new System.Drawing.Point(27, 148);
            this.m_chkStep4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep4.Name = "m_chkStep4";
            this.m_chkStep4.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep4.TabIndex = 23;
            this.m_chkStep4.Text = "디지털 멀티미터 연결상태 확인";
            this.m_chkStep4.UseVisualStyleBackColor = false;
            // 
            // m_lbResult10
            // 
            this.m_lbResult10.BackColor = System.Drawing.Color.White;
            this.m_lbResult10.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult10.Location = new System.Drawing.Point(442, 190);
            this.m_lbResult10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult10.Name = "m_lbResult10";
            this.m_lbResult10.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult10.TabIndex = 51;
            this.m_lbResult10.Text = "대기중";
            this.m_lbResult10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lbResult4
            // 
            this.m_lbResult4.BackColor = System.Drawing.Color.White;
            this.m_lbResult4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult4.Location = new System.Drawing.Point(442, 150);
            this.m_lbResult4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult4.Name = "m_lbResult4";
            this.m_lbResult4.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult4.TabIndex = 24;
            this.m_lbResult4.Text = "대기중";
            this.m_lbResult4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lbResult9
            // 
            this.m_lbResult9.BackColor = System.Drawing.Color.White;
            this.m_lbResult9.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult9.Location = new System.Drawing.Point(442, 310);
            this.m_lbResult9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult9.Name = "m_lbResult9";
            this.m_lbResult9.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult9.TabIndex = 50;
            this.m_lbResult9.Text = "대기중";
            this.m_lbResult9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lbResult5
            // 
            this.m_lbResult5.BackColor = System.Drawing.Color.White;
            this.m_lbResult5.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult5.Location = new System.Drawing.Point(442, 390);
            this.m_lbResult5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult5.Name = "m_lbResult5";
            this.m_lbResult5.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult5.TabIndex = 25;
            this.m_lbResult5.Text = "대기중";
            this.m_lbResult5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_chkStep10
            // 
            this.m_chkStep10.BackColor = System.Drawing.Color.White;
            this.m_chkStep10.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep10.Location = new System.Drawing.Point(27, 188);
            this.m_chkStep10.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep10.Name = "m_chkStep10";
            this.m_chkStep10.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep10.TabIndex = 46;
            this.m_chkStep10.Text = "MVB 보드 연결상태 확인";
            this.m_chkStep10.UseVisualStyleBackColor = false;
            // 
            // m_lbResult6
            // 
            this.m_lbResult6.BackColor = System.Drawing.Color.White;
            this.m_lbResult6.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult6.Location = new System.Drawing.Point(442, 230);
            this.m_lbResult6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult6.Name = "m_lbResult6";
            this.m_lbResult6.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult6.TabIndex = 29;
            this.m_lbResult6.Text = "대기중";
            this.m_lbResult6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_chkStep9
            // 
            this.m_chkStep9.BackColor = System.Drawing.Color.White;
            this.m_chkStep9.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep9.Location = new System.Drawing.Point(27, 308);
            this.m_chkStep9.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep9.Name = "m_chkStep9";
            this.m_chkStep9.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep9.TabIndex = 45;
            this.m_chkStep9.Text = "광보드1 연결상태 확인";
            this.m_chkStep9.UseVisualStyleBackColor = false;
            // 
            // m_chkStep5
            // 
            this.m_chkStep5.BackColor = System.Drawing.Color.White;
            this.m_chkStep5.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep5.Location = new System.Drawing.Point(27, 388);
            this.m_chkStep5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep5.Name = "m_chkStep5";
            this.m_chkStep5.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep5.TabIndex = 31;
            this.m_chkStep5.Text = "DC 가변파워 연결상태 확인";
            this.m_chkStep5.UseVisualStyleBackColor = false;
            // 
            // m_chkStep8
            // 
            this.m_chkStep8.BackColor = System.Drawing.Color.White;
            this.m_chkStep8.Font = new System.Drawing.Font("맑은 고딕", 12.75F);
            this.m_chkStep8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_chkStep8.Location = new System.Drawing.Point(27, 268);
            this.m_chkStep8.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.m_chkStep8.Name = "m_chkStep8";
            this.m_chkStep8.Size = new System.Drawing.Size(320, 26);
            this.m_chkStep8.TabIndex = 44;
            this.m_chkStep8.Text = "PCB 연결상태 확인";
            this.m_chkStep8.UseVisualStyleBackColor = false;
            // 
            // m_lbResult8
            // 
            this.m_lbResult8.BackColor = System.Drawing.Color.White;
            this.m_lbResult8.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.m_lbResult8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.m_lbResult8.Location = new System.Drawing.Point(442, 270);
            this.m_lbResult8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_lbResult8.Name = "m_lbResult8";
            this.m_lbResult8.Size = new System.Drawing.Size(80, 18);
            this.m_lbResult8.TabIndex = 35;
            this.m_lbResult8.Text = "대기중";
            this.m_lbResult8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormLoad
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(615, 300);
            this.ControlBox = false;
            this.Controls.Add(this.customProgressBar1);
            this.Controls.Add(this.roundedLabel1);
            this.Controls.Add(this.label80);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.roundedPanel1);
            this.Controls.Add(this.m_chkStep7);
            this.Controls.Add(this.m_lbResult7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLoad";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TCMS Tester";
            this.Load += new System.EventHandler(this.FormLoad_Load);
            this.roundedPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer m_Timer;
        private YourNamespace.CustomProgressBar customProgressBar1;
        private RoundedLabel roundedLabel1;
        private System.Windows.Forms.Label label80;
        private System.Windows.Forms.Label label1;
        private RoundedPanel roundedPanel1;
        private System.Windows.Forms.Label m_lbResult14;
        private System.Windows.Forms.CheckBox m_chkStep6;
        private System.Windows.Forms.CheckBox m_chkStep14;
        private System.Windows.Forms.CheckBox m_chkStep2;
        private System.Windows.Forms.Label m_lbResult13;
        private System.Windows.Forms.CheckBox m_chkStep1;
        private System.Windows.Forms.CheckBox m_chkStep13;
        private System.Windows.Forms.Label m_lbResult1;
        private System.Windows.Forms.Label m_lbResult12;
        private System.Windows.Forms.Label m_lbResult2;
        private System.Windows.Forms.CheckBox m_chkStep12;
        private System.Windows.Forms.CheckBox m_chkStep3;
        private System.Windows.Forms.Label m_lbResult11;
        private System.Windows.Forms.Label m_lbResult3;
        private System.Windows.Forms.CheckBox m_chkStep11;
        private System.Windows.Forms.CheckBox m_chkStep4;
        private System.Windows.Forms.Label m_lbResult10;
        private System.Windows.Forms.Label m_lbResult4;
        private System.Windows.Forms.Label m_lbResult9;
        private System.Windows.Forms.Label m_lbResult5;
        private System.Windows.Forms.CheckBox m_chkStep10;
        private System.Windows.Forms.Label m_lbResult6;
        private System.Windows.Forms.CheckBox m_chkStep9;
        private System.Windows.Forms.CheckBox m_chkStep5;
        private System.Windows.Forms.CheckBox m_chkStep8;
        private System.Windows.Forms.Label m_lbResult7;
        private System.Windows.Forms.Label m_lbResult8;
        private System.Windows.Forms.CheckBox m_chkStep7;
    }
}