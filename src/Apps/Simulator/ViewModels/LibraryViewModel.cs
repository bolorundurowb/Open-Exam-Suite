using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Simulator.ViewModels;

public enum LibraryFilter
{
    All,
    NotAttempted,
    Passed,
    NotPassed,
    NeedsAttention
}

public enum LibrarySort
{
    Recent,
    Name,
    LastScore
}

public sealed record FilterOption(LibraryFilter Value, string Label);

public sealed record SortOption(LibrarySort Value, string Label);

/// <summary>
/// An exam card. Shows title, code, counts, time limit, pass mark and the last attempt, with
/// Practice and Exam actions, Edit when Creator is installed, and an overflow menu.
/// </summary>
public sealed class ExamCardViewModel
{
    private readonly LibraryViewModel _owner;

    public ExamCardViewModel(LibraryViewModel owner, ExamCard card, bool creatorAvailable)
    {
        _owner = owner;
        Card = card;
        CanEdit = creatorAvailable && card is { IsMissing: false, IsCorrupt: false };
        IsAvailable = card is { IsMissing: false, IsCorrupt: false };

        Title = string.IsNullOrWhiteSpace(card.Title) ? Strings.Get("Library_Untitled") : card.Title;
        Code = card.Code;
        HasCode = !string.IsNullOrWhiteSpace(card.Code);

        if (IsAvailable)
        {
            Facts =
            [
                Strings.Plural("Common_Questions", card.QuestionCount),
                Strings.Plural("Common_Sections", card.SectionCount),
                card.TimeLimitMinutes > 0
                    ? Strings.Plural("Common_Minutes", card.TimeLimitMinutes)
                    : Strings.Get("Common_Untimed"),
                Strings.Format("Library_PassMark", ResultsFormat.Percent(card.PassMarkPercent))
            ];
        }
        else
        {
            Facts = [];
        }

        HasLastAttempt = card.LastAttemptDate.HasValue && card.LastScorePercent.HasValue;
        if (HasLastAttempt)
        {
            LastAttemptText = Strings.Format(
                "Library_LastAttempt",
                card.LastScorePercent!.Value,
                ResultsFormat.ShortDate(card.LastAttemptDate!.Value));
            LastPassed = card.LastPassed == true;
            LastFailed = card.LastPassed == false;
            LastResultLabel = Strings.Get(LastPassed ? "Result_Passed" : "Result_NotPassed");
        }

        StatusText = card.IsMissing
            ? Strings.Get("Library_Missing")
            : card.IsCorrupt ? Strings.Get("Library_Corrupt") : string.Empty;
        StatusHint = card.IsMissing
            ? Strings.Get("Library_MissingHint")
            : card.IsCorrupt ? Strings.Get("Library_CorruptHint") : string.Empty;

        PracticeCommand = new AsyncRelayCommand(() => _owner.StartAsync(this, ExamMode.Practice), () => IsAvailable);
        ExamCommand = new AsyncRelayCommand(() => _owner.StartAsync(this, ExamMode.Exam), () => IsAvailable);
        OpenCommand = new AsyncRelayCommand(() => _owner.StartAsync(this, null), () => IsAvailable);
        EditCommand = new RelayCommand(() => _owner.Edit(this), () => CanEdit);
        PropertiesCommand = new AsyncRelayCommand(() => _owner.ShowPropertiesAsync(this), () => IsAvailable);
        DuplicateCommand = new AsyncRelayCommand(() => _owner.DuplicateAsync(this), () => IsAvailable);
        ShowInFolderCommand = new AsyncRelayCommand(() => _owner.ShowInFolderAsync(this));
        RemoveCommand = new AsyncRelayCommand(() => _owner.RemoveAsync(this));
        LocateCommand = new AsyncRelayCommand(() => _owner.LocateAsync(this));
    }

