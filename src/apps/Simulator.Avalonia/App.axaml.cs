using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.ViewModels;
using OpenExamSuite.Simulator.ViewModels.Routes;
using OpenExamSuite.Simulator.Views;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Simulator;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        // Storage (unchanged)
        services.AddSingleton<IAppSettingsService>(_ => new AppSettingsService());

        // Shared
        services.AddSingleton<IFilePickerService, FilePickerService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddSingleton<IOpenUrlService, OpenUrlService>();

        // Simulator services
        services.AddSingleton<ITimerService, DispatcherTimerService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IScoreSheetPrintService, ScoreSheetPrintService>();

        // ViewModels
        services.AddTransient<HomeViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Initial arg → add to recents so it shows up in the grid.
            var initialFile = Program.InitialExamFile;
            if (!string.IsNullOrWhiteSpace(initialFile)
                && Path.GetExtension(initialFile).Equals(".oef", StringComparison.OrdinalIgnoreCase))
            {
                var settings = Services.GetRequiredService<IAppSettingsService>();
                settings.Add(new AppSetting
                {
                    Name = Path.GetFileNameWithoutExtension(initialFile),
                    FilePath = initialFile,
                }, AppSettingsType.Simulator);
            }

            var vm = Services.GetRequiredService<MainWindowViewModel>();
            var window = new MainWindow { DataContext = vm };
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
