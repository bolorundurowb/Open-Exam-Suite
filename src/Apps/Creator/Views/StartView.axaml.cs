using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using OpenExamSuite.Creator.ViewModels;

namespace OpenExamSuite.Creator.Views;

public partial class StartView : UserControl
{
    public StartView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (DataContext is not StartViewModel viewModel)
            return;

        var hasFiles = e.DataTransfer.Contains(DataFormat.File);
        e.DragEffects = hasFiles ? DragDropEffects.Copy : DragDropEffects.None;
        viewModel.IsDropTarget = hasFiles;
        e.Handled = true;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        if (DataContext is StartViewModel viewModel)
            viewModel.IsDropTarget = false;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not StartViewModel viewModel)
            return;

        viewModel.IsDropTarget = false;

        var paths = e.DataTransfer.TryGetFiles()?
            .Select(f => f.TryGetLocalPath())
            .OfType<string>()
            .ToList();

        if (paths is { Count: > 0 })
            await viewModel.DropFilesAsync(paths);

        e.Handled = true;
    }
}
