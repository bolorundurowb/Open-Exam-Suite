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

        exam.Verify().NotToBeNull();
        exam!.Properties.Title.Verify().ToBe("FixtureExam");
        exam.Properties.Code.Verify().ToBe("FX");
        exam.Properties.HideAnswers.Verify().ToBeFalse();
        exam.Sections.Count.Verify().ToBe(1);
        exam.Sections[0].Title.Verify().ToBe("SectionOne");
        exam.Sections[0].Questions.Count.Verify().ToBe(1);
        exam.Sections[0].Questions[0].Text.Verify().ToBe("Sample question?");
        exam.Sections[0].Questions[0].Answer.Verify().ToBe('A');
        exam.NumberOfQuestions.Verify().ToBe(1);
    }

    [Fact]
    public void RoundTrip_CommittedJsonFixture_PreservesCoreFields()
    {
        var sourcePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "minimal.json");
        var tempPath = Path.Combine(Path.GetTempPath(), $"oes-fixture-{Guid.NewGuid():N}.json");
        try
        {
            var original = Reader.FromJsonFile(sourcePath);
            original.Verify().NotToBeNull();

            Writer.ToJson(original!, tempPath).Verify().ToBeTrue();

            var roundTripped = Reader.FromJsonFile(tempPath);
            roundTripped.Verify().NotToBeNull();
            roundTripped!.Properties.Title.Verify().ToBe(original!.Properties.Title);
            roundTripped.Properties.Code.Verify().ToBe(original.Properties.Code);
            roundTripped.Sections.Count.Verify().ToBe(original.Sections.Count);
            roundTripped.NumberOfQuestions.Verify().ToBe(original.NumberOfQuestions);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
