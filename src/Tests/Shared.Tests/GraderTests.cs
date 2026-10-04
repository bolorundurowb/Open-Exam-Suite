using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Models;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.Shared.Tests;

public class GraderTests
{
    [Fact]
    public void Grade_UnansweredQuestions_DoesNotThrow()
    {
        var questions = new List<Question>
        {
            new() { No = 1, Answer = 'A' },
            new() { No = 2, Answer = 'B' },
            new() { No = 3, Answer = 'C' }
        };
        var sections = new List<Section> { new() { Title = "Section A", Questions = questions } };

        var userAnswers = new object?[] { 'A', null, null };

        var result = Grader.Grade(userAnswers, questions, sections);

        result.NumberOfCorrectAnswers.Verify().ToBe(1);
    }

    [Fact]
    public void Grade_AllUnanswered_DoesNotThrow()
    {
        var questions = new List<Question>
        {
            new() { No = 1, Answer = 'A' },
            new() { No = 2, Answer = 'B' }
        };
        var sections = new List<Section> { new() { Title = "Section A", Questions = questions } };

        var userAnswers = new object?[] { null, null };

        var result = Grader.Grade(userAnswers, questions, sections);

        result.NumberOfCorrectAnswers.Verify().ToBe(0);
    }

    [Fact]
    public void Grade_AllCorrect_SingleChoice_CountsCorrectly()
    {
        var questions = new List<Question>
        {
            new() { No = 1, Answer = 'A' },
            new() { No = 2, Answer = 'B' }
        };
        var sections = new List<Section> { new() { Title = "Section A", Questions = questions } };

        var userAnswers = new object[] { 'A', 'B' };

        var result = Grader.Grade(userAnswers, questions, sections);

        result.NumberOfCorrectAnswers.Verify().ToBe(2);
    }

    [Fact]
    public void Grade_MultipleChoice_EmptySelection_IsIncorrect()
    {
        var questions = new List<Question>
        {
            new() { No = 1, IsMultipleChoice = true, Answers = ['A', 'B'] }
        };
        var sections = new List<Section> { new() { Title = "Section A", Questions = questions } };

        var userAnswers = new object[] { Array.Empty<char>() };

        var result = Grader.Grade(userAnswers, questions, sections);

        result.NumberOfCorrectAnswers.Verify().ToBe(0);
    }

    [Fact]
    public void Grade_MultipleChoice_ExactMatch_IsCorrect()
    {
        var questions = new List<Question>
        {
            new() { No = 1, IsMultipleChoice = true, Answers = ['A', 'B'] }
        };
        var sections = new List<Section> { new() { Title = "Section A", Questions = questions } };

        var userAnswers = new object[] { new[] { 'A', 'B' } };

        var result = Grader.Grade(userAnswers, questions, sections);

        result.NumberOfCorrectAnswers.Verify().ToBe(1);
    }

    [Fact]
    public void Grade_PerSectionBreakdown_ComputesTotals()
    {
        var sectionA = new Section
        {
            Title = "Section A",
            Questions =
            [
                new Question { No = 1, Answer = 'A' },
                new Question { No = 2, Answer = 'B' }
            ]
        };
        var sectionB = new Section
        {
            Title = "Section B",
            Questions = [new Question { No = 3, Answer = 'C' }]
        };
        var questions = new List<Question>
        {
            sectionA.Questions[0],
            sectionA.Questions[1],
            sectionB.Questions[0]
        };
        var sections = new List<Section> { sectionA, sectionB };

        var userAnswers = new object[] { 'A', 'X', 'C' };

        var result = Grader.Grade(userAnswers, questions, sections);

        result.NumberOfCorrectAnswers.Verify().ToBe(2);
        result.ResultSpread.Count.Verify().ToBe(2);
        result.ResultSpread[0].Verify().ToBe(new SectionResult("Section A", 2, 1));
        result.ResultSpread[1].Verify().ToBe(new SectionResult("Section B", 1, 1));
    }
}
