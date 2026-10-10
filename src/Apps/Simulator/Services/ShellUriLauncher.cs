using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenExamSuite.Simulator.Session.HostPorts;

namespace OpenExamSuite.Simulator.Services;

public sealed class ShellUriLauncher : IUriLauncher
{
    /// <summary>
    /// Extensions that run code or a script. Exam content may supply a path, so the shell must never
    /// be handed one of these.
    /// </summary>
    private static readonly HashSet<string> ExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".app", ".application", ".bat", ".bin", ".bash", ".cmd", ".com", ".command", ".cpl", ".deb",
        ".desktop", ".dmg", ".exe", ".gadget", ".hta", ".inf", ".jar", ".js", ".jse", ".lnk", ".msc",
        ".msi", ".msp", ".pif", ".pkg", ".ps1", ".psd1", ".psm1", ".reg", ".rpm", ".run", ".scr",
        ".sh", ".vbe", ".vbs", ".wsf", ".wsh", ".zsh"
    };

    private readonly ILogger<ShellUriLauncher> _logger;

    public ShellUriLauncher(ILogger<ShellUriLauncher>? logger = null) =>
        _logger = logger ?? NullLogger<ShellUriLauncher>.Instance;

    public Task<bool> LaunchUriAsync(string uri)
    {
        if (!TryGetWebUri(uri, out var safeUri))
        {
            _logger.LogWarning("Refused to launch URI '{Uri}' because it is not an http(s) URL.", uri);
            return Task.FromResult(false);
        }

        return Task.FromResult(Start(safeUri.AbsoluteUri, $"URI '{safeUri}'"));
    }

    public Task<bool> OpenFileAsync(string filePath)
    {
        if (IsExecutable(filePath))
        {
            _logger.LogWarning("Refused to open '{FilePath}' because its extension is executable.", filePath);
            return Task.FromResult(false);
        }

        return Task.FromResult(Start(filePath, $"file '{filePath}'"));
    }

    public Task<bool> ShowInFolderAsync(string filePath)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                // ArgumentList quotes the path, so a file name containing a quote cannot inject switches.
                var info = new ProcessStartInfo("explorer.exe");
                info.ArgumentList.Add($"/select,{filePath}");
                Process.Start(info);
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start(new ProcessStartInfo("open") { ArgumentList = { "-R", filePath } });
            }
            else
            {
                var folder = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(folder))
                {
                    _logger.LogWarning("Could not resolve the parent folder of '{FilePath}'.", filePath);
                    return Task.FromResult(false);
                }

                Process.Start(new ProcessStartInfo("xdg-open") { ArgumentList = { folder } });
            }

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not show '{FilePath}' in the file manager.", filePath);
            return Task.FromResult(false);
        }
    }

    /// <summary>Accepts only absolute http(s) URLs with a host. Anything else could open a file or a protocol handler.</summary>
    private static bool TryGetWebUri(string uri, out Uri safeUri)
    {
        safeUri = null!;
        if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsed)
            || (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrEmpty(parsed.Host))
        {
            return false;
        }

        safeUri = parsed;
        return true;
    }

    private static bool IsExecutable(string filePath) =>
        !string.IsNullOrWhiteSpace(filePath)
        && ExecutableExtensions.Contains(Path.GetExtension(filePath));

    private bool Start(string target, string description)
    {
        try
        {
            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not launch {Description}.", description);
            return false;
        }
    }
}
