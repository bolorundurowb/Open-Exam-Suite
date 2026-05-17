using System.Collections.Generic;
using OpenExamSuite.Simulator.ViewModels;
using OpenExamSuite.Simulator.ViewModels.Routes;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Maintains a back-stack of routed view models. The Simulator's
/// <see cref="MainWindowViewModel"/> registers itself once via
/// <see cref="Configure"/> so navigation can target its
/// <see cref="MainWindowViewModel.CurrentRoute"/> property.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private MainWindowViewModel? _host;
    private readonly Stack<IRouteViewModel> _back = new();

    public bool CanGoBack => _back.Count > 0;

    public void Configure(MainWindowViewModel host)
    {
        _host = host;
    }

    public void GoTo(IRouteViewModel route)
    {
        if (_host is null) return;
        if (_host.CurrentRoute is not null)
            _back.Push(_host.CurrentRoute);
        _host.CurrentRoute = route;
    }

    public void GoBack()
    {
        if (_host is null) return;
        if (_back.Count == 0) return;
        _host.CurrentRoute = _back.Pop();
    }
}
