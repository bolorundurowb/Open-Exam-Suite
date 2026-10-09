using OmniAssert;
using OpenExamSuite.Simulator.Services;
using Xunit;

namespace OpenExamSuite.Simulator.Tests;

public class SingleInstanceCoordinatorTests
{
    [Fact]
    public void ParsePaths_RequiresTheProtocolHeader()
    {
        SingleInstanceCoordinator.ParsePaths(["not-the-header", "C:\\exams\\a.oef"]).Count.Must().Be(0);
    }

    [Fact]
    public void ParsePaths_ReturnsPathLinesAndSkipsBlanks()
    {
        var paths = SingleInstanceCoordinator.ParsePaths(["OES-SIMULATOR-1", "C:\\exams\\a.oef", "", "  ", null, "/home/u/b.oef"]);

        string.Join("|", paths).Must().Be("C:\\exams\\a.oef|/home/u/b.oef");
    }

    [Fact]
    public void ParsePaths_CapsTheNumberOfPaths()
    {
        var lines = new System.Collections.Generic.List<string?> { "OES-SIMULATOR-1" };
        for (var i = 0; i < 100; i++)
            lines.Add($"/exams/{i}.oef");

        SingleInstanceCoordinator.ParsePaths(lines).Count.Must().Be(32);
    }
}
