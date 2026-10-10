using System.Collections.ObjectModel;
using Avalonia.Threading;

namespace OpenExamSuite.Creator.Services;

public sealed class ToastItem
{
    public string Message { get; }
    public bool IsWarning { get; }

    public ToastItem(string message, bool isWarning = false)
    {
        Message = message;
        IsWarning = isWarning;
    }
}

public interface IToastService
{
    void Show(string message);
    void ShowWarning(string message);
}

public sealed class ToastService : IToastService
{
    private readonly ObservableCollection<ToastItem> _toasts = [];

    public ObservableCollection<ToastItem> Toasts => _toasts;

    public void Show(string message) => Post(new ToastItem(message));

    public void ShowWarning(string message) => Post(new ToastItem(message, true));

    private void Post(ToastItem item)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _toasts.Add(item);
            _ = DismissAfterDelay(item);
        });
    }

    private async Task DismissAfterDelay(ToastItem item)
    {
        await Task.Delay(2500);
        Dispatcher.UIThread.Post(() => _toasts.Remove(item));
    }
}
