using Microsoft.Extensions.Logging;
using OpenExamSuite.Simulator.Localization;
using OpenExamSuite.Simulator.Engine.HostPorts;
using OpenExamSuite.Simulator.Engine.Models;
using OpenExamSuite.Simulator.Engine.Services;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// The services every screen view model needs, bundled so constructors stay short.
/// </summary>
public sealed class ShellServices
{
    public ShellServices(
        ISimulatorSession session,
        DialogService dialogs,
        IToastService toasts,
        IUiDispatcher dispatcher,
        CreatorLocator creator,
        IUriLauncher launcher,
        IPrompts prompts,
        IAppPaths paths,
        ILogger<ShellServices> logger)
    {
        Session = session;
        Dialogs = dialogs;
        Toasts = toasts;
        Dispatcher = dispatcher;
        Creator = creator;
        Launcher = launcher;
        Prompts = prompts;
        Paths = paths;
        Logger = logger;
    }

    public ISimulatorSession Session { get; }
    public DialogService Dialogs { get; }
    public IToastService Toasts { get; }
    public IUiDispatcher Dispatcher { get; }
    public CreatorLocator Creator { get; }
    public IUriLauncher Launcher { get; }
    public IPrompts Prompts { get; }
    public IAppPaths Paths { get; }
    public ILogger<ShellServices> Logger { get; }

    /// <summary>
    /// Runs a user action. Failures are logged and shown as a localised message instead of crashing the app.
    /// </summary>
    public async Task RunAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (ExamLoadException ex)
        {
            Logger.LogWarning(ex, "Exam could not be opened.");
            await Dialogs.ShowInfoAsync(Strings.Get("Error_OpenTitle"), DescribeLoadError(ex));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected failure in a user action.");
            await Dialogs.ShowInfoAsync(Strings.Get("Error_UnexpectedTitle"), Strings.Get("Error_UnexpectedMessage"));
        }
    }

    public static string DescribeLoadError(ExamLoadException ex) => ex.IsUnsupportedFormat
        ? Strings.Get("Error_Unsupported")
        : ex.Error switch
    {
        OpenExamSuite.Shared.Enums.ExamFileLoadError.FileNotFound => Strings.Get("Error_FileMissing"),
        OpenExamSuite.Shared.Enums.ExamFileLoadError.EmptyOrInvalidJson => Strings.Get("Error_JsonInvalid"),
        OpenExamSuite.Shared.Enums.ExamFileLoadError.EmptyOrInvalidXml => Strings.Get("Error_XmlInvalid"),
        OpenExamSuite.Shared.Enums.ExamFileLoadError.InvalidXml => Strings.Get("Error_XmlInvalid"),
        _ => Strings.Get("Error_FileCorrupt")
    };

    public static string DescribeAction(LibraryActionStatus status) => status switch
    {
        LibraryActionStatus.FileNotFound => Strings.Get("Error_FileMissing"),
        LibraryActionStatus.UnsupportedFormat => Strings.Get("Error_Unsupported"),
        LibraryActionStatus.CorruptFile => Strings.Get("Error_FileCorrupt"),
        LibraryActionStatus.AlreadyInLibrary => Strings.Get("Library_AlreadyAdded"),
        LibraryActionStatus.WriteFailed => Strings.Get("Error_WriteFailed"),
        _ => string.Empty
    };
}
