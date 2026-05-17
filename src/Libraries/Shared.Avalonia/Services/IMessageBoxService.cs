using System.Threading.Tasks;

namespace OpenExamSuite.Shared.Avalonia.Services;

public enum MessageBoxButtons
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel,
}

public enum MessageBoxIconKind
{
    None,
    Information,
    Warning,
    Error,
    Question,
}

public enum MessageBoxResult
{
    None,
    Ok,
    Cancel,
    Yes,
    No,
}

public interface IMessageBoxService
{
    Task<MessageBoxResult> ShowAsync(
        string message,
        string title,
        MessageBoxButtons buttons = MessageBoxButtons.Ok,
        MessageBoxIconKind icon   = MessageBoxIconKind.Information);
}
