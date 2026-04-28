namespace OpenExamSuite.Shared.Controls
{
    partial class OptionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.rdb_letter = new RadioButton();
            this.txt_text = new TextBox();
            this.SuspendLayout();
            // 
            // rdb_letter
            // 
            this.rdb_letter.Anchor = AnchorStyles.Left;
            this.rdb_letter.AutoCheck = false;
            this.rdb_letter.AutoSize = true;
            this.rdb_letter.Location = new Point(9, 9);
            this.rdb_letter.Name = "rdb_letter";
            this.rdb_letter.Size = new Size(32, 19);
            this.rdb_letter.TabIndex = 0;
            this.rdb_letter.Text = "Z";
            this.rdb_letter.UseVisualStyleBackColor = true;
            this.rdb_letter.Click += this.rdb_letter_Click;
            // 
            // txt_text
            // 
            this.txt_text.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txt_text.Location = new Point(46, 8);
            this.txt_text.Name = "txt_text";
            this.txt_text.Size = new Size(419, 23);
            this.txt_text.TabIndex = 1;
            // 
            // OptionControl
            // 
            this.Controls.Add(this.txt_text);
            this.Controls.Add(this.rdb_letter);
            this.MinimumSize = new Size(120, 30);
            this.Name = "OptionControl";
            this.Size = new Size(475, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdb_letter;
        private System.Windows.Forms.TextBox txt_text;
    }
}
