namespace Creator
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
        private void InitializeComponent()
        {
            this.rdb_option = new System.Windows.Forms.RadioButton();
            this.txt_option = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // rdb_option
            // 
            this.rdb_option.AutoCheck = false;
            this.rdb_option.AutoSize = true;
            this.rdb_option.Location = new System.Drawing.Point(8, 8);
            this.rdb_option.Name = "rdb_option";
            this.rdb_option.Size = new System.Drawing.Size(32, 17);
            this.rdb_option.TabIndex = 0;
            this.rdb_option.Text = "Z";
            this.rdb_option.UseVisualStyleBackColor = true;
            this.rdb_option.CheckedChanged += new System.EventHandler(this.rdb_option_CheckedChanged);
            this.rdb_option.Click += new System.EventHandler(this.rdb_option_Click);
            // 
            // txt_option
            // 
            this.txt_option.Location = new System.Drawing.Point(46, 7);
            this.txt_option.Name = "txt_option";
            this.txt_option.Size = new System.Drawing.Size(254, 20);
            this.txt_option.TabIndex = 1;
            // 
            // OptionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt_option);
            this.Controls.Add(this.rdb_option);
            this.Name = "OptionControl";
            this.Size = new System.Drawing.Size(316, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdb_option;
        private System.Windows.Forms.TextBox txt_option;
    }
}
