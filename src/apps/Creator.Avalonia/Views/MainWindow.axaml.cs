using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenExamSuite.Creator.ViewModels;
using OpenExamSuite.Creator.ViewModels.Nodes;

namespace OpenExamSuite.Creator.Views;

public partial class MainWindow : Window
{
    public static MainWindow? Current { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Current = this;
        Closing += OnClosing;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    /// <summary>
    /// Replaces WinForms <c>TreeView.AfterSelect</c> by pushing the new
    /// selection into the ViewModel. The pane swap happens in the VM via
    /// <c>OnSelectedNodeChanged</c>.
    /// </summary>
    private void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        if (sender is not TreeView tree) return;
        vm.SelectedNode = tree.SelectedItem as NodeViewModel;
    }

    /// <summary>
    /// Returns the currently-focused <see cref="TextBox"/>, used by the
    /// Cut/Copy/Paste commands on <see cref="MainWindowViewModel"/>.
    /// </summary>
    public TextBox? GetFocusedTextBox()
    {
        return FocusManager?.GetFocusedElement() as TextBox;
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        if (!vm.IsDirty) return;

        // Cancel the close, ask, and if the user confirmed close again.
        e.Cancel = true;
        var ok = await vm.ConfirmDiscardChangesAsync();
        if (ok)
        {
            vm.IsDirty = false;
            Close();
        }
    }
}
