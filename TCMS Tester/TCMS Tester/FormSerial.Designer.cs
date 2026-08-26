namespace CITester
{
    partial class FormSerial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSerial));
            this.BtnClose = new System.Windows.Forms.Button();
            this.TextBox_GroupNo = new System.Windows.Forms.TextBox();
            this.BtnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.TextBox_Search_SerialNo = new System.Windows.Forms.TextBox();
            this.TextBox_Search_TrainNo = new System.Windows.Forms.TextBox();
            this.TextBox_Search_GroupNo = new System.Windows.Forms.TextBox();
            this.CheckBox_SerialNo = new System.Windows.Forms.CheckBox();
            this.CheckBox_TrainNo = new System.Windows.Forms.CheckBox();
            this.ListView_Serial = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TextBox_SerialNo = new System.Windows.Forms.TextBox();
            this.TextBox_TrainNo = new System.Windows.Forms.TextBox();
            this.CheckBox_GroupNo = new System.Windows.Forms.CheckBox();
            this.BtnSearch = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnClose
            // 
            resources.ApplyResources(this.BtnClose, "BtnClose");
            this.BtnClose.BackColor = System.Drawing.Color.PaleVioletRed;
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // TextBox_GroupNo
            // 
            resources.ApplyResources(this.TextBox_GroupNo, "TextBox_GroupNo");
            this.TextBox_GroupNo.Name = "TextBox_GroupNo";
            this.TextBox_GroupNo.ReadOnly = true;
            // 
            // BtnOK
            // 
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = false;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Name = "label2";
            this.label2.Tag = "1";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.ForeColor = System.Drawing.Color.Black;
            this.label18.Name = "label18";
            this.label18.Tag = "1";
            // 
            // TextBox_Search_SerialNo
            // 
            resources.ApplyResources(this.TextBox_Search_SerialNo, "TextBox_Search_SerialNo");
            this.TextBox_Search_SerialNo.Name = "TextBox_Search_SerialNo";
            // 
            // TextBox_Search_TrainNo
            // 
            resources.ApplyResources(this.TextBox_Search_TrainNo, "TextBox_Search_TrainNo");
            this.TextBox_Search_TrainNo.Name = "TextBox_Search_TrainNo";
            // 
            // TextBox_Search_GroupNo
            // 
            resources.ApplyResources(this.TextBox_Search_GroupNo, "TextBox_Search_GroupNo");
            this.TextBox_Search_GroupNo.Name = "TextBox_Search_GroupNo";
            // 
            // CheckBox_SerialNo
            // 
            resources.ApplyResources(this.CheckBox_SerialNo, "CheckBox_SerialNo");
            this.CheckBox_SerialNo.Name = "CheckBox_SerialNo";
            this.CheckBox_SerialNo.UseVisualStyleBackColor = true;
            // 
            // CheckBox_TrainNo
            // 
            resources.ApplyResources(this.CheckBox_TrainNo, "CheckBox_TrainNo");
            this.CheckBox_TrainNo.Name = "CheckBox_TrainNo";
            this.CheckBox_TrainNo.UseVisualStyleBackColor = true;
            // 
            // ListView_Serial
            // 
            resources.ApplyResources(this.ListView_Serial, "ListView_Serial");
            this.ListView_Serial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.ListView_Serial.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3});
            this.ListView_Serial.FullRowSelect = true;
            this.ListView_Serial.GridLines = true;
            this.ListView_Serial.HideSelection = false;
            this.ListView_Serial.Name = "ListView_Serial";
            this.ListView_Serial.UseCompatibleStateImageBehavior = false;
            this.ListView_Serial.View = System.Windows.Forms.View.Details;
            this.ListView_Serial.SelectedIndexChanged += new System.EventHandler(this.ListView_Serial_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // TextBox_SerialNo
            // 
            resources.ApplyResources(this.TextBox_SerialNo, "TextBox_SerialNo");
            this.TextBox_SerialNo.Name = "TextBox_SerialNo";
            this.TextBox_SerialNo.ReadOnly = true;
            // 
            // TextBox_TrainNo
            // 
            resources.ApplyResources(this.TextBox_TrainNo, "TextBox_TrainNo");
            this.TextBox_TrainNo.Name = "TextBox_TrainNo";
            this.TextBox_TrainNo.ReadOnly = true;
            // 
            // CheckBox_GroupNo
            // 
            resources.ApplyResources(this.CheckBox_GroupNo, "CheckBox_GroupNo");
            this.CheckBox_GroupNo.Name = "CheckBox_GroupNo";
            this.CheckBox_GroupNo.UseVisualStyleBackColor = true;
            // 
            // BtnSearch
            // 
            resources.ApplyResources(this.BtnSearch, "BtnSearch");
            this.BtnSearch.BackColor = System.Drawing.Color.DarkKhaki;
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.UseVisualStyleBackColor = false;
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.TextBox_Search_SerialNo);
            this.groupBox1.Controls.Add(this.TextBox_Search_TrainNo);
            this.groupBox1.Controls.Add(this.TextBox_Search_GroupNo);
            this.groupBox1.Controls.Add(this.CheckBox_SerialNo);
            this.groupBox1.Controls.Add(this.CheckBox_TrainNo);
            this.groupBox1.Controls.Add(this.CheckBox_GroupNo);
            this.groupBox1.Controls.Add(this.BtnSearch);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Name = "label17";
            this.label17.Tag = "1";
            // 
            // FormSerial
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.TextBox_GroupNo);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.ListView_Serial);
            this.Controls.Add(this.TextBox_SerialNo);
            this.Controls.Add(this.TextBox_TrainNo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label17);
            this.Name = "FormSerial";
            this.Load += new System.EventHandler(this.FormSerial_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.TextBox TextBox_GroupNo;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox TextBox_Search_SerialNo;
        private System.Windows.Forms.TextBox TextBox_Search_TrainNo;
        private System.Windows.Forms.TextBox TextBox_Search_GroupNo;
        private System.Windows.Forms.CheckBox CheckBox_SerialNo;
        private System.Windows.Forms.CheckBox CheckBox_TrainNo;
        private System.Windows.Forms.ListView ListView_Serial;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox TextBox_SerialNo;
        private System.Windows.Forms.TextBox TextBox_TrainNo;
        private System.Windows.Forms.CheckBox CheckBox_GroupNo;
        private System.Windows.Forms.Button BtnSearch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label17;
    }
}