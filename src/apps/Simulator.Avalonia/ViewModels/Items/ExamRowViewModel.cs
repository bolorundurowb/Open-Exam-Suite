using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenExamSuite.Simulator.ViewModels.Items;

public sealed partial class ExamRowViewModel : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _filePath = string.Empty;

    public ExamRowViewModel(string name, string filePath)
    {
        _name = name;
        _filePath = filePath;
    }
}
