using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.ViewModels.Nodes;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels.Panes;

/// <summary>
/// Replaces the WinForms <c>pan_exam_properties</c> panel: editable Title,
/// Code, Pass Mark, Time Limit, Instructions, Hide Answers + a Save button.
/// </summary>
public sealed partial class ExamPropertiesPaneViewModel : ObservableObject, IRightPaneViewModel
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _code = string.Empty;
    [ObservableProperty] private decimal _passmark;
    [ObservableProperty] private decimal _timeLimit;
    [ObservableProperty] private string _instructions = string.Empty;
    [ObservableProperty] private bool _hideAnswers;
    [ObservableProperty] private int _version = 4;

    public string VersionDisplay => $"v{Version}";

    public event EventHandler<Properties>? Saved;

    public ExamPropertiesPaneViewModel Initialize(ExamNodeViewModel node)
    {
        var p = node.Properties;
        Title = p.Title;
        Code = p.Code;
        Passmark = (decimal)p.Passmark;
        TimeLimit = p.TimeLimit;
        Instructions = p.Instructions;
        HideAnswers = p.HideAnswers;
        Version = p.Version == 0 ? 4 : p.Version;
        return this;
    }

    public ExamPropertiesPaneViewModel InitializeNew()
    {
        Title = string.Empty;
        Code = string.Empty;
        Passmark = 0;
        TimeLimit = 0;
        Instructions = string.Empty;
        HideAnswers = false;
        Version = 4;
        return this;
    }

    [RelayCommand]
    private void Save()
    {
        var p = new Properties
        {
            Title = Title,
            Code = Code,
            Passmark = (double)Passmark,
            TimeLimit = (int)TimeLimit,
            Instructions = Instructions,
            HideAnswers = HideAnswers,
            Version = 4,
        };

        Saved?.Invoke(this, p);
    }
}
