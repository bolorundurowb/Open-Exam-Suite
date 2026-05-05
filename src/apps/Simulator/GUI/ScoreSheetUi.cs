using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Simulator.GUI;

public partial class ScoreSheetUi : Form
{
    #region Global Variables

    private readonly Settings _settings;
    private readonly Exam _exam;

    #endregion

    public ScoreSheetUi(Settings settings, Exam exam)
    {
        InitializeComponent();

        _settings = settings;
        _exam = exam;
        lbl_candidate_name.Text = settings.CandidateName;
        lbl_date.Text = DateTime.Now.ToShortDateString();
        lbl_elapsed_time.Text = settings.ElapsedTime.TotalMinutes.ToString("F");
        lbl_exam_number.Text = exam.Properties.Code;
        lbl_time_allowed.Text = settings.TimeLimit.ToString();
    } 

    private void LoadDataToUi(object sender, EventArgs e)
    {
        var normalizedScore = (_settings.NumberOfCorrectAnswers * 1000 / _settings.Questions.Count);
        if (normalizedScore >= _exam.Properties.Passmark)
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

        chr_display_score.Series["Pass Mark"].Points.AddXY(1, _exam.Properties.Passmark);
        chr_display_score.Series["Your Score"].Points.AddXY(0, normalizedScore);

        foreach (var spread in _settings.ResultSpread)
        {
            dgv_show_breakdown.Rows.Add(spread.SectionTitle, spread.Total, spread.Correct);
        }
    }

    private void Exit(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void Retake(object sender, EventArgs e)
    {
        Close();
    }

    private void Print(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        var normalizedScore = (_settings.NumberOfCorrectAnswers * 1000 / _settings.Questions.Count);

        var headerFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        var subFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        var specialFont = new Font("Segoe UI", 10F, FontStyle.Bold);

        float yPos = e.MarginBounds.Top;

        if (e.Graphics == null)
        {
            throw new Exception("Unable to print, graphics context is null");
        }

        e.Graphics.DrawString("EXAMINATION SCORE SHEET", headerFont, Brushes.Black,
            new PointF((e.MarginBounds.Width / 2.0f) - 50, yPos));
        yPos += (2 * headerFont.GetHeight(e.Graphics));
        var name = lbl_candidate_name.Text.Length < 35
            ? lbl_candidate_name.Text
            : lbl_candidate_name.Text.Substring(0, 35);
        e.Graphics.DrawString("CANDIDATE NAME: " + name, subFont, Brushes.DarkSlateBlue,
            new PointF(e.MarginBounds.Left, yPos));
        e.Graphics.DrawString("TIME ALLOWED: " + lbl_time_allowed.Text + " min(s)", subFont, Brushes.DarkSlateBlue,
            new PointF((e.MarginBounds.Width / 2.0f) + 175, yPos));
        yPos += (2 * subFont.GetHeight(e.Graphics));
        e.Graphics.DrawString("DATE: " + DateTime.Now.ToShortDateString(), subFont, Brushes.DarkSlateBlue,
            new PointF(e.MarginBounds.Left, yPos));
        e.Graphics.DrawString("TIME ELAPSED: " + lbl_elapsed_time.Text + " min(s)", subFont, Brushes.DarkSlateBlue,
            new PointF((e.MarginBounds.Width / 2.0f) + 175, yPos));
        yPos += (2 * subFont.GetHeight(e.Graphics));
        e.Graphics.DrawString("EXAM CODE: " + lbl_exam_number.Text, subFont, Brushes.DarkSlateBlue,
            new PointF(e.MarginBounds.Left, yPos));
        yPos += (2 * subFont.GetHeight(e.Graphics));

        var imgStream = new MemoryStream();
        chr_display_score.SaveImage(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        var bmp = new Bitmap(imgStream);
        e.Graphics.DrawImage(bmp, new PointF(e.MarginBounds.Left + 50, yPos));
        yPos += ((2 * subFont.GetHeight(e.Graphics)) + (bmp.Height));

        e.Graphics.DrawString("Required Score: " + _exam.Properties.Passmark, subFont, Brushes.DarkSlateBlue,
            new PointF(e.MarginBounds.Left, yPos));
        e.Graphics.DrawString("Your Score: " + normalizedScore, subFont, Brushes.DarkSlateBlue,
            new PointF((e.MarginBounds.Width / 2.0f) + 175, yPos));
        yPos += (2 * subFont.GetHeight(e.Graphics));
        e.Graphics.DrawString("STATUS: ", subFont, Brushes.DarkSlateBlue, new PointF(e.MarginBounds.Left, yPos));
        var brush = normalizedScore < _exam.Properties.Passmark ? Brushes.Red : Brushes.Green;
        var status = normalizedScore < _exam.Properties.Passmark ? "Failed" : "Passed";
        e.Graphics.DrawString(status, subFont, brush, new PointF(e.MarginBounds.Left + 70, yPos));
        yPos += (2 * subFont.GetHeight(e.Graphics));

        e.Graphics.DrawLine(new Pen(Brushes.DarkSlateBlue), new PointF(150, yPos), new PointF(700, yPos));
        e.Graphics.DrawString("SECTION", specialFont, Brushes.DarkSlateBlue, new PointF(180F, yPos));
        e.Graphics.DrawString("NUMBER", specialFont, Brushes.DarkSlateBlue, new PointF(490F, yPos));
        e.Graphics.DrawString("ACCURACY", specialFont, Brushes.DarkSlateBlue, new PointF(600F, yPos));
        yPos += specialFont.GetHeight(e.Graphics);
        e.Graphics.DrawLine(new Pen(Brushes.DarkSlateBlue), new PointF(150, yPos), new PointF(700, yPos));


        foreach (DataGridViewRow row in dgv_show_breakdown.Rows)
        {
            e.Graphics.DrawString(row.Cells[0].Value?.ToString(), subFont, Brushes.DarkSlateBlue,
                new PointF(180, yPos));
            e.Graphics.DrawString(row.Cells[1].Value?.ToString(), subFont, Brushes.DarkSlateBlue,
                new PointF(490, yPos));
            e.Graphics.DrawString(row.Cells[2].Value?.ToString(), subFont, Brushes.DarkSlateBlue,
                new PointF(600, yPos));
            yPos += (subFont.GetHeight(e.Graphics));
        }

        e.Graphics.DrawLine(new Pen(Brushes.DarkSlateBlue), new PointF(150, yPos), new PointF(700, yPos));
    }

    private void PrintResult(object sender, EventArgs e)
    {
        pnt_prv_dlg.ShowDialog();
    }
}