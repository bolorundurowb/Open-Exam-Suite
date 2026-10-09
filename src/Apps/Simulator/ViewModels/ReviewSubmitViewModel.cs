using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.States;

namespace OpenExamSuite.Simulator.ViewModels;

/// <summary>A clickable question chip that jumps back to that question.</summary>
public sealed class QuestionChipViewModel
{
    public QuestionChipViewModel(int index)
    {
        Index = index;
    }

    public int Index { get; }

    public int Number => Index + 1;

    public string AutomationName => Strings.Format("Review_GoToQuestion", Number);
}

/// <summary>
/// Review and submit: counts, chips that jump back, the full navigator, and a confirmed submit.
/// Also shown during time-up, where the submission happens automatically after a short countdown.
/// </summary>
public sealed partial class ReviewSubmitViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private bool _timeUpAnnounced;

    public ReviewSubmitViewModel(ShellServices shell)
    {
        _shell = shell;
        Navigator = new NavigatorViewModel(index =>
            CanGoBack ? _shell.RunAsync(() => _shell.Session.NavigateToQuestionAsync(index)) : Task.CompletedTask);
    }

    public NavigatorViewModel Navigator { get; }

    public ObservableCollection<QuestionChipViewModel> UnansweredChips { get; } = [];

    public ObservableCollection<QuestionChipViewModel> FlaggedChips { get; } = [];

    [ObservableProperty] private int _answeredCount;
    [ObservableProperty] private int _unansweredCount;
    [ObservableProperty] private int _flaggedCount;
    [ObservableProperty] private bool _hasUnanswered;
    [ObservableProperty] private bool _hasFlagged;

    [ObservableProperty] private bool _isTimeUp;
    [ObservableProperty] private string _timeUpText = string.Empty;
    [ObservableProperty] private bool _canGoBack = true;

    [ObservableProperty] private bool _showTimer;
    [ObservableProperty] private string _timerText = string.Empty;
    [ObservableProperty] private bool _isLowTime;
    [ObservableProperty] private bool _isCriticalTime;

    /// <summary>Spoken once when time runs out.</summary>
    [ObservableProperty] private string _urgentAnnouncement = string.Empty;

    public void Apply(ISessionState state)
    {
        AttemptState attempt;
        IReadOnlyList<int> unanswered;
        IReadOnlyList<int> flagged;

        switch (state)
        {
            case ReviewAndSubmitState review:
                attempt = review.Attempt;
                unanswered = review.UnansweredIndices.OrderBy(i => i).ToList();
                flagged = review.FlaggedIndices.OrderBy(i => i).ToList();
                IsTimeUp = false;
                break;
            case TimeUpState timeUp:
                attempt = timeUp.Attempt;
                unanswered = attempt.Answers.Where(a => !a.Value.IsAnswered).Select(a => a.Key).OrderBy(i => i).ToList();
                flagged = attempt.Answers.Where(a => a.Value.IsFlagged).Select(a => a.Key).OrderBy(i => i).ToList();
                IsTimeUp = true;
                TimeUpText = Strings.Format("Review_TimeUp", timeUp.AutoSubmitCountdownSeconds);
                if (!_timeUpAnnounced)
                {
                    _timeUpAnnounced = true;
                    UrgentAnnouncement = Strings.Get("Review_TimeUpAnnounce");
                }

                break;
            default:
                return;
        }

        CanGoBack = !IsTimeUp;
        UnansweredCount = unanswered.Count;
        FlaggedCount = flagged.Count;
        AnsweredCount = attempt.Questions.Count - unanswered.Count;
        HasUnanswered = unanswered.Count > 0;
        HasFlagged = flagged.Count > 0;

        SyncChips(UnansweredChips, unanswered);
        SyncChips(FlaggedChips, flagged);
        Navigator.Update(attempt);

        ShowTimer = attempt.Settings.Mode == Session.Models.ExamMode.Exam;
        TimerText = ResultsFormat.Duration(attempt.TimeRemaining);
        IsLowTime = attempt.IsLowTimeWarning;
        IsCriticalTime = attempt.IsCriticalTimeWarning;
    }

    private static void SyncChips(ObservableCollection<QuestionChipViewModel> target, IReadOnlyList<int> indices)
    {
        // Rebuilding every second would reset keyboard focus, so only touch the list when it changes.
        if (target.Select(c => c.Index).SequenceEqual(indices))
            return;

        target.Clear();
        foreach (var index in indices)
            target.Add(new QuestionChipViewModel(index));
    }

    [RelayCommand]
    private Task BackAsync() => !CanGoBack ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.ReturnToAttemptAsync());

    [RelayCommand]
    private Task JumpAsync(QuestionChipViewModel? chip) =>
        chip == null || !CanGoBack
            ? Task.CompletedTask
            : _shell.RunAsync(() => _shell.Session.NavigateToQuestionAsync(chip.Index));

    [RelayCommand]
    private Task SubmitAsync() => _shell.RunAsync(async () =>
    {
        if (!IsTimeUp && UnansweredCount > 0)
        {
            var confirmed = await _shell.Dialogs.ConfirmAsync(
                Strings.Get("Review_ConfirmTitle"),
                Strings.Plural("Review_ConfirmMessage", UnansweredCount),
                Strings.Get("Review_SubmitAnyway"),
                Strings.Get("Review_GoBack"));
            if (!confirmed)
                return;
        }

        await _shell.Session.SubmitAsync();
    });
}
