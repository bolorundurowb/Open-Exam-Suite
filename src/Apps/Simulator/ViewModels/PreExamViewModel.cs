using System.Collections.Immutable;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;

namespace OpenExamSuite.Simulator.ViewModels;

public sealed partial class SectionChoiceViewModel : ObservableObject
{
    private readonly Action _changed;

    public SectionChoiceViewModel(string title, int questionCount, bool isSelected, Action changed)
    {
        Title = title;
        QuestionCount = questionCount;
        _isSelected = isSelected;
        _changed = changed;
    }

    public string Title { get; }

    public int QuestionCount { get; }

    public string Label => $"{Title} ({Strings.Plural("Common_Questions", QuestionCount)})";

    [ObservableProperty]
    private bool _isSelected;

    partial void OnIsSelectedChanged(bool value) => _changed();
}

/// <summary>
/// The pre-exam sheet. One screen with everything needed to start: mode, candidate, question set,
/// shuffle and timer options, a live summary, and a Start button that explains why it is disabled.
/// </summary>
public sealed partial class PreExamViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly ExamSummary _exam;
    private readonly ImmutableArray<int> _missedIndices;
    private bool _suppress = true;

    public PreExamViewModel(ShellServices shell, PreExamState state)
    {
        _shell = shell;
        _exam = state.Exam;
        var settings = state.Settings;
        _missedIndices = settings.MissedQuestionIndices;

        Title = _exam.Title;
        Code = _exam.Code;
        Instructions = _exam.Instructions;
        HideAnswers = _exam.HideAnswers;
        IsMissedSet = settings.QuestionSet == QuestionSetMode.MissedQuestions;
        TotalQuestions = _exam.TotalQuestions;
        DefaultMinutes = _exam.TimeLimitMinutes;
        PassMarkPercent = _exam.PassMarkPercent;
        MaxRandom = Math.Max(1, _exam.TotalQuestions);

        var selectedTitles = settings.SelectedSectionTitles;
        foreach (var (sectionTitle, count) in _exam.SectionQuestionCounts)
        {
            var isSelected = selectedTitles.IsEmpty || selectedTitles.Contains(sectionTitle);
            Sections.Add(new SectionChoiceViewModel(sectionTitle, count, isSelected, OnChoiceChanged));
        }

        _mode = settings.Mode;
        _questionSet = settings.QuestionSet;
        _candidateName = settings.CandidateName;
        _randomCount = settings.RandomQuestionCount > 0
            ? Math.Min(settings.RandomQuestionCount, MaxRandom)
            : Math.Min(20, MaxRandom);
        _shuffleQuestions = settings.ShuffleQuestions;
        _shuffleOptions = settings.ShuffleOptions;
        _timerMinutes = settings.TimerOverrideMinutes > 0
            ? settings.TimerOverrideMinutes
            : DefaultMinutes > 0 ? DefaultMinutes : null;

        _suppress = false;
        Refresh(state);
    }

    public string Title { get; }
    public string Code { get; }
    public string Instructions { get; }
    public bool HasInstructions => !string.IsNullOrWhiteSpace(Instructions);
    public bool HideAnswers { get; }
    public bool IsMissedSet { get; }
    public bool PracticeAvailable => !HideAnswers;
    public int TotalQuestions { get; }
    public int DefaultMinutes { get; }
    public double PassMarkPercent { get; }
    public decimal MaxRandom { get; }
    public string DefaultTimeText => DefaultMinutes > 0
        ? Strings.Format("PreExam_DefaultTime", Strings.Plural("Common_Minutes", DefaultMinutes))
        : Strings.Get("PreExam_NoDefaultTime");
    public string MissedText => Strings.Plural("PreExam_MissedSet", _missedIndices.Length);

    public ObservableCollection<SectionChoiceViewModel> Sections { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPracticeSelected))]
    [NotifyPropertyChangedFor(nameof(IsExamSelected))]
    [NotifyPropertyChangedFor(nameof(ShowTimerOptions))]
    [NotifyPropertyChangedFor(nameof(StartText))]
    private ExamMode _mode;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAllSelected))]
    [NotifyPropertyChangedFor(nameof(IsSectionsSelected))]
    [NotifyPropertyChangedFor(nameof(IsRandomSelected))]
    private QuestionSetMode _questionSet;

    [ObservableProperty] private string _candidateName = string.Empty;
    [ObservableProperty] private decimal? _randomCount;
    [ObservableProperty] private bool _shuffleQuestions;
    [ObservableProperty] private bool _shuffleOptions;
    [ObservableProperty] private decimal? _timerMinutes;

    [ObservableProperty] private string _summaryText = string.Empty;
    [ObservableProperty] private bool _canStart;
    [ObservableProperty] private string _reasonText = string.Empty;
    [ObservableProperty] private bool _hasReason;

    public bool IsPracticeSelected
    {
        get => Mode == ExamMode.Practice;
        set
        {
            if (value)
                Mode = ExamMode.Practice;
        }
    }

    public bool IsExamSelected
    {
        get => Mode == ExamMode.Exam;
        set
        {
            if (value)
                Mode = ExamMode.Exam;
        }
    }

    public bool IsAllSelected
    {
        get => QuestionSet == QuestionSetMode.AllQuestions;
        set
        {
            if (value)
                QuestionSet = QuestionSetMode.AllQuestions;
        }
    }

    public bool IsSectionsSelected
    {
        get => QuestionSet == QuestionSetMode.SelectedSections;
        set
        {
            if (value)
                QuestionSet = QuestionSetMode.SelectedSections;
        }
    }

    public bool IsRandomSelected
    {
        get => QuestionSet == QuestionSetMode.RandomN;
        set
        {
            if (value)
                QuestionSet = QuestionSetMode.RandomN;
        }
    }

    public bool ShowTimerOptions => Mode == ExamMode.Exam;

    public string StartText => Strings.Get(Mode == ExamMode.Exam ? "PreExam_StartExam" : "PreExam_StartPractice");

    partial void OnModeChanged(ExamMode value) => OnChoiceChanged();
    partial void OnQuestionSetChanged(QuestionSetMode value) => OnChoiceChanged();
    partial void OnCandidateNameChanged(string value) => OnChoiceChanged();
    partial void OnRandomCountChanged(decimal? value) => OnChoiceChanged();
    partial void OnShuffleQuestionsChanged(bool value) => OnChoiceChanged();
    partial void OnShuffleOptionsChanged(bool value) => OnChoiceChanged();
    partial void OnTimerMinutesChanged(decimal? value) => OnChoiceChanged();

    /// <summary>Takes the session's verdict on whether Start is allowed, then explains it in the user's language.</summary>
    public void Apply(PreExamState state) => Refresh(state);

    private void Refresh(PreExamState state)
    {
        var questionCount = CountQuestions();
        var effectiveMinutes = EffectiveMinutes();
        var needsTimeLimit = Mode == ExamMode.Exam && effectiveMinutes <= 0;

        CanStart = !state.StartDisabled && !needsTimeLimit;
        ReasonText = ExplainDisabled(state.StartDisabled, needsTimeLimit);
        HasReason = ReasonText.Length > 0;

        var questions = Strings.Plural("Common_Questions", questionCount);
        SummaryText = Mode == ExamMode.Exam
            ? Strings.Format(
                "PreExam_SummaryExam",
                questions,
                Strings.Plural("Common_Minutes", Math.Max(0, effectiveMinutes)),
                ResultsFormat.Percent(PassMarkPercent))
            : Strings.Format("PreExam_SummaryPractice", questions);
    }

    private string ExplainDisabled(bool sessionDisabled, bool needsTimeLimit)
    {
        if (Mode == ExamMode.Practice && HideAnswers)
            return Strings.Get("PreExam_PracticeHidden");

        if (sessionDisabled)
        {
            return QuestionSet switch
            {
                QuestionSetMode.SelectedSections => Strings.Get("PreExam_NoSections"),
                QuestionSetMode.RandomN => Strings.Get("PreExam_NoRandom"),
                _ => Strings.Get("PreExam_ZeroQuestions")
            };
        }

        return needsTimeLimit ? Strings.Get("PreExam_NeedTimeLimit") : string.Empty;
    }

    private int CountQuestions() => QuestionSet switch
    {
        QuestionSetMode.SelectedSections => Sections.Where(s => s.IsSelected).Sum(s => s.QuestionCount),
        QuestionSetMode.RandomN => Math.Min((int)(RandomCount ?? 0), TotalQuestions),
        QuestionSetMode.MissedQuestions => _missedIndices.Length,
        _ => TotalQuestions
    };

    private int EffectiveMinutes() => TimerMinutes is { } minutes && minutes > 0 ? (int)minutes : DefaultMinutes;

    private PreExamSettings BuildSettings()
    {
        var override_ = TimerMinutes is { } m && m > 0 && (int)m != DefaultMinutes ? (int)m : 0;
        return new PreExamSettings
        {
            Mode = Mode,
            CandidateName = CandidateName.Trim(),
            QuestionSet = QuestionSet,
            SelectedSectionTitles = Sections.Where(s => s.IsSelected).Select(s => s.Title).ToImmutableArray(),
            RandomQuestionCount = (int)(RandomCount ?? 0),
            ShuffleQuestions = ShuffleQuestions,
            ShuffleOptions = ShuffleOptions,
            TimerOverrideMinutes = Mode == ExamMode.Exam ? override_ : 0,
            RandomSeed = 0,
            MissedQuestionIndices = _missedIndices
        };
    }

    private void OnChoiceChanged()
    {
        if (_suppress)
            return;

        // The session is the authority on whether the selection is valid; its reply refreshes the sheet.
        _ = _shell.RunAsync(() => _shell.Session.UpdatePreExamSettingsAsync(BuildSettings()));
    }

    [RelayCommand]
    private void SelectAllSections()
    {
        foreach (var section in Sections)
            section.IsSelected = true;
    }

    [RelayCommand]
    private void ClearSections()
    {
        foreach (var section in Sections)
            section.IsSelected = false;
    }

    [RelayCommand]
    private Task StartAsync() => _shell.RunAsync(async () =>
    {
        if (!CanStart)
            return;

        await _shell.Session.UpdatePreExamSettingsAsync(BuildSettings());
        await _shell.Session.StartAttemptAsync();
    });

    [RelayCommand]
    private Task CancelAsync() => _shell.RunAsync(() => _shell.Session.ReturnToLibraryAsync());
}
