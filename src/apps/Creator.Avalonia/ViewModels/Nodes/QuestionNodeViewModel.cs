using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels.Nodes;

/// <summary>Question node — equivalent to <c>QuestionNode</c> in the WinForms tree.</summary>
public sealed partial class QuestionNodeViewModel : NodeViewModel
{
    [ObservableProperty] private Question _question;

    public QuestionNodeViewModel(Question question)
    {
        _question = question;
        DisplayName = $"Question {question.No}";
    }

    partial void OnQuestionChanged(Question value)
    {
        DisplayName = $"Question {value.No}";
    }
}
