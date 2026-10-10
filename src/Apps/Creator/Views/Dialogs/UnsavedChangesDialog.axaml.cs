using Avalonia.Controls;
using OpenExamSuite.Creator.Services;

namespace OpenExamSuite.Creator.Views.Dialogs;

public partial class UnsavedChangesDialog : Window
{
    private PromptResult _result = PromptResult.Cancel;

    public UnsavedChangesDialog()
    {
        InitializeComponent();
    }

    public async Task<PromptResult> ShowAsync(Window owner)
    {
        await ShowDialog(owner);
        return _result;
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _result = PromptResult.Save;
        Close();
    }

    private void DontSave_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _result = PromptResult.DontSave;
        Close();
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _result = PromptResult.Cancel;
        Close();
    }
}
