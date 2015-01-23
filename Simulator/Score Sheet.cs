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

        public Score_Sheet(string candidateName, int time, int elapsedTime, string examNumber, int score, int requiredScore, List<string[]> sections)
        {
            InitializeComponent();
            lbl_date.Text = DateTime.Now.Date.ToShortDateString();
            if (score >= requiredScore)
            {
                lbl_status.Text = "Passed";
                lbl_status.Font = new Font("Microsoft Sans Serif",8.25F);
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
            lbl_exam_number.Text = examNumber;
            lbl_time.Text = time.ToString();
            dgv_show_breakdown.Rows.Clear();
            foreach(string[] section in sections)
            {
                dgv_show_breakdown.Rows.Add(section[0], section[1], section[2]);
            }
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
