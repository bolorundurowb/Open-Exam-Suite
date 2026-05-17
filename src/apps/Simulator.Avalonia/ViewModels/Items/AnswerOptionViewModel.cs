using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenExamSuite.Simulator.ViewModels.Items;

public enum AnswerOptionState
{
    Neutral,
    Correct,
    Incorrect,
}

public sealed partial class AnswerOptionViewModel : ObservableObject
{
    [ObservableProperty] private char _letter;
    [ObservableProperty] private string _text = string.Empty;
    [ObservableProperty] private bool _isChecked;
    [ObservableProperty] private bool _isMultipleChoice;
    [ObservableProperty] private AnswerOptionState _state = AnswerOptionState.Neutral;

    /// <summary>
    /// Replaces the WinForms <c>ForeColor = Color.Green/Red/Black</c> trick
    /// used by <c>HighlightCorrectAndIncorrectAnswers</c>.
    /// </summary>
    public IBrush Foreground => State switch
    {
        AnswerOptionState.Correct   => Brushes.Green,
        AnswerOptionState.Incorrect => Brushes.Red,
        _                           => Brushes.Black,
    };

    partial void OnStateChanged(AnswerOptionState value)
    {
        OnPropertyChanged(nameof(Foreground));
    }

    public string Display => $"{Letter}. - {Text}";
}
