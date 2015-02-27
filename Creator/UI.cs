using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Creator
{
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent();
        }

        private void btn_new_exam_Click(object sender, EventArgs e)
        {
            Exam_and_Sections exsec = new Exam_and_Sections();
            exsec.ShowDialog();
            if (IsValidExam())
            {
                //disable some functions
                btn_new_exam.Enabled = false;
                newToolStripMenuItem.Enabled = false;
                btn_open_exam.Enabled = false;
                openToolStripMenuItem.Enabled = false;
                //enable some others
                btn_save_exam.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                closeToolStripMenuItem.Enabled = true;
                insertPictureToolStripMenuItem.Enabled = true;
                newSectionToolStripMenuItem.Enabled = true;
                addOptionToolStripMenuItem.Enabled = true;
                //adding elements to treeview
                TreeNode examNode = new TreeNode();
                examNode.Text = Properties.Settings.Default.ExamTitle;
                examNode.ImageIndex = 2;
                examNode.SelectedImageIndex = 2;
                trv_explorer.Nodes.Add(examNode);           //Root node in TreeView

                for (int i = 0; i < Properties.Settings.Default.SectionTitles.Count; i++)
                {
                    TreeNode SectionNode = new TreeNode();
                    SectionNode.Name = "secNode_" + i;
                    SectionNode.Text = Properties.Settings.Default.SectionTitles[i];
                    SectionNode.ImageIndex = 0;
                    SectionNode.SelectedImageIndex = 0;
                    examNode.Nodes.Add(SectionNode);        //Section Nodes
                }

                TreeNode QuestionNode = new TreeNode();
                QuestionNode.Name = "quesNode_0";
                QuestionNode.Text = "Question";
                QuestionNode.ImageIndex = 1;
                QuestionNode.SelectedImageIndex = 1;
                examNode.Nodes[0].Nodes.Add(QuestionNode);      //The default question Node

                trv_explorer.ExpandAll();
                trv_explorer.SelectedNode = QuestionNode;
            }
        }

        private void aboutCreatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// Method to check if the new exam form was correctly filled
        /// </summary>
        /// <returns>a true or false value depending on correct form filling</returns>
        private bool IsValidExam()
        {
            bool one = string.IsNullOrWhiteSpace(Properties.Settings.Default.ExamTitle);
            bool two = string.IsNullOrWhiteSpace(Properties.Settings.Default.ExamCode);
            bool three = string.IsNullOrWhiteSpace(Properties.Settings.Default.FileVersion);
            bool four = string.IsNullOrWhiteSpace(Properties.Settings.Default.ExamInstructions);
            bool five = false;
            foreach (string s in Properties.Settings.Default.SectionTitles)
            {
                bool temp = string.IsNullOrWhiteSpace(s);
                five = five && temp;
            }
            if (!one && !two && !three && !four && !five)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
