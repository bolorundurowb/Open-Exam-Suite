using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Score_Sheet : Form
    {
        private int required;
        private int got;
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
            //
            required = requiredScore;
            got = score;
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_retake_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_print_score_Click(object sender, EventArgs e)
        {
            pnt_prv_dlg.ShowDialog();
        }

        private void pnt_doc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font headerFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            Font subFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            Font specialFont = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);

            float ypos = e.MarginBounds.Top;
            e.Graphics.DrawString("EXAMINATION SCORE SHEET", headerFont, Brushes.Black, new PointF((e.MarginBounds.Width / 2) - 50, ypos));
            ypos += (2 * headerFont.GetHeight(e.Graphics));
            string name = lbl_candidate_name.Text.Length<35?lbl_candidate_name.Text:lbl_candidate_name.Text.Substring(0,35);
            e.Graphics.DrawString("CANDIDATE NAME: " + name, subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            e.Graphics.DrawString("TIME ALLOWED: " + lbl_time.Text+ " min(s)", subFont, Brushes.DarkSlateBlue, new PointF((e.MarginBounds.Width / 2) + 175, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));
            e.Graphics.DrawString("DATE: " + DateTime.Now.ToShortDateString(), subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            e.Graphics.DrawString("TIME ELAPSED: " + lbl_elapsed_time.Text + " min(s)", subFont, Brushes.DarkSlateBlue, new PointF((e.MarginBounds.Width / 2) + 175, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));
            e.Graphics.DrawString("EXAM CODE: " + lbl_exam_number.Text, subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));

            System.IO.MemoryStream imgStream = new System.IO.MemoryStream();
            chr_display_score.SaveImage(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            Bitmap bmp = new Bitmap(imgStream);
            e.Graphics.DrawImage(bmp, new PointF(e.MarginBounds.Left + 50, ypos));
            ypos += ((2 * subFont.GetHeight(e.Graphics)) + (bmp.Height));

            e.Graphics.DrawString("Required Score: " + required, subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            e.Graphics.DrawString("Your Score: " + got, subFont, Brushes.DarkSlateBlue, new PointF((e.MarginBounds.Width / 2) + 175, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));
            e.Graphics.DrawString("STATUS: ", subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            Brush brush = got < required ? Brushes.Red : Brushes.Green;
            string status = got <required?"failed":"passed";
            e.Graphics.DrawString(status, subFont, brush, new PointF(e.MarginBounds.Left + 70, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));

            e.Graphics.DrawLine(new Pen(Brushes.DarkSlateBlue), new PointF(150, ypos), new PointF(700, ypos));
            e.Graphics.DrawString("SECTION", specialFont, Brushes.DarkSlateBlue, new PointF(180F, ypos));
            e.Graphics.DrawString("NUMBER", specialFont, Brushes.DarkSlateBlue, new PointF(490F, ypos));
            e.Graphics.DrawString("ACCURACY", specialFont, Brushes.DarkSlateBlue, new PointF(600F, ypos));
            ypos += specialFont.GetHeight(e.Graphics);
            e.Graphics.DrawLine(new Pen(Brushes.DarkSlateBlue), new PointF(150, ypos), new PointF(700, ypos));

            
            foreach (DataGridViewRow row in dgv_show_breakdown.Rows)
            {
                e.Graphics.DrawString(row.Cells[0].Value.ToString(), subFont, Brushes.DarkSlateBlue, new PointF(180, ypos));
                e.Graphics.DrawString(row.Cells[1].Value.ToString(), subFont, Brushes.DarkSlateBlue, new PointF(490, ypos));
                e.Graphics.DrawString(row.Cells[2].Value.ToString(), subFont, Brushes.DarkSlateBlue, new PointF(600, ypos));
                ypos += (subFont.GetHeight(e.Graphics));
            }
            e.Graphics.DrawLine(new Pen(Brushes.DarkSlateBlue), new PointF(150, ypos), new PointF(700, ypos));
        }
    }
}
