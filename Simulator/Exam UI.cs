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

namespace OpenExam_Suite
{
    public partial class Exam_UI : Form
    {
        //Global Variables
        int timeLeft;
        string filename;
        string outputPath;
        List<Question> questions;
        int questionIndex;
        char[] givenAnswers;
        int totalSeconds;
        string examCode;
        int passingScore;

        public Exam_UI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The alternate and correct constructor
        /// </summary>
        /// <param name="fileName">the name of the exam file without extensions</param>
        public Exam_UI(string fileName)
        {
            InitializeComponent();
            timeLeft = Properties.Settings.Default.TimerValue * 60;
            this.filename = fileName;
            this.outputPath = Properties.Settings.Default.ExamPath;
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
            for (int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
            {
                var ctrls = pan_display.Controls.OfType<RadioButton>();
                var ctrl = ctrls.ElementAt(j);
                if (((RadioButton)ctrl).Checked)
                {
                    givenAnswers[questionIndex] = Convert.ToChar(ctrl.Name.Replace("rdb_", ""));
                }
                pan_display.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
            timer.Stop();
            //determine how many answers are correct and get section details
            int numOfCorrect = 0;
            int total;
            Dictionary<string, int> totalQuestionsPerSection = new Dictionary<string, int>();
            Dictionary<string, int> rightQuestionsPerSection = new Dictionary<string, int>();
            string temp = "";
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
                }
            }
            total = questions.Count;
            Score_Sheet scs = new Score_Sheet(Properties.Settings.Default.CandidatesName, Properties.Settings.Default.TimerValue, totalSeconds / 60, examCode, ((numOfCorrect / total) * 1000), passingScore, totalQuestionsPerSection, rightQuestionsPerSection);
            this.Hide();
            scs.ShowDialog();
            this.Close();
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            //pan_display.Controls.Clear();
            DisplayQuestion("next");
        }

        private void btn_previous_Click(object sender, EventArgs e)
        {
            //pan_display.Controls.Clear();
            DisplayQuestion("prev");
        }

        private void btn_begin_Click(object sender, EventArgs e)
        {
            //pan_display.Controls.Clear();
            lbl_exam_code.Visible = false;
            lbl_exam_instructions.Visible = false;
            lbl_exam_title.Visible = false;
            //
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
            lbl_elapsed_time.Visible = true;
            lbl_elapsed_time.Enabled = true;
            label1.Enabled = true;
            label1.Visible = true;
            timer.Start();
            DisplayQuestion("begin");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                // Display the new time left 
                // by updating the Time Left label.
                timeLeft = timeLeft - 1;
                double x = timeLeft/3600;
                double hours = Math.Floor(x);
                double y = (timeLeft % 3600) / 60;
                double minutes = Math.Floor(y);
                double seconds = timeLeft % 60;
                string temp = String.Format("{0:0}:{1:00}:{2:00}", hours.ToString(), minutes.ToString(), seconds.ToString());
                lbl_elapsed_time.Text = temp;
                this.totalSeconds += 1;
            }
            else
            {
                // If the user ran out of time, stop the timer, show 
                // a MessageBox, and fill in the answers.
                timer.Stop();
                lbl_elapsed_time.Text = "Time's up!";
            }
        }

        private void Exam_UI_Load(object sender, EventArgs e)
        {
            try
            {
                XPathDocument doc = new XPathDocument(outputPath + Path.DirectorySeparatorChar + filename + ".xml");
                XPathNavigator nav = doc.CreateNavigator();
                // Compile a standard XPath expression
                XPathExpression expr;
                expr = nav.Compile("//ExamTitle");
                XPathNodeIterator iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    lbl_exam_title.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//ExamCode");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    lbl_exam_code.Text = iterator.Current.Value;
                    this.examCode = iterator.Current.Value;
                }
                expr = nav.Compile("//ExamInstructions");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    lbl_exam_instructions.Text = iterator.Current.Value;
                }
                expr = nav.Compile("//PassingScore");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    this.passingScore = Convert.ToInt32(iterator.Current.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            questions = new List<Question>();
            questions = Question.GetQuestions(outputPath);
            givenAnswers = new char[questions.Count];
        }

