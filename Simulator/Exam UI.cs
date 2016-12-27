using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;
using System.IO;

namespace Simulator
{
    public partial class Exam_UI : Form
    {
        //Global Variables
        List<Question> questions;
        char[] givenAnswers;
        int currentQuestionIndex;
        int timeLeft;
        string filename;
        int totalSeconds;
        string examCode;

        public enum NavigationOption
        {
        	Next,
        	Previous,
        	Begin,
        	End
        };
        
        public Exam_UI(string fileName)
        {
            InitializeComponent();
            timeLeft = Properties.Settings.Default.TimerValue * 60;
            this.filename = fileName;
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            timer.Stop();
            DialogResult result = MessageBox.Show("Exam paused, Click 'OK' to continue.", "Paused", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                timer.Start(); 
            }
        }

        private void btn_end_Click(object sender, EventArgs e)
        {
            DisplayQuestion(NavigationOption.End);
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            DisplayQuestion(NavigationOption.Next);
        }

        private void btn_previous_Click(object sender, EventArgs e)
        {
            DisplayQuestion(NavigationOption.Previous);
        }

        private void btn_begin_Click(object sender, EventArgs e)
        {
            lbl_exam_code.Visible = false;
            lbl_exam_instructions.Visible = false;
            lbl_exam_title.Visible = false;
            btn_begin.Enabled = false;
            btn_begin.Visible = false;
            btn_end.Enabled = true;
            btn_end.Visible = true;
            btn_pause.Enabled = true;
            btn_pause.Visible = true;
            btn_previous.Enabled = false;
            btn_previous.Visible = true;
            btn_next.Enabled = true;
            btn_next.Visible = true;
            btn_show_answer.Visible = true;
            btn_show_answer.Enabled = true;
            lbl_elapsed_time.Visible = true;
            lbl_elapsed_time.Enabled = true;
            label1.Enabled = true;
            label1.Visible = true;
            timer.Start();
            DisplayQuestion(NavigationOption.Begin);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft = timeLeft - 1;
                double x = timeLeft/3600;
                double hours = Math.Floor(x);
                double y = (timeLeft % 3600) / 60;
                double minutes = Math.Floor(y);
                double seconds = timeLeft % 60;
                string temp = String.Format("{0}:{1}:{2}", hours.ToString("00"), minutes.ToString("00"), seconds.ToString("00"));
                lbl_elapsed_time.Text = temp;
                this.totalSeconds += 1;
            }
            else
            {
                timer.Stop();
                lbl_elapsed_time.Text = "Time's up!";
                MessageBox.Show("Your time ran out!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btn_end_Click(btn_end, null);
            }
        }

        private void Exam_UI_Load(object sender, EventArgs e)
        {
            try
            {
                XPathDocument doc = new XPathDocument(GlobalPathVariables.GetXmlFilePath(GlobalPathVariables.GetExamFilesFolder(filename)));
                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr;
                expr = nav.Compile("//ExamTitle");
                XPathNodeIterator iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                    lbl_exam_title.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//ExamCode");
                iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                    lbl_exam_code.Text = iterator.Current.Value;
                    this.examCode = iterator.Current.Value;
                }
                expr = nav.Compile("//ExamInstructions");
                iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                    lbl_exam_instructions.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//PassingScore");
                iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                    Properties.Settings.Default.RequiredScore = Convert.ToInt32(iterator.Current.Value);
                }
                Properties.Settings.Default.Save();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("Sorry, the selected exam was corrupted, please re-add the exam before retrying.", "Exam Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GlobalPathVariables.WriteError(ex, this.Name);
                this.Close();
            }

            questions = new List<Question>();
            questions = Question.GetQuestions(GlobalPathVariables.GetExamFilesFolder(filename));
            givenAnswers = new char[questions.Count];
        }
        
