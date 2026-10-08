namespace OpenExamSuite.Simulator.GUI
{
    partial class ExamSettingsUi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExamSettingsUi));
            this.label1 = new Label();
            this.txt_candidate_name = new TextBox();
            this.groupBox1 = new GroupBox();
            this.clb_section_options = new CheckedListBox();
            this.num_questions = new NumericUpDown();
            this.label4 = new Label();
            this.btn_deselect_all = new Button();
            this.btn_select_all = new Button();
            this.rdb_fixed_number_questions = new RadioButton();
            this.rdb_selected_sections = new RadioButton();
            this.groupBox2 = new GroupBox();
            this.num_time_limit = new NumericUpDown();
            this.label3 = new Label();
            this.label2 = new Label();
            this.chk_enable_timer = new CheckBox();
            this.btn_ok = new Button();
            this.btn_cancel = new Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.num_questions).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.num_time_limit).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(15, 15);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(99, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Candidate Name:";
            // 
            // txt_candidate_name
            // 
            this.txt_candidate_name.Location = new Point(126, 12);
            this.txt_candidate_name.Margin = new Padding(4, 3, 4, 3);
            this.txt_candidate_name.Name = "txt_candidate_name";
            this.txt_candidate_name.Size = new Size(254, 23);
            this.txt_candidate_name.TabIndex = 1;
            this.txt_candidate_name.Text = "Candidate Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clb_section_options);
            this.groupBox1.Controls.Add(this.num_questions);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btn_deselect_all);
            this.groupBox1.Controls.Add(this.btn_select_all);
            this.groupBox1.Controls.Add(this.rdb_fixed_number_questions);
            this.groupBox1.Controls.Add(this.rdb_selected_sections);
            this.groupBox1.Location = new Point(15, 48);
            this.groupBox1.Margin = new Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new Padding(4, 3, 4, 3);
            this.groupBox1.Size = new Size(601, 347);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exam";
            // 
            // clb_section_options
            // 
            this.clb_section_options.BackColor = SystemColors.Control;
            this.clb_section_options.BorderStyle = BorderStyle.FixedSingle;
            this.clb_section_options.CheckOnClick = true;
            this.clb_section_options.FormattingEnabled = true;
            this.clb_section_options.Location = new Point(44, 59);
            this.clb_section_options.Margin = new Padding(4, 3, 4, 3);
            this.clb_section_options.Name = "clb_section_options";
            this.clb_section_options.Size = new Size(527, 182);
            this.clb_section_options.TabIndex = 7;
            // 
            // num_questions
            // 
            this.num_questions.Enabled = false;
            this.num_questions.Location = new Point(86, 306);
            this.num_questions.Margin = new Padding(4, 3, 4, 3);
            this.num_questions.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.num_questions.Name = "num_questions";
            this.num_questions.Size = new Size(62, 23);
            this.num_questions.TabIndex = 6;
            this.num_questions.TextAlign = HorizontalAlignment.Right;
            this.num_questions.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new Point(160, 309);
            this.label4.Margin = new Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(170, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "questions from entire exam file";
            // 
            // btn_deselect_all
            // 
            this.btn_deselect_all.Location = new Point(140, 265);
            this.btn_deselect_all.Margin = new Padding(4, 3, 4, 3);
            this.btn_deselect_all.Name = "btn_deselect_all";
            this.btn_deselect_all.Size = new Size(88, 27);
            this.btn_deselect_all.TabIndex = 4;
            this.btn_deselect_all.Text = "Deselect All";
            this.btn_deselect_all.UseVisualStyleBackColor = true;
            this.btn_deselect_all.Click += this.DeselectAll;
            // 
            // btn_select_all
            // 
            this.btn_select_all.Location = new Point(44, 265);
            this.btn_select_all.Margin = new Padding(4, 3, 4, 3);
            this.btn_select_all.Name = "btn_select_all";
            this.btn_select_all.Size = new Size(88, 27);
            this.btn_select_all.TabIndex = 3;
            this.btn_select_all.Text = "Select All";
            this.btn_select_all.UseVisualStyleBackColor = true;
            this.btn_select_all.Click += this.SelectAll;
            // 
            // rdb_fixed_number_questions
            // 
            this.rdb_fixed_number_questions.AutoSize = true;
            this.rdb_fixed_number_questions.Location = new Point(24, 308);
            this.rdb_fixed_number_questions.Margin = new Padding(4, 3, 4, 3);
            this.rdb_fixed_number_questions.Name = "rdb_fixed_number_questions";
            this.rdb_fixed_number_questions.Size = new Size(49, 19);
            this.rdb_fixed_number_questions.TabIndex = 1;
            this.rdb_fixed_number_questions.Text = "Take";
            this.rdb_fixed_number_questions.UseVisualStyleBackColor = true;
            this.rdb_fixed_number_questions.CheckedChanged += this.ChooseNumOfQuestions;
            // 
            // rdb_selected_sections
            // 
            this.rdb_selected_sections.AutoSize = true;
            this.rdb_selected_sections.Checked = true;
            this.rdb_selected_sections.Location = new Point(24, 32);
            this.rdb_selected_sections.Margin = new Padding(4, 3, 4, 3);
            this.rdb_selected_sections.Name = "rdb_selected_sections";
            this.rdb_selected_sections.Size = new Size(253, 19);
            this.rdb_selected_sections.TabIndex = 0;
            this.rdb_selected_sections.TabStop = true;
            this.rdb_selected_sections.Text = "Take questions from selected sections only:";
            this.rdb_selected_sections.UseVisualStyleBackColor = true;
            this.rdb_selected_sections.CheckedChanged += this.ChooseSections;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.num_time_limit);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.chk_enable_timer);
            this.groupBox2.Location = new Point(15, 404);
            this.groupBox2.Margin = new Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new Padding(4, 3, 4, 3);
            this.groupBox2.Size = new Size(601, 59);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Timer";
            // 
            // num_time_limit
            // 
            this.num_time_limit.Enabled = false;
            this.num_time_limit.Location = new Point(268, 20);
            this.num_time_limit.Margin = new Padding(4, 3, 4, 3);
            this.num_time_limit.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.num_time_limit.Name = "num_time_limit";
            this.num_time_limit.Size = new Size(62, 23);
            this.num_time_limit.TabIndex = 3;
            this.num_time_limit.TextAlign = HorizontalAlignment.Right;
            this.num_time_limit.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(337, 23);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(58, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "minute(s)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(203, 23);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(61, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Time limit";
            // 
            // chk_enable_timer
            // 
            this.chk_enable_timer.AutoSize = true;
            this.chk_enable_timer.Location = new Point(20, 23);
            this.chk_enable_timer.Margin = new Padding(4, 3, 4, 3);
            this.chk_enable_timer.Name = "chk_enable_timer";
            this.chk_enable_timer.Size = new Size(132, 19);
            this.chk_enable_timer.TabIndex = 0;
            this.chk_enable_timer.Text = "Set exam time limit?";
            this.chk_enable_timer.UseVisualStyleBackColor = true;
            this.chk_enable_timer.CheckedChanged += this.CustomTimer;
            // 
            // btn_ok
            // 
            this.btn_ok.Image = (Image)resources.GetObject("btn_ok.Image");
            this.btn_ok.Location = new Point(468, 475);
            this.btn_ok.Margin = new Padding(4, 3, 4, 3);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new Size(68, 27);
            this.btn_ok.TabIndex = 4;
            this.btn_ok.Text = "Ok";
            this.btn_ok.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += this.Proceed;
            // 
            // btn_cancel
            // 
            this.btn_cancel.Image = (Image)resources.GetObject("btn_cancel.Image");
            this.btn_cancel.Location = new Point(542, 475);
            this.btn_cancel.Margin = new Padding(4, 3, 4, 3);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new Size(74, 27);
            this.btn_cancel.TabIndex = 5;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += this.Close;
            // 
            // ExamSettingsUi
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(630, 518);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_candidate_name);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.Margin = new Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExamSettingsUi";
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.num_questions).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.num_time_limit).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_candidate_name;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_deselect_all;
        private System.Windows.Forms.Button btn_select_all;
        private System.Windows.Forms.RadioButton rdb_fixed_number_questions;
        private System.Windows.Forms.RadioButton rdb_selected_sections;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chk_enable_timer;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num_questions;
        private System.Windows.Forms.NumericUpDown num_time_limit;
        private System.Windows.Forms.CheckedListBox clb_section_options;
    }
}