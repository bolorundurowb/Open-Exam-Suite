using System.Threading.Tasks;

namespace OpenExamSuite.Simulator.Session.HostPorts;

/// <summary>
/// URI launching (opening URLs, files, etc.). The host implements this.
/// </summary>
public interface IUriLauncher
{
    /// <summary>
    /// Opens a URI in the default browser or associated application.
    /// </summary>
    Task<bool> LaunchUriAsync(string uri);

    /// <summary>
    /// Opens a file with its associated application.
    /// </summary>
    Task<bool> OpenFileAsync(string filePath);

    /// <summary>
    /// Shows a file in the system file manager (Explorer, Finder, etc.).
    /// </summary>
    Task<bool> ShowInFolderAsync(string filePath);
}