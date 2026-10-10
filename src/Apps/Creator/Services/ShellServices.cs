using OpenExamSuite.Creator.Engine.Services;
using OpenExamSuite.Storage.Interfaces;

namespace OpenExamSuite.Creator.Services;

/// <summary>
/// Host adapters exposed to the Creator view models. Keeps the session layer free of Avalonia types.
/// </summary>
public sealed class ShellServices
{
    public ShellServices(
        CreatorDocument document,
        DialogService dialogs,
        ToastService toasts,
        ThemeService theme,
        IAppSettingsService settings,
        IExamLibraryService library,
        SimulatorLocator simulator,
        RecentExamsService recents)
    {
        Document = document;
        Dialogs = dialogs;
        Toasts = toasts;
        Theme = theme;
        Settings = settings;
        Library = library;
        Simulator = simulator;
        Recents = recents;
    }

    public CreatorDocument Document { get; }

    public DialogService Dialogs { get; }

    public ToastService Toasts { get; }

    public ThemeService Theme { get; }

    public IAppSettingsService Settings { get; }

    public IExamLibraryService Library { get; }

    public SimulatorLocator Simulator { get; }

    public RecentExamsService Recents { get; }
}
