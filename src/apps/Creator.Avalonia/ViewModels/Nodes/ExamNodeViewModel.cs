using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels.Nodes;

/// <summary>Root node — equivalent to <c>ExamNode</c> in the WinForms tree.</summary>
public sealed partial class ExamNodeViewModel : NodeViewModel
{
    [ObservableProperty] private Properties _properties;

    public ExamNodeViewModel(Properties properties)
    {
        _properties = properties;
        DisplayName = string.IsNullOrEmpty(properties.Title) ? "Exam" : properties.Title;
        IsExpanded = true;
    }

    partial void OnPropertiesChanged(Properties value)
    {
        DisplayName = string.IsNullOrEmpty(value.Title) ? "Exam" : value.Title;
    }
}
