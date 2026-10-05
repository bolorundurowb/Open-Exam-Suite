using OmniAssert;
using Xunit;

namespace OpenExamSuite.Core.Tests;

public class ExamComposerTests
{
    private readonly ExamComposer _composer = new();

    private static Exam CreateExam()
    {
        return new Exam
        {
            Sections =
            [
                new Section
                {
                    Title = "Section A",
                    Questions =
                    [
                        new Question { No = 1, Text = "A1" },
                        new Question { No = 2, Text = "A2" }
                    ]
                },
                new Section
                {
                    Title = "Section B",
                    Questions =
                    [
                        new Question { No = 1, Text = "B1" },
                        new Question { No = 2, Text = "B2" },
                        new Question { No = 3, Text = "B3" }
                    ]
                }
            ]
        };
    }

    [Fact]
    public void SelectSections_ReturnsSelectedSectionsAndQuestions()
    {
        var exam = CreateExam();

        var result = _composer.SelectSections(exam, exam.Sections.Take(1));

        result.Sections.Count.Verify().ToBe(1);
        result.Questions.Count.Verify().ToBe(2);
        result.Sections[0].Title.Verify().ToBe("Section A");
    }

    [Fact]
    public void SelectFixedQuestions_ExactCount_TakesFullSections()
    {
        var exam = CreateExam();

        var result = _composer.SelectFixedQuestions(exam, 2);

        result.Questions.Count.Verify().ToBe(2);
        result.Sections.Count.Verify().ToBe(1);
    }

    [Fact]
    public void SelectFixedQuestions_PartialSection_ClonesSectionWithTruncatedQuestions()
    {
        var exam = CreateExam();

        var result = _composer.SelectFixedQuestions(exam, 3);

        result.Questions.Count.Verify().ToBe(3);
        result.Sections.Count.Verify().ToBe(2);
        result.Sections[1].Questions.Count.Verify().ToBe(1);
        result.Sections[1].Title.Verify().ToBe("Section B");
        exam.Sections[1].Questions.Count.Verify().ToBe(3);
    }

    [Fact]
    public void SelectFixedQuestions_CountExceedsTotal_TakesAll()
    {
        var exam = CreateExam();

        var result = _composer.SelectFixedQuestions(exam, 100);

        result.Questions.Count.Verify().ToBe(5);
    }

    [Fact]
    public void SelectFixedQuestions_ZeroCount_ReturnsEmpty()
    {
        var exam = CreateExam();

        var result = _composer.SelectFixedQuestions(exam, 0);

        result.Questions.Count.Verify().ToBe(0);
        result.Sections.Count.Verify().ToBe(0);
    }

    [Fact]
    public void SelectFixedQuestions_SectionTotalsMatchQuestionCountForGrading()
    {
        var exam = CreateExam();

        var result = _composer.SelectFixedQuestions(exam, 3);

        result.Sections[0].Questions.Count.Verify().ToBe(2);
        result.Sections[1].Questions.Count.Verify().ToBe(1);
        result.Sections.Sum(s => s.Questions.Count).Verify().ToBe(result.Questions.Count);
    }
}
