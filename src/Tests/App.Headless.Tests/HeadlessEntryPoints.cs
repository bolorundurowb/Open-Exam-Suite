using Avalonia;
using Avalonia.Headless;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace OpenExamSuite.App.Headless.Tests;

/// <summary>
/// Entry points that build each app on the Avalonia headless platform. <see cref="HeadlessUnitTestSession"/>
/// uses a type that exposes a <c>BuildAvaloniaApp</c> method, so both apps can share one test assembly.
/// </summary>
internal static class SimulatorHeadlessEntry
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<OpenExamSuite.Simulator.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

internal static class CreatorHeadlessEntry
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<OpenExamSuite.Creator.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
