using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using Shared.Util;
using Xunit;

namespace Shared.Tests
{
    public class ExamTests
    {
        private readonly Exam _exam;

        public ExamTests()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream("Shared.Tests.test.png"))
            {
                var image = (Bitmap) Image.FromStream(stream);
                _exam = new Exam
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
                                    Image = image
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
            }
        }

        [Fact]
        public void ExamGetsSerialized()
        {
            var filepath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "test.oef";
            Reader.WriteExamToOefFile(_exam, filepath, true);
            Assert.Equal(true, File.Exists(filepath));
        }

        [Fact]
        public void ExamGetsDeserialized()
        {
            var filepath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "test.oef";
            var exam = Reader.FromOefFile(filepath, true);
            Assert.NotNull(exam);
        }

        [Fact]
        public void NullExamPassed()
        {
            Exam nullExam = null;
            Assert.Throws<NullReferenceException>(() => { Reader.WriteExamToOefFile(nullExam, @"C:\"); });
        }

        [Fact]
        public void EmptyFilePath()
        {
            Assert.Throws<ArgumentException>(() => { Reader.WriteExamToOefFile(_exam, string.Empty); });
        }
    }
}