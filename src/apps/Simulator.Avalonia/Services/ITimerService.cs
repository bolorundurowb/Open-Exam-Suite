using System;

namespace OpenExamSuite.Simulator.Services;

public interface ITimerService
{
    /// <summary>Returns a new 1-second-interval timer instance.</summary>
    ICountdownTimer Create(TimeSpan interval);
}

public interface ICountdownTimer : IDisposable
{
    event EventHandler? Tick;
    bool IsRunning { get; }
    void Start();
    void Stop();
}
