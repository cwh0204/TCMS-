namespace CITester
{
    partial class FormResultView
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnClose = new CustomIconButton();
            this.BtnConfig = new CustomIconButton();
            this.roundedPanel3 = new CITester.RoundedPanel();
            this.TestResult_Comn = new CustomIconButton();
            this.dataGridViewComm = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.roundedPanel4 = new CITester.RoundedPanel();
            this.TestResult_Memory = new CustomIconButton();
            this.dataGridViewMemory = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.roundedPanel5 = new CITester.RoundedPanel();
            this.imagebtn2 = new CustomIconButton();
            this.roundedLabel11 = new CITester.RoundedLabel();
            this.richTextBox_Err = new System.Windows.Forms.RichTextBox();
            this.roundedPanel2 = new CITester.RoundedPanel();
            this.TestResult_IO = new CustomIconButton();
            this.dataGridViewDIO = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.roundedPanel1 = new CITester.RoundedPanel();
            this.roundedLabel13 = new CITester.RoundedLabel();
            this.Label_FinalResult = new CITester.CITesterLabel();
            this.Label_Round = new CITester.CITesterLabel();
            this.roundedLabel14 = new CITester.RoundedLabel();
            this.roundedLabel8 = new CITester.RoundedLabel();
            this.roundedPanel6 = new CITester.RoundedPanel();
            this.roundedLabel1 = new CITester.RoundedLabel();
            this.Label_Tester = new CITester.CITesterLabel();
            this.Label_Train = new CITester.CITesterLabel();
            this.Label_Fleet = new CITester.CITesterLabel();
            this.Label_Serial = new CITester.CITesterLabel();
            this.roundedLabel7 = new CITester.RoundedLabel();
            this.roundedLabel6 = new CITester.RoundedLabel();
            this.roundedLabel5 = new CITester.RoundedLabel();
            this.roundedLabel4 = new CITester.RoundedLabel();
            this.roundedLabel3 = new CITester.RoundedLabel();
            this.Label_Unit = new CITester.CITesterLabel();
            this.Label_Date = new CITester.CITesterLabel();
            this.roundedLabel2 = new CITester.RoundedLabel();
            this.roundedPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewComm)).BeginInit();
            this.roundedPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMemory)).BeginInit();
            this.roundedPanel5.SuspendLayout();
            this.roundedPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDIO)).BeginInit();
            this.roundedPanel1.SuspendLayout();
            this.roundedPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnClose
            // 
            this.BtnClose.AutoCenterIcon = false;
            this.BtnClose.AutoCenterText = false;
            this.BtnClose.BackColor = System.Drawing.Color.White;
            this.BtnClose.BaseBorderColor = System.Drawing.Color.Brown;
            this.BtnClose.BaseBorderThickness = 1;
            this.BtnClose.CornerRadius = 10;
            this.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClose.FlatAppearance.BorderSize = 0;
            this.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnClose.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            this.BtnClose.ForeColor = System.Drawing.Color.Brown;
            this.BtnClose.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnClose.HoverBorderColor = System.Drawing.Color.Brown;
            this.BtnClose.HoverBorderThickness = 3;
            this.BtnClose.IconLocation = new System.Drawing.Point(15, 8);
            this.BtnClose.IconScale = 0.6F;
            this.BtnClose.Image = global::TCMSTester.Properties.Resources.cross;
            this.BtnClose.Location = new System.Drawing.Point(994, 8);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnClose.Size = new System.Drawing.Size(134, 42);
            this.BtnClose.TabIndex = 102;
            this.BtnClose.Text = "종 료";
            this.BtnClose.TextBottomMargin = 7;
            this.BtnClose.TextLocation = new System.Drawing.Point(47, 7);
            this.BtnClose.UseHoverBackColor = false;
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click_2);
            // 
            // BtnConfig
            // 
            this.BtnConfig.AutoCenterIcon = false;
            this.BtnConfig.AutoCenterText = false;
            this.BtnConfig.BackColor = System.Drawing.Color.White;
            this.BtnConfig.BaseBorderColor = System.Drawing.Color.SteelBlue;
            this.BtnConfig.BaseBorderThickness = 1;
            this.BtnConfig.CornerRadius = 10;
            this.BtnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnConfig.FlatAppearance.BorderSize = 0;
            this.BtnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnConfig.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            this.BtnConfig.ForeColor = System.Drawing.Color.SteelBlue;
            this.BtnConfig.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnConfig.HoverBorderColor = System.Drawing.Color.SteelBlue;
            this.BtnConfig.HoverBorderThickness = 3;
            this.BtnConfig.IconLocation = new System.Drawing.Point(15, 7);
            this.BtnConfig.IconScale = 0.6F;
            this.BtnConfig.Image = global::TCMSTester.Properties.Resources.printer;
            this.BtnConfig.Location = new System.Drawing.Point(846, 8);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnConfig.Size = new System.Drawing.Size(134, 42);
            this.BtnConfig.TabIndex = 101;
            this.BtnConfig.Text = "인 쇄";
            this.BtnConfig.TextBottomMargin = 7;
            this.BtnConfig.TextLocation = new System.Drawing.Point(47, 7);
            this.BtnConfig.UseHoverBackColor = false;
            this.BtnConfig.UseVisualStyleBackColor = false;
            // 
            // roundedPanel3
            // 
            this.roundedPanel3.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel3.Controls.Add(this.TestResult_Comn);
            this.roundedPanel3.Controls.Add(this.dataGridViewComm);
            this.roundedPanel3.Controls.Add(this.label2);
            this.roundedPanel3.CornerRadius = 10;
            this.roundedPanel3.Location = new System.Drawing.Point(397, 384);
            this.roundedPanel3.Name = "roundedPanel3";
            this.roundedPanel3.Size = new System.Drawing.Size(731, 226);
            this.roundedPanel3.TabIndex = 100;
            // 
            // TestResult_Comn
            // 
            this.TestResult_Comn.AutoCenterIcon = true;
            this.TestResult_Comn.AutoCenterText = true;
            this.TestResult_Comn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(245)))), ((int)(((byte)(230)))));
            this.TestResult_Comn.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(226)))), ((int)(((byte)(199)))));
            this.TestResult_Comn.BaseBorderThickness = 1;
            this.TestResult_Comn.CornerRadius = 10;
            this.TestResult_Comn.Enabled = false;
            this.TestResult_Comn.FlatAppearance.BorderSize = 0;
            this.TestResult_Comn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TestResult_Comn.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.TestResult_Comn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(93)))), ((int)(((byte)(24)))));
            this.TestResult_Comn.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.TestResult_Comn.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.TestResult_Comn.HoverBorderThickness = 3;
            this.TestResult_Comn.IconLocation = new System.Drawing.Point(0, 0);
            this.TestResult_Comn.IconScale = 0.4F;
            this.TestResult_Comn.Location = new System.Drawing.Point(642, 13);
            this.TestResult_Comn.Name = "TestResult_Comn";
            this.TestResult_Comn.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.TestResult_Comn.Size = new System.Drawing.Size(67, 37);
            this.TestResult_Comn.TabIndex = 94;
            this.TestResult_Comn.Text = "합 격";
            this.TestResult_Comn.TextBottomMargin = 9;
            this.TestResult_Comn.TextLocation = new System.Drawing.Point(0, 0);
            this.TestResult_Comn.UseHoverBackColor = false;
            this.TestResult_Comn.UseVisualStyleBackColor = false;
            // 
            // dataGridViewComm
            // 
            this.dataGridViewComm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewComm.Location = new System.Drawing.Point(15, 60);
            this.dataGridViewComm.Name = "dataGridViewComm";
            this.dataGridViewComm.RowTemplate.Height = 23;
            this.dataGridViewComm.Size = new System.Drawing.Size(700, 147);
            this.dataGridViewComm.TabIndex = 92;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(19, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 25);
            this.label2.TabIndex = 93;
            this.label2.Text = "통신 시험 결과";
            // 
            // roundedPanel4
            // 
            this.roundedPanel4.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel4.Controls.Add(this.TestResult_Memory);
            this.roundedPanel4.Controls.Add(this.dataGridViewMemory);
            this.roundedPanel4.Controls.Add(this.label3);
            this.roundedPanel4.CornerRadius = 10;
            this.roundedPanel4.Location = new System.Drawing.Point(397, 616);
            this.roundedPanel4.Name = "roundedPanel4";
            this.roundedPanel4.Size = new System.Drawing.Size(731, 226);
            this.roundedPanel4.TabIndex = 99;
            // 
            // TestResult_Memory
            // 
            this.TestResult_Memory.AutoCenterIcon = true;
            this.TestResult_Memory.AutoCenterText = true;
            this.TestResult_Memory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(245)))), ((int)(((byte)(230)))));
            this.TestResult_Memory.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(226)))), ((int)(((byte)(199)))));
            this.TestResult_Memory.BaseBorderThickness = 1;
            this.TestResult_Memory.CornerRadius = 10;
            this.TestResult_Memory.Enabled = false;
            this.TestResult_Memory.FlatAppearance.BorderSize = 0;
            this.TestResult_Memory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TestResult_Memory.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.TestResult_Memory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(93)))), ((int)(((byte)(24)))));
            this.TestResult_Memory.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.TestResult_Memory.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.TestResult_Memory.HoverBorderThickness = 3;
            this.TestResult_Memory.IconLocation = new System.Drawing.Point(0, 0);
            this.TestResult_Memory.IconScale = 0.4F;
            this.TestResult_Memory.Location = new System.Drawing.Point(642, 13);
            this.TestResult_Memory.Name = "TestResult_Memory";
            this.TestResult_Memory.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.TestResult_Memory.Size = new System.Drawing.Size(67, 37);
            this.TestResult_Memory.TabIndex = 94;
            this.TestResult_Memory.Text = "합 격";
            this.TestResult_Memory.TextBottomMargin = 9;
            this.TestResult_Memory.TextLocation = new System.Drawing.Point(0, 0);
            this.TestResult_Memory.UseHoverBackColor = false;
            this.TestResult_Memory.UseVisualStyleBackColor = false;
            // 
            // dataGridViewMemory
            // 
            this.dataGridViewMemory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMemory.Location = new System.Drawing.Point(15, 60);
            this.dataGridViewMemory.Name = "dataGridViewMemory";
            this.dataGridViewMemory.RowTemplate.Height = 23;
            this.dataGridViewMemory.Size = new System.Drawing.Size(700, 147);
            this.dataGridViewMemory.TabIndex = 92;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(19, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 25);
            this.label3.TabIndex = 93;
            this.label3.Text = "메모리 시험 결과";
            // 
            // roundedPanel5
            // 
            this.roundedPanel5.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel5.Controls.Add(this.imagebtn2);
            this.roundedPanel5.Controls.Add(this.roundedLabel11);
            this.roundedPanel5.Controls.Add(this.richTextBox_Err);
            this.roundedPanel5.CornerRadius = 10;
            this.roundedPanel5.Location = new System.Drawing.Point(29, 584);
            this.roundedPanel5.Name = "roundedPanel5";
            this.roundedPanel5.Size = new System.Drawing.Size(348, 257);
            this.roundedPanel5.TabIndex = 97;
            // 
            // imagebtn2
            // 
            this.imagebtn2.AutoCenterIcon = true;
            this.imagebtn2.AutoCenterText = true;
            this.imagebtn2.BackColor = System.Drawing.Color.White;
            this.imagebtn2.BaseBorderColor = System.Drawing.Color.Gray;
            this.imagebtn2.BaseBorderThickness = 0;
            this.imagebtn2.CornerRadius = 1;
            this.imagebtn2.Enabled = false;
            this.imagebtn2.FlatAppearance.BorderSize = 0;
            this.imagebtn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.imagebtn2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imagebtn2.ForeColor = System.Drawing.Color.DarkGray;
            this.imagebtn2.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.imagebtn2.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.imagebtn2.HoverBorderThickness = 0;
            this.imagebtn2.IconLocation = new System.Drawing.Point(0, 0);
            this.imagebtn2.IconScale = 0.4F;
            this.imagebtn2.Image = global::TCMSTester.Properties.Resources.note;
            this.imagebtn2.Location = new System.Drawing.Point(56, 89);
            this.imagebtn2.Name = "imagebtn2";
            this.imagebtn2.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.imagebtn2.Size = new System.Drawing.Size(237, 120);
            this.imagebtn2.TabIndex = 88;
            this.imagebtn2.Text = "오류가 없습니다.";
            this.imagebtn2.TextBottomMargin = 15;
            this.imagebtn2.TextLocation = new System.Drawing.Point(0, 0);
            this.imagebtn2.UseHoverBackColor = true;
            this.imagebtn2.UseVisualStyleBackColor = false;
            // 
            // roundedLabel11
            // 
            this.roundedLabel11.AutoCenterImage = false;
            this.roundedLabel11.AutoCenterText = false;
            this.roundedLabel11.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel11.CustomImage = global::TCMSTester.Properties.Resources.report__12_;
            this.roundedLabel11.FillColor = System.Drawing.Color.White;
            this.roundedLabel11.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel11.ImageLocation = new System.Drawing.Point(5, 3);
            this.roundedLabel11.Location = new System.Drawing.Point(13, 12);
            this.roundedLabel11.Name = "roundedLabel11";
            this.roundedLabel11.Size = new System.Drawing.Size(176, 40);
            this.roundedLabel11.TabIndex = 69;
            this.roundedLabel11.Text = "오류 목록";
            this.roundedLabel11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.roundedLabel11.TextLocation = new System.Drawing.Point(43, 8);
            // 
            // richTextBox_Err
            // 
            this.richTextBox_Err.Location = new System.Drawing.Point(21, 59);
            this.richTextBox_Err.Name = "richTextBox_Err";
            this.richTextBox_Err.Size = new System.Drawing.Size(309, 184);
            this.richTextBox_Err.TabIndex = 87;
            this.richTextBox_Err.Text = "";
            // 
            // roundedPanel2
            // 
            this.roundedPanel2.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel2.Controls.Add(this.TestResult_IO);
            this.roundedPanel2.Controls.Add(this.dataGridViewDIO);
            this.roundedPanel2.Controls.Add(this.label1);
            this.roundedPanel2.CornerRadius = 10;
            this.roundedPanel2.Location = new System.Drawing.Point(397, 59);
            this.roundedPanel2.Name = "roundedPanel2";
            this.roundedPanel2.Size = new System.Drawing.Size(731, 319);
            this.roundedPanel2.TabIndex = 93;
            // 
            // TestResult_IO
            // 
            this.TestResult_IO.AutoCenterIcon = true;
            this.TestResult_IO.AutoCenterText = true;
            this.TestResult_IO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(245)))), ((int)(((byte)(230)))));
            this.TestResult_IO.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(226)))), ((int)(((byte)(199)))));
            this.TestResult_IO.BaseBorderThickness = 1;
            this.TestResult_IO.CornerRadius = 10;
            this.TestResult_IO.Enabled = false;
            this.TestResult_IO.FlatAppearance.BorderSize = 0;
            this.TestResult_IO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TestResult_IO.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.TestResult_IO.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(93)))), ((int)(((byte)(24)))));
            this.TestResult_IO.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.TestResult_IO.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.TestResult_IO.HoverBorderThickness = 3;
            this.TestResult_IO.IconLocation = new System.Drawing.Point(0, 0);
            this.TestResult_IO.IconScale = 0.4F;
            this.TestResult_IO.Location = new System.Drawing.Point(642, 13);
            this.TestResult_IO.Name = "TestResult_IO";
            this.TestResult_IO.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.TestResult_IO.Size = new System.Drawing.Size(67, 37);
            this.TestResult_IO.TabIndex = 94;
            this.TestResult_IO.Text = "합 격";
            this.TestResult_IO.TextBottomMargin = 9;
            this.TestResult_IO.TextLocation = new System.Drawing.Point(0, 0);
            this.TestResult_IO.UseHoverBackColor = false;
            this.TestResult_IO.UseVisualStyleBackColor = false;
            // 
            // dataGridViewDIO
            // 
            this.dataGridViewDIO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDIO.Location = new System.Drawing.Point(15, 60);
            this.dataGridViewDIO.Name = "dataGridViewDIO";
            this.dataGridViewDIO.RowTemplate.Height = 23;
            this.dataGridViewDIO.Size = new System.Drawing.Size(700, 240);
            this.dataGridViewDIO.TabIndex = 92;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(19, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 25);
            this.label1.TabIndex = 93;
            this.label1.Text = "입출력 시험 결과";
            // 
            // roundedPanel1
            // 
            this.roundedPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel1.Controls.Add(this.roundedLabel13);
            this.roundedPanel1.Controls.Add(this.Label_FinalResult);
            this.roundedPanel1.Controls.Add(this.Label_Round);
            this.roundedPanel1.Controls.Add(this.roundedLabel14);
            this.roundedPanel1.Controls.Add(this.roundedLabel8);
            this.roundedPanel1.CornerRadius = 10;
            this.roundedPanel1.Location = new System.Drawing.Point(29, 407);
            this.roundedPanel1.Name = "roundedPanel1";
            this.roundedPanel1.Size = new System.Drawing.Size(348, 171);
            this.roundedPanel1.TabIndex = 92;
            // 
            // roundedLabel13
            // 
            this.roundedLabel13.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel13.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel13.CornerRadius = 5;
            this.roundedLabel13.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel13.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel13.Location = new System.Drawing.Point(19, 105);
            this.roundedLabel13.Name = "roundedLabel13";
            this.roundedLabel13.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel13.TabIndex = 73;
            this.roundedLabel13.Text = "최종 판정";
            this.roundedLabel13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_FinalResult
            // 
            this.Label_FinalResult.BackColor = System.Drawing.Color.Transparent;
            this.Label_FinalResult.BorderColor = System.Drawing.Color.LightGray;
            this.Label_FinalResult.BorderThickness = 1;
            this.Label_FinalResult.FillColor = System.Drawing.Color.White;
            this.Label_FinalResult.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_FinalResult.ForeColor = System.Drawing.Color.Blue;
            this.Label_FinalResult.Location = new System.Drawing.Point(145, 105);
            this.Label_FinalResult.Name = "Label_FinalResult";
            this.Label_FinalResult.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_FinalResult.Size = new System.Drawing.Size(185, 40);
            this.Label_FinalResult.TabIndex = 72;
            this.Label_FinalResult.Text = "합 격";
            this.Label_FinalResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_FinalResult.VisibleBorders = CITester.BorderSides.Top;
            // 
            // Label_Round
            // 
            this.Label_Round.BackColor = System.Drawing.Color.Transparent;
            this.Label_Round.BorderColor = System.Drawing.Color.LightGray;
            this.Label_Round.BorderThickness = 1;
            this.Label_Round.FillColor = System.Drawing.Color.White;
            this.Label_Round.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_Round.Location = new System.Drawing.Point(145, 65);
            this.Label_Round.Name = "Label_Round";
            this.Label_Round.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_Round.Size = new System.Drawing.Size(185, 40);
            this.Label_Round.TabIndex = 71;
            this.Label_Round.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_Round.VisibleBorders = CITester.BorderSides.Top;
            // 
            // roundedLabel14
            // 
            this.roundedLabel14.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel14.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel14.CornerRadius = 5;
            this.roundedLabel14.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel14.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel14.Location = new System.Drawing.Point(19, 65);
            this.roundedLabel14.Name = "roundedLabel14";
            this.roundedLabel14.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel14.TabIndex = 70;
            this.roundedLabel14.Text = "시험 차수";
            this.roundedLabel14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedLabel8
            // 
            this.roundedLabel8.AutoCenterImage = false;
            this.roundedLabel8.AutoCenterText = false;
            this.roundedLabel8.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel8.CustomImage = global::TCMSTester.Properties.Resources.clipboard;
            this.roundedLabel8.FillColor = System.Drawing.Color.White;
            this.roundedLabel8.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel8.ImageLocation = new System.Drawing.Point(5, 3);
            this.roundedLabel8.Location = new System.Drawing.Point(13, 12);
            this.roundedLabel8.Name = "roundedLabel8";
            this.roundedLabel8.Size = new System.Drawing.Size(176, 40);
            this.roundedLabel8.TabIndex = 69;
            this.roundedLabel8.Text = "결과 판정";
            this.roundedLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.roundedLabel8.TextLocation = new System.Drawing.Point(43, 8);
            // 
            // roundedPanel6
            // 
            this.roundedPanel6.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel6.Controls.Add(this.roundedLabel1);
            this.roundedPanel6.Controls.Add(this.Label_Tester);
            this.roundedPanel6.Controls.Add(this.Label_Train);
            this.roundedPanel6.Controls.Add(this.Label_Fleet);
            this.roundedPanel6.Controls.Add(this.Label_Serial);
            this.roundedPanel6.Controls.Add(this.roundedLabel7);
            this.roundedPanel6.Controls.Add(this.roundedLabel6);
            this.roundedPanel6.Controls.Add(this.roundedLabel5);
            this.roundedPanel6.Controls.Add(this.roundedLabel4);
            this.roundedPanel6.Controls.Add(this.roundedLabel3);
            this.roundedPanel6.Controls.Add(this.Label_Unit);
            this.roundedPanel6.Controls.Add(this.Label_Date);
            this.roundedPanel6.Controls.Add(this.roundedLabel2);
            this.roundedPanel6.CornerRadius = 10;
            this.roundedPanel6.Location = new System.Drawing.Point(29, 59);
            this.roundedPanel6.Name = "roundedPanel6";
            this.roundedPanel6.Size = new System.Drawing.Size(348, 342);
            this.roundedPanel6.TabIndex = 91;
            // 
            // roundedLabel1
            // 
            this.roundedLabel1.AutoCenterImage = false;
            this.roundedLabel1.AutoCenterText = false;
            this.roundedLabel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel1.CustomImage = global::TCMSTester.Properties.Resources.train1;
            this.roundedLabel1.FillColor = System.Drawing.Color.White;
            this.roundedLabel1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel1.ImageLocation = new System.Drawing.Point(5, 3);
            this.roundedLabel1.Location = new System.Drawing.Point(13, 12);
            this.roundedLabel1.Name = "roundedLabel1";
            this.roundedLabel1.Size = new System.Drawing.Size(176, 40);
            this.roundedLabel1.TabIndex = 69;
            this.roundedLabel1.Text = "시험 정보";
            this.roundedLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.roundedLabel1.TextLocation = new System.Drawing.Point(43, 8);
            // 
            // Label_Tester
            // 
            this.Label_Tester.BackColor = System.Drawing.Color.Transparent;
            this.Label_Tester.BorderColor = System.Drawing.Color.LightGray;
            this.Label_Tester.BorderThickness = 1;
            this.Label_Tester.FillColor = System.Drawing.Color.White;
            this.Label_Tester.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_Tester.Location = new System.Drawing.Point(145, 270);
            this.Label_Tester.Name = "Label_Tester";
            this.Label_Tester.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_Tester.Size = new System.Drawing.Size(185, 40);
            this.Label_Tester.TabIndex = 68;
            this.Label_Tester.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_Tester.VisibleBorders = ((CITester.BorderSides)((CITester.BorderSides.Top | CITester.BorderSides.Bottom)));
            // 
            // Label_Train
            // 
            this.Label_Train.BackColor = System.Drawing.Color.Transparent;
            this.Label_Train.BorderColor = System.Drawing.Color.LightGray;
            this.Label_Train.BorderThickness = 1;
            this.Label_Train.FillColor = System.Drawing.Color.White;
            this.Label_Train.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_Train.Location = new System.Drawing.Point(145, 230);
            this.Label_Train.Name = "Label_Train";
            this.Label_Train.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_Train.Size = new System.Drawing.Size(185, 40);
            this.Label_Train.TabIndex = 67;
            this.Label_Train.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_Train.VisibleBorders = CITester.BorderSides.Top;
            // 
            // Label_Fleet
            // 
            this.Label_Fleet.BackColor = System.Drawing.Color.Transparent;
            this.Label_Fleet.BorderColor = System.Drawing.Color.LightGray;
            this.Label_Fleet.BorderThickness = 1;
            this.Label_Fleet.FillColor = System.Drawing.Color.White;
            this.Label_Fleet.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_Fleet.Location = new System.Drawing.Point(145, 190);
            this.Label_Fleet.Name = "Label_Fleet";
            this.Label_Fleet.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_Fleet.Size = new System.Drawing.Size(185, 40);
            this.Label_Fleet.TabIndex = 66;
            this.Label_Fleet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_Fleet.VisibleBorders = CITester.BorderSides.Top;
            // 
            // Label_Serial
            // 
            this.Label_Serial.BackColor = System.Drawing.Color.Transparent;
            this.Label_Serial.BorderColor = System.Drawing.Color.LightGray;
            this.Label_Serial.BorderThickness = 1;
            this.Label_Serial.FillColor = System.Drawing.Color.White;
            this.Label_Serial.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_Serial.Location = new System.Drawing.Point(145, 150);
            this.Label_Serial.Name = "Label_Serial";
            this.Label_Serial.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_Serial.Size = new System.Drawing.Size(185, 40);
            this.Label_Serial.TabIndex = 65;
            this.Label_Serial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_Serial.VisibleBorders = CITester.BorderSides.Top;
            // 
            // roundedLabel7
            // 
            this.roundedLabel7.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel7.CornerRadius = 5;
            this.roundedLabel7.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel7.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel7.Location = new System.Drawing.Point(19, 270);
            this.roundedLabel7.Name = "roundedLabel7";
            this.roundedLabel7.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel7.TabIndex = 64;
            this.roundedLabel7.Text = "시험자";
            this.roundedLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedLabel6
            // 
            this.roundedLabel6.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel6.CornerRadius = 5;
            this.roundedLabel6.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel6.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel6.Location = new System.Drawing.Point(19, 230);
            this.roundedLabel6.Name = "roundedLabel6";
            this.roundedLabel6.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel6.TabIndex = 63;
            this.roundedLabel6.Text = "차량 번호";
            this.roundedLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedLabel5
            // 
            this.roundedLabel5.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel5.CornerRadius = 5;
            this.roundedLabel5.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel5.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel5.Location = new System.Drawing.Point(19, 190);
            this.roundedLabel5.Name = "roundedLabel5";
            this.roundedLabel5.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel5.TabIndex = 62;
            this.roundedLabel5.Text = "편성 번호";
            this.roundedLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedLabel4
            // 
            this.roundedLabel4.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel4.CornerRadius = 5;
            this.roundedLabel4.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel4.Location = new System.Drawing.Point(19, 150);
            this.roundedLabel4.Name = "roundedLabel4";
            this.roundedLabel4.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel4.TabIndex = 61;
            this.roundedLabel4.Text = "일련 번호";
            this.roundedLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedLabel3
            // 
            this.roundedLabel3.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel3.CornerRadius = 5;
            this.roundedLabel3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel3.Location = new System.Drawing.Point(19, 110);
            this.roundedLabel3.Name = "roundedLabel3";
            this.roundedLabel3.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel3.TabIndex = 60;
            this.roundedLabel3.Text = "유닛 구분";
            this.roundedLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Unit
            // 
            this.Label_Unit.BackColor = System.Drawing.Color.Transparent;
            this.Label_Unit.BorderColor = System.Drawing.Color.LightGray;
            this.Label_Unit.BorderThickness = 1;
            this.Label_Unit.FillColor = System.Drawing.Color.White;
            this.Label_Unit.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_Unit.Location = new System.Drawing.Point(145, 110);
            this.Label_Unit.Name = "Label_Unit";
            this.Label_Unit.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_Unit.Size = new System.Drawing.Size(185, 40);
            this.Label_Unit.TabIndex = 59;
            this.Label_Unit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_Unit.VisibleBorders = CITester.BorderSides.Top;
            // 
            // Label_Date
            // 
            this.Label_Date.BackColor = System.Drawing.Color.Transparent;
            this.Label_Date.BorderColor = System.Drawing.Color.LightGray;
            this.Label_Date.BorderThickness = 1;
            this.Label_Date.FillColor = System.Drawing.Color.White;
            this.Label_Date.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label_Date.Location = new System.Drawing.Point(145, 70);
            this.Label_Date.Name = "Label_Date";
            this.Label_Date.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Label_Date.Size = new System.Drawing.Size(185, 40);
            this.Label_Date.TabIndex = 58;
            this.Label_Date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_Date.VisibleBorders = CITester.BorderSides.Top;
            // 
            // roundedLabel2
            // 
            this.roundedLabel2.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel2.CornerRadius = 5;
            this.roundedLabel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            this.roundedLabel2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.roundedLabel2.Location = new System.Drawing.Point(19, 70);
            this.roundedLabel2.Name = "roundedLabel2";
            this.roundedLabel2.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel2.TabIndex = 57;
            this.roundedLabel2.Text = "시험 일자";
            this.roundedLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormResultView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1156, 861);
            this.ControlBox = false;
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnConfig);
            this.Controls.Add(this.roundedPanel3);
            this.Controls.Add(this.roundedPanel4);
            this.Controls.Add(this.roundedPanel5);
            this.Controls.Add(this.roundedPanel2);
            this.Controls.Add(this.roundedPanel1);
            this.Controls.Add(this.roundedPanel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormResultView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "시험 결과";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.roundedPanel3.ResumeLayout(false);
            this.roundedPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewComm)).EndInit();
            this.roundedPanel4.ResumeLayout(false);
            this.roundedPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMemory)).EndInit();
            this.roundedPanel5.ResumeLayout(false);
            this.roundedPanel2.ResumeLayout(false);
            this.roundedPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDIO)).EndInit();
            this.roundedPanel1.ResumeLayout(false);
            this.roundedPanel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RoundedPanel roundedPanel6;
        private RoundedLabel roundedLabel1;
        private CITesterLabel Label_Tester;
        private CITesterLabel Label_Train;
        private CITesterLabel Label_Fleet;
        private CITesterLabel Label_Serial;
        private RoundedLabel roundedLabel7;
        private RoundedLabel roundedLabel6;
        private RoundedLabel roundedLabel5;
        private RoundedLabel roundedLabel4;
        private RoundedLabel roundedLabel3;
        private CITesterLabel Label_Unit;
        private CITesterLabel Label_Date;
        private RoundedLabel roundedLabel2;
        private System.Windows.Forms.DataGridView dataGridViewDIO;
        private System.Windows.Forms.Label label1;
        private RoundedPanel roundedPanel1;
        private RoundedLabel roundedLabel8;
        private RoundedPanel roundedPanel2;
        private RoundedLabel roundedLabel13;
        private CITesterLabel Label_FinalResult;
        private CITesterLabel Label_Round;
        private RoundedLabel roundedLabel14;
        private RoundedPanel roundedPanel5;
        private RoundedLabel roundedLabel11;
        private CustomIconButton TestResult_IO;
        private RoundedPanel roundedPanel4;
        private CustomIconButton TestResult_Memory;
        private System.Windows.Forms.DataGridView dataGridViewMemory;
        private System.Windows.Forms.Label label3;
        private RoundedPanel roundedPanel3;
        private CustomIconButton TestResult_Comn;
        private System.Windows.Forms.DataGridView dataGridViewComm;
        private System.Windows.Forms.Label label2;
        private CustomIconButton BtnConfig;
        private CustomIconButton BtnClose;
        private CustomIconButton imagebtn2;
        private System.Windows.Forms.RichTextBox richTextBox_Err;
    }
}

