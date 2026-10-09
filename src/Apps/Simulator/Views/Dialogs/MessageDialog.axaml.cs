using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using OpenExamSuite.Simulator.Services;

namespace OpenExamSuite.Simulator.Views.Dialogs;

/// <summary>
/// A modal decision dialog. Used only when the user has to decide something; routine confirmations are toasts.
/// Closes with the index of the pressed button, or the cancel button's index on Escape.
/// </summary>
public partial class MessageDialog : Window
{
    private readonly int _cancelIndex;
    private Button? _primaryButton;

    public int CancelIndex => _cancelIndex;

    public MessageDialog()
        : this(string.Empty, string.Empty, [])
    {
    }

    public MessageDialog(string title, string message, IReadOnlyList<DialogButton> buttons)
    {
        InitializeComponent();
        Title = title;
        TitleText.Text = title;
        MessageText.Text = message;
        _cancelIndex = buttons.ToList().FindIndex(b => b.IsCancel);

        for (var i = 0; i < buttons.Count; i++)
        {
            var index = i;
            var definition = buttons[i];
            var button = new Button
            {
                Content = definition.Text,
                Margin = new Thickness(8, 0, 0, 0),
                Theme = (ControlTheme)Application.Current!.FindResource(definition.IsPrimary ? "PrimaryButton" : "SecondaryButton")!
            };
            button.Click += (_, _) => Close(index);
            ButtonPanel.Children.Add(button);

            if (definition.IsPrimary || _primaryButton == null)
                _primaryButton = button;
        }

        Opened += (_, _) => _primaryButton?.Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close(_cancelIndex);
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }
}
