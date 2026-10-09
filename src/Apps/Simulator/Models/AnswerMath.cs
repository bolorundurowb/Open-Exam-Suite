using OpenExamSuite.Shared;
using OpenExamSuite.Simulator.Session.Models;

namespace OpenExamSuite.Simulator.Models;

/// <summary>
/// Small, UI-free helpers for comparing a candidate's selection with a question's answer.
/// </summary>
public static class AnswerMath
{
    public static IReadOnlyList<char> CorrectLetters(Question question) =>
        question.IsMultipleChoice
            ? (question.Answers ?? []).OrderBy(c => c).ToArray()
            : [question.Answer];

    public static IReadOnlyList<char> SelectedLetters(AnswerSelection selection) =>
        selection.ToCharArray().OrderBy(c => c).ToArray();

    public static bool IsCorrect(Question question, AnswerSelection selection) =>
        selection.IsAnswered && CorrectLetters(question).SequenceEqual(SelectedLetters(selection));

    /// <summary>
    /// Builds the selection after the candidate activates <paramref name="letter"/>.
    /// Single-answer questions replace the choice; multiple-answer questions toggle it.
    /// Emptying a multiple-answer selection leaves the question unanswered, so it never counts as answered.
    /// </summary>
    public static AnswerSelection Activate(Question question, AnswerSelection current, char letter)
    {
        if (!question.IsMultipleChoice)
            return AnswerSelection.Answered(letter, current.IsFlagged);

        var letters = SelectedLetters(current).ToList();
        if (!letters.Remove(letter))
            letters.Add(letter);

        return letters.Count == 0
            ? AnswerSelection.Unanswered(current.IsFlagged)
            : AnswerSelection.Answered(System.Collections.Immutable.ImmutableArray.CreateRange(letters.OrderBy(c => c)), current.IsFlagged);
    }
}
