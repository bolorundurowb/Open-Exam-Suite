using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Localization;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.Session.Models;
using OpenExamSuite.Storage.Enums;

namespace OpenExamSuite.Creator.ViewModels;

/// <summary>
/// One row in the start screen's "Recent exams" list.
/// </summary>
public sealed class RecentExamItemViewModel
{
    private readonly StartViewModel _owner;

    public RecentExamItemViewModel(StartViewModel owner, RecentExam exam)
    {
        _owner = owner;
        Exam = exam;
        Title = exam.Title;
        FilePath = exam.FilePath;
        AvatarLetter = string.IsNullOrWhiteSpace(exam.Title) ? "?" : exam.Title[..1];
        CountsText = exam.IsReadable
            ? $"{Strings.Plural("Common_Questions", exam.QuestionCount)} · {Strings.Plural("Common_Sections", exam.SectionCount)}"
            : string.Empty;
        EditedText = FormatEdited(exam.ModifiedAt);
        OpenCommand = new RelayCommand(() => _owner.OpenRecent(this));
        RemoveCommand = new RelayCommand(() => _owner.RemoveRecent(this));
    }

    public RecentExam Exam { get; }
    public string Title { get; }
    public string FilePath { get; }
    public string AvatarLetter { get; }
    public string CountsText { get; }
    public string EditedText { get; }
    public bool IsReadable => Exam.IsReadable;
    public string AutomationName => $"{Title}, {FilePath}, {CountsText}, {EditedText}";

    public IRelayCommand OpenCommand { get; }
    public IRelayCommand RemoveCommand { get; }

    private static string FormatEdited(DateTime modifiedAt)
    {
        if (modifiedAt == DateTime.MinValue)
            return string.Empty;

        var culture = CultureInfo.CurrentCulture;
        return modifiedAt.Date == DateTime.Today
            ? Strings.Format("Start_EditedToday", modifiedAt.ToString("HH:mm", culture))
            : Strings.Format("Start_EditedOn", modifiedAt.ToString("MMM d", culture));
    }
}

/// <summary>
/// The Creator start screen, shown when no exam is open: branding and New/Open/Import actions on
/// the left, and the recent-exams list plus a drop zone on the right.
/// </summary>
public sealed partial class StartViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly MainWindowViewModel _host;

    public StartViewModel(ShellServices shell, MainWindowViewModel host)
    {
        _shell = shell;
        _host = host;
        RefreshRecents();
    }

    public ObservableCollection<RecentExamItemViewModel> RecentExams { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoRecents))]
    private bool _hasRecents;

    [ObservableProperty] private bool _isDropTarget;

    public bool HasNoRecents => !HasRecents;

    public string SuiteLabel => Strings.Get("Start_Suite");
    public string Title => Strings.Get("Start_Title");
    public string Tagline => Strings.Get("Start_Tagline");
    public string NewExamLabel => Strings.Get("Start_NewExam");
    public string OpenExamLabel => Strings.Get("Start_OpenExam");
    public string ImportLabel => Strings.Get("Start_Import");
    public string NewShortcut => Strings.Get("Start_ShortcutNew");
    public string OpenShortcut => Strings.Get("Start_ShortcutOpen");
    public string RecentExamsLabel => Strings.Get("Start_RecentExams");
    public string ClearHistoryLabel => Strings.Get("Start_ClearHistory");
    public string DropTitle => Strings.Get("Start_DropTitle");
    public string DropHint => Strings.Get("Start_DropHint");
    public string NoRecentsLabel => Strings.Get("Start_NoRecents");
    public string VersionLabel => Strings.Format("Start_Version", DialogService.AppVersion);

    public void RefreshRecents()
    {
        RecentExams.Clear();
        foreach (var exam in _shell.Recents.GetRecents())
            RecentExams.Add(new RecentExamItemViewModel(this, exam));

        HasRecents = RecentExams.Count > 0;
    }

    internal void OpenRecent(RecentExamItemViewModel item) => _ = _host.OpenFileAsync(item.FilePath);

    internal void RemoveRecent(RecentExamItemViewModel item)
    {
        _shell.Library.RemoveExam(ExamCatalog.Creator, item.FilePath);
        RefreshRecents();
    }

    [RelayCommand]
    private async Task NewExamAsync() => await _host.NewExamAsync();

    [RelayCommand]
    private async Task OpenExamAsync() => await _host.OpenExamAsync();

    [RelayCommand]
    private async Task ImportAsync() => await _host.ImportExamAsync();

    [RelayCommand]
    private void ClearHistory()
    {
        _shell.Library.ClearExams(ExamCatalog.Creator);
        RefreshRecents();
        _shell.Toasts.Show(Strings.Get("Toast_HistoryCleared"));
    }

    /// <summary>Opens the first supported file dropped onto the start screen.</summary>
    public async Task DropFilesAsync(IEnumerable<string> paths)
    {
        var path = paths.FirstOrDefault(MainWindowViewModel.IsSupportedPath);
        if (path != null)
            await _host.OpenFileAsync(path);
    }
}
