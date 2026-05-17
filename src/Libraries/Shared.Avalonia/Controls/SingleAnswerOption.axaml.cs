using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Shared.Avalonia.Controls;

/// <summary>
/// Editable option control for the single-answer case. Mirrors the WinForms
/// <c>OptionControl</c> (RadioButton + TextBox).
/// </summary>
public partial class SingleAnswerOption : UserControl
{
    public static readonly StyledProperty<string?> GroupNameProperty =
        AvaloniaProperty.Register<SingleAnswerOption, string?>(nameof(GroupName));

    public static readonly StyledProperty<string?> LetterProperty =
        AvaloniaProperty.Register<SingleAnswerOption, string?>(nameof(Letter));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SingleAnswerOption, string?>(nameof(Text), defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<SingleAnswerOption, bool>(nameof(IsChecked), defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay);

    public string? GroupName
    {
        get => GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }

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

    public SingleAnswerOption()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