        /// <summary>
        /// A method to remove the empty strings in the selected strings value
        /// </summary>
        /// <param name="sections">the sring gotten from settings</param>
        /// <returns>a string array containing the title of the selected sections</returns>
        public static string[] GetSections (string sections)
        {
            List<string> temp2 = new List<string>();
            try
            {
                string[] temp1 = sections.Split(',');
                for (int i = 0; i < temp1.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(temp1[i]))
                    {
                        //blah
                    }
                    else
                    {
                        temp2.Add(temp1[i]);
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Please select at least one section", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string[] returnValue = temp2.ToArray();
            return returnValue;
        }

        /// <summary>
        /// Displays the question depending on the given option
        /// </summary>
        /// <param name="option">can be "begin", "next" or "prev" depending on situation</param>
        public void DisplayQuestion (string option)
        {
            //display the controls
            lbl_question_number.Visible = true;
            lbl_section_title.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            txt_question.Visible = true;
            pic_image.Visible = true;
            //
            if (option == "begin")
            {
                this.questionIndex = 0;
                Question temp = questions.ElementAt(questionIndex);
                lbl_question_number.Text = temp.QuestionNumber.ToString();
                lbl_section_title.Text = temp.SectionTitle;
                txt_question.Text = temp.QuestionText;
                if (!(string.IsNullOrWhiteSpace(temp.QuestionImagePath)))
                {
                    string imagePath = Path.Combine(outputPath, temp.QuestionImagePath);
                    pic_image.ImageLocation = imagePath;
                }
                for (int i = 0; i < temp.QuestionOptions.Count; i++)
                {
                    RadioButton rdb = new RadioButton();
                    rdb.Text = temp.QuestionOptions.ElementAt(i).Key + ". - " + temp.QuestionOptions.ElementAt(i).Value;
                    rdb.Name = "rdb" + (" " + temp.QuestionOptions.ElementAt(i).Key).Replace(' ', '_');
                    rdb.Location = new Point(51, 464 + (i * 18));
                    pan_display.Controls.Add(rdb);
                }
            }

            if (option == "prev")
            {
                //check the selected answer
                for (int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
                {
                    var ctrls = pan_display.Controls.OfType<RadioButton>();
                    var ctrl = ctrls.ElementAt(j);
                    if (((RadioButton)ctrl).Checked)
                    {
                        givenAnswers[questionIndex] = Convert.ToChar(ctrl.Name.Replace("rdb_", ""));
                    }
                    pan_display.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                if (questionIndex > 0)
                {
                    //btn_previous.Enabled = true;
                    questionIndex -= 1;
                    Question temp = questions.ElementAt(questionIndex);
                    lbl_question_number.Text = temp.QuestionNumber.ToString();
                    lbl_section_title.Text = temp.SectionTitle;
                    txt_question.Text = temp.QuestionText;
                    if (!(string.IsNullOrWhiteSpace(temp.QuestionImagePath)))
                    {
                        string imagePath = Path.Combine(outputPath, temp.QuestionImagePath);
                        pic_image.ImageLocation = imagePath;
                    }
                    else
                    {
                        pic_image.ImageLocation = "";
                    }
                    for (int i = 0; i < temp.QuestionOptions.Count; i++)
                    {
                        RadioButton rdb = new RadioButton();
                        rdb.Text = temp.QuestionOptions.ElementAt(i).Key + ". - " + temp.QuestionOptions.ElementAt(i).Value;
                        rdb.Name = "rdb" + (" " + temp.QuestionOptions.ElementAt(i).Key).Replace(' ', '_');
                        rdb.Location = new Point(51, 464 + (i * 18));
                        if (temp.QuestionOptions.ElementAt(questionIndex).Key == givenAnswers[questionIndex])
                            rdb.Checked = true;
                        pan_display.Controls.Add(rdb);
                    }
                    if (!(string.IsNullOrWhiteSpace(givenAnswers[questionIndex].ToString())))
                    {
                        for (int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
                        {
                            var ctrl = pan_display.Controls[j];
                            if (ctrl.Name == "rdb_" + givenAnswers[questionIndex])
                                ((RadioButton)ctrl).Checked = true;
                        }
                    }
                    if (questionIndex == 0)
                    {
                        btn_previous.Enabled = false;
                    }
                }
            }

            if (option == "next")
            {
                //
                //check the selected answer
                for (int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
                {
                    var ctrls = pan_display.Controls.OfType<RadioButton>();
                    var ctrl = ctrls.ElementAt(j);
                    if (((RadioButton)ctrl).Checked)
                    {
                        givenAnswers[questionIndex] = Convert.ToChar(ctrl.Name.Replace("rdb_", ""));
                    }
                    pan_display.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                questionIndex += 1;
                Question temp = questions.ElementAt(questionIndex);
                lbl_question_number.Text = temp.QuestionNumber.ToString();
                lbl_section_title.Text = temp.SectionTitle;
                txt_question.Text = temp.QuestionText;
                if (!(string.IsNullOrWhiteSpace(temp.QuestionImagePath)))
                {
                    string imagePath = Path.Combine(outputPath, temp.QuestionImagePath);
                    pic_image.ImageLocation = imagePath;
                }
                else
                {
                    pic_image.ImageLocation = "";
                }
                for (int i = 0; i < temp.QuestionOptions.Count; i++)
                {
                    RadioButton rdb = new RadioButton();
                    rdb.Text = temp.QuestionOptions.ElementAt(i).Key + ". - " + temp.QuestionOptions.ElementAt(i).Value;
                    rdb.Name = "rdb" + (" " + temp.QuestionOptions.ElementAt(i).Key).Replace(' ', '_');
                    rdb.Location = new Point(51, 464 + (i * 18));
                    pan_display.Controls.Add(rdb);
                }
                btn_previous.Enabled = true;
                int type = Properties.Settings.Default.ExamType;
                if ((type == 1 && questionIndex == questions.Count - 1) || (type == 2 && questionIndex == Properties.Settings.Default.NumberOfQuestions -1))
                {
                    btn_next.Enabled = false;
                }
            }            
        }

        /// <summary>
        /// Returns the index of the given key in the dictionary
        /// </summary>
        /// <param name="key">The string key for the dictionary</param>
        /// <param name="dictionary">The dictionary to check</param>
        /// <returns></returns>
        public int GetIndex(Dictionary<string, int> dictionary, string key)
        {
            int index = 0;
            for (int i = 0; i < dictionary.Count; i++)
            {
                if (dictionary.ElementAt(i).Key == key)
                {
                    index = 1;
                }
            }
            return index;
        }
    }
}
