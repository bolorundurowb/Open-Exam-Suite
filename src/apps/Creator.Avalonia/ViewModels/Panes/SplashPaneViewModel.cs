using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Shared.Avalonia.Services;

namespace OpenExamSuite.Creator.ViewModels.Panes;

/// <summary>
/// Replaces the WinForms <c>pan_splash</c> shown when no exam is loaded:
/// banner + Exam History group with recent file links and a Clear History link.
/// </summary>
public sealed partial class SplashPaneViewModel : ObservableObject, IRightPaneViewModel
{
    private readonly IExamHistoryService _history;
    private readonly IMessageBoxService _msg;

    /// <summary>Invoked when the user clicks a history link. Wired by the parent VM.</summary>
    public Func<string, Task>? OpenRequested { get; set; }

    public ObservableCollection<ExamHistoryEntryViewModel> History { get; } = new();

    public SplashPaneViewModel(IExamHistoryService history, IMessageBoxService msg)
    {
        _history = history;
        _msg = msg;
        ReloadHistory();
    }

    public void ReloadHistory()
    {
        History.Clear();
        foreach (var path in _history.GetAll())
            History.Add(new ExamHistoryEntryViewModel(path));
    }

    [RelayCommand]
    private async Task OpenAsync(ExamHistoryEntryViewModel? entry)
    {
        if (entry is null) return;
        if (!System.IO.File.Exists(entry.FilePath))
        {
            await _msg.ShowAsync(
                "Sorry, the selected file has been moved or deleted.",
                "Access error",
                MessageBoxButtons.Ok, MessageBoxIconKind.Warning);
            _history.Remove(entry.FilePath);
            History.Remove(entry);
            return;
        }

        if (OpenRequested is not null)
            await OpenRequested(entry.FilePath);
    }

    [RelayCommand]
    private void ClearHistory()
    {
        _history.Clear();
        History.Clear();
    }
}
