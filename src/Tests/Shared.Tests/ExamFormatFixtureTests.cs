using OpenExamSuite.Shared.Utilities;
using Shouldly;
using Xunit;

namespace OpenExamSuite.Shared.Tests;

public class ExamFormatFixtureTests
{
    [Fact]
    public void FromJsonFile_MinimalFixture_LoadsExpectedExam()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "minimal.json");
        var exam = Reader.FromJsonFile(path);

        exam.ShouldNotBeNull();
        exam!.Properties.Title.ShouldBe("FixtureExam");
        exam.Properties.Code.ShouldBe("FX");
        exam.Sections.Count.ShouldBe(1);
        exam.Sections[0].Title.ShouldBe("SectionOne");
        exam.Sections[0].Questions.Count.ShouldBe(1);
        exam.Sections[0].Questions[0].Text.ShouldBe("Sample question?");
        exam.Sections[0].Questions[0].Answer.ShouldBe('A');
        exam.NumberOfQuestions.ShouldBe(1);
    }

    [Fact]
    public void RoundTrip_CommittedJsonFixture_PreservesCoreFields()
    {
        var sourcePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "minimal.json");
        var tempPath = Path.Combine(Path.GetTempPath(), $"oes-fixture-{Guid.NewGuid():N}.json");
        try
        {
            var original = Reader.FromJsonFile(sourcePath);
            original.ShouldNotBeNull();

            Writer.ToJson(original!, tempPath).ShouldBeTrue();

            var roundTripped = Reader.FromJsonFile(tempPath);
            roundTripped.ShouldNotBeNull();
            roundTripped!.Properties.Title.ShouldBe(original.Properties.Title);
            roundTripped.Properties.Code.ShouldBe(original.Properties.Code);
            roundTripped.Sections.Count.ShouldBe(original.Sections.Count);
            roundTripped.NumberOfQuestions.ShouldBe(original.NumberOfQuestions);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
