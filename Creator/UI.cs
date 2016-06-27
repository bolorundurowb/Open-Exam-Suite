using Shared;
using Shared.Controls;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System;

namespace Creator
{
    public partial class UI : Form
    {
        #region Class Variables
        private Exam exam;
        private string currentExamFile;
        private bool IsDirty { get; set; }
        #endregion

        public UI()
        {
            InitializeComponent();
        }

        private void New(object sender, EventArgs e)
        {
            Close(sender, e);
            //
            this.exam = new Exam();
            splitContainer2.Panel2.Controls.Remove(pan_splash);
            splitContainer2.Panel2.Controls.Add(pan_exam_properties);
        }

        private void Open(object sender, EventArgs e)
        {
            if (ofd_open_exam.ShowDialog() == DialogResult.OK)
            {
                this.exam = Helper.GetExamFromFile(ofd_open_exam.FileName);
                //
                trv_view_exam.Nodes.Clear();
                //
                ExamNode examNode = new ExamNode(this.exam);
                trv_view_exam.Nodes.Add(examNode);
                trv_view_exam.ExpandAll();
            }
        }

        private void Save(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentExamFile))
            {
                SaveAs(sender, e);
            }
            else
            {
                Helper.WriteExamToFile(this.exam, currentExamFile);
                IsDirty = false;
            }
        }

        private void SaveAs(object sender, EventArgs e)
        {
            sfd_save_as_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sfd_save_as_exam.ShowDialog();
            if (string.IsNullOrWhiteSpace(sfd_save_as_exam.FileName))
            {
                MessageBox.Show("Improper file name, Exam not saved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                currentExamFile = sfd_save_as_exam.FileName;
                Save(sender, e);
            }
        }

        private void Print(object sender, EventArgs e)
        {

        }

        private void PrintPreview(object sender, EventArgs e)
        {

        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Undo(object sender, EventArgs e)
        {

        }

        private void Redo(object sender, EventArgs e)
        {

        }

        private void NewSection(object sender, EventArgs e)
        {
            AddSection addSection = new AddSection();
            addSection.ShowDialog();
            //
            Section section = new Section();
            section.Title = addSection.Title;
            this.exam.Sections.Add(section);
            //
            SectionNode sectionNode = new SectionNode(section);
            sectionNode.ImageIndex = 1;
            sectionNode.SelectedImageIndex = 1;
            trv_view_exam.Nodes[0].Nodes.Add(sectionNode);
            //
            trv_view_exam.ExpandAll();
        }

        private void NewQuestion(object sender, EventArgs e)
        {
            var nodeToBeAddedTo = trv_view_exam.SelectedNode.GetType() == typeof(SectionNode) ? trv_view_exam.SelectedNode : trv_view_exam.SelectedNode.Parent;
            Question question = new Question();
            question.No = nodeToBeAddedTo.Nodes.Count + 1;
            //
            QuestionNode questionNode = new QuestionNode(question);
            questionNode.ImageIndex = 2;
            questionNode.SelectedImageIndex = 2;
            nodeToBeAddedTo.Nodes.Add(questionNode);
            //
            trv_view_exam.ExpandAll();
        }

        private void Cut(object sender, EventArgs e)
        {

        }

        private void Copy(object sender, System.EventArgs e)
        {

        }

        private void Paste(object sender, System.EventArgs e)
        {

        }

        private void Help(object sender, System.EventArgs e)
        {

        }

        private void About(object sender, System.EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(trv_view_exam.SelectedNode.GetType() == typeof(ExamNode))
            {
                newQuestionToolStripButton.Enabled = false;
                //
                if (splitContainer2.Panel2.Controls.Contains(pan_display_questions))
                {
                    splitContainer2.Panel2.Controls.Remove(pan_display_questions);
                    splitContainer2.Panel2.Controls.Add(pan_exam_properties);
                }
                else if (splitContainer2.Panel2.Controls.Contains(pan_splash))
                {
                    splitContainer2.Panel2.Controls.Remove(pan_splash);
                    splitContainer2.Panel2.Controls.Add(pan_exam_properties);
                }
                //
                txt_code.Text = exam.Properties.Code;
                txt_instruction.Text = exam.Properties.Instructions;
                txt_title.Text = exam.Properties.Title;
                num_passmark.Value = (decimal)exam.Properties.Passmark;
                num_time_limit.Value = exam.Properties.TimeLimit;
            }
            else if(trv_view_exam.SelectedNode.GetType() == typeof(SectionNode))
            {
                newQuestionToolStripButton.Enabled = true;
                //
                if (splitContainer2.Panel2.Controls.Contains(pan_exam_properties))
                {
                    splitContainer2.Panel2.Controls.Remove(pan_exam_properties);
                    splitContainer2.Panel2.Controls.Add(pan_display_questions);
                }
                else if (splitContainer2.Panel2.Controls.Contains(pan_splash))
                {
                    splitContainer2.Panel2.Controls.Remove(pan_splash);
                    splitContainer2.Panel2.Controls.Add(pan_display_questions);
                }
                pan_display_questions.Enabled = false;
            }
            else
            {
                newQuestionToolStripButton.Enabled = true;
                //
                if (splitContainer2.Panel2.Controls.Contains(pan_exam_properties))
                {
                    splitContainer2.Panel2.Controls.Remove(pan_exam_properties);
                    splitContainer2.Panel2.Controls.Add(pan_display_questions);
                }
                else if (splitContainer2.Panel2.Controls.Contains(pan_splash))
                {
                    splitContainer2.Panel2.Controls.Remove(pan_splash);
                    splitContainer2.Panel2.Controls.Add(pan_display_questions);
                }
                pan_display_questions.Enabled = true;
                //
                Question question = ((QuestionNode)trv_view_exam.SelectedNode).Question;
                txt_explanation.Text = question.Explanation;
                txt_question_text.Text = question.Text;
                lbl_section_question.Text = "Section: " + trv_view_exam.SelectedNode.Parent.Text + " Question " + question.No;
                pct_image.Image = question.Image;
                //
                int i = 0;
                foreach(var option in question.Options)
                {
                    OptionControl ctrl = new OptionControl();
                    ctrl.Letter = option.Alphabet;
                    ctrl.Text = option.Text;
                    ctrl.Location = new Point(2, i * 36);
                    if (option.Alphabet == question.Answer)
                    {
                        ctrl.Checked = true;
                    }
                    pan_options.Controls.Add(ctrl);
                    i++;
                } 
            }
        }

        private void BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (trv_view_exam.SelectedNode != null)
                if (trv_view_exam.SelectedNode.GetType() == typeof(QuestionNode))
                {
                    Question question = ((QuestionNode)trv_view_exam.SelectedNode).Question;
                    var answerCtrl = pan_options.Controls.OfType<OptionControl>().FirstOrDefault(s => s.Checked);
                    question.Answer = answerCtrl == null ? '\0' : answerCtrl.Letter;
                    question.Explanation = txt_explanation.Text;
                    question.Image = (Bitmap)pct_image.Image;
                    question.No = trv_view_exam.SelectedNode.Index + 1;
                    question.Options.Clear();
                    foreach (var ctrl in pan_options.Controls.OfType<OptionControl>())
                    {
                        Option option = new Option();
                        option.Alphabet = ctrl.Letter;
                        option.Text = ctrl.Text;
                        question.Options.Add(option);
                    }
                    question.Text = txt_question_text.Text;
                    //
                    ClearControls();
                }
        }

        private void ClearControls()
        {
            lbl_section_question.Text = "";
            txt_question_text.Clear();
            txt_explanation.Clear();
            pct_image.Image = null;
            //
            pan_options.Controls.Clear();
        }

        private void SaveProperties(object sender, System.EventArgs e)
        {
            this.exam.Properties.Code = txt_code.Text;
            this.exam.Properties.Instructions = txt_instruction.Text;
            this.exam.Properties.Passmark = (int)num_passmark.Value;
            this.exam.Properties.TimeLimit = (int)num_time_limit.Value;
            this.exam.Properties.Title = txt_title.Text;
            this.exam.Properties.Version = int.Parse(lbl_version.Text);
            //
            ExamNode examNode = new ExamNode(exam);
            examNode.ImageIndex = 0;
            examNode.SelectedImageIndex = 0;
            trv_view_exam.Nodes.Add(examNode);
            //
            trv_view_exam.ExpandAll();
            //
            EnableExamControls();
            EnableSectionControls();
        }

        private void EnableExamControls()
        {
            closeToolStripMenuItem.Enabled = true;
            //
            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripButton.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            //
            printPreviewToolStripMenuItem.Enabled = true;
            printToolStripButton.Enabled = true;
            printToolStripMenuItem.Enabled = true;
            //
            undoToolStripMenuItem.Enabled = true;
            redoToolStripMenuItem.Enabled = true;
        }

        private void EnableSectionControls()
        {
            newSectionToolStripButton.Enabled = true;
        }

        private void EnableQuestionControls()
        {
            cutToolStripButton.Enabled = true;
            cutToolStripMenuItem.Enabled = true;
            pasteToolStripButton.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            copyToolStripButton.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
        }

        private void DisableQuestionControls()
        {
            cutToolStripButton.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
            pasteToolStripButton.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            copyToolStripButton.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
        }

        private void DisableAllControls()
        {
            closeToolStripMenuItem.Enabled = false;
            //
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            //
            printPreviewToolStripMenuItem.Enabled = false;
            printToolStripButton.Enabled = false;
            printToolStripMenuItem.Enabled = false;
            //
            newQuestionToolStripButton.Enabled = false;
            newSectionToolStripButton.Enabled = false;
            //
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            //
            cutToolStripButton.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
            pasteToolStripButton.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            copyToolStripButton.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
        }

        private void Close(object sender, EventArgs e)
        {
            if(IsDirty)
            {
                var response = MessageBox.Show("There are unsaved changes in your project. Do you want to save the changes before closing it?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if(response == DialogResult.Yes)
                {
                    Save(sender, e);
                }
                else if(response == DialogResult.Cancel)
                {
                    return;
                }
            }
            this.exam = null;
            IsDirty = false;
            //
            ClearControls();
            trv_view_exam.Nodes.Clear();
            DisableAllControls();
            //
            if (splitContainer2.Panel2.Contains(pan_display_questions))
            {
                splitContainer2.Panel2.Controls.Remove(pan_display_questions);
                splitContainer2.Panel2.Controls.Add(pan_splash);
            }
            else if (splitContainer2.Panel2.Contains(pan_exam_properties))
            {
                splitContainer2.Panel2.Controls.Remove(pan_exam_properties);
                splitContainer2.Panel2.Controls.Add(pan_splash);
            }
        }

        private void OptionsChanged(object sender, ControlEventArgs e)
        {
            if (pan_options.Controls.Count > 0)
                btn_remove_option.Enabled = true;
            else
                btn_remove_option.Enabled = false;
        }

        private void InsertImage(object sender, EventArgs e)
        {
            ofd_select_image.ShowDialog();
            if (!string.IsNullOrWhiteSpace(ofd_select_image.FileName))
            {
                pct_image.ImageLocation = ofd_select_image.FileName;
            }
        }

        private void ClearImage(object sender, EventArgs e)
        {
            pct_image.Image = null;
        }

        private void RemoveOption(object sender, EventArgs e)
        {
            pan_options.Controls.Remove(pan_options.Controls.OfType<OptionControl>().ElementAt(pan_options.Controls.OfType<OptionControl>().Count() - 1));
        }

        private void AddOption(object sender, EventArgs e)
        {
            if (pan_options.Controls.Count > 0)
            {
                OptionControl ctrl = new OptionControl();
                ctrl.Name = "option" + (pan_options.Controls.Count - 1);
                ctrl.Letter = (char)(Convert.ToInt32(((OptionControl)pan_options.Controls[pan_options.Controls.Count - 1]).Letter) + 1);
                ctrl.Location = new Point(2, 2 + (pan_options.Controls.Count * 36));
                pan_options.Controls.Add(ctrl);
            }
            else
            {
                OptionControl ctrl = new OptionControl();
                ctrl.Location = new Point(2, 2);
                ctrl.Name = "option0";
                ctrl.Letter = 'A';
                pan_options.Controls.Add(ctrl);
            }
        }

        private void Editable(object sender, EventArgs e)
        {
            EnableQuestionControls();
        }

        private void NotEditable(object sender, EventArgs e)
        {
            DisableQuestionControls();
        }
    }
}
