namespace CITester
{
    partial class FormTester
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTester));
            this.label3 = new System.Windows.Forms.Label();
            this.TextBox_ID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_Department = new System.Windows.Forms.TextBox();
            this.TextBox_Tester = new System.Windows.Forms.TextBox();
            this.BtnClose = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BtnOK = new System.Windows.Forms.Button();
            this.ListView_Tester = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // TextBox_ID
            // 
            resources.ApplyResources(this.TextBox_ID, "TextBox_ID");
            this.TextBox_ID.Name = "TextBox_ID";
            this.TextBox_ID.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TextBox_Department
            // 
            resources.ApplyResources(this.TextBox_Department, "TextBox_Department");
            this.TextBox_Department.Name = "TextBox_Department";
            this.TextBox_Department.ReadOnly = true;
            // 
            // TextBox_Tester
            // 
            resources.ApplyResources(this.TextBox_Tester, "TextBox_Tester");
            this.TextBox_Tester.Name = "TextBox_Tester";
            this.TextBox_Tester.ReadOnly = true;
            // 
            // BtnClose
            // 
            resources.ApplyResources(this.BtnClose, "BtnClose");
            this.BtnClose.BackColor = System.Drawing.Color.PaleVioletRed;
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // BtnOK
            // 
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = false;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // ListView_Tester
            // 
            resources.ApplyResources(this.ListView_Tester, "ListView_Tester");
            this.ListView_Tester.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.ListView_Tester.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
            this.ListView_Tester.FullRowSelect = true;
            this.ListView_Tester.GridLines = true;
            this.ListView_Tester.HideSelection = false;
            this.ListView_Tester.Name = "ListView_Tester";
            this.ListView_Tester.UseCompatibleStateImageBehavior = false;
            this.ListView_Tester.View = System.Windows.Forms.View.Details;
            this.ListView_Tester.SelectedIndexChanged += new System.EventHandler(this.ListView_Tester_SelectedIndexChanged);
            // 
            // FormTester
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextBox_ID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_Department);
            this.Controls.Add(this.TextBox_Tester);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.ListView_Tester);
            this.Name = "FormTester";
            this.Load += new System.EventHandler(this.FormTester_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBox_ID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_Department;
        private System.Windows.Forms.TextBox TextBox_Tester;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.ListView ListView_Tester;
    }
}