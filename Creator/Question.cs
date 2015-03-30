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
    /// <summary>
    /// The question object class that enables the interaction with questions from the xml.
    /// </summary>
    public class Question
    {
        //Variables
        private string sectionTitle;
        private int questionNumber;
        private string questionText;
        private string questionImagePath;
        private Dictionary<char, string> questionOptions;
        private char questionAnswer;

        /// <summary>
        /// The default constructor to the question class
        /// </summary>
        public Question()
        {
            sectionTitle = null;
            questionAnswer = 'A';
            questionImagePath = null;
            questionNumber = 0;
            questionOptions = new Dictionary<char, string>();
            questionText = null;
        }


        //Properties
        public string SectionTitle
        {
            get
            {
                return sectionTitle;
            }
            set
            {
                sectionTitle = value;
            }
        }

        public int QuestionNumber
        {
            get
            {
                return questionNumber;
            }
            set
            {
                questionNumber = value;
            }
        }

        public string QuestionText
        {
            get
            {
                return questionText;
            }
            set
            {
                questionText = value;
            }
        }

        public string QuestionImagePath
        {
            get
            {
                return questionImagePath;
            }
            set
            {
                questionImagePath = value;
            }
        }

        public Dictionary<char,string> QuestionOptions
        {
            get
            {
                return questionOptions;
            }
            set
            {
                questionOptions = value;
            }
        }

        public char QuestionAnswer
        {
            get
            {
                return questionAnswer;
            }
            set
            {
                questionAnswer = value;
            }
        }


        /* public static List<Question> GetQuestions (string OutputPath)
        {
            List<Question> result = new List<Question>();
            string xmlFilePath;
            string fileName = OutputPath.Replace(Path.GetDirectoryName(OutputPath), "");            
            string[] sections = Exam_UI.GetSections(Properties.Settings.Default.SelectedSections);
            xmlFilePath = OutputPath + fileName + ".xml";
            XPathDocument doc = new XPathDocument(xmlFilePath);
            XPathNavigator nav = doc.CreateNavigator();
            XmlNamespaceManager nm = new XmlNamespaceManager(nav.NameTable);
            // Compile a standard XPath expression
            XPathExpression expr;
            for (int i = 0; i < sections.Length; i++)
            {
                expr = nav.Compile("//Section[@Title='" + sections[i] + "']/Question");
                XPathNodeIterator iterator = nav.Select(expr);
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
                    }
                    result.Add(ques);
                }
            }
            return result;
        } */
    }
}
