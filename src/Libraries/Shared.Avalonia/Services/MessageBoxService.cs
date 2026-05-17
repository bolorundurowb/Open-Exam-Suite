using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace OpenExamSuite.Shared.Avalonia.Services;

/// <summary>
/// Replacement for <see cref="System.Windows.Forms.MessageBox"/>.
/// </summary>
public sealed class MessageBoxService : IMessageBoxService
{
    public async Task<MessageBoxResult> ShowAsync(
        string message,
        string title,
        MessageBoxButtons buttons = MessageBoxButtons.Ok,
        MessageBoxIconKind icon = MessageBoxIconKind.Information)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, message, MapButtons(buttons), MapIcon(icon));
        var result = await box.ShowAsync();
        return MapResult(result);
    }

    private static ButtonEnum MapButtons(MessageBoxButtons buttons) => buttons switch
    {
        MessageBoxButtons.OkCancel    => ButtonEnum.OkCancel,
        MessageBoxButtons.YesNo       => ButtonEnum.YesNo,
        MessageBoxButtons.YesNoCancel => ButtonEnum.YesNoCancel,
        _                             => ButtonEnum.Ok,
    };

    private static Icon MapIcon(MessageBoxIconKind icon) => icon switch
    {
        MessageBoxIconKind.Information => Icon.Info,
        MessageBoxIconKind.Warning     => Icon.Warning,
        MessageBoxIconKind.Error       => Icon.Error,
        MessageBoxIconKind.Question    => Icon.Question,
        _                              => Icon.None,
    };

    private static MessageBoxResult MapResult(ButtonResult r) => r switch
    {
        ButtonResult.Ok     => MessageBoxResult.Ok,
        ButtonResult.Cancel => MessageBoxResult.Cancel,
        ButtonResult.Yes    => MessageBoxResult.Yes,
        ButtonResult.No     => MessageBoxResult.No,
        _                   => MessageBoxResult.None,
    };
}
