using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
    private readonly ILogger<CreatorLocator> _logger;

    public CreatorLocator(string? applicationDirectory = null, ILogger<CreatorLocator>? logger = null)
    {
        _applicationDirectory = applicationDirectory ?? AppContext.BaseDirectory;
        _executable = new Lazy<string?>(Resolve);
        _logger = logger ?? NullLogger<CreatorLocator>.Instance;
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
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not start Creator at '{Executable}'.", executable);
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
        // macOS: Simulator and Creator are sibling .app bundles. From Contents/MacOS this
        // resolves to <install>/Open Exam Suite Creator.app/Contents/MacOS/<name>.
        yield return Path.GetFullPath(Path.Combine(
            trimmed, "..", "..", "..", "Open Exam Suite Creator.app", "Contents", "MacOS", name));
    }

    private string? Resolve()
    {
        var overridePath = Environment.GetEnvironmentVariable(OverrideEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
            return overridePath;

        return Candidates(_applicationDirectory).FirstOrDefault(File.Exists);
    }
}
