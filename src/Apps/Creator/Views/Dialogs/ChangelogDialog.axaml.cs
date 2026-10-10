using System.Reflection;
using Avalonia.Controls;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class ChangelogDialog : Window
{
    public ChangelogDialog()
    {
        InitializeComponent();
        DataContext = new { ChangelogText = ReadChangelog() };
    }

    private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

    private static string ReadChangelog()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var names = assembly.GetManifestResourceNames();
        var resourceName = names.FirstOrDefault(n => n.EndsWith("CHANGELOG.md", StringComparison.OrdinalIgnoreCase));
        if (resourceName == null)
            return "Changelog not found.";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return "Changelog not found.";

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
