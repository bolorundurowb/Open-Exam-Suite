using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenExamSuite.Simulator.Session.HostPorts;

/// <summary>
/// File system access for the session layer. The host implements this.
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Opens a file for reading.
    /// </summary>
    Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a file for writing.
    /// </summary>
    Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file.
    /// </summary>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all files matching a pattern in a directory.
    /// </summary>
    Task<string[]> GetFilesAsync(string directory, string pattern, CancellationToken cancellationToken = default);
}