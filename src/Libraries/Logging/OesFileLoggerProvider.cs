using Microsoft.Extensions.Logging;

namespace OpenExamSuite.Logging;

/// <summary>
/// An <see cref="ILoggerProvider"/> that writes <see cref="Microsoft.Extensions.Logging"/>
/// entries to the same log file used by the legacy <see cref="Logger"/>.
/// </summary>
public sealed class OesFileLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new OesFileLogger(categoryName);

    public void Dispose()
    {
    }

    private sealed class OesFileLogger : ILogger
    {
        private readonly string _categoryName;

        public OesFileLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            if (exception != null)
                message += $" - {exception}";

            var line = $"{DateTime.Now:G} - [{logLevel}] {_categoryName}: {message}";
            WriteToLog(line);
        }

        private static void WriteToLog(string message)
        {
            try
            {
                var directory = Path.GetDirectoryName(Logger.LogFilePath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                using var stream = new FileStream(Logger.LogFilePath, FileMode.Append, FileAccess.Write);
                using var writer = new StreamWriter(stream);
                writer.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
