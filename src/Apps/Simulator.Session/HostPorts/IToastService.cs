using System.Threading.Tasks;

namespace OpenExamSuite.Simulator.Session.HostPorts;

/// <summary>
/// Non-blocking toast notifications for routine confirmations. The host implements this.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Shows a brief informational toast.
    /// </summary>
    Task ShowAsync(string message, ToastKind kind = ToastKind.Info, TimeSpan? duration = null);

    /// <summary>
    /// Shows a success toast (e.g., "Saved").
    /// </summary>
    Task ShowSuccessAsync(string message, TimeSpan? duration = null) => ShowAsync(message, ToastKind.Success, duration);

    /// <summary>
    /// Shows a warning toast.
    /// </summary>
    Task ShowWarningAsync(string message, TimeSpan? duration = null) => ShowAsync(message, ToastKind.Warning, duration);

    /// <summary>
    /// Shows an error toast.
    /// </summary>
    Task ShowErrorAsync(string message, TimeSpan? duration = null) => ShowAsync(message, ToastKind.Error, duration);
}

public enum ToastKind
{
    Info,
    Success,
    Warning,
    Error
}