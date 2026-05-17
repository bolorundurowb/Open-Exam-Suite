using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class EditSectionDialog : Window
{
    public EditSectionDialog() : this(string.Empty) { }

    public EditSectionDialog(string currentTitle)
    {
        InitializeComponent();
        Opened += (_, _) =>
        {
            var tb = this.FindControl<TextBox>("TitleBox");
            if (tb is null) return;
            tb.Text = currentTitle;
            tb.SelectAll();
            tb.Focus();
        };
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        var tb = this.FindControl<TextBox>("TitleBox");
        Close(tb?.Text);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(null);
}
