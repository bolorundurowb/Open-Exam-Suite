using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using Ionic.Zip;

namespace Creator
{
    public partial class UI : Form
    {
        //Class variables
        Dictionary<string, List<Question>> examQuestions;
        List<Question> tempExamStore;
        string fileNameWithExtension;

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
                btn_save_as.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                closeToolStripMenuItem.Enabled = true;
                insertPictureToolStripMenuItem.Enabled = true;
                newSectionToolStripMenuItem.Enabled = true;
                addOptionToolStripMenuItem.Enabled = true;
                btn_new_section.Enabled = true;
                importToolStripMenuItem.Enabled = true;
                //adding elements to treeview
                TreeNode examNode = new TreeNode();
                examNode.Name = "examNode";
                examNode.Text = Properties.Settings.Default.ExamTitle;
                examNode.ImageIndex = 2;
                examNode.SelectedImageIndex = 2;
                trv_explorer.Nodes.Add(examNode);           //Root node in TreeView
                if (Properties.Settings.Default.SectionTitles.Count > 0)
                {
                    for (int i = 0; i < Properties.Settings.Default.SectionTitles.Count; i++)
                    {
                        TreeNode SectionNode = new TreeNode();
                        SectionNode.Name = "secNode_" + i;
                        SectionNode.Text = Properties.Settings.Default.SectionTitles[i];
                        SectionNode.ImageIndex = 0;
                        SectionNode.SelectedImageIndex = 0;
                        SectionNode.ContextMenuStrip = contextMenuStrip;
                        examNode.Nodes.Add(SectionNode);        //Section Nodes
                    }

                    TreeNode QuestionNode = new TreeNode();
                    QuestionNode.Name = "quesNode_0";
                    QuestionNode.Text = "Question 1";
                    QuestionNode.ImageIndex = 1;
                    QuestionNode.SelectedImageIndex = 1;
                    examNode.Nodes[0].Nodes.Add(QuestionNode);      //The default question Node

                    trv_explorer.SelectedNode = QuestionNode;
                    //Enable Question fillout mode
                    splcn_main_view.Panel2.Enabled = true;
                }
                //Initialize store for Questions
                tempExamStore = new List<Question>();
                trv_explorer.ExpandAll();
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
            if (((TreeView)sender).SelectedNode.Name.Contains("ques"))
            {
                //enable relevant options
                addOptionToolStripMenuItem.Enabled = true;
                newQuestionToolStripMenuItem.Enabled = false;
                btn_new_question.Enabled = false;
                newSectionToolStripMenuItem.Enabled = false;
                btn_new_section.Enabled = false;
                splcn_main_view.Panel2.Enabled = true;
                editToolStripMenuItem1.Enabled = false;
                previousItemToolStripMenuItem.Enabled = true;
                nextItemToolStripMenuItem.Enabled = true;
                //enable printing
                btn_print_exam.Enabled = true;
                printToolStripMenuItem.Enabled = true;
                btn_print_preview.Enabled = true;
                //display question 
                lbl_question_and_section.Text = "Section: " + ((TreeView)sender).SelectedNode.Parent.Text + ", " + ((TreeView)sender).SelectedNode.Text;

                //RefreshInputControls();

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
                        Debug.Print("Error: " + ex.Message + ", Inner Exception: " + ex.InnerException);
                    }
                    finally
                    {
                        if (exists)
                        {
                            txt_question_text.Text = present.QuestionText;
                            if (string.IsNullOrWhiteSpace(present.QuestionImagePath))
                            {
                                pct_question_picture.Image = null;
                                pct_question_picture.ImageLocation = null;
                                btn_clear_picture.Visible = false;
                                btn_select_picture.Visible = false;
                                pct_question_picture.Visible = false;
                            }
                            else
                            {
                                pct_question_picture.Image = new Bitmap(present.QuestionImagePath);
                                pct_question_picture.ImageLocation = present.QuestionImagePath;
                                btn_clear_picture.Visible = true;
                                btn_select_picture.Visible = true;
                                pct_question_picture.Visible = true;
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
                        {
                            btn_select_picture.Visible = false;
                            btn_clear_picture.Visible = false;
                            pct_question_picture.Visible = false;
                        }
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
                previousItemToolStripMenuItem.Enabled = true;
                nextItemToolStripMenuItem.Enabled = true;
                //clear status text
                lbl_question_and_section.Text = "";
                //dislplay edit option
                editToolStripMenuItem1.Enabled = true;
                //disable printing
                btn_print_exam.Enabled = false;
                printToolStripMenuItem.Enabled = false;
                btn_print_preview.Enabled = false;
            }
            //enable add sections
            else if (((TreeView)sender).SelectedNode.Name.Contains("examNode"))
            {
                splcn_main_view.Panel2.Enabled = false;
                newSectionToolStripMenuItem.Enabled = true;
                btn_new_section.Enabled = true;
                newQuestionToolStripMenuItem.Enabled = false;
                btn_new_question.Enabled = false;
                previousItemToolStripMenuItem.Enabled = false;
                nextItemToolStripMenuItem.Enabled = false;
                //display edit option
                editToolStripMenuItem1.Enabled = false;
                //disable printing
                btn_print_exam.Enabled = false;
                printToolStripMenuItem.Enabled = false;
                btn_print_preview.Enabled = false;
            }
            else
            {
                splcn_main_view.Panel2.Enabled = false;
                addOptionToolStripMenuItem.Enabled = false;
                newQuestionToolStripMenuItem.Enabled = false;
                btn_new_question.Enabled = false;
                newSectionToolStripMenuItem.Enabled = false;
                btn_new_section.Enabled = false;
                //disable printing
                btn_print_exam.Enabled = false;
                printToolStripMenuItem.Enabled = false;
                btn_print_preview.Enabled = false;
            }
        }

        /// <summary>
        /// This method removes any data in the input controls
        /// </summary>
        private void RefreshInputControls()
        {
            txt_question_text.Clear();
            pan_options.Controls.Clear();
            pct_question_picture.ImageLocation = null;
            lbl_question_and_section.Text = "";
            lbl_save_status.Text = "";
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
                bool exists = false;
                for (int i = 0; i < trv_explorer.Nodes[0].Nodes.Count; i++)
                {
                    if (trv_explorer.Nodes[0].Nodes[i].Text == section)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    TreeNode node = new TreeNode();
                    node.Name = "secNode" + (trv_explorer.SelectedNode.Nodes.Count);
                    node.Text = section;
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    node.ContextMenuStrip = contextMenuStrip;
                    trv_explorer.SelectedNode.Nodes.Add(node);
                }
            }
            trv_explorer.ExpandAll();
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
                    catch (NullReferenceException ex)
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
                            RefreshInputControls();
                            //add question to store
                            this.tempExamStore.Add(ques);
                        }
                    }
                }
            }
            catch (NullReferenceException)
            { 
                // A necessary Evil
            }
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditSectionTitle edsec = new EditSectionTitle();
            edsec.ShowDialog();
            string newName = edsec.NewName;
            trv_explorer.SelectedNode.Text = newName;
        }

        private void SaveAs(object sender, EventArgs e)
        {
            svf_save_exam.FileName = Properties.Settings.Default.ExamTitle;
            svf_save_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var result = svf_save_exam.ShowDialog();    
            if (result == DialogResult.OK)
            {
                string filename = svf_save_exam.FileName;
                fileNameWithExtension = filename;
                Save(filename);
                btn_save_exam.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Does the job of creating and working on the temp exam files and compiling the final exam file
        /// </summary>
        /// <param name="filePath">The full file path (including the file extension) the file should be saved to</param>
        private void Save(string filePath)
        {
            //Add current question being worked on
            AddCurrentQuestion();
            //create the creator temp folder if it doesnt exist
            if (!Directory.Exists(GlobalPathVariables.creatorFolderPath))
            {
                Directory.CreateDirectory(GlobalPathVariables.creatorFolderPath);
            }
            List<string> tempSections = new List<string>();
            
            //create temp folder for this exam file
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);            
            string tempFolderPath = Path.Combine(GlobalPathVariables.creatorFolderPath, fileNameWithoutExtension);
            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            foreach (TreeNode node in trv_explorer.Nodes[0].Nodes)
            {
                tempSections.Add(node.Text);
            }
            string[] sections = tempSections.ToArray();

            //Get the dictionary of questions
            examQuestions = new Dictionary<string, List<Question>>();
            examQuestions = XML_Handler.ConvertListToFormat(sections, tempExamStore);
            XmlDocument docToBeSaved = XML_Handler.WriteDictionaryToXMLDocument(examQuestions);
            //Save XML
            docToBeSaved.Save(Path.Combine(tempFolderPath, fileNameWithoutExtension + ".xml"));
            //copy requisite images to folder
            foreach (Question ques in tempExamStore)
            {
                try
                {
                    File.Copy(ques.QuestionImagePath, Path.Combine(tempFolderPath, Path.GetFileName(ques.QuestionImagePath)), true);
                }
                catch (ArgumentException exc)
                {
                    Debug.Print("The following error occured: " + exc.Message + "\nInner Exception: " + exc.InnerException);
                }
            }

            //compile the final *.oef file
            using(ZipFile oefFile = new ZipFile())
            {
                foreach (string file in Directory.GetFiles(tempFolderPath))
                {
                    oefFile.AddFile(file, "");
                }
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                oefFile.Save(filePath);
            }
            lbl_save_status.Text = "Exam Saved Successfully!";
            lbl_save_status.Visible = true;
            //Thread
        }

        /// <summary>
        /// This method makes sure when the save method is called the current question is committed
        /// </summary>
        private void AddCurrentQuestion()
        {
            try
            {
                if (trv_explorer.SelectedNode.Name.Contains("ques"))
                {
                    bool exists = false;
                    Question present = new Question();
                    try
                    {
                        var temp = tempExamStore.FindAll(s => s.SectionTitle == (trv_explorer.SelectedNode.Parent.Text));
                        present = temp.SingleOrDefault(c => c.QuestionNumber == Convert.ToInt32(trv_explorer.SelectedNode.Text.Replace("Question ", "")));
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
                            tempExamStore[index].QuestionNumber = Convert.ToInt32(trv_explorer.SelectedNode.Text.Replace("Question ", ""));
                            Dictionary<char, string> tempDic = new Dictionary<char, string>();
                            foreach (var ctrl in pan_options.Controls.OfType<OptionControl>())
                            {
                                tempDic.Add(Convert.ToChar(ctrl.OptionLetter), ctrl.OptionText);
                            }
                            tempExamStore[index].QuestionOptions = tempDic;
                            tempExamStore[index].SectionTitle = trv_explorer.SelectedNode.Parent.Text;
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
                            ques.QuestionNumber = Convert.ToInt32(trv_explorer.SelectedNode.Text.Replace("Question ", ""));
                            Dictionary<char, string> tempDic = new Dictionary<char, string>();
                            foreach (var ctrl in pan_options.Controls.OfType<OptionControl>())
                            {
                                tempDic.Add(Convert.ToChar(ctrl.OptionLetter), ctrl.OptionText);
                            }
                            ques.QuestionOptions = tempDic;
                            ques.QuestionText = txt_question_text.Text;
                            ques.SectionTitle = trv_explorer.SelectedNode.Parent.Text;
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


        

        private void SaveExam(object sender, EventArgs e)
        {
            Save(fileNameWithExtension);
        }

        private void btn_select_picture_Click(object sender, EventArgs e)
        {
            opf_get_files.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (opf_get_files.ShowDialog() == DialogResult.OK)
            {
                pct_question_picture.ImageLocation = opf_get_files.FileName;
            }
        }

        private void txt_question_text_Enter(object sender, EventArgs e)
        {
            btn_paste.Enabled = true;
            btn_cut.Enabled = true;
            btn_undo.Enabled = true;
            btn_redo.Enabled = true;
            btn_copy.Enabled = true;
        }

        private void txt_question_text_Leave(object sender, EventArgs e)
        {
            btn_paste.Enabled = false;
            btn_cut.Enabled = false;
            btn_undo.Enabled = false;
            btn_redo.Enabled = false;
            btn_copy.Enabled = false;
        }

        private void btn_cut_Click(object sender, EventArgs e)
        {
            if (txt_question_text.SelectionLength > 0)
            {
                txt_question_text.Cut();
            }
        }

        private void btn_copy_Click(object sender, EventArgs e)
        {
            if (txt_question_text.SelectionLength > 0)
            {
                txt_question_text.Copy();
            }
        }

        private void btn_paste_Click(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                // Determine if any text is selected in the text box. 
                if (txt_question_text.SelectionLength > 0)
                {
                    // Ask user if they want to paste over currently selected text. 
                    if (MessageBox.Show("Do you want to paste over current selection?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
                        // Move selection to the point after the current selection and paste.
                        txt_question_text.SelectionStart = txt_question_text.SelectionStart + txt_question_text.SelectionLength;
                }
                // Paste current text in Clipboard into text box.
                txt_question_text.Paste();
            }
        }

        private void PrintQuestion(object sender, EventArgs e)
        {
            if (pntdlg_print.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void PrintPreview(object sender, EventArgs e)
        {
            if (pntprvdlg_print.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void CloseExam(object sender, EventArgs e)
        {
            if (trv_explorer.Nodes.Count > 0)
            {
                var result = MessageBox.Show("Have you saved the current exam?", "Close Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    //Clear class variables 
                    try
                    {
                        examQuestions.Clear();
                        tempExamStore.Clear();
                    }
                    catch (NullReferenceException)
                    { }
                    fileNameWithExtension = null;
                    //Clear settings
                    Properties.Settings.Default.Reset();
                    Properties.Settings.Default.Save();
                    //Clear controls
                    txt_question_text.Clear();
                    pct_question_picture.Image = null;
                    pct_question_picture.ImageLocation = null;
                    pan_options.Controls.Clear();
                    trv_explorer.Nodes.Clear();
                    lbl_question_and_section.Text = "";
                    lbl_save_status.Text = "";
                    //Disable Controls
                    btn_print_exam.Enabled = false;
                    btn_print_preview.Enabled = false;
                    btn_save_as.Enabled = false;
                    btn_save_exam.Enabled = false;
                    splcn_main_view.Panel2.Enabled = false;
                    insertPictureToolStripMenuItem.Enabled = false;
                    saveAsToolStripMenuItem.Enabled = false;
                    saveToolStripMenuItem.Enabled = false;
                    importToolStripMenuItem.Enabled = false;
                    //Enable Open
                    btn_open_exam.Enabled = true;
                    openToolStripMenuItem.Enabled = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                //Clear class variables 
                try
                {
                    examQuestions.Clear();
                    tempExamStore.Clear();
                }
                catch (NullReferenceException)
                { }
                fileNameWithExtension = null;
                //Clear settings
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                //Clear controls
                txt_question_text.Clear();
                pct_question_picture.Image = null;
                pct_question_picture.ImageLocation = null;
                pan_options.Controls.Clear();
                trv_explorer.Nodes.Clear();
                lbl_question_and_section.Text = "";
                lbl_save_status.Text = "";
                //Disable Controls
                btn_print_exam.Enabled = false;
                btn_print_preview.Enabled = false;
                btn_save_as.Enabled = false;
                btn_save_exam.Enabled = false;
                splcn_main_view.Panel2.Enabled = false;
                insertPictureToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                importToolStripMenuItem.Enabled = false;
                //Enable Open
                btn_open_exam.Enabled = true;
                openToolStripMenuItem.Enabled = true;
            }
        }

        private void ImportExam(object sender, EventArgs e)
        {
            opf_get_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = "";
            if (opf_get_exam.ShowDialog() == DialogResult.OK)
            {
                filePath = opf_get_exam.FileName;
                XML_Handler.ExtractExamToFolder(filePath);
                Dictionary<string, List<Question>> questionList = new Dictionary<string, List<Question>>();
                questionList = XML_Handler.ReadExamToFormat(GlobalPathVariables.GetXmlFilePath(GlobalPathVariables.GetExamFilesFolder(Path.GetFileNameWithoutExtension(filePath))));
                WriteQuestionsToTreeView(questionList);
                EnableOptions();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseExam(sender, new EventArgs());

            opf_get_exam.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = "";
            if (opf_get_exam.ShowDialog() == DialogResult.OK)
            {
                filePath = opf_get_exam.FileName;
                //Get the basic data
                XML_Handler.ExtractExamToFolder(filePath);
                XPathDocument doc = new XPathDocument(GlobalPathVariables.GetXmlFilePath(GlobalPathVariables.GetExamFilesFolder(Path.GetFileNameWithoutExtension(filePath))));
                XPathNavigator navigator = doc.CreateNavigator();
                XPathExpression expression = navigator.Compile("//FileVersion");
                XPathNodeIterator fvIterator = navigator.Select(expression);
                while (fvIterator.MoveNext())
                {
                    Properties.Settings.Default.FileVersion = fvIterator.Current.Value;
                }
                expression = navigator.Compile("//ExamTitle");
                fvIterator = navigator.Select(expression);
                string examTitle = string.Empty;
                while (fvIterator.MoveNext())
                {
                    Properties.Settings.Default.ExamTitle = fvIterator.Current.Value;
                    examTitle = fvIterator.Current.Value;
                }
                trv_explorer.Nodes.Add(examTitle);
                expression = navigator.Compile("//TimeAllowed");
                fvIterator = navigator.Select(expression);
                while (fvIterator.MoveNext())
                {
                    Properties.Settings.Default.TimeAllowed = Convert.ToInt32(fvIterator.Current.Value);
                }
                expression = navigator.Compile("//PassingScore");
                fvIterator = navigator.Select(expression);
                while (fvIterator.MoveNext())
                {
                    Properties.Settings.Default.PassingScore = Convert.ToInt32(fvIterator.Current.Value);
                }
                expression = navigator.Compile("//ExamCode");
                fvIterator = navigator.Select(expression);
                while (fvIterator.MoveNext())
                {
                    Properties.Settings.Default.ExamCode = fvIterator.Current.Value;
                }
                expression = navigator.Compile("//ExamInstructions");
                fvIterator = navigator.Select(expression);
                while (fvIterator.MoveNext())
                {
                    Properties.Settings.Default.ExamInstructions = fvIterator.Current.Value;
                }
                expression = navigator.Compile("//Section");
                fvIterator = navigator.Select(expression);
                Properties.Settings.Default.SectionTitles = new System.Collections.Specialized.StringCollection();
                while (fvIterator.MoveNext())
                {
                    Properties.Settings.Default.SectionTitles.Add(fvIterator.Current.GetAttribute("Title", ""));
                }
                Properties.Settings.Default.Save();

                Dictionary<string, List<Question>> questionList = new Dictionary<string, List<Question>>();
                questionList = XML_Handler.ReadExamToFormat(GlobalPathVariables.GetXmlFilePath(GlobalPathVariables.GetExamFilesFolder(Path.GetFileNameWithoutExtension(filePath))));
                WriteQuestionsToTreeView(questionList);
                EnableOptions();
            }
        }

        public void WriteQuestionsToTreeView (Dictionary<string, List<Question>> formattedQuestionList)
        {
            tempExamStore = new List<Question>();
            foreach(KeyValuePair<string, List<Question>> section in formattedQuestionList)
            {
                TreeNode sectionNode = new TreeNode();
                sectionNode.Name = "secNode_" + trv_explorer.Nodes[0].GetNodeCount(false);
                sectionNode.Text = section.Key;
                sectionNode.ImageIndex = 0;
                sectionNode.SelectedImageIndex = 0;
                sectionNode.ContextMenuStrip = contextMenuStrip;
                
                int count = 0;
                foreach (Question question in section.Value)
                {
                    TreeNode questionNode = new TreeNode();
                    questionNode.Name = "quesNode_" + count;
                    questionNode.Text = "Question " + (count + 1);
                    questionNode.ImageIndex = 1;
                    questionNode.SelectedImageIndex = 1;
                    sectionNode.Nodes.Add(questionNode);
                    tempExamStore.Add(question);
                    count++;
                }
                trv_explorer.Nodes[0].Nodes.Add(sectionNode);                
            }
        }

        public void EnableOptions()
        {
            trv_explorer.ExpandAll();
            trv_explorer.SelectedNode = trv_explorer.Nodes[0].Nodes[0];
            btn_open_exam.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            importToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            btn_save_as.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            btn_save_exam.Enabled = true;
        }

        private void previousItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trv_explorer.SelectedNode.Parent.Nodes.IndexOf(trv_explorer.SelectedNode) != 0)
            {
                trv_explorer.SelectedNode = trv_explorer.SelectedNode.Parent.Nodes[trv_explorer.SelectedNode.Parent.Nodes.IndexOf(trv_explorer.SelectedNode) - 1];
            }
        }

        private void nextItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trv_explorer.SelectedNode.Parent.Nodes.IndexOf(trv_explorer.SelectedNode) < trv_explorer.SelectedNode.Parent.Nodes.Count - 1)
            {
                trv_explorer.SelectedNode = trv_explorer.SelectedNode.Parent.Nodes[trv_explorer.SelectedNode.Parent.Nodes.IndexOf(trv_explorer.SelectedNode) + 1];
            }
        }

        private void btn_undo_Click(object sender, EventArgs e)
        {
            txt_question_text.Undo();
        }

        private void btn_redo_Click(object sender, EventArgs e)
        {
            txt_question_text.Redo();
        }

        private void pntdoc_print_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ev)
        {
            float yPos = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            Font printFont = new System.Drawing.Font("Calibri", 12);
            Font headerFont = new System.Drawing.Font("Calibri", 14, FontStyle.Bold);

            yPos = topMargin + (headerFont.GetHeight(ev.Graphics));
            ev.Graphics.DrawString(lbl_question_and_section.Text, headerFont, Brushes.Black, leftMargin, yPos, new StringFormat());

            for (int i = 0; i < txt_question_text.Lines.Length; i++)
            {
                yPos = yPos + (printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(txt_question_text.Lines[i], printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
            }

            yPos = yPos + 60;
            if (!((string.IsNullOrWhiteSpace(pct_question_picture.ImageLocation)) || (pct_question_picture.Image == null)))
            {
                ev.Graphics.DrawImage(pct_question_picture.Image, new Rectangle(Convert.ToInt32(leftMargin + 100), Convert.ToInt32(yPos + 15), 450, 400));
            }

            yPos = yPos + 435;
            foreach (OptionControl control in pan_options.Controls)
            {
                string temp;
                temp = control.OptionLetter + ". -  " + control.OptionText;
                ev.Graphics.DrawString(temp, printFont, Brushes.Black, leftMargin + 35, yPos, new StringFormat());
                yPos = yPos + (printFont.GetHeight(ev.Graphics));
            }
        }
    }
}
