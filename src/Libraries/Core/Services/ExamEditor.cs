using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Shared.Interfaces;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared.Services;

public class ExamEditor : IExamEditor
{
    public ChangeRepresentationObject AddSection(Exam exam, string title)
    {
        var section = new Section { Title = title };
        exam.Sections.Add(section);

        return new ChangeRepresentationObject
        {
            Action = ActionType.Add,
            SectionTitle = title
        };
    }

    public ChangeRepresentationObject RenameSection(Exam exam, string oldTitle, string newTitle)
    {
        var section = exam.Sections.FirstOrDefault(s => s.Title == oldTitle);
        if (section != null)
            section.Title = newTitle;

        return new ChangeRepresentationObject
        {
            Action = ActionType.RenameSection,
            SectionTitle = newTitle,
            PreviousSectionTitle = oldTitle
        };
    }

    public ChangeRepresentationObject RemoveSection(Exam exam, string title)
    {
        var section = exam.Sections.FirstOrDefault(s => s.Title == title);
        if (section != null)
            exam.Sections.Remove(section);

        return new ChangeRepresentationObject
        {
            Action = ActionType.Delete,
            SectionTitle = title
        };
    }

    public ChangeRepresentationObject AddQuestion(Exam exam, string sectionTitle, Question question)
    {
        var section = GetOrCreateSection(exam, sectionTitle);
        question.No = section.Questions.Count + 1;
        section.Questions.Add(question);

        return new ChangeRepresentationObject
        {
            Action = ActionType.Add,
            SectionTitle = sectionTitle,
            Question = question
        };
    }

    public ChangeRepresentationObject RemoveQuestion(Exam exam, string sectionTitle, int questionNumber)
    {
        var section = exam.Sections.FirstOrDefault(s => s.Title == sectionTitle);
        if (section == null || questionNumber < 1 || questionNumber > section.Questions.Count)
            throw new ArgumentOutOfRangeException(nameof(questionNumber));

        var question = section.Questions[questionNumber - 1];
        section.Questions.RemoveAt(questionNumber - 1);
        RenumberQuestions(section);

        return new ChangeRepresentationObject
        {
            Action = ActionType.Delete,
            SectionTitle = sectionTitle,
            Question = question
        };
    }

    public ChangeRepresentationObject ReplaceQuestion(Exam exam, string sectionTitle, int questionNumber,
        Question newQuestion)
    {
        var section = exam.Sections.FirstOrDefault(s => s.Title == sectionTitle);
        if (section == null || questionNumber < 1 || questionNumber > section.Questions.Count)
            throw new ArgumentOutOfRangeException(nameof(questionNumber));

        newQuestion.No = questionNumber;
        section.Questions[questionNumber - 1] = newQuestion;

        return new ChangeRepresentationObject
        {
            Action = ActionType.Modify,
            SectionTitle = sectionTitle,
            Question = newQuestion
        };
    }

    public ChangeRepresentationObject UpdateQuestion(Exam exam, string sectionTitle, int questionNumber,
        Question newQuestion)
    {
        var section = exam.Sections.FirstOrDefault(s => s.Title == sectionTitle);
        if (section == null || questionNumber < 1 || questionNumber > section.Questions.Count)
            throw new ArgumentOutOfRangeException(nameof(questionNumber));

        var previousQuestion = section.Questions[questionNumber - 1];
        newQuestion.No = questionNumber;
        section.Questions[questionNumber - 1] = newQuestion;

        return new ChangeRepresentationObject
        {
            Action = ActionType.Modify,
            SectionTitle = sectionTitle,
            Question = newQuestion,
            PreviousQuestion = previousQuestion
        };
    }

    public void ApplyProperties(Exam exam, Properties properties)
    {
        exam.Properties = properties;
    }

    public void ApplyChange(Exam exam, ChangeRepresentationObject change)
    {
        switch (change.Action)
        {
            case ActionType.Add:
                AddQuestion(exam, change.SectionTitle, CloneQuestion(change.Question!));
                break;
            case ActionType.Delete:
                RemoveQuestion(exam, change.SectionTitle, change.Question!.No);
                break;
            case ActionType.Modify:
                ReplaceQuestion(exam, change.SectionTitle, change.Question!.No, CloneQuestion(change.Question));
                break;
            case ActionType.RenameSection:
                RenameSection(exam, change.PreviousSectionTitle!, change.SectionTitle);
                break;
        }
    }

    public void RevertChange(Exam exam, ChangeRepresentationObject change)
    {
        switch (change.Action)
        {
            case ActionType.Add:
                RemoveQuestion(exam, change.SectionTitle, change.Question!.No);
                break;
            case ActionType.Delete:
                var deletedQuestion = CloneQuestion(change.Question!);
                InsertQuestion(exam, change.SectionTitle, deletedQuestion, deletedQuestion.No);
                break;
            case ActionType.Modify:
                ReplaceQuestion(exam, change.SectionTitle, change.Question!.No,
                    CloneQuestion(change.PreviousQuestion!));
                break;
            case ActionType.RenameSection:
                RenameSection(exam, change.SectionTitle, change.PreviousSectionTitle!);
                break;
        }
    }

    private static Section GetOrCreateSection(Exam exam, string sectionTitle)
    {
        var section = exam.Sections.FirstOrDefault(s => s.Title == sectionTitle);
        if (section != null)
            return section;

        section = new Section { Title = sectionTitle };
        exam.Sections.Add(section);
        return section;
    }

    private static void RenumberQuestions(Section section)
    {
        for (var i = 0; i < section.Questions.Count; i++)
            section.Questions[i].No = i + 1;
    }

    private static void InsertQuestion(Exam exam, string sectionTitle, Question question, int questionNumber)
    {
        var section = GetOrCreateSection(exam, sectionTitle);
        var index = Math.Min(questionNumber - 1, section.Questions.Count);
        section.Questions.Insert(index, question);
        RenumberQuestions(section);
    }

    private static Question CloneQuestion(Question question)
    {
        return new Question
        {
            No = question.No,
            Text = question.Text,
            ImageData = question.ImageData,
            Answer = question.Answer,
            IsMultipleChoice = question.IsMultipleChoice,
            Answers = question.Answers.ToArray(),
            Options = question.Options.Select(o => new Option { Alphabet = o.Alphabet, Text = o.Text }).ToList(),
            Explanation = question.Explanation
        };
    }
}
