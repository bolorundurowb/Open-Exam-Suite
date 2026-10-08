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
        var remaining = questionCount;

        foreach (var section in exam.Sections)
        {
            if (remaining <= 0)
                break;

            if (section.Questions.Count <= remaining)
            {
                selectedSections.Add(CloneSection(section));
                remaining -= section.Questions.Count;
            }
            else
            {
                selectedSections.Add(new Section
                {
                    Title = section.Title,
                    Questions = section.Questions.Take(remaining).Select(CloneQuestion).ToList()
                });
                remaining = 0;
            }
        }

        var selectedQuestions = selectedSections.SelectMany(s => s.Questions).ToList();
        return new SelectionResult(selectedSections, selectedQuestions);
    }

    public SelectionResult SelectRandomQuestions(Exam exam, int questionCount, int seed)
    {
        if (questionCount <= 0 || exam.Sections.Count == 0)
            return new SelectionResult([], []);

        var random = new Random(seed);
        var sourceQuestions = exam.Sections
            .SelectMany((section, sectionIndex) => section.Questions.Select(question => (SectionIndex: sectionIndex, Question: question)))
            .ToList();
        var count = Math.Min(questionCount, sourceQuestions.Count);

        // Keep section membership alongside each clone. The section lists and the presented
        // question list must contain the same instances so section grading remains correct.
        var pairs = sourceQuestions
            .Select(item => (item.SectionIndex, Clone: CloneQuestion(item.Question)))
            .ToList();

        // Fisher-Yates shuffle of the full pool, then take the prefix.
        for (int i = pairs.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (pairs[i], pairs[j]) = (pairs[j], pairs[i]);
        }

        var selectedPairs = pairs.Take(count).ToList();
        var selectedQuestions = selectedPairs.Select(p => p.Clone).ToList();
        var selectedSections = selectedPairs
            .GroupBy(p => p.SectionIndex)
            .OrderBy(group => group.Key)
            .Select(group => new Section
            {
                Title = exam.Sections[group.Key].Title,
                Questions = group.Select(p => p.Clone).ToList()
            })
            .ToList();

        return new SelectionResult(selectedSections, selectedQuestions);
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
            Answers = question.Answers?.ToArray() ?? [],
            Options = question.Options.Select(o => new Option { Alphabet = o.Alphabet, Text = o.Text }).ToList(),
            Explanation = question.Explanation
        };
    }

    private static Section CloneSection(Section section)
    {
        return new Section
        {
            Title = section.Title,
            Questions = section.Questions.Select(CloneQuestion).ToList()
        };
    }
}
