using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.Session.Services;
using OpenExamSuite.Simulator.ViewModels;
using OpenExamSuite.Simulator.Views;

namespace OpenExamSuite.Simulator;

public partial class App : Application
{
    private PowerEvents? _power;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && AppHost.Services != null)
        {
            var viewModel = AppHost.Get<MainWindowViewModel>();
            var window = new MainWindow { DataContext = viewModel };
            AppHost.Get<MainWindowAccessor>().Window = window;
            desktop.MainWindow = window;

            foreach (var path in AppHost.StartupPaths)
                _ = viewModel.OpenFileAsync(path);

            // A second launch forwards its file here over the named pipe.
            if (AppHost.Coordinator is { } coordinator)
                coordinator.Activated += paths => Dispatcher.UIThread.Post(() => BringToFrontAndOpen(window, viewModel, paths));

            // macOS and other platforms deliver "open document" as an activation event.
            if (this.TryGetFeature<IActivatableLifetime>() is { } activatable)
            {
                activatable.Activated += (_, args) =>
                {
                    if (args is FileActivatedEventArgs files)
                    {
                        var paths = files.Files.Select(f => f.TryGetLocalPath()).OfType<string>().ToList();
                        Dispatcher.UIThread.Post(() => BringToFrontAndOpen(window, viewModel, paths));
                    }
                };
            }

            if (OperatingSystem.IsWindows())
                _power = new PowerEvents(AppHost.Get<ISimulatorSession>());

            desktop.Exit += (_, _) =>
            {
                if (OperatingSystem.IsWindows())
                    _power?.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void BringToFrontAndOpen(Window window, MainWindowViewModel viewModel, IReadOnlyList<string> paths)
    {
        if (window.WindowState == WindowState.Minimized)
            window.WindowState = WindowState.Normal;

        window.Activate();

        foreach (var path in paths)
            _ = viewModel.OpenFileAsync(path);
    }
}
