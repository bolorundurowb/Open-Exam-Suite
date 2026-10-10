namespace OpenExamSuite.Creator.GUI.Dialogs
{
    partial class AddSection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddSection));
            this.label1 = new Label();
            this.txt_title = new TextBox();
            this.btn_add_section = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(10, 14);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(33, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title:";
            // 
            // txt_title
            // 
            this.txt_title.Location = new Point(48, 10);
            this.txt_title.Margin = new Padding(4, 3, 4, 3);
            this.txt_title.Name = "txt_title";
            this.txt_title.Size = new Size(271, 23);
            this.txt_title.TabIndex = 1;
            // 
            // btn_add_section
            // 
            this.btn_add_section.Image = (Image)resources.GetObject("btn_add_section.Image");
            this.btn_add_section.Location = new Point(327, 8);
            this.btn_add_section.Margin = new Padding(4, 3, 4, 3);
            this.btn_add_section.Name = "btn_add_section";
            this.btn_add_section.Size = new Size(62, 27);
            this.btn_add_section.TabIndex = 2;
            this.btn_add_section.Text = "Add";
            this.btn_add_section.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.btn_add_section.UseVisualStyleBackColor = true;
            this.btn_add_section.Click += this.btn_add_section_Click;
            // 
            // AddSection
            // 
            this.AcceptButton = this.btn_add_section;
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(397, 43);
            this.Controls.Add(this.btn_add_section);
            this.Controls.Add(this.txt_title);
            this.Controls.Add(this.label1);
            this.Margin = new Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddSection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add Section";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_title;
        private System.Windows.Forms.Button btn_add_section;
    }
}