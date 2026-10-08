namespace OpenExamSuite.Storage;

/// <summary>
/// The sample exams installed at <c>{app}/Samples</c> for both Creator and Simulator.
/// </summary>
public static class BundledSamples
{
    public static readonly string[] FileNames = ["Basic Science.oef", "GMAT Sample.oef"];

    /// <summary>
    /// Resolves the shipped sample files next to an application directory. Installed builds
    /// keep the executable in <c>{app}/Creator</c> or <c>{app}/Simulator</c> and the samples
    /// in <c>{app}/Samples</c>. On macOS the samples live inside the app bundle at
    /// <c>Contents/Resources</c>.
    /// </summary>
    public static IReadOnlyList<string> Resolve(string applicationDirectory)
    {
        if (string.IsNullOrWhiteSpace(applicationDirectory))
            return [];

        var candidates = new[]
        {
            // Windows/Linux installed layout: executable is under {app}/Creator or {app}/Simulator.
            Path.GetFullPath(Path.Combine(applicationDirectory, "..", "Samples")),
            // Standalone or development layout.
            Path.Combine(applicationDirectory, "Samples"),
            // macOS app bundle: executable is at OpenExamSuite.app/Contents/MacOS/{Creator|Simulator}.
            Path.GetFullPath(Path.Combine(applicationDirectory, "..", "Resources"))
        };

        foreach (var directory in candidates)
        {
            if (!Directory.Exists(directory))
                continue;

            var found = FileNames
                .Select(name => Path.Combine(directory, name))
                .Where(File.Exists)
                .ToList();

            if (found.Count > 0)
                return found;
        }

        return [];
    }
}
