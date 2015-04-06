using System;
using System.IO;
using System.Xml;
using Ionic.Zip;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Creator
{
    public class XML_Handler
    {
        /// <summary>
        /// Gets the questions sorted into sections
        /// </summary>
        /// <param name="sectionTitles">The list  of sections the questions are to be sorted into</param>
        /// <param name="questionList">the complete list of all questions</param>
        /// <returns>A dictionary of the sorted questions ready for writing</returns>
        public static Dictionary<string, List<Question>> ConvertListToFormat(string[] sectionTitles, List<Question> questionList)
        {
            Dictionary<string, List<Question>> result = new Dictionary<string, List<Question>>();
            foreach (string section in sectionTitles)
            {
                var sectionQuestionList = questionList.FindAll(s => s.SectionTitle == section);
                result.Add(section, sectionQuestionList);
            }
            return result;
        }

        /// <summary>
        /// Creates an XmlDocument from the question list
        /// </summary>
        /// <param name="questionList">The dictionary containing questions sorted by section</param>
        /// <returns>An XMLDocument containing the whole exam</returns>
        public static XmlDocument WriteDictionaryToXMLDocument(Dictionary<string, List<Question>> questionList)
        {
            XmlDocument exam = new XmlDocument();

            XmlNode rootNode = exam.CreateElement("OpenExamDocument");
            exam.AppendChild(rootNode);

            XmlNode fileVersion = exam.CreateElement("FileVersion");
            fileVersion.InnerText = "1.0";
            rootNode.AppendChild(fileVersion);

            XmlNode comment = exam.CreateComment("This document contains the details of an Open Exam exam, please do not modify!");
            rootNode.AppendChild(comment);

            //Exam Details
            XmlNode examDetails = exam.CreateElement("ExamDetails");
            rootNode.AppendChild(examDetails);

            XmlNode examTitle = exam.CreateElement("ExamTitle");
            examTitle.InnerText = Properties.Settings.Default.ExamTitle;
            examDetails.AppendChild(examTitle);

            XmlNode timeAllowed = exam.CreateElement("TimeAllowed");
            timeAllowed.InnerText = Properties.Settings.Default.TimeAllowed.ToString();
            examDetails.AppendChild(timeAllowed);

            XmlNode passingScore = exam.CreateElement("PassingScore");
            passingScore.InnerText = Properties.Settings.Default.PassingScore.ToString();
            examDetails.AppendChild(passingScore);

            XmlNode examCode = exam.CreateElement("ExamCode");
            examCode.InnerText = Properties.Settings.Default.ExamCode;
            examDetails.AppendChild(examCode);

            XmlNode examInstructions = exam.CreateElement("ExamInstructions");
            examInstructions.InnerText = Properties.Settings.Default.ExamInstructions;
            examDetails.AppendChild(examInstructions);

            //Questions Group
            XmlNode questions = exam.CreateElement("Questions");
            rootNode.AppendChild(questions);

            //Sections and Questions
            foreach( var sectionAndQuestions in questionList)
            {
                //Sections
                XmlNode sectionNode = exam.CreateElement("Section");
                XmlAttribute sectionAttribute = exam.CreateAttribute("Title");
                sectionAttribute.Value = sectionAndQuestions.Key;
                sectionNode.Attributes.Append(sectionAttribute);
                questions.AppendChild(sectionNode);

                //Questions
                foreach(Question quest in sectionAndQuestions.Value)
                {
                    XmlNode questionNode = exam.CreateElement("Question");
                    XmlAttribute attrib = exam.CreateAttribute("No");
                    attrib.Value = quest.QuestionNumber.ToString();
                    questionNode.Attributes.Append(attrib);
                    sectionNode.AppendChild(questionNode);

                    XmlNode questionText = exam.CreateElement("Text");
                    questionText.InnerText = quest.QuestionText;
                    questionNode.AppendChild(questionText);

                    XmlNode questionImage = exam.CreateElement("Image");
                    questionImage.InnerText = Path.GetFileName(quest.QuestionImagePath);
                    questionNode.AppendChild(questionImage);

                    XmlNode questionOptions = exam.CreateElement("Options");
                    questionNode.AppendChild(questionOptions);

                    foreach(var option in quest.QuestionOptions)
                    {
                        XmlNode questionOption = exam.CreateElement("Option");
                        XmlAttribute optionAttribute = exam.CreateAttribute("Title");
                        optionAttribute.Value = option.Key.ToString();
                        questionOption.Attributes.Append(optionAttribute);
                        questionOption.InnerText = option.Value;
                        questionOptions.AppendChild(questionOption);
                    }

                    XmlNode questionAnswer = exam.CreateElement("Answer");
                    questionAnswer.InnerText = quest.QuestionAnswer.ToString();
                    questionNode.AppendChild(questionAnswer);
                }
            }

            //return complete exam document
            return exam;
        }

        /// <summary>
        /// Method to read questions from an xml file
        /// </summary>
        /// <param name="xmlFilePath">Fully qualified path to xml file</param>
        /// <returns>A specially formatted dictionary containing the questions in the xml file</returns>
        public static Dictionary<string, List<Question>> ReadExamToFormat(string xmlFilePath)
        {
            XPathDocument doc = new XPathDocument(xmlFilePath);
            XPathNavigator nav = doc.CreateNavigator();
            XmlNamespaceManager nm = new XmlNamespaceManager(nav.NameTable);
            Dictionary<string, List<Question>> formattedQuestionList = new Dictionary<string, List<Question>>();
            XPathExpression expression = nav.Compile("//Section");
            XPathNodeIterator rootIterator = nav.Select(expression);
            while (rootIterator.MoveNext())
            {
                string sectionTitle = rootIterator.Current.GetAttribute("Title", nm.DefaultNamespace);
                List<Question> sectionQuestions = new List<Question>();
                XPathNodeIterator subIterator = rootIterator.Current.SelectChildren(XPathNodeType.Element);
                while (subIterator.MoveNext())
                {
                    Question ques = new Question();
                    ques.SectionTitle = sectionTitle;
                    ques.QuestionNumber = Convert.ToInt32(subIterator.Current.GetAttribute("No", nm.DefaultNamespace));
                    Dictionary<char, string> options = new Dictionary<char, string>();
                    XPathNodeIterator iter = subIterator.Current.SelectChildren(XPathNodeType.Element);
                    while (iter.MoveNext())
                    {
                        if (iter.Current.LocalName == "Text")
                        {
                            ques.QuestionText = iter.Current.Value;
                        }
                        else if (iter.Current.LocalName == "Image")
                        {
                            if (string.IsNullOrWhiteSpace(iter.Current.Value))
                            {
                                ques.QuestionImagePath = null;
                            }
                            else
                            {
                                ques.QuestionImagePath = Path.Combine(Path.GetDirectoryName(xmlFilePath), iter.Current.Value);
                            }
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
                                option = Convert.ToChar(ite.Current.GetAttribute("Title", nm.DefaultNamespace));
                                optionText = ite.Current.Value;
                                options.Add(option, optionText);
                            }
                            ques.QuestionOptions = options;
                        }
                    }
                    sectionQuestions.Add(ques);
                }
                formattedQuestionList.Add(sectionTitle, sectionQuestions);
            }
            return formattedQuestionList;
        }

        public static void ExtractExamToFolder(string examPath)
        {
            if (!Directory.Exists(GlobalPathVariables.GetExamFilesFolder(Path.GetFileNameWithoutExtension(examPath))))
            {
                Directory.CreateDirectory(GlobalPathVariables.GetExamFilesFolder(Path.GetFileNameWithoutExtension(examPath)));
            }
            using (ZipFile zip = new ZipFile(examPath))
            {
                zip.ExtractAll(GlobalPathVariables.GetExamFilesFolder(Path.GetFileNameWithoutExtension(examPath)), ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
