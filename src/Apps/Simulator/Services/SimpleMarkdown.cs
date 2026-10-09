namespace OpenExamSuite.Simulator.Services;

public enum MarkdownBlockKind
{
    Heading1,
    Heading2,
    Heading3,
    Bullet,
    Paragraph
}

public sealed record MarkdownBlock(MarkdownBlockKind Kind, string Text);

public sealed record MarkdownRun(string Text, bool Bold = false, bool Code = false);

/// <summary>
/// A deliberately small Markdown reader for the bundled changelog: headings, bullets, paragraphs,
/// <c>**bold**</c> and <c>`code`</c>. Nothing else in the app needs more than that.
/// </summary>
public static class SimpleMarkdown
{
    public static IReadOnlyList<MarkdownBlock> Parse(string markdown)
    {
        var blocks = new List<MarkdownBlock>();
        var paragraph = new List<string>();

        void FlushParagraph()
        {
            if (paragraph.Count == 0)
                return;

            blocks.Add(new MarkdownBlock(MarkdownBlockKind.Paragraph, string.Join(' ', paragraph)));
            paragraph.Clear();
        }

        foreach (var rawLine in markdown.Replace("\r\n", "\n").Split('\n'))
        {
            var line = rawLine.TrimEnd();
            var trimmed = line.TrimStart();

            if (trimmed.Length == 0)
            {
                FlushParagraph();
            }
            else if (trimmed.StartsWith("### ", StringComparison.Ordinal))
            {
                FlushParagraph();
                blocks.Add(new MarkdownBlock(MarkdownBlockKind.Heading3, trimmed[4..].Trim()));
            }
            else if (trimmed.StartsWith("## ", StringComparison.Ordinal))
            {
                FlushParagraph();
                blocks.Add(new MarkdownBlock(MarkdownBlockKind.Heading2, trimmed[3..].Trim()));
            }
            else if (trimmed.StartsWith("# ", StringComparison.Ordinal))
            {
                FlushParagraph();
                blocks.Add(new MarkdownBlock(MarkdownBlockKind.Heading1, trimmed[2..].Trim()));
            }
            else if (trimmed.StartsWith("- ", StringComparison.Ordinal) || trimmed.StartsWith("* ", StringComparison.Ordinal))
            {
                FlushParagraph();
                blocks.Add(new MarkdownBlock(MarkdownBlockKind.Bullet, trimmed[2..].Trim()));
            }
            else
            {
                paragraph.Add(trimmed);
            }
        }

        FlushParagraph();
        return blocks;
    }

    public static IReadOnlyList<MarkdownRun> ParseInline(string text)
    {
        var runs = new List<MarkdownRun>();
        var buffer = new System.Text.StringBuilder();
        var bold = false;
        var code = false;

        void Flush()
        {
            if (buffer.Length == 0)
                return;

            runs.Add(new MarkdownRun(buffer.ToString(), bold, code));
            buffer.Clear();
        }

        for (var i = 0; i < text.Length; i++)
        {
            if (!code && text[i] == '*' && i + 1 < text.Length && text[i + 1] == '*')
            {
                Flush();
                bold = !bold;
                i++;
            }
            else if (text[i] == '`')
            {
                Flush();
                code = !code;
            }
            else
            {
                buffer.Append(text[i]);
            }
        }

        Flush();
        return runs;
    }
}
