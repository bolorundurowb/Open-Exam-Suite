using System.Collections.ObjectModel;
using Avalonia.Threading;
using OpenExamSuite.Simulator.Session.HostPorts;

namespace OpenExamSuite.Simulator.Services;

public sealed record ToastItem(string Message, ToastKind Kind)
{
    public string IconKey => Kind switch
    {
        ToastKind.Success => "IconCheck",
        ToastKind.Warning => "IconWarning",
        ToastKind.Error => "IconClose",
        _ => "IconDot"
    };
}

/// <summary>
/// Non-blocking confirmations. The view announces the toast area as a polite live region.
/// </summary>
public sealed class ToastService : IToastService
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(4);

    public ObservableCollection<ToastItem> Toasts { get; } = [];

    public Task ShowAsync(string message, ToastKind kind = ToastKind.Info, TimeSpan? duration = null)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var toast = new ToastItem(message, kind);
            Toasts.Add(toast);
            DispatcherTimer.RunOnce(() => Toasts.Remove(toast), duration ?? DefaultDuration);
        });

        return Task.CompletedTask;
    }
}
