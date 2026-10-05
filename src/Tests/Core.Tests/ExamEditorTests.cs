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

        exam.Sections.Count.Must().Be(1);
        exam.Sections[0].Title.Must().Be("Section A");
        change.Action.Must().Be(ActionType.Add);
        change.SectionTitle.Must().Be("Section A");
    }

    [Fact]
    public void RenameSection_ChangesTitle()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Old");

        var change = _editor.RenameSection(exam, "Old", "New");

        exam.Sections[0].Title.Must().Be("New");
        change.Action.Must().Be(ActionType.RenameSection);
        change.PreviousSectionTitle.Must().Be("Old");
        change.SectionTitle.Must().Be("New");
    }

    [Fact]
    public void RemoveSection_RemovesSection()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");

        _editor.RemoveSection(exam, "Section A");

        exam.Sections.Count.Must().Be(0);
    }

    [Fact]
    public void AddQuestion_AppendsAndSetsNumber()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        var question = new Question { Text = "Q1" };

        _editor.AddQuestion(exam, "Section A", question);

        exam.Sections[0].Questions.Count.Must().Be(1);
        question.No.Must().Be(1);
    }

    [Fact]
    public void RemoveQuestion_RemovesAndRenumbers()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        _editor.AddQuestion(exam, "Section A", new Question { Text = "Q1" });
        _editor.AddQuestion(exam, "Section A", new Question { Text = "Q2" });

        _editor.RemoveQuestion(exam, "Section A", 1);

        exam.Sections[0].Questions.Count.Must().Be(1);
        exam.Sections[0].Questions[0].No.Must().Be(1);
        exam.Sections[0].Questions[0].Text.Must().Be("Q2");
    }

    [Fact]
    public void UpdateQuestion_ReplacesAndCapturesPrevious()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        _editor.AddQuestion(exam, "Section A", new Question { Text = "Original" });
        var updated = new Question { Text = "Updated" };

        var change = _editor.UpdateQuestion(exam, "Section A", 1, updated);

        exam.Sections[0].Questions[0].Text.Must().Be("Updated");
        change.PreviousQuestion.Must().NotBeNull();
        change.PreviousQuestion!.Text.Must().Be("Original");
    }

    [Fact]
    public void ApplyProperties_SetsProperties()
    {
        var exam = new Exam();
        var properties = new Properties { Title = "New Title" };

        _editor.ApplyProperties(exam, properties);

        exam.Properties.Title.Must().Be("New Title");
    }
}
