using Avalonia;
using Avalonia.Headless;
using Microsoft.Extensions.DependencyInjection;
using OmniAssert;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Engine.States;
using OpenExamSuite.Simulator.ViewModels;
using OpenExamSuite.Storage.Services;
using Xunit;

namespace OpenExamSuite.App.Headless.Tests;

/// <summary>
/// Starts the Simulator on the headless platform and checks the Library's Creator actions.
/// "New exam" follows <see cref="LibraryViewModel.CreatorAvailable"/> and each card's Edit action
/// follows <see cref="ExamCardViewModel.CanEdit"/>.
/// </summary>
public class SimulatorHeadlessTests
{
    private static string NewRoot(string prefix) =>
        Path.Combine(Path.GetTempPath(), $"{prefix} {Guid.NewGuid():N}");

    private static ExamCard Card(string root) => new(
        Path.Combine(root, "Sample.oef"), "Sample", "S1", 3, 1, 0, 50, null, null, null, false, false);

    [Fact]
    public void Simulator_Starts_Headlessly_And_Hides_Creator_Actions_When_Creator_Is_Absent()
    {
        var previous = Environment.GetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable);
        Environment.SetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable, null);

        var root = NewRoot("oes headless absent");
        Directory.CreateDirectory(root);

        try
        {
            using var session = HeadlessUnitTestSession.StartNew(typeof(SimulatorHeadlessEntry));
            session.Dispatch(() =>
            {
                using var provider = OpenExamSuite.Simulator.Program.ConfigureServices(root);
                var shell = provider.GetRequiredService<ShellServices>();
                var settings = new AppSettingsService(Path.Combine(root, "settings.db"));
                var library = new LibraryViewModel(shell, settings);
                library.Apply(new LibraryState([Card(root)], [], null, null, false));

                Application.Current.Must().NotBeNull();
                library.CreatorAvailable.Must().BeFalse();
                library.Cards.Count.Must().Be(1);
                library.Cards[0].CanEdit.Must().BeFalse();
            }, CancellationToken.None);
        }
        finally
        {
            Environment.SetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable, previous);
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Simulator_Shows_Creator_Actions_When_Creator_Is_Beside_It()
    {
        var previous = Environment.GetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable);
        Environment.SetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable, null);

        var root = NewRoot("oes headless present");
        Directory.CreateDirectory(root);
        var creatorName = OperatingSystem.IsWindows() ? "OpenExamSuite.Creator.exe" : "OpenExamSuite.Creator";
        File.WriteAllText(Path.Combine(root, creatorName), string.Empty);

        try
        {
            using var session = HeadlessUnitTestSession.StartNew(typeof(SimulatorHeadlessEntry));
            session.Dispatch(() =>
            {
                using var provider = OpenExamSuite.Simulator.Program.ConfigureServices(root);
                var shell = provider.GetRequiredService<ShellServices>();
                var settings = new AppSettingsService(Path.Combine(root, "settings.db"));
                var library = new LibraryViewModel(shell, settings);
                library.Apply(new LibraryState([Card(root)], [], null, null, false));

                library.CreatorAvailable.Must().BeTrue();
                library.Cards[0].CanEdit.Must().BeTrue();
            }, CancellationToken.None);
        }
        finally
        {
            Environment.SetEnvironmentVariable(CreatorLocator.OverrideEnvironmentVariable, previous);
            Directory.Delete(root, true);
        }
    }
}
