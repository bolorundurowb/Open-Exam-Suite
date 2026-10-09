using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Session.HostPorts;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Simulator.Views.Dialogs;

namespace OpenExamSuite.Simulator.Services;

public sealed record DialogButton(string Text, bool IsPrimary = false, bool IsCancel = false);

/// <summary>
/// Modal dialogs, used only when a decision is needed. Routine confirmations are toasts.
/// </summary>
public sealed class DialogService
{
    private readonly MainWindowAccessor _window;
    private readonly IUriLauncher _launcher;

    public DialogService(MainWindowAccessor window, IUriLauncher launcher)
    {
        _window = window;
        _launcher = launcher;
    }

    /// <summary>Returns the index of the pressed button, or the cancel button's index on Escape. -1 when no window is available.</summary>
    public async Task<int> ShowMessageAsync(string title, string message, params DialogButton[] buttons)
    {
        if (_window.Window is not { } owner)
            return -1;

        var dialog = new MessageDialog(title, message, buttons);
        // Closing with the window's own close button yields null, which counts as cancel.
        var result = await dialog.ShowDialog<int?>(owner);
        return result ?? dialog.CancelIndex;
    }

    public async Task<bool> ConfirmAsync(string title, string message, string confirmText, string cancelText)
    {
        var result = await ShowMessageAsync(
            title,
            message,
            new DialogButton(cancelText, IsCancel: true),
            new DialogButton(confirmText, IsPrimary: true));
        return result == 1;
    }

    public Task ShowInfoAsync(string title, string message) =>
        ShowMessageAsync(title, message, new DialogButton(Strings.Get("Dialog_Ok"), IsPrimary: true, IsCancel: true));

    public async Task<string?> PromptTextAsync(string title, string prompt, string? defaultValue)
    {
        if (_window.Window is not { } owner)
            return null;

        var box = new TextBox { Text = defaultValue ?? string.Empty, MinWidth = 320, AcceptsReturn = false };
        var ok = new Button { Content = Strings.Get("Dialog_Ok"), Margin = new Thickness(8, 0, 0, 0) };
        var cancel = new Button { Content = Strings.Get("Dialog_Cancel") };
        ok.Theme = (Avalonia.Styling.ControlTheme)Application.Current!.FindResource("PrimaryButton")!;
        cancel.Theme = (Avalonia.Styling.ControlTheme)Application.Current.FindResource("SecondaryButton")!;

        var dialog = new Window
        {
            Title = title,
            Width = 480,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(24),
                Spacing = 14,
                Children =
                {
                    new TextBlock { Text = prompt },
                    box,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Children = { cancel, ok }
                    }
                }
            }
        };
        ok.Click += (_, _) => dialog.Close(box.Text);
        cancel.Click += (_, _) => dialog.Close(null);
        dialog.Opened += (_, _) => box.Focus();
        return await dialog.ShowDialog<string?>(owner);
    }

    public async Task ShowAboutAsync()
    {
        if (_window.Window is { } owner)
            await new AboutDialog(_launcher).ShowDialog(owner);
    }

    public async Task ShowDocumentAsync(string titleKey, string assetName, bool isMarkdown)
    {
        if (_window.Window is not { } owner)
            return;

        string text;
        try
        {
            text = AppInfo.ReadAsset(assetName);
        }
        catch (Exception)
        {
            text = Strings.Get("Dialog_DocumentMissing");
        }

        await new DocumentDialog(Strings.Get(titleKey), text, isMarkdown).ShowDialog(owner);
    }

    public Task ShowLicenseAsync() => ShowDocumentAsync("Menu_License", "LICENSE.txt", isMarkdown: false);

    public Task ShowChangelogAsync() => ShowDocumentAsync("Menu_Changelog", "CHANGELOG.md", isMarkdown: true);

    public async Task ShowPropertiesAsync(ExamFileProperties properties)
    {
        if (_window.Window is { } owner)
            await new PropertiesDialog(properties, path => _ = _launcher.ShowInFolderAsync(path)).ShowDialog(owner);
    }

    public async Task ShowAttemptAsync(AttemptSummary attempt)
    {
        if (_window.Window is { } owner)
            await new AttemptDialog(attempt).ShowDialog(owner);
    }

    public async Task ShowImageAsync(Bitmap image)
    {
        if (_window.Window is { } owner)
            await new ImageDialog(image).ShowDialog(owner);
    }
}
