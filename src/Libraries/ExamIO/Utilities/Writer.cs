using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Xml.Serialization;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using OpenExamSuite.Logging;
using ProtoBuf;

namespace OpenExamSuite.Shared.Utilities;

public static class Writer
{
    private static readonly object FontResolverLock = new();
    private static bool _fontResolverConfigured;
    private const string PdfFontFamily = "Noto Sans";

    public static bool ToOef(Exam exam, string filePath, bool throwOnError = false)
    {
        if (exam == null)
            throw new ArgumentNullException(nameof(exam), "The exam to be written cannot be null.");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        try
        {
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            Serializer.Serialize(stream, exam);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);

            if (throwOnError)
                throw new Exception("Failed to save .oef file in protobuf format.", ex);

            return false;
        }
    }

    public static bool ToPdf(Exam exam, string filePath)
    {
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

                    if (question.Image != null)
                        layout.DrawBitmap(question.Image);

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
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return false;
        }

        return true;
    }

    public static bool ToJson(Exam exam, string filePath)
    {
        try
        {
            var examJsonString = JsonSerializer.Serialize(exam, ExamJsonSerialization.Options);
            File.WriteAllText(filePath, examJsonString);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return false;
        }
    }

    public static bool ToXml(Exam exam, string filePath)
    {
        try
        {
            var examXmlStringWriter = new StringWriter();
            var serializer = new XmlSerializer(exam.GetType());
            serializer.Serialize(examXmlStringWriter, exam);
            File.WriteAllText(filePath, examXmlStringWriter.ToString());
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return false;
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

            GlobalFontSettings.FontResolver ??= new EmbeddedNotoSansFontResolver();
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

        public void DrawBitmap(Bitmap bitmap)
        {
            using var imageStream = new MemoryStream();
            bitmap.Save(imageStream, ImageFormat.Png);
            imageStream.Position = 0;

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