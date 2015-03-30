using System;
using System.IO;
using System.Text;

namespace Creator
{
    public class GlobalPathVariables
    {
        //Path to OES temp folder
        private static string oefTempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Open Exam Files");
        //Creator temp folder
        public static string creatorFolderPath = Path.Combine(oefTempFolderPath, "Creator");
        //Simulator temp folder
        public static string simulatorFolderPath = Path.Combine(oefTempFolderPath, "Simulator");
        //Error log file path
        public static string errorLogPath = Path.Combine(oefTempFolderPath, "Error Log.log");

        /// <summary>
        /// Method to write errors to log file
        /// </summary>
        /// <param name="exc">The exception that occured</param>
        public static void WriteError (Exception exc)
        {
            //Check if log file exists, else create it
            if (!File.Exists(GlobalPathVariables.errorLogPath))
            {
                File.Create(GlobalPathVariables.errorLogPath);
            }
            using(StreamWriter sw = new StreamWriter(GlobalPathVariables.errorLogPath))
            {
                sw.WriteLine("[" + DateTime.UtcNow.Date + " " + DateTime.UtcNow.ToShortTimeString() + " UTC]: An " + exc.GetType().ToString() + " exception occured in " + exc.Source + "; Message: " + exc.Message + "; InnerException: " + exc.InnerException);
                sw.WriteLine("");
            }
        }
    }
}
