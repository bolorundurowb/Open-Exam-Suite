using System.Drawing;
using OpenExamSuite.Shared.Utilities;
using OmniAssert;
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

        result.Verify().ToBeTrue();
        File.Exists(_testOefPath).Verify().ToBeTrue();
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

        Action act = () => Writer.ToOef(nullExam!, _testOefPath);
        act.Throws<ArgumentNullException>();
    }

    [Fact]
    public void ToOef_EmptyFilePath_ThrowsArgumentException()
    {
        Action act = () => Writer.ToOef(_exam, string.Empty);
        act.Throws<ArgumentException>();
    }

    [Fact]
    public void FromOefFile_CorruptFile_ThrowsException()
    {
        File.WriteAllText(_testOefPath, "Not a valid format at all");

        Action act = () => Reader.FromOefFile(_testOefPath, true);
        act.Throws<Exception>().WithMessage("Unsupported or corrupted .oef file format.");
    }

    [Fact]
    public void ToJson_ValidExam_SerializesCorrectly()
    {
        var result = Writer.ToJson(_exam, _testJsonPath);

        result.Verify().ToBeTrue();
        File.Exists(_testJsonPath).Verify().ToBeTrue();
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

        result.Verify().ToBeTrue();
        File.Exists(_testXmlPath).Verify().ToBeTrue();
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
        actual.Verify().NotToBeNull();
        expected.Verify().NotToBeNull();
        ArgumentNullException.ThrowIfNull(actual);
        ArgumentNullException.ThrowIfNull(expected);

        actual.Properties.Title.Verify().ToBe(expected.Properties.Title);
        actual.Properties.Code.Verify().ToBe(expected.Properties.Code);
        actual.Properties.Version.Verify().ToBe(expected.Properties.Version);
        actual.Properties.Passmark.Verify().ToBe(expected.Properties.Passmark);
        actual.Properties.TimeLimit.Verify().ToBe(expected.Properties.TimeLimit);
        actual.Properties.Instructions.Verify().ToBe(expected.Properties.Instructions);
        if (expected.Properties.HideAnswers)
            actual.Properties.HideAnswers.Verify().ToBeTrue();
        else
            actual.Properties.HideAnswers.Verify().ToBeFalse();

        actual.Sections.Count.Verify().ToBe(expected.Sections.Count);

        for (var i = 0; i < expected.Sections.Count; i++)
        {
            var expectedSection = expected.Sections[i];
            var actualSection = actual.Sections[i];

            actualSection.Title.Verify().ToBe(expectedSection.Title);
            actualSection.Questions.Count.Verify().ToBe(expectedSection.Questions.Count);

            for (var j = 0; j < expectedSection.Questions.Count; j++)
            {
                var expectedQuestion = expectedSection.Questions[j];
                var actualQuestion = actualSection.Questions[j];

                actualQuestion.No.Verify().ToBe(expectedQuestion.No);
                actualQuestion.Text.Verify().ToBe(expectedQuestion.Text);
                actualQuestion.Answer.Verify().ToBe(expectedQuestion.Answer);
                if (expectedQuestion.IsMultipleChoice)
                    actualQuestion.IsMultipleChoice.Verify().ToBeTrue();
                else
                    actualQuestion.IsMultipleChoice.Verify().ToBeFalse();
                actualQuestion.Explanation.Verify().ToBe(expectedQuestion.Explanation);
                actualQuestion.Answers.SequenceEqual(expectedQuestion.Answers).Verify().ToBeTrue();

                if (expectedQuestion.Image != null)
                {
                    actualQuestion.Image.Verify().NotToBeNull();
                    actualQuestion.Image!.Width.Verify().ToBe(expectedQuestion.Image.Width);
                    actualQuestion.Image.Height.Verify().ToBe(expectedQuestion.Image.Height);
                }
                else
                {
                    actualQuestion.Image.Verify().ToBeNull();
                }

                actualQuestion.Options.Count.Verify().ToBe(expectedQuestion.Options.Count);
                for (var k = 0; k < expectedQuestion.Options.Count; k++)
                {
                    actualQuestion.Options[k].Alphabet.Verify().ToBe(expectedQuestion.Options[k].Alphabet);
                    actualQuestion.Options[k].Text.Verify().ToBe(expectedQuestion.Options[k].Text);
                }
            }
        }
    }
}