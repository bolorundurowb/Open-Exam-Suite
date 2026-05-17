using OpenExamSuite.Simulator.ViewModels.Routes;

namespace OpenExamSuite.Simulator.Services;

public interface INavigationService
{
    void GoTo(IRouteViewModel route);
    void GoBack();
    bool CanGoBack { get; }
}
