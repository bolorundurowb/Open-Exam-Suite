using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using OpenExamSuite.Creator.ViewModels;
using OpenExamSuite.Creator.Views;

namespace OpenExamSuite.Creator;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && AppHost.Services != null)
        {
            var viewModel = AppHost.Get<MainWindowViewModel>();
            var window = new MainWindow { DataContext = viewModel };
            desktop.MainWindow = window;

            foreach (var path in AppHost.StartupPaths)
                _ = viewModel.OpenFileAsync(path);

            if (this.TryGetFeature<IActivatableLifetime>() is { } activatable)
            {
                activatable.Activated += (_, args) =>
                {
                    if (args is FileActivatedEventArgs files)
                    {
                        var paths = files.Files.Select(f => f.TryGetLocalPath()).OfType<string>().ToList();
                        Dispatcher.UIThread.Post(() =>
                        {
                            window.Activate();
                            foreach (var path in paths)
                                _ = viewModel.OpenFileAsync(path);
                        });
                    }
                };
            }

            desktop.Exit += (_, _) => viewModel.Shutdown();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
