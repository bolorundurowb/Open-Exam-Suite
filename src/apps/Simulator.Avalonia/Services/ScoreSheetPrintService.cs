using System;
using System.IO;
using System.Threading.Tasks;
using OpenExamSuite.Logging;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Shared.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Faithful port of <c>ScoreSheetUi.Print</c> using PdfSharp. The layout
/// mirrors the WinForms GDI+ drawing (heading → candidate name, time allowed,
/// date, time elapsed → exam code → required vs. your score → status →
/// per-section breakdown table).
/// </summary>
public sealed class ScoreSheetPrintService : IScoreSheetPrintService
{
    private readonly IOpenUrlService _opener;

    public ScoreSheetPrintService(IOpenUrlService opener)
    {
        _opener = opener;
    }

    public async Task PrintAsync(Settings settings, Exam exam)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(),
                $"OpenExamSuite_ScoreSheet_{DateTime.Now:yyyyMMdd_HHmmssfff}.pdf");

            using (var doc = new PdfDocument())
            {
                doc.Info.Title = $"Score Sheet - {exam.Properties.Title}";
                doc.Info.Creator = "Open Exam Suite";

                var page = doc.AddPage();
                using var gfx = XGraphics.FromPdfPage(page);

                var headerFont  = new XFont("Arial", 14, XFontStyleEx.Bold);
                var subFont     = new XFont("Arial", 10);
                var specialFont = new XFont("Arial", 10, XFontStyleEx.Bold);

                var margin = 50.0;
                var width = page.Width.Point - 2 * margin;
                double y = margin;

                gfx.DrawString("EXAMINATION SCORE SHEET", headerFont, XBrushes.Black,
                    new XRect(margin, y, width, 30), XStringFormats.TopCenter);
                y += 40;

                var normalizedScore = settings.Questions.Count > 0
                    ? settings.NumberOfCorrectAnswers * 1000 / settings.Questions.Count
                    : 0;

                gfx.DrawString($"CANDIDATE NAME: {Trim(settings.CandidateName, 35)}", subFont, XBrushes.DarkSlateBlue,
                    new XPoint(margin, y));
                gfx.DrawString($"TIME ALLOWED: {settings.TimeLimit} min(s)", subFont, XBrushes.DarkSlateBlue,
                    new XPoint(margin + width / 2 + 40, y));
                y += 18;

                gfx.DrawString($"DATE: {DateTime.Now.ToShortDateString()}", subFont, XBrushes.DarkSlateBlue,
                    new XPoint(margin, y));
                gfx.DrawString($"TIME ELAPSED: {settings.ElapsedTime.TotalMinutes:F} min(s)", subFont, XBrushes.DarkSlateBlue,
                    new XPoint(margin + width / 2 + 40, y));
                y += 18;

                gfx.DrawString($"EXAM CODE: {exam.Properties.Code}", subFont, XBrushes.DarkSlateBlue,
                    new XPoint(margin, y));
                y += 28;

                gfx.DrawString($"Required Score: {exam.Properties.Passmark}", subFont, XBrushes.DarkSlateBlue,
                    new XPoint(margin, y));
                gfx.DrawString($"Your Score: {normalizedScore}", subFont, XBrushes.DarkSlateBlue,
                    new XPoint(margin + width / 2 + 40, y));
                y += 18;

                var passed = normalizedScore >= exam.Properties.Passmark;
                gfx.DrawString("STATUS:", subFont, XBrushes.DarkSlateBlue, new XPoint(margin, y));
                gfx.DrawString(passed ? "Passed" : "Failed", subFont,
                    passed ? XBrushes.Green : XBrushes.Red, new XPoint(margin + 70, y));
                y += 28;

                // Section breakdown table.
                var pen = new XPen(XColors.DarkSlateBlue);
                gfx.DrawLine(pen, margin, y, margin + width, y);
                gfx.DrawString("SECTION",  specialFont, XBrushes.DarkSlateBlue, new XPoint(margin + 30, y + 12));
                gfx.DrawString("NUMBER",   specialFont, XBrushes.DarkSlateBlue, new XPoint(margin + width / 2,       y + 12));
                gfx.DrawString("ACCURACY", specialFont, XBrushes.DarkSlateBlue, new XPoint(margin + width - 80,      y + 12));
                y += 18;
                gfx.DrawLine(pen, margin, y, margin + width, y);
                y += 4;

                foreach (var s in settings.ResultSpread)
                {
                    gfx.DrawString(s.SectionTitle, subFont, XBrushes.DarkSlateBlue, new XPoint(margin + 30, y + 12));
                    gfx.DrawString(s.Total.ToString(),  subFont, XBrushes.DarkSlateBlue, new XPoint(margin + width / 2, y + 12));
                    gfx.DrawString(s.Correct.ToString(), subFont, XBrushes.DarkSlateBlue, new XPoint(margin + width - 80, y + 12));
                    y += 16;
                }
                gfx.DrawLine(pen, margin, y, margin + width, y);

                doc.Save(tempPath);
            }

            await _opener.OpenFileAsync(tempPath);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    private static string Trim(string s, int max)
        => s.Length <= max ? s : s.Substring(0, max);
}
