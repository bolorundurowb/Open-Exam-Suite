using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenExamSuite.Creator.ViewModels.Panes;

public sealed partial class ExamHistoryEntryViewModel : ObservableObject
{
    [ObservableProperty] private string _filePath = string.Empty;

    public string DisplayName => string.IsNullOrEmpty(FilePath)
        ? string.Empty
        : Path.GetFileNameWithoutExtension(FilePath);

    public ExamHistoryEntryViewModel(string filePath)
    {
        _filePath = filePath;
    }
}
