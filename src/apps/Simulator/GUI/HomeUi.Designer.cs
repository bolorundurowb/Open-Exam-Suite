namespace Simulator.GUI
{
    partial class HomeUi
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeUi));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addExamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgv_exams = new System.Windows.Forms.DataGridView();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ofd_exam = new System.Windows.Forms.OpenFileDialog();
            this.btn_properties = new System.Windows.Forms.Button();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_add = new System.Windows.Forms.Button();
            this.btn_start = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_exams)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(761, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addExamToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addExamToolStripMenuItem
            // 
            this.addExamToolStripMenuItem.Image = global::Simulator.Properties.Resources._1;
            this.addExamToolStripMenuItem.Name = "addExamToolStripMenuItem";
            this.addExamToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.addExamToolStripMenuItem.Text = "Add Exam";
            this.addExamToolStripMenuItem.Click += new System.EventHandler(this.AddExam);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.About);
            // 
            // dgv_exams
            // 
            this.dgv_exams.AllowUserToAddRows = false;
            this.dgv_exams.AllowUserToDeleteRows = false;
            this.dgv_exams.AllowUserToResizeRows = false;
            this.dgv_exams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_exams.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv_exams.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_exams.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_exams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_exams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.name,
            this.path});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_exams.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_exams.Location = new System.Drawing.Point(119, 35);
            this.dgv_exams.Name = "dgv_exams";
            this.dgv_exams.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_exams.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_exams.RowHeadersVisible = false;
            this.dgv_exams.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_exams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_exams.Size = new System.Drawing.Size(630, 417);
            this.dgv_exams.TabIndex = 5;
            this.dgv_exams.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Start);
            this.dgv_exams.SelectionChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // name
            // 
            this.name.HeaderText = "Exam Name";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Width = 189;
            // 
            // path
            // 
            this.path.HeaderText = "Exam Path";
            this.path.Name = "path";
            this.path.ReadOnly = true;
            this.path.Width = 439;
            // 
            // ofd_exam
            // 
            this.ofd_exam.Filter = "Open Exam Files (*.oef)|*.oef";
            this.ofd_exam.Multiselect = true;
            // 
            // btn_properties
            // 
            this.btn_properties.Enabled = false;
            this.btn_properties.Image = global::Simulator.Properties.Resources._5;
            this.btn_properties.Location = new System.Drawing.Point(12, 162);
            this.btn_properties.Name = "btn_properties";
            this.btn_properties.Size = new System.Drawing.Size(85, 23);
            this.btn_properties.TabIndex = 4;
            this.btn_properties.Text = "Properties";
            this.btn_properties.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_properties.UseVisualStyleBackColor = true;
            this.btn_properties.Click += new System.EventHandler(this.Properties);
            // 
            // btn_remove
            // 
            this.btn_remove.Enabled = false;
            this.btn_remove.Image = global::Simulator.Properties.Resources._2;
            this.btn_remove.Location = new System.Drawing.Point(12, 122);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(85, 23);
            this.btn_remove.TabIndex = 3;
            this.btn_remove.Text = "Remove";
            this.btn_remove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Click += new System.EventHandler(this.Remove);
            // 
            // btn_add
            // 
            this.btn_add.Image = global::Simulator.Properties.Resources._1;
            this.btn_add.Location = new System.Drawing.Point(12, 81);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(85, 23);
            this.btn_add.TabIndex = 2;
            this.btn_add.Text = "Add";
            this.btn_add.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += new System.EventHandler(this.AddExam);
            // 
            // btn_start
            // 
            this.btn_start.Enabled = false;
            this.btn_start.Image = global::Simulator.Properties.Resources._4;
            this.btn_start.Location = new System.Drawing.Point(12, 40);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(85, 23);
            this.btn_start.TabIndex = 1;
            this.btn_start.Text = "Start";
            this.btn_start.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.Start);
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(761, 464);
            this.Controls.Add(this.dgv_exams);
            this.Controls.Add(this.btn_properties);
            this.Controls.Add(this.btn_remove);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Ui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simulator";
            this.Shown += new System.EventHandler(this.LoadAppData);
            this.SizeChanged += new System.EventHandler(this.ChangeHeaderSize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_exams)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Button btn_properties;
        private System.Windows.Forms.DataGridView dgv_exams;
        private System.Windows.Forms.OpenFileDialog ofd_exam;
        private System.Windows.Forms.ToolStripMenuItem addExamToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn path;
    }
}

