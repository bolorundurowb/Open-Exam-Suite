using System;
using System.IO;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Shared.Avalonia.Dialogs;

public partial class LicenseDialog : Window
{
    public LicenseDialog()
    {
        InitializeComponent();
        LoadLicenseText();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void LoadLicenseText()
    {
        var tb = this.FindControl<TextBox>("LicenseText");
        if (tb is null) return;

        try
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("OpenExamSuite.Shared.Avalonia.LICENSE");

            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                tb.Text = reader.ReadToEnd();
            }
            else
            {
                tb.Text = "License file could not be found.";
            }

            tb.CaretIndex = 0;
        }
        catch
        {
            tb.Text = "An error occurred while loading the license file.";
        }
    }

    private void OnClose(object? sender, RoutedEventArgs e) => Close();
}
