using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared.Interfaces;

public interface IExamComposer
{
    SelectionResult SelectSections(Exam exam, IEnumerable<Section> selectedSections);

    SelectionResult SelectFixedQuestions(Exam exam, int questionCount);
}
