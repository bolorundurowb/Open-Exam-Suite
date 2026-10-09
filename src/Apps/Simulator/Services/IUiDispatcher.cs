using Avalonia.Threading;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Marshals work onto the UI thread. The session raises state changes from timer threads.
/// </summary>
public interface IUiDispatcher
{
    void Post(Action action);
}

public sealed class AvaloniaUiDispatcher : IUiDispatcher
{
    public void Post(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
            action();
        else
            Dispatcher.UIThread.Post(action);
    }
}
