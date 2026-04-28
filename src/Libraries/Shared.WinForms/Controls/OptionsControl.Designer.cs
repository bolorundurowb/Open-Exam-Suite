namespace OpenExamSuite.Shared.Controls
{
    partial class OptionsControl
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
            this.chkLetter = new CheckBox();
            this.txtText = new TextBox();
            this.SuspendLayout();
            // 
            // chkLetter
            // 
            this.chkLetter.AutoSize = true;
            this.chkLetter.Location = new Point(10, 10);
            this.chkLetter.Margin = new Padding(4, 3, 4, 3);
            this.chkLetter.Name = "chkLetter";
            this.chkLetter.Size = new Size(33, 19);
            this.chkLetter.TabIndex = 0;
            this.chkLetter.Text = "Z";
            this.chkLetter.UseVisualStyleBackColor = true;
            // 
            // txtText
            // 
            this.txtText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txtText.Location = new Point(54, 9);
            this.txtText.Margin = new Padding(4, 3, 4, 3);
            this.txtText.Name = "txtText";
            this.txtText.Size = new Size(409, 23);
            this.txtText.TabIndex = 2;
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.chkLetter);
            this.Margin = new Padding(4, 3, 4, 3);
            this.Name = "OptionsControl";
            this.Size = new Size(475, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkLetter;
        private System.Windows.Forms.TextBox txtText;
    }
}
