using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class ExamPropertiesViewModel : ViewModelBase
{
    private readonly ShellServices _shell;

    public ExamPropertiesViewModel(ShellServices shell)
    {
        _shell = shell;
        var props = _shell.Document.Exam.Properties;
        _title = props.Title;
        _code = props.Code;
        _instructions = props.Instructions;
        _passMark = props.Passmark;
        _timeLimit = props.TimeLimit;
        _hideAnswers = props.HideAnswers;
    }

    [ObservableProperty] private string _title;
    [ObservableProperty] private string _code;
    [ObservableProperty] private string _instructions;
    [ObservableProperty] private double _passMark;
    [ObservableProperty] private int _timeLimit;
    [ObservableProperty] private bool _hideAnswers;

    public int ScaledPassMark => (int)PassMark * 10;

    partial void OnTitleChanged(string value) => Commit();
    partial void OnCodeChanged(string value) => Commit();
    partial void OnInstructionsChanged(string value) => Commit();
    partial void OnPassMarkChanged(double value) => Commit();
    partial void OnTimeLimitChanged(int value) => Commit();
    partial void OnHideAnswersChanged(bool value) => Commit();

    private void Commit()
    {
        _shell.Document.UpdateExamProperties(new Properties
        {
            Title = Title,
            Code = Code,
            Instructions = Instructions,
            Passmark = PassMark,
            TimeLimit = TimeLimit,
            HideAnswers = HideAnswers,
            Version = _shell.Document.Exam.Properties.Version
        });
    }
}
