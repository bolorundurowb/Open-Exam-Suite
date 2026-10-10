using Avalonia.Controls;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class MessageDialog : Window
{
    private bool _result;

    public MessageDialog()
    {
        InitializeComponent();
    }

    public new async Task<bool> ShowDialog(Window owner)
    {
        await base.ShowDialog(owner);
        return _result;
    }

    private void Yes_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _result = true;
        Close();
    }

    private void No_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _result = false;
        Close();
    }
}
