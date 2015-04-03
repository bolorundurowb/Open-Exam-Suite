using System;
using System.IO;
using System.Text;

namespace Simulator
{
    /// <summary>
    /// Class that contains fields and methods that enale access to exam innards
    /// </summary>
    public class GlobalPathVariables
    {
        //Path to OES temp folder
        private static string oefTempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Open Exam Files");
        /// <summary>
        /// Creator temp folder
        /// </summary>
        public static string creatorFolderPath = Path.Combine(oefTempFolderPath, "Creator");
        /// <summary>
        /// Simulator temp folder
        /// </summary>
        public static string simulatorFolderPath = Path.Combine(oefTempFolderPath, "Simulator");
        /// <summary>
        /// Error log file path
        /// </summary>
        public static string errorLogPath = Path.Combine(oefTempFolderPath, "Error Log.log");

        /// <summary>
        /// A method to determine the particular folder for an exam file
        /// </summary>
        /// <param name="examTitle">The title of the exam</param>
        /// <returns>A fully qualified folder path</returns>
        public static string GetExamFilesFolder(string examTitle)
        {
            return Path.Combine(simulatorFolderPath, examTitle);
        }

        /// <summary>
        /// Generates the fully qualified path to the exam xml file
        /// </summary>
        /// <param name="examFilesFolder">The fully qualified path to the folder that the exam was extracted to</param>
        /// <returns>a path to the xml file or null if any error occurs</returns>
        public static string GetXmlFilePath (string examFilesFolder)
        {
            string[] temp = Directory.GetFiles(examFilesFolder, "*.xml", SearchOption.TopDirectoryOnly);
            if (temp.Length > 0)
            {
                return temp[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Method to write errors to log file
        /// </summary>
        /// <param name="exc">The exception that occured</param>
        /// <param name="sender">The class that wants to record the error</param>
        public static void WriteError(Exception exc, string sender)
        {
            //Check if log file exists, else create it
            if (!File.Exists(GlobalPathVariables.errorLogPath))
            {
                File.Create(GlobalPathVariables.errorLogPath);
            }
            using (StreamWriter sw = new StreamWriter(GlobalPathVariables.errorLogPath))
            {
                sw.WriteLine("[" + DateTime.UtcNow.Date + " " + DateTime.UtcNow.ToShortTimeString() + " UTC]: An " + exc.GetType().ToString() + " exception occured in " + sender + "; Message: " + exc.Message + "; InnerException: " + exc.InnerException);
                sw.WriteLine("");
            }
        }
    }
}