    public ExamCard Card { get; }
    public string FilePath => Card.FilePath;
    public string Title { get; }
    public string Code { get; }
    public bool HasCode { get; }
    public bool IsAvailable { get; }
    public bool IsMissing => Card.IsMissing;
    public bool IsCorrupt => Card.IsCorrupt;
    public bool NeedsAttention => !IsAvailable;
    public bool CanEdit { get; }
    public IReadOnlyList<string> Facts { get; }
    public string FactsText => string.Join(" · ", Facts);
    public bool HasLastAttempt { get; }
    public bool ShowNotAttempted => IsAvailable && !HasLastAttempt;
    public string LastAttemptText { get; } = string.Empty;
    public bool LastPassed { get; }
    public bool LastFailed { get; }
    public string LastResultLabel { get; } = string.Empty;
    public string StatusText { get; }
    public string StatusHint { get; }
    public string FileName => Path.GetFileName(Card.FilePath);

    public string AutomationName => HasLastAttempt
        ? $"{Title}, {FactsText}, {LastAttemptText}, {LastResultLabel}"
        : $"{Title}, {FactsText}{(NeedsAttention ? StatusText : string.Empty)}";

    public IAsyncRelayCommand PracticeCommand { get; }
    public IAsyncRelayCommand ExamCommand { get; }
    public IAsyncRelayCommand OpenCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IAsyncRelayCommand PropertiesCommand { get; }
    public IAsyncRelayCommand DuplicateCommand { get; }
    public IAsyncRelayCommand ShowInFolderCommand { get; }
    public IAsyncRelayCommand RemoveCommand { get; }
    public IAsyncRelayCommand LocateCommand { get; }
}

public sealed class AttemptItemViewModel
{
    private readonly LibraryViewModel _owner;

    public AttemptItemViewModel(LibraryViewModel owner, AttemptSummary attempt)
    {
        _owner = owner;
        Attempt = attempt;
        Title = attempt.ExamTitle;
        ScoreText = Strings.Format("Library_AttemptScore", attempt.ScorePercent, attempt.ScaledScore);
        DateText = ResultsFormat.ShortDate(attempt.Date);
        ResultLabel = Strings.Get(attempt.Passed ? "Result_Passed" : "Result_NotPassed");
        OpenCommand = new AsyncRelayCommand(() => _owner.ShowAttemptAsync(this));
    }

    public AttemptSummary Attempt { get; }
    public string Title { get; }
    public string ScoreText { get; }
    public string DateText { get; }
    public string ResultLabel { get; }
    public bool Passed => Attempt.Passed;
    public bool Failed => !Attempt.Passed;
    public string AutomationName => $"{Title}, {ResultLabel}, {ScoreText}, {DateText}";
    public IAsyncRelayCommand OpenCommand { get; }
}

/// <summary>
/// The Library home: cards in a grid or list, search, filter and sort, add and drop, recent attempts,
/// and designed empty, missing-file and corrupt states.
/// </summary>
public sealed partial class LibraryViewModel : ViewModelBase
{
    private const string ViewKey = "Simulator.LibraryView";
    private const string WelcomeKey = "Simulator.WelcomeDismissed";

    private readonly ShellServices _shell;
    private readonly IAppSettingsService _settings;
    private IReadOnlyList<ExamCard> _cards = [];
    private IReadOnlyList<AttemptSummary> _attempts = [];

    public LibraryViewModel(ShellServices shell, IAppSettingsService settings)
    {
        _shell = shell;
        _settings = settings;

        FilterOptions =
        [
            new FilterOption(LibraryFilter.All, Strings.Get("Library_Filter_All")),
            new FilterOption(LibraryFilter.NotAttempted, Strings.Get("Library_Filter_NotAttempted")),
            new FilterOption(LibraryFilter.Passed, Strings.Get("Library_Filter_Passed")),
            new FilterOption(LibraryFilter.NotPassed, Strings.Get("Library_Filter_NotPassed")),
            new FilterOption(LibraryFilter.NeedsAttention, Strings.Get("Library_Filter_NeedsAttention"))
        ];
        SortOptions =
        [
            new SortOption(LibrarySort.Recent, Strings.Get("Library_Sort_Recent")),
            new SortOption(LibrarySort.Name, Strings.Get("Library_Sort_Name")),
            new SortOption(LibrarySort.LastScore, Strings.Get("Library_Sort_LastScore"))
        ];
        _selectedFilter = FilterOptions[0];
        _selectedSort = SortOptions[0];
        _isGridView = _settings.Get(ViewKey, AppSettingsType.Other)?.Value != "list";
        _welcomeDismissed = _settings.Get(WelcomeKey, AppSettingsType.Other)?.Value == "1";
    }

