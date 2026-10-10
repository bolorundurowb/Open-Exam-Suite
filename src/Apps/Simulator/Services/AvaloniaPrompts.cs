using Avalonia.Platform.Storage;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Engine.HostPorts;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Host implementation of the session's prompt port: dialogs and the system file pickers.
/// Filters use the form <c>Description|*.ext;*.ext2</c>.
/// </summary>
public sealed class AvaloniaPrompts : IPrompts
{
    private readonly DialogService _dialogs;
    private readonly MainWindowAccessor _window;

    public AvaloniaPrompts(DialogService dialogs, MainWindowAccessor window)
    {
        _dialogs = dialogs;
        _window = window;
    }

    public async Task<PromptResult> ConfirmAsync(string title, string message, string yesText = "Yes", string noText = "No", string? cancelText = "Cancel")
    {
        var buttons = new List<DialogButton>
        {
            new(yesText, IsPrimary: true),
            new(noText)
        };
        if (cancelText != null)
            buttons.Add(new DialogButton(cancelText, IsCancel: true));

        var index = await _dialogs.ShowMessageAsync(title, message, buttons.ToArray());
        return index switch
        {
            0 => PromptResult.Yes,
            1 => PromptResult.No,
            _ => PromptResult.Cancel
        };
    }

    public Task InformAsync(string title, string message) => _dialogs.ShowInfoAsync(title, message);

    public Task ErrorAsync(string title, string message) => _dialogs.ShowInfoAsync(title, message);

    public Task<string?> InputAsync(string title, string prompt, string? defaultValue = null) =>
        _dialogs.PromptTextAsync(title, prompt, defaultValue);

    public async Task<string?> PickFileAsync(string title, string filter, string? initialDirectory = null)
    {
        if (_window.Window?.StorageProvider is not { } storage)
            return null;

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = ParseFilter(filter)
        };
        if (!string.IsNullOrEmpty(initialDirectory))
            options.SuggestedStartLocation = await storage.TryGetFolderFromPathAsync(initialDirectory);

        var files = await storage.OpenFilePickerAsync(options);
        return files.Count > 0 ? files[0].TryGetLocalPath() : null;
    }

    public async Task<string?> SaveFileAsync(string title, string filter, string? suggestedFileName = null, string? initialDirectory = null)
    {
        if (_window.Window?.StorageProvider is not { } storage)
            return null;

        var types = ParseFilter(filter);
        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedFileName,
            FileTypeChoices = types,
            ShowOverwritePrompt = true
        };
        if (!string.IsNullOrEmpty(initialDirectory))
            options.SuggestedStartLocation = await storage.TryGetFolderFromPathAsync(initialDirectory);

        var file = await storage.SaveFilePickerAsync(options);
        return file?.TryGetLocalPath();
    }

    public async Task<string?> PickFolderAsync(string title, string? initialDirectory = null)
    {
        if (_window.Window?.StorageProvider is not { } storage)
            return null;

        var options = new FolderPickerOpenOptions { Title = title, AllowMultiple = false };
        if (!string.IsNullOrEmpty(initialDirectory))
            options.SuggestedStartLocation = await storage.TryGetFolderFromPathAsync(initialDirectory);

        var folders = await storage.OpenFolderPickerAsync(options);
        return folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
    }

    internal static IReadOnlyList<FilePickerFileType> ParseFilter(string filter)
    {
        var types = new List<FilePickerFileType>();
        var parts = filter.Split('|');
        for (var i = 0; i + 1 < parts.Length; i += 2)
        {
            var patterns = parts[i + 1].Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            types.Add(new FilePickerFileType(parts[i]) { Patterns = patterns });
        }

        if (types.Count == 0)
            types.Add(FilePickerFileTypes.All);

        return types;
    }
}
