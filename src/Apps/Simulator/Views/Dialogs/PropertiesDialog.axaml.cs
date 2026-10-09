using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Session.Models;

namespace OpenExamSuite.Simulator.Views.Dialogs;

public partial class PropertiesDialog : Window
{
    public sealed record PropertyRow(string Label, string Value);

    public PropertiesDialog()
        : this(null, null)
    {
    }

    public PropertiesDialog(ExamFileProperties? properties, Action<string>? showInFolder)
    {
        InitializeComponent();
        Title = Strings.Get("Properties_Title");

        if (properties == null)
            return;

        TitleText.Text = string.IsNullOrWhiteSpace(properties.Title) ? Strings.Get("Library_Untitled") : properties.Title;
        var culture = CultureInfo.CurrentCulture;
        Rows.ItemsSource = new[]
        {
            new PropertyRow(Strings.Get("Properties_Code"), properties.Code),
            new PropertyRow(Strings.Get("Properties_File"), properties.FilePath),
            new PropertyRow(Strings.Get("Properties_Size"), FormatSize(properties.FileSizeBytes)),
            new PropertyRow(Strings.Get("Properties_Modified"), properties.ModifiedAt.ToString("g", culture)),
            new PropertyRow(Strings.Get("Properties_Version"), properties.Version.ToString(culture)),
            new PropertyRow(Strings.Get("Properties_Questions"), properties.QuestionCount.ToString(culture)),
            new PropertyRow(Strings.Get("Properties_Sections"), properties.SectionCount.ToString(culture)),
            new PropertyRow(Strings.Get("Properties_TimeLimit"),
                properties.TimeLimitMinutes > 0
                    ? Strings.Plural("Common_Minutes", properties.TimeLimitMinutes)
                    : Strings.Get("Common_Untimed")),
            new PropertyRow(Strings.Get("Properties_PassMark"), string.Format(culture, "{0:0.#}%", properties.PassMarkPercent)),
            new PropertyRow(Strings.Get("Properties_HideAnswers"),
                Strings.Get(properties.HideAnswers ? "Common_Yes" : "Common_No"))
        };
        LegacyNotice.IsVisible = properties.IsLegacy;

        FolderButton.Click += (_, _) => showInFolder?.Invoke(properties.FilePath);
        CloseButton.Click += (_, _) => Close();
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
                Close();
        };
        Opened += (_, _) => CloseButton.Focus();
    }

    internal static string FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        double size = bytes;
        var unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return unit == 0
            ? $"{bytes} {units[0]}"
            : string.Format(CultureInfo.CurrentCulture, "{0:0.#} {1}", size, units[unit]);
    }
}
