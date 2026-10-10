using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OpenExamSuite.Simulator.Engine.HostPorts;

/// <summary>
/// Default cross-platform implementation of <see cref="IAppPaths"/>.
/// Hosts (WinForms, Avalonia, browser) may replace this with their own resolver.
/// </summary>
public sealed class PlatformAppPaths : IAppPaths
{
    private readonly string _applicationDirectory;

    public PlatformAppPaths(string? applicationDirectory = null)
    {
        _applicationDirectory = applicationDirectory?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            ?? AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <inheritdoc />
    public string BundledSamplesRoot => GetBundledSamplesRoot(_applicationDirectory);

    /// <inheritdoc />
    public string UserDataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OpenExamSuite");

    /// <inheritdoc />
    public string TempDirectory => Path.Combine(Path.GetTempPath(), "OpenExamSuite");

    /// <inheritdoc />
    public string DocumentsDirectory
    {
        get
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!string.IsNullOrWhiteSpace(documents))
                return documents;

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return string.IsNullOrWhiteSpace(userProfile)
                ? UserDataDirectory
                : Path.Combine(userProfile, "Documents");
        }
    }

    /// <inheritdoc />
    public string GetBundledSamplePath(string fileName) => Path.Combine(BundledSamplesRoot, fileName);

    private static string GetBundledSamplesRoot(string applicationDirectory)
    {
        // macOS app bundle: Contents/MacOS and Contents/Resources are siblings.
        // Recognize the explicit bundle layout on every OS so this path contract is testable.
        var parentDirectory = Path.GetDirectoryName(applicationDirectory);
        if (string.Equals(Path.GetFileName(applicationDirectory), "MacOS", StringComparison.OrdinalIgnoreCase)
            && string.Equals(Path.GetFileName(parentDirectory), "Contents", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFullPath(Path.Combine(applicationDirectory, "..", "Resources"));
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // Fallback: look for a .app bundle ancestor.
            var candidate = applicationDirectory;
            while (!string.IsNullOrEmpty(candidate))
            {
                if (Path.GetExtension(candidate).Equals(".app", StringComparison.OrdinalIgnoreCase))
                    return Path.Combine(candidate, "Contents", "Resources");
                candidate = Path.GetDirectoryName(candidate);
            }
        }

        // Windows/Linux installed layout: executable is under {app}/Creator or {app}/Simulator.
        var siblingSamples = Path.GetFullPath(Path.Combine(applicationDirectory, "..", "Samples"));
        if (Directory.Exists(siblingSamples))
            return siblingSamples;

        // Development/standalone layout.
        var localSamples = Path.Combine(applicationDirectory, "Samples");
        if (Directory.Exists(localSamples))
            return localSamples;

        return siblingSamples;
    }
}
