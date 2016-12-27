using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Ionic.Zip;
using System.IO;

namespace Creator
{
    public partial class UI : Form
    {
        #region Classwise Variables
        /* 
         * Compared to the v1.0 I've significantly reduced
         * my use of classwise variables, but as I'm not
         * yet the programmer I intend to be, these few are necessary.
         * 
         */
        List<Question> rawQuestionList;
        private bool allChangesSaved;
        private bool savedAs;
        private PrintOption whatToPrint;
        private int count;
        IEnumerable<Question> sectionQuestions;
        List<Question> allQuestions;
        private bool firstPage;
        private string sectionName;
        #endregion

        private bool AllChangesSaved
        {
            get
            {
                return allChangesSaved;
            }
            set
            {
                allChangesSaved = value;
                if (allChangesSaved)
                {
                    saveToolStripButton.Enabled = false;
                    saveToolStripMenuItem.Enabled = false;
                    if (trv_explorer.Nodes[0].Text.Contains('*'))
                        trv_explorer.Nodes[0].Text = trv_explorer.Nodes[0].Text.Remove(0, 2);
                    lbl_show_save_status.Text = "Exam saved successfully!";
                }
                else
                {
                    if (savedAs)
                    {
                        saveToolStripButton.Enabled = true;
                        saveToolStripMenuItem.Enabled = true;
                    }
                    if (trv_explorer.Nodes.Count > 0)
                        if (!trv_explorer.Nodes[0].Text.Contains('*'))
                            trv_explorer.Nodes[0].Text = "* " + trv_explorer.Nodes[0].Text;
                    lbl_show_save_status.Text = "";
                }
            }
        }

        private string CurrentFileNameWithExtension { get; set; }

        public UI()
        {
            InitializeComponent();
        }

        private void UI_Load(object sender, EventArgs e)
        {
            tab_display_questions.Enabled = false;
            tab_exam_properties.Enabled = false;
            if (Properties.Settings.Default.ExamHistory != null)
            {
                foreach (Control link in grp_exam_list.Controls.OfType<LinkLabel>())
                {
                    grp_exam_list.Controls.Remove(link);
                }
                int i = 0;
                foreach (string exam in Properties.Settings.Default.ExamHistory)
                {
                    LinkLabel examLink = new LinkLabel();
                    examLink.Location = new Point(12, (55 + (i * 24)));
                    examLink.AutoSize = true;
                    examLink.Text = exam;
                    examLink.Click += examLink_Click;
                    grp_exam_list.Controls.Add(examLink);
                    i++;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void New_Exam(object sender, EventArgs e)
        {
            this.rawQuestionList = new List<Question>();
            tab_exam_properties.Enabled = true;
            tabControl.TabPages.Remove(tab_start);
            tabControl.SelectedIndex = 0;
        }

        private void Open_Click(object sender, EventArgs e)
        {
            ofd_open_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd_open_exam.ShowDialog();
            Open(ofd_open_exam.FileName);
        }

        private void Open(string examFilePath)
        {
            CloseExam(null, null);
            New_Exam(null, null);

            CurrentFileNameWithExtension = examFilePath;
            string foldername = Path.GetFileNameWithoutExtension(examFilePath);
            string extractionFolderPath = Path.Combine(GlobalPathVariables.creatorFolderPath, foldername);
            if (Directory.Exists(extractionFolderPath))
            {
                Directory.Delete(extractionFolderPath, true);
                Directory.CreateDirectory(extractionFolderPath);
            }
            else
            {
                Directory.CreateDirectory(extractionFolderPath);
            }
            using (ZipFile zip = new ZipFile(examFilePath))
            {
                zip.ExtractAll(extractionFolderPath, ExtractExistingFileAction.OverwriteSilently);
            }

            string[] temp = Directory.GetFiles(extractionFolderPath, "*.xml", SearchOption.TopDirectoryOnly);
            string xmlFilePath = null;
            if (temp.Length > 0)
                xmlFilePath = temp[0];
            
            if (string.IsNullOrWhiteSpace(xmlFilePath))
            {
                MessageBox.Show("Sorry, the selected exam file is invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                XPathDocument doc = new XPathDocument(xmlFilePath);
                XPathNavigator navigator = doc.CreateNavigator();
                string version = "";
                XPathExpression expression = navigator.Compile("//FileVersion");
                XPathNodeIterator iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    version = iterator.Current.Value;
                }
                expression = navigator.Compile("//ExamTitle");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    txt_exam_title.Text = iterator.Current.Value;
                }
                expression = navigator.Compile("//TimeAllowed");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    num_time_limit.Value = Convert.ToDecimal(iterator.Current.Value);
                }
                expression = navigator.Compile("//PassingScore");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    num_passing_score.Value = Convert.ToDecimal(iterator.Current.Value);
                }
                expression = navigator.Compile("//ExamCode");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    txt_exam_code.Text = iterator.Current.Value;
                }
                expression = navigator.Compile("//ExamInstructions");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    txt_exam_instructions.Text = iterator.Current.Value;
                }
                expression = navigator.Compile("//Section");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    TreeNode secNode = new TreeNode();
                    secNode.Name = "sectionNode" + (trv_explorer.Nodes[0].Nodes.Count);
                    secNode.Text = iterator.Current.GetAttribute("Title", "");
                    secNode.ImageIndex = 1;
                    secNode.SelectedImageIndex = 1;
                    secNode.ContextMenuStrip = cms_explorer;

