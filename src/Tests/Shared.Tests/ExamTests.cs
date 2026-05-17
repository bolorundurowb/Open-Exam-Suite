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
        var imageBytes = File.ReadAllBytes("./Resources/ExamTestImage.png");
        _exam = new Exam
        {
            Properties = new Properties
            {
                Title = "Test",
                Version = 3,
                Code = "T01",
                Instructions = "Goodluck! Make good use of your time.",
                Passmark = 650,
                TimeLimit = 5,
                HideAnswers = true
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
                            ImageData = imageBytes
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

        VerifyExamsMatch(exam, _exam);
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

        VerifyExamsMatch(exam, _exam);
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

        VerifyExamsMatch(exam, _exam);
    }

    private static void VerifyExamsMatch(Exam? actual, Exam? expected)
    {
        actual.ShouldNotBeNull();
        expected.ShouldNotBeNull();

        actual.Properties.Title.ShouldBe(expected.Properties.Title);
        actual.Properties.Code.ShouldBe(expected.Properties.Code);
        actual.Properties.Version.ShouldBe(expected.Properties.Version);
        actual.Properties.Passmark.ShouldBe(expected.Properties.Passmark);
        actual.Properties.TimeLimit.ShouldBe(expected.Properties.TimeLimit);
        actual.Properties.Instructions.ShouldBe(expected.Properties.Instructions);
        actual.Properties.HideAnswers.ShouldBe(expected.Properties.HideAnswers);

        actual.Sections.Count.ShouldBe(expected.Sections.Count);

        for (var i = 0; i < expected.Sections.Count; i++)
        {
            var expectedSection = expected.Sections[i];
            var actualSection = actual.Sections[i];

            actualSection.Title.ShouldBe(expectedSection.Title);
            actualSection.Questions.Count.ShouldBe(expectedSection.Questions.Count);

            for (var j = 0; j < expectedSection.Questions.Count; j++)
            {
                var expectedQuestion = expectedSection.Questions[j];
                var actualQuestion = actualSection.Questions[j];

                actualQuestion.No.ShouldBe(expectedQuestion.No);
                actualQuestion.Text.ShouldBe(expectedQuestion.Text);
                actualQuestion.Answer.ShouldBe(expectedQuestion.Answer);
                actualQuestion.IsMultipleChoice.ShouldBe(expectedQuestion.IsMultipleChoice);
                actualQuestion.Explanation.ShouldBe(expectedQuestion.Explanation);
                actualQuestion.Answers.ShouldBe(expectedQuestion.Answers);

                if (expectedQuestion.ImageData is { Length: > 0 })
                {
                    actualQuestion.ImageData.ShouldNotBeNull();
                    actualQuestion.ImageData.Length.ShouldBe(expectedQuestion.ImageData.Length);
                }
                else
                {
                    actualQuestion.ImageData.ShouldBeNull();
                }

                actualQuestion.Options.Count.ShouldBe(expectedQuestion.Options.Count);
                for (var k = 0; k < expectedQuestion.Options.Count; k++)
                {
                    actualQuestion.Options[k].Alphabet.ShouldBe(expectedQuestion.Options[k].Alphabet);
                    actualQuestion.Options[k].Text.ShouldBe(expectedQuestion.Options[k].Text);
                }
            }
        }
    }
}