using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Localization;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.Session.Models;
using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Storage.Enums;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly ThemeService _theme;
    private readonly ToastService _toasts;
    private readonly string _previewPath = Path.Combine(Path.GetTempPath(), $"oes-preview-{Environment.ProcessId}.oef");
    private readonly WorkspaceViewModel _workspace;
    private readonly StartViewModel _start;
    private DispatcherTimer? _autosave;
    private string? _untitledKey;
    private bool _initialized;
    private bool _closingWithoutSave;

    public MainWindowViewModel(ShellServices shell, ThemeService theme, ToastService toasts)
    {
        _shell = shell;
        _theme = theme;
        _toasts = toasts;

        _workspace = new WorkspaceViewModel(shell);
        _start = new StartViewModel(shell, this);
        _currentScreen = _start;
        _title = Strings.Get("AppName");

        _shell.Document.Changed += OnDocumentChanged;
        _shell.Document.ProblemsChanged += _ => OnPropertyChanged(nameof(HasProblems));
        _shell.Document.Opened += OnDocumentOpened;
        _theme.Changed += OnThemeChanged;
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

    public string ProductName => Strings.Get("AppName");

    public string ScreenLabel => Strings.Get("Screen_Creator");

    public bool IsStartScreen => CurrentScreen is StartViewModel;

    public bool ShowProductBar => !IsStartScreen;

    public bool IsDirty => _shell.Document.IsDirty;

    public string? FilePath => _shell.Document.FilePath;

    public bool HasProblems => _shell.Document.Problems.Count > 0;

    public bool IsLegacy => _shell.Document.IsLegacy;

    public ObservableCollection<ToastItem> Toasts => _toasts.Toasts;

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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStartScreen))]
    [NotifyPropertyChangedFor(nameof(ShowProductBar))]
    private object _currentScreen;

    [ObservableProperty] private string _title;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveAsCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportJsonCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportXmlCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportPdfCommand))]
    [NotifyCanExecuteChangedFor(nameof(TryExamCommand))]
    [NotifyCanExecuteChangedFor(nameof(UndoCommand))]
    [NotifyCanExecuteChangedFor(nameof(RedoCommand))]
    private bool _hasDocument;

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        _theme.Load();
        await CheckRecoveryAsync();
        _autosave = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _autosave.Tick += (_, _) => Autosave();
        _autosave.Start();
    }

    public async Task OpenFileAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !IsSupportedPath(path))
            return;

        if (!await ConfirmUnsavedAndSaveAsync())
            return;

        LoadPath(path);
    }

    public static bool IsSupportedPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension is ".oef" or ".json" or ".xml";
    }

    public Task NewExamAsync() => NewAsync();

    public Task OpenExamAsync() => OpenAsync();

    public async Task ImportExamAsync()
    {
        if (!await ConfirmUnsavedAndSaveAsync())
            return;

        var path = await _shell.Dialogs.PickOpenFileAsync(
            Strings.Get("Dialog_ImportTitle"),
            DialogService.JsonType,
            DialogService.XmlType);

        if (string.IsNullOrEmpty(path))
            return;

        LoadPath(path);
    }

    public void Shutdown()
    {
        if (_closingWithoutSave)
            return;

        Autosave();
    }

    [RelayCommand]
    private async Task NewAsync()
    {
        if (!await ConfirmUnsavedAndSaveAsync())
            return;

        _shell.Document.NewDocument();
        UpdateTitle();
    }

    [RelayCommand]
    private async Task OpenAsync()
    {
        if (!await ConfirmUnsavedAndSaveAsync())
            return;

        var path = await _shell.Dialogs.PickOpenFileAsync(
            Strings.Get("Dialog_OpenTitle"),
            DialogService.OefType,
            DialogService.JsonType,
            DialogService.XmlType);

        if (string.IsNullOrEmpty(path))
            return;

        LoadPath(path);
    }

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            await SaveAsAsync();
            return;
        }

        await ExecuteSaveAsync(FilePath);
    }

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private async Task SaveAsAsync()
    {
        var path = await _shell.Dialogs.PickSaveFileAsync(
            Strings.Get("Dialog_SaveTitle"),
            _shell.Document.Exam.Properties.Title,
            DialogService.OefType);

        if (string.IsNullOrEmpty(path))
            return;

        await ExecuteSaveAsync(path);
    }

    [RelayCommand]
    private async Task ImportJsonAsync()
    {
        if (!await ConfirmUnsavedAndSaveAsync())
            return;

        var path = await _shell.Dialogs.PickOpenFileAsync(
            Strings.Get("Dialog_ImportTitle"),
            DialogService.JsonType);

        if (string.IsNullOrEmpty(path))
            return;

        LoadPath(path);
    }

    [RelayCommand]
    private async Task ImportXmlAsync()
    {
        if (!await ConfirmUnsavedAndSaveAsync())
            return;

        var path = await _shell.Dialogs.PickOpenFileAsync(
            Strings.Get("Dialog_ImportXmlTitle"),
            DialogService.XmlType);

        if (string.IsNullOrEmpty(path))
            return;

        LoadPath(path);
    }

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private async Task ExportJsonAsync()
    {
        var path = await _shell.Dialogs.PickSaveFileAsync(
            Strings.Get("Dialog_ExportJsonTitle"),
            _shell.Document.Exam.Properties.Title,
            DialogService.JsonType);

        if (string.IsNullOrEmpty(path))
            return;

        var result = _shell.Document.SaveJson(path);
        if (result.Success)
            _toasts.Show(Strings.Get("Toast_Saved"));
        else
            _toasts.ShowWarning(SaveFailure(result));
    }

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private async Task ExportXmlAsync()
    {
        var path = await _shell.Dialogs.PickSaveFileAsync(
            Strings.Get("Dialog_ExportXmlTitle"),
            _shell.Document.Exam.Properties.Title,
            DialogService.XmlType);

        if (string.IsNullOrEmpty(path))
            return;

        var result = _shell.Document.SaveXml(path);
        if (result.Success)
            _toasts.Show(Strings.Get("Toast_Saved"));
        else
            _toasts.ShowWarning(SaveFailure(result));
    }

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private async Task ExportPdfAsync()
    {
        var path = await _shell.Dialogs.PickSaveFileAsync(
            Strings.Get("Dialog_ExportPdfTitle"),
            _shell.Document.Exam.Properties.Title,
            DialogService.PdfType);

        if (string.IsNullOrEmpty(path))
            return;

        var result = _shell.Document.SavePdf(path);
        if (result.Success)
            _toasts.Show(Strings.Get("Toast_Saved"));
        else
            _toasts.ShowWarning(SaveFailure(result));
    }

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private void TryExam()
    {
        if (!_shell.Simulator.IsInstalled)
        {
            _toasts.ShowWarning(Strings.Get("Toast_NoSimulator"));
            return;
        }

        var path = FilePath;
        if (string.IsNullOrEmpty(path) || IsDirty)
        {
            var result = _shell.Document.WriteCopy(_previewPath);
            if (!result.Success)
            {
                _toasts.ShowWarning(SaveFailure(result));
                return;
            }

            path = _previewPath;
        }

        _shell.Simulator.LaunchPractice(path);
        ReturnFocusToCreator();
    }

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private void Undo() => _shell.Document.Undo();

    [RelayCommand(CanExecute = nameof(CanEditDocument))]
    private void Redo() => _shell.Document.Redo();

    [RelayCommand]
    private void SetTheme(ThemeMode mode) => _theme.Set(mode);

    [RelayCommand]
    private async Task ShowAboutAsync() => await _shell.Dialogs.ShowAboutAsync();

    [RelayCommand]
    private async Task ShowLicenseAsync() => await _shell.Dialogs.ShowLicenseAsync();

    [RelayCommand]
    private async Task ShowChangelogAsync() => await _shell.Dialogs.ShowChangelogAsync();

    public async Task<bool> ConfirmCloseAsync()
    {
        if (!_shell.Document.IsDirty)
            return true;

        var result = await _shell.Dialogs.ConfirmUnsavedChangesAsync(Strings.Get("Title_Unsaved"));
        switch (result)
        {
            case PromptResult.Save:
                await SaveAsync();
                return !_shell.Document.IsDirty;
            case PromptResult.DontSave:
                DiscardRecovery();
                _closingWithoutSave = true;
                return true;
            default:
                return false;
        }
    }

    private async Task<bool> ConfirmUnsavedAndSaveAsync()
    {
        if (!_shell.Document.IsDirty)
            return true;

        var result = await _shell.Dialogs.ConfirmUnsavedChangesAsync(Strings.Get("Title_Unsaved"));
        switch (result)
        {
            case PromptResult.Save:
                await SaveAsync();
                return !_shell.Document.IsDirty;
            case PromptResult.DontSave:
                DiscardRecovery();
                return true;
            default:
                return false;
        }
    }

    private async Task ExecuteSaveAsync(string path)
    {
        var previousKey = RecoveryKey();
        var result = _shell.Document.Save(path);
        if (result.Success)
        {
            _untitledKey = null;
            RecoveryCatalog.Delete(previousKey);
            RecoveryCatalog.Delete(RecoveryCatalog.KeyForPath(path));
            _shell.Library.AddExam(ExamCatalog.Creator, path, _shell.Document.Exam.Properties.Title);
            _toasts.Show(Strings.Get("Toast_Saved"));
            UpdateTitle();
        }
        else
        {
            _toasts.ShowWarning(SaveFailure(result));
        }
    }

    private void LoadPath(string path)
    {
        var extension = Path.GetExtension(path);
        var result = extension.Equals(".json", StringComparison.OrdinalIgnoreCase)
            ? _shell.Document.LoadJson(path)
            : extension.Equals(".xml", StringComparison.OrdinalIgnoreCase)
                ? _shell.Document.LoadXml(path)
                : _shell.Document.Load(path);

        if (!result.Success)
        {
            _toasts.ShowWarning($"{Strings.Get("Toast_OpenFailed")} {IoMessages.Explain(result.Error, null)}");
            return;
        }

        if (extension.Equals(".oef", StringComparison.OrdinalIgnoreCase))
            _shell.Library.AddExam(ExamCatalog.Creator, path, _shell.Document.Exam.Properties.Title);

        _untitledKey = null;
        UpdateTitle();
        OnPropertyChanged(nameof(IsLegacy));
    }

    private async Task CheckRecoveryAsync()
    {
        var offer = RecoveryCatalog.FindNewer(_shell.Document).FirstOrDefault();
        if (offer == null)
            return;

        var recover = await _shell.Dialogs.ConfirmRecoveryAsync(offer.SourcePath);
        if (!recover)
        {
            RecoveryCatalog.Delete(offer.Key);
            return;
        }

        var result = _shell.Document.LoadRecovered(offer.RecoveryPath, offer.SourcePath);
        if (!result.Success)
        {
            _toasts.ShowWarning($"{Strings.Get("Toast_OpenFailed")} {IoMessages.Explain(result.Error, null)}");
            return;
        }

        if (string.IsNullOrWhiteSpace(offer.SourcePath))
            RecoveryCatalog.RememberUntitledKey(offer.Key, ref _untitledKey);
        else
            _untitledKey = null;

        UpdateTitle();
        OnPropertyChanged(nameof(IsLegacy));
    }

    private void Autosave() => RecoveryCatalog.Write(_shell.Document, RecoveryKey(), FilePath);

    private void DiscardRecovery()
    {
        RecoveryCatalog.Delete(RecoveryKey());
        _untitledKey = null;
    }

    private string RecoveryKey() =>
        string.IsNullOrWhiteSpace(FilePath)
            ? RecoveryCatalog.KeyForUntitled(ref _untitledKey)
            : RecoveryCatalog.KeyForPath(FilePath);

    private static string SaveFailure(DocumentSaveResult result) =>
        $"{Strings.Get("Toast_SaveFailed")} {IoMessages.Explain(result.Error, result.Detail)}";

    private static void ReturnFocusToCreator()
    {
        void Activate()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow?.Activate();
        }

        Activate();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            Activate();
        };
        timer.Start();
    }

    private void OnDocumentChanged()
    {
        HasDocument = _shell.Document.HasDocument;
        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(HasProblems));
        OnPropertyChanged(nameof(IsLegacy));
        UpdateTitle();
    }

    private void OnDocumentOpened()
    {
        HasDocument = true;
        ShowWorkspace();
    }

    private void ShowWorkspace()
    {
        if (ReferenceEquals(CurrentScreen, _workspace))
            return;

        CurrentScreen = _workspace;
    }

    private bool CanEditDocument() => _shell.Document.HasDocument;

    private void UpdateTitle()
    {
        if (!_shell.Document.HasDocument)
        {
            Title = Strings.Get("AppName");
            return;
        }

        var file = string.IsNullOrEmpty(FilePath) ? "Untitled" : Path.GetFileName(FilePath);
        var dirty = IsDirty ? "*" : string.Empty;
        Title = $"{Strings.Get("AppName")} - {file}{dirty}";
    }
}
