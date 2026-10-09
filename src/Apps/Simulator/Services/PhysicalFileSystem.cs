using OpenExamSuite.Simulator.Session.HostPorts;

namespace OpenExamSuite.Simulator.Services;

public sealed class PhysicalFileSystem : IFileSystem
{
    public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default) =>
        Task.FromResult<Stream>(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true));

    public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default) =>
        Task.FromResult<Stream>(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true));

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default) =>
        Task.FromResult(File.Exists(path));

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        File.Delete(path);
        return Task.CompletedTask;
    }

    public Task<string[]> GetFilesAsync(string directory, string pattern, CancellationToken cancellationToken = default) =>
        Task.FromResult(Directory.Exists(directory)
            ? Directory.GetFiles(directory, pattern)
            : []);
}
