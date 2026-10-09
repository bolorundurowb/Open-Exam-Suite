using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.Session.Services;
using OpenExamSuite.Creator.ViewModels;
using OpenExamSuite.Logging;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Creator;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        using var provider = ConfigureServices();
        AppHost.Services = provider;
        AppHost.StartupPaths = args
            .Where(MainWindowViewModel.IsSupportedPath)
            .Select(Path.GetFullPath)
            .ToList();

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddProvider(new OesFileLoggerProvider()));

        services.AddSingleton<IAppSettingsService>(_ => new AppSettingsService());
        services.AddSingleton<IExamLibraryService>(_ => new ExamLibraryService());
        services.AddSingleton<Reader>();
        services.AddSingleton<Writer>();
        services.AddSingleton<CreatorDocument>();
        services.AddSingleton<RecentExamsService>();

        services.AddSingleton<ThemeService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<ToastService>();
        services.AddSingleton<SimulatorLocator>();
        services.AddSingleton<IToastService>(sp => sp.GetRequiredService<ToastService>());
        services.AddSingleton<ShellServices>();
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
