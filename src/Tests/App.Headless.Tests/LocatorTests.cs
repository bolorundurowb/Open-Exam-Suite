using OmniAssert;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Simulator.Services;
using Xunit;

namespace OpenExamSuite.App.Headless.Tests;

/// <summary>
/// The sibling layouts the installers produce: <c>{app}/Simulator</c> and <c>{app}/Creator</c> on
/// Windows and Linux, and two sibling <c>.app</c> bundles on macOS. Paths intentionally contain
/// spaces to mirror "Open Exam Suite" / "Open Exam Suite Creator.app".
/// </summary>
public class LocatorTests
{
    private static string NewRoot(string prefix) =>
        Path.Combine(Path.GetTempPath(), $"{prefix} {Guid.NewGuid():N}");

    private static string CreatorName => OperatingSystem.IsWindows() ? "OpenExamSuite.Creator.exe" : "OpenExamSuite.Creator";

    private static string SimulatorName => OperatingSystem.IsWindows() ? "OpenExamSuite.Simulator.exe" : "OpenExamSuite.Simulator";

    [Fact]
    public void CreatorLocator_FindsSiblingCreatorFolder()
    {
        var root = NewRoot("oes creator folder");
        var appDir = Path.Combine(root, "Simulator");
        Directory.CreateDirectory(appDir);
        var creatorPath = Path.Combine(root, "Creator", CreatorName);
        Directory.CreateDirectory(Path.GetDirectoryName(creatorPath)!);
        File.WriteAllText(creatorPath, string.Empty);

        try
        {
            var locator = new CreatorLocator(appDir);
            locator.IsInstalled.Must().BeTrue();
            Path.GetFullPath(locator.ExecutablePath!).Must().Be(Path.GetFullPath(creatorPath));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void CreatorLocator_FindsSiblingAppBundle()
    {
        var root = NewRoot("oes creator app");
        var appDir = Path.Combine(root, "Open Exam Suite Simulator.app", "Contents", "MacOS");
        Directory.CreateDirectory(appDir);
        var creatorPath = Path.Combine(root, "Open Exam Suite Creator.app", "Contents", "MacOS", CreatorName);
        Directory.CreateDirectory(Path.GetDirectoryName(creatorPath)!);
        File.WriteAllText(creatorPath, string.Empty);

        try
        {
            var locator = new CreatorLocator(appDir);
            locator.IsInstalled.Must().BeTrue();
            Path.GetFullPath(locator.ExecutablePath!).Must().Be(Path.GetFullPath(creatorPath));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void CreatorLocator_IsNotInstalledWhenNoBinaryExists()
    {
        var previous = Environment.GetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable);
        Environment.SetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable, null);
        var root = NewRoot("oes creator missing");
        var appDir = Path.Combine(root, "Simulator");
        Directory.CreateDirectory(appDir);

        try
        {
            new CreatorLocator(appDir).IsInstalled.Must().BeFalse();
        }
        finally
        {
            Directory.Delete(root, true);
            Environment.SetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable, previous);
        }
    }

    [Fact]
    public void SimulatorLocator_FindsSiblingSimulatorFolder()
    {
        var root = NewRoot("oes sim folder");
        var appDir = Path.Combine(root, "Creator");
        Directory.CreateDirectory(appDir);
        var simulatorPath = Path.Combine(root, "Simulator", SimulatorName);
        Directory.CreateDirectory(Path.GetDirectoryName(simulatorPath)!);
        File.WriteAllText(simulatorPath, string.Empty);

        try
        {
            var locator = new SimulatorLocator(appDir);
            locator.IsInstalled.Must().BeTrue();
            Path.GetFullPath(locator.ExecutablePath!).Must().Be(Path.GetFullPath(simulatorPath));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void SimulatorLocator_FindsSiblingAppBundle()
    {
        var root = NewRoot("oes sim app");
        var appDir = Path.Combine(root, "Open Exam Suite Creator.app", "Contents", "MacOS");
        Directory.CreateDirectory(appDir);
        var simulatorPath = Path.Combine(root, "Open Exam Suite Simulator.app", "Contents", "MacOS", SimulatorName);
        Directory.CreateDirectory(Path.GetDirectoryName(simulatorPath)!);
        File.WriteAllText(simulatorPath, string.Empty);

        try
        {
            var locator = new SimulatorLocator(appDir);
            locator.IsInstalled.Must().BeTrue();
            Path.GetFullPath(locator.ExecutablePath!).Must().Be(Path.GetFullPath(simulatorPath));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void SimulatorLocator_IsNotInstalledWhenNoBinaryExists()
    {
        var root = NewRoot("oes sim missing");
        var appDir = Path.Combine(root, "Creator");
        Directory.CreateDirectory(appDir);

        try
        {
            new SimulatorLocator(appDir).IsInstalled.Must().BeFalse();
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
