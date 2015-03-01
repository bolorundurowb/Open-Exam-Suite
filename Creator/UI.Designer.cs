namespace Creator
{
    partial class UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.examToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addOptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newQuestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutCreatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btn_new_exam = new System.Windows.Forms.ToolStripButton();
            this.btn_open_exam = new System.Windows.Forms.ToolStripButton();
            this.btn_save_exam = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_print_exam = new System.Windows.Forms.ToolStripButton();
            this.btn_print_preview = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_undo = new System.Windows.Forms.ToolStripButton();
            this.btn_redo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_cut = new System.Windows.Forms.ToolStripButton();
            this.btn_copy = new System.Windows.Forms.ToolStripButton();
            this.btn_paste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_new_section = new System.Windows.Forms.ToolStripButton();
            this.btn_new_question = new System.Windows.Forms.ToolStripButton();
            this.splcn_main_view = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trv_explorer = new System.Windows.Forms.TreeView();
            this.ilst_images = new System.Windows.Forms.ImageList(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_clear_picture = new System.Windows.Forms.Button();
            this.btn_select_picture = new System.Windows.Forms.Button();
            this.pct_question_picture = new System.Windows.Forms.PictureBox();
            this.btn_remove_option = new System.Windows.Forms.Button();
            this.btn_add_option = new System.Windows.Forms.Button();
            this.pan_options = new System.Windows.Forms.Panel();
            this.txt_question_text = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.svf_save_exam = new System.Windows.Forms.SaveFileDialog();
            this.opf_get_files = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splcn_main_view)).BeginInit();
            this.splcn_main_view.Panel1.SuspendLayout();
            this.splcn_main_view.Panel2.SuspendLayout();
            this.splcn_main_view.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pct_question_picture)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.examToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1241, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.importToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.printToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::Creator.Properties.Resources.New_Logo;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.btn_new_exam_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::Creator.Properties.Resources.Open_Logo;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Enabled = false;
            this.importToolStripMenuItem.Image = global::Creator.Properties.Resources.Import_Logo;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Enabled = false;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = global::Creator.Properties.Resources.Save_Logo;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Enabled = false;
            this.printToolStripMenuItem.Image = global::Creator.Properties.Resources.Print_Logo;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.printToolStripMenuItem.Text = "Print";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::Creator.Properties.Resources.Exit_Logo;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.insertPictureToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Image = global::Creator.Properties.Resources.Undo_Logo;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Image = global::Creator.Properties.Resources.Redo_logo;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            // 
            // insertPictureToolStripMenuItem
            // 
            this.insertPictureToolStripMenuItem.Enabled = false;
            this.insertPictureToolStripMenuItem.Name = "insertPictureToolStripMenuItem";
            this.insertPictureToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.insertPictureToolStripMenuItem.Text = "Insert Picture";
            this.insertPictureToolStripMenuItem.Click += new System.EventHandler(this.insertPictureToolStripMenuItem_Click);
            // 
            // examToolStripMenuItem
            // 
            this.examToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addOptionToolStripMenuItem,
            this.newQuestionToolStripMenuItem,
            this.newSectionToolStripMenuItem,
            this.previousItemToolStripMenuItem,
            this.nextItemToolStripMenuItem});
            this.examToolStripMenuItem.Name = "examToolStripMenuItem";
            this.examToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.examToolStripMenuItem.Text = "Exam";
            // 
            // addOptionToolStripMenuItem
            // 
            this.addOptionToolStripMenuItem.Enabled = false;
            this.addOptionToolStripMenuItem.Name = "addOptionToolStripMenuItem";
            this.addOptionToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.addOptionToolStripMenuItem.Text = "Add Option";
            this.addOptionToolStripMenuItem.Click += new System.EventHandler(this.addOptionToolStripMenuItem_Click);
            // 
            // newQuestionToolStripMenuItem
            // 
            this.newQuestionToolStripMenuItem.Enabled = false;
            this.newQuestionToolStripMenuItem.Image = global::Creator.Properties.Resources.New_Question_Logo;
            this.newQuestionToolStripMenuItem.Name = "newQuestionToolStripMenuItem";
            this.newQuestionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newQuestionToolStripMenuItem.Text = "New Question";
            this.newQuestionToolStripMenuItem.Click += new System.EventHandler(this.AddQuestion);
            // 
            // newSectionToolStripMenuItem
            // 
            this.newSectionToolStripMenuItem.Enabled = false;
            this.newSectionToolStripMenuItem.Image = global::Creator.Properties.Resources.New_Section_Logo;
            this.newSectionToolStripMenuItem.Name = "newSectionToolStripMenuItem";
            this.newSectionToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.newSectionToolStripMenuItem.Text = "New Section";
            // 
            // previousItemToolStripMenuItem
            // 
            this.previousItemToolStripMenuItem.Enabled = false;
            this.previousItemToolStripMenuItem.Image = global::Creator.Properties.Resources.Previous_Logo;
            this.previousItemToolStripMenuItem.Name = "previousItemToolStripMenuItem";
            this.previousItemToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.previousItemToolStripMenuItem.Text = "Previous Item";
            // 
            // nextItemToolStripMenuItem
            // 
            this.nextItemToolStripMenuItem.Enabled = false;
            this.nextItemToolStripMenuItem.Image = global::Creator.Properties.Resources.Next_Logo;
            this.nextItemToolStripMenuItem.Name = "nextItemToolStripMenuItem";
            this.nextItemToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.nextItemToolStripMenuItem.Text = "Next Item";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutCreatorToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutCreatorToolStripMenuItem
            // 
            this.aboutCreatorToolStripMenuItem.Name = "aboutCreatorToolStripMenuItem";
            this.aboutCreatorToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.aboutCreatorToolStripMenuItem.Text = "About Creator";
            this.aboutCreatorToolStripMenuItem.Click += new System.EventHandler(this.aboutCreatorToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_new_exam,
            this.btn_open_exam,
            this.btn_save_exam,
            this.toolStripSeparator1,
            this.btn_print_exam,
            this.btn_print_preview,
            this.toolStripSeparator2,
            this.btn_undo,
            this.btn_redo,
            this.toolStripSeparator3,
            this.btn_cut,
            this.btn_copy,
            this.btn_paste,
            this.toolStripSeparator4,
            this.btn_new_section,
            this.btn_new_question});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1241, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btn_new_exam
            // 
            this.btn_new_exam.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_new_exam.Image = global::Creator.Properties.Resources.New_Logo;
            this.btn_new_exam.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_new_exam.Name = "btn_new_exam";
            this.btn_new_exam.Size = new System.Drawing.Size(23, 22);
            this.btn_new_exam.Text = "New Exam";
            this.btn_new_exam.Click += new System.EventHandler(this.btn_new_exam_Click);
            // 
            // btn_open_exam
            // 
            this.btn_open_exam.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_open_exam.Image = global::Creator.Properties.Resources.Open_Logo;
            this.btn_open_exam.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_open_exam.Name = "btn_open_exam";
            this.btn_open_exam.Size = new System.Drawing.Size(23, 22);
            this.btn_open_exam.Text = "Open Existing Exam";
            // 
            // btn_save_exam
            // 
            this.btn_save_exam.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_save_exam.Enabled = false;
            this.btn_save_exam.Image = global::Creator.Properties.Resources.Save_Logo;
            this.btn_save_exam.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_save_exam.Name = "btn_save_exam";
            this.btn_save_exam.Size = new System.Drawing.Size(23, 22);
            this.btn_save_exam.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_print_exam
            // 
            this.btn_print_exam.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_print_exam.Enabled = false;
            this.btn_print_exam.Image = global::Creator.Properties.Resources.Print_Logo;
            this.btn_print_exam.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_print_exam.Name = "btn_print_exam";
            this.btn_print_exam.Size = new System.Drawing.Size(23, 22);
            this.btn_print_exam.Text = "Print";
            // 
            // btn_print_preview
            // 
            this.btn_print_preview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_print_preview.Enabled = false;
            this.btn_print_preview.Image = global::Creator.Properties.Resources.Print_Preview_Logo;
            this.btn_print_preview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_print_preview.Name = "btn_print_preview";
            this.btn_print_preview.Size = new System.Drawing.Size(23, 22);
            this.btn_print_preview.Text = "Print Preview";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_undo
            // 
            this.btn_undo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_undo.Enabled = false;
            this.btn_undo.Image = global::Creator.Properties.Resources.Undo_Logo;
            this.btn_undo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_undo.Name = "btn_undo";
            this.btn_undo.Size = new System.Drawing.Size(23, 22);
            this.btn_undo.Text = "Undo";
            // 
            // btn_redo
            // 
            this.btn_redo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_redo.Enabled = false;
            this.btn_redo.Image = global::Creator.Properties.Resources.Redo_logo;
            this.btn_redo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_redo.Name = "btn_redo";
            this.btn_redo.Size = new System.Drawing.Size(23, 22);
            this.btn_redo.Text = "Redo";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_cut
            // 
            this.btn_cut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_cut.Enabled = false;
            this.btn_cut.Image = global::Creator.Properties.Resources.Cut_Logo;
            this.btn_cut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_cut.Name = "btn_cut";
            this.btn_cut.Size = new System.Drawing.Size(23, 22);
            this.btn_cut.Text = "toolStripButton1";
            // 
            // btn_copy
            // 
            this.btn_copy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_copy.Enabled = false;
            this.btn_copy.Image = global::Creator.Properties.Resources.Copy_Logo;
            this.btn_copy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_copy.Name = "btn_copy";
            this.btn_copy.Size = new System.Drawing.Size(23, 22);
            this.btn_copy.Text = "toolStripButton2";
            // 
            // btn_paste
            // 
            this.btn_paste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_paste.Enabled = false;
            this.btn_paste.Image = global::Creator.Properties.Resources.Paste_Logo;
            this.btn_paste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_paste.Name = "btn_paste";
            this.btn_paste.Size = new System.Drawing.Size(23, 22);
            this.btn_paste.Text = "toolStripButton3";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_new_section
            // 
            this.btn_new_section.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_new_section.Enabled = false;
            this.btn_new_section.Image = global::Creator.Properties.Resources.New_Section_Logo;
            this.btn_new_section.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_new_section.Name = "btn_new_section";
            this.btn_new_section.Size = new System.Drawing.Size(23, 22);
            this.btn_new_section.Text = "Add Section";
            // 
            // btn_new_question
            // 
            this.btn_new_question.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_new_question.Enabled = false;
            this.btn_new_question.Image = global::Creator.Properties.Resources.New_Question_Logo;
            this.btn_new_question.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_new_question.Name = "btn_new_question";
            this.btn_new_question.Size = new System.Drawing.Size(23, 22);
            this.btn_new_question.Text = "Add Question";
            this.btn_new_question.Click += new System.EventHandler(this.AddQuestion);
            // 
            // splcn_main_view
            // 
            this.splcn_main_view.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splcn_main_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splcn_main_view.Location = new System.Drawing.Point(0, 49);
            this.splcn_main_view.Name = "splcn_main_view";
            // 
            // splcn_main_view.Panel1
            // 
            this.splcn_main_view.Panel1.Controls.Add(this.splitContainer2);
            this.splcn_main_view.Panel1MinSize = 200;
            // 
            // splcn_main_view.Panel2
            // 
            this.splcn_main_view.Panel2.Controls.Add(this.btn_clear_picture);
            this.splcn_main_view.Panel2.Controls.Add(this.btn_select_picture);
            this.splcn_main_view.Panel2.Controls.Add(this.pct_question_picture);
            this.splcn_main_view.Panel2.Controls.Add(this.btn_remove_option);
            this.splcn_main_view.Panel2.Controls.Add(this.btn_add_option);
            this.splcn_main_view.Panel2.Controls.Add(this.pan_options);
            this.splcn_main_view.Panel2.Controls.Add(this.txt_question_text);
            this.splcn_main_view.Panel2.Controls.Add(this.label1);
            this.splcn_main_view.Panel2.Enabled = false;
            this.splcn_main_view.Panel2MinSize = 500;
            this.splcn_main_view.Size = new System.Drawing.Size(1241, 712);
            this.splcn_main_view.SplitterDistance = 240;
            this.splcn_main_view.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel1MinSize = 300;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Panel2MinSize = 250;
            this.splitContainer2.Size = new System.Drawing.Size(240, 712);
            this.splitContainer2.SplitterDistance = 306;
            this.splitContainer2.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trv_explorer);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 304);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Explorer";
            // 
            // trv_explorer
            // 
            this.trv_explorer.BackColor = System.Drawing.SystemColors.Control;
            this.trv_explorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trv_explorer.HideSelection = false;
            this.trv_explorer.ImageIndex = 0;
            this.trv_explorer.ImageList = this.ilst_images;
            this.trv_explorer.Location = new System.Drawing.Point(3, 19);
            this.trv_explorer.Name = "trv_explorer";
            this.trv_explorer.SelectedImageIndex = 0;
            this.trv_explorer.Size = new System.Drawing.Size(232, 282);
            this.trv_explorer.TabIndex = 0;
            this.trv_explorer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trv_explorer_AfterSelect);
            // 
            // ilst_images
            // 
            this.ilst_images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilst_images.ImageStream")));
            this.ilst_images.TransparentColor = System.Drawing.Color.Transparent;
            this.ilst_images.Images.SetKeyName(0, "New Section Logo.png");
            this.ilst_images.Images.SetKeyName(1, "New Question Logo.png");
            this.ilst_images.Images.SetKeyName(2, "Blank.fw.png");
            // 
            // groupBox2
            // 
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(238, 400);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Properties";
            // 
            // btn_clear_picture
            // 
            this.btn_clear_picture.Location = new System.Drawing.Point(101, 224);
            this.btn_clear_picture.Name = "btn_clear_picture";
            this.btn_clear_picture.Size = new System.Drawing.Size(75, 23);
            this.btn_clear_picture.TabIndex = 7;
            this.btn_clear_picture.Text = "Clear";
            this.btn_clear_picture.UseVisualStyleBackColor = true;
            this.btn_clear_picture.Visible = false;
            this.btn_clear_picture.Click += new System.EventHandler(this.btn_clear_picture_Click);
            // 
            // btn_select_picture
            // 
            this.btn_select_picture.Location = new System.Drawing.Point(101, 194);
            this.btn_select_picture.Name = "btn_select_picture";
            this.btn_select_picture.Size = new System.Drawing.Size(75, 23);
            this.btn_select_picture.TabIndex = 6;
            this.btn_select_picture.Text = "Select";
            this.btn_select_picture.UseVisualStyleBackColor = true;
            this.btn_select_picture.Visible = false;
            // 
            // pct_question_picture
            // 
            this.pct_question_picture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pct_question_picture.Location = new System.Drawing.Point(182, 194);
            this.pct_question_picture.Name = "pct_question_picture";
            this.pct_question_picture.Size = new System.Drawing.Size(285, 169);
            this.pct_question_picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pct_question_picture.TabIndex = 5;
            this.pct_question_picture.TabStop = false;
            this.pct_question_picture.Visible = false;
            // 
            // btn_remove_option
            // 
            this.btn_remove_option.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_remove_option.Enabled = false;
            this.btn_remove_option.Location = new System.Drawing.Point(636, 465);
            this.btn_remove_option.Name = "btn_remove_option";
            this.btn_remove_option.Size = new System.Drawing.Size(75, 23);
            this.btn_remove_option.TabIndex = 4;
            this.btn_remove_option.Text = "Remove";
            this.btn_remove_option.UseVisualStyleBackColor = true;
            this.btn_remove_option.Click += new System.EventHandler(this.btn_remove_option_Click);
            // 
            // btn_add_option
            // 
            this.btn_add_option.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_add_option.Location = new System.Drawing.Point(636, 435);
            this.btn_add_option.Name = "btn_add_option";
            this.btn_add_option.Size = new System.Drawing.Size(75, 23);
            this.btn_add_option.TabIndex = 3;
            this.btn_add_option.Text = "Add";
            this.btn_add_option.UseVisualStyleBackColor = true;
            this.btn_add_option.Click += new System.EventHandler(this.addOptionToolStripMenuItem_Click);
            // 
            // pan_options
            // 
            this.pan_options.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pan_options.AutoScroll = true;
            this.pan_options.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pan_options.Location = new System.Drawing.Point(269, 435);
            this.pan_options.Name = "pan_options";
            this.pan_options.Size = new System.Drawing.Size(352, 252);
            this.pan_options.TabIndex = 2;
            this.pan_options.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.pan_options_ControlChanged);
            this.pan_options.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.pan_options_ControlChanged);
            // 
            // txt_question_text
            // 
            this.txt_question_text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_question_text.Location = new System.Drawing.Point(49, 43);
            this.txt_question_text.Multiline = true;
            this.txt_question_text.Name = "txt_question_text";
            this.txt_question_text.Size = new System.Drawing.Size(895, 118);
            this.txt_question_text.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter question details:";
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 761);
            this.Controls.Add(this.splcn_main_view);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "UI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CREATOR";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UI_FormClosing);
            this.Load += new System.EventHandler(this.UI_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splcn_main_view.Panel1.ResumeLayout(false);
            this.splcn_main_view.Panel2.ResumeLayout(false);
            this.splcn_main_view.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splcn_main_view)).EndInit();
            this.splcn_main_view.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pct_question_picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem examToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addOptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newQuestionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutCreatorToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_new_exam;
        private System.Windows.Forms.ToolStripButton btn_open_exam;
        private System.Windows.Forms.ToolStripButton btn_save_exam;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btn_print_exam;
        private System.Windows.Forms.ToolStripButton btn_print_preview;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btn_undo;
        private System.Windows.Forms.ToolStripButton btn_redo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.SplitContainer splcn_main_view;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView trv_explorer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_remove_option;
        private System.Windows.Forms.Button btn_add_option;
        private System.Windows.Forms.Panel pan_options;
        private System.Windows.Forms.TextBox txt_question_text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SaveFileDialog svf_save_exam;
        private System.Windows.Forms.OpenFileDialog opf_get_files;
        private System.Windows.Forms.ToolStripButton btn_cut;
        private System.Windows.Forms.ToolStripButton btn_copy;
        private System.Windows.Forms.ToolStripButton btn_paste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ImageList ilst_images;
        private System.Windows.Forms.PictureBox pct_question_picture;
        private System.Windows.Forms.Button btn_clear_picture;
        private System.Windows.Forms.Button btn_select_picture;
        private System.Windows.Forms.ToolStripButton btn_new_section;
        private System.Windows.Forms.ToolStripButton btn_new_question;
    }
}

