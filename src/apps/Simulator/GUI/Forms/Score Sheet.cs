using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Shared;
using Shared.Models;

namespace Simulator.GUI.Forms
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
            int normalizedScore = (settings.NumberOfCorrectAnswers * 1000 / settings.Questions.Count);
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

        private void Print(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int normalizedScore = (settings.NumberOfCorrectAnswers * 1000 / settings.Questions.Count);
            //
            Font headerFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            Font subFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            Font specialFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            //
            float ypos = e.MarginBounds.Top;
            e.Graphics.DrawString("EXAMINATION SCORE SHEET", headerFont, Brushes.Black, new PointF((e.MarginBounds.Width / 2) - 50, ypos));
            ypos += (2 * headerFont.GetHeight(e.Graphics));
            string name = lbl_candidate_name.Text.Length < 35 ? lbl_candidate_name.Text : lbl_candidate_name.Text.Substring(0, 35);
            e.Graphics.DrawString("CANDIDATE NAME: " + name, subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            e.Graphics.DrawString("TIME ALLOWED: " + lbl_time_allowed.Text + " min(s)", subFont, Brushes.DarkSlateBlue, new PointF((e.MarginBounds.Width / 2) + 175, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));
            e.Graphics.DrawString("DATE: " + DateTime.Now.ToShortDateString(), subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            e.Graphics.DrawString("TIME ELAPSED: " + lbl_elapsed_time.Text + " min(s)", subFont, Brushes.DarkSlateBlue, new PointF((e.MarginBounds.Width / 2) + 175, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));
            e.Graphics.DrawString("EXAM CODE: " + lbl_exam_number.Text, subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));
            //
            MemoryStream imgStream = new MemoryStream();
            chr_display_score.SaveImage(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            Bitmap bmp = new Bitmap(imgStream);
            e.Graphics.DrawImage(bmp, new PointF(e.MarginBounds.Left + 50, ypos));
            ypos += ((2 * subFont.GetHeight(e.Graphics)) + (bmp.Height));

            e.Graphics.DrawString("Required Score: " + exam.Properties.Passmark, subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            e.Graphics.DrawString("Your Score: " + normalizedScore, subFont, Brushes.DarkSlateBlue, new PointF((e.MarginBounds.Width / 2) + 175, ypos));
            ypos += (2 * subFont.GetHeight(e.Graphics));
            e.Graphics.DrawString("STATUS: ", subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, ypos));
            Brush brush = normalizedScore < exam.Properties.Passmark ? Brushes.Red : Brushes.Green;
            string status = normalizedScore < exam.Properties.Passmark ? "Failed" : "Passed";
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

        private void PrintResult(object sender, EventArgs e)
        {
            pnt_prv_dlg.ShowDialog();
        }
    }
}