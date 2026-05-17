using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenExamSuite.Simulator.ViewModels.Items;
using OpenExamSuite.Simulator.ViewModels.Routes;

namespace OpenExamSuite.Simulator.Views.Routes;

public partial class HomeView : UserControl
{
    public HomeView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    /// <summary>
    /// Mirrors the WinForms <c>SelectionChanged</c> on <c>dgv_exams</c> —
    /// rather than toggling button-enabled flags in code, we keep the VM's
    /// <c>SelectedExams</c> collection in sync; the buttons' CanExecute does
    /// the rest.
    /// </summary>
    private void OnGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not HomeViewModel vm) return;
        if (sender is not DataGrid grid) return;

        if (e.RemovedItems is not null)
            foreach (var removed in e.RemovedItems)
                if (removed is ExamRowViewModel r)
                    vm.SelectedExams.Remove(r);

        if (e.AddedItems is not null)
            foreach (var added in e.AddedItems)
                if (added is ExamRowViewModel r && !vm.SelectedExams.Contains(r))
                    vm.SelectedExams.Add(r);
    }

    /// <summary>Mirrors the WinForms <c>CellDoubleClick → Start</c> wiring.</summary>
    private void OnGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not HomeViewModel vm) return;
        if (vm.StartCommand.CanExecute(null))
            vm.StartCommand.Execute(null);
    }
}
