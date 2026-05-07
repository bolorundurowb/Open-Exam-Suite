namespace OpenExamSuite.Simulator.GUI
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
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeUi));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            this.menuStrip1 = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.addExamToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.helpToolStripMenuItem = new ToolStripMenuItem();
            this.aboutToolStripMenuItem = new ToolStripMenuItem();
            this.licenseToolStripMenuItem = new ToolStripMenuItem();
            this.dgv_exams = new DataGridView();
            this.name = new DataGridViewTextBoxColumn();
            this.path = new DataGridViewTextBoxColumn();
            this.ofd_exam = new OpenFileDialog();
            this.btn_properties = new Button();
            this.btn_remove = new Button();
            this.btn_add = new Button();
            this.btn_start = new Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.dgv_exams).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = Color.Transparent;
            this.menuStrip1.Items.AddRange(new ToolStripItem[] { this.fileToolStripMenuItem, this.helpToolStripMenuItem });
            this.menuStrip1.Location = new Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new Size(888, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.addExamToolStripMenuItem, this.toolStripSeparator1, this.exitToolStripMenuItem });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addExamToolStripMenuItem
            // 
            this.addExamToolStripMenuItem.Image = (Image)resources.GetObject("addExamToolStripMenuItem.Image");
            this.addExamToolStripMenuItem.Name = "addExamToolStripMenuItem";
            this.addExamToolStripMenuItem.Size = new Size(127, 22);
            this.addExamToolStripMenuItem.Text = "Add Exam";
            this.addExamToolStripMenuItem.Click += this.AddExam;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(124, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new Size(127, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += this.Exit;
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.aboutToolStripMenuItem, this.licenseToolStripMenuItem });
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new Size(113, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += this.About;
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new Size(113, 22);
            this.licenseToolStripMenuItem.Text = "License";
            this.licenseToolStripMenuItem.Click += this.License;
            // 
            // dgv_exams
            // 
            this.dgv_exams.AllowUserToAddRows = false;
            this.dgv_exams.AllowUserToDeleteRows = false;
            this.dgv_exams.AllowUserToResizeRows = false;
            this.dgv_exams.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgv_exams.BackgroundColor = SystemColors.Control;
            this.dgv_exams.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.dgv_exams.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_exams.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_exams.Columns.AddRange(new DataGridViewColumn[] { this.name, this.path });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.dgv_exams.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_exams.Location = new Point(139, 40);
            this.dgv_exams.Margin = new Padding(4, 3, 4, 3);
            this.dgv_exams.Name = "dgv_exams";
            this.dgv_exams.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            this.dgv_exams.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_exams.RowHeadersVisible = false;
            this.dgv_exams.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_exams.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv_exams.Size = new Size(735, 481);
            this.dgv_exams.TabIndex = 5;
            this.dgv_exams.CellDoubleClick += this.Start;
            this.dgv_exams.SelectionChanged += this.SelectionChanged;
            // 
            // name
            // 
            this.name.HeaderText = "Exam Name";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Width = 190;
            // 
            // path
            // 
            this.path.HeaderText = "Exam Path";
            this.path.MinimumWidth = 50;
            this.path.Name = "path";
            this.path.ReadOnly = true;
            this.path.Width = 540;
            // 
            // ofd_exam
            // 
            this.ofd_exam.Filter = "Open Exam Files (*.oef)|*.oef";
            this.ofd_exam.Multiselect = true;
            // 
            // btn_properties
            // 
            this.btn_properties.Enabled = false;
            this.btn_properties.Image = (Image)resources.GetObject("btn_properties.Image");
            this.btn_properties.Location = new Point(14, 187);
            this.btn_properties.Margin = new Padding(4, 3, 4, 3);
            this.btn_properties.Name = "btn_properties";
            this.btn_properties.Size = new Size(99, 27);
            this.btn_properties.TabIndex = 4;
            this.btn_properties.Text = "Properties";
            this.btn_properties.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_properties.UseVisualStyleBackColor = true;
            this.btn_properties.Click += this.Properties;
            // 
            // btn_remove
            // 
            this.btn_remove.Enabled = false;
            this.btn_remove.Image = (Image)resources.GetObject("btn_remove.Image");
            this.btn_remove.Location = new Point(14, 141);
            this.btn_remove.Margin = new Padding(4, 3, 4, 3);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new Size(99, 27);
            this.btn_remove.TabIndex = 3;
            this.btn_remove.Text = "Remove";
            this.btn_remove.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Click += this.Remove;
            // 
            // btn_add
            // 
            this.btn_add.Image = (Image)resources.GetObject("btn_add.Image");
            this.btn_add.Location = new Point(14, 93);
            this.btn_add.Margin = new Padding(4, 3, 4, 3);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new Size(99, 27);
            this.btn_add.TabIndex = 2;
            this.btn_add.Text = "Add";
            this.btn_add.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += this.AddExam;
            // 
            // btn_start
            // 
            this.btn_start.Enabled = false;
            this.btn_start.Image = (Image)resources.GetObject("btn_start.Image");
            this.btn_start.Location = new Point(14, 46);
            this.btn_start.Margin = new Padding(4, 3, 4, 3);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new Size(99, 27);
            this.btn_start.TabIndex = 1;
            this.btn_start.Text = "Start";
            this.btn_start.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += this.Start;
            // 
            // HomeUi
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.Control;
            this.ClientSize = new Size(888, 535);
            this.Controls.Add(this.dgv_exams);
            this.Controls.Add(this.btn_properties);
            this.Controls.Add(this.btn_remove);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.menuStrip1);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new Padding(4, 3, 4, 3);
            this.Name = "HomeUi";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Simulator";
            this.Shown += this.LoadAppData;
            this.SizeChanged += this.ChangeHeaderSize;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.dgv_exams).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Button btn_properties;
        private System.Windows.Forms.DataGridView dgv_exams;
        private System.Windows.Forms.OpenFileDialog ofd_exam;
        private System.Windows.Forms.ToolStripMenuItem addExamToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn path;
    }
}

