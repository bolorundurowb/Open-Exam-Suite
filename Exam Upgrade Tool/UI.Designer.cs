namespace Exam_Upgrade_Tool
{
    partial class UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI));
            this.opf_old_exam = new System.Windows.Forms.OpenFileDialog();
            this.sfd_new_exam = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_old_exam = new System.Windows.Forms.TextBox();
            this.btn_browse_open = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_new_exam = new System.Windows.Forms.TextBox();
            this.btn_browse_save = new System.Windows.Forms.Button();
            this.btn_convert = new System.Windows.Forms.Button();
            this.lbl_status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // opf_old_exam
            // 
            this.opf_old_exam.Filter = "OEF (*.oef)|*.oef";
            // 
            // sfd_new_exam
            // 
            this.sfd_new_exam.Filter = "OEF (*.oef)|*.oef";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Old Exam File:";
            // 
            // txt_old_exam
            // 
            this.txt_old_exam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_old_exam.Location = new System.Drawing.Point(93, 10);
            this.txt_old_exam.Name = "txt_old_exam";
            this.txt_old_exam.ReadOnly = true;
            this.txt_old_exam.Size = new System.Drawing.Size(236, 20);
            this.txt_old_exam.TabIndex = 1;
            // 
            // btn_browse_open
            // 
            this.btn_browse_open.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_browse_open.Location = new System.Drawing.Point(335, 8);
            this.btn_browse_open.Name = "btn_browse_open";
            this.btn_browse_open.Size = new System.Drawing.Size(34, 23);
            this.btn_browse_open.TabIndex = 2;
            this.btn_browse_open.Text = ". . .";
            this.btn_browse_open.UseVisualStyleBackColor = true;
            this.btn_browse_open.Click += new System.EventHandler(this.btn_browse_open_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "New Exam File:";
            // 
            // txt_new_exam
            // 
            this.txt_new_exam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_new_exam.Location = new System.Drawing.Point(93, 47);
            this.txt_new_exam.Name = "txt_new_exam";
            this.txt_new_exam.ReadOnly = true;
            this.txt_new_exam.Size = new System.Drawing.Size(236, 20);
            this.txt_new_exam.TabIndex = 4;
            // 
            // btn_browse_save
            // 
            this.btn_browse_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_browse_save.Location = new System.Drawing.Point(335, 45);
            this.btn_browse_save.Name = "btn_browse_save";
            this.btn_browse_save.Size = new System.Drawing.Size(34, 23);
            this.btn_browse_save.TabIndex = 5;
            this.btn_browse_save.Text = ". . .";
            this.btn_browse_save.UseVisualStyleBackColor = true;
            this.btn_browse_save.Click += new System.EventHandler(this.btn_browse_save_Click);
            // 
            // btn_convert
            // 
            this.btn_convert.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_convert.Location = new System.Drawing.Point(158, 76);
            this.btn_convert.Name = "btn_convert";
            this.btn_convert.Size = new System.Drawing.Size(75, 23);
            this.btn_convert.TabIndex = 6;
            this.btn_convert.Text = "Convert";
            this.btn_convert.UseVisualStyleBackColor = true;
            this.btn_convert.Click += new System.EventHandler(this.btn_convert_Click);
            // 
            // lbl_status
            // 
            this.lbl_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_status.AutoSize = true;
            this.lbl_status.Location = new System.Drawing.Point(50, 114);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(0, 13);
            this.lbl_status.TabIndex = 7;
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 137);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.btn_convert);
            this.Controls.Add(this.btn_browse_save);
            this.Controls.Add(this.txt_new_exam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_browse_open);
            this.Controls.Add(this.txt_old_exam);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1920, 176);
            this.Name = "UI";
            this.Text = "Exam Upgrade Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog opf_old_exam;
        private System.Windows.Forms.SaveFileDialog sfd_new_exam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_old_exam;
        private System.Windows.Forms.Button btn_browse_open;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_new_exam;
        private System.Windows.Forms.Button btn_browse_save;
        private System.Windows.Forms.Button btn_convert;
        private System.Windows.Forms.Label lbl_status;
    }
}

