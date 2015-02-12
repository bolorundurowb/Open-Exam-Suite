using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenExam_Suite
{
    public partial class Score_Sheet : Form
    {
        public Score_Sheet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The Constructor that displays result details
        /// </summary>
        /// <param name="candidateName">The name of the candidate</param>
        /// <param name="time">the total allocatable time</param>
        /// <param name="elapsedTime">the time used</param>
        /// <param name="examCode">the exam code, gotten from the exam file</param>
        /// <param name="score">the candidates score</param>
        /// <param name="requiredScore">the score required to pass</param>
        /// <param name="sectionQuestionNumbers">the number of questions per section</param>
        /// <param name="rightSectionQuestionNumbers">the number of correct questions per section</param>
        public Score_Sheet(string candidateName, int time, int elapsedTime, string examCode, int score, int requiredScore, Dictionary<string, int> sectionQuestionNumbers, Dictionary<string, int> rightSectionQuestionNumbers)
        {
            InitializeComponent();
            lbl_date.Text = DateTime.Now.Date.ToShortDateString();
            if (score >= requiredScore)
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
            lbl_candidate_name.Text = candidateName;
            lbl_elapsed_time.Text = elapsedTime.ToString();
            lbl_exam_number.Text = examCode;
            lbl_time.Text = time.ToString();
            dgv_show_breakdown.Rows.Clear();
            //
            for (int i = 0; i < sectionQuestionNumbers.Count; i++)
            {
                dgv_show_breakdown.Rows.Add(sectionQuestionNumbers.ElementAt(i).Key, sectionQuestionNumbers.ElementAt(i).Value, rightSectionQuestionNumbers.ElementAt(i).Value);
            }
            //
            chr_display_score.Series["Required Score"].Points.AddXY(1, requiredScore);
            chr_display_score.Series["Score"].Points.AddXY(0, score);
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_retake_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
