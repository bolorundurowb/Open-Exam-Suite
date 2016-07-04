using Ionic.Zip;
using Shared;
using System;
using System.Drawing;
using System.IO;
using System.Xml.XPath;

namespace Exam_Upgrade_Tool
{
    public static class Helper
    {
        #region Global Variables
        private static string appFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + "Open Exam Suite";
        #endregion

        public static string ExtractExamToFolder(string examFilePath)
        {
            try
            {
                string foldername = Path.GetFileNameWithoutExtension(examFilePath);
                string extractionFolderPath = Path.Combine(appFolderPath, foldername);
                if (Directory.Exists(extractionFolderPath))
                {
                    Directory.Delete(extractionFolderPath, true);
                    Directory.CreateDirectory(extractionFolderPath);
                }
                else
                {
                    Directory.CreateDirectory(extractionFolderPath);
                }
                using (ZipFile zip = new ZipFile(examFilePath))
                {
                    zip.ExtractAll(extractionFolderPath, ExtractExistingFileAction.OverwriteSilently);
                }
                return extractionFolderPath;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static string GetXmlFilePath(string examFolderPath)
        {
            string[] temp = Directory.GetFiles(examFolderPath, "*.xml", SearchOption.TopDirectoryOnly);
            string xmlFilePath = string.Empty;
            if (temp.Length > 0)
                xmlFilePath = temp[0];
            return xmlFilePath;
        }

        public static Exam CreateExamFromXml(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath))
            {
                throw new ArgumentException("Empty xml file path");
            }
            else
            {
                Exam exam = new Exam();
                //
                XPathDocument doc = new XPathDocument(xmlFilePath);
                XPathNavigator navigator = doc.CreateNavigator();
                XPathExpression expression = navigator.Compile("//FileVersion");
                XPathNodeIterator iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    exam.Properties.Version = (int)float.Parse(iterator.Current.Value);
                }
                expression = navigator.Compile("//ExamTitle");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    exam.Properties.Title = iterator.Current.Value;
                }
                expression = navigator.Compile("//TimeAllowed");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    exam.Properties.TimeLimit = Convert.ToInt32(iterator.Current.Value);
                }
                expression = navigator.Compile("//PassingScore");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    exam.Properties.Passmark = Convert.ToInt32(iterator.Current.Value);
                }
                expression = navigator.Compile("//ExamCode");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    exam.Properties.Code = iterator.Current.Value;
                }
                expression = navigator.Compile("//ExamInstructions");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    exam.Properties.Instructions = iterator.Current.Value;
                }
                //
                expression = navigator.Compile("//Section");
                iterator = navigator.Select(expression);
                while (iterator.MoveNext())
                {
                    Section section = new Section();
                    section.Title = iterator.Current.GetAttribute("Title", "");
                    //
                    XPathNodeIterator subIterator = iterator.Current.SelectChildren(XPathNodeType.Element);
                    while (subIterator.MoveNext())
                    {
                        Question question = new Question();
                        question.No = Convert.ToInt32(subIterator.Current.GetAttribute("No", ""));
                        //
                        XPathNodeIterator subIter = subIterator.Current.SelectChildren(XPathNodeType.Element);
                        while (subIter.MoveNext())
                        {
                            if (subIter.Current.LocalName == "Text")
                            {
                                question.Text = subIter.Current.Value;
                            }
                            else if (subIter.Current.LocalName == "Image")
                            {
                                string examFolderPath = Path.GetDirectoryName(xmlFilePath);
                                try
                                {
                                    question.Image = new Bitmap(Path.Combine(examFolderPath, subIter.Current.Value));
                                }
                                catch (ArgumentException) { }
                            }
                            else if (subIter.Current.LocalName == "Answer")
                            {
                                question.Answer = Convert.ToChar(subIter.Current.Value);
                            }
                            //
                            if (subIter.Current.LocalName == "Options")
                            {
                                XPathNodeIterator ite = subIter.Current.SelectChildren(XPathNodeType.Element);
                                while (ite.MoveNext())
                                {
                                    Option option = new Option();
                                    string tempp = ite.Current.GetAttribute("Title", "");
                                    option.Alphabet = Convert.ToChar(tempp);
                                    option.Text = ite.Current.Value;
                                    question.Options.Add(option);
                                }
                            }
                            if (exam.Properties.Version == 1.0F)
                            {
                                question.Explanation = "Version 1.0 files do not support explanations";
                            }
                            else
                            {
                                if (iterator.Current.LocalName == "AnswerExplanation")
                                    question.Explanation = iterator.Current.Value;
                            }
                        }
                        section.Questions.Add(question);
                    }
                    exam.Sections.Add(section);
                }
                return exam;
            }
        }
    }
}
