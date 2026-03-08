namespace Shared.Controls
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
        private void InitializeComponent()
        {
            this.chkLetter = new System.Windows.Forms.CheckBox();
            this.txtText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chkLetter
            // 
            this.chkLetter.AutoSize = true;
            this.chkLetter.Location = new System.Drawing.Point(9, 9);
            this.chkLetter.Name = "chkLetter";
            this.chkLetter.Size = new System.Drawing.Size(33, 17);
            this.chkLetter.TabIndex = 0;
            this.chkLetter.Text = "Z";
            this.chkLetter.UseVisualStyleBackColor = true;
            // 
            // txtText
            // 
            this.txtText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtText.Location = new System.Drawing.Point(46, 8);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(244, 20);
            this.txtText.TabIndex = 2;
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.chkLetter);
            this.Name = "OptionsControl";
            this.Size = new System.Drawing.Size(300, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkLetter;
        private System.Windows.Forms.TextBox txtText;
    }
}
