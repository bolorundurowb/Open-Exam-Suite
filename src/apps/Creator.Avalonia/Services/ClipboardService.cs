using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace OpenExamSuite.Creator.Services;

public sealed class ClipboardService : IClipboardService
{
    private static TopLevel? GetTopLevel()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is
                IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow is not null)
            return TopLevel.GetTopLevel(desktop.MainWindow);
        return null;
    }

    public async Task<string?> GetTextAsync()
    {
        var top = GetTopLevel();
        if (top?.Clipboard is null) return null;
        return await top.Clipboard.GetTextAsync();
    }

    public async Task SetTextAsync(string text)
    {
        var top = GetTopLevel();
        if (top?.Clipboard is null) return;
        await top.Clipboard.SetTextAsync(text);
    }
}
