using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared.Interfaces;

public interface IExamComposer
{
    SelectionResult SelectSections(Exam exam, IEnumerable<Section> selectedSections);

    SelectionResult SelectFixedQuestions(Exam exam, int questionCount);

    /// <summary>
    /// Draws a random sample of <paramref name="questionCount"/> questions using the supplied seed.
    /// A fixed seed produces a stable draw; different seeds produce different draws.
    /// </summary>
    SelectionResult SelectRandomQuestions(Exam exam, int questionCount, int seed);
}
