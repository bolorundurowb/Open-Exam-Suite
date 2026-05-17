using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace OpenExamSuite.Shared.Avalonia.Services;

/// <summary>
/// Replacement for the WinForms <see cref="System.Diagnostics.Process.Start(System.Diagnostics.ProcessStartInfo)"/>
/// pattern used by the About box's <see cref="System.Windows.Forms.LinkLabel"/> click handlers.
/// </summary>
public sealed class OpenUrlService : IOpenUrlService
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

    public Task<bool> OpenUriAsync(Uri uri)
    {
        var top = GetTopLevel();
        if (top is null) return Task.FromResult(false);
        return top.Launcher.LaunchUriAsync(uri);
    }

    public Task<bool> OpenUriAsync(string uri)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsed))
            return Task.FromResult(false);
        return OpenUriAsync(parsed);
    }

    public async Task<bool> OpenFileAsync(string localPath)
    {
        var top = GetTopLevel();
        if (top is null) return false;

        var storageFile = await top.StorageProvider.TryGetFileFromPathAsync(localPath);
        if (storageFile is null) return false;

        return await top.Launcher.LaunchFileAsync(storageFile);
    }
}