    public IReadOnlyList<FilterOption> FilterOptions { get; }
    public IReadOnlyList<SortOption> SortOptions { get; }

    public ObservableCollection<ExamCardViewModel> Cards { get; } = [];
    public ObservableCollection<AttemptItemViewModel> RecentAttempts { get; } = [];

    public bool CreatorAvailable => _shell.Creator.IsInstalled;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private FilterOption _selectedFilter;
    [ObservableProperty] private SortOption _selectedSort;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListView))]
    private bool _isGridView;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private bool _hasNoMatches;
    [ObservableProperty] private bool _hasRecent;
    [ObservableProperty] private bool _isRecentExpanded = true;
    [ObservableProperty] private bool _showWelcome;
    [ObservableProperty] private bool _isDropTarget;

    private bool _welcomeDismissed;

    /// <summary>
    /// The list half of the view toggle. Setting it on selects list view; clearing it is ignored,
    /// so one of the two segments is always selected.
    /// </summary>
    public bool IsListView
    {
        get => !IsGridView;
        set
        {
            if (value)
                IsGridView = false;
        }
    }

    public void Apply(LibraryState state)
    {
        _cards = state.Exams;
        _attempts = state.RecentAttempts;
        IsLoading = state.IsLoading;
        Rebuild();
    }

    partial void OnSearchTextChanged(string value) => Rebuild();
    partial void OnSelectedFilterChanged(FilterOption value) => Rebuild();
    partial void OnSelectedSortChanged(SortOption value) => Rebuild();

    partial void OnIsGridViewChanged(bool value) =>
        _settings.Set(new AppSetting { Key = ViewKey, Value = value ? "grid" : "list" }, AppSettingsType.Other);

    private void Rebuild()
    {
        var creator = _shell.Creator.IsInstalled;
        IEnumerable<ExamCardViewModel> items = _cards.Select(c => new ExamCardViewModel(this, c, creator));

        var query = SearchText.Trim();
        if (query.Length > 0)
        {
            items = items.Where(c =>
                c.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || c.Code.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || c.FileName.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        items = SelectedFilter.Value switch
        {
            LibraryFilter.NotAttempted => items.Where(c => c.IsAvailable && !c.HasLastAttempt),
            LibraryFilter.Passed => items.Where(c => c.LastPassed),
            LibraryFilter.NotPassed => items.Where(c => c.LastFailed),
            LibraryFilter.NeedsAttention => items.Where(c => c.NeedsAttention),
            _ => items
        };

        items = SortItems(items, SelectedSort.Value);

        Cards.Clear();
        foreach (var item in items)
            Cards.Add(item);

        IsEmpty = !IsLoading && _cards.Count == 0;
        HasNoMatches = !IsLoading && _cards.Count > 0 && Cards.Count == 0;

        RecentAttempts.Clear();
        foreach (var attempt in _attempts)
            RecentAttempts.Add(new AttemptItemViewModel(this, attempt));
        HasRecent = RecentAttempts.Count > 0;

        ShowWelcome = !_welcomeDismissed && _cards.Count > 0 && !IsLoading;
    }

    internal static IEnumerable<ExamCardViewModel> SortItems(IEnumerable<ExamCardViewModel> items, LibrarySort sort) => sort switch
    {
        LibrarySort.Name => items.OrderBy(c => c.Title, StringComparer.CurrentCultureIgnoreCase),
        LibrarySort.LastScore => items
            .OrderBy(c => c.Card.LastScorePercent.HasValue ? 0 : 1)
            .ThenByDescending(c => c.Card.LastScorePercent ?? 0)
            .ThenBy(c => c.Title, StringComparer.CurrentCultureIgnoreCase),
        _ => items
            .OrderBy(c => c.Card.LastAttemptDate.HasValue ? 0 : 1)
            .ThenByDescending(c => c.Card.LastAttemptDate ?? DateTime.MinValue)
            .ThenBy(c => c.Title, StringComparer.CurrentCultureIgnoreCase)
    };

    // ---- Actions invoked by cards -------------------------------------------------------------

    internal Task StartAsync(ExamCardViewModel card, ExamMode? mode) => _shell.RunAsync(async () =>
    {
        var state = await _shell.Session.LoadExamAsync(card.FilePath);
        if (mode is { } requested && state.Settings.Mode != requested && !(requested == ExamMode.Practice && state.Exam.HideAnswers))
            await _shell.Session.UpdatePreExamSettingsAsync(state.Settings with { Mode = requested });
    });

    internal void Edit(ExamCardViewModel card)
    {
        if (!_shell.Creator.Launch(card.FilePath))
            _ = _shell.Toasts.ShowErrorAsync(Strings.Get("Library_CreatorFailed"));
    }

    internal async Task ShowPropertiesAsync(ExamCardViewModel card)
    {
        var properties = await _shell.Session.GetExamPropertiesAsync(card.FilePath);
        if (properties != null)
            await _shell.Dialogs.ShowPropertiesAsync(properties);
    }

    internal Task DuplicateAsync(ExamCardViewModel card) => _shell.RunAsync(async () =>
    {
        var result = await _shell.Session.DuplicateExamAsync(card.FilePath);
        if (result.Success)
            await _shell.Toasts.ShowSuccessAsync(Strings.Format("Library_Duplicated", card.Title));
        else
            await _shell.Toasts.ShowErrorAsync(ShellServices.DescribeAction(result.Status));
    });

    internal Task ShowInFolderAsync(ExamCardViewModel card) => _shell.RunAsync(async () =>
    {
        if (!await _shell.Launcher.ShowInFolderAsync(card.FilePath))
            await _shell.Toasts.ShowErrorAsync(Strings.Get("Library_FolderFailed"));
    });

    internal Task RemoveAsync(ExamCardViewModel card) => _shell.RunAsync(async () =>
    {
        await _shell.Session.RemoveExamAsync(card.FilePath);
        await _shell.Toasts.ShowSuccessAsync(Strings.Format("Library_Removed", card.Title));
    });

    internal Task LocateAsync(ExamCardViewModel card) => _shell.RunAsync(async () =>
    {
        var path = await _shell.Prompts.PickFileAsync(Strings.Get("Library_LocateTitle"), Strings.Get("Library_FileFilter") + "|*.oef");
        if (path == null)
            return;

        var result = await _shell.Session.RelocateExamAsync(card.FilePath, path);
        if (result.Success)
            await _shell.Toasts.ShowSuccessAsync(Strings.Get("Library_Relocated"));
        else
            await _shell.Toasts.ShowErrorAsync(ShellServices.DescribeAction(result.Status));
    });

    internal Task ShowAttemptAsync(AttemptItemViewModel item) => _shell.Dialogs.ShowAttemptAsync(item.Attempt);

    // ---- Library-level commands ---------------------------------------------------------------

    [RelayCommand]
    public Task AddExamAsync() => _shell.RunAsync(async () =>
    {
        var path = await _shell.Prompts.PickFileAsync(Strings.Get("Library_AddTitle"), Strings.Get("Library_FileFilter") + "|*.oef");
        if (path != null)
            await AddPathAsync(path);
    });

    /// <summary>Adds exams dropped onto the window or passed in from elsewhere.</summary>
    public Task AddPathsAsync(IEnumerable<string> paths) => _shell.RunAsync(async () =>
    {
        foreach (var path in paths)
            await AddPathAsync(path);
    });

    private async Task AddPathAsync(string path)
    {
        var result = await _shell.Session.AddExamAsync(path);
        var name = Path.GetFileNameWithoutExtension(path);
        if (result.Success)
            await _shell.Toasts.ShowSuccessAsync(Strings.Format("Library_Added", name));
        else
            await _shell.Toasts.ShowErrorAsync($"{name}: {ShellServices.DescribeAction(result.Status)}");
    }

    [RelayCommand]
    public void NewExam()
    {
        if (!_shell.Creator.Launch())
            _ = _shell.Toasts.ShowErrorAsync(Strings.Get("Library_CreatorFailed"));
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        SelectedFilter = FilterOptions[0];
    }

    [RelayCommand]
    private void DismissWelcome()
    {
        _welcomeDismissed = true;
        ShowWelcome = false;
        _settings.Set(new AppSetting { Key = WelcomeKey, Value = "1" }, AppSettingsType.Other);
    }

    [RelayCommand]
    private void ToggleRecent() => IsRecentExpanded = !IsRecentExpanded;
}
