using System;
using System.Drawing;
using System.IO;
using OpenExamSuite.Shared.Utilities;
using Shouldly;
using Xunit;

namespace OpenExamSuite.Shared.Tests;

public class ExamTests : IDisposable
{
    private readonly Exam _exam;
    private readonly string _testOefPath;
    private readonly string _testJsonPath;
    private readonly string _testXmlPath;

    public ExamTests()
    {
        _testOefPath = Path.Combine(Environment.CurrentDirectory, "test.oef");
        _testJsonPath = Path.Combine(Environment.CurrentDirectory, "test.json");
        _testXmlPath = Path.Combine(Environment.CurrentDirectory, "test.xml");
        using var fileStream = new FileStream("./Resources/ExamTestImage.png", FileMode.Open);
        var image = (Bitmap)Image.FromStream(fileStream);
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
            Sections =
            [
                new Section
                {
                    Title = "Section A",
                    Questions =
                    [
                        new Question
                        {
                            No = 1,
                            Text = "Question 1",
                            Answer = 'A',
                            Options =
                            [
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
                            ],
                            Image = image
                        },

                        new Question
                        {
                            No = 1,
                            Text = "Question 2",
                            Answer = 'B',
                            Options =
                            [
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
                            ]
                        }
                    ]
                }
            ]
        };
    }

    public void Dispose()
    {
        if (File.Exists(_testOefPath))
            File.Delete(_testOefPath);

        if (File.Exists(_testJsonPath))
            File.Delete(_testJsonPath);

        if (File.Exists(_testXmlPath))
            File.Delete(_testXmlPath);
    }

    [Fact]
    public void ToOef_ValidExam_SerializesCorrectly()
    {
        var result = Writer.ToOef(_exam, _testOefPath, true);

        result.ShouldBeTrue();
        File.Exists(_testOefPath).ShouldBeTrue();
    }

    [Fact]
    public void FromOefFile_ValidFile_DeserializesCorrectly()
    {
        Writer.ToOef(_exam, _testOefPath, true);

        var exam = Reader.FromOefFile(_testOefPath, true);

        exam.ShouldNotBeNull();
        exam.Properties.Title.ShouldBe(_exam.Properties.Title);
        exam.Sections.Count.ShouldBe(_exam.Sections.Count);
        exam.Sections[0].Questions[0].Image.ShouldNotBeNull();
    }

    [Fact]
    public void ToOef_NullExam_ThrowsArgumentNullException()
    {
        Exam? nullExam = null;

        Should.Throw<ArgumentNullException>(() => Writer.ToOef(nullExam!, _testOefPath));
    }

    [Fact]
    public void ToOef_EmptyFilePath_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => Writer.ToOef(_exam, string.Empty));
    }

    [Fact]
    public void FromOefFile_CorruptFile_ThrowsException()
    {
        File.WriteAllText(_testOefPath, "Not a valid format at all");

        var ex = Should.Throw<Exception>(() => Reader.FromOefFile(_testOefPath, true));
        ex.Message.ShouldBe("Unsupported or corrupted .oef file format.");
    }

    [Fact]
    public void ToJson_ValidExam_SerializesCorrectly()
    {
        var result = Writer.ToJson(_exam, _testJsonPath);

        result.ShouldBeTrue();
        File.Exists(_testJsonPath).ShouldBeTrue();
    }

    [Fact]
    public void FromJsonFile_ValidFile_DeserializesCorrectly()
    {
        Writer.ToJson(_exam, _testJsonPath);

        var exam = Reader.FromJsonFile(_testJsonPath);

        exam.ShouldNotBeNull();
        exam.Properties.Title.ShouldBe(_exam.Properties.Title);
    }

    [Fact]
    public void ToXml_ValidExam_SerializesCorrectly()
    {
        var result = Writer.ToXml(_exam, _testXmlPath);

        result.ShouldBeTrue();
        File.Exists(_testXmlPath).ShouldBeTrue();
    }

    [Fact]
    public void FromXmlFile_ValidFile_DeserializesCorrectly()
    {
        Writer.ToXml(_exam, _testXmlPath);

        var exam = Reader.FromXmlFile(_testXmlPath);

        exam.ShouldNotBeNull();
        exam.Properties.Title.ShouldBe(_exam.Properties.Title);
    }
}