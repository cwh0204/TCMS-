namespace CITester
{
    partial class FormDB
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
            this.TextBox_Password = new System.Windows.Forms.TextBox();
            this.BtnTesterPassword = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.BtnClose = new CustomIconButton();
            this.flatTabControl1 = new CITester.FlatTabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.roundedPanel2 = new CITester.RoundedPanel();
            this.roundedLabel2 = new CITester.RoundedLabel();
            this.Button_Serial_Delete = new CustomIconButton();
            this.Button_Serial_Add = new CustomIconButton();
            this.TextBox_SerialNo = new System.Windows.Forms.TextBox();
            this.TextBox_TrainNo = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.TextBox_GroupNo = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.roundedPanel1 = new CITester.RoundedPanel();
            this.dataGridView_Serial = new System.Windows.Forms.DataGridView();
            this.BtnSearch = new CustomIconButton();
            this.roundedLabel1 = new CITester.RoundedLabel();
            this.TextBox_Search_SerialNo = new System.Windows.Forms.TextBox();
            this.CheckBox_SerialNo = new System.Windows.Forms.CheckBox();
            this.CheckBox_TrainNo = new System.Windows.Forms.CheckBox();
            this.TextBox_Search_TrainNo = new System.Windows.Forms.TextBox();
            this.TextBox_Search_GroupNo = new System.Windows.Forms.TextBox();
            this.CheckBox_GroupNo = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.roundedPanel4 = new CITester.RoundedPanel();
            this.BtnTesterDelete = new CustomIconButton();
            this.BtnTesterAdd = new CustomIconButton();
            this.roundedLabel4 = new CITester.RoundedLabel();
            this.TextBox_Tester = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TextBox_ID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.TextBox_Department = new System.Windows.Forms.TextBox();
            this.roundedPanel3 = new CITester.RoundedPanel();
            this.dataGridView_Tester = new System.Windows.Forms.DataGridView();
            this.roundedLabel3 = new CITester.RoundedLabel();
            this.flatTabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.roundedPanel2.SuspendLayout();
            this.roundedPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Serial)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.roundedPanel4.SuspendLayout();
            this.roundedPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Tester)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBox_Password
            // 
            this.TextBox_Password.Font = new System.Drawing.Font("맑은 고딕", 10.2F, System.Drawing.FontStyle.Bold);
            this.TextBox_Password.Location = new System.Drawing.Point(200, 730);
            this.TextBox_Password.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Password.Name = "TextBox_Password";
            this.TextBox_Password.PasswordChar = '*';
            this.TextBox_Password.Size = new System.Drawing.Size(155, 26);
            this.TextBox_Password.TabIndex = 90;
            this.TextBox_Password.Text = "0";
            this.TextBox_Password.Visible = false;
            // 
            // BtnTesterPassword
            // 
            this.BtnTesterPassword.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BtnTesterPassword.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.BtnTesterPassword.Location = new System.Drawing.Point(56, 706);
            this.BtnTesterPassword.Margin = new System.Windows.Forms.Padding(2);
            this.BtnTesterPassword.Name = "BtnTesterPassword";
            this.BtnTesterPassword.Size = new System.Drawing.Size(142, 40);
            this.BtnTesterPassword.TabIndex = 82;
            this.BtnTesterPassword.Text = "사용자 비밀번호 변경";
            this.BtnTesterPassword.UseVisualStyleBackColor = false;
            this.BtnTesterPassword.Visible = false;
            this.BtnTesterPassword.Click += new System.EventHandler(this.BtnTesterPassword_Click);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label36.Location = new System.Drawing.Point(203, 716);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(74, 21);
            this.label36.TabIndex = 91;
            this.label36.Text = "비밀번호";
            this.label36.Visible = false;
            // 
            // BtnClose
            // 
            this.BtnClose.AutoCenterIcon = false;
            this.BtnClose.AutoCenterText = true;
            this.BtnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(110)))), ((int)(((byte)(144)))));
            this.BtnClose.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.BtnClose.BaseBorderThickness = 0;
            this.BtnClose.CornerRadius = 5;
            this.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClose.FlatAppearance.BorderSize = 0;
            this.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnClose.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BtnClose.ForeColor = System.Drawing.Color.White;
            this.BtnClose.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(147)))), ((int)(((byte)(175)))));
            this.BtnClose.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.BtnClose.HoverBorderThickness = 0;
            this.BtnClose.IconLocation = new System.Drawing.Point(25, 9);
            this.BtnClose.IconScale = 0.6F;
            this.BtnClose.Image = global::TCMSTester.Properties.Resources.shut_down_4063921;
            this.BtnClose.Location = new System.Drawing.Point(360, 724);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(71)))), ((int)(((byte)(102)))));
            this.BtnClose.Size = new System.Drawing.Size(198, 44);
            this.BtnClose.TabIndex = 84;
            this.BtnClose.Text = "종 료";
            this.BtnClose.TextBottomMargin = 12;
            this.BtnClose.TextLocation = new System.Drawing.Point(55, 11);
            this.BtnClose.UseHoverBackColor = true;
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // flatTabControl1
            // 
            this.flatTabControl1.ContentBackColor = System.Drawing.Color.White;
            this.flatTabControl1.ContentBorderColor = System.Drawing.Color.LightGray;
            this.flatTabControl1.Controls.Add(this.tabPage3);
            this.flatTabControl1.Controls.Add(this.tabPage4);
            this.flatTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.flatTabControl1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.flatTabControl1.HeaderBackColor = System.Drawing.SystemColors.Control;
            this.flatTabControl1.HoverTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.flatTabControl1.ItemSize = new System.Drawing.Size(120, 40);
            this.flatTabControl1.LineColor = System.Drawing.SystemColors.Control;
            this.flatTabControl1.LinePadding = 20;
            this.flatTabControl1.Location = new System.Drawing.Point(12, 10);
            this.flatTabControl1.Name = "flatTabControl1";
            this.flatTabControl1.Padding = new System.Drawing.Point(0, 0);
            this.flatTabControl1.SelectedColor = System.Drawing.Color.White;
            this.flatTabControl1.SelectedIndex = 0;
            this.flatTabControl1.SelectedLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(60)))), ((int)(((byte)(105)))));
            this.flatTabControl1.SelectedTextColor = System.Drawing.Color.Black;
            this.flatTabControl1.ShowContentBorder = true;
            this.flatTabControl1.ShowTabBorders = true;
            this.flatTabControl1.Size = new System.Drawing.Size(543, 704);
            this.flatTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.flatTabControl1.TabBorderColor = System.Drawing.Color.LightGray;
            this.flatTabControl1.TabColor = System.Drawing.SystemColors.Control;
            this.flatTabControl1.TabIndex = 84;
            this.flatTabControl1.TabRadius = 6;
            this.flatTabControl1.TextColor = System.Drawing.Color.DimGray;
            this.flatTabControl1.UseSingleLine = true;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.White;
            this.tabPage3.Controls.Add(this.roundedPanel2);
            this.tabPage3.Controls.Add(this.roundedPanel1);
            this.tabPage3.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabPage3.Location = new System.Drawing.Point(4, 44);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(535, 656);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "CI 정보";
            // 
            // roundedPanel2
            // 
            this.roundedPanel2.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel2.Controls.Add(this.roundedLabel2);
            this.roundedPanel2.Controls.Add(this.Button_Serial_Delete);
            this.roundedPanel2.Controls.Add(this.Button_Serial_Add);
            this.roundedPanel2.Controls.Add(this.TextBox_SerialNo);
            this.roundedPanel2.Controls.Add(this.TextBox_TrainNo);
            this.roundedPanel2.Controls.Add(this.label18);
            this.roundedPanel2.Controls.Add(this.TextBox_GroupNo);
            this.roundedPanel2.Controls.Add(this.label17);
            this.roundedPanel2.Controls.Add(this.label2);
            this.roundedPanel2.Location = new System.Drawing.Point(6, 465);
            this.roundedPanel2.Name = "roundedPanel2";
            this.roundedPanel2.Size = new System.Drawing.Size(529, 188);
            this.roundedPanel2.TabIndex = 84;
            // 
            // roundedLabel2
            // 
            this.roundedLabel2.AutoCenterImage = false;
            this.roundedLabel2.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel2.CustomImage = global::TCMSTester.Properties.Resources.project;
            this.roundedLabel2.FillColor = System.Drawing.Color.White;
            this.roundedLabel2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel2.ImageLocation = new System.Drawing.Point(5, 4);
            this.roundedLabel2.Location = new System.Drawing.Point(19, 10);
            this.roundedLabel2.Name = "roundedLabel2";
            this.roundedLabel2.Size = new System.Drawing.Size(179, 40);
            this.roundedLabel2.TabIndex = 83;
            this.roundedLabel2.Text = "CI 정보 관리";
            this.roundedLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button_Serial_Delete
            // 
            this.Button_Serial_Delete.AutoCenterIcon = false;
            this.Button_Serial_Delete.AutoCenterText = false;
            this.Button_Serial_Delete.BackColor = System.Drawing.Color.White;
            this.Button_Serial_Delete.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(101)))), ((int)(((byte)(105)))));
            this.Button_Serial_Delete.BaseBorderThickness = 1;
            this.Button_Serial_Delete.CornerRadius = 5;
            this.Button_Serial_Delete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Serial_Delete.FlatAppearance.BorderSize = 0;
            this.Button_Serial_Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Serial_Delete.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Button_Serial_Delete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(101)))), ((int)(((byte)(105)))));
            this.Button_Serial_Delete.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.Button_Serial_Delete.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(101)))), ((int)(((byte)(105)))));
            this.Button_Serial_Delete.HoverBorderThickness = 3;
            this.Button_Serial_Delete.IconLocation = new System.Drawing.Point(20, 11);
            this.Button_Serial_Delete.IconScale = 0.5F;
            this.Button_Serial_Delete.Image = global::TCMSTester.Properties.Resources.trash_can;
            this.Button_Serial_Delete.Location = new System.Drawing.Point(316, 107);
            this.Button_Serial_Delete.Name = "Button_Serial_Delete";
            this.Button_Serial_Delete.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.Button_Serial_Delete.Size = new System.Drawing.Size(177, 44);
            this.Button_Serial_Delete.TabIndex = 82;
            this.Button_Serial_Delete.Text = "CI 정보 삭제";
            this.Button_Serial_Delete.TextBottomMargin = 20;
            this.Button_Serial_Delete.TextLocation = new System.Drawing.Point(55, 11);
            this.Button_Serial_Delete.UseHoverBackColor = false;
            this.Button_Serial_Delete.UseVisualStyleBackColor = false;
            this.Button_Serial_Delete.Click += new System.EventHandler(this.Button_Serial_Delete_Click);
            // 
            // Button_Serial_Add
            // 
            this.Button_Serial_Add.AutoCenterIcon = false;
            this.Button_Serial_Add.AutoCenterText = false;
            this.Button_Serial_Add.BackColor = System.Drawing.Color.White;
            this.Button_Serial_Add.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Button_Serial_Add.BaseBorderThickness = 1;
            this.Button_Serial_Add.CornerRadius = 5;
            this.Button_Serial_Add.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Serial_Add.FlatAppearance.BorderSize = 0;
            this.Button_Serial_Add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Serial_Add.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Button_Serial_Add.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Button_Serial_Add.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.Button_Serial_Add.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Button_Serial_Add.HoverBorderThickness = 3;
            this.Button_Serial_Add.IconLocation = new System.Drawing.Point(18, 9);
            this.Button_Serial_Add.IconScale = 0.6F;
            this.Button_Serial_Add.Image = global::TCMSTester.Properties.Resources.free_icon_plus_button_11527960;
            this.Button_Serial_Add.Location = new System.Drawing.Point(316, 52);
            this.Button_Serial_Add.Name = "Button_Serial_Add";
            this.Button_Serial_Add.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.Button_Serial_Add.Size = new System.Drawing.Size(177, 44);
            this.Button_Serial_Add.TabIndex = 81;
            this.Button_Serial_Add.Text = "CI 정보 추가";
            this.Button_Serial_Add.TextBottomMargin = 20;
            this.Button_Serial_Add.TextLocation = new System.Drawing.Point(55, 11);
            this.Button_Serial_Add.UseHoverBackColor = false;
            this.Button_Serial_Add.UseVisualStyleBackColor = false;
            this.Button_Serial_Add.Click += new System.EventHandler(this.Button_Serial_Add_Click);
            // 
            // TextBox_SerialNo
            // 
            this.TextBox_SerialNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBox_SerialNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_SerialNo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextBox_SerialNo.Location = new System.Drawing.Point(99, 128);
            this.TextBox_SerialNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_SerialNo.Name = "TextBox_SerialNo";
            this.TextBox_SerialNo.Size = new System.Drawing.Size(147, 27);
            this.TextBox_SerialNo.TabIndex = 5;
            this.TextBox_SerialNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextBox_TrainNo
            // 
            this.TextBox_TrainNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_TrainNo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextBox_TrainNo.Location = new System.Drawing.Point(99, 95);
            this.TextBox_TrainNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_TrainNo.Name = "TextBox_TrainNo";
            this.TextBox_TrainNo.Size = new System.Drawing.Size(147, 27);
            this.TextBox_TrainNo.TabIndex = 4;
            this.TextBox_TrainNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.White;
            this.label18.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.label18.ForeColor = System.Drawing.Color.Black;
            this.label18.Location = new System.Drawing.Point(29, 98);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(69, 20);
            this.label18.TabIndex = 77;
            this.label18.Tag = "1";
            this.label18.Text = "차량번호";
            // 
            // TextBox_GroupNo
            // 
            this.TextBox_GroupNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_GroupNo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextBox_GroupNo.Location = new System.Drawing.Point(99, 62);
            this.TextBox_GroupNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_GroupNo.Name = "TextBox_GroupNo";
            this.TextBox_GroupNo.Size = new System.Drawing.Size(147, 27);
            this.TextBox_GroupNo.TabIndex = 3;
            this.TextBox_GroupNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.White;
            this.label17.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(29, 66);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 20);
            this.label17.TabIndex = 76;
            this.label17.Tag = "1";
            this.label17.Text = "편성번호";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(29, 131);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 80;
            this.label2.Tag = "1";
            this.label2.Text = "일련번호";
            // 
            // roundedPanel1
            // 
            this.roundedPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel1.Controls.Add(this.dataGridView_Serial);
            this.roundedPanel1.Controls.Add(this.BtnSearch);
            this.roundedPanel1.Controls.Add(this.roundedLabel1);
            this.roundedPanel1.Controls.Add(this.TextBox_Search_SerialNo);
            this.roundedPanel1.Controls.Add(this.CheckBox_SerialNo);
            this.roundedPanel1.Controls.Add(this.CheckBox_TrainNo);
            this.roundedPanel1.Controls.Add(this.TextBox_Search_TrainNo);
            this.roundedPanel1.Controls.Add(this.TextBox_Search_GroupNo);
            this.roundedPanel1.Controls.Add(this.CheckBox_GroupNo);
            this.roundedPanel1.Location = new System.Drawing.Point(6, 9);
            this.roundedPanel1.Name = "roundedPanel1";
            this.roundedPanel1.Size = new System.Drawing.Size(529, 448);
            this.roundedPanel1.TabIndex = 83;
            // 
            // dataGridView_Serial
            // 
            this.dataGridView_Serial.AllowUserToAddRows = false;
            this.dataGridView_Serial.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridView_Serial.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Serial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Serial.Location = new System.Drawing.Point(23, 164);
            this.dataGridView_Serial.Name = "dataGridView_Serial";
            this.dataGridView_Serial.RowTemplate.Height = 23;
            this.dataGridView_Serial.Size = new System.Drawing.Size(472, 270);
            this.dataGridView_Serial.TabIndex = 13;
            this.dataGridView_Serial.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Serial_CellClick);
            // 
            // BtnSearch
            // 
            this.BtnSearch.AutoCenterIcon = true;
            this.BtnSearch.AutoCenterText = true;
            this.BtnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(60)))), ((int)(((byte)(105)))));
            this.BtnSearch.BaseBorderColor = System.Drawing.Color.Gray;
            this.BtnSearch.BaseBorderThickness = 0;
            this.BtnSearch.CornerRadius = 10;
            this.BtnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSearch.FlatAppearance.BorderSize = 0;
            this.BtnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSearch.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BtnSearch.ForeColor = System.Drawing.Color.White;
            this.BtnSearch.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(85)))), ((int)(((byte)(145)))));
            this.BtnSearch.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.BtnSearch.HoverBorderThickness = 0;
            this.BtnSearch.IconLocation = new System.Drawing.Point(0, 0);
            this.BtnSearch.IconScale = 0.3F;
            this.BtnSearch.Image = global::TCMSTester.Properties.Resources.Artboard_6_3x;
            this.BtnSearch.Location = new System.Drawing.Point(375, 33);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(75)))));
            this.BtnSearch.Size = new System.Drawing.Size(120, 120);
            this.BtnSearch.TabIndex = 12;
            this.BtnSearch.Text = "검 색";
            this.BtnSearch.TextBottomMargin = 20;
            this.BtnSearch.TextLocation = new System.Drawing.Point(0, 0);
            this.BtnSearch.UseHoverBackColor = true;
            this.BtnSearch.UseVisualStyleBackColor = false;
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // roundedLabel1
            // 
            this.roundedLabel1.AutoCenterImage = false;
            this.roundedLabel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel1.CustomImage = global::TCMSTester.Properties.Resources.paper__1_;
            this.roundedLabel1.FillColor = System.Drawing.Color.White;
            this.roundedLabel1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel1.ImageLocation = new System.Drawing.Point(5, 4);
            this.roundedLabel1.Location = new System.Drawing.Point(21, 8);
            this.roundedLabel1.Name = "roundedLabel1";
            this.roundedLabel1.Size = new System.Drawing.Size(179, 40);
            this.roundedLabel1.TabIndex = 11;
            this.roundedLabel1.Text = "CI 정보 검색";
            this.roundedLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextBox_Search_SerialNo
            // 
            this.TextBox_Search_SerialNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBox_Search_SerialNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Search_SerialNo.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.TextBox_Search_SerialNo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextBox_Search_SerialNo.Location = new System.Drawing.Point(178, 116);
            this.TextBox_Search_SerialNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Search_SerialNo.Name = "TextBox_Search_SerialNo";
            this.TextBox_Search_SerialNo.Size = new System.Drawing.Size(147, 27);
            this.TextBox_Search_SerialNo.TabIndex = 3;
            this.TextBox_Search_SerialNo.Text = "일련번호 입력";
            this.TextBox_Search_SerialNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextBox_Search_SerialNo.Click += new System.EventHandler(this.TextBox_Search_SerialNo_Click);
            // 
            // CheckBox_SerialNo
            // 
            this.CheckBox_SerialNo.AutoSize = true;
            this.CheckBox_SerialNo.BackColor = System.Drawing.Color.White;
            this.CheckBox_SerialNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.CheckBox_SerialNo.Location = new System.Drawing.Point(69, 116);
            this.CheckBox_SerialNo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBox_SerialNo.Name = "CheckBox_SerialNo";
            this.CheckBox_SerialNo.Size = new System.Drawing.Size(88, 24);
            this.CheckBox_SerialNo.TabIndex = 10;
            this.CheckBox_SerialNo.Text = "일련번호";
            this.CheckBox_SerialNo.UseVisualStyleBackColor = false;
            // 
            // CheckBox_TrainNo
            // 
            this.CheckBox_TrainNo.AutoSize = true;
            this.CheckBox_TrainNo.BackColor = System.Drawing.Color.White;
            this.CheckBox_TrainNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.CheckBox_TrainNo.Location = new System.Drawing.Point(69, 83);
            this.CheckBox_TrainNo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBox_TrainNo.Name = "CheckBox_TrainNo";
            this.CheckBox_TrainNo.Size = new System.Drawing.Size(88, 24);
            this.CheckBox_TrainNo.TabIndex = 7;
            this.CheckBox_TrainNo.Text = "차량번호";
            this.CheckBox_TrainNo.UseVisualStyleBackColor = false;
            // 
            // TextBox_Search_TrainNo
            // 
            this.TextBox_Search_TrainNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Search_TrainNo.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.TextBox_Search_TrainNo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextBox_Search_TrainNo.Location = new System.Drawing.Point(178, 83);
            this.TextBox_Search_TrainNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Search_TrainNo.Name = "TextBox_Search_TrainNo";
            this.TextBox_Search_TrainNo.Size = new System.Drawing.Size(147, 27);
            this.TextBox_Search_TrainNo.TabIndex = 2;
            this.TextBox_Search_TrainNo.Text = "차량번호 입력";
            this.TextBox_Search_TrainNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextBox_Search_TrainNo.Click += new System.EventHandler(this.TextBox_Search_TrainNo_Click);
            // 
            // TextBox_Search_GroupNo
            // 
            this.TextBox_Search_GroupNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Search_GroupNo.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.TextBox_Search_GroupNo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextBox_Search_GroupNo.Location = new System.Drawing.Point(178, 51);
            this.TextBox_Search_GroupNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Search_GroupNo.Name = "TextBox_Search_GroupNo";
            this.TextBox_Search_GroupNo.Size = new System.Drawing.Size(147, 27);
            this.TextBox_Search_GroupNo.TabIndex = 1;
            this.TextBox_Search_GroupNo.Text = "편성번호 입력";
            this.TextBox_Search_GroupNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextBox_Search_GroupNo.Click += new System.EventHandler(this.TextBox_Search_GroupNo_Click);
            this.TextBox_Search_GroupNo.Leave += new System.EventHandler(this.TextBox_Search_GroupNo_Leave);
            // 
            // CheckBox_GroupNo
            // 
            this.CheckBox_GroupNo.AutoSize = true;
            this.CheckBox_GroupNo.BackColor = System.Drawing.Color.White;
            this.CheckBox_GroupNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.CheckBox_GroupNo.Location = new System.Drawing.Point(69, 52);
            this.CheckBox_GroupNo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBox_GroupNo.Name = "CheckBox_GroupNo";
            this.CheckBox_GroupNo.Size = new System.Drawing.Size(88, 24);
            this.CheckBox_GroupNo.TabIndex = 6;
            this.CheckBox_GroupNo.Text = "편성번호";
            this.CheckBox_GroupNo.UseVisualStyleBackColor = false;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.White;
            this.tabPage4.Controls.Add(this.roundedPanel4);
            this.tabPage4.Controls.Add(this.roundedPanel3);
            this.tabPage4.Location = new System.Drawing.Point(4, 44);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(535, 656);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "시험자 정보";
            // 
            // roundedPanel4
            // 
            this.roundedPanel4.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel4.Controls.Add(this.BtnTesterDelete);
            this.roundedPanel4.Controls.Add(this.BtnTesterAdd);
            this.roundedPanel4.Controls.Add(this.roundedLabel4);
            this.roundedPanel4.Controls.Add(this.TextBox_Tester);
            this.roundedPanel4.Controls.Add(this.label5);
            this.roundedPanel4.Controls.Add(this.TextBox_ID);
            this.roundedPanel4.Controls.Add(this.label1);
            this.roundedPanel4.Controls.Add(this.label13);
            this.roundedPanel4.Controls.Add(this.TextBox_Department);
            this.roundedPanel4.Location = new System.Drawing.Point(6, 465);
            this.roundedPanel4.Name = "roundedPanel4";
            this.roundedPanel4.Size = new System.Drawing.Size(529, 188);
            this.roundedPanel4.TabIndex = 1;
            // 
            // BtnTesterDelete
            // 
            this.BtnTesterDelete.AutoCenterIcon = false;
            this.BtnTesterDelete.AutoCenterText = false;
            this.BtnTesterDelete.BackColor = System.Drawing.Color.White;
            this.BtnTesterDelete.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(101)))), ((int)(((byte)(105)))));
            this.BtnTesterDelete.BaseBorderThickness = 1;
            this.BtnTesterDelete.CornerRadius = 5;
            this.BtnTesterDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnTesterDelete.FlatAppearance.BorderSize = 0;
            this.BtnTesterDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnTesterDelete.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BtnTesterDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(101)))), ((int)(((byte)(105)))));
            this.BtnTesterDelete.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnTesterDelete.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(101)))), ((int)(((byte)(105)))));
            this.BtnTesterDelete.HoverBorderThickness = 3;
            this.BtnTesterDelete.IconLocation = new System.Drawing.Point(20, 11);
            this.BtnTesterDelete.IconScale = 0.5F;
            this.BtnTesterDelete.Image = global::TCMSTester.Properties.Resources.trash_can;
            this.BtnTesterDelete.Location = new System.Drawing.Point(304, 111);
            this.BtnTesterDelete.Name = "BtnTesterDelete";
            this.BtnTesterDelete.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnTesterDelete.Size = new System.Drawing.Size(209, 44);
            this.BtnTesterDelete.TabIndex = 86;
            this.BtnTesterDelete.Text = "시험자 정보 삭제";
            this.BtnTesterDelete.TextBottomMargin = 20;
            this.BtnTesterDelete.TextLocation = new System.Drawing.Point(55, 11);
            this.BtnTesterDelete.UseHoverBackColor = false;
            this.BtnTesterDelete.UseVisualStyleBackColor = false;
            this.BtnTesterDelete.Click += new System.EventHandler(this.BtnTesterDelete_Click);
            // 
            // BtnTesterAdd
            // 
            this.BtnTesterAdd.AutoCenterIcon = false;
            this.BtnTesterAdd.AutoCenterText = false;
            this.BtnTesterAdd.BackColor = System.Drawing.Color.White;
            this.BtnTesterAdd.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.BtnTesterAdd.BaseBorderThickness = 1;
            this.BtnTesterAdd.CornerRadius = 5;
            this.BtnTesterAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnTesterAdd.FlatAppearance.BorderSize = 0;
            this.BtnTesterAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnTesterAdd.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BtnTesterAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.BtnTesterAdd.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.BtnTesterAdd.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.BtnTesterAdd.HoverBorderThickness = 3;
            this.BtnTesterAdd.IconLocation = new System.Drawing.Point(18, 9);
            this.BtnTesterAdd.IconScale = 0.6F;
            this.BtnTesterAdd.Image = global::TCMSTester.Properties.Resources.free_icon_plus_button_11527960;
            this.BtnTesterAdd.Location = new System.Drawing.Point(304, 52);
            this.BtnTesterAdd.Name = "BtnTesterAdd";
            this.BtnTesterAdd.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.BtnTesterAdd.Size = new System.Drawing.Size(209, 44);
            this.BtnTesterAdd.TabIndex = 85;
            this.BtnTesterAdd.Text = "시험자 정보 추가";
            this.BtnTesterAdd.TextBottomMargin = 20;
            this.BtnTesterAdd.TextLocation = new System.Drawing.Point(55, 11);
            this.BtnTesterAdd.UseHoverBackColor = false;
            this.BtnTesterAdd.UseVisualStyleBackColor = false;
            this.BtnTesterAdd.Click += new System.EventHandler(this.BtnTesterAdd_Click);
            // 
            // roundedLabel4
            // 
            this.roundedLabel4.AutoCenterImage = false;
            this.roundedLabel4.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel4.CustomImage = global::TCMSTester.Properties.Resources.project;
            this.roundedLabel4.FillColor = System.Drawing.Color.White;
            this.roundedLabel4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel4.ImageLocation = new System.Drawing.Point(5, 4);
            this.roundedLabel4.Location = new System.Drawing.Point(19, 10);
            this.roundedLabel4.Name = "roundedLabel4";
            this.roundedLabel4.Size = new System.Drawing.Size(211, 40);
            this.roundedLabel4.TabIndex = 84;
            this.roundedLabel4.Text = "시험자 정보 관리";
            this.roundedLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextBox_Tester
            // 
            this.TextBox_Tester.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Tester.Location = new System.Drawing.Point(99, 62);
            this.TextBox_Tester.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Tester.Name = "TextBox_Tester";
            this.TextBox_Tester.Size = new System.Drawing.Size(149, 27);
            this.TextBox_Tester.TabIndex = 1;
            this.TextBox_Tester.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(29, 131);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 20);
            this.label5.TabIndex = 81;
            this.label5.Tag = "1";
            this.label5.Text = "사원번호";
            // 
            // TextBox_ID
            // 
            this.TextBox_ID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_ID.Location = new System.Drawing.Point(99, 128);
            this.TextBox_ID.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_ID.Name = "TextBox_ID";
            this.TextBox_ID.Size = new System.Drawing.Size(149, 27);
            this.TextBox_ID.TabIndex = 3;
            this.TextBox_ID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(29, 66);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 20);
            this.label1.TabIndex = 77;
            this.label1.Tag = "1";
            this.label1.Text = "시험자명";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.White;
            this.label13.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(29, 98);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 20);
            this.label13.TabIndex = 79;
            this.label13.Tag = "1";
            this.label13.Text = "부서명";
            // 
            // TextBox_Department
            // 
            this.TextBox_Department.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Department.Location = new System.Drawing.Point(99, 96);
            this.TextBox_Department.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Department.Name = "TextBox_Department";
            this.TextBox_Department.Size = new System.Drawing.Size(149, 27);
            this.TextBox_Department.TabIndex = 2;
            this.TextBox_Department.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // roundedPanel3
            // 
            this.roundedPanel3.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel3.Controls.Add(this.dataGridView_Tester);
            this.roundedPanel3.Controls.Add(this.roundedLabel3);
            this.roundedPanel3.Location = new System.Drawing.Point(6, 9);
            this.roundedPanel3.Name = "roundedPanel3";
            this.roundedPanel3.Size = new System.Drawing.Size(529, 448);
            this.roundedPanel3.TabIndex = 0;
            // 
            // dataGridView_Tester
            // 
            this.dataGridView_Tester.AllowUserToAddRows = false;
            this.dataGridView_Tester.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridView_Tester.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Tester.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Tester.Location = new System.Drawing.Point(21, 56);
            this.dataGridView_Tester.Name = "dataGridView_Tester";
            this.dataGridView_Tester.RowTemplate.Height = 23;
            this.dataGridView_Tester.Size = new System.Drawing.Size(491, 376);
            this.dataGridView_Tester.TabIndex = 14;
            this.dataGridView_Tester.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Tester_CellClick);
            // 
            // roundedLabel3
            // 
            this.roundedLabel3.AutoCenterImage = false;
            this.roundedLabel3.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel3.CustomImage = global::TCMSTester.Properties.Resources.paper__1_;
            this.roundedLabel3.FillColor = System.Drawing.Color.White;
            this.roundedLabel3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel3.ImageLocation = new System.Drawing.Point(5, 4);
            this.roundedLabel3.Location = new System.Drawing.Point(21, 8);
            this.roundedLabel3.Name = "roundedLabel3";
            this.roundedLabel3.Size = new System.Drawing.Size(209, 40);
            this.roundedLabel3.TabIndex = 12;
            this.roundedLabel3.Text = "시험자 정보 목록";
            this.roundedLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormDB
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(570, 778);
            this.Controls.Add(this.TextBox_Password);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.flatTabControl1);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.BtnTesterPassword);
            this.Name = "FormDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "데이터베이스 관리";
            this.Load += new System.EventHandler(this.FormDB_Load);
            this.flatTabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.roundedPanel2.ResumeLayout(false);
            this.roundedPanel2.PerformLayout();
            this.roundedPanel1.ResumeLayout(false);
            this.roundedPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Serial)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.roundedPanel4.ResumeLayout(false);
            this.roundedPanel4.PerformLayout();
            this.roundedPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Tester)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBox_Password;
        private System.Windows.Forms.Button BtnTesterPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TextBox_ID;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox TextBox_Department;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_SerialNo;
        private System.Windows.Forms.TextBox TextBox_TrainNo;
        private System.Windows.Forms.TextBox TextBox_GroupNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CheckBox_SerialNo;
        private System.Windows.Forms.TextBox TextBox_Search_SerialNo;
        private System.Windows.Forms.CheckBox CheckBox_TrainNo;
        private System.Windows.Forms.CheckBox CheckBox_GroupNo;
        private System.Windows.Forms.TextBox TextBox_Search_TrainNo;
        private System.Windows.Forms.TextBox TextBox_Search_GroupNo;
        private System.Windows.Forms.TextBox TextBox_Tester;
        private RoundedPanel roundedPanel1;
        private FlatTabControl flatTabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private RoundedPanel roundedPanel2;
        private CustomIconButton BtnSearch;
        private RoundedLabel roundedLabel1;
        private RoundedPanel roundedPanel4;
        private RoundedPanel roundedPanel3;
        private CustomIconButton Button_Serial_Add;
        private CustomIconButton Button_Serial_Delete;
        private System.Windows.Forms.DataGridView dataGridView_Serial;
        private RoundedLabel roundedLabel2;
        private CustomIconButton BtnClose;
        private RoundedLabel roundedLabel3;
        private CustomIconButton BtnTesterDelete;
        private CustomIconButton BtnTesterAdd;
        private RoundedLabel roundedLabel4;
        private System.Windows.Forms.DataGridView dataGridView_Tester;
    }
}