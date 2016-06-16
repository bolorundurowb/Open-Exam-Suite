using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Exam_UI : Form
    {
        private Exam exam;
        private Settings settings;
        private int timeLeft;

        public Exam_UI(Exam _exam, Settings _settings)
        {
            InitializeComponent();
            exam = _exam;
            settings = _settings;
            timeLeft = exam.Properties.TimeLimit * 60;
        }

        private void timer_Tick(object sender, EventArgs e)
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
            if (option == NavOption.Begin)
            {

            }
            else if (option == NavOption.Next)
            {

            }
            else if (option == NavOption.Previous)
            {

            }
            else if (option == NavOption.End)
            {

            }
        }

        private void AddOptions(List<Option> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                RadioButton rdb = new RadioButton();
                rdb.AutoSize = true;
                rdb.Text = options[i].Alphabet + ". - " + options[i].Text;
                rdb.Name = "rdb" + options[i].Alphabet;
                rdb.Location = new System.Drawing.Point(51, 464 + (i * 22));
                pan_display.Controls.Add(rdb);
            }
        }

        private void RemoveOptions()
        {
            var controls = pan_display.Controls.OfType<RadioButton>();
            for(int j = pan_display.Controls.OfType<RadioButton>().Count() - 1; j >= 0; --j)
            {
                var control = controls.ElementAt(j);
                pan_display.Controls.Remove(control);
                control.Dispose();
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