                    XPathNodeIterator subIterator = iterator.Current.SelectChildren(XPathNodeType.Element);
                    while (subIterator.MoveNext())
                    {
                        Question question = new Question();
                        question.SectionTitle = iterator.Current.GetAttribute("Title", "");
                        question.QuestionNumber = Convert.ToInt32(subIterator.Current.GetAttribute("No", ""));
                        Dictionary<char, string> options = new Dictionary<char, string>();
                        XPathNodeIterator subIter = subIterator.Current.SelectChildren(XPathNodeType.Element);
                        while (subIter.MoveNext())
                        {
                            if (subIter.Current.LocalName == "Text")
                            {
                                question.QuestionText = subIter.Current.Value;
                            }
                            else if (subIter.Current.LocalName == "Image")
                            {
                                if (!string.IsNullOrWhiteSpace(subIter.Current.Value))
                                    question.QuestionImagePath = extractionFolderPath + Path.DirectorySeparatorChar + subIter.Current.Value;
                            }
                            else if (subIter.Current.LocalName == "Answer")
                            {
                                question.QuestionAnswer = Convert.ToChar(subIter.Current.Value);
                            }
                            if (subIter.Current.LocalName == "Options")
                            {
                                XPathNodeIterator ite = subIter.Current.SelectChildren(XPathNodeType.Element);
                                while (ite.MoveNext())
                                {
                                    char option;
                                    string optionText;
                                    string tempp = ite.Current.GetAttribute("Title", "");
                                    option = Convert.ToChar(tempp);
                                    optionText = ite.Current.Value;
                                    options.Add(option, optionText);
                                }
                                question.QuestionOptions = options;
                            }
                            if (version == "1.0")
                            {
                                question.AnswerExplanation = "Version 1.0 files do not support explanations";
                            }
                            else
                            {
                                if (iterator.Current.LocalName == "AnswerExplanation")
                                    question.AnswerExplanation = iterator.Current.Value;
                            }
                        }
                        rawQuestionList.Add(question);

                        TreeNode quesNode = new TreeNode();
                        quesNode.ImageIndex = 2;
                        quesNode.SelectedImageIndex = 2;
                        quesNode.ContextMenuStrip = cms_explorer;
                        quesNode.Text = "Question " + (secNode.Nodes.Count + 1);
                        quesNode.Name = "questionNode" + secNode.Nodes.Count;
                        secNode.Nodes.Add(quesNode);
                    }
                    trv_explorer.Nodes[0].Nodes.Add(secNode);
                }

                trv_explorer.ExpandAll();
                closeExamToolStripMenuItem.Enabled = true;
                printPreviewToolStripMenuItem.Enabled = true;
                printToolStripButton.Enabled = true;
                printToolStripMenuItem.Enabled = true;

