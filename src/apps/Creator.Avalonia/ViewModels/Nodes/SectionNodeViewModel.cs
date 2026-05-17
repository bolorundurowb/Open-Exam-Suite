using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenExamSuite.Creator.ViewModels.Nodes;

/// <summary>Section node — equivalent to <c>SectionNode</c> in the WinForms tree.</summary>
public sealed partial class SectionNodeViewModel : NodeViewModel
{
    [ObservableProperty] private string _title;

    public SectionNodeViewModel(string title)
    {
        _title = title;
        DisplayName = title;
        IsExpanded = true;
    }

    partial void OnTitleChanged(string value)
    {
        DisplayName = value;
    }

    public void RenumberQuestions()
    {
        var i = 1;
        foreach (var child in Children)
        {
            if (child is QuestionNodeViewModel q)
            {
                q.Question.No = i;
                q.DisplayName = $"Question {i}";
                i++;
            }
        }
    }
}
