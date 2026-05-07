namespace OpenExamSuite.Logging;

public static class Logger
{
    private const string LogFileName = "oes-log.log";

    private static readonly string LogDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenExamSuite");

    private static readonly string LogFilePath = Path.Combine(LogDirectory, LogFileName);

    public static void LogException(Exception exception) =>
        WriteToLog($"{exception.Message} - {exception.StackTrace}");

    public static void Log(string message) => WriteToLog(message);

    private static void WriteToLog(string message)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);
            using var stream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write);
            using var writer = new StreamWriter(stream);
            writer.WriteLine($"{DateTime.Now:G} - {message}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}