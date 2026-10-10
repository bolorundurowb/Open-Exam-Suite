using System.Diagnostics;

namespace OpenExamSuite.Creator.Services;

/// <summary>
/// Locates the Simulator executable so Creator can "Try this exam" in Practice mode.
/// Installed layout: <c>{app}/Creator</c> and <c>{app}/Simulator</c> are siblings. On macOS the
/// two executables live in sibling <c>.app</c> bundles.
/// </summary>
public sealed class SimulatorLocator
{
    private readonly string _applicationDirectory;
    private readonly Lazy<string?> _executable;

    public SimulatorLocator(string? applicationDirectory = null)
    {
        _applicationDirectory = applicationDirectory ?? AppContext.BaseDirectory;
        _executable = new Lazy<string?>(Resolve);
    }

    public string? ExecutablePath => _executable.Value;

    public bool IsInstalled => ExecutablePath != null;

    public string? FindSimulatorPath() => ExecutablePath;

    public void LaunchPractice(string examFilePath)
    {
        var path = FindSimulatorPath();
        if (string.IsNullOrEmpty(path))
            return;

        var psi = new ProcessStartInfo(path, $"\"{examFilePath}\"")
        {
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    internal static IEnumerable<string> Candidates(string applicationDirectory)
    {
        var exe = $"OpenExamSuite.Simulator{GetExecutableExtension()}";
        var trimmed = applicationDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // Executable and a Simulator folder beside it.
        yield return Path.Combine(trimmed, exe);
        yield return Path.Combine(trimmed, "Simulator", exe);

        // macOS: Creator and Simulator are sibling .app bundles. From Contents/MacOS this
        // resolves to <install>/Open Exam Suite Simulator.app/Contents/MacOS/<exe>.
        yield return Path.GetFullPath(Path.Combine(
            trimmed, "..", "..", "..", "Open Exam Suite Simulator.app", "Contents", "MacOS", exe));

        // Development tree: walk up looking for Apps/Simulator or src/Apps/Simulator.
        var dir = new DirectoryInfo(trimmed);
        while (dir != null)
        {
            yield return Path.Combine(dir.FullName, "Simulator", exe);
            yield return Path.Combine(dir.FullName, "Apps", "Simulator", exe);
            yield return Path.Combine(dir.FullName, "src", "Apps", "Simulator", exe);
            dir = dir.Parent;
        }
    }

    private string? Resolve() => Candidates(_applicationDirectory).FirstOrDefault(File.Exists);

    private static string GetExecutableExtension() =>
        OperatingSystem.IsWindows() ? ".exe" : string.Empty;
}
