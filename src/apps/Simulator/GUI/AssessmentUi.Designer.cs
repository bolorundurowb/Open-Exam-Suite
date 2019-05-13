namespace Simulator.GUI
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(Simulator.GUI.AssessmentUi));
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_elapsed_time = new System.Windows.Forms.Label();
            this.btn_begin = new System.Windows.Forms.Button();
            this.btn_previous = new System.Windows.Forms.Button();
            this.btn_next = new System.Windows.Forms.Button();
            this.btn_pause = new System.Windows.Forms.Button();
            this.btn_end = new System.Windows.Forms.Button();
            this.pan_display = new System.Windows.Forms.Panel();
            this.pct_image = new System.Windows.Forms.PictureBox();
            this.txt_question = new System.Windows.Forms.TextBox();
            this.lbl_question_number = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_section_title = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_exam_code = new System.Windows.Forms.Label();
            this.lbl_exam_instructions = new System.Windows.Forms.Label();
            this.lbl_exam_title = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btn_show_answer = new System.Windows.Forms.Button();
            this.lbl_explanation = new System.Windows.Forms.TextBox();
            this.btnHideAnswers = new System.Windows.Forms.Button();
            this.pan_display.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.pct_image)).BeginInit();
            this.SuspendLayout();
            this.label1.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1728, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Time Left:";
            this.label1.Visible = false;
            this.lbl_elapsed_time.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_elapsed_time.AutoSize = true;
            this.lbl_elapsed_time.Location = new System.Drawing.Point(1827, 18);
            this.lbl_elapsed_time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_elapsed_time.Name = "lbl_elapsed_time";
            this.lbl_elapsed_time.Size = new System.Drawing.Size(0, 25);
            this.lbl_elapsed_time.TabIndex = 1;
            this.lbl_elapsed_time.Visible = false;
            this.btn_begin.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Left)));
            this.btn_begin.Location = new System.Drawing.Point(22, 1291);
            this.btn_begin.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btn_begin.Name = "btn_begin";
            this.btn_begin.Size = new System.Drawing.Size(124, 44);
            this.btn_begin.TabIndex = 2;
            this.btn_begin.Text = "Begin";
            this.btn_begin.UseVisualStyleBackColor = true;
            this.btn_begin.Click += new System.EventHandler(this.Begin);
            this.btn_previous.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Left)));
            this.btn_previous.Enabled = false;
            this.btn_previous.Location = new System.Drawing.Point(22, 1291);
            this.btn_previous.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btn_previous.Name = "btn_previous";
            this.btn_previous.Size = new System.Drawing.Size(124, 44);
            this.btn_previous.TabIndex = 3;
            this.btn_previous.Text = "Previous";
            this.btn_previous.UseVisualStyleBackColor = true;
            this.btn_previous.Visible = false;
            this.btn_previous.Click += new System.EventHandler(this.Previous);
            this.btn_next.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Left)));
            this.btn_next.Enabled = false;
            this.btn_next.Location = new System.Drawing.Point(176, 1291);
            this.btn_next.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(124, 44);
            this.btn_next.TabIndex = 4;
            this.btn_next.Text = "Next";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Visible = false;
            this.btn_next.Click += new System.EventHandler(this.Next);
            this.btn_pause.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.btn_pause.Location = new System.Drawing.Point(1661, 1291);
            this.btn_pause.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(124, 44);
            this.btn_pause.TabIndex = 5;
            this.btn_pause.Text = "Pause";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Visible = false;
            this.btn_pause.Click += new System.EventHandler(this.PauseExam);
            this.btn_end.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.btn_end.Location = new System.Drawing.Point(1811, 1291);
            this.btn_end.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btn_end.Name = "btn_end";
            this.btn_end.Size = new System.Drawing.Size(124, 44);
            this.btn_end.TabIndex = 6;
            this.btn_end.Text = "End";
            this.btn_end.UseVisualStyleBackColor = true;
            this.btn_end.Visible = false;
            this.btn_end.Click += new System.EventHandler(this.End);
            this.pan_display.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
                                                         System.Windows.Forms.AnchorStyles.Bottom) |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.pan_display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pan_display.Controls.Add(this.pct_image);
            this.pan_display.Controls.Add(this.txt_question);
            this.pan_display.Controls.Add(this.lbl_question_number);
            this.pan_display.Controls.Add(this.label3);
            this.pan_display.Controls.Add(this.lbl_section_title);
            this.pan_display.Controls.Add(this.label2);
            this.pan_display.Controls.Add(this.lbl_exam_code);
            this.pan_display.Controls.Add(this.lbl_exam_instructions);
            this.pan_display.Controls.Add(this.lbl_exam_title);
            this.pan_display.Location = new System.Drawing.Point(50, 65);
            this.pan_display.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.pan_display.Name = "pan_display";
            this.pan_display.Size = new System.Drawing.Size(1864, 1084);
            this.pan_display.TabIndex = 7;
            this.pct_image.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pct_image.Location = new System.Drawing.Point(124, 298);
            this.pct_image.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.pct_image.Name = "pct_image";
            this.pct_image.Size = new System.Drawing.Size(964, 496);
            this.pct_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pct_image.TabIndex = 12;
            this.pct_image.TabStop = false;
            this.pct_image.Visible = false;
            this.txt_question.Anchor =
                ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.txt_question.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_question.Location = new System.Drawing.Point(56, 148);
            this.txt_question.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txt_question.Multiline = true;
            this.txt_question.Name = "txt_question";
            this.txt_question.ReadOnly = true;
            this.txt_question.Size = new System.Drawing.Size(1691, 139);
            this.txt_question.TabIndex = 11;
            this.txt_question.Visible = false;
            this.lbl_question_number.AutoSize = true;
            this.lbl_question_number.Location = new System.Drawing.Point(147, 90);
            this.lbl_question_number.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_question_number.Name = "lbl_question_number";
            this.lbl_question_number.Size = new System.Drawing.Size(176, 25);
            this.lbl_question_number.TabIndex = 10;
            this.lbl_question_number.Text = "lbl_question_number";
            this.lbl_question_number.Visible = false;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 90);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "Question:";
            this.label3.Visible = false;
            this.lbl_section_title.AutoSize = true;
            this.lbl_section_title.Location = new System.Drawing.Point(137, 39);
            this.lbl_section_title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_section_title.Name = "lbl_section_title";
            this.lbl_section_title.Size = new System.Drawing.Size(130, 25);
            this.lbl_section_title.TabIndex = 8;
            this.lbl_section_title.Text = "lbl_section_title";
            this.lbl_section_title.Visible = false;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Section:";
            this.label2.Visible = false;
            this.lbl_exam_code.AutoSize = true;
            this.lbl_exam_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F,
                System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lbl_exam_code.Location = new System.Drawing.Point(42, 104);
            this.lbl_exam_code.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_exam_code.Name = "lbl_exam_code";
            this.lbl_exam_code.Size = new System.Drawing.Size(133, 20);
            this.lbl_exam_code.TabIndex = 2;
            this.lbl_exam_code.Text = "lbl_exam_code";
            this.lbl_exam_instructions.AutoSize = true;
            this.lbl_exam_instructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F,
                System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lbl_exam_instructions.Location = new System.Drawing.Point(42, 164);
            this.lbl_exam_instructions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_exam_instructions.Name = "lbl_exam_instructions";
            this.lbl_exam_instructions.Size = new System.Drawing.Size(53, 20);
            this.lbl_exam_instructions.TabIndex = 1;
            this.lbl_exam_instructions.Text = "label4";
            this.lbl_exam_title.AutoSize = true;
            this.lbl_exam_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F,
                System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lbl_exam_title.Location = new System.Drawing.Point(42, 39);
            this.lbl_exam_title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_exam_title.Name = "lbl_exam_title";
            this.lbl_exam_title.Size = new System.Drawing.Size(137, 24);
            this.lbl_exam_title.TabIndex = 0;
            this.lbl_exam_title.Text = "lbl_exam_title";
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            this.btn_show_answer.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.btn_show_answer.Location = new System.Drawing.Point(1563, 8);
            this.btn_show_answer.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btn_show_answer.Name = "btn_show_answer";
            this.btn_show_answer.Size = new System.Drawing.Size(144, 44);
            this.btn_show_answer.TabIndex = 8;
            this.btn_show_answer.Text = "Show Answer";
            this.btn_show_answer.UseVisualStyleBackColor = true;
            this.btn_show_answer.Visible = false;
            this.btn_show_answer.Click += new System.EventHandler(this.ShowAnswer);
            this.lbl_explanation.Anchor =
                ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_explanation.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_explanation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbl_explanation.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_explanation.Location = new System.Drawing.Point(87, 1175);
            this.lbl_explanation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_explanation.Multiline = true;
            this.lbl_explanation.Name = "lbl_explanation";
            this.lbl_explanation.ReadOnly = true;
            this.lbl_explanation.Size = new System.Drawing.Size(1794, 94);
            this.lbl_explanation.TabIndex = 9;
            this.lbl_explanation.Text = "lbl_explanation";
            this.lbl_explanation.Visible = false;
            this.btnHideAnswers.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.btnHideAnswers.Location = new System.Drawing.Point(1563, 8);
            this.btnHideAnswers.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnHideAnswers.Name = "btnHideAnswers";
            this.btnHideAnswers.Size = new System.Drawing.Size(144, 44);
            this.btnHideAnswers.TabIndex = 10;
            this.btnHideAnswers.Text = "Hide Answer";
            this.btnHideAnswers.UseVisualStyleBackColor = true;
            this.btnHideAnswers.Visible = false;
            this.btnHideAnswers.Click += new System.EventHandler(this.HideAnswer);
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1962, 1358);
            this.Controls.Add(this.btnHideAnswers);
            this.Controls.Add(this.lbl_explanation);
            this.Controls.Add(this.btn_show_answer);
            this.Controls.Add(this.pan_display);
            this.Controls.Add(this.btn_end);
            this.Controls.Add(this.btn_pause);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.btn_previous);
            this.Controls.Add(this.btn_begin);
            this.Controls.Add(this.lbl_elapsed_time);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "AssessmentUi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Exam Simulator";
            this.Load += new System.EventHandler(this.Start);
            this.pan_display.ResumeLayout(false);
            this.pan_display.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.pct_image)).EndInit();
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
        private System.Windows.Forms.Button btnHideAnswers;
        private System.Windows.Forms.TextBox lbl_explanation;
    }
}