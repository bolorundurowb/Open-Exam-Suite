using System.Diagnostics;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Finds the Creator executable. Edit and New exam are offered only when it is installed.
/// Installed layout: <c>{app}/Simulator</c> and <c>{app}/Creator</c> are siblings.
/// </summary>
public sealed class CreatorLocator
{
    public const string OverrideEnvironmentVariable = "OES_CREATOR_PATH";

    private readonly string _applicationDirectory;
    private readonly Lazy<string?> _executable;

    public CreatorLocator(string? applicationDirectory = null)
    {
        _applicationDirectory = applicationDirectory ?? AppContext.BaseDirectory;
        _executable = new Lazy<string?>(Resolve);
    }

    public string? ExecutablePath => _executable.Value;

    public bool IsInstalled => ExecutablePath != null;

    /// <summary>Starts Creator, optionally opening <paramref name="examPath"/>. Returns false when Creator is missing or cannot start.</summary>
    public bool Launch(string? examPath = null)
    {
        var executable = ExecutablePath;
        if (executable == null)
            return false;

        try
        {
            var info = new ProcessStartInfo(executable) { UseShellExecute = false };
            if (!string.IsNullOrEmpty(examPath))
                info.ArgumentList.Add(examPath);

            Process.Start(info);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    internal static IEnumerable<string> Candidates(string applicationDirectory)
    {
        var name = OperatingSystem.IsWindows() ? "OpenExamSuite.Creator.exe" : "OpenExamSuite.Creator";
        var trimmed = applicationDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // Windows/Linux installed layout and portable zip.
        yield return Path.GetFullPath(Path.Combine(trimmed, "..", "Creator", name));
        // macOS bundle and flat layouts keep both executables in one folder.
        yield return Path.Combine(trimmed, name);
    }

    private string? Resolve()
    {
        var overridePath = Environment.GetEnvironmentVariable(OverrideEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
            return overridePath;

        return Candidates(_applicationDirectory).FirstOrDefault(File.Exists);
    }
}
