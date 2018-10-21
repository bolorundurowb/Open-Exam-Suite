﻿namespace Creator.GUI.Dialogs
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
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txt_title = new System.Windows.Forms.TextBox();
            this.btn_add_section = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.num_numberOfQuestions = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.num_numberOfQuestions)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title:";
            // 
            // txt_title
            // 
            this.txt_title.Location = new System.Drawing.Point(41, 9);
            this.txt_title.Name = "txt_title";
            this.txt_title.Size = new System.Drawing.Size(233, 20);
            this.txt_title.TabIndex = 1;
            // 
            // btn_add_section
            // 
            this.btn_add_section.Image = global::Creator.Properties.Resources.ok;
            this.btn_add_section.Location = new System.Drawing.Point(280, 7);
            this.btn_add_section.Name = "btn_add_section";
            this.btn_add_section.Size = new System.Drawing.Size(53, 23);
            this.btn_add_section.TabIndex = 2;
            this.btn_add_section.Text = "Add";
            this.btn_add_section.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_add_section.UseVisualStyleBackColor = true;
            this.btn_add_section.Click += new System.EventHandler(this.btn_add_section_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Default number of questions:";
            // 
            // num_numberOfQuestions
            // 
            this.num_numberOfQuestions.Location = new System.Drawing.Point(157, 36);
            this.num_numberOfQuestions.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.num_numberOfQuestions.Name = "num_numberOfQuestions";
            this.num_numberOfQuestions.Size = new System.Drawing.Size(117, 20);
            this.num_numberOfQuestions.TabIndex = 4;
            // 
            // AddSection
            // 
            this.AcceptButton = this.btn_add_section;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 65);
            this.Controls.Add(this.num_numberOfQuestions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_add_section);
            this.Controls.Add(this.txt_title);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddSection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Section";
            ((System.ComponentModel.ISupportInitialize)(this.num_numberOfQuestions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_title;
        private System.Windows.Forms.Button btn_add_section;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_numberOfQuestions;
    }
}