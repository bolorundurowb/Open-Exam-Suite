namespace Simulator
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lnk_web = new System.Windows.Forms.LinkLabel();
            this.lnk_issues = new System.Windows.Forms.LinkLabel();
            this.lnk_wiki = new System.Windows.Forms.LinkLabel();
            this.lnk_email = new System.Windows.Forms.LinkLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(94, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Open Exam Simulator 1.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Web";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Issues";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Wiki";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "eMail";
            // 
            // lnk_web
            // 
            this.lnk_web.AutoSize = true;
            this.lnk_web.Location = new System.Drawing.Point(64, 49);
            this.lnk_web.Name = "lnk_web";
            this.lnk_web.Size = new System.Drawing.Size(257, 13);
            this.lnk_web.TabIndex = 5;
            this.lnk_web.TabStop = true;
            this.lnk_web.Text = "https://github.com/bolorundurowb/Open-Exam-Suite";
            this.lnk_web.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_web_LinkClicked);
            // 
            // lnk_issues
            // 
            this.lnk_issues.AutoSize = true;
            this.lnk_issues.Location = new System.Drawing.Point(64, 78);
            this.lnk_issues.Name = "lnk_issues";
            this.lnk_issues.Size = new System.Drawing.Size(291, 13);
            this.lnk_issues.TabIndex = 6;
            this.lnk_issues.TabStop = true;
            this.lnk_issues.Text = "https://github.com/bolorundurowb/Open-Exam-Suite/issues";
            this.lnk_issues.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_issues_LinkClicked);
            // 
            // lnk_wiki
            // 
            this.lnk_wiki.AutoSize = true;
            this.lnk_wiki.Location = new System.Drawing.Point(64, 110);
            this.lnk_wiki.Name = "lnk_wiki";
            this.lnk_wiki.Size = new System.Drawing.Size(280, 13);
            this.lnk_wiki.TabIndex = 7;
            this.lnk_wiki.TabStop = true;
            this.lnk_wiki.Text = "https://github.com/bolorundurowb/Open-Exam-Suite/wiki";
            this.lnk_wiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_wiki_LinkClicked);
            // 
            // lnk_email
            // 
            this.lnk_email.AutoSize = true;
            this.lnk_email.Location = new System.Drawing.Point(64, 143);
            this.lnk_email.Name = "lnk_email";
            this.lnk_email.Size = new System.Drawing.Size(146, 13);
            this.lnk_email.TabIndex = 8;
            this.lnk_email.TabStop = true;
            this.lnk_email.Text = "bolorundurowb@outlook.com";
            this.lnk_email.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_email_LinkClicked);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(22, 176);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(352, 74);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(407, 269);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.lnk_email);
            this.Controls.Add(this.lnk_wiki);
            this.Controls.Add(this.lnk_issues);
            this.Controls.Add(this.lnk_web);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel lnk_web;
        private System.Windows.Forms.LinkLabel lnk_issues;
        private System.Windows.Forms.LinkLabel lnk_wiki;
        private System.Windows.Forms.LinkLabel lnk_email;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}