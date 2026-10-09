using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using OpenExamSuite.Simulator.Services;

namespace OpenExamSuite.Simulator.Views.Controls;

public static class MarkdownRenderer
{
    public static Control Render(string markdown)
    {
        var panel = new StackPanel { Spacing = 8 };
        foreach (var block in SimpleMarkdown.Parse(markdown))
            panel.Children.Add(RenderBlock(block));

        return panel;
    }

    private static Control RenderBlock(MarkdownBlock block)
    {
        var text = new SelectableTextBlock { TextWrapping = TextWrapping.Wrap };
        FillInlines(text, block.Text);

        switch (block.Kind)
        {
            case MarkdownBlockKind.Heading1:
                text.FontSize = 26;
                text.FontWeight = FontWeight.Bold;
                text.Margin = new Thickness(0, 8, 0, 4);
                return text;
            case MarkdownBlockKind.Heading2:
                text.FontSize = 20;
                text.FontWeight = FontWeight.SemiBold;
                text.Margin = new Thickness(0, 14, 0, 2);
                return text;
            case MarkdownBlockKind.Heading3:
                text.FontSize = 16;
                text.FontWeight = FontWeight.SemiBold;
                text.Margin = new Thickness(0, 8, 0, 0);
                return text;
            case MarkdownBlockKind.Bullet:
                var row = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*"), Margin = new Thickness(8, 0, 0, 0) };
                var bullet = new TextBlock { Text = "•", Margin = new Thickness(0, 0, 10, 0) };
                Grid.SetColumn(text, 1);
                row.Children.Add(bullet);
                row.Children.Add(text);
                return row;
            default:
                return text;
        }
    }

    private static void FillInlines(SelectableTextBlock target, string text)
    {
        var inlines = target.Inlines ??= new InlineCollection();
        foreach (var part in SimpleMarkdown.ParseInline(text))
        {
            var run = new Run(part.Text);
            if (part.Bold)
                run.FontWeight = FontWeight.Bold;
            if (part.Code)
                run.FontFamily = new FontFamily("avares://OpenExamSuite.Shared.Avalonia/Assets/Fonts#IBM Plex Mono");

            inlines.Add(run);
        }
    }
}
