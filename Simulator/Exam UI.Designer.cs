using System;

namespace Simulator
{
    partial class Exam_UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Exam_UI));
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_elapsed_time = new System.Windows.Forms.Label();
            this.btn_begin = new System.Windows.Forms.Button();
            this.btn_previous = new System.Windows.Forms.Button();
            this.btn_next = new System.Windows.Forms.Button();
            this.btn_pause = new System.Windows.Forms.Button();
            this.btn_end = new System.Windows.Forms.Button();
            this.pan_display = new System.Windows.Forms.Panel();
            this.pic_image = new System.Windows.Forms.PictureBox();
            this.txt_question = new System.Windows.Forms.TextBox();
            this.lbl_question_number = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_section_title = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_exam_code = new System.Windows.Forms.Label();
            this.lbl_exam_instructions = new System.Windows.Forms.Label();
            this.lbl_exam_title = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pan_display.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_image)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1098, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Time Left:";
            this.label1.Visible = false;
            // 
            // lbl_elapsed_time
            // 
            this.lbl_elapsed_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_elapsed_time.AutoSize = true;
            this.lbl_elapsed_time.Location = new System.Drawing.Point(1157, 9);
            this.lbl_elapsed_time.Name = "lbl_elapsed_time";
            this.lbl_elapsed_time.Size = new System.Drawing.Size(0, 13);
            this.lbl_elapsed_time.TabIndex = 1;
            this.lbl_elapsed_time.Visible = false;
            // 
            // btn_begin
            // 
            this.btn_begin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_begin.Location = new System.Drawing.Point(13, 649);
            this.btn_begin.Name = "btn_begin";
            this.btn_begin.Size = new System.Drawing.Size(75, 23);
            this.btn_begin.TabIndex = 2;
            this.btn_begin.Text = "Begin";
            this.btn_begin.UseVisualStyleBackColor = true;
            // 
            // btn_previous
            // 
            this.btn_previous.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_previous.Enabled = false;
            this.btn_previous.Location = new System.Drawing.Point(13, 649);
            this.btn_previous.Name = "btn_previous";
            this.btn_previous.Size = new System.Drawing.Size(75, 23);
            this.btn_previous.TabIndex = 3;
            this.btn_previous.Text = "Previous";
            this.btn_previous.UseVisualStyleBackColor = true;
            this.btn_previous.Visible = false;
            // 
            // btn_next
            // 
            this.btn_next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_next.Enabled = false;
            this.btn_next.Location = new System.Drawing.Point(105, 649);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(75, 23);
            this.btn_next.TabIndex = 4;
            this.btn_next.Text = "Next";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Visible = false;
            // 
            // btn_pause
            // 
            this.btn_pause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_pause.Enabled = false;
            this.btn_pause.Location = new System.Drawing.Point(1058, 649);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(75, 23);
            this.btn_pause.TabIndex = 5;
            this.btn_pause.Text = "Pause";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Visible = false;
            // 
            // btn_end
            // 
            this.btn_end.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_end.Enabled = false;
            this.btn_end.Location = new System.Drawing.Point(1148, 649);
            this.btn_end.Name = "btn_end";
            this.btn_end.Size = new System.Drawing.Size(75, 23);
            this.btn_end.TabIndex = 6;
            this.btn_end.Text = "End";
            this.btn_end.UseVisualStyleBackColor = true;
            this.btn_end.Visible = false;
            // 
            // pan_display
            // 
            this.pan_display.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pan_display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pan_display.Controls.Add(this.pic_image);
            this.pan_display.Controls.Add(this.txt_question);
            this.pan_display.Controls.Add(this.lbl_question_number);
            this.pan_display.Controls.Add(this.label3);
            this.pan_display.Controls.Add(this.lbl_section_title);
            this.pan_display.Controls.Add(this.label2);
            this.pan_display.Controls.Add(this.lbl_exam_code);
            this.pan_display.Controls.Add(this.lbl_exam_instructions);
            this.pan_display.Controls.Add(this.lbl_exam_title);
            this.pan_display.Location = new System.Drawing.Point(30, 34);
            this.pan_display.Name = "pan_display";
            this.pan_display.Size = new System.Drawing.Size(1181, 597);
            this.pan_display.TabIndex = 7;
            // 
            // pic_image
            // 
            this.pic_image.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pic_image.Location = new System.Drawing.Point(105, 155);
            this.pic_image.Name = "pic_image";
            this.pic_image.Size = new System.Drawing.Size(579, 289);
            this.pic_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_image.TabIndex = 12;
            this.pic_image.TabStop = false;
            this.pic_image.Visible = false;
            // 
            // txt_question
            // 
            this.txt_question.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_question.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_question.Location = new System.Drawing.Point(33, 77);
            this.txt_question.Multiline = true;
            this.txt_question.Name = "txt_question";
            this.txt_question.ReadOnly = true;
            this.txt_question.Size = new System.Drawing.Size(1076, 72);
            this.txt_question.TabIndex = 11;
            this.txt_question.Visible = false;
            // 
            // lbl_question_number
            // 
            this.lbl_question_number.AutoSize = true;
            this.lbl_question_number.Location = new System.Drawing.Point(88, 47);
            this.lbl_question_number.Name = "lbl_question_number";
            this.lbl_question_number.Size = new System.Drawing.Size(35, 13);
            this.lbl_question_number.TabIndex = 10;
            this.lbl_question_number.Text = "label4";
            this.lbl_question_number.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Question:";
            this.label3.Visible = false;
            // 
            // lbl_section_title
            // 
            this.lbl_section_title.AutoSize = true;
            this.lbl_section_title.Location = new System.Drawing.Point(82, 20);
            this.lbl_section_title.Name = "lbl_section_title";
            this.lbl_section_title.Size = new System.Drawing.Size(35, 13);
            this.lbl_section_title.TabIndex = 8;
            this.lbl_section_title.Text = "label2";
            this.lbl_section_title.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Section:";
            this.label2.Visible = false;
            // 
            // lbl_exam_code
            // 
            this.lbl_exam_code.AutoSize = true;
            this.lbl_exam_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_exam_code.Location = new System.Drawing.Point(25, 54);
            this.lbl_exam_code.Name = "lbl_exam_code";
            this.lbl_exam_code.Size = new System.Drawing.Size(47, 15);
            this.lbl_exam_code.TabIndex = 2;
            this.lbl_exam_code.Text = "label4";
            // 
            // lbl_exam_instructions
            // 
            this.lbl_exam_instructions.AutoSize = true;
            this.lbl_exam_instructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_exam_instructions.Location = new System.Drawing.Point(25, 85);
            this.lbl_exam_instructions.Name = "lbl_exam_instructions";
            this.lbl_exam_instructions.Size = new System.Drawing.Size(41, 15);
            this.lbl_exam_instructions.TabIndex = 1;
            this.lbl_exam_instructions.Text = "label4";
            // 
            // lbl_exam_title
            // 
            this.lbl_exam_title.AutoSize = true;
            this.lbl_exam_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_exam_title.Location = new System.Drawing.Point(25, 20);
            this.lbl_exam_title.Name = "lbl_exam_title";
            this.lbl_exam_title.Size = new System.Drawing.Size(51, 16);
            this.lbl_exam_title.TabIndex = 0;
            this.lbl_exam_title.Text = "label3";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // Exam_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 684);
            this.Controls.Add(this.pan_display);
            this.Controls.Add(this.btn_end);
            this.Controls.Add(this.btn_pause);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.btn_previous);
            this.Controls.Add(this.btn_begin);
            this.Controls.Add(this.lbl_elapsed_time);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Exam_UI";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Exam Simulator";
            this.pan_display.ResumeLayout(false);
            this.pan_display.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_image)).EndInit();
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
        private System.Windows.Forms.PictureBox pic_image;
        private System.Windows.Forms.TextBox txt_question;
        private System.Windows.Forms.Label lbl_question_number;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_section_title;
        private System.Windows.Forms.Label label2;
    }
}