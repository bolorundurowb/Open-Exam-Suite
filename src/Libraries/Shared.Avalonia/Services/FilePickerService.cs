using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace OpenExamSuite.Shared.Avalonia.Services;

/// <summary>
/// Avalonia <see cref="IStorageProvider"/>-backed file picker, replacing
/// <see cref="System.Windows.Forms.OpenFileDialog"/> / <c>SaveFileDialog</c>.
/// </summary>
public sealed class FilePickerService : IFilePickerService
{
    private static TopLevel? GetTopLevel()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is
                IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow is not null)
        {
            return TopLevel.GetTopLevel(desktop.MainWindow);
        }
        return null;
    }

    private static IReadOnlyList<FilePickerFileType> Map(IReadOnlyList<FileFilter> filters)
        => filters
            .Select(f => new FilePickerFileType(f.Display) { Patterns = f.Patterns.ToList() })
            .ToList();

    public async Task<string?> PickOpenFileAsync(
        string title, IReadOnlyList<FileFilter> filters, string? suggestedStartFolder = null)
    {
        var top = GetTopLevel();
        if (top is null) return null;

        IStorageFolder? start = null;
        if (!string.IsNullOrEmpty(suggestedStartFolder))
            start = await top.StorageProvider.TryGetFolderFromPathAsync(suggestedStartFolder);

        var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = Map(filters),
            SuggestedStartLocation = start,
        });

        return files.Count == 0 ? null : files[0].TryGetLocalPath();
    }

    public async Task<IReadOnlyList<string>> PickOpenFilesAsync(
        string title, IReadOnlyList<FileFilter> filters, string? suggestedStartFolder = null)
    {
        var top = GetTopLevel();
        if (top is null) return Array.Empty<string>();

        IStorageFolder? start = null;
        if (!string.IsNullOrEmpty(suggestedStartFolder))
            start = await top.StorageProvider.TryGetFolderFromPathAsync(suggestedStartFolder);

        var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
            FileTypeFilter = Map(filters),
            SuggestedStartLocation = start,
        });

        return files
            .Select(f => f.TryGetLocalPath())
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(p => p!)
            .ToList();
    }

    public async Task<string?> PickSaveFileAsync(
        string title, IReadOnlyList<FileFilter> filters,
        string? suggestedFileName = null, string? suggestedStartFolder = null)
    {
        var top = GetTopLevel();
        if (top is null) return null;

        IStorageFolder? start = null;
        if (!string.IsNullOrEmpty(suggestedStartFolder))
            start = await top.StorageProvider.TryGetFolderFromPathAsync(suggestedStartFolder);

        var defaultExt = filters
            .SelectMany(f => f.Patterns)
            .Select(p => p.TrimStart('*'))
            .FirstOrDefault(p => p.StartsWith('.'));

        var file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            FileTypeChoices = Map(filters),
            SuggestedFileName = suggestedFileName,
            SuggestedStartLocation = start,
            DefaultExtension = defaultExt?.TrimStart('.'),
            ShowOverwritePrompt = true,
        });

        return file?.TryGetLocalPath();
    }
}
