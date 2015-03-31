namespace Creator
{
    partial class New_Section
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_section_title = new System.Windows.Forms.TextBox();
            this.btn_add = new System.Windows.Forms.Button();
            this.err_new_section = new System.Windows.Forms.ErrorProvider(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.err_new_section)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter Title:";
            // 
            // txt_section_title
            // 
            this.txt_section_title.Location = new System.Drawing.Point(76, 19);
            this.txt_section_title.Name = "txt_section_title";
            this.txt_section_title.Size = new System.Drawing.Size(212, 20);
            this.txt_section_title.TabIndex = 1;
            this.txt_section_title.TextChanged += new System.EventHandler(this.txt_section_title_TextChanged);
            // 
            // btn_add
            // 
            this.btn_add.Enabled = false;
            this.btn_add.Image = global::Creator.Properties.Resources._1;
            this.btn_add.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_add.Location = new System.Drawing.Point(321, 16);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(58, 23);
            this.btn_add.TabIndex = 2;
            this.btn_add.Text = "ADD";
            this.btn_add.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // err_new_section
            // 
            this.err_new_section.ContainerControl = this;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox1.ForeColor = System.Drawing.Color.Red;
            this.richTextBox1.Location = new System.Drawing.Point(12, 48);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(381, 45);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "NOTE: Once done adding sections, please close this window. Added sections would b" +
    "e automatically reflected.";
            // 
            // New_Section
            // 
            this.AcceptButton = this.btn_add;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 105);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.txt_section_title);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "New_Section";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add New Section";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.New_Section_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.err_new_section)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_section_title;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.ErrorProvider err_new_section;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}