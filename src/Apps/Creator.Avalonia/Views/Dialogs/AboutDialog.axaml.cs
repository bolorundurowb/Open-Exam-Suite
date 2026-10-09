using Avalonia.Controls;
using OpenExamSuite.Creator.Services;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
        DataContext = new { Version = $"Version {DialogService.AppVersion}" };
    }

    private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();
}
