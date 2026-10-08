using OpenExamSuite.Shared.Utilities;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.Shared.Tests;

public class ExamFormatFixtureTests
{
    private readonly Reader _reader = new();
    private readonly Writer _writer = new();

    [Fact]
    public void FromJsonFile_MinimalFixture_LoadsExpectedExam()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "minimal.json");
        var result = _reader.FromJsonFile(path);

        result.Success.Must().BeTrue();
        var exam = result.Exam;
        exam.Must().NotBeNull();
        exam!.Properties.Title.Must().Be("FixtureExam");
        exam.Properties.Code.Must().Be("FX");
        exam.Properties.HideAnswers.Must().BeFalse();
        exam.Sections.Count.Must().Be(1);
        exam.Sections[0].Title.Must().Be("SectionOne");
        exam.Sections[0].Questions.Count.Must().Be(1);
        exam.Sections[0].Questions[0].Text.Must().Be("Sample question?");
        exam.Sections[0].Questions[0].Answer.Must().Be('A');
        exam.NumberOfQuestions.Must().Be(1);
    }

    [Fact]
    public void RoundTrip_CommittedJsonFixture_PreservesCoreFields()
    {
        var sourcePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "minimal.json");
        var tempPath = Path.Combine(Path.GetTempPath(), $"oes-fixture-{Guid.NewGuid():N}.json");
        try
        {
            var originalResult = _reader.FromJsonFile(sourcePath);
            originalResult.Success.Must().BeTrue();
            var original = originalResult.Exam;
            original.Must().NotBeNull();

            _writer.ToJson(original!, tempPath).Success.Must().BeTrue();

            var roundTrippedResult = _reader.FromJsonFile(tempPath);
            roundTrippedResult.Success.Must().BeTrue();
            var roundTripped = roundTrippedResult.Exam;
            roundTripped.Must().NotBeNull();
            roundTripped!.Properties.Title.Must().Be(original!.Properties.Title);
            roundTripped.Properties.Code.Must().Be(original.Properties.Code);
            roundTripped.Sections.Count.Must().Be(original.Sections.Count);
            roundTripped.NumberOfQuestions.Must().Be(original.NumberOfQuestions);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