        public void DisplayQuestion (NavigationOption option)
        {
            lbl_question_number.Visible = true;
            lbl_answer_explanation.Visible = false;
            lbl_section_title.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            txt_question.Visible = true;
            pic_image.Visible = true;
            
            if (option == NavigationOption.Begin)
            {
                currentQuestionIndex = 0;
                Question question = questions.ElementAt(currentQuestionIndex);
                lbl_question_number.Text = question.QuestionNumber.ToString();
                lbl_section_title.Text = question.SectionTitle;
                txt_question.Text = question.QuestionText;
                lbl_answer_explanation.Text = question.AnswerExplanation;
                if (!(string.IsNullOrWhiteSpace(question.QuestionImagePath)))
                {
                    string imagePath = Path.Combine(GlobalPathVariables.GetExamFilesFolder(filename), question.QuestionImagePath);
                    pic_image.ImageLocation = imagePath;
                }
                for (int i = 0; i < question.QuestionOptions.Count; i++)
                {
                    RadioButton rdb = new RadioButton();
                    rdb.AutoSize = true;
                    rdb.Text = question.QuestionOptions.ElementAt(i).Key + ". - " + question.QuestionOptions.ElementAt(i).Value;
                    rdb.Name = "rdb_" + question.QuestionOptions.ElementAt(i).Key;
                    rdb.Location = new Point(51, 464 + (i * 22));
                    pan_display.Controls.Add(rdb);
                }
                if ((Exam_Settings.ExamType.SelectedSections.ToString() == Properties.Settings.Default.ExamType && questions.Count == 1) || (Exam_Settings.ExamType.SelectedNumber.ToString() == Properties.Settings.Default.ExamType && Properties.Settings.Default.NumOfQuestions == 1))
                    btn_next.Enabled = false;
                this.Invalidate();
            }

            if (option == NavigationOption.Previous)
            {
                //determine the selected answer for this question and save it before moving to the previous question
                for (int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
                {
                    var ctrls = pan_display.Controls.OfType<RadioButton>();
                    var ctrl = ctrls.ElementAt(j);
                    if (((RadioButton)ctrl).Checked)
                    {
                        givenAnswers[currentQuestionIndex] = Convert.ToChar(ctrl.Name.Replace("rdb_", ""));
                    }
                    pan_display.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                if (currentQuestionIndex > 0)
                {
                    currentQuestionIndex -= 1;
                    Question question = questions.ElementAt(currentQuestionIndex);
                    lbl_question_number.Text = question.QuestionNumber.ToString();
                    lbl_section_title.Text = question.SectionTitle;
                    txt_question.Text = question.QuestionText;
                    lbl_answer_explanation.Text = question.AnswerExplanation;
                    if (string.IsNullOrWhiteSpace(question.QuestionImagePath))
                    {
                        pic_image.ImageLocation = "";
                    }
                    else
                    {
                        pic_image.ImageLocation = Path.Combine(GlobalPathVariables.GetExamFilesFolder(filename), question.QuestionImagePath);
                    }
                    for (int i = 0; i < question.QuestionOptions.Count; i++)
                    {
                        RadioButton rdb = new RadioButton();
                        rdb.AutoSize = true;
                        rdb.Text = question.QuestionOptions.ElementAt(i).Key + ". - " + question.QuestionOptions.ElementAt(i).Value;
                        rdb.Name = "rdb_" + question.QuestionOptions.ElementAt(i).Key;
                        rdb.Location = new Point(51, 464 + (i * 22));
                        if (question.QuestionOptions.ElementAt(i).Key == givenAnswers[currentQuestionIndex])
                            rdb.Checked = true;
                        pan_display.Controls.Add(rdb);
                    }
                    if (currentQuestionIndex == 0)
                    {
                        btn_previous.Enabled = false;
                    }
                }
            }

            if (option == NavigationOption.Next)
            {
                //determine the selected answer for this question and save it before moving to the next question
                for (int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
                {
                    var ctrls = pan_display.Controls.OfType<RadioButton>();
                    var ctrl = ctrls.ElementAt(j);
                    if (((RadioButton)ctrl).Checked)
                    { 
                        givenAnswers[currentQuestionIndex] = Convert.ToChar(ctrl.Name.Replace("rdb_", ""));
                    }
                    pan_display.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                currentQuestionIndex += 1;
                Question question = questions.ElementAt(currentQuestionIndex);
                lbl_question_number.Text = question.QuestionNumber.ToString();
                lbl_section_title.Text = question.SectionTitle;
                txt_question.Text = question.QuestionText;
                lbl_answer_explanation.Text = question.AnswerExplanation;
                if (string.IsNullOrWhiteSpace(question.QuestionImagePath))
                {
                    pic_image.ImageLocation = "";
                }
                else
                {
                    pic_image.ImageLocation = Path.Combine(GlobalPathVariables.GetExamFilesFolder(filename), question.QuestionImagePath);
                }
                for (int i = 0; i < question.QuestionOptions.Count; i++)
                {
                    RadioButton rdb = new RadioButton();
                    rdb.AutoSize = true;
                    rdb.Text = question.QuestionOptions.ElementAt(i).Key + ". - " + question.QuestionOptions.ElementAt(i).Value;
                    rdb.Name = "rdb_" + question.QuestionOptions.ElementAt(i).Key;
                    rdb.Location = new Point(51, 464 + (i * 22));
                    if (question.QuestionOptions.ElementAt(i).Key == givenAnswers[currentQuestionIndex])
                        rdb.Checked = true;
                    pan_display.Controls.Add(rdb);
                }
                btn_previous.Enabled = true;
                if ((Exam_Settings.ExamType.SelectedSections.ToString() == Properties.Settings.Default.ExamType && currentQuestionIndex == questions.Count - 1) || (Exam_Settings.ExamType.SelectedNumber.ToString() == Properties.Settings.Default.ExamType && currentQuestionIndex == Properties.Settings.Default.NumOfQuestions))
                    btn_next.Enabled = false;
                this.Invalidate();
            }           
 
            if (option == NavigationOption.End)
            {
                //determine the selected answer for this question and save it before ending the exam
                for (int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
                {
                    var ctrls = pan_display.Controls.OfType<RadioButton>();
                    var ctrl = ctrls.ElementAt(j);
                    if (((RadioButton)ctrl).Checked)
                    {
                        givenAnswers[currentQuestionIndex] = Convert.ToChar(ctrl.Name.Replace("rdb_", ""));
                    }
                    pan_display.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                //Stop the countdown timer
                timer.Stop();
                //determine how many answers are correct and get section details
                int numOfCorrect = 0;
                int total;
                Dictionary<string, int> totalQuestionsPerSection = new Dictionary<string, int>();
                Dictionary<string, int> rightQuestionsPerSection = new Dictionary<string, int>();
                for (int i = 0; i < questions.Count; i++)
                {
                    if (totalQuestionsPerSection.ContainsKey(questions.ElementAt(i).SectionTitle))
                    {
                        totalQuestionsPerSection[questions.ElementAt(i).SectionTitle] += 1;
                    }
                    else
                    {
                        totalQuestionsPerSection.Add(questions.ElementAt(i).SectionTitle, 1);
                    }
                    if (rightQuestionsPerSection.ContainsKey(questions.ElementAt(i).SectionTitle))
                    {
                        if (questions.ElementAt(i).QuestionAnswer == givenAnswers[i])
                        {
                            rightQuestionsPerSection[questions.ElementAt(i).SectionTitle] += 1;
                            numOfCorrect += 1;
                        }
                    }
                    else
                    {
                        rightQuestionsPerSection.Add(questions.ElementAt(i).SectionTitle, 0);
                        if (questions.ElementAt(i).QuestionAnswer == givenAnswers[i])
                        {
                            rightQuestionsPerSection[questions.ElementAt(i).SectionTitle] += 1;
                            numOfCorrect += 1;
                        }
                    }
                }
                total = questions.Count;
                Score_Sheet scs;
                if (Properties.Settings.Default.ExamType == Exam_Settings.ExamType.SelectedNumber.ToString())
                {
                    scs = new Score_Sheet(totalSeconds / 60.0, examCode, ((numOfCorrect * 1000) / Properties.Settings.Default.NumOfQuestions), totalQuestionsPerSection, rightQuestionsPerSection);
                }
                else
                {
                    scs = new Score_Sheet(totalSeconds / 60.0, examCode, ((numOfCorrect * 1000) / total), totalQuestionsPerSection, rightQuestionsPerSection);
                }
                this.Hide();
                scs.ShowDialog();
                this.Close();
            }
        }
        
        private void btn_show_answer_Click(object sender, EventArgs e)
        {
            lbl_answer_explanation.Visible = true;
            RadioButton answer = pan_display.Controls.OfType<RadioButton>().FirstOrDefault(s => s.Name.Replace("rdb_", "") == questions[currentQuestionIndex].QuestionAnswer.ToString());
            if (answer != null)
            {
                int index = pan_display.Controls.IndexOf(answer);
                ((RadioButton)pan_display.Controls[index]).Checked = true;
                ((RadioButton)pan_display.Controls[index]).ForeColor = Color.Green;
            }
        }
    }
}
