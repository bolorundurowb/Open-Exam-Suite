using Avalonia.Controls;
using Avalonia.Input;
using OpenExamSuite.Creator.Localization;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.ViewModels;

namespace OpenExamSuite.Creator.Views;

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
        };
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        if (_allowClose || DataContext is not MainWindowViewModel { IsDirty: true } viewModel)
            return;

        e.Cancel = true;
        _ = ConfirmCloseAsync(viewModel);
    }

    private async Task ConfirmCloseAsync(MainWindowViewModel viewModel)
    {
        if (!await viewModel.ConfirmCloseAsync())
            return;

        _allowClose = true;
        Close();
    }

    private NativeMenu BuildNativeMenu(MainWindowViewModel viewModel)
    {
        var file = new NativeMenu
        {
            new NativeMenuItem(Strings.Get("Menu_New")) { Command = viewModel.NewCommand, Gesture = new KeyGesture(Key.N, KeyModifiers.Meta) },
            new NativeMenuItem(Strings.Get("Menu_Open")) { Command = viewModel.OpenCommand, Gesture = new KeyGesture(Key.O, KeyModifiers.Meta) },
            new NativeMenuItem(Strings.Get("Menu_Save")) { Command = viewModel.SaveCommand, Gesture = new KeyGesture(Key.S, KeyModifiers.Meta) }
        };

        var edit = new NativeMenu
        {
            new NativeMenuItem(Strings.Get("Menu_Undo")) { Command = viewModel.UndoCommand, Gesture = new KeyGesture(Key.Z, KeyModifiers.Meta) },
            new NativeMenuItem(Strings.Get("Menu_Redo")) { Command = viewModel.RedoCommand, Gesture = new KeyGesture(Key.Y, KeyModifiers.Meta) },
            new NativeMenuItem(Strings.Get("Menu_Theme")) { Menu = new NativeMenu
            {
                new NativeMenuItem(Strings.Get("Theme_System")) { Command = viewModel.SetThemeCommand, CommandParameter = ThemeMode.System },
                new NativeMenuItem(Strings.Get("Theme_Light")) { Command = viewModel.SetThemeCommand, CommandParameter = ThemeMode.Light },
                new NativeMenuItem(Strings.Get("Theme_Dark")) { Command = viewModel.SetThemeCommand, CommandParameter = ThemeMode.Dark }
            }}
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
