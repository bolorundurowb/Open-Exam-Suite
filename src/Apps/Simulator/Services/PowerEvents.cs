using System.Runtime.Versioning;
using Microsoft.Win32;
using OpenExamSuite.Simulator.Engine.Services;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Stops the exam clock while the machine sleeps so sleep does not consume exam time.
/// Wired on Windows, where the operating system reports power changes. On macOS and Linux a sleep
/// is not detected yet.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class PowerEvents : IDisposable
{
    private readonly ISimulatorSession _session;

    public PowerEvents(ISimulatorSession session)
    {
        _session = session;
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
    }

    public void Dispose() => SystemEvents.PowerModeChanged -= OnPowerModeChanged;

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        switch (e.Mode)
        {
            case PowerModes.Suspend:
                _ = _session.NotifySystemSuspendingAsync();
                break;
            case PowerModes.Resume:
                _ = _session.NotifySystemResumedAsync();
                break;
        }
    }
}
