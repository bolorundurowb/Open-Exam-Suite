using OpenExamSuite.Shared.Utilities;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.Shared.Tests;

public class ExamFormatFixtureTests
{
    [Fact]
    public void FromJsonFile_MinimalFixture_LoadsExpectedExam()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "minimal.json");
        var exam = Reader.FromJsonFile(path);

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
            var original = Reader.FromJsonFile(sourcePath);
            original.Must().NotBeNull();

            Writer.ToJson(original!, tempPath).Must().BeTrue();

            var roundTripped = Reader.FromJsonFile(tempPath);
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
