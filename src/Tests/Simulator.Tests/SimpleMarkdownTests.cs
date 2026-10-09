using System.Linq;
using OmniAssert;
using OpenExamSuite.Simulator.Services;
using Xunit;

namespace OpenExamSuite.Simulator.Tests;

public class SimpleMarkdownTests
{
    [Fact]
    public void Parse_ReadsHeadingsBulletsAndParagraphs()
    {
        var blocks = SimpleMarkdown.Parse("# Changelog\r\n\r\n## [4.0.5]\r\n### Simulator\r\n- Fixed a crash.\r\n- Added a toast.\r\n\r\nFirst line\r\nsecond line\r\n");

        blocks.Select(b => b.Kind).SequenceEqual(
        [
            MarkdownBlockKind.Heading1, MarkdownBlockKind.Heading2, MarkdownBlockKind.Heading3,
            MarkdownBlockKind.Bullet, MarkdownBlockKind.Bullet, MarkdownBlockKind.Paragraph
        ]).Must().BeTrue();
        blocks[3].Text.Must().Be("Fixed a crash.");
        blocks[5].Text.Must().Be("First line second line");
    }

    [Fact]
    public void ParseInline_SplitsBoldAndCode()
    {
        var runs = SimpleMarkdown.ParseInline("Use **Help > Changelog** or `Ctrl+O`.");

        runs.Count.Must().Be(5);
        runs[1].Text.Must().Be("Help > Changelog");
        runs[1].Bold.Must().BeTrue();
        runs[3].Text.Must().Be("Ctrl+O");
        runs[3].Code.Must().BeTrue();
        runs[4].Bold.Must().BeFalse();
    }
}
