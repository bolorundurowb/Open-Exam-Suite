using System.Threading.Tasks;

namespace OpenExamSuite.Simulator.Session.HostPorts;

/// <summary>
/// User prompt/confirmation dialogs. The host implements this.
/// </summary>
public interface IPrompts
{
    /// <summary>
    /// Shows a confirmation dialog with Yes/No/Cancel options.
    /// </summary>
    Task<PromptResult> ConfirmAsync(string title, string message, string yesText = "Yes", string noText = "No", string? cancelText = "Cancel");

    /// <summary>
    /// Shows an information dialog with an OK button.
    /// </summary>
    Task InformAsync(string title, string message);

    /// <summary>
    /// Shows an error dialog with an OK button.
    /// </summary>
    Task ErrorAsync(string title, string message);

    /// <summary>
    /// Shows a dialog to enter text.
    /// </summary>
    Task<string?> InputAsync(string title, string prompt, string? defaultValue = null);

    /// <summary>
    /// Shows a file picker dialog.
    /// </summary>
    Task<string?> PickFileAsync(string title, string filter, string? initialDirectory = null);

    /// <summary>
    /// Shows a save file dialog.
    /// </summary>
    Task<string?> SaveFileAsync(string title, string filter, string? suggestedFileName = null, string? initialDirectory = null);

    /// <summary>
    /// Shows a folder picker dialog.
    /// </summary>
    Task<string?> PickFolderAsync(string title, string? initialDirectory = null);
}

public enum PromptResult
{
    Yes,
    No,
    Cancel
}