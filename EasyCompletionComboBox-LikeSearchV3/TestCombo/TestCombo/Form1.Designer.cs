namespace TestCombo
{
    partial class Form1
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
            this.m_label = new System.Windows.Forms.Label();
            this.m_methodCB = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxICDChinh = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.m_combo = new UMCComboBox.EasyUMCComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // m_label
            // 
            this.m_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_label.Location = new System.Drawing.Point(12, 516);
            this.m_label.Name = "m_label";
            this.m_label.Size = new System.Drawing.Size(732, 13);
            this.m_label.TabIndex = 2;
            this.m_label.Text = "Selection:";
            this.m_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_methodCB
            // 
            this.m_methodCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_methodCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_methodCB.FormattingEnabled = true;
            this.m_methodCB.Location = new System.Drawing.Point(110, 140);
            this.m_methodCB.Name = "m_methodCB";
            this.m_methodCB.Size = new System.Drawing.Size(634, 21);
            this.m_methodCB.TabIndex = 1;
            this.m_methodCB.SelectionChangeCommitted += new System.EventHandler(this.onMethodChanged);
            this.m_methodCB.SelectedIndexChanged += new System.EventHandler(this.m_methodCB_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Matching method:";
            // 
            // textBoxICDChinh
            // 
            this.textBoxICDChinh.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxICDChinh.Location = new System.Drawing.Point(15, 100);
            this.textBoxICDChinh.Name = "textBoxICDChinh";
            this.textBoxICDChinh.Size = new System.Drawing.Size(60, 20);
            this.textBoxICDChinh.TabIndex = 0;
            this.textBoxICDChinh.TextChanged += new System.EventHandler(this.textBoxICDChinh_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(110, 226);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(110, 290);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 5;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(33, 170);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(71, 20);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(110, 197);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.Visible = false;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "fdf",
            "sdfs",
            "f",
            "sd"});
            this.listBox1.Location = new System.Drawing.Point(273, 268);
            this.listBox1.MultiColumn = true;
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(320, 121);
            this.listBox1.TabIndex = 6;
            // 
            // m_combo
            // 
            this.m_combo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_combo.ColWidthsString = "400;50;50";
            this.m_combo.DisplayColumns = "ICDdescript;ICDcode;ICDid";
            this.m_combo.FormatColumns = "string;string;number";
            this.m_combo.FormattingEnabled = true;
            this.m_combo.ItemHeight = 13;
            this.m_combo.Location = new System.Drawing.Point(110, 170);
            this.m_combo.MatchingMethod = UMCComboBox.StringMatchingMethod.UseWildcardsStartsWith;
            this.m_combo.MinLengthTextSearch = 4F;
            this.m_combo.Name = "m_combo";
            this.m_combo.RowHeight = 25F;
            this.m_combo.SearchAllColumns = false;
            this.m_combo.SearchColumnName = "ICDdescript";
            this.m_combo.Size = new System.Drawing.Size(483, 21);
            this.m_combo.TabIndex = 3;
            this.m_combo.SelectedIndexChanged += new System.EventHandler(this.onSelectionChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "trần ngọc quốc",
            "nguyễn văn a",
            "lý thị bẹn",
            "trần quang a",
            "trần thiện thanh"});
            this.comboBox1.Location = new System.Drawing.Point(227, 64);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(250, 21);
            this.comboBox1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 538);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBoxICDChinh);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_methodCB);
            this.Controls.Add(this.m_combo);
            this.Controls.Add(this.m_label);
            this.Name = "Form1";
            this.Text = "EasyCompletionComboBox Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_label;
        private UMCComboBox.EasyUMCComboBox m_combo;
        private System.Windows.Forms.ComboBox m_methodCB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxICDChinh;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

