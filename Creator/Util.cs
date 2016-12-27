using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.IO;
using System.Xml;

namespace Creator
{
    internal enum PrintOption
    {
        CurrentQuestion,
        CurrentSection,
        AllQuestions
    };

    public class Question
    {
        /// <summary>
        /// The default constructor to the question class
        /// </summary>
        public Question()
        {
            SectionTitle = "";
            QuestionNumber = 0;
            QuestionText = "";
            QuestionImagePath = null;
            QuestionOptions = new Dictionary<char, string>();
            QuestionAnswer = 'A';
            AnswerExplanation = null;
        }


    #region Properties
        public string SectionTitle { get; set; }

        public int QuestionNumber { get; set; }

        public string QuestionText { get; set; }

        public string QuestionImagePath { get; set; }

        public Dictionary<char, string> QuestionOptions { get; set; }

        public char QuestionAnswer { get; set; }

        public string AnswerExplanation { get; set; }
    #endregion

    }

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
                
        public static void WriteError(Exception exc)
        {
            //Check if log file exists, else create it
            if (!File.Exists(errorLogPath))
            {
                File.Create(errorLogPath);
            }
            try
            {
                using (StreamWriter sw = File.AppendText(errorLogPath))
                {
                    sw.WriteLine("[" + DateTime.Now.Date + " " + DateTime.Now.ToShortTimeString() + " UTC]: An " + exc.GetType().ToString() + " exception occured in Creator UI; Message: " + exc.Message + "; InnerException: " + exc.InnerException);
                    sw.WriteLine("\n");
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error occured: \n" + e.Message);
            }
        }

        public static List<Question> GetListFromDictionaryList(List<KeyValuePair<string,List<Question>>> dictionaryList)
        {
            List<Question> list = new List<Question>();
            foreach(var dictionary in dictionaryList)
            {
                foreach(var question in dictionary.Value)
                {
                    list.Add(question);
                }
            }
            return list;
        }
    }
}
