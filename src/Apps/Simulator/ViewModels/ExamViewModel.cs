using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Shared;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Models;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;

namespace OpenExamSuite.Simulator.ViewModels;

/// <summary>
/// The exam and practice view: question, options, countdown, navigator, flag, pause and check answer.
/// </summary>
public sealed partial class ExamViewModel : ViewModelBase, IDisposable
{
    private static readonly TimeSpan WarningVisibleFor = TimeSpan.FromSeconds(12);

    private readonly ShellServices _shell;
    private Question? _question;
    private bool _warnedLow;
    private bool _warnedCritical;
    private int _warningVersion;
    private bool _announcePending;
    private bool _feedbackAnnounced;
    private bool _applying;

    public ExamViewModel(ShellServices shell)
    {
        _shell = shell;
        Navigator = new NavigatorViewModel(JumpToAsync);
    }

    public NavigatorViewModel Navigator { get; }

    public ObservableCollection<OptionItemViewModel> Options { get; } = [];

    [ObservableProperty] private string _sectionName = string.Empty;
    [ObservableProperty] private string _questionNumberText = string.Empty;
    [ObservableProperty] private string _questionText = string.Empty;
    [ObservableProperty] private string _hint = string.Empty;
    [ObservableProperty] private bool _isMultiple;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    private Bitmap? _image;

    public bool HasImage => Image != null;

    [ObservableProperty] private bool _isPractice;
    [ObservableProperty] private bool _showTimer;
    [ObservableProperty] private string _timerText = string.Empty;
    [ObservableProperty] private bool _isLowTime;
    [ObservableProperty] private bool _isCriticalTime;
    [ObservableProperty] private string _timerStateText = string.Empty;

    [ObservableProperty] private string _answeredText = string.Empty;
    [ObservableProperty] private double _progressPercent;

    [ObservableProperty] private bool _isPaused;
    [ObservableProperty] private bool _isFlagged;
    [ObservableProperty] private string _flagText = string.Empty;
    [ObservableProperty] private bool _isLocked;
    [ObservableProperty] private bool _canGoPrevious;
    [ObservableProperty] private bool _canGoNext;
    [ObservableProperty] private bool _canCheck;
    [ObservableProperty] private bool _canClear;

    [ObservableProperty] private bool _showFeedback;
    [ObservableProperty] private bool _feedbackIsCorrect;
    [ObservableProperty] private string _feedbackText = string.Empty;
    [ObservableProperty] private bool _showExplanation;
    [ObservableProperty] private string _explanation = string.Empty;

    [ObservableProperty] private bool _showWarning;
    [ObservableProperty] private bool _warningIsCritical;
    [ObservableProperty] private string _warningText = string.Empty;

    /// <summary>Text for the polite live region read out by screen readers.</summary>
    [ObservableProperty] private string _announcement = string.Empty;

