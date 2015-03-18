using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;
using System.Diagnostics;

namespace Creator
{
    public partial class UI : Form
    {
        //Class variables
        Dictionary<Dictionary<int, Question>, string> examQuestions;
        List<Question> tempExamStore;
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
                this.examQuestions = new Dictionary<Dictionary<int, Question>, string>();
                this.tempExamStore = new List<Question>();
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
                splcn_main_view.Panel2.Enabled = true;
                try
                {
                    bool exists = false;
                    Question present = new Question();
                    try
                    {
                        var temp = tempExamStore.FindAll(s => s.SectionTitle == ((TreeView)sender).SelectedNode.Parent.Text);
                        present = temp.SingleOrDefault(c => c.QuestionNumber == Convert.ToInt32(((TreeView)sender).SelectedNode.Text.Replace("Question ", "")));
                        exists = tempExamStore.Contains(present);
                    }
                    catch (NullReferenceException ex)
                    {
                        //MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
                        Debug.Print("Error: " + ex.Message + ", Inner Exception: " + ex.InnerException);
                    }
                    finally
                    {
                        if (exists)
                        {
                            txt_question_text.Text = present.QuestionText;
                            try
                            {
                                pct_question_picture.Image = new Bitmap(present.QuestionImagePath);
                            }
                            catch (ArgumentNullException)
                            {
                                pct_question_picture.Image = null;
                            }
                            foreach (var item in present.QuestionOptions)
                            {
                                OptionControl ctrl = new OptionControl();
                                ctrl.OptionLetter = item.Key;
                                ctrl.OptionText = item.Value;
                                if (item.Key == present.QuestionAnswer)
                                {
                                    ctrl.IsChecked = true;
                                }
                                if (pan_options.Controls.Count != 0)
                                {
                                    OptionControl info = (OptionControl)pan_options.Controls[pan_options.Controls.OfType<OptionControl>().Count() - 1];
                                    ctrl.Location = new Point(info.Location.X, info.Location.Y + 35);
                                    int num = Convert.ToInt32(info.Name.Replace("optionControl", ""));
                                    ctrl.Name = "optionControl" + (num + 1);
                                }
                                else
                                {
                                    ctrl.Location = new Point(0, 0);
                                    ctrl.Name = "optionControl1";
                                }
                                pan_options_ControlChanged(btn_add_option, null);
                                pan_options.Controls.Add(ctrl);
                            }
                        }
                        else
                        { }
                    }
                }
                catch (NullReferenceException ex)
                {
                    //MessageBox.Show(ex.Message + " " + ex.InnerException + Environment.NewLine + ex.Source);
                    Debug.Print("Error: " + ex.Message + ", Inner Exception: " + ex.InnerException);
                }
                catch (ArgumentNullException ex)
                {
                    //MessageBox.Show(ex.Message + " " + ex.InnerException + Environment.NewLine + ex.Source);
                    Debug.Print("Error: " + ex.Message + ", Inner Exception: " + ex.InnerException);
                }
            }
            //enable add questions
            else if (((TreeView)sender).SelectedNode.Name.Contains("secNode"))
            {
                splcn_main_view.Panel2.Enabled = false;
                newQuestionToolStripMenuItem.Enabled = true;
                btn_new_question.Enabled = true;
                newSectionToolStripMenuItem.Enabled = false;
                btn_new_section.Enabled = false;
            }
            //enable add sections
            else if (((TreeView)sender).SelectedNode.Name.Contains("examNode"))
            {
                splcn_main_view.Panel2.Enabled = false;
                newSectionToolStripMenuItem.Enabled = true;
                btn_new_section.Enabled = true;
                newQuestionToolStripMenuItem.Enabled = false;
                btn_new_question.Enabled = false;
            }
            else
            {
                splcn_main_view.Panel2.Enabled = false;
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
            TreeNode node = new TreeNode();
            node.Name = "quesNode" + (trv_explorer.SelectedNode.Nodes.Count);
            node.Text = "Question " + (trv_explorer.SelectedNode.Nodes.Count + 1);
            node.ImageIndex = 1;
            node.SelectedImageIndex = 1;
            trv_explorer.SelectedNode.Nodes.Add(node);
            trv_explorer.ExpandAll();
        }

