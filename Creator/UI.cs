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
        //Class variables
        Dictionary<string, Dictionary<int, Question>> examQuestions;
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
                btn_new_section.Enabled = true;
                //adding elements to treeview
                TreeNode examNode = new TreeNode();
                examNode.Name = "examNode";
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
                QuestionNode.Text = "Question 1";
                QuestionNode.ImageIndex = 1;
                QuestionNode.SelectedImageIndex = 1;
                examNode.Nodes[0].Nodes.Add(QuestionNode);      //The default question Node

                trv_explorer.ExpandAll();
                trv_explorer.SelectedNode = QuestionNode;
                //Initialize store for Questions
                this.examQuestions = new Dictionary<string, Dictionary<int, Question>>();
                //Enable Question fillout mode
                splcn_main_view.Panel2.Enabled = true;
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
            if (Properties.Settings.Default.SectionTitles != null)
            {
                foreach (string s in Properties.Settings.Default.SectionTitles)
                {
                    bool temp = string.IsNullOrWhiteSpace(s);
                    five = five && temp;
                }
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

        private void trv_explorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //enable add options
            if (((TreeView)sender).SelectedNode.Name.Contains("ques"))
            {
                addOptionToolStripMenuItem.Enabled = true;
                newQuestionToolStripMenuItem.Enabled = false;
                btn_new_question.Enabled = false;
                newSectionToolStripMenuItem.Enabled = false;
                btn_new_section.Enabled = false;
            }
            //enable add questions
            else if (((TreeView)sender).SelectedNode.Name.Contains("secNode"))
            {
                newQuestionToolStripMenuItem.Enabled = true;
                btn_new_question.Enabled = true;
                newSectionToolStripMenuItem.Enabled = false;
                btn_new_section.Enabled = false;
            }
            //enable add sections
            else if (((TreeView)sender).SelectedNode.Name.Contains("examNode"))
            {
                newSectionToolStripMenuItem.Enabled = true;
                btn_new_section.Enabled = true;
                newQuestionToolStripMenuItem.Enabled = false;
                btn_new_question.Enabled = false;
            }
            else
            {
                addOptionToolStripMenuItem.Enabled = false;
                newQuestionToolStripMenuItem.Enabled = false;
                btn_new_question.Enabled = false;
                newSectionToolStripMenuItem.Enabled = false;
                btn_new_section.Enabled = false;
            }
        }

        private void addOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionControl ctrl = new OptionControl();
            if (pan_options.Controls.Count != 0)
            {
                OptionControl info = (OptionControl)pan_options.Controls[pan_options.Controls.OfType<OptionControl>().Count() - 1];
                ctrl.Location = new Point(info.Location.X, info.Location.Y + 35);
                int num = Convert.ToInt32(info.Name.Replace("optionControl", ""));
                ctrl.Name = "optionControl" + (num + 1);
                ctrl.OptionLetter = (char)(Convert.ToInt32(info.OptionLetter) + 1);
            }
            else
            {
                ctrl.Location = new Point(0, 0);
                ctrl.Name = "optionControl1";
                ctrl.OptionLetter = 'A';
            }
            pan_options_ControlChanged(btn_add_option, null);
            pan_options.Controls.Add(ctrl);
        }

        private void btn_remove_option_Click(object sender, EventArgs e)
        {
            pan_options.Controls.Remove(pan_options.Controls.OfType<OptionControl>().ElementAt(pan_options.Controls.OfType<OptionControl>().Count() - 1));
            //pan_options.Controls.OfType<OptionControl>().Count() - 1
        }

        private void insertPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btn_clear_picture.Visible = true;
            btn_select_picture.Visible = true;
            pct_question_picture.Visible = true;
        }

        private void btn_clear_picture_Click(object sender, EventArgs e)
        {
            pct_question_picture.Image = null;
        }

        private void pan_options_ControlChanged(object sender, ControlEventArgs e)
        {
            if (pan_options.Controls.Count <= 0)
            {
                btn_remove_option.Enabled = false;
            }
            else
            {
                btn_remove_option.Enabled = true;
            }
        }

        private void UI_Load(object sender, EventArgs e)
        {
            if (pan_options.Controls.Count > 0)
            {
                btn_remove_option.Enabled = true;
            }
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
        }

        private void AddQuestion(object sender, EventArgs e)
        {

        }
    }
}
