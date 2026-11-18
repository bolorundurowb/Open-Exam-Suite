using Avalonia.Controls;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class LicenseDialog : Window
{
    public LicenseDialog()
    {
        InitializeComponent();
        DocumentText.Text = ReadLicense();
    }

    private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

    private static string ReadLicense()
    {
        try
        {
            return BundledDocuments.Read("LICENSE.txt");
        }
        catch (Exception)
        {
            return "License text not found.";
        }
    }
}