        private void AddSection(object sender, EventArgs e)
        {
            New_Section ns = new New_Section();
            ns.ShowDialog();
            string[] newSections = ns.Sections;
            foreach (string section in newSections)
            {
                TreeNode node = new TreeNode();
                node.Name = "secNode" + (trv_explorer.SelectedNode.Nodes.Count);
                node.Text = section;
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                trv_explorer.SelectedNode.Nodes.Add(node);
            }
        }

        private void trv_explorer_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                if (((TreeView)sender).SelectedNode.Name.Contains("ques"))
                {
                    bool exists = false;
                    Question present = new Question();
                    try
                    {
                        var temp = tempExamStore.FindAll(s => s.SectionTitle == ((TreeView)sender).SelectedNode.Parent.Text);
                        present = temp.SingleOrDefault(c => c.QuestionNumber == Convert.ToInt32(((TreeView)sender).SelectedNode.Text.Replace("Question ", "")));
                        exists = tempExamStore.Contains(present);
                    }
                    catch (InvalidOperationException ex)
                    {
                       Debug.Print(ex.Message + Environment.NewLine + ex.InnerException);
                    }
                    finally
                    {
                        if (tempExamStore.Contains(present))
                        {
                            int index = tempExamStore.IndexOf(present);
                            tempExamStore[index].QuestionText = txt_question_text.Text;
                            tempExamStore[index].QuestionImagePath = pct_question_picture.ImageLocation;
                            try
                            {
                                tempExamStore[index].QuestionAnswer = Convert.ToChar(((OptionControl)pan_options.Controls.OfType<OptionControl>().First<OptionControl>(p => p.IsChecked == true)).OptionLetter);
                            }
                            catch (InvalidOperationException)
                            {
                                tempExamStore[index].QuestionAnswer = 'A';
                            }
                            tempExamStore[index].QuestionNumber = Convert.ToInt32(((TreeView)sender).SelectedNode.Text.Replace("Question ", ""));
                            Dictionary<char, string> tempDic = new Dictionary<char, string>();
                            foreach (var ctrl in pan_options.Controls.OfType<OptionControl>())
                            {
                                tempDic.Add(Convert.ToChar(ctrl.OptionLetter), ctrl.OptionText);
                            }
                            tempExamStore[index].QuestionOptions = tempDic;
                            tempExamStore[index].SectionTitle = ((TreeView)sender).SelectedNode.Parent.Text;
                            //clear the boxes
                            txt_question_text.Clear();
                            pct_question_picture.Image = null;
                            pan_options.Controls.Clear();
                        }
                        else
                        {
                            //save the previous question
                            Question ques = new Question();
                            try
                            {
                                ques.QuestionAnswer = Convert.ToChar(((OptionControl)pan_options.Controls.OfType<OptionControl>().First<OptionControl>(p => p.IsChecked == true)).OptionLetter);
                            }
                            catch (InvalidOperationException)
                            {
                                ques.QuestionAnswer = 'A';
                            }
                            ques.QuestionImagePath = pct_question_picture.ImageLocation;
                            ques.QuestionNumber = Convert.ToInt32(((TreeView)sender).SelectedNode.Text.Replace("Question ", ""));
                            Dictionary<char, string> tempDic = new Dictionary<char, string>();
                            foreach (var ctrl in pan_options.Controls.OfType<OptionControl>())
                            {
                                tempDic.Add(Convert.ToChar(ctrl.OptionLetter), ctrl.OptionText);
                            }
                            ques.QuestionOptions = tempDic;
                            ques.QuestionText = txt_question_text.Text;
                            ques.SectionTitle = ((TreeView)sender).SelectedNode.Parent.Text;
                            //clear the boxes
                            txt_question_text.Clear();
                            pct_question_picture.Image = null;
                            pan_options.Controls.Clear();
                            //add question to store
                            this.tempExamStore.Add(ques);
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.Print("Error: " + ex.Message + ", Inner Exception: " + ex.InnerException);
            }
            catch (ArgumentNullException ex)
            {
                Debug.Print("Error: " + ex.Message + ", Inner Exception: " + ex.InnerException);
            }
        }       
    }
}
