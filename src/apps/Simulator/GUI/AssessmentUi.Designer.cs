namespace OpenExamSuite.Simulator.GUI
{
    partial class AssessmentUi
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssessmentUi));
            this.label1 = new Label();
            this.lbl_elapsed_time = new Label();
            this.btn_begin = new Button();
            this.btn_previous = new Button();
            this.btn_next = new Button();
            this.btn_pause = new Button();
            this.btn_end = new Button();
            this.pan_display = new Panel();
            this.pct_image = new PictureBox();
            this.lbl_explanation = new TextBox();
            this.txt_question = new TextBox();
            this.lbl_question_number = new Label();
            this.label3 = new Label();
            this.lbl_section_title = new Label();
            this.label2 = new Label();
            this.lbl_exam_code = new Label();
            this.lbl_exam_instructions = new Label();
            this.lbl_exam_title = new Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btn_show_answer = new Button();
            this.dspExamProgress = new Label();
            this.lblExamProgress = new Label();
            this.pan_display.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pct_image).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(1210, 10);
            this.label1.Margin = new Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(60, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Time Left:";
            this.label1.Visible = false;
            // 
            // lbl_elapsed_time
            // 
            this.lbl_elapsed_time.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.lbl_elapsed_time.AutoSize = true;
            this.lbl_elapsed_time.Location = new Point(1279, 10);
            this.lbl_elapsed_time.Margin = new Padding(2, 0, 2, 0);
            this.lbl_elapsed_time.Name = "lbl_elapsed_time";
            this.lbl_elapsed_time.Size = new Size(0, 15);
            this.lbl_elapsed_time.TabIndex = 1;
            this.lbl_elapsed_time.Visible = false;
            // 
            // btn_begin
            // 
            this.btn_begin.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btn_begin.Location = new Point(1062, 614);
            this.btn_begin.Margin = new Padding(2, 3, 2, 3);
            this.btn_begin.Name = "btn_begin";
            this.btn_begin.Size = new Size(86, 27);
            this.btn_begin.TabIndex = 2;
            this.btn_begin.Text = "Begin";
            this.btn_begin.UseVisualStyleBackColor = true;
            this.btn_begin.Click += this.Begin;
            // 
            // btn_previous
            // 
            this.btn_previous.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btn_previous.Enabled = false;
            this.btn_previous.Location = new Point(37, 614);
            this.btn_previous.Margin = new Padding(2, 3, 2, 3);
            this.btn_previous.Name = "btn_previous";
            this.btn_previous.Size = new Size(86, 27);
            this.btn_previous.TabIndex = 3;
            this.btn_previous.Text = "Previous";
            this.btn_previous.UseVisualStyleBackColor = true;
            this.btn_previous.Visible = false;
            this.btn_previous.Click += this.Previous;
            // 
            // btn_next
            // 
            this.btn_next.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btn_next.Enabled = false;
            this.btn_next.Location = new Point(128, 614);
            this.btn_next.Margin = new Padding(2, 3, 2, 3);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new Size(86, 27);
            this.btn_next.TabIndex = 4;
            this.btn_next.Text = "Next";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Visible = false;
            this.btn_next.Click += this.Next;
            // 
            // btn_pause
            // 
            this.btn_pause.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btn_pause.Location = new Point(1155, 614);
            this.btn_pause.Margin = new Padding(2, 3, 2, 3);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new Size(86, 27);
            this.btn_pause.TabIndex = 5;
            this.btn_pause.Text = "Pause";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Visible = false;
            this.btn_pause.Click += this.PauseExam;
            // 
            // btn_end
            // 
            this.btn_end.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btn_end.Location = new Point(1246, 614);
            this.btn_end.Margin = new Padding(2, 3, 2, 3);
            this.btn_end.Name = "btn_end";
            this.btn_end.Size = new Size(86, 27);
            this.btn_end.TabIndex = 6;
            this.btn_end.Text = "End";
            this.btn_end.UseVisualStyleBackColor = true;
            this.btn_end.Visible = false;
            this.btn_end.Click += this.End;
            // 
            // pan_display
            // 
            this.pan_display.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.pan_display.AutoScroll = true;
            this.pan_display.AutoSize = true;
            this.pan_display.BorderStyle = BorderStyle.FixedSingle;
            this.pan_display.Controls.Add(this.pct_image);
            this.pan_display.Controls.Add(this.lbl_explanation);
            this.pan_display.Controls.Add(this.txt_question);
            this.pan_display.Controls.Add(this.lbl_question_number);
            this.pan_display.Controls.Add(this.label3);
            this.pan_display.Controls.Add(this.lbl_section_title);
            this.pan_display.Controls.Add(this.label2);
            this.pan_display.Controls.Add(this.lbl_exam_code);
            this.pan_display.Controls.Add(this.lbl_exam_instructions);
            this.pan_display.Controls.Add(this.lbl_exam_title);
            this.pan_display.Location = new Point(35, 39);
            this.pan_display.Margin = new Padding(2, 3, 2, 3);
            this.pan_display.Name = "pan_display";
            this.pan_display.Size = new Size(1297, 552);
            this.pan_display.TabIndex = 7;
            // 
            // pct_image
            // 
            this.pct_image.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.pct_image.BackColor = SystemColors.Control;
            this.pct_image.Location = new Point(33, 129);
            this.pct_image.Margin = new Padding(2, 3, 2, 3);
            this.pct_image.Name = "pct_image";
            this.pct_image.Size = new Size(513, 203);
            this.pct_image.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pct_image.TabIndex = 12;
            this.pct_image.TabStop = false;
            this.pct_image.Visible = false;
            // 
            // lbl_explanation
            // 
            this.lbl_explanation.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lbl_explanation.BackColor = SystemColors.Control;
            this.lbl_explanation.BorderStyle = BorderStyle.None;
            this.lbl_explanation.Location = new Point(29, 485);
            this.lbl_explanation.Margin = new Padding(2, 0, 2, 0);
            this.lbl_explanation.Multiline = true;
            this.lbl_explanation.Name = "lbl_explanation";
            this.lbl_explanation.ReadOnly = true;
            this.lbl_explanation.Size = new Size(1254, 57);
            this.lbl_explanation.TabIndex = 9;
            this.lbl_explanation.Text = "lbl_explanation";
            this.lbl_explanation.Visible = false;
            // 
            // txt_question
            // 
            this.txt_question.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txt_question.BorderStyle = BorderStyle.None;
            this.txt_question.Location = new Point(40, 89);
            this.txt_question.Margin = new Padding(2, 3, 2, 3);
            this.txt_question.Multiline = true;
            this.txt_question.Name = "txt_question";
            this.txt_question.ReadOnly = true;
            this.txt_question.Size = new Size(1128, 83);
            this.txt_question.TabIndex = 11;
            this.txt_question.Visible = false;
            // 
            // lbl_question_number
            // 
            this.lbl_question_number.AutoSize = true;
            this.lbl_question_number.Location = new Point(103, 54);
            this.lbl_question_number.Margin = new Padding(2, 0, 2, 0);
            this.lbl_question_number.Name = "lbl_question_number";
            this.lbl_question_number.Size = new Size(118, 15);
            this.lbl_question_number.TabIndex = 10;
            this.lbl_question_number.Text = "lbl_question_number";
            this.lbl_question_number.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(35, 54);
            this.label3.Margin = new Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(58, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Question:";
            this.label3.Visible = false;
            // 
            // lbl_section_title
            // 
            this.lbl_section_title.AutoSize = true;
            this.lbl_section_title.Location = new Point(96, 23);
            this.lbl_section_title.Margin = new Padding(2, 0, 2, 0);
            this.lbl_section_title.Name = "lbl_section_title";
            this.lbl_section_title.Size = new Size(88, 15);
            this.lbl_section_title.TabIndex = 8;
            this.lbl_section_title.Text = "lbl_section_title";
            this.lbl_section_title.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(35, 23);
            this.label2.Margin = new Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(49, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Section:";
            this.label2.Visible = false;
            // 
            // lbl_exam_code
            // 
            this.lbl_exam_code.AutoSize = true;
            this.lbl_exam_code.Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.lbl_exam_code.Location = new Point(29, 62);
            this.lbl_exam_code.Margin = new Padding(2, 0, 2, 0);
            this.lbl_exam_code.Name = "lbl_exam_code";
            this.lbl_exam_code.Size = new Size(105, 15);
            this.lbl_exam_code.TabIndex = 2;
            this.lbl_exam_code.Text = "lbl_exam_code";
            // 
            // lbl_exam_instructions
            // 
            this.lbl_exam_instructions.AutoSize = true;
            this.lbl_exam_instructions.Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.lbl_exam_instructions.Location = new Point(29, 98);
            this.lbl_exam_instructions.Margin = new Padding(2, 0, 2, 0);
            this.lbl_exam_instructions.Name = "lbl_exam_instructions";
            this.lbl_exam_instructions.Size = new Size(41, 15);
            this.lbl_exam_instructions.TabIndex = 1;
            this.lbl_exam_instructions.Text = "label4";
            // 
            // lbl_exam_title
            // 
            this.lbl_exam_title.AutoSize = true;
            this.lbl_exam_title.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.lbl_exam_title.Location = new Point(29, 23);
            this.lbl_exam_title.Margin = new Padding(2, 0, 2, 0);
            this.lbl_exam_title.Name = "lbl_exam_title";
            this.lbl_exam_title.Size = new Size(102, 16);
            this.lbl_exam_title.TabIndex = 0;
            this.lbl_exam_title.Text = "lbl_exam_title";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += this.TimerTick;
            // 
            // btn_show_answer
            // 
            this.btn_show_answer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btn_show_answer.Location = new Point(1094, 5);
            this.btn_show_answer.Margin = new Padding(2, 3, 2, 3);
            this.btn_show_answer.Name = "btn_show_answer";
            this.btn_show_answer.Size = new Size(100, 27);
            this.btn_show_answer.TabIndex = 8;
            this.btn_show_answer.Text = "Show Answer";
            this.btn_show_answer.UseVisualStyleBackColor = true;
            this.btn_show_answer.Visible = false;
            this.btn_show_answer.Click += this.AnswerButtonClick;
            // 
            // dspExamProgress
            // 
            this.dspExamProgress.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.dspExamProgress.AutoSize = true;
            this.dspExamProgress.Location = new Point(951, 10);
            this.dspExamProgress.Margin = new Padding(2, 0, 2, 0);
            this.dspExamProgress.Name = "dspExamProgress";
            this.dspExamProgress.Size = new Size(0, 15);
            this.dspExamProgress.TabIndex = 12;
            this.dspExamProgress.Visible = false;
            // 
            // lblExamProgress
            // 
            this.lblExamProgress.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.lblExamProgress.AutoSize = true;
            this.lblExamProgress.Location = new Point(849, 10);
            this.lblExamProgress.Margin = new Padding(2, 0, 2, 0);
            this.lblExamProgress.Name = "lblExamProgress";
            this.lblExamProgress.Size = new Size(89, 15);
            this.lblExamProgress.TabIndex = 11;
            this.lblExamProgress.Text = " Exam Progress:";
            this.lblExamProgress.Visible = false;
            // 
            // AssessmentUi
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1346, 651);
            this.Controls.Add(this.btn_show_answer);
            this.Controls.Add(this.pan_display);
            this.Controls.Add(this.btn_pause);
            this.Controls.Add(this.btn_end);
            this.Controls.Add(this.btn_begin);
            this.Controls.Add(this.btn_previous);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.lbl_elapsed_time);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblExamProgress);
            this.Controls.Add(this.dspExamProgress);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.Margin = new Padding(2, 3, 2, 3);
            this.Name = "AssessmentUi";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Open Exam Simulator";
            this.Load += this.Start;
            this.pan_display.ResumeLayout(false);
            this.pan_display.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pct_image).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_elapsed_time;
        private System.Windows.Forms.Button btn_begin;
        private System.Windows.Forms.Button btn_previous;
        private System.Windows.Forms.Button btn_next;
        private System.Windows.Forms.Button btn_pause;
        private System.Windows.Forms.Button btn_end;
        private System.Windows.Forms.Panel pan_display;
        private System.Windows.Forms.Label lbl_exam_instructions;
        private System.Windows.Forms.Label lbl_exam_title;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lbl_exam_code;
        private System.Windows.Forms.PictureBox pct_image;
        private System.Windows.Forms.TextBox txt_question;
        private System.Windows.Forms.Label lbl_question_number;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_section_title;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_show_answer;
        private System.Windows.Forms.Label dspExamProgress;
        private System.Windows.Forms.Label lblExamProgress;
        private System.Windows.Forms.TextBox lbl_explanation;
    }
}