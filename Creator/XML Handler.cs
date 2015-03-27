using System.Collections.Generic;
using System.Xml;
using System.IO;

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
    }
}
