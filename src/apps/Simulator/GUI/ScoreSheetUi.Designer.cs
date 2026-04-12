namespace OpenExamSuite.Simulator.GUI
{
    partial class ScoreSheetUi
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScoreSheetUi));
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.label5 = new Label();
            this.label6 = new Label();
            this.btn_retake = new Button();
            this.btn_exit = new Button();
            this.dgv_show_breakdown = new DataGridView();
            this.section = new DataGridViewTextBoxColumn();
            this.number = new DataGridViewTextBoxColumn();
            this.accuracy = new DataGridViewTextBoxColumn();
            this.lbl_candidate_name = new Label();
            this.lbl_date = new Label();
            this.lbl_exam_number = new Label();
            this.lbl_elapsed_time = new Label();
            this.lbl_time_allowed = new Label();
            this.label7 = new Label();
            this.lbl_status = new Label();
            this.label8 = new Label();
            this.label9 = new Label();
            this.btn_print_score = new Button();
            this.pnt_prv_dlg = new PrintPreviewDialog();
            this.pnt_doc = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)this.dgv_show_breakdown).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(382, 27);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(167, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Examination Report";
            // 
            // label2
            // 
            this.label2.Anchor = AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(260, 80);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(74, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "CANDIDATE:";
            // 
            // label3
            // 
            this.label3.Anchor = AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new Point(260, 118);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(38, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "DATE:";
            // 
            // label4
            // 
            this.label4.Anchor = AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new Point(260, 155);
            this.label4.Margin = new Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(76, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "EXAM CODE:";
            // 
            // label5
            // 
            this.label5.Anchor = AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new Point(587, 80);
            this.label5.Margin = new Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(94, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "TIME ALLOWED:";
            // 
            // label6
            // 
            this.label6.Anchor = AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new Point(587, 118);
            this.label6.Margin = new Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new Size(76, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "TIME TAKEN:";
            // 
            // btn_retake
            // 
            this.btn_retake.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btn_retake.Location = new Point(27, 744);
            this.btn_retake.Margin = new Padding(4, 3, 4, 3);
            this.btn_retake.Name = "btn_retake";
            this.btn_retake.Size = new Size(88, 27);
            this.btn_retake.TabIndex = 7;
            this.btn_retake.Text = "Retake";
            this.btn_retake.UseVisualStyleBackColor = true;
            this.btn_retake.Click += this.Retake;
            // 
            // btn_exit
            // 
            this.btn_exit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btn_exit.Location = new Point(967, 744);
            this.btn_exit.Margin = new Padding(4, 3, 4, 3);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new Size(88, 27);
            this.btn_exit.TabIndex = 9;
            this.btn_exit.Text = "Exit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += this.Exit;
            // 
            // dgv_show_breakdown
            // 
            this.dgv_show_breakdown.AllowUserToAddRows = false;
            this.dgv_show_breakdown.AllowUserToDeleteRows = false;
            this.dgv_show_breakdown.Anchor = AnchorStyles.None;
            this.dgv_show_breakdown.BackgroundColor = SystemColors.Control;
            this.dgv_show_breakdown.BorderStyle = BorderStyle.None;
            this.dgv_show_breakdown.CellBorderStyle = DataGridViewCellBorderStyle.None;
            this.dgv_show_breakdown.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.Control;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.dgv_show_breakdown.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_show_breakdown.ColumnHeadersHeight = 25;
            this.dgv_show_breakdown.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv_show_breakdown.Columns.AddRange(new DataGridViewColumn[] { this.section, this.number, this.accuracy });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = Color.MidnightBlue;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.dgv_show_breakdown.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_show_breakdown.Location = new Point(240, 462);
            this.dgv_show_breakdown.Margin = new Padding(4, 3, 4, 3);
            this.dgv_show_breakdown.Name = "dgv_show_breakdown";
            this.dgv_show_breakdown.ReadOnly = true;
            this.dgv_show_breakdown.RowHeadersVisible = false;
            this.dgv_show_breakdown.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv_show_breakdown.Size = new Size(578, 263);
            this.dgv_show_breakdown.TabIndex = 11;
            // 
            // section
            // 
            this.section.HeaderText = "Section";
            this.section.Name = "section";
            this.section.ReadOnly = true;
            this.section.Resizable = DataGridViewTriState.False;
            this.section.Width = 345;
            // 
            // number
            // 
            this.number.HeaderText = "Number";
            this.number.Name = "number";
            this.number.ReadOnly = true;
            this.number.Width = 75;
            // 
            // accuracy
            // 
            this.accuracy.HeaderText = "Correct";
            this.accuracy.Name = "accuracy";
            this.accuracy.ReadOnly = true;
            this.accuracy.Width = 75;
            // 
            // lbl_candidate_name
            // 
            this.lbl_candidate_name.Anchor = AnchorStyles.None;
            this.lbl_candidate_name.AutoSize = true;
            this.lbl_candidate_name.Location = new Point(351, 80);
            this.lbl_candidate_name.Margin = new Padding(4, 0, 4, 0);
            this.lbl_candidate_name.Name = "lbl_candidate_name";
            this.lbl_candidate_name.Size = new Size(0, 15);
            this.lbl_candidate_name.TabIndex = 12;
            // 
            // lbl_date
            // 
            this.lbl_date.Anchor = AnchorStyles.None;
            this.lbl_date.AutoSize = true;
            this.lbl_date.Location = new Point(313, 118);
            this.lbl_date.Margin = new Padding(4, 0, 4, 0);
            this.lbl_date.Name = "lbl_date";
            this.lbl_date.Size = new Size(0, 15);
            this.lbl_date.TabIndex = 13;
            // 
            // lbl_exam_number
            // 
            this.lbl_exam_number.Anchor = AnchorStyles.None;
            this.lbl_exam_number.AutoSize = true;
            this.lbl_exam_number.Location = new Point(352, 155);
            this.lbl_exam_number.Margin = new Padding(4, 0, 4, 0);
            this.lbl_exam_number.Name = "lbl_exam_number";
            this.lbl_exam_number.Size = new Size(0, 15);
            this.lbl_exam_number.TabIndex = 14;
            // 
            // lbl_elapsed_time
            // 
            this.lbl_elapsed_time.Anchor = AnchorStyles.None;
            this.lbl_elapsed_time.AutoSize = true;
            this.lbl_elapsed_time.Location = new Point(693, 118);
            this.lbl_elapsed_time.Margin = new Padding(4, 0, 4, 0);
            this.lbl_elapsed_time.Name = "lbl_elapsed_time";
            this.lbl_elapsed_time.Size = new Size(0, 15);
            this.lbl_elapsed_time.TabIndex = 16;
            this.lbl_elapsed_time.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbl_time_allowed
            // 
            this.lbl_time_allowed.Anchor = AnchorStyles.None;
            this.lbl_time_allowed.AutoSize = true;
            this.lbl_time_allowed.Location = new Point(694, 80);
            this.lbl_time_allowed.Margin = new Padding(4, 0, 4, 0);
            this.lbl_time_allowed.Name = "lbl_time_allowed";
            this.lbl_time_allowed.Size = new Size(0, 15);
            this.lbl_time_allowed.TabIndex = 15;
            this.lbl_time_allowed.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new Point(434, 407);
            this.label7.Margin = new Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new Size(50, 15);
            this.label7.TabIndex = 17;
            this.label7.Text = "STATUS:";
            // 
            // lbl_status
            // 
            this.lbl_status.Anchor = AnchorStyles.None;
            this.lbl_status.AutoSize = true;
            this.lbl_status.Location = new Point(503, 407);
            this.lbl_status.Margin = new Padding(4, 0, 4, 0);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new Size(0, 15);
            this.lbl_status.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new Point(732, 80);
            this.label8.Margin = new Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new Size(41, 15);
            this.label8.TabIndex = 19;
            this.label8.Text = "min(s)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new Point(735, 118);
            this.label9.Margin = new Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new Size(41, 15);
            this.label9.TabIndex = 20;
            this.label9.Text = "min(s)";
            // 
            // btn_print_score
            // 
            this.btn_print_score.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btn_print_score.Location = new Point(148, 744);
            this.btn_print_score.Margin = new Padding(4, 3, 4, 3);
            this.btn_print_score.Name = "btn_print_score";
            this.btn_print_score.Size = new Size(88, 27);
            this.btn_print_score.TabIndex = 21;
            this.btn_print_score.Text = "Print";
            this.btn_print_score.UseVisualStyleBackColor = true;
            this.btn_print_score.Click += this.PrintResult;
            // 
            // pnt_prv_dlg
            // 
            this.pnt_prv_dlg.AutoScrollMargin = new Size(0, 0);
            this.pnt_prv_dlg.AutoScrollMinSize = new Size(0, 0);
            this.pnt_prv_dlg.ClientSize = new Size(400, 300);
            this.pnt_prv_dlg.Document = this.pnt_doc;
            this.pnt_prv_dlg.Enabled = true;
            this.pnt_prv_dlg.Icon = (Icon)resources.GetObject("pnt_prv_dlg.Icon");
            this.pnt_prv_dlg.Name = "pnt_prv_dlg";
            this.pnt_prv_dlg.ShowIcon = false;
            this.pnt_prv_dlg.Visible = false;
            // 
            // pnt_doc
            // 
            this.pnt_doc.PrintPage += this.Print;
            // 
            // ScoreSheetUi
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1069, 785);
            this.Controls.Add(this.btn_print_score);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbl_elapsed_time);
            this.Controls.Add(this.lbl_time_allowed);
            this.Controls.Add(this.lbl_exam_number);
            this.Controls.Add(this.lbl_date);
            this.Controls.Add(this.lbl_candidate_name);
            this.Controls.Add(this.dgv_show_breakdown);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.btn_retake);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.Margin = new Padding(4, 3, 4, 3);
            this.Name = "ScoreSheetUi";
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Score Sheet";
            this.Load += this.LoadDataToUi;
            ((System.ComponentModel.ISupportInitialize)this.dgv_show_breakdown).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_retake;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.DataVisualization.Charting.Chart chr_display_score;
        private System.Windows.Forms.DataGridView dgv_show_breakdown;
        private System.Windows.Forms.Label lbl_candidate_name;
        private System.Windows.Forms.Label lbl_date;
        private System.Windows.Forms.Label lbl_exam_number;
        private System.Windows.Forms.Label lbl_elapsed_time;
        private System.Windows.Forms.Label lbl_time_allowed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_print_score;
        private System.Windows.Forms.PrintPreviewDialog pnt_prv_dlg;
        private System.Drawing.Printing.PrintDocument pnt_doc;
        private System.Windows.Forms.DataGridViewTextBoxColumn section;
        private System.Windows.Forms.DataGridViewTextBoxColumn number;
        private System.Windows.Forms.DataGridViewTextBoxColumn accuracy;
    }
}