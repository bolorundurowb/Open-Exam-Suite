namespace Simulator
{
    partial class Score_Sheet
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel3 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel4 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.lbl_time = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbl_status = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chr_display_score)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_show_breakdown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(398, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Examination Score Report";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(294, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "CANDIDATE:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(294, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "DATE:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(294, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "EXAM CODE:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(574, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "TIME ALLOWED:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(574, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "ELAPSED TIME:";
            // 
            // btn_retake
            // 
            this.btn_retake.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_retake.Location = new System.Drawing.Point(23, 692);
            this.btn_retake.Name = "btn_retake";
            this.btn_retake.Size = new System.Drawing.Size(75, 23);
            this.btn_retake.TabIndex = 7;
            this.btn_retake.Text = "Retake";
            this.btn_retake.UseVisualStyleBackColor = true;
            this.btn_retake.Click += new System.EventHandler(this.btn_retake_Click);
            // 
            // btn_exit
            // 
            this.btn_exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_exit.Location = new System.Drawing.Point(971, 692);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(75, 23);
            this.btn_exit.TabIndex = 9;
            this.btn_exit.Text = "Exit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // chr_display_score
            // 
            this.chr_display_score.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chr_display_score.BackColor = System.Drawing.SystemColors.Control;
            customLabel3.Text = "Required Score";
            customLabel4.Text = "Your Score";
            chartArea2.AxisX.CustomLabels.Add(customLabel3);
            chartArea2.AxisX.CustomLabels.Add(customLabel4);
            chartArea2.AxisX.Maximum = 2D;
            chartArea2.AxisY.Maximum = 1000D;
            chartArea2.Name = "ChartArea1";
            this.chr_display_score.ChartAreas.Add(chartArea2);
            this.chr_display_score.Location = new System.Drawing.Point(271, 192);
            this.chr_display_score.Name = "chr_display_score";
            this.chr_display_score.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.chr_display_score.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192))))),
        System.Drawing.Color.Green};
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            series3.Name = "Required Score";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            series4.Name = "Score";
            this.chr_display_score.Series.Add(series3);
            this.chr_display_score.Series.Add(series4);
            this.chr_display_score.Size = new System.Drawing.Size(509, 171);
            this.chr_display_score.TabIndex = 10;
            this.chr_display_score.Text = "chart1";
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_show_breakdown.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_show_breakdown.ColumnHeadersHeight = 25;
            this.dgv_show_breakdown.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv_show_breakdown.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.section,
            this.number,
            this.accuracy});
            this.dgv_show_breakdown.Location = new System.Drawing.Point(271, 445);
            this.dgv_show_breakdown.Name = "dgv_show_breakdown";
            this.dgv_show_breakdown.ReadOnly = true;
            this.dgv_show_breakdown.RowHeadersVisible = false;
            this.dgv_show_breakdown.Size = new System.Drawing.Size(495, 233);
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
            this.accuracy.HeaderText = "Accuracy";
            this.accuracy.Name = "accuracy";
            this.accuracy.ReadOnly = true;
            this.accuracy.Width = 75;
            // 
            // lbl_candidate_name
            // 
            this.lbl_candidate_name.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_candidate_name.AutoSize = true;
            this.lbl_candidate_name.Location = new System.Drawing.Point(372, 92);
            this.lbl_candidate_name.Name = "lbl_candidate_name";
            this.lbl_candidate_name.Size = new System.Drawing.Size(35, 13);
            this.lbl_candidate_name.TabIndex = 12;
            this.lbl_candidate_name.Text = "label7";
            // 
            // lbl_date
            // 
            this.lbl_date.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_date.AutoSize = true;
            this.lbl_date.Location = new System.Drawing.Point(339, 125);
            this.lbl_date.Name = "lbl_date";
            this.lbl_date.Size = new System.Drawing.Size(35, 13);
            this.lbl_date.TabIndex = 13;
            this.lbl_date.Text = "label8";
            // 
            // lbl_exam_number
            // 
            this.lbl_exam_number.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_exam_number.AutoSize = true;
            this.lbl_exam_number.Location = new System.Drawing.Point(373, 157);
            this.lbl_exam_number.Name = "lbl_exam_number";
            this.lbl_exam_number.Size = new System.Drawing.Size(35, 13);
            this.lbl_exam_number.TabIndex = 14;
            this.lbl_exam_number.Text = "label9";
            // 
            // lbl_elapsed_time
            // 
            this.lbl_elapsed_time.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_elapsed_time.AutoSize = true;
            this.lbl_elapsed_time.Location = new System.Drawing.Point(668, 125);
            this.lbl_elapsed_time.Name = "lbl_elapsed_time";
            this.lbl_elapsed_time.Size = new System.Drawing.Size(41, 13);
            this.lbl_elapsed_time.TabIndex = 16;
            this.lbl_elapsed_time.Text = "label11";
            this.lbl_elapsed_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_time
            // 
            this.lbl_time.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_time.AutoSize = true;
            this.lbl_time.Location = new System.Drawing.Point(666, 92);
            this.lbl_time.Name = "lbl_time";
            this.lbl_time.Size = new System.Drawing.Size(41, 13);
            this.lbl_time.TabIndex = 15;
            this.lbl_time.Text = "label12";
            this.lbl_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(436, 389);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "STATUS:";
            // 
            // lbl_status
            // 
            this.lbl_status.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_status.AutoSize = true;
            this.lbl_status.Location = new System.Drawing.Point(495, 389);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(35, 13);
            this.lbl_status.TabIndex = 18;
            this.lbl_status.Text = "label8";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(698, 92);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "min(s)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(699, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "min(s)";
            // 
            // Score_Sheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1058, 727);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbl_elapsed_time);
            this.Controls.Add(this.lbl_time);
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
            this.Name = "Score_Sheet";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Score Sheet";
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
        private System.Windows.Forms.Label lbl_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn section;
        private System.Windows.Forms.DataGridViewTextBoxColumn number;
        private System.Windows.Forms.DataGridViewTextBoxColumn accuracy;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}