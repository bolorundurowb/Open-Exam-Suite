namespace Logging;

public static class Logger
{
    private const string LogFileName = "oes-log.log";

    private static readonly string LogDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenExamSuite");

    private static readonly string LogFilePath = Path.Combine(LogDirectory, LogFileName);

    public static void LogException(Exception exception)
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            using var stream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write);
            using var streamWriter = new StreamWriter(stream);
            streamWriter.WriteLine($"{DateTime.Now:G} - {exception.Message} - {exception.StackTrace}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}