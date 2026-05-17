using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenExamSuite.Shared.Avalonia.Services;

public interface IFilePickerService
{
    /// <summary>Returns the picked file's local path, or null if cancelled.</summary>
    Task<string?> PickOpenFileAsync(
        string title,
        IReadOnlyList<FileFilter> filters,
        string? suggestedStartFolder = null);

    /// <summary>Returns the picked files' local paths, or empty if cancelled.</summary>
    Task<IReadOnlyList<string>> PickOpenFilesAsync(
        string title,
        IReadOnlyList<FileFilter> filters,
        string? suggestedStartFolder = null);

    /// <summary>Returns the destination file's local path, or null if cancelled.</summary>
    Task<string?> PickSaveFileAsync(
        string title,
        IReadOnlyList<FileFilter> filters,
        string? suggestedFileName = null,
        string? suggestedStartFolder = null);
}
