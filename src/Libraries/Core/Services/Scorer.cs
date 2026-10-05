using OpenExamSuite.Shared.Interfaces;

namespace OpenExamSuite.Shared.Services;

public class Scorer : IScorer
{
    public GradingResult Grade(object?[] answers, List<Question> questions, List<Section> sections)
    {
        return Grader.Grade(answers, questions, sections);
    }

    public int ComputeNormalizedScore(int correct, int total)
    {
        if (total <= 0)
            return 0;

        return correct * 1000 / total;
    }

    public bool IsPassed(int normalizedScore, double passmark)
    {
        return normalizedScore >= passmark;
    }
}
