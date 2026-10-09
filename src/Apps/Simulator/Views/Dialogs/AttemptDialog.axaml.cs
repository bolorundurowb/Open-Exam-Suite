using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.States;

namespace OpenExamSuite.Simulator.Views.Dialogs;

/// <summary>
/// A saved attempt from the recent-attempts strip. Only the summary is stored, so there is no answer review here.
/// </summary>
public partial class AttemptDialog : Window
{
    public AttemptDialog()
        : this(null)
    {
    }

    public AttemptDialog(AttemptSummary? attempt)
    {
        InitializeComponent();
        Title = Strings.Get("Attempt_Title");
        if (attempt == null)
            return;

        var culture = CultureInfo.CurrentCulture;
        TitleText.Text = attempt.ExamTitle;
        DateText.Text = attempt.Date.ToLocalTime().ToString("f", culture);
        ResultText.Text = Strings.Get(attempt.Passed ? "Result_Passed" : "Result_NotPassed");
        ResultIcon.Data = (Avalonia.Media.Geometry)Application.Current!.FindResource(attempt.Passed ? "IconCheck" : "IconClose")!;
        var outcomeClass = attempt.Passed ? "pass" : "fail";
        ResultIcon.Classes.Add(outcomeClass);
        ResultText.Classes.Add(outcomeClass);
        ScoreText.Text = Strings.Format("Attempt_Score", attempt.ScorePercent, attempt.ScaledScore);
        TimeText.Text = Strings.Format("Attempt_TimeUsed", ResultsFormat.Duration(attempt.TimeUsed));

        CloseButton.Click += (_, _) => Close();
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
                Close();
        };
        Opened += (_, _) => CloseButton.Focus();
    }
}