                savedAs = true;
                AllChangesSaved = true;
            }
        }

        private void Print(object sender, EventArgs e)
        {
            PrintOptions po = new PrintOptions(trv_explorer.SelectedNode);
            po.ShowDialog();
            whatToPrint = po.SelectedPrintOption;
            pdg_print.ShowDialog();
            count = 0;
            sectionName = "";
        }

        private void Print_Preview(object sender, EventArgs e)
        {
            PrintOptions po = new PrintOptions(trv_explorer.SelectedNode);
            po.ShowDialog();
            whatToPrint = po.SelectedPrintOption;
            ppd_print.ShowDialog();
            count = 0;
            sectionName = "";
        }

        private void Save_As(object sender, EventArgs e)
        {
            sfd_save_as_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sfd_save_as_exam.FileName = txt_exam_title.Text;
            sfd_save_as_exam.ShowDialog();
            if (string.IsNullOrWhiteSpace(sfd_save_as_exam.FileName))
            {
                lbl_show_save_status.Text = "Improper file name, Exam not saved!";
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(GlobalPathVariables.creatorFolderPath, Path.GetFileNameWithoutExtension(sfd_save_as_exam.FileName)));
                CurrentFileNameWithExtension = sfd_save_as_exam.FileName;
                Save(sender, null);
            }
            savedAs = true;
            saveAsToolStripMenuItem.Enabled = false;
        }

        private void Save(object sender, EventArgs e)
        {
            //Save the currently edited question
            if (trv_explorer.SelectedNode != null)
                if (trv_explorer.SelectedNode.Name.Contains("questionNode"))
                    PushPreviousQuestion(trv_explorer.SelectedNode);

            SaveXMLFromDictionary(GetDictionaryFromQuestionList(rawQuestionList));
            using (ZipFile oefFile = new ZipFile())
            {
                foreach (string file in Directory.GetFiles(Path.Combine(GlobalPathVariables.creatorFolderPath,
                Path.GetFileNameWithoutExtension(CurrentFileNameWithExtension))))
                {
                    oefFile.AddFile(file, "");
                }
                if (File.Exists(CurrentFileNameWithExtension))
                {
                    File.Delete(CurrentFileNameWithExtension);
                }
                oefFile.Save(CurrentFileNameWithExtension);
            }
            //Show that the changes have been committed
            AllChangesSaved = true;
            //Save exam in previous work list
            if (Properties.Settings.Default.ExamHistory == null)
            {
                Properties.Settings.Default.ExamHistory = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.ExamHistory.Add(CurrentFileNameWithExtension);
                Properties.Settings.Default.Save();
            }
            else
            {
                if (!Properties.Settings.Default.ExamHistory.Contains(CurrentFileNameWithExtension))
                    Properties.Settings.Default.ExamHistory.Add(CurrentFileNameWithExtension);
                Properties.Settings.Default.Save();
            }
        }

        private void Cut(object sender, EventArgs e)
        {
            if (txt_question_text.SelectionLength > 0)
            {
                txt_question_text.Cut();
            }
        }

        private void Copy(object sender, EventArgs e)
        {
            if (txt_question_text.SelectionLength > 0)
            {
                txt_question_text.Copy();
            }
        }

        private void Paste(object sender, EventArgs e)
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

        private void txt_exam_title_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_exam_title.Text))
            {
                newSectionToolStripButton.Enabled = false;
            }
            else
            {
                if (trv_explorer.Nodes.Count == 0)
                {
                    trv_explorer.Nodes.Add("examNode", txt_exam_title.Text, 0, 0);
                }
                else
                {
                    trv_explorer.Nodes[0].Text = txt_exam_title.Text;
                }
                trv_explorer.ExpandAll();
                newSectionToolStripButton.Enabled = true;
            }
            AllChangesSaved = false;
        }

        private void newSectionToolStripButton_Click(object sender, EventArgs e)
        {
            New_Section section = new New_Section();
            section.ShowDialog();
            bool exists = false;
            for (int i = 0; i < trv_explorer.Nodes[0].Nodes.Count; i++)
            {
                if (trv_explorer.Nodes[0].Nodes[i].Text == section.SectionName)
                {
                    exists = true;
                }
            }
            if (!exists)
            {
                TreeNode secNode = new TreeNode();
                secNode.Name = "sectionNode" + (trv_explorer.Nodes[0].Nodes.Count);
                secNode.Text = section.SectionName;
                secNode.ImageIndex = 1;
                secNode.SelectedImageIndex = 1;
                secNode.ContextMenuStrip = cms_explorer;
                trv_explorer.Nodes[0].Nodes.Add(secNode);
            }
            trv_explorer.ExpandAll();
        }

        private void After_Select(object sender, TreeViewEventArgs e)
        {
            if (trv_explorer.SelectedNode.Name.Contains("examNode"))
            {
                newQuestionToolStripButton.Enabled = false;
            }
            else
            {
                newQuestionToolStripButton.Enabled = true;
            }
            if (trv_explorer.SelectedNode.Name.Contains("questionNode"))
            {
                tab_display_questions.Enabled = true;
                tabControl.SelectedIndex = 1;
                tab_display_questions.Text = "Section: " + trv_explorer.SelectedNode.Parent.Text + ", " + trv_explorer.SelectedNode.Text;
                PullNextQuestion(trv_explorer.SelectedNode);
            }
            else
            {
                tab_display_questions.Enabled = false;
            }
        }

        private void Before_Select(object sender, TreeViewCancelEventArgs e)
        {
            if (trv_explorer.SelectedNode != null)
                if (trv_explorer.SelectedNode.Name.Contains("questionNode"))
                    PushPreviousQuestion(trv_explorer.SelectedNode);
        }

        private void Add_Question(object sender, EventArgs e)
        {
            TreeNode quesNode = new TreeNode();
            quesNode.ImageIndex = 2;
            quesNode.SelectedImageIndex = 2;
            quesNode.ContextMenuStrip = cms_explorer;
            if (trv_explorer.SelectedNode.Name.Contains("questionNode"))
            {
                quesNode.Text = "Question " + (trv_explorer.SelectedNode.Parent.Nodes.Count + 1);
                quesNode.Name = "questionNode" + trv_explorer.SelectedNode.Parent.Nodes.Count;
                trv_explorer.SelectedNode.Parent.Nodes.Add(quesNode);
            }
            else
            {
                quesNode.Text = "Question " + (trv_explorer.SelectedNode.Nodes.Count + 1);
                quesNode.Name = "questionNode" + trv_explorer.SelectedNode.Nodes.Count;
                trv_explorer.SelectedNode.Nodes.Add(quesNode);
            }
            trv_explorer.ExpandAll();
            NodeChanged(trv_explorer, null);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sectionTitle;
            if (trv_explorer.SelectedNode.Name.Contains("ques"))
            {
                sectionTitle = trv_explorer.SelectedNode.Parent.Text;
                trv_explorer.SelectedNode.Parent.Nodes.RemoveAt(trv_explorer.SelectedNode.Index);
                rawQuestionList.Remove(rawQuestionList.Find(s => s.SectionTitle == sectionTitle && s.QuestionNumber == int.Parse(trv_explorer.SelectedNode.Text.Replace("Question ", ""))));
            }
            else if (trv_explorer.SelectedNode.Name.Contains("sec"))
            {
                sectionTitle = trv_explorer.SelectedNode.Text;
                List<Question> sectionQuestions = new List<Question>();
                trv_explorer.SelectedNode.Parent.Nodes.RemoveAt(trv_explorer.SelectedNode.Index);
                sectionQuestions = rawQuestionList.Where(s => s.SectionTitle == sectionTitle).ToList();
                foreach (var sectionQuestion in sectionQuestions)
                    rawQuestionList.Remove(sectionQuestion);
            }
            NodeChanged(trv_explorer, null);
        }

        private void NodeChanged(object sender, ControlEventArgs e)
        {
            bool atLeastOneQuestionExists = false;
            foreach (TreeNode node in trv_explorer.Nodes[0].Nodes)
            {
                if (node.Nodes.Count > 0)
                    atLeastOneQuestionExists = true;
            }
            if (atLeastOneQuestionExists)
            {
                saveAsToolStripMenuItem.Enabled = true;
                closeExamToolStripMenuItem.Enabled = true;
                printPreviewToolStripMenuItem.Enabled = true;
                printToolStripButton.Enabled = true;
                printToolStripMenuItem.Enabled = true;
            }
            else
            {
                saveAsToolStripMenuItem.Enabled = false;
                closeExamToolStripMenuItem.Enabled = false;
                printPreviewToolStripMenuItem.Enabled = false;
                printToolStripButton.Enabled = false;
                printToolStripMenuItem.Enabled = false;
            }
        }

        private void PushPreviousQuestion(TreeNode questionNode)
        {
            Question queryResult = rawQuestionList.FirstOrDefault(s => s.SectionTitle == questionNode.Parent.Text && s.QuestionNumber == Convert.ToInt32(questionNode.Text.Replace("Question ", "")));
            if (queryResult != null)
            {
                int questionIndex = rawQuestionList.IndexOf(queryResult);
                rawQuestionList[questionIndex].AnswerExplanation = txt_answer_explanation.Text;
                if (pan_options.Controls.Count > 0)
                {
                    if ((pan_options.Controls.OfType<OptionControl>().First(p => p.IsChecked == true)) != null)
                    {
                        rawQuestionList[questionIndex].QuestionAnswer = (pan_options.Controls.OfType<OptionControl>().First<OptionControl>(p => p.IsChecked == true)).OptionLetter;
                    }
                    else
                    {
                        rawQuestionList[questionIndex].QuestionAnswer = 'A';
                    }
                }
                rawQuestionList[questionIndex].QuestionImagePath = pct_question_picture.ImageLocation;
                rawQuestionList[questionIndex].QuestionNumber = Int32.Parse(questionNode.Text.Replace("Question ", ""));
                Dictionary<char, string> questionOptionList = new Dictionary<char, string>();
                foreach (OptionControl option in (pan_options.Controls.OfType<OptionControl>()))
                {
                    questionOptionList.Add(option.OptionLetter, option.OptionText);
                }
                rawQuestionList[questionIndex].QuestionOptions = questionOptionList;
                rawQuestionList[questionIndex].QuestionText = txt_question_text.Text;
                rawQuestionList[questionIndex].SectionTitle = questionNode.Parent.Text;
            }
            else
            {
                Question question = new Question();
                question.AnswerExplanation = txt_answer_explanation.Text;
                if (pan_options.Controls.Count > 0)
                {
                    try
                    {
                        if ((pan_options.Controls.OfType<OptionControl>().First(p => p.IsChecked == true)) != null)
                        {
                            question.QuestionAnswer = (pan_options.Controls.OfType<OptionControl>().First<OptionControl>(p => p.IsChecked == true)).OptionLetter;
                        }
                        else
                        {
                            question.QuestionAnswer = 'A';
                        }
                    }
                    catch(InvalidOperationException)
                    {
                        question.QuestionAnswer = 'A';
                    }
                }
                question.QuestionImagePath = pct_question_picture.ImageLocation;
                question.QuestionNumber = Int32.Parse(questionNode.Text.Replace("Question ", ""));
                Dictionary<char, string> questionOptionList = new Dictionary<char, string>();
                foreach (OptionControl option in (pan_options.Controls.OfType<OptionControl>()))
                {
                    questionOptionList.Add(option.OptionLetter, option.OptionText);
                }
                question.QuestionOptions = questionOptionList;
                question.QuestionText = txt_question_text.Text;
                question.SectionTitle = questionNode.Parent.Text;
                rawQuestionList.Add(question);
            }
        }

        private void PullNextQuestion(TreeNode questionNode)
        {
            Question queryResult = rawQuestionList.FirstOrDefault(s => s.SectionTitle == questionNode.Parent.Text && s.QuestionNumber == Convert.ToInt32(questionNode.Text.Replace("Question ", "")));
            if (queryResult != null)
            {
                ClearControls();
                int questionIndex = rawQuestionList.IndexOf(queryResult);
                txt_question_text.Text = rawQuestionList[questionIndex].QuestionText;
                txt_answer_explanation.Text = rawQuestionList[questionIndex].AnswerExplanation;
                if (string.IsNullOrWhiteSpace(rawQuestionList[questionIndex].QuestionImagePath))
                {
                    pct_question_picture.Image = null;
                    pct_question_picture.ImageLocation = null;
                    btn_clear_picture.Visible = false;
                    btn_select_picture.Visible = false;
                    pct_question_picture.Visible = false;
                }
                else
                {
                    pct_question_picture.Image = new Bitmap(rawQuestionList[questionIndex].QuestionImagePath);
                    pct_question_picture.ImageLocation = rawQuestionList[questionIndex].QuestionImagePath;
                    btn_clear_picture.Visible = true;
                    btn_select_picture.Visible = true;
                    pct_question_picture.Visible = true;
                }
                int i = 0;
                foreach(var option in rawQuestionList[questionIndex].QuestionOptions)
                {
                    OptionControl oc = new OptionControl();
                    oc.OptionLetter = option.Key;
                    oc.OptionText = option.Value;
                    oc.Location = new Point(2, i * 36);
                    if (option.Key == rawQuestionList[questionIndex].QuestionAnswer)
                    {
                        oc.IsChecked = true;
                    }
                    pan_options.Controls.Add(oc);
                    i++;
                }
            }
            else
            {
                ClearControls();
            }
        }

        private void ClearControls()
        {
            txt_answer_explanation.Clear();
            txt_question_text.Clear();
            pct_question_picture.ImageLocation = null;
            pct_question_picture.Visible = false;
            btn_clear_picture.Visible = false;
            btn_select_picture.Visible = false;
            pan_options.Controls.Clear();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void CloseExam(object sender, EventArgs e)
        {
            if (trv_explorer.Nodes.Count > 0)
                //To make sure user saves changes before closing
                if (!AllChangesSaved)
                {
                    DialogResult result = MessageBox.Show("The current exam has not been saved, are you sure you want to close it?", "Unsaved Changes",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                        return;
                }

            /*
             * Exam closing statements
             * resetting variables
             * resetting controls etc
             */
            ClearControls();
            savedAs = false;
            if (tabControl.TabPages.Count == 2)
                tabControl.TabPages.Insert(0, tab_start);
            AllChangesSaved = false;
            rawQuestionList = null;
            CurrentFileNameWithExtension = null;

            txt_answer_explanation.Clear();
            txt_exam_code.Clear();
            txt_exam_instructions.Clear();
            txt_exam_title.Clear();
            txt_question_text.Clear();
            num_passing_score.Value = 0;
            num_time_limit.Value = 1;
            trv_explorer.Nodes.Clear();
            tabControl.SelectedIndex = 0;

            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            newSectionToolStripButton.Enabled = false;
            newQuestionToolStripButton.Enabled = false;
            closeExamToolStripMenuItem.Enabled = false;
            printPreviewToolStripMenuItem.Enabled = false;
            printToolStripButton.Enabled = false;
            printToolStripMenuItem.Enabled = false;

            tab_display_questions.Enabled = false;
            tab_exam_properties.Enabled = false;
            tab_display_questions.Text = "Section:";
        }

        private void btn_add_image_Click(object sender, EventArgs e)
        {
            btn_clear_picture.Visible = true;
            btn_select_picture.Visible = true;
            pct_question_picture.Visible = true;
        }

        private void btn_clear_picture_Click(object sender, EventArgs e)
        {
            pct_question_picture.ImageLocation = null;
        }

        private void btn_select_picture_Click(object sender, EventArgs e)
        {
            ofd_select_image.ShowDialog();
            if (!string.IsNullOrWhiteSpace(ofd_select_image.FileName))
            {
                pct_question_picture.ImageLocation = ofd_select_image.FileName;
                AllChangesSaved = false;
            }
        }

        private void btn_remove_option_Click(object sender, EventArgs e)
        {
            pan_options.Controls.Remove(pan_options.Controls.OfType<OptionControl>().ElementAt(pan_options.Controls.OfType<OptionControl>().Count() - 1));
        }

        private void btn_add_option_Click(object sender, EventArgs e)
        {
            if (pan_options.Controls.Count > 0)
            {
                    OptionControl oc = new OptionControl();
                    oc.Name = "option" + (pan_options.Controls.Count - 1);
                    oc.OptionLetter = (char)(Convert.ToInt32(((OptionControl)pan_options.Controls[(pan_options.Controls.Count - 1)]).OptionLetter) + 1);
                    oc.Location = new Point(2, 2 + ((pan_options.Controls.Count) * 36));
                    pan_options.Controls.Add(oc);
            }
            else
            {
                OptionControl oc = new OptionControl();
                oc.Location = new Point(2, 2);
                oc.Name = "option0";
                oc.OptionLetter = 'A';
                pan_options.Controls.Add(oc);
            }
        }

        private void Control_Changed(object sender, ControlEventArgs e)
        {
            if (pan_options.Controls.Count <= 0)
            {
                btn_remove_option.Enabled = false;
            }
            else
            {
                btn_remove_option.Enabled = true;
            }
            AllChangesSaved = false;
        }

        private Dictionary<string,List<Question>> GetDictionaryFromQuestionList(List<Question> questionList)
        {
            Dictionary<string, List<Question>> dictionary = new Dictionary<string, List<Question>>();
            foreach(TreeNode sectionNode in trv_explorer.Nodes[0].Nodes)
            {
                dictionary.Add(sectionNode.Text, questionList.FindAll(s => s.SectionTitle == sectionNode.Text));
            }
            return dictionary;
        }

        private void SaveXMLFromDictionary (Dictionary<string,List<Question>> questionDictionary)
        {
            XmlDocument exam = new XmlDocument();

            XmlNode rootNode = exam.CreateElement("OpenExamDocument");
            exam.AppendChild(rootNode);

            XmlNode fileVersion = exam.CreateElement("FileVersion");
            fileVersion.InnerText = "2.0";
            rootNode.AppendChild(fileVersion);

            XmlNode comment = exam.CreateComment("This document contains the details of an Open Exam Suite exam, please do not modify!");
            rootNode.AppendChild(comment);

            //Exam Details
            XmlNode examDetails = exam.CreateElement("ExamDetails");
            rootNode.AppendChild(examDetails);

            XmlNode examTitle = exam.CreateElement("ExamTitle");
            examTitle.InnerText = txt_exam_title.Text;
            examDetails.AppendChild(examTitle);

            XmlNode timeAllowed = exam.CreateElement("TimeAllowed");
            timeAllowed.InnerText = num_time_limit.Value.ToString();
            examDetails.AppendChild(timeAllowed);

            XmlNode passingScore = exam.CreateElement("PassingScore");
            passingScore.InnerText = num_passing_score.Value.ToString();
            examDetails.AppendChild(passingScore);

            XmlNode examCode = exam.CreateElement("ExamCode");
            examCode.InnerText = txt_exam_code.Text;
            examDetails.AppendChild(examCode);

            XmlNode examInstructions = exam.CreateElement("ExamInstructions");
            examInstructions.InnerText = txt_exam_instructions.Text;
            examDetails.AppendChild(examInstructions);

            //Questions Group
            XmlNode questions = exam.CreateElement("Questions");
            rootNode.AppendChild(questions);

            //Sections and Questions
            foreach (var sectionAndQuestions in questionDictionary)
            {
                //Sections
                XmlNode sectionNode = exam.CreateElement("Section");
                XmlAttribute sectionAttribute = exam.CreateAttribute("Title");
                sectionAttribute.Value = sectionAndQuestions.Key;
                sectionNode.Attributes.Append(sectionAttribute);
                questions.AppendChild(sectionNode);

                //Questions
                foreach (Question quest in sectionAndQuestions.Value)
                {
                    XmlNode questionNode = exam.CreateElement("Question");
                    XmlAttribute attrib = exam.CreateAttribute("No");
                    attrib.Value = quest.QuestionNumber.ToString();
                    questionNode.Attributes.Append(attrib);
                    sectionNode.AppendChild(questionNode);

                    XmlNode questionText = exam.CreateElement("Text");
                    questionText.InnerText = quest.QuestionText;
                    questionNode.AppendChild(questionText);

                    XmlNode questionImage = exam.CreateElement("Image");
                    questionImage.InnerText = Path.GetFileName(quest.QuestionImagePath);
                    questionNode.AppendChild(questionImage);
                    CopyImageToOutputFolder(quest.QuestionImagePath);

                    XmlNode questionOptions = exam.CreateElement("Options");
                    questionNode.AppendChild(questionOptions);

                    foreach (var option in quest.QuestionOptions)
                    {
                        XmlNode questionOption = exam.CreateElement("Option");
                        XmlAttribute optionAttribute = exam.CreateAttribute("Title");
                        optionAttribute.Value = option.Key.ToString();
                        questionOption.Attributes.Append(optionAttribute);
                        questionOption.InnerText = option.Value;
                        questionOptions.AppendChild(questionOption);
                    }

                    XmlNode questionAnswer = exam.CreateElement("Answer");
                    questionAnswer.InnerText = quest.QuestionAnswer.ToString();
                    questionNode.AppendChild(questionAnswer);

                    XmlNode answerExplanation = exam.CreateElement("AnswerExplanation");
                    answerExplanation.InnerText = quest.AnswerExplanation;
                    questionNode.AppendChild(answerExplanation);
                }
            }
            string path = Path.Combine(Path.Combine(GlobalPathVariables.creatorFolderPath,
                Path.GetFileNameWithoutExtension(CurrentFileNameWithExtension)), Path.GetFileNameWithoutExtension(CurrentFileNameWithExtension) + ".xml");
            string[] xmlfiles = Directory.GetFiles(Path.Combine(GlobalPathVariables.creatorFolderPath,
                Path.GetFileNameWithoutExtension(CurrentFileNameWithExtension)), "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string xmlfile in xmlfiles)
                File.Delete(xmlfile);
            exam.Save(path);
        }

        private void CopyImageToOutputFolder(string imagePath)
        {
            if (!string.IsNullOrWhiteSpace(imagePath))
                try
                {
                    File.Copy(imagePath, Path.Combine(Path.Combine(GlobalPathVariables.creatorFolderPath,
                        Path.GetFileNameWithoutExtension(CurrentFileNameWithExtension)), Path.GetFileName(imagePath)), true);
                }
                catch (IOException)
                { }
        }

        private void Changes_Made(object sender, EventArgs e)
        {
            AllChangesSaved = false;
        }

        private void tab_start_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ExamHistory != null)
            {
                foreach(Control link in grp_exam_list.Controls.OfType<LinkLabel>())
                {
                    grp_exam_list.Controls.Remove(link);
                }
                int i = 0;
                foreach(string exam in Properties.Settings.Default.ExamHistory)
                {
                    LinkLabel examLink = new LinkLabel();
                    examLink.Location = new Point(12, (55 + (i * 24)));
                    examLink.AutoSize = true;
                    examLink.Text = exam;
                    examLink.Click += examLink_Click;
                    grp_exam_list.Controls.Add(examLink);
                    i++;
                }
            }
        }

        void examLink_Click(object sender, EventArgs e)
        {
            if (File.Exists(((LinkLabel)sender).Text))
            {
                Open(((LinkLabel)sender).Text);
            }
            else
            {
                MessageBox.Show("Sorry, the selected file has been moved or deleted.", "Access error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Properties.Settings.Default.ExamHistory.Remove(((LinkLabel)sender).Text);
                Properties.Settings.Default.Save();
                grp_exam_list.Controls.Remove(((Control)sender));
            }
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (trv_explorer.Nodes.Count > 0)
                //To make sure user saves changes before closing
                if (!AllChangesSaved)
                {
                    DialogResult result = MessageBox.Show("The current exam has not been saved, are you sure you want to close it?", "Unsaved Changes",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.No)
                        throw new NotImplementedException("Prevent form from closing");
                }
        }

        private void EnableEditTools(object sender, EventArgs e)
        {
            pasteToolStripButton.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            cutToolStripButton.Enabled = true;
            cutToolStripMenuItem.Enabled = true;
            copyToolStripButton.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
        }

        private void DisableEditTools(object sender, EventArgs e)
        {
            pasteToolStripButton.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            cutToolStripButton.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
            copyToolStripButton.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
        }
        
        private void pdc_doc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            if (whatToPrint == PrintOption.CurrentQuestion)
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                Font normFont = new System.Drawing.Font("Calibri", 12);
                Font subHeadFont = new System.Drawing.Font("Calibri", 13F);
                Font headerFont = new System.Drawing.Font("Cambria", 14, FontStyle.Bold);

                e.Graphics.DrawString("OPEN EXAM SUITE - CREATOR", headerFont, Brushes.Purple, new PointF(250, yPos));
                yPos += 2 * headerFont.GetHeight(e.Graphics);

                e.Graphics.DrawString("EXAM: " + txt_exam_title.Text + "  EXAM CODE: " + txt_exam_code.Text, subHeadFont, Brushes.Green, new PointF(200, yPos));
                yPos += 2 * subHeadFont.GetHeight(e.Graphics);

                e.Graphics.DrawString("Section: " + trv_explorer.SelectedNode.Parent.Text, subHeadFont, Brushes.Green, new PointF(leftMargin, yPos));
                yPos += 2 * subHeadFont.GetHeight(e.Graphics);

                e.Graphics.DrawString(trv_explorer.SelectedNode.Text, subHeadFont, Brushes.Black, new PointF(leftMargin, yPos));
                yPos += subHeadFont.GetHeight(e.Graphics);

                e.Graphics.DrawString(txt_question_text.Text, normFont, Brushes.Black, new RectangleF(leftMargin, yPos, e.MarginBounds.Width + 60, 150),
                    StringFormat.GenericTypographic);
                yPos += 150;

                yPos += subHeadFont.GetHeight(e.Graphics);
                if (pct_question_picture.Image != null)
                {
                    yPos += 50;
                    e.Graphics.DrawImage(pct_question_picture.Image, new Rectangle(Convert.ToInt32(leftMargin + 100), Convert.ToInt32(yPos + 15), 450, 400));
                    yPos += 400;
                }

                foreach (OptionControl control in pan_options.Controls)
                {
                    string temp = control.OptionLetter + ". -  " + control.OptionText;
                    e.Graphics.DrawString(temp, normFont, Brushes.Black, new PointF(leftMargin + 35, yPos));
                    yPos += normFont.GetHeight(e.Graphics);
                }
            }
            else if (whatToPrint == PrintOption.CurrentSection)
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                Font normFont = new System.Drawing.Font("Calibri", 12);
                Font subHeadFont = new System.Drawing.Font("Calibri", 13F);
                Font headerFont = new System.Drawing.Font("Cambria", 14, FontStyle.Bold);

                if (firstPage)
                {
                    e.Graphics.DrawString("OPEN EXAM SUITE - CREATOR", headerFont, Brushes.Purple, new PointF(250, yPos));
                    yPos += 2 * headerFont.GetHeight(e.Graphics);

                    e.Graphics.DrawString("EXAM: " + txt_exam_title.Text + "  EXAM CODE: " + txt_exam_code.Text, subHeadFont, Brushes.Green, new PointF(200, yPos));
                    yPos += 2 * subHeadFont.GetHeight(e.Graphics);

                    if (trv_explorer.SelectedNode.Name.Contains("ques"))
                    {
                        e.Graphics.DrawString("Section: " + trv_explorer.SelectedNode.Parent.Text, subHeadFont, Brushes.Green, new PointF(leftMargin, yPos));
                        yPos += 2 * subHeadFont.GetHeight(e.Graphics);
                        sectionQuestions = rawQuestionList.Where(s => s.SectionTitle == trv_explorer.SelectedNode.Parent.Text);
                    }
                    else
                    {
                        e.Graphics.DrawString("Section: " + trv_explorer.SelectedNode.Text, subHeadFont, Brushes.Green, new PointF(leftMargin, yPos));
                        yPos += 2 * subHeadFont.GetHeight(e.Graphics);
                        sectionQuestions = rawQuestionList.Where(s => s.SectionTitle == trv_explorer.SelectedNode.Text);
                    }
                    firstPage = false;
                }

                for (; count < sectionQuestions.Count(); count++)
                {
                    if (yPos > e.MarginBounds.Bottom -50)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                    e.Graphics.DrawString("Question " + sectionQuestions.ElementAt(count).QuestionNumber, subHeadFont, Brushes.Black, new PointF(leftMargin, yPos));
                    yPos += subHeadFont.GetHeight(e.Graphics);

                    e.Graphics.DrawString(sectionQuestions.ElementAt(count).QuestionText, normFont, Brushes.Black, new RectangleF(leftMargin, yPos, e.MarginBounds.Width + 60, 100),
                        StringFormat.GenericTypographic);

                    yPos += 3 * subHeadFont.GetHeight(e.Graphics);
                    if (!string.IsNullOrWhiteSpace(sectionQuestions.ElementAt(count).QuestionImagePath))
                    {
                        yPos += 50;
                        e.Graphics.DrawImage(new Bitmap(sectionQuestions.ElementAt(count).QuestionImagePath), new Rectangle(Convert.ToInt32(leftMargin + 100), Convert.ToInt32(yPos + 15), 450, 400));
                        yPos += 400;
                    }

                    foreach (KeyValuePair<char,string> control in sectionQuestions.ElementAt(count).QuestionOptions)
                    {
                        string temp = control.Key + ". -  " + control.Value;
                        e.Graphics.DrawString(temp, normFont, Brushes.Black, new PointF(leftMargin + 35, yPos));
                        yPos += normFont.GetHeight(e.Graphics);
                    }
                }
                e.HasMorePages = false;
            }
            else
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                Font normFont = new System.Drawing.Font("Calibri", 12);
                Font subHeadFont = new System.Drawing.Font("Calibri", 13F);
                Font headerFont = new System.Drawing.Font("Cambria", 14, FontStyle.Bold);

                if (firstPage)
                {
                    e.Graphics.DrawString("OPEN EXAM SUITE - CREATOR", headerFont, Brushes.Purple, new PointF(250, yPos));
                    yPos += 2 * headerFont.GetHeight(e.Graphics);

                    e.Graphics.DrawString("EXAM: " + txt_exam_title.Text + "  EXAM CODE: " + txt_exam_code.Text, subHeadFont, Brushes.Green, new PointF(200, yPos));
                    yPos += 2 * subHeadFont.GetHeight(e.Graphics);

                    Dictionary<string, List<Question>> questionDictionary = GetDictionaryFromQuestionList(rawQuestionList);
                    allQuestions = GlobalPathVariables.GetListFromDictionaryList(questionDictionary.ToList());

                    firstPage = false;
                }

                for (; count < allQuestions.Count; count++)
                {
                    if (allQuestions.ElementAt(count).SectionTitle != sectionName)
                    {
                        e.Graphics.DrawString("Section: " + allQuestions.ElementAt(count).SectionTitle, subHeadFont, Brushes.Green, new PointF(leftMargin, yPos));
                        yPos += 2 * subHeadFont.GetHeight(e.Graphics);
                        sectionName = allQuestions.ElementAt(count).SectionTitle;
                    }

                    if (yPos > e.MarginBounds.Bottom - 50)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    e.Graphics.DrawString("Question " + allQuestions.ElementAt(count).QuestionNumber, subHeadFont, Brushes.Black, new PointF(leftMargin, yPos));
                    yPos += subHeadFont.GetHeight(e.Graphics);

                    e.Graphics.DrawString(allQuestions.ElementAt(count).QuestionText, normFont, Brushes.Black, new RectangleF(leftMargin, yPos, e.MarginBounds.Width + 60, 100),
                        StringFormat.GenericTypographic);

                    yPos += 3 * subHeadFont.GetHeight(e.Graphics);
                    if (!string.IsNullOrWhiteSpace(allQuestions.ElementAt(count).QuestionImagePath))
                    {
                        yPos += 50;
                        e.Graphics.DrawImage(new Bitmap(allQuestions.ElementAt(count).QuestionImagePath), new Rectangle(Convert.ToInt32(leftMargin + 100), Convert.ToInt32(yPos + 15), 450, 400));
                        yPos += 400;
                    }

                    foreach (KeyValuePair<char, string> control in allQuestions.ElementAt(count).QuestionOptions)
                    {
                        string temp = control.Key + ". -  " + control.Value;
                        e.Graphics.DrawString(temp, normFont, Brushes.Black, new PointF(leftMargin + 35, yPos));
                        yPos += normFont.GetHeight(e.Graphics);
                    }
                }
                e.HasMorePages = false;
            }
        }

        private void examPropertiesChanged(object sender, EventArgs e)
        {
            AllChangesSaved = false;
        }

        private void pdc_doc_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            firstPage = true;
        }
    }
}