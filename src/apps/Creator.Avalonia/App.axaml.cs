using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.ViewModels;
using OpenExamSuite.Creator.ViewModels.Panes;
using OpenExamSuite.Creator.Views;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Creator;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        // Storage (unchanged)
        services.AddSingleton<IAppSettingsService>(_ => new AppSettingsService());

        // Shared services
        services.AddSingleton<IFilePickerService, FilePickerService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddSingleton<IOpenUrlService, OpenUrlService>();

        // Creator services
        services.AddSingleton<IUndoRedoService, UndoRedoService>();
        services.AddSingleton<IExamHistoryService, ExamHistoryService>();
        services.AddSingleton<IPrintService, PdfPrintService>();
        services.AddSingleton<IClipboardService, ClipboardService>();

        // ViewModels (transient pane factories)
        services.AddTransient<SplashPaneViewModel>();
        services.AddTransient<ExamPropertiesPaneViewModel>();
        services.AddTransient<QuestionEditorPaneViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = Services.GetRequiredService<MainWindowViewModel>();
            var window = new MainWindow { DataContext = vm };
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
