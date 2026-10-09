using System.Diagnostics;
using OpenExamSuite.Simulator.Session.HostPorts;

namespace OpenExamSuite.Simulator.Services;

public sealed class ShellUriLauncher : IUriLauncher
{
    public Task<bool> LaunchUriAsync(string uri) => Task.FromResult(Start(uri));

    public Task<bool> OpenFileAsync(string filePath) => Task.FromResult(Start(filePath));

    public Task<bool> ShowInFolderAsync(string filePath)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo("explorer.exe") { Arguments = $"/select,\"{filePath}\"" });
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start(new ProcessStartInfo("open") { ArgumentList = { "-R", filePath } });
            }
            else
            {
                var folder = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(folder))
                    return Task.FromResult(false);

                Process.Start(new ProcessStartInfo("xdg-open") { ArgumentList = { folder } });
            }

            return Task.FromResult(true);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }

    private static bool Start(string target)
    {
        try
        {
            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