    /// <summary>Text for the assertive live region, used for time warnings.</summary>
    [ObservableProperty] private string _urgentAnnouncement = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowOverlayNavigator))]
    private bool _isNavigatorOpen;

    /// <summary>
    /// False when the window is narrow. The navigator then collapses to an overlay opened from the bottom bar.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowInlineNavigator))]
    [NotifyPropertyChangedFor(nameof(ShowNavigatorToggle))]
    [NotifyPropertyChangedFor(nameof(ShowOverlayNavigator))]
    private bool _isWide = true;

    public bool ShowInlineNavigator => IsWide;
    public bool ShowNavigatorToggle => !IsWide;
    public bool ShowOverlayNavigator => !IsWide && IsNavigatorOpen;

    public void Apply(ISessionState state)
    {
        AttemptState attempt;
        switch (state)
        {
            case PausedState paused:
                attempt = paused.Attempt;
                IsPaused = true;
                break;
            case AttemptState active:
                attempt = active;
                IsPaused = false;
                break;
            default:
                return;
        }

        var question = attempt.CurrentQuestion;
        var isNewQuestion = !ReferenceEquals(question, _question);
        if (isNewQuestion)
            LoadQuestion(attempt, question);

        ApplyAnswerState(attempt, question);
        ApplyHeader(attempt);
        Navigator.Update(attempt);

        if (isNewQuestion && _announcePending)
            Announcement = $"{QuestionNumberText}. {SectionName}";
        _announcePending = true;
    }

    public void Reset()
    {
        _question = null;
        _warnedLow = false;
        _warnedCritical = false;
        _announcePending = false;
        ShowWarning = false;
        Announcement = string.Empty;
        UrgentAnnouncement = string.Empty;
        ClearImage();
    }

    private void LoadQuestion(AttemptState attempt, Question question)
    {
        _question = question;
        _feedbackAnnounced = attempt.CurrentAnswer.ExplanationRevealed;
        var index = attempt.CurrentQuestionIndex;

        SectionName = attempt.Sections.FirstOrDefault(s => s.Questions.Contains(question))?.Title ?? string.Empty;
        QuestionNumberText = Strings.Format("Exam_QuestionOf", index + 1, attempt.Questions.Count);
        QuestionText = question.Text;
        IsMultiple = question.IsMultipleChoice;
        Hint = question.IsMultipleChoice
            ? (question.Answers is { Length: > 0 }
                ? Strings.Format("Exam_SelectN", question.Answers.Length)
                : Strings.Get("Exam_SelectAll"))
            : Strings.Get("Exam_SelectOne");

        SetImage(question.ImageData);

        Options.Clear();
        foreach (var option in question.Options)
        {
            Options.Add(new OptionItemViewModel(option.Alphabet, option.Text, question.IsMultipleChoice)
            {
                Changed = OnOptionChanged
            });
        }
    }

    private void ApplyAnswerState(AttemptState attempt, Question question)
    {
        var selection = attempt.CurrentAnswer;
        var selected = AnswerMath.SelectedLetters(selection);
        var revealed = selection.ExplanationRevealed;
        var correct = AnswerMath.CorrectLetters(question);

        _applying = true;
        foreach (var option in Options)
        {
            option.IsSelected = selected.Contains(option.Letter);
            option.Mark = !revealed
                ? OptionMark.None
                : option.IsSelected
                    ? (correct.Contains(option.Letter) ? OptionMark.YourAnswerCorrect : OptionMark.YourAnswerWrong)
                    : (correct.Contains(option.Letter) ? OptionMark.CorrectAnswer : OptionMark.None);
        }

        _applying = false;

        IsPractice = attempt.Settings.Mode == ExamMode.Practice;
        IsLocked = selection.IsLocked;
        IsFlagged = selection.IsFlagged;
        FlagText = Strings.Get(selection.IsFlagged ? "Exam_Unflag" : "Exam_Flag");

        CanGoPrevious = attempt.CurrentQuestionIndex > 0;
        CanGoNext = attempt.CurrentQuestionIndex < attempt.Questions.Count - 1;
        CanCheck = IsPractice && selection.IsAnswered && !selection.IsLocked;
        CanClear = selection.IsAnswered && !selection.IsLocked;

        ShowFeedback = IsPractice && revealed;
        if (ShowFeedback)
        {
            FeedbackIsCorrect = AnswerMath.IsCorrect(question, selection);
            FeedbackText = FeedbackIsCorrect
                ? Strings.Get("Practice_Correct")
                : Strings.Format("Practice_Incorrect", string.Join(", ", correct));
            if (!_feedbackAnnounced)
            {
                _feedbackAnnounced = true;
                Announcement = FeedbackText;
            }
        }
        else
        {
            FeedbackText = string.Empty;
        }

        ShowExplanation = ShowFeedback && !string.IsNullOrWhiteSpace(question.Explanation);
        Explanation = ShowExplanation ? question.Explanation : string.Empty;
    }

    private void ApplyHeader(AttemptState attempt)
    {
        ShowTimer = attempt.Settings.Mode == ExamMode.Exam;
        TimerText = ResultsFormat.Duration(attempt.TimeRemaining);
        IsLowTime = attempt.IsLowTimeWarning;
        IsCriticalTime = attempt.IsCriticalTimeWarning;
        TimerStateText = attempt.IsCriticalTimeWarning
            ? Strings.Get("Timer_Critical")
            : attempt.IsLowTimeWarning ? Strings.Get("Timer_Low") : string.Empty;

        AnsweredText = Strings.Format("Exam_AnsweredOf", attempt.AnsweredCount, attempt.Questions.Count);
        ProgressPercent = attempt.ProgressPercent;

        // A short exam never says "5 minutes remaining" at its start.
        var limitMinutes = attempt.Settings.TimerOverrideMinutes > 0
            ? attempt.Settings.TimerOverrideMinutes
            : attempt.Exam.Properties.TimeLimit;
        if (limitMinutes <= 5)
            _warnedLow = true;
        if (limitMinutes <= 1)
            _warnedCritical = true;

        if (attempt.IsCriticalTimeWarning && !_warnedCritical)
        {
            _warnedCritical = true;
            _warnedLow = true;
            RaiseWarning(Strings.Get("Timer_OneMinute"), critical: true);
        }
        else if (attempt.IsLowTimeWarning && !_warnedLow)
        {
            _warnedLow = true;
            RaiseWarning(Strings.Get("Timer_FiveMinutes"), critical: false);
        }
    }

    private void RaiseWarning(string text, bool critical)
    {
        WarningText = text;
        WarningIsCritical = critical;
        ShowWarning = true;
        // Clear first so an identical message is still announced.
        UrgentAnnouncement = string.Empty;
        UrgentAnnouncement = text;

        var version = ++_warningVersion;
        _ = Task.Delay(WarningVisibleFor).ContinueWith(_ =>
            _shell.Dispatcher.Post(() =>
            {
                if (version == _warningVersion)
                    ShowWarning = false;
            }));
    }

    private void SetImage(byte[]? data)
    {
        ClearImage();
        if (data is not { Length: > 0 })
            return;

        try
        {
            using var stream = new MemoryStream(data);
            Image = new Bitmap(stream);
        }
        catch (Exception)
        {
            // A corrupt image must not stop the question from showing.
            Image = null;
        }
    }

    private void ClearImage()
    {
        var old = Image;
        Image = null;
        old?.Dispose();
    }

    /// <summary>
    /// The control changed an option (click, Space or touch). Accepted only when the question is open;
    /// otherwise the control is put back to what the session holds.
    /// </summary>
    private void OnOptionChanged(OptionItemViewModel option)
    {
        if (_applying)
            return;

        if (IsLocked || IsPaused || _question == null)
        {
            RestoreSelection();
            return;
        }

        // A radio button that lost its check because another was chosen needs no action.
        if (!option.IsMultiple && !option.IsSelected)
            return;

        var question = _question;
        _ = _shell.RunAsync(async () =>
        {
            var current = CurrentAttempt().CurrentAnswer;
            var selection = question.IsMultipleChoice
                ? ToSelection(Options.Where(o => o.IsSelected).Select(o => o.Letter), current)
                : AnswerSelection.Answered(option.Letter, current.IsFlagged);
            await _shell.Session.AnswerQuestionAsync(selection);
        });
    }

    private static AnswerSelection ToSelection(IEnumerable<char> letters, AnswerSelection current)
    {
        var sorted = letters.OrderBy(c => c).ToList();
        return sorted.Count == 0
            ? AnswerSelection.Unanswered(current.IsFlagged)
            : AnswerSelection.Answered(System.Collections.Immutable.ImmutableArray.CreateRange(sorted), current.IsFlagged);
    }

    private void RestoreSelection()
    {
        var selected = _shell.Session.CurrentState switch
        {
            AttemptState attempt => AnswerMath.SelectedLetters(attempt.CurrentAnswer),
            PausedState paused => AnswerMath.SelectedLetters(paused.Attempt.CurrentAnswer),
            _ => []
        };

        _applying = true;
        foreach (var option in Options)
        {
            option.IsSelected = selected.Contains(option.Letter);
            option.RefreshSelection();
        }

        _applying = false;
    }

    private Task JumpToAsync(int index) =>
        IsPaused ? Task.CompletedTask : _shell.RunAsync(async () =>
        {
            IsNavigatorOpen = false;
            await _shell.Session.NavigateToQuestionAsync(index);
        });

    /// <summary>Selects or toggles the option with this letter, if the question has one.</summary>
    public Task SelectLetterAsync(char letter)
    {
        var question = _question;
        if (question == null || IsPaused || IsLocked)
            return Task.CompletedTask;

        var upper = char.ToUpperInvariant(letter);
        if (!Options.Any(o => o.Letter == upper))
            return Task.CompletedTask;

        return _shell.RunAsync(async () =>
        {
            var current = CurrentAttempt().CurrentAnswer;
            await _shell.Session.AnswerQuestionAsync(AnswerMath.Activate(question, current, upper));
        });
    }

    [RelayCommand]
    private Task ClearAnswerAsync() => _shell.RunAsync(async () =>
    {
        var current = CurrentAttempt().CurrentAnswer;
        await _shell.Session.AnswerQuestionAsync(AnswerSelection.Unanswered(current.IsFlagged));
    });

    [RelayCommand]
    public Task PreviousAsync() => IsPaused ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.NavigatePreviousAsync());

    [RelayCommand]
    public Task NextAsync() => IsPaused ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.NavigateNextAsync());

    [RelayCommand]
    public Task ToggleFlagAsync() => IsPaused ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.ToggleFlagAsync());

    [RelayCommand]
    public Task CheckAsync() => !CanCheck ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.CheckAnswerAsync());

    [RelayCommand]
    public Task ReviewAsync() => IsPaused ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.EnterReviewAndSubmitAsync());

    [RelayCommand]
    public Task PauseAsync() => !ShowTimer || IsPaused ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.PauseAsync());

    [RelayCommand]
    public Task ResumeAsync() => !IsPaused ? Task.CompletedTask : _shell.RunAsync(() => _shell.Session.ResumeAsync());

    [RelayCommand]
    private void ToggleNavigator() => IsNavigatorOpen = !IsNavigatorOpen;

    [RelayCommand]
    private async Task EnlargeImageAsync()
    {
        if (Image != null)
            await _shell.Dialogs.ShowImageAsync(Image);
    }

    [RelayCommand]
    private Task AbandonAsync() => _shell.RunAsync(async () =>
    {
        var wasRunning = !IsPaused && ShowTimer;
        if (wasRunning)
            await _shell.Session.PauseAsync();

        var abandon = await _shell.Dialogs.ConfirmAsync(
            Strings.Get("Abandon_Title"),
            Strings.Get("Abandon_Message"),
            Strings.Get("Abandon_Confirm"),
            Strings.Get("Abandon_Keep"));

        if (abandon)
        {
            await _shell.Session.ReturnToLibraryAsync();
        }
        else if (wasRunning)
        {
            await _shell.Session.ResumeAsync();
        }
    });

    // The attempt the view model last applied. Kept so selection changes read the latest answers.
    private AttemptState CurrentAttempt() => _shell.Session.CurrentState switch
    {
        PausedState paused => paused.Attempt,
        AttemptState attempt => attempt,
        _ => throw new InvalidOperationException("No attempt is in progress.")
    };

    public void Dispose() => ClearImage();
}
