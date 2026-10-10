using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenExamSuite.Logging;
using OpenExamSuite.Shared.Interfaces;
using OpenExamSuite.Shared.Services;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.HostPorts;
using OpenExamSuite.Simulator.Session.Services;
using OpenExamSuite.Simulator.ViewModels;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Simulator;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var paths = ResolveExamPaths(args);

        // A second Simulator hands its file to the running one and exits. It no longer shows a message and drops the path.
        if (SingleInstanceCoordinator.TryForward(paths))
            return 0;

        var coordinator = new SingleInstanceCoordinator();
        if (!coordinator.TryStart())
        {
            // Another process won the race to become the primary instance.
            if (SingleInstanceCoordinator.TryForward(paths, timeoutMs: 2000))
                return 0;

            coordinator.Dispose();
            coordinator = null;
        }

        using var provider = ConfigureServices();
        AppHost.Services = provider;
        AppHost.StartupPaths = paths;
        AppHost.Coordinator = coordinator;

        try
        {
            return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            coordinator?.Dispose();
        }
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    internal static IReadOnlyList<string> ResolveExamPaths(IEnumerable<string> args) =>
        args
            .Where(MainWindowViewModel.IsSupportedPath)
            .Select(a => Path.GetFullPath(a))
            .ToList();

    private static ServiceProvider ConfigureServices() => ConfigureServices(null);

    /// <summary>
    /// Builds the service graph. <paramref name="applicationDirectory"/> overrides where the
    /// Creator executable is looked for; production passes <c>null</c> to use the app directory.
    /// </summary>
    internal static ServiceProvider ConfigureServices(string? applicationDirectory)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddProvider(new OesFileLoggerProvider()));

        services.AddSingleton<IAppSettingsService>(_ => new AppSettingsService());
        services.AddSingleton<IExamLibraryService>(_ => new ExamLibraryService());
        services.AddSingleton<Reader>();
        services.AddSingleton<Writer>();
        services.AddSingleton<ExamFileLoader>();
        services.AddSingleton<IExamComposer, ExamComposer>();
        services.AddSingleton<IScorer, Scorer>();
        services.AddSingleton(TimeProvider.System);

        // Host ports for the session layer.
        services.AddSingleton<IAppPaths>(_ => new PlatformAppPaths());
        services.AddSingleton<IFileSystem, PhysicalFileSystem>();
        services.AddSingleton<IUriLauncher, ShellUriLauncher>();
        services.AddSingleton<IPrintService, PdfPrintService>();
        services.AddSingleton<MainWindowAccessor>();
        services.AddSingleton<IUiDispatcher, AvaloniaUiDispatcher>();
        services.AddSingleton<ToastService>();
        services.AddSingleton<IToastService>(sp => sp.GetRequiredService<ToastService>());
        services.AddSingleton<DialogService>();
        services.AddSingleton<IPrompts, AvaloniaPrompts>();
        services.AddSingleton(_ => new CreatorLocator(applicationDirectory));
        services.AddSingleton<ThemeService>();

        services.AddSingleton<ISimulatorSession, SimulatorSession>();
        services.AddSingleton<ShellServices>();
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
