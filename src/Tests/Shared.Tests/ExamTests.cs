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
        byte[] imageBytes;
        using (var ms = new MemoryStream())
        {
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            imageBytes = ms.ToArray();
        }
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

        result.Must().BeTrue();
        File.Exists(_testOefPath).Must().BeTrue();
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

        result.Must().BeTrue();
        File.Exists(_testJsonPath).Must().BeTrue();
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

        result.Must().BeTrue();
        File.Exists(_testXmlPath).Must().BeTrue();
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
        actual.Must().NotBeNull();
        expected.Must().NotBeNull();
        ArgumentNullException.ThrowIfNull(actual);
        ArgumentNullException.ThrowIfNull(expected);

        actual.Properties.Title.Must().Be(expected.Properties.Title);
        actual.Properties.Code.Must().Be(expected.Properties.Code);
        actual.Properties.Version.Must().Be(expected.Properties.Version);
        actual.Properties.Passmark.Must().Be(expected.Properties.Passmark);
        actual.Properties.TimeLimit.Must().Be(expected.Properties.TimeLimit);
        actual.Properties.Instructions.Must().Be(expected.Properties.Instructions);
        if (expected.Properties.HideAnswers)
            actual.Properties.HideAnswers.Must().BeTrue();
        else
            actual.Properties.HideAnswers.Must().BeFalse();

        actual.Sections.Count.Must().Be(expected.Sections.Count);

        for (var i = 0; i < expected.Sections.Count; i++)
        {
            var expectedSection = expected.Sections[i];
            var actualSection = actual.Sections[i];

            actualSection.Title.Must().Be(expectedSection.Title);
            actualSection.Questions.Count.Must().Be(expectedSection.Questions.Count);

            for (var j = 0; j < expectedSection.Questions.Count; j++)
            {
                var expectedQuestion = expectedSection.Questions[j];
                var actualQuestion = actualSection.Questions[j];

                actualQuestion.No.Must().Be(expectedQuestion.No);
                actualQuestion.Text.Must().Be(expectedQuestion.Text);
                actualQuestion.Answer.Must().Be(expectedQuestion.Answer);
                if (expectedQuestion.IsMultipleChoice)
                    actualQuestion.IsMultipleChoice.Must().BeTrue();
                else
                    actualQuestion.IsMultipleChoice.Must().BeFalse();
                actualQuestion.Explanation.Must().Be(expectedQuestion.Explanation);
                actualQuestion.Answers.SequenceEqual(expectedQuestion.Answers).Must().BeTrue();

                if (expectedQuestion.ImageData != null)
                {
                    Xunit.Assert.NotNull(actualQuestion.ImageData);
                    var actualImageData = actualQuestion.ImageData;
                    using var expectedBitmap = new Bitmap(new MemoryStream(expectedQuestion.ImageData));
                    using var actualBitmap = new Bitmap(new MemoryStream(actualImageData));
                    actualBitmap.Width.Must().Be(expectedBitmap.Width);
                    actualBitmap.Height.Must().Be(expectedBitmap.Height);
                }
                else
                {
                    Xunit.Assert.Null(actualQuestion.ImageData);
                }

                actualQuestion.Options.Count.Must().Be(expectedQuestion.Options.Count);
                for (var k = 0; k < expectedQuestion.Options.Count; k++)
                {
                    actualQuestion.Options[k].Alphabet.Must().Be(expectedQuestion.Options[k].Alphabet);
                    actualQuestion.Options[k].Text.Must().Be(expectedQuestion.Options[k].Text);
                }
            }
        }
    }
}