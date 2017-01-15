using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Exam_UI : Form
    {
        #region Global Variables
        private Exam exam;
        private Settings settings;
        private int timeLeft;
        private int currentQuestionIndex;
        private object[] userAnswers;
        #endregion

        public Exam_UI(Exam _exam, Settings _settings)
        {
            InitializeComponent();
            exam = _exam;
            settings = _settings;
            timeLeft = _settings.TimeLimit * 60;
            userAnswers = new object[exam.NumberOfQuestions];
        }

        private void TimerTick(object sender, EventArgs e)
        {
            timeLeft--;
            if (timeLeft <= 0)
            {
                timer.Stop();
                lbl_elapsed_time.Text = "Time Up!";
                MessageBox.Show("Your time ran out!", "Time out", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NavigateExam(NavOption.End);
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(timeLeft);
                lbl_elapsed_time.Text = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
            }
        }

        private void Start(object sender, EventArgs e)
        {
            lbl_exam_code.Text = exam.Properties.Code;
            lbl_exam_title.Text = exam.Properties.Title;
            lbl_exam_instructions.Text = exam.Properties.Instructions;
        }

        private void EnableControls()
        {
            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            lbl_question_number.Visible = true;
            lbl_section_title.Visible = true;
            lbl_elapsed_time.Visible = true;
            //
            lbl_exam_code.Visible = false;
            lbl_exam_instructions.Visible = false;
            lbl_exam_title.Visible = false;
            //
            btn_begin.Visible = false;
            //
            btn_end.Visible = true;
            btn_next.Visible = true;
            btn_pause.Visible = true;
            btn_previous.Visible = true;
            btn_show_answer.Visible = true;
            //
            pct_image.Visible = true;
            //
            txt_question.Visible = true;
        }

        private void PauseExam(object sender, EventArgs e)
        {
            timer.Stop();
            MessageBox.Show("Your exam has been paused. Click 'OK' to continue.", "Paused", MessageBoxButtons.OK, MessageBoxIcon.Information);
            timer.Start();
        }

        private void Begin(object sender, EventArgs e)
        {
            timer.Start();
            EnableControls();
            NavigateExam(NavOption.Begin);
        }

        private void Next(object sender, EventArgs e)
        {
            NavigateExam(NavOption.Next);
        }

        private void Previous(object sender, EventArgs e)
        {
            NavigateExam(NavOption.Previous);
        }

        private void End(object sender, EventArgs e)
        {
            NavigateExam(NavOption.End);
        }

        private void NavigateExam(NavOption option)
        {
            lbl_explanation.Visible = false;
            //
            if (option == NavOption.Begin)
            {
                if(settings.Questions.Count > 0)
                {
                    currentQuestionIndex = 0;
                    PrintQuestionToScreen();
                }
                //
                if(settings.Questions.Count > 1)
                {
                    btn_next.Enabled = true;
                }
            }
            else if (option == NavOption.Next)
            {
                //Save current answer
                userAnswers[currentQuestionIndex] = SelectedAnswer();
                //
                RemoveOptions();
                //
                currentQuestionIndex++;
                PrintQuestionToScreen();
                //
                btn_previous.Enabled = true;
                //
                if (currentQuestionIndex == settings.Questions.Count - 1)
                    btn_next.Enabled = false;
            }
            else if (option == NavOption.Previous)
            {
                //Save current answer
                userAnswers[currentQuestionIndex] = SelectedAnswer();
                //
                RemoveOptions();
                //
                currentQuestionIndex--;
                PrintQuestionToScreen();
                //
                btn_next.Enabled = true;
                //
                if (currentQuestionIndex == 0)
                    btn_previous.Enabled = false;
            }
            else if (option == NavOption.End)
            {
                //Save current answer
                userAnswers[currentQuestionIndex] = SelectedAnswer();
                //
                settings.ElapsedTime = TimeSpan.FromSeconds(exam.Properties.TimeLimit * 60 - timeLeft);
                //
                int numOfCorrectAnswers = 0;
                for(int i = 0; i < settings.Questions.Count; i++)
                {
                    if (userAnswers[i].GetType().IsArray)
                    {
                        if (((char[])userAnswers[i]).SequenceEqual(settings.Questions[i].Answers))
                        {
                            numOfCorrectAnswers++;
                        }
                    }
                    else if ((char)userAnswers[i] == settings.Questions[i].Answer)
                    {
                        numOfCorrectAnswers++;
                    }
                }
                settings.NumberOfCorrectAnswers = numOfCorrectAnswers;
                //
                foreach(var section in settings.Sections)
                {
                    int numOfQuestions = 0;
                    int numOfCorrect = 0;
                    for (int i = 0; i < settings.Questions.Count; i++)
                    {
                        if(section.Questions.Contains(settings.Questions[i]))
                        {
                            numOfQuestions++;
                            if (userAnswers[i].GetType().IsArray)
                            {
                                if (((char[])userAnswers[i]).SequenceEqual(settings.Questions[i].Answers))
                                {
                                    numOfCorrect++;
                                }
                            }
                            else if ((char)userAnswers[i] == settings.Questions[i].Answer)
                            {
                                numOfCorrect++;
                            }
                        }
                    }
                    settings.ResultSpread.Add(new Tuple<string, int, int>(section.Title, numOfQuestions, numOfCorrect));
                }
                //
                Score_Sheet ss = new Score_Sheet(settings, exam);
                this.Hide();
                ss.ShowDialog();
                this.Close();
            }
        }

        private void PrintQuestionToScreen()
        {
            Question question = settings.Questions[currentQuestionIndex];
            lbl_question_number.Text = question.No.ToString();
            lbl_section_title.Text = settings.Sections.First(s => s.Questions.Contains(question)).Title;
            lbl_explanation.Text = question.Explanation;
            txt_question.Text = question.Text;
            pct_image.Image = question.Image;
            AddOptions(question.Options, question.IsMultipleChoice);
        }

        private void AddOptions(List<Option> options, bool isMultipleChoice)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (isMultipleChoice)
                {
                    CheckBox chk = new CheckBox()
                    {
                        AutoSize = true,
                        Text = options[i].Alphabet + ". - " + options[i].Text,
                        Name = "chk" + options[i].Alphabet,
                        Location = new Point(51, 464 + (i * 22))
                    };
                    if (userAnswers[currentQuestionIndex] != null && ((char[])userAnswers[currentQuestionIndex]).Contains(options[i].Alphabet))
                        chk.Checked = true;
                    pan_display.Controls.Add(chk);
                }
                else
                {
                    RadioButton rdb = new RadioButton()
                    {
                        AutoSize = true,
                        Text = options[i].Alphabet + ". - " + options[i].Text,
                        Name = "rdb" + options[i].Alphabet,
                        Location = new Point(51, 464 + (i * 22))
                    };
                    if (userAnswers[currentQuestionIndex] != null && (char)userAnswers[currentQuestionIndex] == options[i].Alphabet)
                        rdb.Checked = true;
                    pan_display.Controls.Add(rdb);
                }
            }
        }

        private void RemoveOptions()
        {
            for(int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
            {
                var controls = pan_display.Controls.OfType<RadioButton>();
                var control = controls.ElementAt(j);
                pan_display.Controls.Remove(control);
                control.Dispose();
            }
            for (int j = pan_display.Controls.OfType<CheckBox>().Count() - 1; j >= 0; --j)
            {
                var controls = pan_display.Controls.OfType<CheckBox>();
                var control = controls.ElementAt(j);
                pan_display.Controls.Remove(control);
                control.Dispose();
            }
        }

        private object SelectedAnswer()
        {
            Question currentQuestion = settings.Questions[currentQuestionIndex];
            if (currentQuestion.IsMultipleChoice)
            {
                var chks = pan_display.Controls.OfType<CheckBox>().Where(s => s.Checked);
                if (chks == null || chks.Count() == 0)
                {
                    return new List<char>().ToArray();
                }
                else
                {
                    return chks.Select(s => Convert.ToChar(s.Text.Substring(0, 1))).ToArray();
                }
            }
            else
            {
                var rdb = pan_display.Controls.OfType<RadioButton>().FirstOrDefault(s => s.Checked);
                if (rdb == null)
                {
                    return '\0';
                }
                else
                {
                    return Convert.ToChar(rdb.Text.Substring(0, 1));
                }
            }
        }

        private void ShowAnswer(object sender, EventArgs e)
        {
            lbl_explanation.Visible = true;
            //
            var chks = pan_display.Controls.OfType<CheckBox>();
            if (chks.Count() > 0)
            {
                var answers = chks.Where(s => settings.Questions[currentQuestionIndex].Answers.Contains(Convert.ToChar(s.Name.Replace("chk", ""))));
                foreach(var answer in answers)
                {
                    int index = pan_display.Controls.IndexOf(answer);
                    ((CheckBox)pan_display.Controls[index]).ForeColor = Color.Green;
                }
                var selectedOptions = chks.Where(s => s.Checked);
                foreach (var selectedOption in selectedOptions)
                {
                    if (!settings.Questions[currentQuestionIndex].Answers.Contains(Convert.ToChar(selectedOption.Name.Replace("chk", ""))))
                    {
                        int index = pan_display.Controls.IndexOf(selectedOption);
                        ((CheckBox)pan_display.Controls[index]).ForeColor = Color.Red;
                    }
                }
            }
            else
            {
                RadioButton answer = pan_display.Controls.OfType<RadioButton>().FirstOrDefault(s => s.Name.Replace("rdb", "") == settings.Questions[currentQuestionIndex].Answer.ToString());
                if (answer != null)
                {
                    int index = pan_display.Controls.IndexOf(answer);
                    ((RadioButton)pan_display.Controls[index]).ForeColor = Color.Green;
                }
                RadioButton currentSelectedOption = pan_display.Controls.OfType<RadioButton>().FirstOrDefault(s => s.Checked);
                if (currentSelectedOption != null && currentSelectedOption.Text != answer.Text)
                {
                    int index = pan_display.Controls.IndexOf(currentSelectedOption);
                    ((RadioButton)pan_display.Controls[index]).ForeColor = Color.Red;
                }
            }
        }
    }

    enum NavOption
    {
        Begin,
        Next,
        Previous,
        End
    }
}