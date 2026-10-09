using System.Diagnostics;

namespace OpenExamSuite.Creator.Services;

/// <summary>
/// Locates the Simulator executable so Creator can "Try this exam" in Practice mode.
/// </summary>
public sealed class SimulatorLocator
{
    public bool IsInstalled => !string.IsNullOrEmpty(FindSimulatorPath());

    public string? FindSimulatorPath()
    {
        var ownDir = AppContext.BaseDirectory;
        var exe = $"OpenExamSuite.Simulator{GetExecutableExtension()}";

        var candidates = new List<string>
        {
            Path.Combine(ownDir, exe),
            Path.Combine(ownDir, "Simulator", exe)
        };

        var dir = new DirectoryInfo(ownDir);
        while (dir != null)
        {
            candidates.Add(Path.Combine(dir.FullName, $"Simulator{Path.DirectorySeparatorChar}{exe}"));
            candidates.Add(Path.Combine(dir.FullName, $"Apps{Path.DirectorySeparatorChar}Simulator{Path.DirectorySeparatorChar}{exe}"));
            candidates.Add(Path.Combine(dir.FullName, $"src{Path.DirectorySeparatorChar}Apps{Path.DirectorySeparatorChar}Simulator{Path.DirectorySeparatorChar}{exe}"));
            dir = dir.Parent;
        }

        return candidates.FirstOrDefault(File.Exists);
    }

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

    private static string GetExecutableExtension() =>
        OperatingSystem.IsWindows() ? ".exe" : string.Empty;
}
