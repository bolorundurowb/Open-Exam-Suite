using System;

namespace OpenExamSuite.Simulator.Session.HostPorts;

/// <summary>
/// Platform-agnostic application paths. The host (WinForms, Avalonia, browser) implements this.
/// </summary>
public interface IAppPaths
{
    /// <summary>
    /// Root directory where bundled sample exams are installed.
    /// Windows/Linux: {app}/Samples
    /// macOS: Contents/Resources
    /// </summary>
    string BundledSamplesRoot { get; }

    /// <summary>
    /// Directory for user data (library database, settings, recovery files).
    /// </summary>
    string UserDataDirectory { get; }

    /// <summary>
    /// Directory for temporary files (auto-save, crash recovery).
    /// </summary>
    string TempDirectory { get; }

    /// <summary>
    /// Gets the path to a bundled sample exam file.
    /// </summary>
    string GetBundledSamplePath(string fileName);
}