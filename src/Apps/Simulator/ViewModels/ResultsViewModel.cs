using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Shared.Models;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;

namespace OpenExamSuite.Simulator.ViewModels;

public sealed class SectionRowViewModel
{
    public SectionRowViewModel(SectionResult result, bool isWeakest)
    {
        Title = result.SectionTitle;
        Correct = result.Correct;
        Total = result.Total;
        Percent = result.Total == 0 ? 0 : result.Correct * 100d / result.Total;
        IsWeakest = isWeakest;
    }

    public string Title { get; }
    public int Correct { get; }
    public int Total { get; }
    public double Percent { get; }
    public bool IsWeakest { get; }

    public string ScoreText => Strings.Format("Results_SectionScore", Correct, Total, ResultsFormat.Percent(Percent));

    public string AutomationName =>
        IsWeakest ? $"{Title}, {ScoreText}, {Strings.Get("Results_Weakest")}" : $"{Title}, {ScoreText}";
}

/// <summary>
/// Passed or Not passed as text plus an icon, the score, one bar with the pass mark, and the section breakdown.
/// The attempt has already been saved by the session when this screen is shown.
/// </summary>
public sealed partial class ResultsViewModel : ViewModelBase
{
    private readonly ShellServices _shell;

    public ResultsViewModel(ShellServices shell, ResultsState results)
    {
        _shell = shell;

        Passed = results.Passed;
        ResultLabel = Strings.Get(results.Passed ? "Result_Passed" : "Result_NotPassed");
        PercentScore = results.PercentScore;
        PercentText = ResultsFormat.Percent(results.PercentScore);
        ScaledText = Strings.Format("Results_Scaled", results.ScaledScore);
        PassMarkPercent = results.PassMarkPercent;
        PassMarkText = Strings.Format(
            "Results_PassMark",
            ResultsFormat.Percent(results.PassMarkPercent),
            (int)Math.Round(results.PassMarkPercent * 10));
        BarAutomationName = Strings.Format("Results_BarAutomation", PercentText, ResultsFormat.Percent(results.PassMarkPercent), ResultLabel);

        Candidate = string.IsNullOrWhiteSpace(results.CandidateName) ? Strings.Get("Results_NoCandidate") : results.CandidateName;
        ExamTitle = results.Exam.Properties.Title;
        ExamCode = results.Exam.Properties.Code;
        CompletedText = results.CompletedAt.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
        CorrectText = Strings.Format("Results_CorrectOf", results.CorrectAnswers, results.TotalQuestions);

        var allowed = results.Settings.Mode == ExamMode.Exam
            ? (results.Settings.TimerOverrideMinutes > 0 ? results.Settings.TimerOverrideMinutes : results.Exam.Properties.TimeLimit)
            : 0;
        TimeText = allowed > 0
            ? Strings.Format("Results_TimeOf", ResultsFormat.Duration(results.ElapsedTime), ResultsFormat.Duration(TimeSpan.FromMinutes(allowed)))
            : ResultsFormat.Duration(results.ElapsedTime);

        HasPrevious = results.PreviousScorePercent.HasValue && results.PreviousDate.HasValue;
        if (HasPrevious)
        {
            PreviousText = Strings.Format(
                "Results_Previous",
                results.PreviousScorePercent!.Value,
                results.PreviousScaledScore ?? 0,
                ResultsFormat.ShortDate(results.PreviousDate!.Value));
        }

        var weakest = results.SectionBreakdown
            .Where(s => s.Total > 0)
            .OrderBy(s => (double)s.Correct / s.Total)
            .FirstOrDefault();
        var flagWeakest = results.SectionBreakdown.Count(s => s.Total > 0) > 1
                          && weakest != null
                          && weakest.Correct < weakest.Total;
        foreach (var section in results.SectionBreakdown)
            Sections.Add(new SectionRowViewModel(section, flagWeakest && ReferenceEquals(section, weakest)));

        HasSections = Sections.Count > 0;
        HasMissedAnswers = results.GradingDetails?.Any(d => d.IsAnswered && !d.IsCorrect) == true;
    }

    public bool Passed { get; }
    public string ResultLabel { get; }
    public double PercentScore { get; }
    public string PercentText { get; }
    public string ScaledText { get; }
    public double PassMarkPercent { get; }
    public string PassMarkText { get; }
    public string BarAutomationName { get; }
    public string Candidate { get; }
    public string ExamTitle { get; }
    public string ExamCode { get; }
    public string CompletedText { get; }
    public string CorrectText { get; }
    public string TimeText { get; }
    public bool HasPrevious { get; }
    public string PreviousText { get; } = string.Empty;
    public bool HasSections { get; }
    public bool HasMissedAnswers { get; }

    public ObservableCollection<SectionRowViewModel> Sections { get; } = [];

    [RelayCommand]
    private Task ReviewAnswersAsync() => _shell.RunAsync(() => _shell.Session.EnterAnswerReviewAsync());

    [RelayCommand]
    private Task ReviewMissedAsync() =>
        _shell.RunAsync(() => _shell.Session.EnterAnswerReviewAsync(AnswerReviewFilter.Wrong));

    [RelayCommand]
    private Task RetakeAsync() => _shell.RunAsync(() => _shell.Session.RetakeAsync());

    [RelayCommand]
    private Task BackToLibraryAsync() => _shell.RunAsync(() => _shell.Session.ReturnToLibraryAsync());

    [RelayCommand]
    private Task ExportPdfAsync() => _shell.RunAsync(async () =>
    {
        var suggested = $"{(string.IsNullOrWhiteSpace(ExamTitle) ? "results" : ExamTitle)}-results.pdf";
        var path = await _shell.Prompts.SaveFileAsync(
            Strings.Get("Results_ExportTitle"),
            Strings.Get("Results_PdfFilter") + "|*.pdf",
            suggested,
            _shell.Paths.DocumentsDirectory);
        if (path == null)
            return;

        var bytes = await _shell.Session.ExportResultsPdfAsync();
        await File.WriteAllBytesAsync(path, bytes);
        await _shell.Toasts.ShowSuccessAsync(Strings.Get("Results_Exported"));
    });

    [RelayCommand]
    private Task PrintAsync() => _shell.RunAsync(() => _shell.Session.PrintResultsAsync());
}
