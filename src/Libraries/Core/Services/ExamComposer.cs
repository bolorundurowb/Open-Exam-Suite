using OpenExamSuite.Shared.Interfaces;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared.Services;

public class ExamComposer : IExamComposer
{
    public SelectionResult SelectSections(Exam exam, IEnumerable<Section> selectedSections)
    {
        var sections = selectedSections.Select(CloneSection).ToList();
        var questions = sections.SelectMany(s => s.Questions).ToList();
        return new SelectionResult(sections, questions);
    }

    public SelectionResult SelectFixedQuestions(Exam exam, int questionCount)
    {
        if (questionCount <= 0 || exam.Sections.Count == 0)
            return new SelectionResult([], []);

        var selectedSections = new List<Section>();
        var selectedQuestions = new List<Question>();
        var remaining = questionCount;

        foreach (var section in exam.Sections)
        {
            if (remaining <= 0)
                break;

            if (section.Questions.Count <= remaining)
            {
                selectedSections.Add(CloneSection(section));
                selectedQuestions.AddRange(section.Questions);
                remaining -= section.Questions.Count;
            }
            else
            {
                var truncated = section.Questions.Take(remaining).ToList();
                selectedSections.Add(new Section
                {
                    Title = section.Title,
                    Questions = truncated
                });
                selectedQuestions.AddRange(truncated);
                remaining = 0;
            }
        }

        return new SelectionResult(selectedSections, selectedQuestions);
    }

    private static Section CloneSection(Section section)
    {
        return new Section
        {
            Title = section.Title,
            Questions = section.Questions.ToList()
        };
    }
}
