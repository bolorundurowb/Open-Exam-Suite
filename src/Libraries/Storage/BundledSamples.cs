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
    /// in <c>{app}/Samples</c>.
    /// </summary>
    public static IReadOnlyList<string> Resolve(string applicationDirectory)
    {
        if (string.IsNullOrWhiteSpace(applicationDirectory))
            return [];

        var candidates = new[]
        {
            Path.GetFullPath(Path.Combine(applicationDirectory, "..", "Samples")),
            Path.Combine(applicationDirectory, "Samples")
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
