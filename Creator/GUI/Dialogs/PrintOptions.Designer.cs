namespace Creator.Dialogs
{
    partial class PrintOptions
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdb_all_questions = new System.Windows.Forms.RadioButton();
            this.rdb_current_section = new System.Windows.Forms.RadioButton();
            this.rdb_current_question = new System.Windows.Forms.RadioButton();
            this.btn_ok = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdb_all_questions);
            this.groupBox1.Controls.Add(this.rdb_current_section);
            this.groupBox1.Controls.Add(this.rdb_current_question);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(283, 97);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What do you want to print?";
            // 
            // rdb_all_questions
            // 
            this.rdb_all_questions.AutoSize = true;
            this.rdb_all_questions.Location = new System.Drawing.Point(17, 69);
            this.rdb_all_questions.Name = "rdb_all_questions";
            this.rdb_all_questions.Size = new System.Drawing.Size(86, 17);
            this.rdb_all_questions.TabIndex = 3;
            this.rdb_all_questions.TabStop = true;
            this.rdb_all_questions.Text = "All Questions";
            this.rdb_all_questions.UseVisualStyleBackColor = true;
            // 
            // rdb_current_section
            // 
            this.rdb_current_section.AutoSize = true;
            this.rdb_current_section.Location = new System.Drawing.Point(17, 46);
            this.rdb_current_section.Name = "rdb_current_section";
            this.rdb_current_section.Size = new System.Drawing.Size(98, 17);
            this.rdb_current_section.TabIndex = 2;
            this.rdb_current_section.TabStop = true;
            this.rdb_current_section.Text = "Current Section";
            this.rdb_current_section.UseVisualStyleBackColor = true;
            // 
            // rdb_current_question
            // 
            this.rdb_current_question.AutoSize = true;
            this.rdb_current_question.Location = new System.Drawing.Point(17, 23);
            this.rdb_current_question.Name = "rdb_current_question";
            this.rdb_current_question.Size = new System.Drawing.Size(104, 17);
            this.rdb_current_question.TabIndex = 1;
            this.rdb_current_question.TabStop = true;
            this.rdb_current_question.Text = "Current Question";
            this.rdb_current_question.UseVisualStyleBackColor = true;
            // 
            // btn_ok
            // 
            this.btn_ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ok.Image = global::Creator.Properties.Resources.ok;
            this.btn_ok.Location = new System.Drawing.Point(220, 115);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 1;
            this.btn_ok.Text = "OK";
            this.btn_ok.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // PrintOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 146);
            this.ControlBox = false;
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Print Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdb_all_questions;
        private System.Windows.Forms.RadioButton rdb_current_section;
        private System.Windows.Forms.RadioButton rdb_current_question;
        private System.Windows.Forms.Button btn_ok;
    }
}