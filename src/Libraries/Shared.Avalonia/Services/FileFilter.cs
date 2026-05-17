using System.Collections.Generic;

namespace OpenExamSuite.Shared.Avalonia.Services;

/// <summary>
/// Replacement for a single segment of the WinForms <c>OpenFileDialog.Filter</c>
/// pipe-separated string (e.g. <c>"OEF Files|*.oef"</c>).
/// </summary>
/// <param name="Display">The display name shown in the platform file picker.</param>
/// <param name="Patterns">One or more glob patterns (e.g. "*.oef").</param>
public sealed record FileFilter(string Display, IReadOnlyList<string> Patterns)
{
    public FileFilter(string display, params string[] patterns)
        : this(display, (IReadOnlyList<string>)patterns) { }

    /// <summary>
    /// Parses a single pipe-separated WinForms-style filter string into a list
    /// of <see cref="FileFilter"/>s.
    /// </summary>
    public static IReadOnlyList<FileFilter> ParseLegacy(string legacyFilter)
    {
        var parts = legacyFilter.Split('|');
        var list = new List<FileFilter>(parts.Length / 2);
        for (var i = 0; i + 1 < parts.Length; i += 2)
        {
            var name = parts[i];
            var patterns = parts[i + 1].Split(';');
            list.Add(new FileFilter(name, patterns));
        }
        return list;
    }
}
