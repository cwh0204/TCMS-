namespace CITester
{
    partial class FormResult
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
            this.Button_Print = new System.Windows.Forms.Button();
            this.dataGridView_Search = new System.Windows.Forms.DataGridView();
            this.vscrollbarCustom = new System.Windows.Forms.VScrollBar();
            this.Btn_Close = new CustomIconButton();
            this.roundedPanel1 = new CITester.RoundedPanel();
            this.TextBox_Search_Tester = new System.Windows.Forms.TextBox();
            this.TextBox_Search_SerialNo = new System.Windows.Forms.TextBox();
            this.TextBox_Search_TrainNo = new System.Windows.Forms.TextBox();
            this.Btn_Search = new CustomIconButton();
            this.TextBox_Search_GroupNo = new System.Windows.Forms.TextBox();
            this.roundedLabel1 = new CITester.RoundedLabel();
            this.CheckBox_Period = new System.Windows.Forms.CheckBox();
            this.CheckBox_SerialNo = new System.Windows.Forms.CheckBox();
            this.DateTime_From = new System.Windows.Forms.DateTimePicker();
            this.CheckBox_TrainNo = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CheckBox_GroupNo = new System.Windows.Forms.CheckBox();
            this.DateTime_To = new System.Windows.Forms.DateTimePicker();
            this.CheckBox_Tester = new System.Windows.Forms.CheckBox();
            this.Btn_View = new CustomIconButton();
            this.Btn_Delete = new CustomIconButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Search)).BeginInit();
            this.roundedPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_Print
            // 
            this.Button_Print.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Button_Print.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.Button_Print.Location = new System.Drawing.Point(16, 679);
            this.Button_Print.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Button_Print.Name = "Button_Print";
            this.Button_Print.Size = new System.Drawing.Size(180, 40);
            this.Button_Print.TabIndex = 53;
            this.Button_Print.Text = "출 력";
            this.Button_Print.UseVisualStyleBackColor = false;
            this.Button_Print.Visible = false;
            this.Button_Print.Click += new System.EventHandler(this.Button_Print_Click);
            // 
            // dataGridView_Search
            // 
            this.dataGridView_Search.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.dataGridView_Search.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Search.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(228)))), ((int)(((byte)(231)))));
            this.dataGridView_Search.Location = new System.Drawing.Point(18, 253);
            this.dataGridView_Search.Name = "dataGridView_Search";
            this.dataGridView_Search.RowTemplate.Height = 23;
            this.dataGridView_Search.Size = new System.Drawing.Size(953, 418);
            this.dataGridView_Search.TabIndex = 54;
            this.dataGridView_Search.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Search_CellDoubleClick);
            this.dataGridView_Search.Paint += new System.Windows.Forms.PaintEventHandler(this.DataGridView_Search_Paint);
            // 
            // vscrollbarCustom
            // 
            this.vscrollbarCustom.Location = new System.Drawing.Point(958, 289);
            this.vscrollbarCustom.Name = "vscrollbarCustom";
            this.vscrollbarCustom.Size = new System.Drawing.Size(11, 376);
            this.vscrollbarCustom.TabIndex = 88;
            this.vscrollbarCustom.Visible = false;
            // 
            // Btn_Close
            // 
            this.Btn_Close.AutoCenterIcon = false;
            this.Btn_Close.AutoCenterText = false;
            this.Btn_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(110)))), ((int)(((byte)(144)))));
            this.Btn_Close.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Btn_Close.BaseBorderThickness = 0;
            this.Btn_Close.CornerRadius = 5;
            this.Btn_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Close.FlatAppearance.BorderSize = 0;
            this.Btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Close.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Btn_Close.ForeColor = System.Drawing.Color.White;
            this.Btn_Close.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(147)))), ((int)(((byte)(175)))));
            this.Btn_Close.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.Btn_Close.HoverBorderThickness = 0;
            this.Btn_Close.IconLocation = new System.Drawing.Point(20, 9);
            this.Btn_Close.IconScale = 0.55F;
            this.Btn_Close.Image = global::TCMSTester.Properties.Resources.shut_down_4063921;
            this.Btn_Close.Location = new System.Drawing.Point(825, 679);
            this.Btn_Close.Name = "Btn_Close";
            this.Btn_Close.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(71)))), ((int)(((byte)(102)))));
            this.Btn_Close.Size = new System.Drawing.Size(146, 42);
            this.Btn_Close.TabIndex = 85;
            this.Btn_Close.Text = "종 료";
            this.Btn_Close.TextBottomMargin = 12;
            this.Btn_Close.TextLocation = new System.Drawing.Point(55, 10);
            this.Btn_Close.UseHoverBackColor = true;
            this.Btn_Close.UseVisualStyleBackColor = false;
            this.Btn_Close.Click += new System.EventHandler(this.Btn_Close_Click);
            // 
            // roundedPanel1
            // 
            this.roundedPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedPanel1.Controls.Add(this.TextBox_Search_Tester);
            this.roundedPanel1.Controls.Add(this.TextBox_Search_SerialNo);
            this.roundedPanel1.Controls.Add(this.TextBox_Search_TrainNo);
            this.roundedPanel1.Controls.Add(this.Btn_Search);
            this.roundedPanel1.Controls.Add(this.TextBox_Search_GroupNo);
            this.roundedPanel1.Controls.Add(this.roundedLabel1);
            this.roundedPanel1.Controls.Add(this.CheckBox_Period);
            this.roundedPanel1.Controls.Add(this.CheckBox_SerialNo);
            this.roundedPanel1.Controls.Add(this.DateTime_From);
            this.roundedPanel1.Controls.Add(this.CheckBox_TrainNo);
            this.roundedPanel1.Controls.Add(this.label1);
            this.roundedPanel1.Controls.Add(this.CheckBox_GroupNo);
            this.roundedPanel1.Controls.Add(this.DateTime_To);
            this.roundedPanel1.Controls.Add(this.CheckBox_Tester);
            this.roundedPanel1.CornerRadius = 5;
            this.roundedPanel1.Location = new System.Drawing.Point(18, 26);
            this.roundedPanel1.Name = "roundedPanel1";
            this.roundedPanel1.Size = new System.Drawing.Size(953, 203);
            this.roundedPanel1.TabIndex = 84;
            // 
            // TextBox_Search_Tester
            // 
            this.TextBox_Search_Tester.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Search_Tester.Location = new System.Drawing.Point(135, 104);
            this.TextBox_Search_Tester.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Search_Tester.Name = "TextBox_Search_Tester";
            this.TextBox_Search_Tester.Size = new System.Drawing.Size(232, 27);
            this.TextBox_Search_Tester.TabIndex = 42;
            // 
            // TextBox_Search_SerialNo
            // 
            this.TextBox_Search_SerialNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Search_SerialNo.Location = new System.Drawing.Point(539, 149);
            this.TextBox_Search_SerialNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Search_SerialNo.Name = "TextBox_Search_SerialNo";
            this.TextBox_Search_SerialNo.Size = new System.Drawing.Size(187, 27);
            this.TextBox_Search_SerialNo.TabIndex = 36;
            // 
            // TextBox_Search_TrainNo
            // 
            this.TextBox_Search_TrainNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Search_TrainNo.Location = new System.Drawing.Point(135, 149);
            this.TextBox_Search_TrainNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Search_TrainNo.Name = "TextBox_Search_TrainNo";
            this.TextBox_Search_TrainNo.Size = new System.Drawing.Size(232, 27);
            this.TextBox_Search_TrainNo.TabIndex = 35;
            // 
            // Btn_Search
            // 
            this.Btn_Search.AutoCenterIcon = true;
            this.Btn_Search.AutoCenterText = true;
            this.Btn_Search.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(60)))), ((int)(((byte)(105)))));
            this.Btn_Search.BaseBorderColor = System.Drawing.Color.Gray;
            this.Btn_Search.BaseBorderThickness = 0;
            this.Btn_Search.CornerRadius = 20;
            this.Btn_Search.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Search.FlatAppearance.BorderSize = 0;
            this.Btn_Search.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Search.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Btn_Search.ForeColor = System.Drawing.Color.White;
            this.Btn_Search.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(85)))), ((int)(((byte)(145)))));
            this.Btn_Search.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.Btn_Search.HoverBorderThickness = 0;
            this.Btn_Search.IconLocation = new System.Drawing.Point(0, 0);
            this.Btn_Search.IconScale = 0.3F;
            this.Btn_Search.Image = global::TCMSTester.Properties.Resources.magnifying_glass;
            this.Btn_Search.Location = new System.Drawing.Point(783, 56);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(75)))));
            this.Btn_Search.Size = new System.Drawing.Size(120, 120);
            this.Btn_Search.TabIndex = 12;
            this.Btn_Search.Text = "검 색";
            this.Btn_Search.TextBottomMargin = 20;
            this.Btn_Search.TextLocation = new System.Drawing.Point(0, 0);
            this.Btn_Search.UseHoverBackColor = true;
            this.Btn_Search.UseVisualStyleBackColor = false;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // TextBox_Search_GroupNo
            // 
            this.TextBox_Search_GroupNo.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.TextBox_Search_GroupNo.Location = new System.Drawing.Point(539, 104);
            this.TextBox_Search_GroupNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TextBox_Search_GroupNo.Name = "TextBox_Search_GroupNo";
            this.TextBox_Search_GroupNo.Size = new System.Drawing.Size(187, 27);
            this.TextBox_Search_GroupNo.TabIndex = 34;
            // 
            // roundedLabel1
            // 
            this.roundedLabel1.AutoCenterImage = false;
            this.roundedLabel1.BackColor = System.Drawing.Color.Transparent;
            this.roundedLabel1.CornerRadius = 5;
            this.roundedLabel1.CustomImage = global::TCMSTester.Properties.Resources.paper__1_1;
            this.roundedLabel1.FillColor = System.Drawing.Color.White;
            this.roundedLabel1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.roundedLabel1.ImageLocation = new System.Drawing.Point(5, 4);
            this.roundedLabel1.Location = new System.Drawing.Point(21, 8);
            this.roundedLabel1.Name = "roundedLabel1";
            this.roundedLabel1.Size = new System.Drawing.Size(179, 40);
            this.roundedLabel1.TabIndex = 11;
            this.roundedLabel1.Text = "검색 조건";
            this.roundedLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CheckBox_Period
            // 
            this.CheckBox_Period.AutoSize = true;
            this.CheckBox_Period.Location = new System.Drawing.Point(25, 62);
            this.CheckBox_Period.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.CheckBox_Period.Name = "CheckBox_Period";
            this.CheckBox_Period.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_Period.TabIndex = 22;
            this.CheckBox_Period.Text = "측정기간";
            this.CheckBox_Period.UseVisualStyleBackColor = true;
            // 
            // CheckBox_SerialNo
            // 
            this.CheckBox_SerialNo.AutoSize = true;
            this.CheckBox_SerialNo.Location = new System.Drawing.Point(433, 155);
            this.CheckBox_SerialNo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBox_SerialNo.Name = "CheckBox_SerialNo";
            this.CheckBox_SerialNo.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_SerialNo.TabIndex = 39;
            this.CheckBox_SerialNo.Text = "일련번호";
            this.CheckBox_SerialNo.UseVisualStyleBackColor = true;
            // 
            // DateTime_From
            // 
            this.DateTime_From.CustomFormat = "yyyy-MM-dd";
            this.DateTime_From.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.DateTime_From.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateTime_From.Location = new System.Drawing.Point(135, 57);
            this.DateTime_From.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.DateTime_From.Name = "DateTime_From";
            this.DateTime_From.Size = new System.Drawing.Size(156, 27);
            this.DateTime_From.TabIndex = 23;
            // 
            // CheckBox_TrainNo
            // 
            this.CheckBox_TrainNo.AutoSize = true;
            this.CheckBox_TrainNo.Location = new System.Drawing.Point(25, 155);
            this.CheckBox_TrainNo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBox_TrainNo.Name = "CheckBox_TrainNo";
            this.CheckBox_TrainNo.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_TrainNo.TabIndex = 38;
            this.CheckBox_TrainNo.Text = "차량번호";
            this.CheckBox_TrainNo.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(298, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 24;
            this.label1.Text = "∼";
            // 
            // CheckBox_GroupNo
            // 
            this.CheckBox_GroupNo.AutoSize = true;
            this.CheckBox_GroupNo.Location = new System.Drawing.Point(433, 110);
            this.CheckBox_GroupNo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBox_GroupNo.Name = "CheckBox_GroupNo";
            this.CheckBox_GroupNo.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_GroupNo.TabIndex = 37;
            this.CheckBox_GroupNo.Text = "편성번호";
            this.CheckBox_GroupNo.UseVisualStyleBackColor = true;
            // 
            // DateTime_To
            // 
            this.DateTime_To.CustomFormat = "yyyy-MM-dd";
            this.DateTime_To.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.DateTime_To.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateTime_To.Location = new System.Drawing.Point(319, 57);
            this.DateTime_To.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.DateTime_To.Name = "DateTime_To";
            this.DateTime_To.Size = new System.Drawing.Size(156, 27);
            this.DateTime_To.TabIndex = 25;
            // 
            // CheckBox_Tester
            // 
            this.CheckBox_Tester.AutoSize = true;
            this.CheckBox_Tester.Location = new System.Drawing.Point(25, 110);
            this.CheckBox_Tester.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.CheckBox_Tester.Name = "CheckBox_Tester";
            this.CheckBox_Tester.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_Tester.TabIndex = 26;
            this.CheckBox_Tester.Text = "시험자명";
            this.CheckBox_Tester.UseVisualStyleBackColor = true;
            // 
            // Btn_View
            // 
            this.Btn_View.AutoCenterIcon = false;
            this.Btn_View.AutoCenterText = false;
            this.Btn_View.BackColor = System.Drawing.Color.White;
            this.Btn_View.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Btn_View.BaseBorderThickness = 1;
            this.Btn_View.CornerRadius = 5;
            this.Btn_View.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_View.FlatAppearance.BorderSize = 0;
            this.Btn_View.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_View.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Btn_View.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Btn_View.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.Btn_View.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Btn_View.HoverBorderThickness = 3;
            this.Btn_View.IconLocation = new System.Drawing.Point(17, 9);
            this.Btn_View.IconScale = 0.58F;
            this.Btn_View.Image = global::TCMSTester.Properties.Resources.view;
            this.Btn_View.Location = new System.Drawing.Point(501, 679);
            this.Btn_View.Name = "Btn_View";
            this.Btn_View.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.Btn_View.Size = new System.Drawing.Size(146, 42);
            this.Btn_View.TabIndex = 87;
            this.Btn_View.Text = "결과 보기";
            this.Btn_View.TextBottomMargin = 20;
            this.Btn_View.TextLocation = new System.Drawing.Point(47, 10);
            this.Btn_View.UseHoverBackColor = false;
            this.Btn_View.UseVisualStyleBackColor = false;
            this.Btn_View.Click += new System.EventHandler(this.Btn_View_Click);
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.AutoCenterIcon = false;
            this.Btn_Delete.AutoCenterText = false;
            this.Btn_Delete.BackColor = System.Drawing.Color.White;
            this.Btn_Delete.BaseBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Btn_Delete.BaseBorderThickness = 1;
            this.Btn_Delete.CornerRadius = 5;
            this.Btn_Delete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Delete.FlatAppearance.BorderSize = 0;
            this.Btn_Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Delete.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Btn_Delete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Btn_Delete.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.Btn_Delete.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(129)))), ((int)(((byte)(229)))));
            this.Btn_Delete.HoverBorderThickness = 3;
            this.Btn_Delete.IconLocation = new System.Drawing.Point(18, 10);
            this.Btn_Delete.IconScale = 0.5F;
            this.Btn_Delete.Image = global::TCMSTester.Properties.Resources.trash_can_blue;
            this.Btn_Delete.Location = new System.Drawing.Point(661, 679);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(38)))));
            this.Btn_Delete.Size = new System.Drawing.Size(146, 42);
            this.Btn_Delete.TabIndex = 86;
            this.Btn_Delete.Text = "결과 삭제";
            this.Btn_Delete.TextBottomMargin = 20;
            this.Btn_Delete.TextLocation = new System.Drawing.Point(47, 10);
            this.Btn_Delete.UseHoverBackColor = false;
            this.Btn_Delete.UseVisualStyleBackColor = false;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // FormResult
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(990, 731);
            this.Controls.Add(this.vscrollbarCustom);
            this.Controls.Add(this.Btn_View);
            this.Controls.Add(this.Btn_Delete);
            this.Controls.Add(this.Btn_Close);
            this.Controls.Add(this.roundedPanel1);
            this.Controls.Add(this.dataGridView_Search);
            this.Controls.Add(this.Button_Print);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormResult";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "결과 검색";
            this.Load += new System.EventHandler(this.FormResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Search)).EndInit();
            this.roundedPanel1.ResumeLayout(false);
            this.roundedPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox CheckBox_SerialNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_Search_Tester;
        private System.Windows.Forms.TextBox TextBox_Search_SerialNo;
        private System.Windows.Forms.TextBox TextBox_Search_TrainNo;
        private System.Windows.Forms.TextBox TextBox_Search_GroupNo;
        private System.Windows.Forms.CheckBox CheckBox_TrainNo;
        private System.Windows.Forms.CheckBox CheckBox_GroupNo;
        private System.Windows.Forms.CheckBox CheckBox_Tester;
        private System.Windows.Forms.DateTimePicker DateTime_To;
        private System.Windows.Forms.DateTimePicker DateTime_From;
        private System.Windows.Forms.CheckBox CheckBox_Period;
        private System.Windows.Forms.Button Button_Print;
        private RoundedPanel roundedPanel1;
        private CustomIconButton Btn_Search;
        private RoundedLabel roundedLabel1;
        private System.Windows.Forms.DataGridView dataGridView_Search;
        private CustomIconButton Btn_Close;
        private CustomIconButton Btn_Delete;
        private CustomIconButton Btn_View;
        private System.Windows.Forms.VScrollBar vscrollbarCustom;
    }
}