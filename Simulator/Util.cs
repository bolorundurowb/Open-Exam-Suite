using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.IO;
using System.Xml;

namespace Simulator
{
    /// <summary>
    /// The question object class that enables the interaction with questions from the xml.
    /// </summary>
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


        //Properties
        public string SectionTitle { get; set; }

        public int QuestionNumber { get; set; }

        public string QuestionText { get; set; }

        public string QuestionImagePath { get; set; }

        public Dictionary<char, string> QuestionOptions { get; set; }

        public char QuestionAnswer { get; set; }

        public string AnswerExplanation { get; set; }

        public static List<Question> GetQuestions(string ExamFilesFolderPath)
        {
            List<Question> result = new List<Question>();
            string fileName = ExamFilesFolderPath.Replace(Path.GetDirectoryName(ExamFilesFolderPath), "");
            string[] sections = Question.GetSections(Properties.Settings.Default.SelectedSections);
            string xmlFilePath = GlobalPathVariables.GetXmlFilePath(ExamFilesFolderPath);
            XPathDocument doc = new XPathDocument(xmlFilePath);
            XPathNavigator nav = doc.CreateNavigator();
            XmlNamespaceManager nm = new XmlNamespaceManager(nav.NameTable);
            // Compile a standard XPath expression
            XPathExpression expr;
            XPathNodeIterator iterator;
            string version = "";
            expr = nav.Compile("//FileVersion");
            iterator = nav.Select(expr);
            while(iterator.MoveNext())
            {
                version = iterator.Current.Value;
            }
            for (int i = 0; i < sections.Length; i++)
            {
                expr = nav.Compile("//Section[@Title='" + sections[i] + "']/Question");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    Question ques = new Question();
                    ques.SectionTitle = sections[i];
                    ques.QuestionNumber = Convert.ToInt32(iterator.Current.GetAttribute("No", nm.DefaultNamespace));
                    Dictionary<char, string> options = new Dictionary<char, string>();
                    XPathNodeIterator iter = iterator.Current.SelectChildren(XPathNodeType.Element);
                    while (iter.MoveNext())
                    {
                        if (iter.Current.LocalName == "Text")
                        {
                            ques.QuestionText = iter.Current.Value;
                        }
                        else if (iter.Current.LocalName == "Image")
                        {
                            ques.QuestionImagePath = iter.Current.Value;
                        }
                        else if (iter.Current.LocalName == "Answer")
                        {
                            ques.QuestionAnswer = Convert.ToChar(iter.Current.Value);
                        }
                        if (iter.Current.LocalName == "Options")
                        {
                            XPathNodeIterator ite = iter.Current.SelectChildren(XPathNodeType.Element);
                            while (ite.MoveNext())
                            {
                                char option;
                                string optionText;
                                string tempp = ite.Current.GetAttribute("Title", nm.DefaultNamespace);
                                option = Convert.ToChar(tempp);
                                optionText = ite.Current.Value;
                                options.Add(option, optionText);
                            }
                            ques.QuestionOptions = options;
                        }
                        if (version == "1.0")
                        {
                            ques.AnswerExplanation = "Version 1.0 files do not support explanations";
                        }
                        else
                        {
                            if (iterator.Current.LocalName == "AnswerExplanation")
                                ques.AnswerExplanation = iterator.Current.Value;
                        }
                    }
                    result.Add(ques);
                }
            }
            return result;
        }

        /// <summary>
        /// A method to remove the empty strings in the selected strings value
        /// </summary>
        /// <param name="sections">the sring gotten from settings</param>
        /// <returns>a string array containing the title of the selected sections</returns>
        public static string[] GetSections(System.Collections.Specialized.StringCollection sections)
        {
            List<string> redactedSections = new List<string>();
            try
            {
                foreach (var section in sections)
                {
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        redactedSections.Add(section);
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                GlobalPathVariables.WriteError(ex, "Exam UI");
            }
            return redactedSections.ToArray();
        }
    }

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
        public static string GetXmlFilePath(string examFilesFolder)
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
            if (!File.Exists(errorLogPath))
            {
                File.Create(errorLogPath);
            }
            try
            {
                using (StreamWriter sw = File.AppendText(errorLogPath))
                {
                    sw.WriteLine("[" + DateTime.Now.Date + " " + DateTime.Now.ToShortTimeString() + " UTC]: An " + exc.GetType().ToString() + " exception occured in " + sender + "; Message: " + exc.Message + "; InnerException: " + exc.InnerException);
                    sw.WriteLine("\n");
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("An error occured:  \n" + e.Message);
            }
        }
    }
}
