using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Simulator.Views.Dialogs;

public partial class ExamPropertiesDialog : Window
{
    public ExamPropertiesDialog() : this(new Exam(), string.Empty) { }

    public ExamPropertiesDialog(Exam exam, string filePath)
    {
        InitializeComponent();
        Populate(exam, filePath);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void Populate(Exam exam, string filePath)
    {
        SetText("LblTitle",             exam.Properties.Title);
        SetText("LblFileVersion",       exam.Properties.Version.ToString());
        SetText("LblSectionNumber",     exam.Sections.Count.ToString());
        SetText("LblNumberOfQuestions", exam.NumberOfQuestions.ToString());
        SetText("LblPassingScore",      exam.Properties.Passmark.ToString(CultureInfo.InvariantCulture));
        SetText("LblTimeLimit",         exam.Properties.TimeLimit.ToString());
        SetText("LblFullPath",          filePath);

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            var info = new FileInfo(filePath);
            SetText("LblCreated",  info.CreationTime.ToShortDateString());
            SetText("LblFileSize", info.Length > 1022976
                ? (info.Length / 1048576.0).ToString("F") + " MB"
                : (info.Length /    1024.0).ToString("F") + " KB");
        }
    }

    private void SetText(string name, string value)
    {
        var tb = this.FindControl<TextBlock>(name);
        if (tb is not null) tb.Text = value;
    }

    private void OnClose(object? sender, RoutedEventArgs e) => Close();
}
