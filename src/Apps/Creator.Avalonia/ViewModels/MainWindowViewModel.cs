using System.Collections.ObjectModel;
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
    private bool _initialized;

    public MainWindowViewModel(ShellServices shell, ThemeService theme, ToastService toasts)
    {
        _shell = shell;
        _theme = theme;
        _toasts = toasts;

        _currentScreen = new WorkspaceViewModel(shell);
        _title = Strings.Get("AppName");

        _shell.Document.Changed += OnDocumentChanged;
        _shell.Document.ProblemsChanged += _ => OnPropertyChanged(nameof(HasProblems));
        _theme.Changed += () => OnPropertyChanged(nameof(ThemeMode));
    }

    public string ProductName => Strings.Get("AppName");

    public string ScreenLabel => Strings.Get("Screen_Creator");

    public bool IsDirty => _shell.Document.IsDirty;

    public string? FilePath => _shell.Document.FilePath;

    public bool HasProblems => _shell.Document.Problems.Count > 0;

    public ObservableCollection<ToastItem> Toasts => _toasts.Toasts;

    public ThemeMode ThemeMode => _theme.Mode;

    [ObservableProperty] private object _currentScreen;
    [ObservableProperty] private string _title;

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        _theme.Load();
        await CheckRecoveryAsync();
    }

    public async Task OpenFileAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !IsSupportedPath(path))
            return;

        if (!await ConfirmUnsavedAndSaveAsync())
            return;

        var result = _shell.Document.Load(path);
        if (result.Success)
        {
            _shell.Library.AddExam(ExamCatalog.Creator, path, _shell.Document.Exam.Properties.Title);
            UpdateTitle();
        }
        else
        {
            _toasts.ShowWarning($"Could not open {Path.GetFileName(path)}: {result.Error}");
        }
    }

    public static bool IsSupportedPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension is ".oef" or ".json";
    }

    public void Shutdown()
    {
        if (!string.IsNullOrEmpty(FilePath))
        {
            var recoveryPath = GetRecoveryPath();
            _shell.Document.WriteRecoveryCopy(recoveryPath);
        }
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
            DialogService.JsonType);

        if (string.IsNullOrEmpty(path))
            return;

        if (Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            var result = _shell.Document.LoadJson(path);
            if (!result.Success)
            {
                _toasts.ShowWarning($"Could not import {Path.GetFileName(path)}: {result.Error}");
                return;
            }
        }
        else
        {
            await OpenFileAsync(path);
            return;
        }

        UpdateTitle();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            await SaveAsAsync();
            return;
        }

        await ExecuteSaveAsync(FilePath);
    }

    [RelayCommand]
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
        var path = await _shell.Dialogs.PickOpenFileAsync(
            Strings.Get("Dialog_ImportTitle"),
            DialogService.JsonType);

        if (string.IsNullOrEmpty(path))
            return;

        var result = _shell.Document.LoadJson(path);
        if (result.Success)
            UpdateTitle();
        else
            _toasts.ShowWarning($"Could not import {Path.GetFileName(path)}: {result.Error}");
    }

    [RelayCommand]
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
            _toasts.ShowWarning($"Export failed: {result.Error}");
    }

    [RelayCommand]
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
            _toasts.ShowWarning($"Export failed: {result.Error}");
    }

    [RelayCommand]
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
            _toasts.ShowWarning($"Export failed: {result.Error}");
    }

    [RelayCommand]
    private void TryExam()
    {
        var path = FilePath;
        if (string.IsNullOrEmpty(path) || IsDirty)
        {
            path = Path.Combine(Path.GetTempPath(), $"oes-preview-{Guid.NewGuid():N}.oef");
            var result = _shell.Document.Save(path);
            if (!result.Success)
            {
                _toasts.ShowWarning("Could not prepare preview file.");
                return;
            }
        }

        if (!_shell.Simulator.IsInstalled)
        {
            _toasts.ShowWarning(Strings.Get("Toast_NoSimulator"));
            return;
        }

        _shell.Simulator.LaunchPractice(path);
    }

    [RelayCommand]
    private void Undo() => _shell.Document.Undo();

    [RelayCommand]
    private void Redo() => _shell.Document.Redo();

    [RelayCommand]
    private void ToggleTheme() => _theme.Toggle();

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
                return true;
            default:
                return false;
        }
    }

    private async Task ExecuteSaveAsync(string path)
    {
        var result = _shell.Document.Save(path);
        if (result.Success)
        {
            _shell.Library.AddExam(ExamCatalog.Creator, path, _shell.Document.Exam.Properties.Title);
            _toasts.Show(Strings.Get("Toast_Saved"));
            UpdateTitle();
        }
        else
        {
            _toasts.ShowWarning($"Save failed: {result.Error}");
        }
    }

    private async Task CheckRecoveryAsync()
    {
        var recoveryPath = GetRecoveryPath();
        if (!_shell.Document.HasRecoveryCopy(recoveryPath))
            return;

        var recover = await _shell.Dialogs.ConfirmRecoveryAsync(recoveryPath);
        if (!recover)
            return;

        var result = _shell.Document.Load(recoveryPath);
        if (result.Success)
            UpdateTitle();
    }

    private string GetRecoveryPath()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OpenExamSuite",
            "CreatorRecovery");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var name = string.IsNullOrEmpty(FilePath)
            ? $"Untitled-{Guid.NewGuid():N}.recovery.oef"
            : Path.GetFileNameWithoutExtension(FilePath) + ".recovery.oef";

        return Path.Combine(dir, name);
    }

    private void OnDocumentChanged()
    {
        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(HasProblems));
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var file = string.IsNullOrEmpty(FilePath) ? "Untitled" : Path.GetFileName(FilePath);
        var dirty = IsDirty ? "*" : string.Empty;
        Title = $"{Strings.Get("AppName")} - {file}{dirty}";
    }
}
