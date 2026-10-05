using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared.Interfaces;

public interface IScorer
{
    GradingResult Grade(object?[] answers, List<Question> questions, List<Section> sections);

    int ComputeNormalizedScore(int correct, int total);

    bool IsPassed(int normalizedScore, double passmark);
}
