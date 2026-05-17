using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenExamSuite.Creator.ViewModels;

/// <summary>
/// Editable row for a single answer option in the question editor. The same
/// VM is used for the multi-answer and single-answer cases; the view picks the
/// appropriate UserControl via DataTemplate.
/// </summary>
public sealed partial class OptionRowViewModel : ObservableObject
{
    [ObservableProperty] private char _letter;
    [ObservableProperty] private string _text = string.Empty;
    [ObservableProperty] private bool _isChecked;
    [ObservableProperty] private bool _isMultipleChoice;

    public string LetterDisplay => Letter.ToString();
}
