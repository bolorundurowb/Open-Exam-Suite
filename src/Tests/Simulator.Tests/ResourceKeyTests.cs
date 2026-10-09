using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.Simulator.Tests;

/// <summary>
/// Every user-visible string must come from <c>Strings.resx</c>. These tests fail when code or XAML asks for a key
/// that does not exist, so a missing translation never reaches a user as "[Key]".
/// </summary>
public class ResourceKeyTests
{
    private static readonly string[] DynamicKeys =
    [
        "Common_Yes", "Common_No", "Exam_Flag", "Exam_Unflag", "PreExam_StartExam", "PreExam_StartPractice",
        "Screen_Exam", "Screen_Practice",
        "Review_Filter_All", "Review_Filter_Wrong", "Review_Filter_Unanswered", "Review_Filter_Flagged", "Review_Filter_Correct"
    ];

    private static readonly string[] PluralKeys =
        ["Common_Questions", "Common_Sections", "Common_Minutes", "PreExam_MissedSet", "Review_ConfirmMessage"];

    private static string SimulatorDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "Apps", "Simulator", "Resources", "Strings.resx");
            if (File.Exists(candidate))
                return Path.Combine(directory.FullName, "Apps", "Simulator");

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate src/Apps/Simulator from " + AppContext.BaseDirectory);
    }

    private static HashSet<string> DefinedKeys() =>
        XDocument.Load(Path.Combine(SimulatorDirectory(), "Resources", "Strings.resx"))
            .Descendants("data")
            .Select(d => (string)d.Attribute("name")!)
            .ToHashSet();

    private static IEnumerable<string> UsedKeys()
    {
        var patterns = new[]
        {
            new Regex(@"\{loc:Loc\s+([A-Za-z0-9_]+)\}"),
            new Regex(@"Strings\.(?:Get|Format|Plural)\(\s*""([A-Za-z0-9_]+)""")
        };

        var root = SimulatorDirectory();
        foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
        {
            var separators = new[] { Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar };
            if (!(file.EndsWith(".cs") || file.EndsWith(".axaml")) || separators.Any(file.Contains))
                continue;

            var text = File.ReadAllText(file);
            foreach (var pattern in patterns)
            {
                foreach (Match match in pattern.Matches(text))
                    yield return match.Groups[1].Value;
            }
        }
    }

    [Fact]
    public void EveryKeyUsedInCodeAndXamlIsDefined()
    {
        var defined = DefinedKeys();
        var missing = UsedKeys()
            .Concat(DynamicKeys)
            .Where(k => k != "Review_Filter_")
            .Where(k => PluralKeys.Contains(k)
                ? !(defined.Contains(k + "_One") && defined.Contains(k + "_Other"))
                : !defined.Contains(k))
            .Distinct()
            .OrderBy(k => k)
            .ToList();

        string.Join(", ", missing).Must().Be(string.Empty);
    }

    [Fact]
    public void EveryPluralKeyHasOneAndOtherForms()
    {
        var defined = DefinedKeys();

        PluralKeys.All(k => defined.Contains(k + "_One") && defined.Contains(k + "_Other")).Must().BeTrue();
    }
}
