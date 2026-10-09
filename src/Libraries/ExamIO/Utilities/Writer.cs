using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenExamSuite.Shared.Enums;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using ProtoBuf;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Writes exams to .oef (protobuf), PDF, JSON and XML. Write failures are surfaced as a typed
/// <see cref="ExamWriteResult"/> and routed through <see cref="ILogger"/>.
/// </summary>
public sealed class Writer
{
    private static readonly object FontResolverLock = new();
    private static bool _fontResolverConfigured;
    private const string PdfFontFamily = "IBM Plex Sans";

    private readonly ILogger<Writer> _logger;

    public Writer(ILogger<Writer>? logger = null)
    {
        _logger = logger ?? NullLogger<Writer>.Instance;
    }

    public ExamWriteResult ToOef(Exam exam, string filePath)
    {
        if (exam == null)
            throw new ArgumentNullException(nameof(exam), "The exam to be written cannot be null.");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        try
        {
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            Serializer.Serialize(stream, exam);
            return new ExamWriteResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save .oef file to '{FilePath}'.", filePath);
            return new ExamWriteResult(false, ExamIoError.WriteFailed);
        }
    }

    /// <summary>
    /// Serializes an exam to protobuf into <paramref name="stream"/>. The stream is never disposed
    /// or repositioned by the caller contract, and no upgrade side effects occur.
    /// </summary>
    public ExamWriteResult ToOef(Exam exam, Stream stream)
    {
        if (exam == null)
            throw new ArgumentNullException(nameof(exam), "The exam to be written cannot be null.");

        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            Serializer.Serialize(stream, exam);
            return new ExamWriteResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to serialize .oef exam to stream.");
            return new ExamWriteResult(false, ExamIoError.WriteFailed);
        }
    }

    public ExamWriteResult ToPdf(Exam exam, string filePath)
    {
        if (exam == null)
            throw new ArgumentNullException(nameof(exam), "The exam to be written cannot be null.");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        try
        {
            EnsurePdfFontsConfigured();
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var document = new PdfDocument();
            document.Info.CreationDate = DateTime.Now;
            document.Info.Creator = "Open Exam Suite";
            document.Info.Subject = exam.Properties.Code;
            document.Info.Title = exam.Properties.Title;

            var bodyFont = new XFont(PdfFontFamily, 11f, XFontStyleEx.Regular);
            var headerFont = new XFont(PdfFontFamily, 13f, XFontStyleEx.Bold);
            var layout = PdfLayout.Create(document);

            layout.DrawLabelAndValue("Exam Title: ", exam.Properties.Title, headerFont, bodyFont);
            layout.DrawBlankLine(bodyFont);
            layout.DrawLabelAndValue("Exam Code: ", exam.Properties.Code, headerFont, bodyFont);
            layout.DrawLabelAndValue("Passmark: ", $"{exam.Properties.Passmark} / 1000", headerFont, bodyFont);
            layout.DrawLabelAndValue("Time Limit: ", $"{exam.Properties.TimeLimit} (min)", headerFont, bodyFont);
            layout.DrawLabelAndValue("Instructions: ", exam.Properties.Instructions, headerFont, bodyFont);
            layout.DrawBlankLine(bodyFont);

            foreach (var section in exam.Sections)
            {
                layout.DrawLabelAndValue("Section: ", section.Title, headerFont, bodyFont);

                foreach (var question in section.Questions)
                {
                    layout.DrawParagraph($"{question.No}. {question.Text}", bodyFont);

                    if (question.ImageData != null)
                        layout.DrawImage(question.ImageData);

                    foreach (var option in question.Options)
                        layout.DrawParagraph($"{option.Alphabet} - {option.Text}", bodyFont);

                    if (!exam.Properties.HideAnswers)
                    {
                        layout.DrawParagraph($"Answer: {question.Answer}", bodyFont);

                        if (!string.IsNullOrWhiteSpace(question.Explanation))
                            layout.DrawParagraph($"Explanation: {question.Explanation}", bodyFont);
                    }

                    layout.DrawBlankLine(bodyFont);
                }

                layout.DrawBlankLine(bodyFont);
            }

            document.Save(stream, false);
            return new ExamWriteResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write PDF to '{FilePath}'.", filePath);
            return new ExamWriteResult(false, ExamIoError.WriteFailed);
        }
    }

    public ExamWriteResult ToResultsPdf(ResultsReport report, string filePath, ResultsReportLabels? labels = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        try
        {
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            return ToResultsPdf(report, stream, labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write results PDF to '{FilePath}'.", filePath);
            return new ExamWriteResult(false, ExamIoError.WriteFailed);
        }
    }

    public ExamWriteResult ToResultsPdf(ResultsReport report, Stream stream, ResultsReportLabels? labels = null)
    {
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(stream);
        labels ??= new ResultsReportLabels();

        try
        {
            EnsurePdfFontsConfigured();
            using var document = new PdfDocument();
            document.Info.CreationDate = DateTime.Now;
            document.Info.Creator = "Open Exam Suite";
            document.Info.Subject = report.ExamCode;
            document.Info.Title = $"{labels.Title}: {report.ExamTitle}";

            var bodyFont = new XFont(PdfFontFamily, 11f, XFontStyleEx.Regular);
            var headerFont = new XFont(PdfFontFamily, 13f, XFontStyleEx.Bold);
            var titleFont = new XFont(PdfFontFamily, 18f, XFontStyleEx.Bold);
            var layout = PdfLayout.Create(document);

            layout.DrawParagraph(labels.Title, titleFont);
            layout.DrawBlankLine(bodyFont);
            layout.DrawLabelAndValue($"{labels.Exam}: ", report.ExamTitle, headerFont, bodyFont);
            layout.DrawLabelAndValue($"{labels.Code}: ", report.ExamCode, headerFont, bodyFont);
            layout.DrawLabelAndValue($"{labels.Candidate}: ", report.CandidateName, headerFont, bodyFont);
            layout.DrawLabelAndValue($"{labels.Date}: ", report.CompletedAt.ToLocalTime().ToString("g"), headerFont, bodyFont);

            var used = $"{(int)report.TimeUsed.TotalMinutes:00}:{report.TimeUsed.Seconds:00}";
            if (report.TimeAllowed is { } allowed && allowed > TimeSpan.Zero)
                used += $" / {(int)allowed.TotalMinutes:00}:{allowed.Seconds:00}";
            layout.DrawLabelAndValue($"{labels.TimeUsed}: ", used, headerFont, bodyFont);
            layout.DrawBlankLine(bodyFont);

            layout.DrawLabelAndValue(
                $"{labels.Result}: ",
                report.Passed ? labels.Passed : labels.NotPassed,
                headerFont,
                bodyFont);
            layout.DrawLabelAndValue(
                $"{labels.Score}: ",
                string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    "{0:0.#}% ({1} / 1000), {2} / {3}",
                    report.PercentScore,
                    report.ScaledScore,
                    report.CorrectAnswers,
                    report.TotalQuestions),
                headerFont,
                bodyFont);
            layout.DrawLabelAndValue($"{labels.PassMark}: ", $"{report.PassMarkScaled} / 1000", headerFont, bodyFont);
            layout.DrawBlankLine(bodyFont);

            layout.DrawParagraph(labels.Sections, headerFont);
            foreach (var section in report.Sections)
            {
                var percent = section.Total == 0 ? 0 : section.Correct * 100d / section.Total;
                layout.DrawParagraph(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        "{0}: {1} / {2} ({3:0.#}%)",
                        section.SectionTitle,
                        section.Correct,
                        section.Total,
                        percent),
                    bodyFont);
            }

            layout.DrawBlankLine(bodyFont);
            layout.DrawParagraph(labels.Questions, headerFont);
            foreach (var question in report.Questions)
            {
                var outcome = question.Outcome switch
                {
                    ResultsReportOutcome.Correct => labels.Correct,
                    ResultsReportOutcome.Wrong => labels.Wrong,
                    _ => labels.Unanswered
                };
                var text = question.Text.Length > 120 ? question.Text[..120] + "…" : question.Text;
                layout.DrawParagraph($"{question.Number}. [{outcome}] {text}", bodyFont);
            }

            document.Save(stream, false);
            return new ExamWriteResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write results PDF.");
            return new ExamWriteResult(false, ExamIoError.WriteFailed);
        }
    }

    public ExamWriteResult ToJson(Exam exam, string filePath)
    {
        if (exam == null)
            throw new ArgumentNullException(nameof(exam), "The exam to be written cannot be null.");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        try
        {
            var examJsonString = JsonSerializer.Serialize(exam, ExamJsonSerialization.Options);
            File.WriteAllText(filePath, examJsonString);
            return new ExamWriteResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write JSON exam to '{FilePath}'.", filePath);
            return new ExamWriteResult(false, ExamIoError.WriteFailed);
        }
    }

    public ExamWriteResult ToXml(Exam exam, string filePath)
    {
        if (exam == null)
            throw new ArgumentNullException(nameof(exam), "The exam to be written cannot be null.");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        try
        {
            var examXmlStringWriter = new StringWriter();
            var serializer = new XmlSerializer(exam.GetType());
            serializer.Serialize(examXmlStringWriter, exam);
            File.WriteAllText(filePath, examXmlStringWriter.ToString());
            return new ExamWriteResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write XML exam to '{FilePath}'.", filePath);
            return new ExamWriteResult(false, ExamIoError.WriteFailed);
        }
    }

    private static void EnsurePdfFontsConfigured()
    {
        if (_fontResolverConfigured)
            return;

        lock (FontResolverLock)
        {
            if (_fontResolverConfigured)
                return;

            GlobalFontSettings.FontResolver ??= new EmbeddedPdfFontResolver();
            _fontResolverConfigured = true;
        }
    }

    private sealed class PdfLayout
    {
        private const double LeftMargin = 40;
        private const double RightMargin = 40;
        private const double TopMargin = 50;
        private const double BottomMargin = 50;
        private const double LineGap = 6;

        private readonly PdfDocument _document;
        private PdfPage _page;
        private XGraphics _graphics;
        private double _contentWidth;
        private double _contentBottom;
        private double _cursorY;

        private PdfLayout(PdfDocument document)
        {
            _document = document;
            (_page, _graphics) = AddPage();
            UpdateLayoutMetrics();
            _cursorY = TopMargin;
        }

        public static PdfLayout Create(PdfDocument document) => new(document);

        public void DrawLabelAndValue(string label, string? value, XFont labelFont, XFont valueFont)
        {
            DrawParagraph(label, labelFont);
            DrawParagraph(value ?? string.Empty, valueFont);
        }

        public void DrawBlankLine(XFont font) => _cursorY += MeasureLineHeight(font);

        public void DrawParagraph(string? text, XFont font)
        {
            var normalized = text ?? string.Empty;
            var height = MeasureTextHeight(normalized, font);
            EnsureSpace(height);

            var drawRect = new XRect(LeftMargin, _cursorY, _contentWidth, height);
            var formatter = new XTextFormatter(_graphics);
            formatter.DrawString(normalized, font, XBrushes.Black, drawRect, XStringFormats.TopLeft);
            _cursorY += height + LineGap;
        }

        public void DrawImage(byte[] imageData)
        {
            using var imageStream = new MemoryStream(imageData);
            using var image = XImage.FromStream(imageStream);
            var desiredWidth = Math.Min(_contentWidth, image.PointWidth);
            var scale = image.PointWidth == 0 ? 1 : desiredWidth / image.PointWidth;
            var desiredHeight = image.PointHeight * scale;

            EnsureSpace(desiredHeight);
            _graphics.DrawImage(image, LeftMargin, _cursorY, desiredWidth, desiredHeight);
            _cursorY += desiredHeight + LineGap;
        }

        private double MeasureTextHeight(string text, XFont font)
        {
            if (string.IsNullOrEmpty(text))
                return MeasureLineHeight(font);

            var roughWidth = _graphics.MeasureString(text, font).Width;
            var wrappedLines = Math.Max(1d, Math.Ceiling(roughWidth / _contentWidth));
            return wrappedLines * MeasureLineHeight(font);
        }

        private double MeasureLineHeight(XFont font) => _graphics.MeasureString("Ag", font).Height;

        private void EnsureSpace(double neededHeight)
        {
            if (_cursorY + neededHeight <= _contentBottom)
                return;

            _graphics.Dispose();
            (_page, _graphics) = AddPage();
            UpdateLayoutMetrics();
            _cursorY = TopMargin;
        }

        private (PdfPage Page, XGraphics Graphics) AddPage()
        {
            var page = _document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            return (page, XGraphics.FromPdfPage(page));
        }

        private void UpdateLayoutMetrics()
        {
            _contentWidth = _page.Width.Point - LeftMargin - RightMargin;
            _contentBottom = _page.Height.Point - BottomMargin;
        }
    }
}
