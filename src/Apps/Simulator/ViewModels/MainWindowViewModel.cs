using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Shared;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Storage.Interfaces;

namespace OpenExamSuite.Simulator.ViewModels;

/// <summary>
/// The shell: owns the current screen and maps the session's state onto a screen view model.
/// The session is the source of truth; this class never keeps its own copy of the attempt.
/// </summary>
public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly ThemeService _theme;
    private readonly IAppSettingsService _settings;
    private readonly IExamLibraryService _library;
    private readonly ToastService _toasts;
    private readonly List<string> _pendingPaths = [];
    private bool _initialized;

    private ExamViewModel? _exam;
    private ReviewSubmitViewModel? _review;
    private AnswerReviewViewModel? _answerReview;

    public MainWindowViewModel(
        ShellServices shell,
        ThemeService theme,
        IAppSettingsService settings,
        IExamLibraryService library,
        ToastService toasts)
    {
        _shell = shell;
        _theme = theme;
        _settings = settings;
        _library = library;
        _toasts = toasts;

        Library = new LibraryViewModel(shell, settings);
        _currentScreen = Library;
        _screenLabel = Strings.Get("Screen_Library");

        shell.Session.ResultsLabels = new ResultsReportLabels
        {
            Title = Strings.Get("Report_Title"),
            Exam = Strings.Get("Report_Exam"),
            Code = Strings.Get("Report_Code"),
            Candidate = Strings.Get("Report_Candidate"),
            Date = Strings.Get("Report_Date"),
            TimeUsed = Strings.Get("Report_TimeUsed"),
            Result = Strings.Get("Report_Result"),
            Passed = Strings.Get("Result_Passed"),
            NotPassed = Strings.Get("Result_NotPassed"),
            Score = Strings.Get("Report_Score"),
            PassMark = Strings.Get("Report_PassMark"),
            Correct = Strings.Get("Review_Correct"),
            Wrong = Strings.Get("Review_Wrong"),
            Unanswered = Strings.Get("Review_Unanswered"),
            Sections = Strings.Get("Report_Sections"),
            Questions = Strings.Get("Report_Questions")
        };

        shell.Session.StateChanged += state => shell.Dispatcher.Post(() => OnState(state));
        theme.Changed += OnThemeChanged;
    }

    private void OnThemeChanged()
    {
        OnPropertyChanged(nameof(ThemeMode));
        OnPropertyChanged(nameof(IsSystemTheme));
        OnPropertyChanged(nameof(IsLightMode));
        OnPropertyChanged(nameof(IsDarkMode));
        OnPropertyChanged(nameof(IsLightTheme));
        OnPropertyChanged(nameof(IsDarkTheme));
    }

    /// <summary>The window title and product name. It never changes with the screen.</summary>
    public string ProductName => AppInfo.ProductName;

    public LibraryViewModel Library { get; }

    public ObservableCollection<ToastItem> Toasts => _toasts.Toasts;

    public bool CreatorAvailable => _shell.Creator.IsInstalled;

    public ThemeMode ThemeMode => _theme.Mode;

    public bool IsSystemTheme => _theme.Mode == ThemeMode.System;

    public bool IsLightMode => _theme.Mode == ThemeMode.Light;

    public bool IsDarkMode => _theme.Mode == ThemeMode.Dark;

    /// <summary>
    /// The top bar's Light and Dark segments show what is on screen. Choosing one sets it explicitly;
    /// clearing one is ignored because the other segment's choice does the work.
    /// </summary>
    public bool IsLightTheme
    {
        get => !_theme.IsDarkShown;
        set
        {
            if (value)
                _theme.Set(ThemeMode.Light);
        }
    }

    public bool IsDarkTheme
    {
        get => _theme.IsDarkShown;
        set
        {
            if (value)
                _theme.Set(ThemeMode.Dark);
        }
    }

    [ObservableProperty] private object _currentScreen;
    [ObservableProperty] private string _screenLabel;
    [ObservableProperty] private bool _isAttemptInProgress;

    /// <summary>Polite live region for screen changes and toasts.</summary>
    [ObservableProperty] private string _announcement = string.Empty;

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        _theme.Load();
        await _shell.RunAsync(() => _shell.Session.InitializeAsync());

        var paths = _pendingPaths.ToList();
        _pendingPaths.Clear();
        foreach (var path in paths)
            await OpenFileAsync(path);
    }

    /// <summary>
    /// Opens an exam requested by the command line, the operating system or a second Simulator process.
    /// Requests that arrive before start-up finishes are queued.
    /// </summary>
    public async Task OpenFileAsync(string path)
    {
        if (!_initialized)
        {
            _pendingPaths.Add(path);
            return;
        }

        if (!IsSupportedPath(path))
            return;

        if (IsAttemptInProgress)
        {
            await _shell.Toasts.ShowWarningAsync(Strings.Get("Open_Busy"));
            return;
        }

        await _shell.RunAsync(() => _shell.Session.LoadExamAsync(path));
    }

    public static bool IsSupportedPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension is ".oef";
    }

    private void OnState(ISessionState state)
    {
        IsAttemptInProgress = state.Kind is SessionStateKind.Attempt or SessionStateKind.Paused
            or SessionStateKind.ReviewAndSubmit or SessionStateKind.TimeUp;

        switch (state)
        {
            case LibraryState library:
                DisposeAttemptScreens();
                Library.Apply(library);
                Show(Library, Strings.Get("Screen_Library"), Strings.Get("Screen_Library"));
                break;

            case PreExamState preExam:
                DisposeAttemptScreens();
                var setup = ModeLabel(preExam.Settings.Mode);
                if (CurrentScreen is PreExamViewModel existing && existing.Title == preExam.Exam.Title)
                {
                    existing.Apply(preExam);
                    ScreenLabel = setup;
                }
                else
                {
                    Show(new PreExamViewModel(_shell, preExam), setup, Strings.Get("Screen_Setup"));
                }

                break;

            case AttemptState or PausedState:
                _exam ??= new ExamViewModel(_shell);
                _exam.Apply(state);
                var attempt = state is PausedState paused ? paused.Attempt : (AttemptState)state;
                var taking = ModeLabel(attempt.Settings.Mode);
                Show(_exam, taking, taking);
                break;

            case ReviewAndSubmitState or TimeUpState:
                _review ??= new ReviewSubmitViewModel(_shell);
                _review.Apply(state);
                var reviewAttempt = state is TimeUpState timeUp ? timeUp.Attempt : ((ReviewAndSubmitState)state).Attempt;
                Show(
                    _review,
                    ModeLabel(reviewAttempt.Settings.Mode),
                    Strings.Get(state is TimeUpState ? "Review_TimeUpAnnounce" : "Screen_Review"));
                break;

            case ResultsState results:
                DisposeAttemptScreens();
                Show(new ResultsViewModel(_shell, results), ModeLabel(results.Settings.Mode), Strings.Get("Screen_Results"));
                break;

            case AnswerReviewState answerReview:
                _answerReview ??= new AnswerReviewViewModel(_shell);
                _answerReview.Apply(answerReview);
                Show(_answerReview, ModeLabel(answerReview.Settings.Mode), Strings.Get("Screen_AnswerReview"));
                break;
        }
    }

    /// <summary>The top bar names the mode. It stays Library, Practice, or Exam on every Simulator screen.</summary>
    private static string ModeLabel(ExamMode mode) =>
        Strings.Get(mode == ExamMode.Practice ? "Screen_Practice" : "Screen_Exam");

    private void Show(object screen, string modeLabel, string announcement)
    {
        if (!ReferenceEquals(CurrentScreen, screen))
        {
            CurrentScreen = screen;
            Announcement = announcement;
        }

        ScreenLabel = modeLabel;
    }

    private void DisposeAttemptScreens()
    {
        _exam?.Dispose();
        _exam = null;
        _review = null;
        _answerReview?.Dispose();
        _answerReview = null;
    }

    // ---- Menu and top bar commands -----------------------------------------------------------

    [RelayCommand]
    private Task AddExamAsync() => Library.AddExamAsync();

    [RelayCommand]
    private void NewExam() => Library.NewExam();

    [RelayCommand]
    private void SetTheme(ThemeMode mode) => _theme.Set(mode);

    [RelayCommand]
    private Task ShowAboutAsync() => _shell.Dialogs.ShowAboutAsync();

    [RelayCommand]
    private Task ShowLicenseAsync() => _shell.Dialogs.ShowLicenseAsync();

    [RelayCommand]
    private Task ShowChangelogAsync() => _shell.Dialogs.ShowChangelogAsync();

    [RelayCommand]
    private Task ClearHistoryAsync() => _shell.RunAsync(async () =>
    {
        var confirmed = await _shell.Dialogs.ConfirmAsync(
            Strings.Get("ClearHistory_Title"),
            Strings.Get("ClearHistory_Message"),
            Strings.Get("ClearHistory_Confirm"),
            Strings.Get("Dialog_Cancel"));
        if (!confirmed)
            return;

        _library.ClearAttempts();
        await _shell.Session.RefreshLibraryAsync();
        await _shell.Toasts.ShowSuccessAsync(Strings.Get("ClearHistory_Done"));
    });

    /// <summary>Shows the changelog once per version, on the first start after an update.</summary>
    public async Task ShowChangelogIfUpdatedAsync()
    {
        if (!AppInfo.ShouldShowChangelog(_settings))
            return;

        await _shell.Dialogs.ShowChangelogAsync();
        AppInfo.MarkChangelogShown(_settings);
    }

    /// <summary>
    /// Called when the window is closing during an attempt. Returns true when the user chose to abandon.
    /// </summary>
    public async Task<bool> ConfirmAbandonAsync() =>
        await _shell.Dialogs.ConfirmAsync(
            Strings.Get("Abandon_Title"),
            Strings.Get("Abandon_Message"),
            Strings.Get("Abandon_Confirm"),
            Strings.Get("Abandon_Keep"));
}
