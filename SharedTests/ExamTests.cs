using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System.IO;

namespace SharedTests
{
    [TestClass]
    public class ExamTests
    {
        Exam exam = new Exam
        {
            Properties = new Properties
            {
                Title = "Test",
                Version = 3,
                Code = "T01",
                Instructions = "",
                Passmark = 10,
                TimeLimit = 5
            },
            Sections = new System.Collections.Generic.List<Section>
            {
                new Section
                {
                    Title = "Section",
                    Questions = new System.Collections.Generic.List<Question>
                    {
                        new Question
                        {
                            No=1,
                            Text = "Question 1",
                            Answer = 'A',
                            Options = new System.Collections.Generic.List<Option>
                            {
                                new Option
                                {
                                    Text = "Option 1",
                                    Alphabet = 'A'
                                }
                            }
                        }
                    }
                }
            }
        };

        [TestMethod]
        public void ExamGetsSerialized()
        {
            string filepath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "test.oef";
            Helper.WriteExamToFile(exam, filepath);
            Assert.IsTrue(File.Exists(filepath));
        }

        [TestMethod]
        public void ExamGetsDeserialized()
        {
            string filepath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "test.oef";
            Exam exam = Helper.GetExamFromFile(filepath);
            Assert.AreEqual(exam, this.exam);
        }
    }
}
