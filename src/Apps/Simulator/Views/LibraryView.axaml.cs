using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using OpenExamSuite.Simulator.ViewModels;

namespace OpenExamSuite.Simulator.Views;

public partial class LibraryView : UserControl
{
    public LibraryView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DoubleTappedEvent, OnDoubleTapped);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        var hasFiles = e.Data.Contains(DataFormats.Files);
        e.DragEffects = hasFiles ? DragDropEffects.Copy : DragDropEffects.None;
        DropOverlay.IsVisible = hasFiles;
        e.Handled = true;
    }

    private void OnDragLeave(object? sender, DragEventArgs e) => DropOverlay.IsVisible = false;

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        DropOverlay.IsVisible = false;
        if (DataContext is not LibraryViewModel library)
            return;

        var paths = e.Data.GetFiles()?
            .Select(f => f.TryGetLocalPath())
            .OfType<string>()
            .Where(MainWindowViewModel.IsSupportedPath)
            .ToList();
        if (paths is { Count: > 0 })
            await library.AddPathsAsync(paths);

        e.Handled = true;
    }

    /// <summary>Double-clicking a card opens the pre-exam sheet.</summary>
    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is not Visual source || source.FindAncestorOfType<Button>(includeSelf: true) != null)
            return;

        if ((e.Source as StyledElement)?.DataContext is ExamCardViewModel { IsAvailable: true } card)
            card.OpenCommand.Execute(null);
    }
}
