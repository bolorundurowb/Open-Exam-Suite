using System;
using Avalonia.Threading;

namespace OpenExamSuite.Simulator.Services;

public sealed class DispatcherTimerService : ITimerService
{
    public ICountdownTimer Create(TimeSpan interval) => new DispatcherCountdownTimer(interval);
}

internal sealed class DispatcherCountdownTimer : ICountdownTimer
{
    private readonly DispatcherTimer _timer;

    public event EventHandler? Tick;

    public bool IsRunning => _timer.IsEnabled;

    public DispatcherCountdownTimer(TimeSpan interval)
    {
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += (s, e) => Tick?.Invoke(this, EventArgs.Empty);
    }

    public void Start() => _timer.Start();
    public void Stop()  => _timer.Stop();

    public void Dispose()
    {
        _timer.Stop();
    }
}
