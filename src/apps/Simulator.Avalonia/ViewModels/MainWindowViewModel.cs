using System;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Shared.Avalonia.Dialogs;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Simulator.Services;
using OpenExamSuite.Simulator.ViewModels.Routes;
using OpenExamSuite.Simulator.Views;
using OpenExamSuite.Simulator.Views.Dialogs;

namespace OpenExamSuite.Simulator.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;
    private readonly IOpenUrlService _urls;

    [ObservableProperty] private IRouteViewModel? _currentRoute;

    public MainWindowViewModel(IServiceProvider sp, INavigationService nav, IOpenUrlService urls)
    {
        _sp = sp;
        _nav = nav;
        _urls = urls;

        ((NavigationService)_nav).Configure(this);

        // Boot to the Home route.
        CurrentRoute = _sp.GetRequiredService<HomeViewModel>();
    }

    [RelayCommand]
    private void Exit()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task AboutAsync()
    {
        var dlg = new AboutDialog();
        await dlg.ShowDialog(MainWindow.Current!);
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LicenseAsync()
    {
        var dlg = new LicenseDialog();
        await dlg.ShowDialog(MainWindow.Current!);
    }
}
