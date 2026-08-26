namespace CITester
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("디지털 입출력 시험");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("아날로그 입출력 시험");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("입·출력 시험", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("통신 시험");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("메모리 시험");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("ER 속도센서 시험");
            this.ProgressBar_Run = new System.Windows.Forms.ProgressBar();
            this.Btn_EmergencyStop = new System.Windows.Forms.Button();
            this.Timer_Measure = new System.Windows.Forms.Timer(this.components);
            this.Serial_PWM = new System.IO.Ports.SerialPort(this.components);
            this.Serial_DCPower1 = new System.IO.Ports.SerialPort(this.components);
            this.Timer_Start = new System.Windows.Forms.Timer(this.components);
            this.Timer_Clear = new System.Windows.Forms.Timer(this.components);
            this.timerRunPowRun = new System.Windows.Forms.Timer(this.components);
            this.timerBasicRun = new System.Windows.Forms.Timer(this.components);
            this.timerRunStop = new System.Windows.Forms.Timer(this.components);
            this.timerRunForRun = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnExit = new CustomIconButton();
            this.BtnConfig = new CustomIconButton();
            this.BtnNew = new CustomIconButton();
            this.BtnDB = new CustomIconButton();
            this.BtnDiagnostic = new CustomIconButton();
            this.BtnResult = new CustomIconButton();
            this.button2 = new System.Windows.Forms.Button();
            this.roundedLabel15 = new CITester.RoundedLabel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.mainTabControl1 = new CITester.MainTabControl();
            this.tabPage17 = new System.Windows.Forms.TabPage();
            this.flatTabControl1 = new CITester.FlatTabControl();
            this.tabPageDI1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewDI1 = new System.Windows.Forms.DataGridView();
            this.tabPageDI2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewDI2 = new System.Windows.Forms.DataGridView();
            this.tabPageDI3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewDI3 = new System.Windows.Forms.DataGridView();
            this.tabPageDO = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewDO = new System.Windows.Forms.DataGridView();
            this.tabPageAnalog = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewAnalog = new System.Windows.Forms.DataGridView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButtonRS3 = new System.Windows.Forms.RadioButton();
            this.radioButtonRS2 = new System.Windows.Forms.RadioButton();
            this.radioButtonRS1 = new System.Windows.Forms.RadioButton();
            this.radioButtonMVB3 = new System.Windows.Forms.RadioButton();
            this.radioButtonWTB3 = new System.Windows.Forms.RadioButton();
            this.radioButtonMVB2 = new System.Windows.Forms.RadioButton();
            this.radioButtonMVB1 = new System.Windows.Forms.RadioButton();
            this.radioButtonWTB2 = new System.Windows.Forms.RadioButton();
            this.radioButtonWTB1 = new System.Windows.Forms.RadioButton();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.flatTabControl4 = new CITester.FlatTabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel20 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewMemory = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.roundedPanel5 = new CITester.RoundedPanel();
            this.customNumeric1 = new CustomNumeric();
            this.roundedLabel9 = new CITester.RoundedLabel();
            this.BtnStart = new CustomIconButton();
            this.roundedLabel10 = new CITester.RoundedLabel();
            this.customProgressBar2 = new YourNamespace.CustomProgressBar();
            this.roundedLabel11 = new CITester.RoundedLabel();
            this.Label_Desc = new System.Windows.Forms.Label();
            this.roundedLabel12 = new CITester.RoundedLabel();
            this.roundedLabel13 = new CITester.RoundedLabel();
            this.roundedLabel14 = new CITester.RoundedLabel();
            this.roundedPanel4 = new CITester.RoundedPanel();
            this.imagebtn2 = new CustomIconButton();
            this.imagebtn1 = new CustomIconButton();
            this.richTextBox_FailLog = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
            this.BtnPLC = new System.Windows.Forms.PictureBox();
            this.BtnPrint = new CustomIconButton();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.roundedPanel1 = new CITester.RoundedPanel();
            this.Button_DeSelect_All = new CustomIconButton();
            this.Button_Select_All = new CustomIconButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TestCount = new CustomControl.StepCountControl();
            this.roundedLabel8 = new CITester.RoundedLabel();
            this.modernTreeView1 = new ModernTreeView();
            this.roundedPanel6 = new CITester.RoundedPanel();
            this.BtnChange = new CustomIconButton();
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
            this.miniToolStrip = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel6.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.mainTabControl1.SuspendLayout();
            this.tabPage17.SuspendLayout();
            this.flatTabControl1.SuspendLayout();
            this.tabPageDI1.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDI1)).BeginInit();
            this.tabPageDI2.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDI2)).BeginInit();
            this.tabPageDI3.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDI3)).BeginInit();
            this.tabPageDO.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDO)).BeginInit();
            this.tabPageAnalog.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAnalog)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.flatTabControl4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tableLayoutPanel20.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMemory)).BeginInit();
            this.tabPage6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.roundedPanel5.SuspendLayout();
            this.roundedPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnPLC)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.roundedPanel1.SuspendLayout();
            this.roundedPanel6.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProgressBar_Run
            // 
            this.ProgressBar_Run.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ProgressBar_Run.Location = new System.Drawing.Point(418, 980);
            this.ProgressBar_Run.Maximum = 500;
            this.ProgressBar_Run.Name = "ProgressBar_Run";
            this.ProgressBar_Run.Size = new System.Drawing.Size(649, 30);
            this.ProgressBar_Run.TabIndex = 37;
            this.ProgressBar_Run.Visible = false;
            this.ProgressBar_Run.Click += new System.EventHandler(this.ProgressBar_Run_Click);
            // 
            // Btn_EmergencyStop
            // 
            this.Btn_EmergencyStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(187)))), ((int)(((byte)(214)))));
            this.Btn_EmergencyStop.Enabled = false;
            this.Btn_EmergencyStop.Font = new System.Drawing.Font("맑은 고딕", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_EmergencyStop.ForeColor = System.Drawing.Color.Black;
            this.Btn_EmergencyStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Btn_EmergencyStop.Location = new System.Drawing.Point(1302, 977);
            this.Btn_EmergencyStop.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.Btn_EmergencyStop.Name = "Btn_EmergencyStop";
            this.Btn_EmergencyStop.Size = new System.Drawing.Size(89, 72);
            this.Btn_EmergencyStop.TabIndex = 81;
            this.Btn_EmergencyStop.Text = "시험 정지";
            this.Btn_EmergencyStop.UseVisualStyleBackColor = false;
            this.Btn_EmergencyStop.Visible = false;
            this.Btn_EmergencyStop.Click += new System.EventHandler(this.Btn_EmergencyStop_Click);
            // 
            // Serial_PWM
            // 
            this.Serial_PWM.BaudRate = 115200;
            this.Serial_PWM.PortName = "COM3";
            // 
            // Serial_DCPower1
            // 
            this.Serial_DCPower1.BaudRate = 2400;
            this.Serial_DCPower1.PortName = "COM25";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 7;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 354F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel6.Controls.Add(this.Btn_EmergencyStop, 4, 6);
            this.tableLayoutPanel6.Controls.Add(this.ProgressBar_Run, 3, 6);
            this.tableLayoutPanel6.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 3, 2);
            this.tableLayoutPanel6.Controls.Add(this.BtnPLC, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.BtnPrint, 6, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel5, 1, 4);
            this.tableLayoutPanel6.Controls.Add(this.roundedPanel6, 1, 2);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 7;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 351F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 489F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1528, 1057);
            this.tableLayoutPanel6.TabIndex = 84;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.LightSlateGray;
            this.tableLayoutPanel6.SetColumnSpan(this.panel3, 7);
            this.panel3.Controls.Add(this.tableLayoutPanel11);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1528, 125);
            this.panel3.TabIndex = 77;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(26)))), ((int)(((byte)(60)))));
            this.tableLayoutPanel11.ColumnCount = 14;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel11.Controls.Add(this.BtnExit, 12, 0);
            this.tableLayoutPanel11.Controls.Add(this.BtnConfig, 11, 0);
            this.tableLayoutPanel11.Controls.Add(this.BtnNew, 5, 0);
            this.tableLayoutPanel11.Controls.Add(this.BtnDB, 4, 0);
            this.tableLayoutPanel11.Controls.Add(this.BtnDiagnostic, 6, 0);
            this.tableLayoutPanel11.Controls.Add(this.BtnResult, 8, 0);
            this.tableLayoutPanel11.Controls.Add(this.button2, 9, 0);
            this.tableLayoutPanel11.Controls.Add(this.roundedLabel15, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.button1, 10, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(1528, 125);
            this.tableLayoutPanel11.TabIndex = 36;
            // 
            // BtnExit
            // 
            this.BtnExit.AutoCenterIcon = true;
            this.BtnExit.AutoCenterText = true;
            this.BtnExit.BackColor = System.Drawing.Color.Transparent;
            this.BtnExit.BaseBorderColor = System.Drawing.Color.Gray;
            this.BtnExit.BaseBorderThickness = 0;
            this.BtnExit.CornerRadius = 10;
            this.BtnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnExit.FlatAppearance.BorderSize = 0;
            this.BtnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnExit.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnExit.ForeColor = System.Drawing.Color.White;
            this.BtnExit.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnExit.HoverBorderColor = System.Drawing.Color.Red;
            this.BtnExit.HoverBorderThickness = 3;
            this.BtnExit.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnExit.IconScale = 0.5F;
            this.BtnExit.Image = global::TCMSTester.Properties.Resources.off;
            this.BtnExit.Location = new System.Drawing.Point(1361, 3);
            this.BtnExit.Name = "BtnExit";
            this.BtnExit.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnExit.Size = new System.Drawing.Size(141, 119);
            this.BtnExit.TabIndex = 32;
            this.BtnExit.Text = "종료";
            this.BtnExit.TextBottomMargin = 7;
            this.BtnExit.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnExit.UseHoverBackColor = false;
            this.BtnExit.UseVisualStyleBackColor = false;
            this.BtnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // BtnConfig
            // 
            this.BtnConfig.AutoCenterIcon = true;
            this.BtnConfig.AutoCenterText = true;
            this.BtnConfig.BackColor = System.Drawing.Color.Transparent;
            this.BtnConfig.BaseBorderColor = System.Drawing.Color.Black;
            this.BtnConfig.BaseBorderThickness = 0;
            this.BtnConfig.CornerRadius = 10;
            this.BtnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnConfig.FlatAppearance.BorderSize = 0;
            this.BtnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnConfig.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnConfig.ForeColor = System.Drawing.Color.White;
            this.BtnConfig.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnConfig.HoverBorderColor = System.Drawing.Color.White;
            this.BtnConfig.HoverBorderThickness = 3;
            this.BtnConfig.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnConfig.IconScale = 0.5F;
            this.BtnConfig.Image = global::TCMSTester.Properties.Resources.Artboard_11_3x;
            this.BtnConfig.Location = new System.Drawing.Point(1214, 3);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnConfig.Size = new System.Drawing.Size(141, 119);
            this.BtnConfig.TabIndex = 5;
            this.BtnConfig.Text = "설정";
            this.BtnConfig.TextBottomMargin = 7;
            this.BtnConfig.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnConfig.UseHoverBackColor = false;
            this.BtnConfig.UseVisualStyleBackColor = false;
            this.BtnConfig.Visible = false;
            this.BtnConfig.Click += new System.EventHandler(this.BtnSetting_Click);
            // 
            // BtnNew
            // 
            this.BtnNew.AutoCenterIcon = true;
            this.BtnNew.AutoCenterText = true;
            this.BtnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(26)))), ((int)(((byte)(60)))));
            this.BtnNew.BaseBorderColor = System.Drawing.Color.Black;
            this.BtnNew.BaseBorderThickness = 0;
            this.BtnNew.CornerRadius = 15;
            this.BtnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnNew.FlatAppearance.BorderSize = 0;
            this.BtnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnNew.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnNew.ForeColor = System.Drawing.Color.White;
            this.BtnNew.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnNew.HoverBorderColor = System.Drawing.Color.White;
            this.BtnNew.HoverBorderThickness = 3;
            this.BtnNew.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnNew.IconScale = 0.5F;
            this.BtnNew.Image = global::TCMSTester.Properties.Resources.new_project;
            this.BtnNew.Location = new System.Drawing.Point(606, 3);
            this.BtnNew.Name = "BtnNew";
            this.BtnNew.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnNew.Size = new System.Drawing.Size(141, 119);
            this.BtnNew.TabIndex = 0;
            this.BtnNew.Text = "새 시험";
            this.BtnNew.TextBottomMargin = 7;
            this.BtnNew.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnNew.UseHoverBackColor = false;
            this.BtnNew.UseVisualStyleBackColor = false;
            this.BtnNew.Click += new System.EventHandler(this.BtnNew_Click);
            // 
            // BtnDB
            // 
            this.BtnDB.AutoCenterIcon = true;
            this.BtnDB.AutoCenterText = true;
            this.BtnDB.BackColor = System.Drawing.Color.Transparent;
            this.BtnDB.BaseBorderColor = System.Drawing.Color.Black;
            this.BtnDB.BaseBorderThickness = 0;
            this.BtnDB.CornerRadius = 10;
            this.BtnDB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnDB.FlatAppearance.BorderSize = 0;
            this.BtnDB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnDB.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnDB.ForeColor = System.Drawing.Color.White;
            this.BtnDB.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnDB.HoverBorderColor = System.Drawing.Color.White;
            this.BtnDB.HoverBorderThickness = 3;
            this.BtnDB.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnDB.IconScale = 0.5F;
            this.BtnDB.Image = global::TCMSTester.Properties.Resources.Artboard_8_3x;
            this.BtnDB.Location = new System.Drawing.Point(459, 3);
            this.BtnDB.Name = "BtnDB";
            this.BtnDB.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnDB.Size = new System.Drawing.Size(141, 119);
            this.BtnDB.TabIndex = 4;
            this.BtnDB.Text = "DB 관리";
            this.BtnDB.TextBottomMargin = 7;
            this.BtnDB.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnDB.UseHoverBackColor = false;
            this.BtnDB.UseVisualStyleBackColor = false;
            this.BtnDB.Visible = false;
            this.BtnDB.Click += new System.EventHandler(this.BtnDB_Click);
            // 
            // BtnDiagnostic
            // 
            this.BtnDiagnostic.AutoCenterIcon = true;
            this.BtnDiagnostic.AutoCenterText = true;
            this.BtnDiagnostic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(26)))), ((int)(((byte)(60)))));
            this.BtnDiagnostic.BaseBorderColor = System.Drawing.Color.Black;
            this.BtnDiagnostic.BaseBorderThickness = 0;
            this.BtnDiagnostic.CornerRadius = 10;
            this.BtnDiagnostic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnDiagnostic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnDiagnostic.FlatAppearance.BorderSize = 0;
            this.BtnDiagnostic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnDiagnostic.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnDiagnostic.ForeColor = System.Drawing.Color.White;
            this.BtnDiagnostic.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnDiagnostic.HoverBorderColor = System.Drawing.Color.White;
            this.BtnDiagnostic.HoverBorderThickness = 3;
            this.BtnDiagnostic.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnDiagnostic.IconScale = 0.5F;
            this.BtnDiagnostic.Image = global::TCMSTester.Properties.Resources.Artboard_9_3x;
            this.BtnDiagnostic.Location = new System.Drawing.Point(775, 8);
            this.BtnDiagnostic.Margin = new System.Windows.Forms.Padding(25, 8, 25, 8);
            this.BtnDiagnostic.Name = "BtnDiagnostic";
            this.BtnDiagnostic.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnDiagnostic.Size = new System.Drawing.Size(97, 109);
            this.BtnDiagnostic.TabIndex = 2;
            this.BtnDiagnostic.Text = "자가 진단";
            this.BtnDiagnostic.TextBottomMargin = 7;
            this.BtnDiagnostic.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnDiagnostic.UseHoverBackColor = false;
            this.BtnDiagnostic.UseVisualStyleBackColor = false;
            this.BtnDiagnostic.Click += new System.EventHandler(this.BtnDiagnostic_Click);
            // 
            // BtnResult
            // 
            this.BtnResult.AutoCenterIcon = true;
            this.BtnResult.AutoCenterText = true;
            this.BtnResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(26)))), ((int)(((byte)(60)))));
            this.BtnResult.BaseBorderColor = System.Drawing.Color.Black;
            this.BtnResult.BaseBorderThickness = 0;
            this.BtnResult.CornerRadius = 10;
            this.BtnResult.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnResult.FlatAppearance.BorderSize = 0;
            this.BtnResult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnResult.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnResult.ForeColor = System.Drawing.Color.White;
            this.BtnResult.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnResult.HoverBorderColor = System.Drawing.Color.White;
            this.BtnResult.HoverBorderThickness = 3;
            this.BtnResult.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnResult.IconScale = 0.5F;
            this.BtnResult.Image = global::TCMSTester.Properties.Resources.Artboard_6_3x;
            this.BtnResult.Location = new System.Drawing.Point(900, 3);
            this.BtnResult.Name = "BtnResult";
            this.BtnResult.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnResult.Size = new System.Drawing.Size(141, 119);
            this.BtnResult.TabIndex = 1;
            this.BtnResult.Text = "결과 검색";
            this.BtnResult.TextBottomMargin = 7;
            this.BtnResult.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnResult.UseHoverBackColor = false;
            this.BtnResult.UseVisualStyleBackColor = false;
            this.BtnResult.Click += new System.EventHandler(this.BtnResult_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1047, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 119);
            this.button2.TabIndex = 84;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_2);
            // 
            // roundedLabel15
            // 
            this.roundedLabel15.AutoCenterImage = false;
            this.roundedLabel15.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel15.BorderThickness = 0;
            this.tableLayoutPanel11.SetColumnSpan(this.roundedLabel15, 3);
            this.roundedLabel15.CustomImage = global::TCMSTester.Properties.Resources._9d9c9c09_7200_4b05_83e9_d5d974e905351;
            this.roundedLabel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundedLabel15.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(26)))), ((int)(((byte)(60)))));
            this.roundedLabel15.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel15.ForeColor = System.Drawing.Color.White;
            this.roundedLabel15.ImageLocation = new System.Drawing.Point(-30, -200);
            this.roundedLabel15.ImageSize = new System.Drawing.Size(600, 550);
            this.roundedLabel15.Location = new System.Drawing.Point(18, 0);
            this.roundedLabel15.Name = "roundedLabel15";
            this.roundedLabel15.Size = new System.Drawing.Size(435, 125);
            this.roundedLabel15.TabIndex = 85;
            this.roundedLabel15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel6.SetColumnSpan(this.tableLayoutPanel7, 3);
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.7558F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.2442F));
            this.tableLayoutPanel7.Controls.Add(this.mainTabControl1, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.roundedPanel5, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.roundedPanel4, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(418, 136);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 4;
            this.tableLayoutPanel6.SetRowSpan(this.tableLayoutPanel7, 3);
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.76133F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.23867F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 239F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(1057, 836);
            this.tableLayoutPanel7.TabIndex = 86;
            // 
            // mainTabControl1
            // 
            this.mainTabControl1.ContentAreaColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.mainTabControl1.Controls.Add(this.tabPage17);
            this.mainTabControl1.Controls.Add(this.tabPage1);
            this.mainTabControl1.Controls.Add(this.tabPage4);
            this.mainTabControl1.Controls.Add(this.tabPage6);
            this.mainTabControl1.CornerRadius = 15;
            this.mainTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mainTabControl1.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            this.mainTabControl1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.mainTabControl1.ItemSize = new System.Drawing.Size(160, 38);
            this.mainTabControl1.Location = new System.Drawing.Point(3, 3);
            this.mainTabControl1.Name = "mainTabControl1";
            this.mainTabControl1.Padding = new System.Drawing.Point(10, 3);
            this.tableLayoutPanel7.SetRowSpan(this.mainTabControl1, 2);
            this.mainTabControl1.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(60)))), ((int)(((byte)(105)))));
            this.mainTabControl1.SelectedIndex = 0;
            this.mainTabControl1.SelectedTextColor = System.Drawing.Color.White;
            this.mainTabControl1.Size = new System.Drawing.Size(646, 572);
            this.mainTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mainTabControl1.TabIndex = 0;
            this.mainTabControl1.UnselectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.mainTabControl1.UnselectedTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            // 
            // tabPage17
            // 
            this.tabPage17.Controls.Add(this.flatTabControl1);
            this.tabPage17.Location = new System.Drawing.Point(4, 42);
            this.tabPage17.Name = "tabPage17";
            this.tabPage17.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage17.Size = new System.Drawing.Size(638, 526);
            this.tabPage17.TabIndex = 0;
            this.tabPage17.Text = "입·출력 시험";
            this.tabPage17.UseVisualStyleBackColor = true;
            // 
            // flatTabControl1
            // 
            this.flatTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flatTabControl1.ContentBackColor = System.Drawing.Color.White;
            this.flatTabControl1.ContentBorderColor = System.Drawing.Color.LightGray;
            this.flatTabControl1.Controls.Add(this.tabPageDI1);
            this.flatTabControl1.Controls.Add(this.tabPageDI2);
            this.flatTabControl1.Controls.Add(this.tabPageDI3);
            this.flatTabControl1.Controls.Add(this.tabPageDO);
            this.flatTabControl1.Controls.Add(this.tabPageAnalog);
            this.flatTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.flatTabControl1.HeaderBackColor = System.Drawing.Color.White;
            this.flatTabControl1.HideTabHeader = false;
            this.flatTabControl1.HoverTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.flatTabControl1.ItemSize = new System.Drawing.Size(140, 40);
            this.flatTabControl1.LineColor = System.Drawing.Color.LightGray;
            this.flatTabControl1.LinePadding = 20;
            this.flatTabControl1.Location = new System.Drawing.Point(3, 3);
            this.flatTabControl1.Name = "flatTabControl1";
            this.flatTabControl1.Padding = new System.Drawing.Point(0, 0);
            this.flatTabControl1.SelectedColor = System.Drawing.Color.White;
            this.flatTabControl1.SelectedIndex = 0;
            this.flatTabControl1.SelectedLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.flatTabControl1.SelectedTextColor = System.Drawing.Color.Black;
            this.flatTabControl1.ShowContentBorder = false;
            this.flatTabControl1.ShowTabBorders = false;
            this.flatTabControl1.Size = new System.Drawing.Size(632, 520);
            this.flatTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.flatTabControl1.TabBorderColor = System.Drawing.Color.LightGray;
            this.flatTabControl1.TabColor = System.Drawing.Color.White;
            this.flatTabControl1.TabIndex = 0;
            this.flatTabControl1.TabRadius = 6;
            this.flatTabControl1.TextColor = System.Drawing.Color.DimGray;
            this.flatTabControl1.UseSingleLine = true;
            // 
            // tabPageDI1
            // 
            this.tabPageDI1.Controls.Add(this.tableLayoutPanel12);
            this.tabPageDI1.Location = new System.Drawing.Point(4, 44);
            this.tabPageDI1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageDI1.Name = "tabPageDI1";
            this.tabPageDI1.Size = new System.Drawing.Size(624, 472);
            this.tabPageDI1.TabIndex = 0;
            this.tabPageDI1.Text = "디지털 입력 1/3";
            this.tabPageDI1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel12.ColumnCount = 3;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel12.Controls.Add(this.dataGridViewDI1, 1, 1);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 3;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(624, 472);
            this.tableLayoutPanel12.TabIndex = 5;
            // 
            // dataGridViewDI1
            // 
            this.dataGridViewDI1.AllowUserToAddRows = false;
            this.dataGridViewDI1.AllowUserToDeleteRows = false;
            this.dataGridViewDI1.AllowUserToResizeColumns = false;
            this.dataGridViewDI1.AllowUserToResizeRows = false;
            this.dataGridViewDI1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridViewDI1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewDI1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewDI1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDI1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewDI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDI1.Location = new System.Drawing.Point(8, 8);
            this.dataGridViewDI1.Name = "dataGridViewDI1";
            this.dataGridViewDI1.ReadOnly = true;
            this.dataGridViewDI1.RowHeadersVisible = false;
            this.dataGridViewDI1.RowTemplate.Height = 23;
            this.dataGridViewDI1.Size = new System.Drawing.Size(608, 456);
            this.dataGridViewDI1.TabIndex = 0;
            this.dataGridViewDI1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDI1_CellContentClick);
            // 
            // tabPageDI2
            // 
            this.tabPageDI2.Controls.Add(this.tableLayoutPanel9);
            this.tabPageDI2.Location = new System.Drawing.Point(4, 44);
            this.tabPageDI2.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageDI2.Name = "tabPageDI2";
            this.tabPageDI2.Size = new System.Drawing.Size(624, 472);
            this.tabPageDI2.TabIndex = 4;
            this.tabPageDI2.Text = "디지털 입력 2/3";
            this.tabPageDI2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel9.ColumnCount = 3;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel9.Controls.Add(this.dataGridViewDI2, 1, 1);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 3;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(624, 472);
            this.tableLayoutPanel9.TabIndex = 4;
            // 
            // dataGridViewDI2
            // 
            this.dataGridViewDI2.AllowUserToAddRows = false;
            this.dataGridViewDI2.AllowUserToDeleteRows = false;
            this.dataGridViewDI2.AllowUserToResizeColumns = false;
            this.dataGridViewDI2.AllowUserToResizeRows = false;
            this.dataGridViewDI2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridViewDI2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewDI2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewDI2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDI2.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewDI2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDI2.Location = new System.Drawing.Point(8, 8);
            this.dataGridViewDI2.Name = "dataGridViewDI2";
            this.dataGridViewDI2.ReadOnly = true;
            this.dataGridViewDI2.RowHeadersVisible = false;
            this.dataGridViewDI2.RowTemplate.Height = 23;
            this.dataGridViewDI2.Size = new System.Drawing.Size(608, 456);
            this.dataGridViewDI2.TabIndex = 0;
            // 
            // tabPageDI3
            // 
            this.tabPageDI3.Controls.Add(this.tableLayoutPanel10);
            this.tabPageDI3.Location = new System.Drawing.Point(4, 44);
            this.tabPageDI3.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageDI3.Name = "tabPageDI3";
            this.tabPageDI3.Size = new System.Drawing.Size(624, 472);
            this.tabPageDI3.TabIndex = 5;
            this.tabPageDI3.Text = "디지털 입력 3/3";
            this.tabPageDI3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel10.ColumnCount = 3;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel10.Controls.Add(this.dataGridViewDI3, 1, 1);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 3;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(624, 472);
            this.tableLayoutPanel10.TabIndex = 4;
            // 
            // dataGridViewDI3
            // 
            this.dataGridViewDI3.AllowUserToAddRows = false;
            this.dataGridViewDI3.AllowUserToDeleteRows = false;
            this.dataGridViewDI3.AllowUserToResizeColumns = false;
            this.dataGridViewDI3.AllowUserToResizeRows = false;
            this.dataGridViewDI3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridViewDI3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewDI3.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewDI3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDI3.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewDI3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDI3.Location = new System.Drawing.Point(8, 8);
            this.dataGridViewDI3.Name = "dataGridViewDI3";
            this.dataGridViewDI3.ReadOnly = true;
            this.dataGridViewDI3.RowHeadersVisible = false;
            this.dataGridViewDI3.RowTemplate.Height = 23;
            this.dataGridViewDI3.Size = new System.Drawing.Size(608, 456);
            this.dataGridViewDI3.TabIndex = 0;
            // 
            // tabPageDO
            // 
            this.tabPageDO.Controls.Add(this.tableLayoutPanel3);
            this.tabPageDO.Location = new System.Drawing.Point(4, 44);
            this.tabPageDO.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageDO.Name = "tabPageDO";
            this.tabPageDO.Size = new System.Drawing.Size(624, 472);
            this.tabPageDO.TabIndex = 2;
            this.tabPageDO.Text = "디지털 출력";
            this.tabPageDO.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel3.Controls.Add(this.dataGridViewDO, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(624, 472);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // dataGridViewDO
            // 
            this.dataGridViewDO.AllowUserToAddRows = false;
            this.dataGridViewDO.AllowUserToDeleteRows = false;
            this.dataGridViewDO.AllowUserToResizeColumns = false;
            this.dataGridViewDO.AllowUserToResizeRows = false;
            this.dataGridViewDO.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridViewDO.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewDO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDO.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewDO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDO.Location = new System.Drawing.Point(8, 8);
            this.dataGridViewDO.Name = "dataGridViewDO";
            this.dataGridViewDO.ReadOnly = true;
            this.dataGridViewDO.RowHeadersVisible = false;
            this.dataGridViewDO.RowTemplate.Height = 23;
            this.dataGridViewDO.Size = new System.Drawing.Size(608, 456);
            this.dataGridViewDO.TabIndex = 0;
            // 
            // tabPageAnalog
            // 
            this.tabPageAnalog.Controls.Add(this.tableLayoutPanel4);
            this.tabPageAnalog.Location = new System.Drawing.Point(4, 44);
            this.tabPageAnalog.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageAnalog.Name = "tabPageAnalog";
            this.tabPageAnalog.Size = new System.Drawing.Size(624, 472);
            this.tabPageAnalog.TabIndex = 1;
            this.tabPageAnalog.Text = "아날로그 입출력";
            this.tabPageAnalog.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel4.Controls.Add(this.dataGridViewAnalog, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(624, 472);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // dataGridViewAnalog
            // 
            this.dataGridViewAnalog.AllowUserToAddRows = false;
            this.dataGridViewAnalog.AllowUserToDeleteRows = false;
            this.dataGridViewAnalog.AllowUserToResizeColumns = false;
            this.dataGridViewAnalog.AllowUserToResizeRows = false;
            this.dataGridViewAnalog.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridViewAnalog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewAnalog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewAnalog.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewAnalog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAnalog.Location = new System.Drawing.Point(8, 8);
            this.dataGridViewAnalog.Name = "dataGridViewAnalog";
            this.dataGridViewAnalog.ReadOnly = true;
            this.dataGridViewAnalog.RowHeadersVisible = false;
            this.dataGridViewAnalog.RowTemplate.Height = 23;
            this.dataGridViewAnalog.Size = new System.Drawing.Size(608, 456);
            this.dataGridViewAnalog.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 42);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(638, 526);
            this.tabPage1.TabIndex = 7;
            this.tabPage1.Text = "통신 시험";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(638, 526);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButtonRS3);
            this.panel1.Controls.Add(this.radioButtonRS2);
            this.panel1.Controls.Add(this.radioButtonRS1);
            this.panel1.Controls.Add(this.radioButtonMVB3);
            this.panel1.Controls.Add(this.radioButtonWTB3);
            this.panel1.Controls.Add(this.radioButtonMVB2);
            this.panel1.Controls.Add(this.radioButtonMVB1);
            this.panel1.Controls.Add(this.radioButtonWTB2);
            this.panel1.Controls.Add(this.radioButtonWTB1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(8, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(622, 480);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // radioButtonRS3
            // 
            this.radioButtonRS3.AutoSize = true;
            this.radioButtonRS3.Location = new System.Drawing.Point(412, 154);
            this.radioButtonRS3.Name = "radioButtonRS3";
            this.radioButtonRS3.Size = new System.Drawing.Size(123, 29);
            this.radioButtonRS3.TabIndex = 8;
            this.radioButtonRS3.TabStop = true;
            this.radioButtonRS3.Text = "RS485 CH3";
            this.radioButtonRS3.UseVisualStyleBackColor = true;
            // 
            // radioButtonRS2
            // 
            this.radioButtonRS2.AutoSize = true;
            this.radioButtonRS2.Location = new System.Drawing.Point(412, 110);
            this.radioButtonRS2.Name = "radioButtonRS2";
            this.radioButtonRS2.Size = new System.Drawing.Size(123, 29);
            this.radioButtonRS2.TabIndex = 7;
            this.radioButtonRS2.TabStop = true;
            this.radioButtonRS2.Text = "RS485 CH2";
            this.radioButtonRS2.UseVisualStyleBackColor = true;
            // 
            // radioButtonRS1
            // 
            this.radioButtonRS1.AutoSize = true;
            this.radioButtonRS1.Location = new System.Drawing.Point(412, 65);
            this.radioButtonRS1.Name = "radioButtonRS1";
            this.radioButtonRS1.Size = new System.Drawing.Size(123, 29);
            this.radioButtonRS1.TabIndex = 6;
            this.radioButtonRS1.TabStop = true;
            this.radioButtonRS1.Text = "RS485 CH1";
            this.radioButtonRS1.UseVisualStyleBackColor = true;
            // 
            // radioButtonMVB3
            // 
            this.radioButtonMVB3.AutoSize = true;
            this.radioButtonMVB3.Location = new System.Drawing.Point(255, 154);
            this.radioButtonMVB3.Name = "radioButtonMVB3";
            this.radioButtonMVB3.Size = new System.Drawing.Size(111, 29);
            this.radioButtonMVB3.TabIndex = 5;
            this.radioButtonMVB3.TabStop = true;
            this.radioButtonMVB3.Text = "MVB CH3";
            this.radioButtonMVB3.UseVisualStyleBackColor = true;
            // 
            // radioButtonWTB3
            // 
            this.radioButtonWTB3.AutoSize = true;
            this.radioButtonWTB3.Location = new System.Drawing.Point(38, 154);
            this.radioButtonWTB3.Name = "radioButtonWTB3";
            this.radioButtonWTB3.Size = new System.Drawing.Size(110, 29);
            this.radioButtonWTB3.TabIndex = 4;
            this.radioButtonWTB3.TabStop = true;
            this.radioButtonWTB3.Text = "WTB CH3";
            this.radioButtonWTB3.UseVisualStyleBackColor = true;
            // 
            // radioButtonMVB2
            // 
            this.radioButtonMVB2.AutoSize = true;
            this.radioButtonMVB2.Location = new System.Drawing.Point(255, 110);
            this.radioButtonMVB2.Name = "radioButtonMVB2";
            this.radioButtonMVB2.Size = new System.Drawing.Size(111, 29);
            this.radioButtonMVB2.TabIndex = 3;
            this.radioButtonMVB2.TabStop = true;
            this.radioButtonMVB2.Text = "MVB CH2";
            this.radioButtonMVB2.UseVisualStyleBackColor = true;
            // 
            // radioButtonMVB1
            // 
            this.radioButtonMVB1.AutoSize = true;
            this.radioButtonMVB1.Location = new System.Drawing.Point(255, 65);
            this.radioButtonMVB1.Name = "radioButtonMVB1";
            this.radioButtonMVB1.Size = new System.Drawing.Size(111, 29);
            this.radioButtonMVB1.TabIndex = 2;
            this.radioButtonMVB1.TabStop = true;
            this.radioButtonMVB1.Text = "MVB CH1";
            this.radioButtonMVB1.UseVisualStyleBackColor = true;
            // 
            // radioButtonWTB2
            // 
            this.radioButtonWTB2.AutoSize = true;
            this.radioButtonWTB2.Location = new System.Drawing.Point(38, 110);
            this.radioButtonWTB2.Name = "radioButtonWTB2";
            this.radioButtonWTB2.Size = new System.Drawing.Size(110, 29);
            this.radioButtonWTB2.TabIndex = 1;
            this.radioButtonWTB2.TabStop = true;
            this.radioButtonWTB2.Text = "WTB CH2";
            this.radioButtonWTB2.UseVisualStyleBackColor = true;
            // 
            // radioButtonWTB1
            // 
            this.radioButtonWTB1.AutoSize = true;
            this.radioButtonWTB1.Location = new System.Drawing.Point(38, 65);
            this.radioButtonWTB1.Name = "radioButtonWTB1";
            this.radioButtonWTB1.Size = new System.Drawing.Size(110, 29);
            this.radioButtonWTB1.TabIndex = 0;
            this.radioButtonWTB1.TabStop = true;
            this.radioButtonWTB1.Text = "WTB CH1";
            this.radioButtonWTB1.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.flatTabControl4);
            this.tabPage4.Location = new System.Drawing.Point(4, 42);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(638, 526);
            this.tabPage4.TabIndex = 5;
            this.tabPage4.Text = "메모리 시험";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // flatTabControl4
            // 
            this.flatTabControl4.ContentBackColor = System.Drawing.Color.White;
            this.flatTabControl4.ContentBorderColor = System.Drawing.Color.LightGray;
            this.flatTabControl4.Controls.Add(this.tabPage5);
            this.flatTabControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flatTabControl4.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.flatTabControl4.HeaderBackColor = System.Drawing.Color.White;
            this.flatTabControl4.HideTabHeader = false;
            this.flatTabControl4.HoverTextColor = System.Drawing.Color.White;
            this.flatTabControl4.ItemSize = new System.Drawing.Size(120, 40);
            this.flatTabControl4.LineColor = System.Drawing.Color.White;
            this.flatTabControl4.LinePadding = 20;
            this.flatTabControl4.Location = new System.Drawing.Point(0, 0);
            this.flatTabControl4.Name = "flatTabControl4";
            this.flatTabControl4.Padding = new System.Drawing.Point(0, 0);
            this.flatTabControl4.SelectedColor = System.Drawing.Color.White;
            this.flatTabControl4.SelectedIndex = 0;
            this.flatTabControl4.SelectedLineColor = System.Drawing.Color.White;
            this.flatTabControl4.SelectedTextColor = System.Drawing.Color.White;
            this.flatTabControl4.ShowContentBorder = false;
            this.flatTabControl4.ShowTabBorders = false;
            this.flatTabControl4.Size = new System.Drawing.Size(638, 526);
            this.flatTabControl4.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.flatTabControl4.TabBorderColor = System.Drawing.Color.LightGray;
            this.flatTabControl4.TabColor = System.Drawing.Color.White;
            this.flatTabControl4.TabIndex = 0;
            this.flatTabControl4.TabRadius = 6;
            this.flatTabControl4.TextColor = System.Drawing.Color.White;
            this.flatTabControl4.UseSingleLine = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.tableLayoutPanel20);
            this.tabPage5.Location = new System.Drawing.Point(4, 44);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(630, 478);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel20
            // 
            this.tableLayoutPanel20.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel20.ColumnCount = 3;
            this.tableLayoutPanel20.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel20.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel20.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel20.Controls.Add(this.dataGridViewMemory, 1, 1);
            this.tableLayoutPanel20.Controls.Add(this.panel2, 1, 3);
            this.tableLayoutPanel20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel20.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel20.Name = "tableLayoutPanel20";
            this.tableLayoutPanel20.RowCount = 5;
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel20.Size = new System.Drawing.Size(630, 478);
            this.tableLayoutPanel20.TabIndex = 2;
            // 
            // dataGridViewMemory
            // 
            this.dataGridViewMemory.AllowUserToAddRows = false;
            this.dataGridViewMemory.AllowUserToDeleteRows = false;
            this.dataGridViewMemory.AllowUserToResizeColumns = false;
            this.dataGridViewMemory.AllowUserToResizeRows = false;
            this.dataGridViewMemory.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridViewMemory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewMemory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewMemory.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMemory.Location = new System.Drawing.Point(8, 8);
            this.dataGridViewMemory.Name = "dataGridViewMemory";
            this.dataGridViewMemory.ReadOnly = true;
            this.dataGridViewMemory.RowHeadersVisible = false;
            this.dataGridViewMemory.RowTemplate.Height = 23;
            this.dataGridViewMemory.Size = new System.Drawing.Size(614, 307);
            this.dataGridViewMemory.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(8, 341);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(614, 128);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.tableLayoutPanel2);
            this.tabPage6.Location = new System.Drawing.Point(4, 42);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(638, 526);
            this.tabPage6.TabIndex = 6;
            this.tabPage6.Text = "ER 속도센서 시험";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel2.Controls.Add(this.dataGridView2, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(638, 526);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(8, 8);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(622, 510);
            this.dataGridView2.TabIndex = 0;
            // 
            // roundedPanel5
            // 
            this.roundedPanel5.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel5.Controls.Add(this.customNumeric1);
            this.roundedPanel5.Controls.Add(this.roundedLabel9);
            this.roundedPanel5.Controls.Add(this.BtnStart);
            this.roundedPanel5.Controls.Add(this.roundedLabel10);
            this.roundedPanel5.Controls.Add(this.customProgressBar2);
            this.roundedPanel5.Controls.Add(this.roundedLabel11);
            this.roundedPanel5.Controls.Add(this.Label_Desc);
            this.roundedPanel5.Controls.Add(this.roundedLabel12);
            this.roundedPanel5.Controls.Add(this.roundedLabel13);
            this.roundedPanel5.Controls.Add(this.roundedLabel14);
            this.roundedPanel5.CornerRadius = 10;
            this.roundedPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundedPanel5.Location = new System.Drawing.Point(3, 599);
            this.roundedPanel5.Name = "roundedPanel5";
            this.roundedPanel5.Size = new System.Drawing.Size(646, 234);
            this.roundedPanel5.TabIndex = 82;
            // 
            // customNumeric1
            // 
            this.customNumeric1.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.customNumeric1.BackColor = System.Drawing.Color.White;
            this.customNumeric1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.customNumeric1.Location = new System.Drawing.Point(487, 90);
            this.customNumeric1.Minimum = 0D;
            this.customNumeric1.Name = "customNumeric1";
            this.customNumeric1.Padding = new System.Windows.Forms.Padding(1);
            this.customNumeric1.Size = new System.Drawing.Size(96, 30);
            this.customNumeric1.Step = 1D;
            this.customNumeric1.TabIndex = 50;
            // 
            // roundedLabel9
            // 
            this.roundedLabel9.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel9.FillColor = System.Drawing.Color.White;
            this.roundedLabel9.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel9.Location = new System.Drawing.Point(133, 148);
            this.roundedLabel9.Name = "roundedLabel9";
            this.roundedLabel9.Size = new System.Drawing.Size(120, 40);
            this.roundedLabel9.TabIndex = 49;
            this.roundedLabel9.Text = "00 : 00 : 00";
            this.roundedLabel9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnStart
            // 
            this.BtnStart.AutoCenterIcon = false;
            this.BtnStart.AutoCenterText = false;
            this.BtnStart.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnStart.BaseBorderColor = System.Drawing.Color.Black;
            this.BtnStart.BaseBorderThickness = 0;
            this.BtnStart.CornerRadius = 10;
            this.BtnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnStart.FlatAppearance.BorderSize = 0;
            this.BtnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnStart.Font = new System.Drawing.Font("맑은 고딕", 24F, System.Drawing.FontStyle.Bold);
            this.BtnStart.ForeColor = System.Drawing.Color.White;
            this.BtnStart.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(110)))), ((int)(((byte)(240)))));
            this.BtnStart.HoverBorderColor = System.Drawing.Color.White;
            this.BtnStart.HoverBorderThickness = 0;
            this.BtnStart.IconLocation = new System.Drawing.Point(20, 22);
            this.BtnStart.IconScale = 0.5F;
            this.BtnStart.Image = global::TCMSTester.Properties.Resources.play_button_arrowhead;
            this.BtnStart.Location = new System.Drawing.Point(603, 56);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(70)))), ((int)(((byte)(180)))));
            this.BtnStart.Size = new System.Drawing.Size(245, 94);
            this.BtnStart.TabIndex = 3;
            this.BtnStart.Text = "시험 시작";
            this.BtnStart.TextBottomMargin = 7;
            this.BtnStart.TextLocation = new System.Drawing.Point(70, 23);
            this.BtnStart.UseHoverBackColor = true;
            this.BtnStart.UseVisualStyleBackColor = false;
            this.BtnStart.Click += new System.EventHandler(this.button1_Click_3Async);
            // 
            // roundedLabel10
            // 
            this.roundedLabel10.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel10.FillColor = System.Drawing.Color.White;
            this.roundedLabel10.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel10.Location = new System.Drawing.Point(133, 100);
            this.roundedLabel10.Name = "roundedLabel10";
            this.roundedLabel10.Size = new System.Drawing.Size(120, 40);
            this.roundedLabel10.TabIndex = 48;
            this.roundedLabel10.Text = "- -";
            this.roundedLabel10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customProgressBar2
            // 
            this.customProgressBar2.BarThickness = 15;
            this.customProgressBar2.CornerRadius = 3;
            this.customProgressBar2.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.customProgressBar2.ForeColor = System.Drawing.Color.Black;
            this.customProgressBar2.Location = new System.Drawing.Point(28, 193);
            this.customProgressBar2.Maximum = 100;
            this.customProgressBar2.Name = "customProgressBar2";
            this.customProgressBar2.ProgressColor = System.Drawing.Color.DodgerBlue;
            this.customProgressBar2.ShowPercentage = true;
            this.customProgressBar2.Size = new System.Drawing.Size(847, 25);
            this.customProgressBar2.TabIndex = 42;
            this.customProgressBar2.Text = "customProgressBar2";
            this.customProgressBar2.TextMargin = 2;
            this.customProgressBar2.TrackColor = System.Drawing.Color.LightGray;
            this.customProgressBar2.UseAnimation = true;
            this.customProgressBar2.Value = 0;
            // 
            // roundedLabel11
            // 
            this.roundedLabel11.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel11.FillColor = System.Drawing.Color.White;
            this.roundedLabel11.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel11.Location = new System.Drawing.Point(133, 56);
            this.roundedLabel11.Name = "roundedLabel11";
            this.roundedLabel11.Size = new System.Drawing.Size(120, 40);
            this.roundedLabel11.TabIndex = 47;
            this.roundedLabel11.Text = "0 / 0 회";
            this.roundedLabel11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Desc
            // 
            this.Label_Desc.AutoSize = true;
            this.Label_Desc.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold);
            this.Label_Desc.Location = new System.Drawing.Point(26, 16);
            this.Label_Desc.Name = "Label_Desc";
            this.Label_Desc.Size = new System.Drawing.Size(95, 25);
            this.Label_Desc.TabIndex = 43;
            this.Label_Desc.Text = "진행 정보";
            // 
            // roundedLabel12
            // 
            this.roundedLabel12.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel12.FillColor = System.Drawing.Color.White;
            this.roundedLabel12.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel12.Location = new System.Drawing.Point(7, 148);
            this.roundedLabel12.Name = "roundedLabel12";
            this.roundedLabel12.Size = new System.Drawing.Size(120, 40);
            this.roundedLabel12.TabIndex = 46;
            this.roundedLabel12.Text = "경과 시간";
            this.roundedLabel12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedLabel13
            // 
            this.roundedLabel13.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel13.FillColor = System.Drawing.Color.White;
            this.roundedLabel13.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel13.Location = new System.Drawing.Point(7, 56);
            this.roundedLabel13.Name = "roundedLabel13";
            this.roundedLabel13.Size = new System.Drawing.Size(120, 40);
            this.roundedLabel13.TabIndex = 44;
            this.roundedLabel13.Text = "진행 차수";
            this.roundedLabel13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedLabel14
            // 
            this.roundedLabel14.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel14.FillColor = System.Drawing.Color.White;
            this.roundedLabel14.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel14.Location = new System.Drawing.Point(7, 100);
            this.roundedLabel14.Name = "roundedLabel14";
            this.roundedLabel14.Size = new System.Drawing.Size(120, 40);
            this.roundedLabel14.TabIndex = 45;
            this.roundedLabel14.Text = "현재 시험";
            this.roundedLabel14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundedPanel4
            // 
            this.roundedPanel4.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel4.Controls.Add(this.imagebtn2);
            this.roundedPanel4.Controls.Add(this.imagebtn1);
            this.roundedPanel4.Controls.Add(this.richTextBox_FailLog);
            this.roundedPanel4.Controls.Add(this.label3);
            this.roundedPanel4.Controls.Add(this.richTextBox_Log);
            this.roundedPanel4.CornerRadius = 10;
            this.roundedPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundedPanel4.Location = new System.Drawing.Point(667, 3);
            this.roundedPanel4.Margin = new System.Windows.Forms.Padding(15, 3, 3, 3);
            this.roundedPanel4.Name = "roundedPanel4";
            this.tableLayoutPanel7.SetRowSpan(this.roundedPanel4, 4);
            this.roundedPanel4.Size = new System.Drawing.Size(387, 830);
            this.roundedPanel4.TabIndex = 81;
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
            this.imagebtn2.Image = global::TCMSTester.Properties.Resources.report__1_;
            this.imagebtn2.Location = new System.Drawing.Point(150, 635);
            this.imagebtn2.Name = "imagebtn2";
            this.imagebtn2.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.imagebtn2.Size = new System.Drawing.Size(237, 120);
            this.imagebtn2.TabIndex = 85;
            this.imagebtn2.Text = "시험 결과가 여기에 표시됩니다.";
            this.imagebtn2.TextBottomMargin = 15;
            this.imagebtn2.TextLocation = new System.Drawing.Point(0, 0);
            this.imagebtn2.UseHoverBackColor = true;
            this.imagebtn2.UseVisualStyleBackColor = false;
            // 
            // imagebtn1
            // 
            this.imagebtn1.AutoCenterIcon = true;
            this.imagebtn1.AutoCenterText = true;
            this.imagebtn1.BackColor = System.Drawing.Color.White;
            this.imagebtn1.BaseBorderColor = System.Drawing.Color.Gray;
            this.imagebtn1.BaseBorderThickness = 0;
            this.imagebtn1.CornerRadius = 1;
            this.imagebtn1.Enabled = false;
            this.imagebtn1.FlatAppearance.BorderSize = 0;
            this.imagebtn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.imagebtn1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imagebtn1.ForeColor = System.Drawing.Color.DarkGray;
            this.imagebtn1.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.imagebtn1.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.imagebtn1.HoverBorderThickness = 0;
            this.imagebtn1.IconLocation = new System.Drawing.Point(0, 0);
            this.imagebtn1.IconScale = 0.5F;
            this.imagebtn1.Image = global::TCMSTester.Properties.Resources.note;
            this.imagebtn1.Location = new System.Drawing.Point(155, 232);
            this.imagebtn1.Name = "imagebtn1";
            this.imagebtn1.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.imagebtn1.Size = new System.Drawing.Size(237, 120);
            this.imagebtn1.TabIndex = 84;
            this.imagebtn1.Text = "시험 로그가 여기에 표시됩니다.";
            this.imagebtn1.TextBottomMargin = 15;
            this.imagebtn1.TextLocation = new System.Drawing.Point(0, 0);
            this.imagebtn1.UseHoverBackColor = true;
            this.imagebtn1.UseVisualStyleBackColor = false;
            // 
            // richTextBox_FailLog
            // 
            this.richTextBox_FailLog.BackColor = System.Drawing.Color.White;
            this.richTextBox_FailLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_FailLog.Location = new System.Drawing.Point(30, 573);
            this.richTextBox_FailLog.Name = "richTextBox_FailLog";
            this.richTextBox_FailLog.ReadOnly = true;
            this.richTextBox_FailLog.Size = new System.Drawing.Size(475, 233);
            this.richTextBox_FailLog.TabIndex = 83;
            this.richTextBox_FailLog.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(29, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 25);
            this.label3.TabIndex = 50;
            this.label3.Text = "시험 로그";
            // 
            // richTextBox_Log
            // 
            this.richTextBox_Log.BackColor = System.Drawing.Color.White;
            this.richTextBox_Log.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_Log.Location = new System.Drawing.Point(30, 50);
            this.richTextBox_Log.Name = "richTextBox_Log";
            this.richTextBox_Log.ReadOnly = true;
            this.richTextBox_Log.Size = new System.Drawing.Size(475, 503);
            this.richTextBox_Log.TabIndex = 0;
            this.richTextBox_Log.Text = "";
            // 
            // BtnPLC
            // 
            this.BtnPLC.BackColor = System.Drawing.Color.LightSlateGray;
            this.BtnPLC.Image = global::TCMSTester.Properties.Resources.Setting48;
            this.BtnPLC.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnPLC.Location = new System.Drawing.Point(2, 136);
            this.BtnPLC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BtnPLC.Name = "BtnPLC";
            this.BtnPLC.Size = new System.Drawing.Size(46, 119);
            this.BtnPLC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.BtnPLC.TabIndex = 22;
            this.BtnPLC.TabStop = false;
            this.BtnPLC.Tag = "6";
            this.BtnPLC.Visible = false;
            // 
            // BtnPrint
            // 
            this.BtnPrint.AutoCenterIcon = true;
            this.BtnPrint.AutoCenterText = true;
            this.BtnPrint.BackColor = System.Drawing.Color.Transparent;
            this.BtnPrint.BaseBorderColor = System.Drawing.Color.Black;
            this.BtnPrint.BaseBorderThickness = 0;
            this.BtnPrint.CornerRadius = 10;
            this.BtnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnPrint.FlatAppearance.BorderSize = 0;
            this.BtnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnPrint.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnPrint.ForeColor = System.Drawing.Color.White;
            this.BtnPrint.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnPrint.HoverBorderColor = System.Drawing.Color.White;
            this.BtnPrint.HoverBorderThickness = 3;
            this.BtnPrint.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnPrint.IconScale = 0.5F;
            this.BtnPrint.Image = global::TCMSTester.Properties.Resources.print2er;
            this.BtnPrint.Location = new System.Drawing.Point(1481, 136);
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnPrint.Size = new System.Drawing.Size(44, 119);
            this.BtnPrint.TabIndex = 33;
            this.BtnPrint.Text = "인쇄";
            this.BtnPrint.TextBottomMargin = 7;
            this.BtnPrint.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnPrint.UseHoverBackColor = false;
            this.BtnPrint.UseVisualStyleBackColor = false;
            this.BtnPrint.Visible = false;
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.roundedPanel1, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(53, 489);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(348, 483);
            this.tableLayoutPanel5.TabIndex = 89;
            // 
            // roundedPanel1
            // 
            this.roundedPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel1.Controls.Add(this.Button_DeSelect_All);
            this.roundedPanel1.Controls.Add(this.Button_Select_All);
            this.roundedPanel1.Controls.Add(this.label2);
            this.roundedPanel1.Controls.Add(this.label1);
            this.roundedPanel1.Controls.Add(this.TestCount);
            this.roundedPanel1.Controls.Add(this.roundedLabel8);
            this.roundedPanel1.Controls.Add(this.modernTreeView1);
            this.roundedPanel1.CornerRadius = 10;
            this.roundedPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundedPanel1.Location = new System.Drawing.Point(3, 3);
            this.roundedPanel1.Name = "roundedPanel1";
            this.tableLayoutPanel5.SetRowSpan(this.roundedPanel1, 2);
            this.roundedPanel1.Size = new System.Drawing.Size(342, 477);
            this.roundedPanel1.TabIndex = 57;
            // 
            // Button_DeSelect_All
            // 
            this.Button_DeSelect_All.AutoCenterIcon = false;
            this.Button_DeSelect_All.AutoCenterText = true;
            this.Button_DeSelect_All.BackColor = System.Drawing.Color.White;
            this.Button_DeSelect_All.BaseBorderColor = System.Drawing.Color.SteelBlue;
            this.Button_DeSelect_All.BaseBorderThickness = 1;
            this.Button_DeSelect_All.CornerRadius = 10;
            this.Button_DeSelect_All.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_DeSelect_All.FlatAppearance.BorderSize = 0;
            this.Button_DeSelect_All.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_DeSelect_All.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Button_DeSelect_All.ForeColor = System.Drawing.Color.SteelBlue;
            this.Button_DeSelect_All.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.Button_DeSelect_All.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(110)))), ((int)(((byte)(240)))));
            this.Button_DeSelect_All.HoverBorderThickness = 2;
            this.Button_DeSelect_All.IconLocation = new System.Drawing.Point(15, 13);
            this.Button_DeSelect_All.IconScale = 0.5F;
            this.Button_DeSelect_All.Location = new System.Drawing.Point(180, 393);
            this.Button_DeSelect_All.Name = "Button_DeSelect_All";
            this.Button_DeSelect_All.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.Button_DeSelect_All.Size = new System.Drawing.Size(148, 50);
            this.Button_DeSelect_All.TabIndex = 80;
            this.Button_DeSelect_All.Text = "전체항목 해제";
            this.Button_DeSelect_All.TextBottomMargin = 15;
            this.Button_DeSelect_All.TextLocation = new System.Drawing.Point(29, 15);
            this.Button_DeSelect_All.UseHoverBackColor = false;
            this.Button_DeSelect_All.UseVisualStyleBackColor = false;
            // 
            // Button_Select_All
            // 
            this.Button_Select_All.AutoCenterIcon = false;
            this.Button_Select_All.AutoCenterText = true;
            this.Button_Select_All.BackColor = System.Drawing.Color.RoyalBlue;
            this.Button_Select_All.BaseBorderColor = System.Drawing.Color.SteelBlue;
            this.Button_Select_All.BaseBorderThickness = 1;
            this.Button_Select_All.CornerRadius = 10;
            this.Button_Select_All.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Select_All.FlatAppearance.BorderSize = 0;
            this.Button_Select_All.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Select_All.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Button_Select_All.ForeColor = System.Drawing.Color.White;
            this.Button_Select_All.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(110)))), ((int)(((byte)(240)))));
            this.Button_Select_All.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(110)))), ((int)(((byte)(240)))));
            this.Button_Select_All.HoverBorderThickness = 0;
            this.Button_Select_All.IconLocation = new System.Drawing.Point(20, 14);
            this.Button_Select_All.IconScale = 0.4F;
            this.Button_Select_All.Location = new System.Drawing.Point(18, 393);
            this.Button_Select_All.Name = "Button_Select_All";
            this.Button_Select_All.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(70)))), ((int)(((byte)(180)))));
            this.Button_Select_All.Size = new System.Drawing.Size(148, 50);
            this.Button_Select_All.TabIndex = 79;
            this.Button_Select_All.Text = "전체항목 선택";
            this.Button_Select_All.TextBottomMargin = 15;
            this.Button_Select_All.TextLocation = new System.Drawing.Point(29, 15);
            this.Button_Select_All.UseHoverBackColor = true;
            this.Button_Select_All.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12.25F);
            this.label2.Location = new System.Drawing.Point(38, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 23);
            this.label2.TabIndex = 78;
            this.label2.Text = "시험 항목";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12.25F);
            this.label1.Location = new System.Drawing.Point(38, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 23);
            this.label1.TabIndex = 77;
            this.label1.Text = "시험 차수";
            // 
            // TestCount
            // 
            this.TestCount.Font = new System.Drawing.Font("맑은 고딕", 16F, System.Drawing.FontStyle.Bold);
            this.TestCount.Location = new System.Drawing.Point(21, 109);
            this.TestCount.Name = "TestCount";
            this.TestCount.Size = new System.Drawing.Size(305, 46);
            this.TestCount.TabIndex = 76;
            this.TestCount.Text = "stepCountControl1";
            this.TestCount.Value = 1;
            // 
            // roundedLabel8
            // 
            this.roundedLabel8.AutoCenterImage = false;
            this.roundedLabel8.AutoCenterText = false;
            this.roundedLabel8.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel8.CustomImage = global::TCMSTester.Properties.Resources.settings__1_;
            this.roundedLabel8.FillColor = System.Drawing.Color.White;
            this.roundedLabel8.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel8.ImageLocation = new System.Drawing.Point(8, 6);
            this.roundedLabel8.ImageSize = new System.Drawing.Size(28, 28);
            this.roundedLabel8.Location = new System.Drawing.Point(13, 18);
            this.roundedLabel8.Name = "roundedLabel8";
            this.roundedLabel8.Size = new System.Drawing.Size(176, 40);
            this.roundedLabel8.TabIndex = 69;
            this.roundedLabel8.Text = "시험 설정";
            this.roundedLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.roundedLabel8.TextLocation = new System.Drawing.Point(43, 8);
            // 
            // modernTreeView1
            // 
            this.modernTreeView1.AutoExpandAllOnLoad = true;
            this.modernTreeView1.BackColor = System.Drawing.Color.White;
            this.modernTreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.modernTreeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.modernTreeView1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.modernTreeView1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.modernTreeView1.FullRowSelect = true;
            this.modernTreeView1.ItemHeight = 32;
            this.modernTreeView1.LineColor = System.Drawing.Color.White;
            this.modernTreeView1.Location = new System.Drawing.Point(15, 218);
            this.modernTreeView1.Name = "modernTreeView1";
            treeNode1.Name = "디지털 입출력 시험";
            treeNode1.Text = "디지털 입출력 시험";
            treeNode2.Name = "아날로그 입출력 시험";
            treeNode2.Text = "아날로그 입출력 시험";
            treeNode3.Name = "입출력 시험";
            treeNode3.Text = "입·출력 시험";
            treeNode4.Name = "통신 시험";
            treeNode4.Text = "통신 시험";
            treeNode5.Name = "메모리 시험";
            treeNode5.Text = "메모리 시험";
            treeNode6.Name = "ER 속도센서 시험";
            treeNode6.Text = "ER 속도센서 시험";
            this.modernTreeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6});
            this.modernTreeView1.ShowLines = false;
            this.modernTreeView1.ShowPlusMinus = false;
            this.modernTreeView1.Size = new System.Drawing.Size(323, 282);
            this.modernTreeView1.TabIndex = 75;
            // 
            // roundedPanel6
            // 
            this.roundedPanel6.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(238)))));
            this.roundedPanel6.Controls.Add(this.BtnChange);
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
            this.roundedPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundedPanel6.Location = new System.Drawing.Point(53, 136);
            this.roundedPanel6.Name = "roundedPanel6";
            this.roundedPanel6.Size = new System.Drawing.Size(348, 345);
            this.roundedPanel6.TabIndex = 90;
            // 
            // BtnChange
            // 
            this.BtnChange.AutoCenterIcon = false;
            this.BtnChange.AutoCenterText = false;
            this.BtnChange.BackColor = System.Drawing.Color.White;
            this.BtnChange.BaseBorderColor = System.Drawing.Color.Gray;
            this.BtnChange.BaseBorderThickness = 0;
            this.BtnChange.CornerRadius = 20;
            this.BtnChange.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnChange.FlatAppearance.BorderSize = 0;
            this.BtnChange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnChange.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BtnChange.ForeColor = System.Drawing.Color.RoyalBlue;
            this.BtnChange.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnChange.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.BtnChange.HoverBorderThickness = 0;
            this.BtnChange.IconLocation = new System.Drawing.Point(10, 18);
            this.BtnChange.IconScale = 0.4F;
            this.BtnChange.Image = global::TCMSTester.Properties.Resources.change;
            this.BtnChange.Location = new System.Drawing.Point(235, 5);
            this.BtnChange.Name = "BtnChange";
            this.BtnChange.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnChange.Size = new System.Drawing.Size(107, 66);
            this.BtnChange.TabIndex = 70;
            this.BtnChange.Text = "변경";
            this.BtnChange.TextBottomMargin = 2;
            this.BtnChange.TextLocation = new System.Drawing.Point(40, 22);
            this.BtnChange.UseHoverBackColor = false;
            this.BtnChange.UseVisualStyleBackColor = false;
            this.BtnChange.Click += new System.EventHandler(this.BtnChange_Click);
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
            this.roundedLabel1.Location = new System.Drawing.Point(13, 18);
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
            this.Label_Tester.Location = new System.Drawing.Point(146, 274);
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
            this.Label_Train.Location = new System.Drawing.Point(146, 234);
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
            this.Label_Fleet.Location = new System.Drawing.Point(146, 194);
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
            this.Label_Serial.Location = new System.Drawing.Point(146, 154);
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
            this.roundedLabel7.Location = new System.Drawing.Point(20, 274);
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
            this.roundedLabel6.Location = new System.Drawing.Point(20, 234);
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
            this.roundedLabel5.Location = new System.Drawing.Point(20, 194);
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
            this.roundedLabel4.Location = new System.Drawing.Point(20, 154);
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
            this.roundedLabel3.Location = new System.Drawing.Point(20, 114);
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
            this.Label_Unit.Location = new System.Drawing.Point(146, 114);
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
            this.Label_Date.Location = new System.Drawing.Point(146, 74);
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
            this.roundedLabel2.Location = new System.Drawing.Point(20, 74);
            this.roundedLabel2.Name = "roundedLabel2";
            this.roundedLabel2.Size = new System.Drawing.Size(126, 40);
            this.roundedLabel2.TabIndex = 57;
            this.roundedLabel2.Text = "시험 일자";
            this.roundedLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
            this.miniToolStrip.Location = new System.Drawing.Point(0, 0);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.Size = new System.Drawing.Size(200, 24);
            this.miniToolStrip.TabIndex = 0;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(12, 20);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(200, 24);
            this.menuStrip1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1194, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(14, 119);
            this.button1.TabIndex = 86;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1528, 1057);
            this.Controls.Add(this.tableLayoutPanel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.miniToolStrip;
            this.Name = "FormMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.mainTabControl1.ResumeLayout(false);
            this.tabPage17.ResumeLayout(false);
            this.flatTabControl1.ResumeLayout(false);
            this.tabPageDI1.ResumeLayout(false);
            this.tableLayoutPanel12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDI1)).EndInit();
            this.tabPageDI2.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDI2)).EndInit();
            this.tabPageDI3.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDI3)).EndInit();
            this.tabPageDO.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDO)).EndInit();
            this.tabPageAnalog.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAnalog)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.flatTabControl4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tableLayoutPanel20.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMemory)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.roundedPanel5.ResumeLayout(false);
            this.roundedPanel5.PerformLayout();
            this.roundedPanel4.ResumeLayout(false);
            this.roundedPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnPLC)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.roundedPanel1.ResumeLayout(false);
            this.roundedPanel1.PerformLayout();
            this.roundedPanel6.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ProgressBar ProgressBar_Run;
        private System.Windows.Forms.Button Btn_EmergencyStop;
        private System.Windows.Forms.Timer Timer_Measure;
        private System.IO.Ports.SerialPort Serial_PWM;
        private System.IO.Ports.SerialPort Serial_DCPower1;
        private System.Windows.Forms.Timer Timer_Start;
        private System.Windows.Forms.Timer Timer_Clear;
        private System.Windows.Forms.Timer timerRunPowRun;
        private System.Windows.Forms.Timer timerBasicRun;
        private System.Windows.Forms.Timer timerRunStop;
        private System.Windows.Forms.Timer timerRunForRun;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.MenuStrip miniToolStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel panel3;
        private CustomIconButton BtnResult;
        private CustomIconButton BtnConfig;
        private CustomIconButton BtnDB;
        private CustomIconButton BtnNew;
        private CustomIconButton BtnDiagnostic;
        private CustomIconButton BtnExit;
        private System.Windows.Forms.PictureBox BtnPLC;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private RoundedPanel roundedPanel1;
        private RoundedLabel roundedLabel8;
        private MainTabControl mainTabControl1;
        private System.Windows.Forms.TabPage tabPage17;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage6;
        private ModernTreeView modernTreeView1;
        private FlatTabControl flatTabControl4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel20;
        private System.Windows.Forms.DataGridView dataGridViewMemory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private FlatTabControl flatTabControl1;
        private System.Windows.Forms.TabPage tabPageDI1;
        private System.Windows.Forms.TabPage tabPageAnalog;
        private CustomIconButton BtnPrint;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        public System.Windows.Forms.DataGridView dataGridViewAnalog;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonMVB3;
        private System.Windows.Forms.RadioButton radioButtonWTB3;
        private System.Windows.Forms.RadioButton radioButtonMVB2;
        private System.Windows.Forms.RadioButton radioButtonMVB1;
        private System.Windows.Forms.RadioButton radioButtonWTB2;
        private System.Windows.Forms.RadioButton radioButtonWTB1;
        private System.Windows.Forms.RadioButton radioButtonRS3;
        private System.Windows.Forms.RadioButton radioButtonRS2;
        private System.Windows.Forms.RadioButton radioButtonRS1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabPage tabPageDO;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        public System.Windows.Forms.DataGridView dataGridViewDO;
        private System.Windows.Forms.Label label1;
        private CustomControl.StepCountControl TestCount;
        private System.Windows.Forms.Label label2;
        private CustomIconButton Button_DeSelect_All;
        private CustomIconButton Button_Select_All;
        private RoundedPanel roundedPanel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBox_Log;
        private RoundedPanel roundedPanel5;
        private RoundedLabel roundedLabel9;
        private RoundedLabel roundedLabel10;
        private YourNamespace.CustomProgressBar customProgressBar2;
        private RoundedLabel roundedLabel11;
        private System.Windows.Forms.Label Label_Desc;
        private RoundedLabel roundedLabel12;
        private RoundedLabel roundedLabel13;
        private RoundedLabel roundedLabel14;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
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
        private CustomIconButton BtnStart;
        private System.Windows.Forms.TabPage tabPageDI2;
        private System.Windows.Forms.TabPage tabPageDI3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        public System.Windows.Forms.DataGridView dataGridViewDI2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        public System.Windows.Forms.DataGridView dataGridViewDI3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        public System.Windows.Forms.DataGridView dataGridViewDI1;
        private System.Windows.Forms.RichTextBox richTextBox_FailLog;
        private System.Windows.Forms.Button button2;
        private CustomIconButton imagebtn1;
        private CustomIconButton imagebtn2;
        private RoundedLabel roundedLabel15;
        private CustomIconButton BtnChange;
        private CustomNumeric customNumeric1;
        private System.Windows.Forms.Button button1;
    }
}

