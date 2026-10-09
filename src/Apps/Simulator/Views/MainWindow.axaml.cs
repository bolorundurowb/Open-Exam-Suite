using Avalonia.Controls;
using Avalonia.Input;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.ViewModels;

namespace OpenExamSuite.Simulator.Views;

public partial class MainWindow : Window
{
    private bool _allowClose;

    public MainWindow()
    {
        InitializeComponent();

        ExitItem.Click += (_, _) => Close();
        MainMenu.IsVisible = !OperatingSystem.IsMacOS();

        Opened += async (_, _) =>
        {
            if (DataContext is not MainWindowViewModel viewModel)
                return;

            if (OperatingSystem.IsMacOS())
                NativeMenu.SetMenu(this, BuildNativeMenu(viewModel));

            await viewModel.InitializeAsync();
            await viewModel.ShowChangelogIfUpdatedAsync();
        };
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        // Closing during an attempt would silently lose it, so ask first. Nothing is recorded if the user abandons.
        if (_allowClose || DataContext is not MainWindowViewModel { IsAttemptInProgress: true } viewModel)
            return;

        e.Cancel = true;
        _ = ConfirmCloseAsync(viewModel);
    }

    private async Task ConfirmCloseAsync(MainWindowViewModel viewModel)
    {
        if (!await viewModel.ConfirmAbandonAsync())
            return;

        _allowClose = true;
        Close();
    }

    private NativeMenu BuildNativeMenu(MainWindowViewModel viewModel)
    {
        var file = new NativeMenu
        {
            new NativeMenuItem(Strings.Get("Menu_AddExam"))
            {
                Command = viewModel.AddExamCommand,
                Gesture = new KeyGesture(Key.O, KeyModifiers.Meta)
            }
        };
        if (viewModel.CreatorAvailable)
            file.Add(new NativeMenuItem(Strings.Get("Menu_NewExam")) { Command = viewModel.NewExamCommand });

        var theme = new NativeMenu
        {
            new NativeMenuItem(Strings.Get("Theme_System")) { Command = viewModel.SetThemeCommand, CommandParameter = Services.ThemeMode.System },
            new NativeMenuItem(Strings.Get("Theme_Light")) { Command = viewModel.SetThemeCommand, CommandParameter = Services.ThemeMode.Light },
            new NativeMenuItem(Strings.Get("Theme_Dark")) { Command = viewModel.SetThemeCommand, CommandParameter = Services.ThemeMode.Dark }
        };
        var edit = new NativeMenu
        {
            new NativeMenuItem(Strings.Get("Menu_Theme")) { Menu = theme },
            new NativeMenuItem(Strings.Get("Menu_ClearHistory")) { Command = viewModel.ClearHistoryCommand }
        };

        var help = new NativeMenu
        {
            new NativeMenuItem(Strings.Get("Menu_About")) { Command = viewModel.ShowAboutCommand },
            new NativeMenuItem(Strings.Get("Menu_License")) { Command = viewModel.ShowLicenseCommand },
            new NativeMenuItem(Strings.Get("Menu_Changelog")) { Command = viewModel.ShowChangelogCommand }
        };

        return new NativeMenu
        {
            new NativeMenuItem(Strings.Get("Menu_File")) { Menu = file },
            new NativeMenuItem(Strings.Get("Menu_Edit")) { Menu = edit },
            new NativeMenuItem(Strings.Get("Menu_Help")) { Menu = help }
        };
    }
}
