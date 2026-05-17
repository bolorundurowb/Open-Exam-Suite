using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Logging;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.ViewModels.Items;
using OpenExamSuite.Simulator.Views;
using OpenExamSuite.Simulator.Views.Dialogs;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Simulator.ViewModels.Routes;

/// <summary>
/// Simulator's home route — replaces the WinForms <c>HomeUi</c> exam grid +
/// Start/Properties/Remove/Add buttons.
/// </summary>
public sealed partial class HomeViewModel : ObservableObject, IRouteViewModel
{
    private readonly IServiceProvider _sp;
    private readonly IAppSettingsService _settings;
    private readonly IFilePickerService _files;
    private readonly IMessageBoxService _msg;
    private readonly INavigationService _nav;

    public ObservableCollection<ExamRowViewModel> Exams { get; } = new();
    public ObservableCollection<ExamRowViewModel> SelectedExams { get; } = new();

    public bool HasSingleSelection => SelectedExams.Count == 1;
    public bool HasAnySelection    => SelectedExams.Count >= 1;

    public HomeViewModel(
        IServiceProvider sp,
        IAppSettingsService settings,
        IFilePickerService files,
        IMessageBoxService msg,
        INavigationService nav)
    {
        _sp = sp;
        _settings = settings;
        _files = files;
        _msg = msg;
        _nav = nav;

        SelectedExams.CollectionChanged += (_, _) =>
        {
            StartCommand.NotifyCanExecuteChanged();
            PropertiesCommand.NotifyCanExecuteChanged();
            RemoveCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(HasSingleSelection));
            OnPropertyChanged(nameof(HasAnySelection));
        };

        LoadExams();
    }

    private void LoadExams()
    {
        // First-run sample loading, replacing the WinForms AppDataManager.
        // The WinForms code used ConfigurationManager-style Settings.Default.FirstRun;
        // here we approximate by checking if the simulator history is empty AND
        // sample files exist next to the EXE.
        try
        {
            var all = _settings.GetAll(AppSettingsType.Simulator);
            if (all.Count == 0)
            {
                var root = AppContext.BaseDirectory;
                var samples = Path.Combine(root, "Samples");
                var gmat = Path.Combine(samples, "GMAT Sample.oef");
                var basic = Path.Combine(samples, "Basic Science.oef");
                if (File.Exists(gmat))
                    _settings.Add(new AppSetting { Name = Path.GetFileNameWithoutExtension(gmat), FilePath = gmat }, AppSettingsType.Simulator);
                if (File.Exists(basic))
                    _settings.Add(new AppSetting { Name = Path.GetFileNameWithoutExtension(basic), FilePath = basic }, AppSettingsType.Simulator);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }

        Exams.Clear();
        foreach (var s in _settings.GetAll(AppSettingsType.Simulator))
            Exams.Add(new ExamRowViewModel(s.Name, s.FilePath));
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        var paths = await _files.PickOpenFilesAsync(
            "Add Exam(s)",
            new[] { new FileFilter("Open Exam Files", "*.oef") });

        foreach (var p in paths)
        {
            if (Exams.Any(e => string.Equals(e.FilePath, p, StringComparison.OrdinalIgnoreCase)))
                continue;

            var name = Path.GetFileNameWithoutExtension(p);
            _settings.Add(new AppSetting { Name = name, FilePath = p }, AppSettingsType.Simulator);
            Exams.Add(new ExamRowViewModel(name, p));
        }
    }

    [RelayCommand(CanExecute = nameof(HasAnySelection))]
    private void Remove()
    {
        var copy = SelectedExams.ToList();
        foreach (var row in copy)
        {
            _settings.Remove(row.FilePath, AppSettingsType.Simulator);
            Exams.Remove(row);
        }
        SelectedExams.Clear();
    }

    [RelayCommand(CanExecute = nameof(HasSingleSelection))]
    private async Task PropertiesAsync()
    {
        var row = SelectedExams.FirstOrDefault();
        if (row is null) return;

        var exam = LoadExamOrReportError(row);
        if (exam is null) return;

        var dlg = new ExamPropertiesDialog(exam, row.FilePath);
        await dlg.ShowDialog(MainWindow.Current!);
    }

    [RelayCommand(CanExecute = nameof(HasSingleSelection))]
    private void Start()
    {
        var row = SelectedExams.FirstOrDefault();
        if (row is null) return;

        var exam = LoadExamOrReportError(row);
        if (exam is null) return;

        var settingsVm = new ExamSettingsViewModel(exam, _sp, _nav, _msg);
        _nav.GoTo(settingsVm);
    }

    private OpenExamSuite.Shared.Exam? LoadExamOrReportError(ExamRowViewModel row)
    {
        try
        {
            var exam = Reader.FromOefFile(row.FilePath);
            if (exam is null)
            {
                _ = _msg.ShowAsync(
                    "Sorry, the exam selected is either old or corrupt. If it is an old exam, please upgrade it with the upgrade tool at:\nhttps://sourceforge.net/projects/exam-upgrade-tool/",
                    "Error", MessageBoxButtons.Ok, MessageBoxIconKind.Error);
                RemoveRow(row);
                return null;
            }
            return exam;
        }
        catch (FileNotFoundException ex)
        {
            Logger.LogException(ex);
            _ = _msg.ShowAsync(
                "Sorry, the selected exam does not exist. It may have been moved or deleted.",
                "Error", MessageBoxButtons.Ok, MessageBoxIconKind.Error);
            RemoveRow(row);
            return null;
        }
        catch (NullReferenceException ex)
        {
            Logger.LogException(ex);
            _ = _msg.ShowAsync(
                "Sorry, the exam selected is either old or corrupt. If it is an old exam, please upgrade it with the upgrade tool at:\nhttps://sourceforge.net/projects/exam-upgrade-tool/",
                "Error", MessageBoxButtons.Ok, MessageBoxIconKind.Error);
            RemoveRow(row);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            _ = _msg.ShowAsync(
                "An unexpected error occurred while opening the exam.",
                "Error", MessageBoxButtons.Ok, MessageBoxIconKind.Error);
            return null;
        }
    }

    private void RemoveRow(ExamRowViewModel row)
    {
        _settings.Remove(row.FilePath, AppSettingsType.Simulator);
        Exams.Remove(row);
        SelectedExams.Remove(row);
    }
}
