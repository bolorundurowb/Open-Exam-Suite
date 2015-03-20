namespace Creator
{
    partial class EditSectionTitle
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_new_title = new System.Windows.Forms.TextBox();
            this.btn_edit_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "New Section Title:";
            // 
            // txt_new_title
            // 
            this.txt_new_title.Location = new System.Drawing.Point(109, 14);
            this.txt_new_title.Name = "txt_new_title";
            this.txt_new_title.Size = new System.Drawing.Size(183, 20);
            this.txt_new_title.TabIndex = 1;
            // 
            // btn_edit_ok
            // 
            this.btn_edit_ok.Location = new System.Drawing.Point(307, 11);
            this.btn_edit_ok.Name = "btn_edit_ok";
            this.btn_edit_ok.Size = new System.Drawing.Size(54, 23);
            this.btn_edit_ok.TabIndex = 2;
            this.btn_edit_ok.Text = "OK";
            this.btn_edit_ok.UseVisualStyleBackColor = true;
            this.btn_edit_ok.Click += new System.EventHandler(this.btn_edit_ok_Click);
            // 
            // EditSectionTitle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 50);
            this.ControlBox = false;
            this.Controls.Add(this.btn_edit_ok);
            this.Controls.Add(this.txt_new_title);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EditSectionTitle";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Section Title";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_new_title;
        private System.Windows.Forms.Button btn_edit_ok;
    }
}