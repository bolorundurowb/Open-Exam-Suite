using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenExamSuite.Shared.Enums;
using ProtoBuf;
using TerraPDF.Core;
using TerraPDF.Helpers;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Writes exams to .oef (protobuf), PDF, JSON and XML. Write failures are surfaced as a typed
/// <see cref="ExamWriteResult"/> and routed through <see cref="ILogger"/>.
/// </summary>
public sealed class Writer
{
    private static readonly object FontRegistrationLock = new();
    private static bool _fontsRegistered;
    private const string PdfFontFamily = "IBM Plex Sans";
    private const double LeftMargin = 40;
    private const double RightMargin = 40;
    private const double TopMargin = 50;
    private const double BottomMargin = 50;
    private const double LineGap = 6;
    private const float BodyFontSize = 11f;
    private const float HeaderFontSize = 13f;
    private const float TitleFontSize = 18f;

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
            return new ExamWriteResult(false, ExamIoError.WriteFailed, ex.Message);
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
            return new ExamWriteResult(false, ExamIoError.WriteFailed, ex.Message);
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
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            return WritePdf(stream, exam.Properties.Title, exam.Properties.Code, content =>
            {
                content.DrawLabelAndValue("Exam Title: ", exam.Properties.Title);
                content.DrawBlankLine();
                content.DrawLabelAndValue("Exam Code: ", exam.Properties.Code);
                content.DrawLabelAndValue("Passmark: ", $"{exam.Properties.Passmark} / 1000");
                content.DrawLabelAndValue("Time Limit: ", $"{exam.Properties.TimeLimit} (min)");
                content.DrawLabelAndValue("Instructions: ", exam.Properties.Instructions);
                content.DrawBlankLine();

                foreach (var section in exam.Sections)
                {
                    content.DrawLabelAndValue("Section: ", section.Title);

                    foreach (var question in section.Questions)
                    {
                        content.DrawParagraph($"{question.No}. {question.Text}");

                        if (question.ImageData != null)
                            content.DrawImage(question.ImageData);

                        foreach (var option in question.Options)
                            content.DrawParagraph($"{option.Alphabet} - {option.Text}");

                        if (!exam.Properties.HideAnswers)
                        {
                            content.DrawParagraph($"Answer: {question.Answer}");

                            if (!string.IsNullOrWhiteSpace(question.Explanation))
                                content.DrawParagraph($"Explanation: {question.Explanation}");
                        }

                        content.DrawBlankLine();
                    }

                    content.DrawBlankLine();
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write PDF to '{FilePath}'.", filePath);
            return new ExamWriteResult(false, ExamIoError.WriteFailed, ex.Message);
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
            return new ExamWriteResult(false, ExamIoError.WriteFailed, ex.Message);
        }
    }

    public ExamWriteResult ToResultsPdf(ResultsReport report, Stream stream, ResultsReportLabels? labels = null)
    {
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(stream);
        labels ??= new ResultsReportLabels();

        try
        {
            return WritePdf(stream, $"{labels.Title}: {report.ExamTitle}", report.ExamCode, content =>
            {
                content.DrawTitle(labels.Title);
                content.DrawBlankLine();
                content.DrawLabelAndValue($"{labels.Exam}: ", report.ExamTitle);
                content.DrawLabelAndValue($"{labels.Code}: ", report.ExamCode);
                content.DrawLabelAndValue($"{labels.Candidate}: ", report.CandidateName);
                content.DrawLabelAndValue($"{labels.Date}: ", report.CompletedAt.ToLocalTime().ToString("g"));

                var used = $"{(int)report.TimeUsed.TotalMinutes:00}:{report.TimeUsed.Seconds:00}";
                if (report.TimeAllowed is { } allowed && allowed > TimeSpan.Zero)
                    used += $" / {(int)allowed.TotalMinutes:00}:{allowed.Seconds:00}";
                content.DrawLabelAndValue($"{labels.TimeUsed}: ", used);
                content.DrawBlankLine();

                content.DrawLabelAndValue($"{labels.Result}: ", report.Passed ? labels.Passed : labels.NotPassed);
                content.DrawLabelAndValue(
                    $"{labels.Score}: ",
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        "{0:0.#}% ({1} / 1000), {2} / {3}",
                        report.PercentScore,
                        report.ScaledScore,
                        report.CorrectAnswers,
                        report.TotalQuestions));
                content.DrawLabelAndValue($"{labels.PassMark}: ", $"{report.PassMarkScaled} / 1000");
                content.DrawBlankLine();

                content.DrawHeading(labels.Sections);
                foreach (var section in report.Sections)
                {
                    var percent = section.Total == 0 ? 0 : section.Correct * 100d / section.Total;
                    content.DrawParagraph(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "{0}: {1} / {2} ({3:0.#}%)",
                            section.SectionTitle,
                            section.Correct,
                            section.Total,
                            percent));
                }

                content.DrawBlankLine();
                content.DrawHeading(labels.Questions);
                foreach (var question in report.Questions)
                {
                    var outcome = question.Outcome switch
                    {
                        ResultsReportOutcome.Correct => labels.Correct,
                        ResultsReportOutcome.Wrong => labels.Wrong,
                        _ => labels.Unanswered
                    };
                    var text = question.Text.Length > 120 ? question.Text[..120] + "…" : question.Text;
                    content.DrawParagraph($"{question.Number}. [{outcome}] {text}");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write results PDF.");
            return new ExamWriteResult(false, ExamIoError.WriteFailed, ex.Message);
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
            return new ExamWriteResult(false, ExamIoError.WriteFailed, ex.Message);
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
            return new ExamWriteResult(false, ExamIoError.WriteFailed, ex.Message);
        }
    }

    private ExamWriteResult WritePdf(Stream stream, string? title, string? subject, Action<PdfContent> compose)
    {
        EnsurePdfFontsConfigured();
        Document.Create(container =>
        {
            container.MetadataTitle(title);
            container.MetadataCreator("Open Exam Suite");
            container.MetadataSubject(subject);
            container.Page(page =>
            {
                page.Size(PageSize.A4);
                page.Margin(TopMargin, RightMargin, BottomMargin, LeftMargin);
                page.DefaultTextStyle(style => style.FontFamily(PdfFontFamily).FontSize(BodyFontSize));
                page.Content().Column(column =>
                {
                    column.Spacing(LineGap);
                    compose(new PdfContent(column));
                });
            });
        }).PublishPdf(stream);

        return new ExamWriteResult(true);
    }

    private static void EnsurePdfFontsConfigured()
    {
        if (_fontsRegistered)
            return;

        lock (FontRegistrationLock)
        {
            if (_fontsRegistered)
                return;

            FontFamily.Register(PdfFontFamily, LoadFont("IBMPlexSans-Regular.ttf"));
            FontFamily.Register(PdfFontFamily, LoadFont("IBMPlexSans-Bold.ttf"), bold: true);
            FontFamily.Register(PdfFontFamily, LoadFont("IBMPlexSans-Italic.ttf"), italic: true);
            _fontsRegistered = true;
        }
    }

    private static byte[] LoadFont(string fileName)
    {
        var resourceName = $"OpenExamSuite.Shared.Resources.fonts.{fileName}";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Embedded font resource not found: {resourceName}");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private sealed class PdfContent
    {
        private readonly ColumnDescriptor _column;

        public PdfContent(ColumnDescriptor column) => _column = column;

        public void DrawLabelAndValue(string label, string? value)
        {
            DrawParagraph(label, HeaderFontSize, bold: true);
            DrawParagraph(value, BodyFontSize, bold: false);
        }

        public void DrawBlankLine() => _column.Item().Text(" ");

        public void DrawTitle(string? text) => DrawParagraph(text, TitleFontSize, bold: true);

        public void DrawHeading(string? text) => DrawParagraph(text, HeaderFontSize, bold: true);

        public void DrawParagraph(string? text) => DrawParagraph(text, BodyFontSize, bold: false);

        public void DrawImage(byte[] imageData)
        {
            var (width, _) = VectorCanvas.GetImageSizeInPoints(imageData);
            var contentWidth = PageSize.A4.Width - LeftMargin - RightMargin;
            var desiredWidth = width > 0 ? Math.Min(contentWidth, width) : contentWidth;
            _column.Item().Image(imageData, desiredWidth);
        }

        private void DrawParagraph(string? text, float size, bool bold)
        {
            var block = _column.Item().Text(text ?? string.Empty).FontFamily(PdfFontFamily).FontSize(size);
            if (bold)
                block.Bold();
        }
    }
}
