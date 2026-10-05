using OmniAssert;
using Xunit;

namespace OpenExamSuite.Core.Tests;

public class ExamEditorTests
{
    private readonly ExamEditor _editor = new();

    [Fact]
    public void AddSection_AddsSection()
    {
        var exam = new Exam();

        var change = _editor.AddSection(exam, "Section A");

        exam.Sections.Count.Verify().ToBe(1);
        exam.Sections[0].Title.Verify().ToBe("Section A");
        change.Action.Verify().ToBe(ActionType.Add);
        change.SectionTitle.Verify().ToBe("Section A");
    }

    [Fact]
    public void RenameSection_ChangesTitle()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Old");

        var change = _editor.RenameSection(exam, "Old", "New");

        exam.Sections[0].Title.Verify().ToBe("New");
        change.Action.Verify().ToBe(ActionType.RenameSection);
        change.PreviousSectionTitle.Verify().ToBe("Old");
        change.SectionTitle.Verify().ToBe("New");
    }

    [Fact]
    public void RemoveSection_RemovesSection()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");

        _editor.RemoveSection(exam, "Section A");

        exam.Sections.Count.Verify().ToBe(0);
    }

    [Fact]
    public void AddQuestion_AppendsAndSetsNumber()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        var question = new Question { Text = "Q1" };

        _editor.AddQuestion(exam, "Section A", question);

        exam.Sections[0].Questions.Count.Verify().ToBe(1);
        question.No.Verify().ToBe(1);
    }

    [Fact]
    public void RemoveQuestion_RemovesAndRenumbers()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        _editor.AddQuestion(exam, "Section A", new Question { Text = "Q1" });
        _editor.AddQuestion(exam, "Section A", new Question { Text = "Q2" });

        _editor.RemoveQuestion(exam, "Section A", 1);

        exam.Sections[0].Questions.Count.Verify().ToBe(1);
        exam.Sections[0].Questions[0].No.Verify().ToBe(1);
        exam.Sections[0].Questions[0].Text.Verify().ToBe("Q2");
    }

    [Fact]
    public void UpdateQuestion_ReplacesAndCapturesPrevious()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        _editor.AddQuestion(exam, "Section A", new Question { Text = "Original" });
        var updated = new Question { Text = "Updated" };

        var change = _editor.UpdateQuestion(exam, "Section A", 1, updated);

        exam.Sections[0].Questions[0].Text.Verify().ToBe("Updated");
        change.PreviousQuestion.Verify().NotToBeNull();
        change.PreviousQuestion!.Text.Verify().ToBe("Original");
    }

    [Fact]
    public void ApplyProperties_SetsProperties()
    {
        var exam = new Exam();
        var properties = new Properties { Title = "New Title" };

        _editor.ApplyProperties(exam, properties);

        exam.Properties.Title.Verify().ToBe("New Title");
    }
}
