using Shared;
using Shared.Controls;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Creator
{
    public partial class UI : Form
    {
        #region Class Variables
        private Exam exam;
        private string currentExamFile;
        private PrintOption whatToPrint;
        private UndoRedo undoRedo;

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
            //
            this.undoRedo = new UndoRedo();
        }

        private void Open(object sender, EventArgs e)
        {
            if (ofd_open_exam.ShowDialog() == DialogResult.OK)
            {
                Close(sender, e);
                currentExamFile = ofd_open_exam.FileName;
                Open();
            }
        }

        private void Open()
        {
            this.exam = Helper.GetExamFromFile(currentExamFile);
            //
            trv_view_exam.Nodes.Clear();
            EnableExamControls();
            EnableSectionControls();
            //
            ExamNode examNode = new ExamNode(exam.Properties);
            trv_view_exam.Nodes.Add(examNode);
            foreach(Section section in exam.Sections)
            {
                SectionNode sectionNode = new SectionNode(section.Title)
                {
                    ContextMenuStrip = cms_section
                };
                foreach (Question question in section.Questions)
                {
                    QuestionNode questionNode = new QuestionNode(question)
                    {
                        ContextMenuStrip = cms_question
                    };
                    //
                    sectionNode.Nodes.Add(questionNode);
                }
                examNode.Nodes.Add(sectionNode);
            }
            trv_view_exam.ExpandAll();
            //
            if (splitContainer2.Panel2.Controls.Contains(pan_splash))
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
            //
            this.undoRedo = new UndoRedo();
        }

        private void Save(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentExamFile))
            {
                SaveAs(sender, e);
            }
            else
            {
                if (trv_view_exam.SelectedNode != null)
                    if (trv_view_exam.SelectedNode.GetType() == typeof(QuestionNode))
                        CommitQuestion();
                //
                ExamNode examNode = (ExamNode)trv_view_exam.Nodes[0];
                this.exam.Properties = examNode.Properties;
                this.exam.Sections.Clear();
                foreach(SectionNode sectionNode in examNode.Nodes)
                {
                    Section section = new Section()
                    {
                        Title = sectionNode.Title
                    };
                    foreach (QuestionNode questionNode in sectionNode.Nodes)
                    {
                        Question question = new Question();
                        question = questionNode.Question;
                        //
                        section.Questions.Add(question);
                    }
                    this.exam.Sections.Add(section);
                }
                //
                Helper.WriteExamToFile(this.exam, currentExamFile);
                MessageBox.Show("Exam has been sucessfully saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                IsDirty = false;
            }
            //
            if (Properties.Settings.Default.Exams == null)
                Properties.Settings.Default.Exams = new System.Collections.Specialized.StringCollection();
            if (!Properties.Settings.Default.Exams.Contains(currentExamFile))
                Properties.Settings.Default.Exams.Add(currentExamFile);
            Properties.Settings.Default.Save();
        }

        private void CommitQuestion()
        {
            Question question = ((QuestionNode)trv_view_exam.SelectedNode).Question;
            question.IsMultipleChoice = chkMulipleChoice.Checked;
            if (question.IsMultipleChoice)
            {
                var answerCtrls = pan_options.Controls.OfType<OptionsControl>().Where(s => s.Checked);
                question.Answers = answerCtrls.Select(x => x.Letter).ToArray();
            }
            else
            {
                var answerCtrl = pan_options.Controls.OfType<OptionControl>().FirstOrDefault(s => s.Checked);
                question.Answer = answerCtrl == null ? '\0' : answerCtrl.Letter;
            }
            question.Explanation = txt_explanation.Text;
            question.Image = (Bitmap)pct_image.Image;
            question.No = trv_view_exam.SelectedNode.Index + 1;
            question.Options.Clear();
            if (question.IsMultipleChoice)
            {
                foreach (var ctrl in pan_options.Controls.OfType<OptionsControl>())
                {
                    Option option = new Option()
                    {
                        Alphabet = ctrl.Letter,
                        Text = ctrl.Text
                    };
                    question.Options.Add(option);
                }
            }
            else
            {
                foreach (var ctrl in pan_options.Controls.OfType<OptionControl>())
                {
                    Option option = new Option()
                    {
                        Alphabet = ctrl.Letter,
                        Text = ctrl.Text
                    };
                    question.Options.Add(option);
                }
            }
            question.Text = txt_question_text.Text;
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
            PrintOptions po = new PrintOptions(trv_view_exam.SelectedNode);
            po.ShowDialog();
            whatToPrint = po.SelectedPrintOption;
            pdg_print.ShowDialog();
        }

        private void PrintPreview(object sender, EventArgs e)
        {
            PrintOptions po = new PrintOptions(trv_view_exam.SelectedNode);
            po.ShowDialog();
            whatToPrint = po.SelectedPrintOption;
            ppd_print.ShowDialog();
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Undo(object sender, EventArgs e)
        {
            ChangeRepresentationObject undoObject = undoRedo.Undo();
            if (undoObject == null) return;
            else
            {
                switch(undoObject.Action)
                {
                    case ActionType.Add:
                        SectionNode _sectionNode = trv_view_exam.Nodes[0].Nodes.Cast<SectionNode>().FirstOrDefault(s => s.Title == undoObject.SectionTitle);
                        if (_sectionNode != null)
                        {
                            if (_sectionNode.Nodes.Count >= undoObject.Question.No)
                            {
                                exam.Sections.First(s => s.Title == undoObject.SectionTitle).Questions.RemoveAt(undoObject.Question.No - 1);
                                _sectionNode.Nodes.RemoveAt(undoObject.Question.No - 1);
                            }
                        }
                        //
                        int j = 1;
                        foreach (QuestionNode questionNode_ in _sectionNode.Nodes)
                        {
                            questionNode_.Text = "Question " + j;
                            questionNode_.Question.No = j;
                            j++;
                        }
                        break;
                    case ActionType.Delete:
                        SectionNode sectionNode = trv_view_exam.Nodes[0].Nodes.Cast<SectionNode>().FirstOrDefault(s => s.Title == undoObject.SectionTitle);
                        if (sectionNode == null)
                        {
                            sectionNode = new SectionNode(undoObject.SectionTitle)
                            {
                                ContextMenuStrip = cms_section
                            };
                            //
                            QuestionNode questionNode = new QuestionNode(undoObject.Question)
                            {
                                ContextMenuStrip = cms_question
                            };
                            sectionNode.Nodes.Add(questionNode);
                            //
                            trv_view_exam.Nodes[0].Nodes.Add(sectionNode);
                            trv_view_exam.ExpandAll();
                        }
                        else
                        {
                            QuestionNode questionNode = new QuestionNode(undoObject.Question)
                            {
                                ContextMenuStrip = cms_question
                            };
                            sectionNode.Nodes.Insert(questionNode.Question.No - 1, questionNode);
                            //
                            trv_view_exam.ExpandAll();
                        }
                        //
                        int i = 1;
                        foreach (QuestionNode questionNode_ in sectionNode.Nodes)
                        {
                            questionNode_.Text = "Question " + i;
                            questionNode_.Question.No = i;
                            i++;
                        }
                        break;
                    case ActionType.Modify:
                        SectionNode sectionNode_ = trv_view_exam.Nodes[0].Nodes.Cast<SectionNode>().FirstOrDefault(s => s.Title == undoObject.SectionTitle);
                        if (sectionNode_ != null)
                        {
                            QuestionNode questionNode = (QuestionNode)sectionNode_.Nodes[undoObject.Question.No - 1];
                            questionNode.Question = undoObject.Question;
                            //
                            txt_explanation.Text = undoObject.Question.Explanation;
                            txt_question_text.Text = undoObject.Question.Text;
                            lbl_section_question.Text = "Section: " + trv_view_exam.SelectedNode.Parent.Text + " Question " + undoObject.Question.No;
                            pct_image.Image = undoObject.Question.Image;
                            //
                            pan_options.Controls.Clear();
                            //
                            int k = 0;
                            if (undoObject.Question.IsMultipleChoice)
                            {
                                foreach (var option in undoObject.Question.Options)
                                {
                                    OptionsControl ctrl = new OptionsControl()
                                    {
                                        Letter = option.Alphabet,
                                        Text = option.Text,
                                        Location = new Point(2, k * 36)
                                    };
                                    if (undoObject.Question.Answers.Contains(option.Alphabet))
                                    {
                                        ctrl.Checked = true;
                                    }
                                    pan_options.Controls.Add(ctrl);
                                    k++;
                                }
                            }
                            else
                            {
                                foreach (var option in undoObject.Question.Options)
                                {
                                    OptionControl ctrl = new OptionControl()
                                    {
                                        Letter = option.Alphabet,
                                        Text = option.Text,
                                        Location = new Point(2, k * 36)
                                    };
                                    if (option.Alphabet == undoObject.Question.Answer)
                                    {
                                        ctrl.Checked = true;
                                    }
                                    pan_options.Controls.Add(ctrl);
                                    k++;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void Redo(object sender, EventArgs e)
        {
            ChangeRepresentationObject redoObject = undoRedo.Redo();
            if (redoObject == null) return;
            else
            {
                switch (redoObject.Action)
                {
                    case ActionType.Add:
                        SectionNode sectionNode = trv_view_exam.Nodes[0].Nodes.Cast<SectionNode>().FirstOrDefault(s => s.Title == redoObject.SectionTitle);
                        if(sectionNode == null)
                        {
                            sectionNode = new SectionNode(redoObject.SectionTitle)
                            {
                                ContextMenuStrip = cms_section
                            };
                            //
                            QuestionNode questionNode = new QuestionNode(redoObject.Question)
                            {
                                ContextMenuStrip = cms_question
                            };
                            sectionNode.Nodes.Add(questionNode);
                            //
                            trv_view_exam.Nodes[0].Nodes.Add(sectionNode);
                            trv_view_exam.ExpandAll();
                        }
                        else
                        {
                            sectionNode.ContextMenuStrip = cms_section;
                            //
                            QuestionNode questionNode = new QuestionNode(redoObject.Question)
                            {
                                ContextMenuStrip = cms_question
                            };
                            sectionNode.Nodes.Add(questionNode);
                            //
                            trv_view_exam.ExpandAll();
                        }
                        //
                        int i = 1;
                        foreach (QuestionNode questionNode_ in sectionNode.Nodes)
                        {
                            questionNode_.Text = "Question " + i;
                            questionNode_.Question.No = i;
                            i++;
                        }
                        break;
                    case ActionType.Delete:
                        SectionNode _sectionNode = trv_view_exam.Nodes[0].Nodes.Cast<SectionNode>().FirstOrDefault(s => s.Title == redoObject.SectionTitle);
                        if (_sectionNode != null)
                        {
                            if(_sectionNode.Nodes.Count >= redoObject.Question.No)
                            {
                                exam.Sections.First(s => s.Title == redoObject.SectionTitle).Questions.RemoveAt(redoObject.Question.No - 1);
                                _sectionNode.Nodes.RemoveAt(redoObject.Question.No - 1);
                            }
                        }
                        //
                        int j = 1;
                        foreach (QuestionNode questionNode_ in _sectionNode.Nodes)
                        {
                            questionNode_.Text = "Question " + j;
                            questionNode_.Question.No = j;
                            j++;
                        }
                        break;
                    case ActionType.Modify:
                        SectionNode sectionNode_ = trv_view_exam.Nodes[0].Nodes.Cast<SectionNode>().FirstOrDefault(s => s.Title == redoObject.SectionTitle);
                        if (sectionNode_ != null)
                        {
                            QuestionNode questionNode = (QuestionNode)sectionNode_.Nodes[redoObject.Question.No - 1];
                            questionNode.Question = redoObject.Question;
                            //
                            txt_explanation.Text = redoObject.Question.Explanation;
                            txt_question_text.Text = redoObject.Question.Text;
                            lbl_section_question.Text = "Section: " + trv_view_exam.SelectedNode.Parent.Text + " Question " + redoObject.Question.No;
                            pct_image.Image = redoObject.Question.Image;
                            //
                            pan_options.Controls.Clear();
                            //
                            int k = 0;
                            if (redoObject.Question.IsMultipleChoice)
                            {
                                foreach (var option in redoObject.Question.Options)
                                {
                                    OptionsControl ctrl = new OptionsControl()
                                    {
                                        Letter = option.Alphabet,
                                        Text = option.Text,
                                        Location = new Point(2, k * 36)
                                    };
                                    if (redoObject.Question.Answers.Contains(option.Alphabet))
                                    {
                                        ctrl.Checked = true;
                                    }
                                    pan_options.Controls.Add(ctrl);
                                    k++;
                                }
                            }
                            else
                            {
                                foreach (var option in redoObject.Question.Options)
                                {
                                    OptionControl ctrl = new OptionControl()
                                    {
                                        Letter = option.Alphabet,
                                        Text = option.Text,
                                        Location = new Point(2, k * 36)
                                    };
                                    if (option.Alphabet == redoObject.Question.Answer)
                                    {
                                        ctrl.Checked = true;
                                    }
                                    pan_options.Controls.Add(ctrl);
                                    k++;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void NewSection(object sender, EventArgs e)
        {
            AddSection addSection = new AddSection();
            addSection.ShowDialog();
            //
            SectionNode sectionNode = new SectionNode(addSection.Title)
            {
                ContextMenuStrip = cms_section
            };
            trv_view_exam.Nodes[0].Nodes.Add(sectionNode);
            //
            trv_view_exam.ExpandAll();
            //
            IsDirty = true;
        }

        private void NewQuestion(object sender, EventArgs e)
        {
            SectionNode nodeToBeAddedTo = trv_view_exam.SelectedNode.GetType() == typeof(SectionNode) ? (SectionNode)trv_view_exam.SelectedNode : (SectionNode)trv_view_exam.SelectedNode.Parent;
            Question question = new Question()
            {
                No = nodeToBeAddedTo.Nodes.Count + 1
            };
            //
            QuestionNode questionNode = new QuestionNode(question)
            {
                ContextMenuStrip = cms_question
            };
            nodeToBeAddedTo.Nodes.Add(questionNode);
            //
            trv_view_exam.ExpandAll();
            //
            ChangeRepresentationObject obj = new ChangeRepresentationObject()
            {
                Action = ActionType.Add,
                Question = question,
                SectionTitle = nodeToBeAddedTo.Title
            };
            undoRedo.InsertObjectforUndoRedo(obj);
            //
            IsDirty = true;
        }

        private void Cut(object sender, EventArgs e)
        {
            if (txt_question_text.SelectionLength > 0)
            {
                txt_question_text.Cut();
            }
        }

        private void Copy(object sender, System.EventArgs e)
        {
            if (txt_question_text.SelectionLength > 0)
            {
                txt_question_text.Copy();
            }
        }

        private void Paste(object sender, System.EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                if (txt_question_text.SelectionLength > 0)
                {
                    if (MessageBox.Show("Do you want to paste over current selection?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
                        txt_question_text.SelectionStart = txt_question_text.SelectionStart + txt_question_text.SelectionLength;
                }
                txt_question_text.Paste();
            }
        }

        private void Help(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(@"https://bolorundurowb.github.io/Open-Exam-Suite");
            Process.Start(sInfo);
        }

        private void About(object sender, EventArgs e)
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
                chkMulipleChoice.Checked = question.IsMultipleChoice;
                //
                pan_options.Controls.Clear();
                //
                int i = 0;
                if (question.IsMultipleChoice)
                {
                    foreach (var option in question.Options)
                    {
                        OptionsControl ctrl = new OptionsControl()
                        {
                            Letter = option.Alphabet,
                            Text = option.Text,
                            Location = new Point(2, i * 36)
                        };
                        if (question.Answers.Contains(option.Alphabet))
                        {
                            ctrl.Checked = true;
                        }
                        pan_options.Controls.Add(ctrl);
                        i++;
                    }
                }
                else
                {
                    foreach (var option in question.Options)
                    {
                        OptionControl ctrl = new OptionControl()
                        {
                            Letter = option.Alphabet,
                            Text = option.Text,
                            Location = new Point(2, i * 36)
                        };
                        if (option.Alphabet == question.Answer)
                        {
                            ctrl.Checked = true;
                        }
                        pan_options.Controls.Add(ctrl);
                        i++;
                    }
                }
            }
            //
            ReconnectHandlers();
        }

        private void BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            DisconnectHandlers();
            //
            if (trv_view_exam.SelectedNode != null)
                if (trv_view_exam.SelectedNode.GetType() == typeof(QuestionNode))
                {
                    CommitQuestion();
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
            // Clear all the options
            pan_options.Controls.Clear();
            // Remove test in the text boxes
            txt_code.Clear();
            txt_instruction.Clear();
            txt_title.Clear();
        }

        private void SaveProperties(object sender, EventArgs e)
        {
            Shared.Properties properties = new Shared.Properties()
            {
                Code = txt_code.Text,
                Instructions = txt_instruction.Text,
                Passmark = (int)num_passmark.Value,
                TimeLimit = (int)num_time_limit.Value,
                Title = txt_title.Text,
                Version = (int) float.Parse(lbl_version.Text)
            };
            //
            if (trv_view_exam.Nodes.Count > 0)
            {
                ExamNode examNode = (ExamNode)trv_view_exam.Nodes[0];
                examNode.Properties = properties;
            }
            else
            {
                ExamNode examNode = new ExamNode(properties);
                trv_view_exam.Nodes.Add(examNode);
            }
            //
            trv_view_exam.ExpandAll();
            //
            EnableExamControls();
            EnableSectionControls();
            //
            IsDirty = true;
        }

        private void EnableExamControls()
        {
            closeToolStripMenuItem.Enabled = true;
            //
            exportToolStripMenuItem.Enabled = true;
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
            exportToolStripMenuItem.Enabled = false;
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
            this.exam = null;
            this.undoRedo = null;
            IsDirty = false;
            //
            LoadExamHistory();
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
            //
            QuestionChanged(sender, e);
        }

        private void ClearImage(object sender, EventArgs e)
        {
            pct_image.Image = null;
            //
            QuestionChanged(sender, e);
        }

        private void RemoveOption(object sender, EventArgs e)
        {
            if (chkMulipleChoice.Checked)
            {
                pan_options.Controls.Remove(pan_options.Controls.OfType<OptionsControl>().ElementAt(pan_options.Controls.OfType<OptionsControl>().Count() - 1));
            }
            else
            {
                pan_options.Controls.Remove(pan_options.Controls.OfType<OptionControl>().ElementAt(pan_options.Controls.OfType<OptionControl>().Count() - 1));
            }
            //
            QuestionChanged(sender, e);
        }

        private void AddOption(object sender, EventArgs e)
        {
            try
            {
                if (chkMulipleChoice.Checked)
                {
                    if (pan_options.Controls.Count > 0)
                    {
                        OptionsControl ctrl = new OptionsControl()
                        {
                            Name = "option" + (pan_options.Controls.Count - 1),
                            Letter = (char)(Convert.ToInt32(((OptionsControl)pan_options.Controls[pan_options.Controls.Count - 1]).Letter) + 1),
                            Location = new Point(2, 2 + (pan_options.Controls.Count * 36))
                        };
                        pan_options.Controls.Add(ctrl);
                    }
                    else
                    {
                        OptionsControl ctrl = new OptionsControl()
                        {
                            Location = new Point(2, 2),
                            Name = "option0",
                            Letter = 'A'
                        };
                        pan_options.Controls.Add(ctrl);
                    }
                }
                else
                {
                    if (pan_options.Controls.Count > 0)
                    {
                        OptionControl ctrl = new OptionControl()
                        {
                            Name = "option" + (pan_options.Controls.Count - 1),
                            Letter = (char)(Convert.ToInt32(((OptionControl)pan_options.Controls[pan_options.Controls.Count - 1]).Letter) + 1),
                            Location = new Point(2, 2 + (pan_options.Controls.Count * 36))
                        };
                        pan_options.Controls.Add(ctrl);
                    }
                    else
                    {
                        OptionControl ctrl = new OptionControl()
                        {
                            Location = new Point(2, 2),
                            Name = "option0",
                            Letter = 'A'
                        };
                        pan_options.Controls.Add(ctrl);
                    }
                }
                //
                QuestionChanged(sender, e);
            }
            catch (Exception)
            {
                MessageBox.Show("Sorry, you cannot mix option types. First remove the existing options then replace them.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (whatToPrint == PrintOption.CurrentQuestion)
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                Font normFont = new Font("Calibri", 12);
                Font subHeadFont = new Font("Calibri", 13F);
                Font headerFont = new Font("Cambria", 14, FontStyle.Bold);

                e.Graphics.DrawString("OPEN EXAM SUITE - CREATOR", headerFont, Brushes.Purple, new PointF(250, yPos));
                yPos += 2 * headerFont.GetHeight(e.Graphics);

                e.Graphics.DrawString("EXAM: " + exam.Properties.Title + "  EXAM CODE: " + exam.Properties.Code, subHeadFont, Brushes.Green, new PointF(200, yPos));
                yPos += 2 * subHeadFont.GetHeight(e.Graphics);

                e.Graphics.DrawString("Section: " + trv_view_exam.SelectedNode.Parent.Text, subHeadFont, Brushes.Green, new PointF(leftMargin, yPos));
                yPos += 2 * subHeadFont.GetHeight(e.Graphics);

                e.Graphics.DrawString(trv_view_exam.SelectedNode.Text, subHeadFont, Brushes.Black, new PointF(leftMargin, yPos));
                yPos += subHeadFont.GetHeight(e.Graphics);

                for (int i = 0; i < txt_question_text.Lines.Length; i++)
                {
                    e.Graphics.DrawString(txt_question_text.Lines[i], normFont, Brushes.Black, new RectangleF(leftMargin, yPos, e.MarginBounds.Width + 60, 150),
                        StringFormat.GenericTypographic);
                    yPos += subHeadFont.GetHeight(e.Graphics);
                }
                yPos += subHeadFont.GetHeight(e.Graphics);
                if (pct_image.Image != null)
                {
                    yPos += 50;
                    e.Graphics.DrawImage(pct_image.Image, new Rectangle(Convert.ToInt32(leftMargin + 100), Convert.ToInt32(yPos + 15), 450, 400));
                    yPos += 400;
                }

                foreach (OptionControl control in pan_options.Controls)
                {
                    string temp = control.Letter + ". -  " + control.Text;
                    e.Graphics.DrawString(temp, normFont, Brushes.Black, new PointF(leftMargin + 35, yPos));
                    yPos += normFont.GetHeight(e.Graphics);
                }
            }
            else if (whatToPrint == PrintOption.CurrentSection)
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                Font normFont = new Font("Calibri", 12);
                Font subHeadFont = new Font("Calibri", 13F);
                Font headerFont = new Font("Cambria", 14, FontStyle.Bold);

                e.Graphics.DrawString("OPEN EXAM SUITE - CREATOR", headerFont, Brushes.Purple, new PointF(250, yPos));
                yPos += 2 * headerFont.GetHeight(e.Graphics);

                e.Graphics.DrawString("EXAM: " + exam.Properties.Title + "  EXAM CODE: " + exam.Properties.Code, subHeadFont, Brushes.Green, new PointF(200, yPos));
                yPos += 2 * subHeadFont.GetHeight(e.Graphics);
            }
            else
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                Font normFont = new Font("Calibri", 12);
                Font subHeadFont = new Font("Calibri", 13F);
                Font headerFont = new Font("Cambria", 14, FontStyle.Bold);

                e.Graphics.DrawString("OPEN EXAM SUITE - CREATOR", headerFont, Brushes.Purple, new PointF(250, yPos));
                yPos += 2 * headerFont.GetHeight(e.Graphics);

                e.Graphics.DrawString("EXAM: " + exam.Properties.Title + "  EXAM CODE: " + exam.Properties.Code, subHeadFont, Brushes.Green, new PointF(200, yPos));
                yPos += 2 * subHeadFont.GetHeight(e.Graphics);
            }
        }

        private void UIFormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDirty)
            {
                DialogResult result = MessageBox.Show("The current exam has not been saved, do you want to save and close?", "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    e.Cancel = true;
                else if (result == DialogResult.Yes)
                    Save(sender, e);
            }
        }

        private void LoadExamHistory()
        {
            if (Properties.Settings.Default.Exams != null)
            {
                foreach (Control link in grp_exam_history.Controls.OfType<LinkLabel>())
                {
                    grp_exam_history.Controls.Remove(link);
                }
                int i = 0;
                foreach (string exam in Properties.Settings.Default.Exams)
                {
                    LinkLabel examLink = new LinkLabel()
                    {
                        Location = new Point(10, (25 + (i * 25))),
                        AutoSize = true,
                        Text = exam
                    };
                    examLink.Click += ExamLinkClick;
                    grp_exam_history.Controls.Add(examLink);
                    i++;
                }
            }
        }

        void ExamLinkClick(object sender, EventArgs e)
        {
            if (File.Exists(((LinkLabel)sender).Text))
            {
                currentExamFile = ((LinkLabel)sender).Text;
                Open();
            }
            else
            {
                MessageBox.Show("Sorry, the selected file has been moved or deleted.", "Access error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Properties.Settings.Default.Exams.Remove(((LinkLabel)sender).Text);
                Properties.Settings.Default.Save();
                grp_exam_history.Controls.Remove(((Control)sender));
            }
        }

        private void LoadUI(object sender, EventArgs e)
        {
            LoadExamHistory();
        }

        private void DeleteQuestion(object sender, EventArgs e)
        {
            var sectionNode = trv_view_exam.SelectedNode.Parent;
            //
            ChangeRepresentationObject obj = new ChangeRepresentationObject()
            {
                Action = ActionType.Delete,
                Question = ((QuestionNode)trv_view_exam.SelectedNode).Question,
                SectionTitle = ((SectionNode)sectionNode).Title
            };
            undoRedo.InsertObjectforUndoRedo(obj);
            //
            sectionNode.Nodes.Remove(trv_view_exam.SelectedNode);
            //
            int i = 1;
            foreach(QuestionNode questionNode in sectionNode.Nodes)
            {
                questionNode.Question.No = 1;
                questionNode.Text = "Question " + i;
                i++;
            }
            //
            IsDirty = true;
        }

        private void EditSection(object sender, EventArgs e)
        {
            SectionNode sectionNode = (SectionNode)trv_view_exam.SelectedNode;
            //
            EditSection editSection = new Creator.EditSection(sectionNode.Title);
            editSection.ShowDialog();
            //
            sectionNode.Title = editSection.Title;
            sectionNode.Text = editSection.Title;
            //
            IsDirty = true;
        }

        private void MakeSureNodeSelected(object sender, TreeNodeMouseClickEventArgs e)
        {
            trv_view_exam.SelectedNode = e.Node;
        }

        private void QuestionChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            //
            ChangeRepresentationObject obj = new ChangeRepresentationObject()
            {
                Action = ActionType.Modify
            };
            //
            Question question = new Question()
            {
                IsMultipleChoice = chkMulipleChoice.Checked
            };
            if (question.IsMultipleChoice)
            {
                var answerCtrls = pan_options.Controls.OfType<OptionsControl>().Where(s => s.Checked);
                question.Answers = answerCtrls.Select(x => x.Letter).ToArray();
            }
            else
            {
                var answerCtrl = pan_options.Controls.OfType<OptionControl>().FirstOrDefault(s => s.Checked);
                question.Answer = answerCtrl == null ? '\0' : answerCtrl.Letter;
            }
            question.Explanation = txt_explanation.Text;
            question.Image = (Bitmap)pct_image.Image;
            question.No = trv_view_exam.SelectedNode.Index + 1;
            question.Options.Clear();
            if (question.IsMultipleChoice)
            {
                var ctrls = pan_options.Controls.OfType<OptionsControl>();
                foreach (var ctrl in ctrls)
                {
                    Option option = new Option()
                    {
                        Alphabet = ctrl.Letter,
                        Text = ctrl.Text
                    };
                    question.Options.Add(option);
                }
            }
            else
            {
                var ctrls = pan_options.Controls.OfType<OptionControl>();
                foreach (var ctrl in ctrls)
                {
                    Option option = new Option()
                    {
                        Alphabet = ctrl.Letter,
                        Text = ctrl.Text
                    };
                    question.Options.Add(option);
                }
            }

            question.Text = txt_question_text.Text;
            //
            obj.Question = question;
            obj.SectionTitle = ((SectionNode)trv_view_exam.SelectedNode.Parent).Title;
            undoRedo.InsertObjectforUndoRedo(obj);
        }

        private void DisconnectHandlers()
        {
            txt_question_text.TextChanged -= QuestionChanged;
            txt_explanation.TextChanged -= QuestionChanged;
        }

        private void ReconnectHandlers()
        {
            txt_question_text.TextChanged += QuestionChanged;
            txt_explanation.TextChanged += QuestionChanged;
        }

        private void ExportAtJson(object sender, EventArgs e)
        {
            if (this.exam != null)
            {
                string examJsonString = JsonConvert.SerializeObject(this.exam, Formatting.Indented);
                if (!string.IsNullOrWhiteSpace(examJsonString))
                {
                    SaveFileDialog sfdExportJson = new SaveFileDialog()
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        Filter = "JSON Files|*.json",
                        FilterIndex = 1,
                        FileName = this.exam.Properties.Title
                    };
                    if (sfdExportJson.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfdExportJson.FileName, examJsonString);
                        MessageBox.Show("JSON successfully exported.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void ExportXML(object sender, EventArgs e)
        {
            if (this.exam != null)
            {
                var examXmlStringWriter = new System.IO.StringWriter();
                var serializer = new System.Xml.Serialization.XmlSerializer(this.exam.GetType());
                SaveFileDialog sfdExportXml = new SaveFileDialog()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Filter = "XML Files|*.Xml",
                    FilterIndex = 1,
                    FileName = this.exam.Properties.Title
                };
                if (sfdExportXml.ShowDialog() == DialogResult.OK)
                {
                    serializer.Serialize(examXmlStringWriter, this.exam);
                    File.WriteAllText(sfdExportXml.FileName, examXmlStringWriter.ToString());
                    MessageBox.Show("XML successfully exported.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
