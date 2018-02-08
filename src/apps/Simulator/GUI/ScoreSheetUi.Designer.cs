namespace Simulator.GUI
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
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel1 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel2 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScoreSheetUi));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_retake = new System.Windows.Forms.Button();
            this.btn_exit = new System.Windows.Forms.Button();
            this.chr_display_score = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dgv_show_breakdown = new System.Windows.Forms.DataGridView();
            this.section = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accuracy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbl_candidate_name = new System.Windows.Forms.Label();
            this.lbl_date = new System.Windows.Forms.Label();
            this.lbl_exam_number = new System.Windows.Forms.Label();
            this.lbl_elapsed_time = new System.Windows.Forms.Label();
            this.lbl_time_allowed = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbl_status = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_print_score = new System.Windows.Forms.Button();
            this.pnt_prv_dlg = new System.Windows.Forms.PrintPreviewDialog();
            this.pnt_doc = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)(this.chr_display_score)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_show_breakdown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(327, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Examination Score Report";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "CANDIDATE:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(223, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "DATE:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(223, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "EXAM CODE:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(503, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "TIME ALLOWED:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(503, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "ELAPSED TIME:";
            // 
            // btn_retake
            // 
            this.btn_retake.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_retake.Location = new System.Drawing.Point(23, 645);
            this.btn_retake.Name = "btn_retake";
            this.btn_retake.Size = new System.Drawing.Size(75, 23);
            this.btn_retake.TabIndex = 7;
            this.btn_retake.Text = "Retake";
            this.btn_retake.UseVisualStyleBackColor = true;
            this.btn_retake.Click += new System.EventHandler(this.Retake);
            // 
            // btn_exit
            // 
            this.btn_exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_exit.Location = new System.Drawing.Point(829, 645);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(75, 23);
            this.btn_exit.TabIndex = 9;
            this.btn_exit.Text = "Exit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.Exit);
            // 
            // chr_display_score
            // 
            this.chr_display_score.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chr_display_score.BackColor = System.Drawing.SystemColors.Control;
            customLabel1.FromPosition = 0.5D;
            customLabel1.Text = "Pass Mark";
            customLabel1.ToPosition = 1.5D;
            customLabel2.FromPosition = -0.5D;
            customLabel2.Text = "Your Score";
            customLabel2.ToPosition = 0.5D;
            chartArea1.AxisX.CustomLabels.Add(customLabel1);
            chartArea1.AxisX.CustomLabels.Add(customLabel2);
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.Maximum = 2D;
            chartArea1.AxisY.Maximum = 1000D;
            chartArea1.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.Name = "ChartArea1";
            this.chr_display_score.ChartAreas.Add(chartArea1);
            this.chr_display_score.Location = new System.Drawing.Point(184, 169);
            this.chr_display_score.Name = "chr_display_score";
            this.chr_display_score.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.chr_display_score.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192))))),
        System.Drawing.Color.Green};
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            series1.Name = "Pass Mark";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            series2.Name = "Your Score";
            this.chr_display_score.Series.Add(series1);
            this.chr_display_score.Series.Add(series2);
            this.chr_display_score.Size = new System.Drawing.Size(542, 171);
            this.chr_display_score.TabIndex = 10;
            // 
            // dgv_show_breakdown
            // 
            this.dgv_show_breakdown.AllowUserToAddRows = false;
            this.dgv_show_breakdown.AllowUserToDeleteRows = false;
            this.dgv_show_breakdown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgv_show_breakdown.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv_show_breakdown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_show_breakdown.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv_show_breakdown.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_show_breakdown.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_show_breakdown.ColumnHeadersHeight = 25;
            this.dgv_show_breakdown.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv_show_breakdown.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.section,
            this.number,
            this.accuracy});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_show_breakdown.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_show_breakdown.Location = new System.Drawing.Point(206, 400);
            this.dgv_show_breakdown.Name = "dgv_show_breakdown";
            this.dgv_show_breakdown.ReadOnly = true;
            this.dgv_show_breakdown.RowHeadersVisible = false;
            this.dgv_show_breakdown.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_show_breakdown.Size = new System.Drawing.Size(495, 228);
            this.dgv_show_breakdown.TabIndex = 11;
            // 
            // section
            // 
            this.section.HeaderText = "Section";
            this.section.Name = "section";
            this.section.ReadOnly = true;
            this.section.Resizable = System.Windows.Forms.DataGridViewTriState.False;
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
            this.lbl_candidate_name.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_candidate_name.AutoSize = true;
            this.lbl_candidate_name.Location = new System.Drawing.Point(301, 69);
            this.lbl_candidate_name.Name = "lbl_candidate_name";
            this.lbl_candidate_name.Size = new System.Drawing.Size(0, 13);
            this.lbl_candidate_name.TabIndex = 12;
            // 
            // lbl_date
            // 
            this.lbl_date.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_date.AutoSize = true;
            this.lbl_date.Location = new System.Drawing.Point(268, 102);
            this.lbl_date.Name = "lbl_date";
            this.lbl_date.Size = new System.Drawing.Size(0, 13);
            this.lbl_date.TabIndex = 13;
            // 
            // lbl_exam_number
            // 
            this.lbl_exam_number.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_exam_number.AutoSize = true;
            this.lbl_exam_number.Location = new System.Drawing.Point(302, 134);
            this.lbl_exam_number.Name = "lbl_exam_number";
            this.lbl_exam_number.Size = new System.Drawing.Size(0, 13);
            this.lbl_exam_number.TabIndex = 14;
            // 
            // lbl_elapsed_time
            // 
            this.lbl_elapsed_time.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_elapsed_time.AutoSize = true;
            this.lbl_elapsed_time.Location = new System.Drawing.Point(594, 102);
            this.lbl_elapsed_time.Name = "lbl_elapsed_time";
            this.lbl_elapsed_time.Size = new System.Drawing.Size(0, 13);
            this.lbl_elapsed_time.TabIndex = 16;
            this.lbl_elapsed_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_time_allowed
            // 
            this.lbl_time_allowed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_time_allowed.AutoSize = true;
            this.lbl_time_allowed.Location = new System.Drawing.Point(595, 69);
            this.lbl_time_allowed.Name = "lbl_time_allowed";
            this.lbl_time_allowed.Size = new System.Drawing.Size(0, 13);
            this.lbl_time_allowed.TabIndex = 15;
            this.lbl_time_allowed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(372, 353);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "STATUS:";
            // 
            // lbl_status
            // 
            this.lbl_status.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_status.AutoSize = true;
            this.lbl_status.Location = new System.Drawing.Point(431, 353);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(0, 13);
            this.lbl_status.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(627, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "min(s)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(630, 102);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "min(s)";
            // 
            // btn_print_score
            // 
            this.btn_print_score.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_print_score.Location = new System.Drawing.Point(127, 645);
            this.btn_print_score.Name = "btn_print_score";
            this.btn_print_score.Size = new System.Drawing.Size(75, 23);
            this.btn_print_score.TabIndex = 21;
            this.btn_print_score.Text = "Print";
            this.btn_print_score.UseVisualStyleBackColor = true;
            this.btn_print_score.Click += new System.EventHandler(this.PrintResult);
            // 
            // pnt_prv_dlg
            // 
            this.pnt_prv_dlg.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.pnt_prv_dlg.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.pnt_prv_dlg.ClientSize = new System.Drawing.Size(400, 300);
            this.pnt_prv_dlg.Document = this.pnt_doc;
            this.pnt_prv_dlg.Enabled = true;
            this.pnt_prv_dlg.Icon = ((System.Drawing.Icon)(resources.GetObject("pnt_prv_dlg.Icon")));
            this.pnt_prv_dlg.Name = "pnt_prv_dlg";
            this.pnt_prv_dlg.ShowIcon = false;
            this.pnt_prv_dlg.Visible = false;
            // 
            // pnt_doc
            // 
            this.pnt_doc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.Print);
            // 
            // Score_Sheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 680);
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
            this.Controls.Add(this.chr_display_score);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.btn_retake);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScoreSheet";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Score Sheet";
            this.Load += new System.EventHandler(this.LoadDataToUi);
            ((System.ComponentModel.ISupportInitialize)(this.chr_display_score)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_show_breakdown)).EndInit();
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