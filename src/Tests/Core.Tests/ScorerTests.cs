using OmniAssert;
using Xunit;

namespace OpenExamSuite.Core.Tests;

public class ScorerTests
{
    private readonly Scorer _scorer = new();

    [Fact]
    public void ComputeNormalizedScore_Basic()
    {
        _scorer.ComputeNormalizedScore(7, 10).Must().Be(700);
    }

    [Fact]
    public void ComputeNormalizedScore_ZeroTotal_ReturnsZero()
    {
        _scorer.ComputeNormalizedScore(5, 0).Must().Be(0);
    }

    [Fact]
    public void ComputeNormalizedScore_NegativeTotal_ReturnsZero()
    {
        _scorer.ComputeNormalizedScore(5, -1).Must().Be(0);
    }

    [Fact]
    public void IsPassed_ExactPassmark_ReturnsTrue()
    {
        _scorer.IsPassed(650, 650).Must().BeTrue();
    }

    [Fact]
    public void IsPassed_BelowPassmark_ReturnsFalse()
    {
        _scorer.IsPassed(649, 650).Must().BeFalse();
    }

    [Fact]
    public void Grade_DelegatesToGrader()
    {
        var questions = new List<Question> { new() { No = 1, Answer = 'A' } };
        var sections = new List<Section> { new() { Title = "S", Questions = questions } };

        var result = _scorer.Grade(new object?[] { 'A' }, questions, sections);

        result.NumberOfCorrectAnswers.Must().Be(1);
    }
}
