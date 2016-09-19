using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System.IO;
using System.Drawing;

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
                Instructions = "Goodluck! Make good use of your time.",
                Passmark = 650,
                TimeLimit = 5
            },
            Sections = new System.Collections.Generic.List<Section>
            {
                new Section
                {
                    Title = "Section A",
                    Questions = new System.Collections.Generic.List<Question>
                    {
                        new Question
                        {
                            No = 1,
                            Text = "Question 1",
                            Answer = 'A',
                            Options = new System.Collections.Generic.List<Option>
                            {
                                new Option
                                {
                                    Text = "Option 1",
                                    Alphabet = 'A'
                                },
                                new Option
                                {
                                    Text = "Option 2",
                                    Alphabet = 'B'
                                }
                            },
                            Image = new Bitmap("test.png")
                        },
                        new Question
                        {
                            No = 1,
                            Text = "Question 2",
                            Answer = 'B',
                            Options = new System.Collections.Generic.List<Option>
                            {
                                new Option
                                {
                                    Text = "Option 1",
                                    Alphabet = 'A'
                                },
                                new Option
                                {
                                    Text = "Option 2",
                                    Alphabet = 'B'
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
            Assert.IsInstanceOfType(exam, typeof(Exam));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void NullExamPassed()
        {
            Exam nullExam = null;
            Assert.IsTrue(Helper.WriteExamToFile(nullExam, @"C:\"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyFilePath()
        {
            Assert.IsTrue(Helper.WriteExamToFile(exam, string.Empty));
        }
    }
}
