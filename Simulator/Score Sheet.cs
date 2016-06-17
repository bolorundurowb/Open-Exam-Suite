using Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Score_Sheet : Form
    {
        #region Global Variables
        private Settings settings;
        private Exam exam;
        #endregion

        public Score_Sheet(Settings _settings, Exam _exam)
        {
            InitializeComponent();
            settings = _settings;
            exam = _exam;
            lbl_candidate_name.Text = _settings.CandidateName;
            lbl_date.Text = DateTime.Now.ToShortDateString();
            lbl_elapsed_time.Text = _settings.ElapsedTime.TotalMinutes.ToString("F");
            lbl_exam_number.Text = _exam.Properties.Code;
            lbl_time_allowed.Text = _settings.TimeLimit.ToString();
        }

        private void LoadDataToUI(object sender, EventArgs e)
        {
            int normalizedScore = (settings.NumberOfCorrectAnswers / settings.Questions.Count) * 1000;
            if (normalizedScore >= exam.Properties.Passmark)
            {
                lbl_status.Text = "Passed";
                lbl_status.Font = new Font("Microsoft Sans Serif", 8.25F);
                lbl_status.ForeColor = Color.Green;
            }
            else
            {
                lbl_status.Text = "Failed";
                lbl_status.Font = new Font("Microsoft Sans Serif", 8.25F);
                lbl_status.ForeColor = Color.Red;
            }
            //
            chr_display_score.Series["Pass Mark"].Points.AddXY(1, exam.Properties.Passmark);
            chr_display_score.Series["Your Score"].Points.AddXY(0, normalizedScore);
            //
            foreach(var spread in settings.ResultSpread)
            {
                dgv_show_breakdown.Rows.Add(spread.Item1, spread.Item2, spread.Item3);
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Retake(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Print(object sender, EventArgs e)
        {

        }
    }
}