using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared.Interfaces;

public interface IExamEditor
{
    ChangeRepresentationObject AddSection(Exam exam, string title);

    ChangeRepresentationObject RenameSection(Exam exam, string oldTitle, string newTitle);

    ChangeRepresentationObject RemoveSection(Exam exam, string title);

    ChangeRepresentationObject AddQuestion(Exam exam, string sectionTitle, Question question);

    ChangeRepresentationObject RemoveQuestion(Exam exam, string sectionTitle, int questionNumber);

    ChangeRepresentationObject ReplaceQuestion(Exam exam, string sectionTitle, int questionNumber, Question newQuestion);

    ChangeRepresentationObject UpdateQuestion(Exam exam, string sectionTitle, int questionNumber, Question newQuestion);

    void ApplyProperties(Exam exam, Properties properties);

    void ApplyChange(Exam exam, ChangeRepresentationObject change);

    void RevertChange(Exam exam, ChangeRepresentationObject change);
}
