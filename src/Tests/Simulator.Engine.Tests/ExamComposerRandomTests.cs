using System.Linq;
using OmniAssert;
using OpenExamSuite.Shared.Services;
using Xunit;

namespace OpenExamSuite.Simulator.Engine.Tests;

public class ExamComposerRandomTests
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
                        new Question { No = 3, Text = "B1" },
                        new Question { No = 4, Text = "B2" },
                        new Question { No = 5, Text = "B3" }
                    ]
                }
            ]
        };
    }

    [Fact]
    public void SelectRandomQuestions_Count_ReturnsExactlyThatMany()
    {
        var exam = CreateExam();

        var result = _composer.SelectRandomQuestions(exam, 3, seed: 42);

        result.Questions.Count.Must().Be(3);
        result.Sections.Sum(s => s.Questions.Count).Must().Be(3);
    }

    [Fact]
    public void SelectRandomQuestions_CountExceedsTotal_ClampedToTotal()
    {
        var exam = CreateExam();

        var result = _composer.SelectRandomQuestions(exam, 100, seed: 42);

        result.Questions.Count.Must().Be(5);
    }

    [Fact]
    public void SelectRandomQuestions_DifferentSeeds_Differ()
    {
        var exam = CreateExam();

        var a = _composer.SelectRandomQuestions(exam, 3, seed: 1);
        var b = _composer.SelectRandomQuestions(exam, 3, seed: 2);

        a.Questions.Select(q => q.Text).SequenceEqual(b.Questions.Select(q => q.Text)).Must().BeFalse();
    }

    [Fact]
    public void SelectRandomQuestions_SameSeed_IsStable()
    {
        var exam = CreateExam();

        var a = _composer.SelectRandomQuestions(exam, 3, seed: 12345);
        var b = _composer.SelectRandomQuestions(exam, 3, seed: 12345);

        a.Questions.Select(q => q.Text).ToList().SequenceEqual(b.Questions.Select(q => q.Text)).Must().BeTrue();
    }

    [Fact]
    public void SelectRandomQuestions_ZeroCount_ReturnsEmpty()
    {
        var exam = CreateExam();

        var result = _composer.SelectRandomQuestions(exam, 0, seed: 42);

        result.Questions.Count.Must().Be(0);
        result.Sections.Count.Must().Be(0);
    }
}
