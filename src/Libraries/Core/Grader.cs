using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared;

/// <summary>
/// Represents the outcome of grading an exam attempt.
/// </summary>
public record GradingResult(int NumberOfCorrectAnswers, List<SectionResult> ResultSpread);

/// <summary>
/// Computes exam results from a candidate's answers. Answers may be
/// <see langword="null"/> for questions the candidate never reached.
/// </summary>
public static class Grader
{
    public static GradingResult Grade(object?[] userAnswers, List<Question> questions, List<Section> sections)
    {
        var numOfCorrectAnswers = 0;
        for (var i = 0; i < questions.Count; i++)
        {
            if (IsCorrect(userAnswers[i], questions[i]))
                numOfCorrectAnswers++;
        }

        var resultSpread = new List<SectionResult>();
        foreach (var section in sections)
        {
            var numOfQuestions = 0;
            var numOfCorrect = 0;
            for (var i = 0; i < questions.Count; i++)
            {
                if (!section.Questions.Contains(questions[i]))
                    continue;

                numOfQuestions++;
                if (IsCorrect(userAnswers[i], questions[i]))
                    numOfCorrect++;
            }

            resultSpread.Add(new SectionResult(section.Title, numOfQuestions, numOfCorrect));
        }

        return new GradingResult(numOfCorrectAnswers, resultSpread);
    }

    private static bool IsCorrect(object? userAnswer, Question question)
    {
        switch (userAnswer)
        {
            case null:
                return false;
            case char[] answers:
                return answers.SequenceEqual(question.Answers ?? []);
            case char answer:
                return answer == question.Answer;
            default:
                return false;
        }
    }
}
