using Avalonia.Controls;
using OpenExamSuite.Shared.Avalonia.Markdown;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class ChangelogDialog : Window
{
    public ChangelogDialog()
    {
        InitializeComponent();
        DocumentHost.Content = MarkdownRenderer.Render(ReadChangelog());
    }

    private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

    private static string ReadChangelog()
    {
        try
        {
            return BundledDocuments.Read("CHANGELOG.md");
        }
        catch (Exception)
        {
            return "Changelog not found.";
        }
    }
}
