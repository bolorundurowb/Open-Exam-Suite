using System.IO.Pipes;
using System.Text;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Keeps one Simulator per user. A second process forwards the requested file to the running
/// instance over a named pipe and exits, instead of showing a message and dropping the path.
/// </summary>
public sealed class SingleInstanceCoordinator : IDisposable
{
    private const string Magic = "OES-SIMULATOR-1";
    private const int MaxPaths = 32;
    private const int MaxLineLength = 4096;

    private readonly string _pipeName;
    private readonly CancellationTokenSource _cts = new();
    private Task? _loop;

    public SingleInstanceCoordinator(string? pipeName = null)
    {
        _pipeName = pipeName ?? DefaultPipeName;
    }

    /// <summary>Raised on a background thread with the paths another launch asked to open. May be empty.</summary>
    public event Action<IReadOnlyList<string>>? Activated;

    public static string DefaultPipeName => "OpenExamSuite.Simulator." + Sanitize(Environment.UserName);

    /// <summary>
    /// Sends <paramref name="paths"/> to a running instance. Returns false when none is listening.
    /// </summary>
    public static bool TryForward(IReadOnlyList<string> paths, string? pipeName = null, int timeoutMs = 600)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", pipeName ?? DefaultPipeName, PipeDirection.Out, PipeOptions.CurrentUserOnly);
            client.Connect(timeoutMs);
            using var writer = new StreamWriter(client, new UTF8Encoding(false)) { AutoFlush = true };
            writer.WriteLine(Magic);
            foreach (var path in paths.Take(MaxPaths))
                writer.WriteLine(path);

            return true;
        }
        catch (Exception ex) when (ex is TimeoutException or IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>
    /// Becomes the primary instance. Returns false when another process already owns the pipe.
    /// </summary>
    public bool TryStart()
    {
        NamedPipeServerStream first;
        try
        {
            first = CreateServer();
        }
        catch (IOException)
        {
            return false;
        }

        _loop = Task.Run(() => ListenAsync(first));
        return true;
    }

    public void Dispose()
    {
        _cts.Cancel();
        try
        {
            _loop?.Wait(TimeSpan.FromSeconds(1));
        }
        catch (AggregateException)
        {
            // The listener only throws when cancelled.
        }

        _cts.Dispose();
    }

    internal static IReadOnlyList<string> ParsePaths(IEnumerable<string?> lines)
    {
        var result = new List<string>();
        var first = true;
        foreach (var line in lines)
        {
            if (first)
            {
                first = false;
                if (line != Magic)
                    return [];

                continue;
            }

            if (!string.IsNullOrWhiteSpace(line) && line.Length <= MaxLineLength && result.Count < MaxPaths)
                result.Add(line);
        }

        return result;
    }

    private NamedPipeServerStream CreateServer() =>
        new(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);

    private async Task ListenAsync(NamedPipeServerStream server)
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await server.WaitForConnectionAsync(_cts.Token);

                // A client that connects and then stalls must not block later launches.
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
                timeout.CancelAfter(TimeSpan.FromSeconds(3));

                var lines = new List<string?>();
                using (var reader = new StreamReader(server, Encoding.UTF8, false, 1024, leaveOpen: true))
                {
                    string? line;
                    while (lines.Count <= MaxPaths && (line = await reader.ReadLineAsync(timeout.Token)) != null)
                        lines.Add(line);
                }

                var paths = ParsePaths(lines);
                if (lines.Count > 0 && lines[0] == Magic)
                    Activated?.Invoke(paths);
            }
            catch (OperationCanceledException)
            {
                if (_cts.IsCancellationRequested)
                    break;
            }
            catch (Exception)
            {
                // A misbehaving client must not take the listener down.
            }
            finally
            {
                server.Dispose();
            }

            if (_cts.IsCancellationRequested)
                break;

            var next = await RecreateServerAsync();
            if (next == null)
                break;

            server = next;
        }
    }

    private async Task<NamedPipeServerStream?> RecreateServerAsync()
    {
        for (var attempt = 0; attempt < 20 && !_cts.IsCancellationRequested; attempt++)
        {
            try
            {
                return CreateServer();
            }
            catch (IOException)
            {
                await Task.Delay(50);
            }
        }

        return null;
    }

    private static string Sanitize(string value) =>
        new(value.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
}
