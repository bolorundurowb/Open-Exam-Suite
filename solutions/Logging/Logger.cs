using System;
using System.IO;

namespace Logging
{
    public static class Logger
    {
        private const string LogFileName = "oes-log.log";

        private static readonly string LogFilePath =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}{Path.DirectorySeparatorChar}{LogFileName}";
        
        public static void LogException(Exception exception)
        {
            try
            {
                using (var stream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(stream);
                    streamWriter.WriteLine($"{DateTime.Now.ToString()} - {exception.Message} - {exception.StackTrace}");
                    streamWriter.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}