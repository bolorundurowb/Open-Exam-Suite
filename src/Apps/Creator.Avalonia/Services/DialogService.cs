using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using OpenExamSuite.Creator.Views.Dialogs;

namespace OpenExamSuite.Creator.Services;

public enum PromptResult
{
    Save,
    DontSave,
    Cancel
}

public sealed class DialogService
{
    private Window? Owner => Application.Current?.ApplicationLifetime
        is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
        ? desktop.MainWindow
        : null;

    public async Task<PromptResult> ConfirmUnsavedChangesAsync(string title)
    {
        var owner = Owner;
        if (owner == null)
            return PromptResult.Cancel;

        var dialog = new UnsavedChangesDialog { Title = title };
        return await dialog.ShowAsync(owner);
    }

    public async Task ShowAboutAsync()
    {
        var owner = Owner;
        if (owner == null)
            return;

        var dialog = new AboutDialog();
        await dialog.ShowDialog(owner);
    }

    public async Task ShowLicenseAsync()
    {
        var owner = Owner;
        if (owner == null)
            return;

        var dialog = new LicenseDialog();
        await dialog.ShowDialog(owner);
    }

    public async Task ShowChangelogAsync()
    {
        var owner = Owner;
        if (owner == null)
            return;

        var dialog = new ChangelogDialog();
        await dialog.ShowDialog(owner);
    }

    public async Task<bool> ConfirmRecoveryAsync(string recoveryPath)
    {
        var owner = Owner;
        if (owner == null)
            return false;

        var box = new MessageDialog
        {
            Title = "Recover unsaved work?",
            DataContext = new { Message = $"A newer autosaved copy exists. Recover it?{Environment.NewLine}{recoveryPath}" }
        };
        return await box.ShowDialog(owner);
    }

    public async Task<string?> PickOpenFileAsync(string title, params FilePickerFileType[] types)
    {
        var owner = Owner;
        if (owner == null)
            return null;

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = types.ToList()
        };

        var result = await owner.StorageProvider.OpenFilePickerAsync(options);
        return result.FirstOrDefault()?.TryGetLocalPath();
    }

    public async Task<string?> PickSaveFileAsync(string title, string suggestedName, params FilePickerFileType[] types)
    {
        var owner = Owner;
        if (owner == null)
            return null;

        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedName,
            FileTypeChoices = types.ToList(),
            DefaultExtension = types.FirstOrDefault()?.Patterns?.FirstOrDefault()
        };

        var result = await owner.StorageProvider.SaveFilePickerAsync(options);
        return result?.TryGetLocalPath();
    }

    public static FilePickerFileType OefType { get; } = new("Open Exam file")
    {
        Patterns = ["*.oef"],
        AppleUniformTypeIdentifiers = ["public.data"]
    };

    public static FilePickerFileType JsonType { get; } = new("JSON exam")
    {
        Patterns = ["*.json"],
        MimeTypes = ["application/json"]
    };

    public static FilePickerFileType XmlType { get; } = new("XML exam")
    {
        Patterns = ["*.xml"],
        MimeTypes = ["application/xml"]
    };

    public static FilePickerFileType PdfType { get; } = new("PDF document")
    {
        Patterns = ["*.pdf"],
        MimeTypes = ["application/pdf"]
    };

    public static string AppVersion =>
        Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "4.0";
}
