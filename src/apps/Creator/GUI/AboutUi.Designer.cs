namespace OpenExamSuite.Creator.GUI
{
    partial class AboutUi
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
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutUi));
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.lnk_web = new LinkLabel();
            this.lnk_issues = new LinkLabel();
            this.lnk_wiki = new LinkLabel();
            this.richTextBox1 = new RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(110, 15);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(168, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Open Exam Creator 4.0.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(22, 57);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(31, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Web";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(22, 90);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(38, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Issues";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new Point(22, 127);
            this.label4.Margin = new Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(30, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Wiki";
            // 
            // lnk_web
            // 
            this.lnk_web.AutoSize = true;
            this.lnk_web.Location = new Point(75, 57);
            this.lnk_web.Margin = new Padding(4, 0, 4, 0);
            this.lnk_web.Name = "lnk_web";
            this.lnk_web.Size = new Size(295, 15);
            this.lnk_web.TabIndex = 5;
            this.lnk_web.TabStop = true;
            this.lnk_web.Text = "https://github.com/bolorundurowb/Open-Exam-Suite";
            this.lnk_web.LinkClicked += this.lnk_web_LinkClicked;
            // 
            // lnk_issues
            // 
            this.lnk_issues.AutoSize = true;
            this.lnk_issues.Location = new Point(75, 90);
            this.lnk_issues.Margin = new Padding(4, 0, 4, 0);
            this.lnk_issues.Name = "lnk_issues";
            this.lnk_issues.Size = new Size(331, 15);
            this.lnk_issues.TabIndex = 6;
            this.lnk_issues.TabStop = true;
            this.lnk_issues.Text = "https://github.com/bolorundurowb/Open-Exam-Suite/issues";
            this.lnk_issues.LinkClicked += this.lnk_issues_LinkClicked;
            // 
            // lnk_wiki
            // 
            this.lnk_wiki.AutoSize = true;
            this.lnk_wiki.Location = new Point(75, 127);
            this.lnk_wiki.Margin = new Padding(4, 0, 4, 0);
            this.lnk_wiki.Name = "lnk_wiki";
            this.lnk_wiki.Size = new Size(321, 15);
            this.lnk_wiki.TabIndex = 7;
            this.lnk_wiki.TabStop = true;
            this.lnk_wiki.Text = "https://github.com/bolorundurowb/Open-Exam-Suite/wiki";
            this.lnk_wiki.LinkClicked += this.lnk_wiki_LinkClicked;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = SystemColors.Control;
            this.richTextBox1.BorderStyle = BorderStyle.None;
            this.richTextBox1.Font = new Font("Microsoft Sans Serif", 8.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.richTextBox1.Location = new Point(26, 158);
            this.richTextBox1.Margin = new Padding(4, 3, 4, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new Size(411, 85);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // AboutUi
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.Control;
            this.ClientSize = new Size(456, 247);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.lnk_wiki);
            this.Controls.Add(this.lnk_issues);
            this.Controls.Add(this.lnk_web);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Margin = new Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutUi";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel lnk_web;
        private System.Windows.Forms.LinkLabel lnk_issues;
        private System.Windows.Forms.LinkLabel lnk_wiki;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}