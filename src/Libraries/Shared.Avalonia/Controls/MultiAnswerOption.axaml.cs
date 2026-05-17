using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Shared.Avalonia.Controls;

/// <summary>
/// Editable option control for the multiple-answer case. Mirrors the WinForms
/// <c>OptionsControl</c> (CheckBox + TextBox).
/// </summary>
public partial class MultiAnswerOption : UserControl
{
    public static readonly StyledProperty<string?> LetterProperty =
        AvaloniaProperty.Register<MultiAnswerOption, string?>(nameof(Letter));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<MultiAnswerOption, string?>(nameof(Text), defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<MultiAnswerOption, bool>(nameof(IsChecked), defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay);

    public string? Letter
    {
        get => GetValue(LetterProperty);
        set => SetValue(LetterProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public MultiAnswerOption()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
