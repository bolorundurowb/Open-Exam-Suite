namespace OpenExamSuite.Creator.GUI.Dialogs
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
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintOptions));
            this.groupBox1 = new GroupBox();
            this.rdb_all_questions = new RadioButton();
            this.rdb_current_section = new RadioButton();
            this.rdb_current_question = new RadioButton();
            this.btn_ok = new Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdb_all_questions);
            this.groupBox1.Controls.Add(this.rdb_current_section);
            this.groupBox1.Controls.Add(this.rdb_current_question);
            this.groupBox1.Location = new Point(14, 14);
            this.groupBox1.Margin = new Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new Padding(4, 3, 4, 3);
            this.groupBox1.Size = new Size(330, 112);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What do you want to print?";
            // 
            // rdb_all_questions
            // 
            this.rdb_all_questions.AutoSize = true;
            this.rdb_all_questions.Location = new Point(20, 80);
            this.rdb_all_questions.Margin = new Padding(4, 3, 4, 3);
            this.rdb_all_questions.Name = "rdb_all_questions";
            this.rdb_all_questions.Size = new Size(95, 19);
            this.rdb_all_questions.TabIndex = 3;
            this.rdb_all_questions.TabStop = true;
            this.rdb_all_questions.Text = "All Questions";
            this.rdb_all_questions.UseVisualStyleBackColor = true;
            // 
            // rdb_current_section
            // 
            this.rdb_current_section.AutoSize = true;
            this.rdb_current_section.Location = new Point(20, 53);
            this.rdb_current_section.Margin = new Padding(4, 3, 4, 3);
            this.rdb_current_section.Name = "rdb_current_section";
            this.rdb_current_section.Size = new Size(107, 19);
            this.rdb_current_section.TabIndex = 2;
            this.rdb_current_section.TabStop = true;
            this.rdb_current_section.Text = "Current Section";
            this.rdb_current_section.UseVisualStyleBackColor = true;
            // 
            // rdb_current_question
            // 
            this.rdb_current_question.AutoSize = true;
            this.rdb_current_question.Location = new Point(20, 27);
            this.rdb_current_question.Margin = new Padding(4, 3, 4, 3);
            this.rdb_current_question.Name = "rdb_current_question";
            this.rdb_current_question.Size = new Size(116, 19);
            this.rdb_current_question.TabIndex = 1;
            this.rdb_current_question.TabStop = true;
            this.rdb_current_question.Text = "Current Question";
            this.rdb_current_question.UseVisualStyleBackColor = true;
            // 
            // btn_ok
            // 
            this.btn_ok.FlatStyle = FlatStyle.Flat;
            this.btn_ok.Image = (Image)resources.GetObject("btn_ok.Image");
            this.btn_ok.Location = new Point(277, 133);
            this.btn_ok.Margin = new Padding(4, 3, 4, 3);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new Size(68, 27);
            this.btn_ok.TabIndex = 1;
            this.btn_ok.Text = "Print";
            this.btn_ok.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += this.btn_ok_Click;
            // 
            // PrintOptions
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(349, 164);
            this.ControlBox = false;
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Margin = new Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
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