using System.Reflection;
using Avalonia.Controls;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class LicenseDialog : Window
{
    public LicenseDialog()
    {
        InitializeComponent();
        DataContext = new { LicenseText = ReadLicense() };
    }

    private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

    private static string ReadLicense()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var names = assembly.GetManifestResourceNames();
        var resourceName = names.FirstOrDefault(n => n.EndsWith("LICENSE.txt", StringComparison.OrdinalIgnoreCase));
        if (resourceName == null)
            return "License text not found.";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return "License text not found.";

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
