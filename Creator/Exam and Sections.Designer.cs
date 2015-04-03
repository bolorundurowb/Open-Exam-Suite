namespace Creator
{
    partial class Exam_and_Sections
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Exam_and_Sections));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.num_time_limit = new System.Windows.Forms.NumericUpDown();
            this.num_passing_score = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_next = new System.Windows.Forms.Button();
            this.txt_exam_instructions = new System.Windows.Forms.TextBox();
            this.txt_exam_code = new System.Windows.Forms.TextBox();
            this.txt_exam_title = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbl_file_version = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_move_down = new System.Windows.Forms.Button();
            this.btn_move_up = new System.Windows.Forms.Button();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_rename = new System.Windows.Forms.Button();
            this.btn_new = new System.Windows.Forms.Button();
            this.dgv_section_titles = new System.Windows.Forms.DataGridView();
            this.sectionTitile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.err_exam_wizard = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_time_limit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_passing_score)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_section_titles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.err_exam_wizard)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(519, 500);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.num_time_limit);
            this.tabPage1.Controls.Add(this.num_passing_score);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.btn_next);
            this.tabPage1.Controls.Add(this.txt_exam_instructions);
            this.tabPage1.Controls.Add(this.txt_exam_code);
            this.tabPage1.Controls.Add(this.txt_exam_title);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.lbl_file_version);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(511, 474);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Exam Properties";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(187, 154);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "minute(s)";
            // 
            // num_time_limit
            // 
            this.num_time_limit.Location = new System.Drawing.Point(112, 151);
            this.num_time_limit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_time_limit.Name = "num_time_limit";
            this.num_time_limit.Size = new System.Drawing.Size(71, 20);
            this.num_time_limit.TabIndex = 15;
            this.num_time_limit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_time_limit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // num_passing_score
            // 
            this.num_passing_score.Location = new System.Drawing.Point(112, 119);
            this.num_passing_score.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_passing_score.Name = "num_passing_score";
            this.num_passing_score.Size = new System.Drawing.Size(71, 20);
            this.num_passing_score.TabIndex = 14;
            this.num_passing_score.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(186, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "(on a scale of 1000)";
            // 
            // btn_next
            // 
            this.btn_next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_next.Image = global::Creator.Properties.Resources.Next;
            this.btn_next.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_next.Location = new System.Drawing.Point(419, 429);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(71, 23);
            this.btn_next.TabIndex = 12;
            this.btn_next.Text = "NEXT";
            this.btn_next.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // txt_exam_instructions
            // 
            this.txt_exam_instructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_exam_instructions.Location = new System.Drawing.Point(35, 213);
            this.txt_exam_instructions.Multiline = true;
            this.txt_exam_instructions.Name = "txt_exam_instructions";
            this.txt_exam_instructions.Size = new System.Drawing.Size(455, 195);
            this.txt_exam_instructions.TabIndex = 11;
            this.txt_exam_instructions.TextChanged += new System.EventHandler(this._TextChanged);
            // 
            // txt_exam_code
            // 
            this.txt_exam_code.Location = new System.Drawing.Point(98, 59);
            this.txt_exam_code.Name = "txt_exam_code";
            this.txt_exam_code.Size = new System.Drawing.Size(164, 20);
            this.txt_exam_code.TabIndex = 8;
            this.txt_exam_code.TextChanged += new System.EventHandler(this._TextChanged);
            // 
            // txt_exam_title
            // 
            this.txt_exam_title.Location = new System.Drawing.Point(98, 28);
            this.txt_exam_title.Name = "txt_exam_title";
            this.txt_exam_title.Size = new System.Drawing.Size(392, 20);
            this.txt_exam_title.TabIndex = 7;
            this.txt_exam_title.TextChanged += new System.EventHandler(this._TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Description/Instructions:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Time Limit:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Passing Score:";
            // 
            // lbl_file_version
            // 
            this.lbl_file_version.AutoSize = true;
            this.lbl_file_version.Location = new System.Drawing.Point(95, 93);
            this.lbl_file_version.Name = "lbl_file_version";
            this.lbl_file_version.Size = new System.Drawing.Size(22, 13);
            this.lbl_file_version.TabIndex = 3;
            this.lbl_file_version.Text = "1.0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "File Version:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Exam Code:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title:";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.btn_save);
            this.tabPage2.Controls.Add(this.btn_move_down);
            this.tabPage2.Controls.Add(this.btn_move_up);
            this.tabPage2.Controls.Add(this.btn_remove);
            this.tabPage2.Controls.Add(this.btn_rename);
            this.tabPage2.Controls.Add(this.btn_new);
            this.tabPage2.Controls.Add(this.dgv_section_titles);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(511, 474);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sections";
            // 
            // btn_save
            // 
            this.btn_save.Enabled = false;
            this.btn_save.Image = global::Creator.Properties.Resources._4;
            this.btn_save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_save.Location = new System.Drawing.Point(407, 434);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(90, 23);
            this.btn_save.TabIndex = 6;
            this.btn_save.Text = "SAVE";
            this.btn_save.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_move_down
            // 
            this.btn_move_down.Enabled = false;
            this.btn_move_down.Image = global::Creator.Properties.Resources.Down;
            this.btn_move_down.Location = new System.Drawing.Point(407, 134);
            this.btn_move_down.Name = "btn_move_down";
            this.btn_move_down.Size = new System.Drawing.Size(90, 23);
            this.btn_move_down.TabIndex = 5;
            this.btn_move_down.Text = "Move Down";
            this.btn_move_down.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_move_down.UseVisualStyleBackColor = true;
            this.btn_move_down.Click += new System.EventHandler(this.btn_move_down_Click);
            // 
            // btn_move_up
            // 
            this.btn_move_up.Enabled = false;
            this.btn_move_up.Image = global::Creator.Properties.Resources.Up;
            this.btn_move_up.Location = new System.Drawing.Point(407, 105);
            this.btn_move_up.Name = "btn_move_up";
            this.btn_move_up.Size = new System.Drawing.Size(90, 23);
            this.btn_move_up.TabIndex = 4;
            this.btn_move_up.Text = "Move Up";
            this.btn_move_up.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_move_up.UseVisualStyleBackColor = true;
            this.btn_move_up.Click += new System.EventHandler(this.btn_move_up_Click);
            // 
            // btn_remove
            // 
            this.btn_remove.Enabled = false;
            this.btn_remove.Image = global::Creator.Properties.Resources._2;
            this.btn_remove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_remove.Location = new System.Drawing.Point(407, 76);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(90, 23);
            this.btn_remove.TabIndex = 3;
            this.btn_remove.Text = "Remove";
            this.btn_remove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Click += new System.EventHandler(this.btn_remove_Click);
            // 
            // btn_rename
            // 
            this.btn_rename.Enabled = false;
            this.btn_rename.Image = global::Creator.Properties.Resources.Rename;
            this.btn_rename.Location = new System.Drawing.Point(407, 46);
            this.btn_rename.Name = "btn_rename";
            this.btn_rename.Size = new System.Drawing.Size(90, 23);
            this.btn_rename.TabIndex = 2;
            this.btn_rename.Text = "Rename";
            this.btn_rename.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_rename.UseVisualStyleBackColor = true;
            this.btn_rename.Click += new System.EventHandler(this.btn_rename_Click);
            // 
            // btn_new
            // 
            this.btn_new.Image = global::Creator.Properties.Resources.New;
            this.btn_new.Location = new System.Drawing.Point(407, 16);
            this.btn_new.Name = "btn_new";
            this.btn_new.Size = new System.Drawing.Size(90, 23);
            this.btn_new.TabIndex = 1;
            this.btn_new.Text = "New";
            this.btn_new.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_new.UseVisualStyleBackColor = true;
            this.btn_new.Click += new System.EventHandler(this.btn_new_Click);
            // 
            // dgv_section_titles
            // 
            this.dgv_section_titles.AllowUserToAddRows = false;
            this.dgv_section_titles.AllowUserToDeleteRows = false;
            this.dgv_section_titles.AllowUserToResizeColumns = false;
            this.dgv_section_titles.AllowUserToResizeRows = false;
            this.dgv_section_titles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_section_titles.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv_section_titles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_section_titles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sectionTitile});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DarkSlateBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_section_titles.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_section_titles.Location = new System.Drawing.Point(9, 10);
            this.dgv_section_titles.Name = "dgv_section_titles";
            this.dgv_section_titles.RowHeadersVisible = false;
            this.dgv_section_titles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_section_titles.Size = new System.Drawing.Size(385, 447);
            this.dgv_section_titles.TabIndex = 0;
            this.dgv_section_titles.SelectionChanged += new System.EventHandler(this.dgv_section_titles_SelectionChanged);
            // 
            // sectionTitile
            // 
            this.sectionTitile.HeaderText = "Section Title";
            this.sectionTitile.Name = "sectionTitile";
            this.sectionTitile.ReadOnly = true;
            // 
            // err_exam_wizard
            // 
            this.err_exam_wizard.ContainerControl = this;
            // 
            // Exam_and_Sections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 500);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Exam_and_Sections";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Exam Wizard";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_time_limit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_passing_score)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_section_titles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.err_exam_wizard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.NumericUpDown num_time_limit;
        private System.Windows.Forms.NumericUpDown num_passing_score;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_next;
        private System.Windows.Forms.TextBox txt_exam_instructions;
        private System.Windows.Forms.TextBox txt_exam_code;
        private System.Windows.Forms.TextBox txt_exam_title;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_file_version;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_move_down;
        private System.Windows.Forms.Button btn_move_up;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Button btn_rename;
        private System.Windows.Forms.Button btn_new;
        private System.Windows.Forms.DataGridView dgv_section_titles;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionTitile;
        private System.Windows.Forms.ErrorProvider err_exam_wizard;
    }
}