using Avalonia.Controls;
using Avalonia.Input;
using OpenExamSuite.Simulator.Views.Controls;

namespace OpenExamSuite.Simulator.Views.Dialogs;

/// <summary>
/// Shows a bundled text document such as the changelog or the license.
/// </summary>
public partial class DocumentDialog : Window
{
    public DocumentDialog()
        : this(string.Empty, string.Empty, true)
    {
    }

    public DocumentDialog(string title, string text, bool isMarkdown)
    {
        InitializeComponent();
        Title = title;

        Host.Content = isMarkdown
            ? MarkdownRenderer.Render(text)
            : new SelectableTextBlock
            {
                Text = text,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                FontFamily = new Avalonia.Media.FontFamily("avares://OpenExamSuite.Shared.Avalonia/Assets/Fonts#IBM Plex Mono"),
                FontSize = 13
            };

        CloseButton.Click += (_, _) => Close();
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
                Close();
        };
        Opened += (_, _) => CloseButton.Focus();
    }
}
