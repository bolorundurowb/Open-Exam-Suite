using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class AddSectionDialog : Window
{
    public AddSectionDialog()
    {
        InitializeComponent();
        Opened += (_, _) => this.FindControl<TextBox>("TitleBox")?.Focus();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnAdd(object? sender, RoutedEventArgs e)
    {
        var tb = this.FindControl<TextBox>("TitleBox");
        Close(tb?.Text);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(null);
}
