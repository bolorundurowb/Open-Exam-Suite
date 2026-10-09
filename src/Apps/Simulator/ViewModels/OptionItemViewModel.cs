using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Simulator.Localization;

namespace OpenExamSuite.Simulator.ViewModels;

public enum OptionMark
{
    None,
    YourAnswer,
    YourAnswerCorrect,
    YourAnswerWrong,
    CorrectAnswer
}

/// <summary>
/// One answer option. Correct, wrong and missed are always shown with an icon and a label as well as colour.
/// </summary>
public sealed partial class OptionItemViewModel : ObservableObject
{
    public OptionItemViewModel(char letter, string text, bool isMultiple)
    {
        Letter = letter;
        Text = text;
        IsMultiple = isMultiple;
    }

    /// <summary>Raised when the control changes <see cref="IsSelected"/>. The owner decides whether to accept it.</summary>
    public Action<OptionItemViewModel>? Changed { get; set; }

    public char Letter { get; }

    public string LetterText => $"{Letter}.";

    public string Text { get; }

    public bool IsMultiple { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    private bool _isSelected;

    partial void OnIsSelectedChanged(bool value) => Changed?.Invoke(this);

    /// <summary>Re-pushes the current value to the bound control after a rejected change.</summary>
    public void RefreshSelection() => OnPropertyChanged(nameof(IsSelected));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMark))]
    [NotifyPropertyChangedFor(nameof(IsCorrectMark))]
    [NotifyPropertyChangedFor(nameof(IsWrongMark))]
    [NotifyPropertyChangedFor(nameof(IsNeutralMark))]
    [NotifyPropertyChangedFor(nameof(MarkLabel))]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    private OptionMark _mark;

    public bool HasMark => Mark != OptionMark.None;

    public bool IsCorrectMark => Mark is OptionMark.YourAnswerCorrect or OptionMark.CorrectAnswer;

    public bool IsWrongMark => Mark == OptionMark.YourAnswerWrong;

    public bool IsNeutralMark => Mark == OptionMark.YourAnswer;

    public string MarkLabel => Mark switch
    {
        OptionMark.YourAnswer => Strings.Get("Option_YourAnswer"),
        OptionMark.YourAnswerCorrect => Strings.Get("Option_YourAnswerCorrect"),
        OptionMark.YourAnswerWrong => Strings.Get("Option_YourAnswerWrong"),
        OptionMark.CorrectAnswer => Strings.Get("Option_CorrectAnswer"),
        _ => string.Empty
    };

    /// <summary>The spoken name: letter, text, then selection and result state.</summary>
    public string AutomationName
    {
        get
        {
            var name = $"{Letter}. {Text}";
            if (IsSelected)
                name += ", " + Strings.Get("Option_Selected");
            if (HasMark)
                name += ", " + MarkLabel;
            return name;
        }
    }
}
