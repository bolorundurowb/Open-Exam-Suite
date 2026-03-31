using System.Linq;
using Shouldly;
using Xunit;

namespace OpenExamSuite.Shared.Tests;

public class ExamModelTests
{
    [Fact]
    public void AddSection_NewSection_AddsSectionToList()
    {
        var exam = new Exam();
        var sectionName = "New Section";

        exam.AddSection(sectionName);

        exam.Sections.ShouldContain(s => s.Title == sectionName);
        exam.Sections.Count.ShouldBe(1);
    }

    [Fact]
    public void AddSection_ExistingSection_DoesNotAddDuplicate()
    {
        var exam = new Exam();
        var sectionName = "Existing Section";
        exam.AddSection(sectionName);

        exam.AddSection(sectionName);

        exam.Sections.Count.ShouldBe(1);
    }

    [Fact]
    public void RemoveSection_ExistingSection_RemovesSectionFromList()
    {
        var exam = new Exam();
        var sectionName = "To Be Removed";
        exam.AddSection(sectionName);
        exam.RemoveSection(sectionName);

        exam.Sections.ShouldNotContain(s => s.Title == sectionName);
        exam.Sections.Count.ShouldBe(0);
    }

    [Fact]
    public void AddQuestion_NewSection_CreatesSectionAndAddsQuestion()
    {
        var exam = new Exam();
        var sectionName = "New Section";
        var question = new Question { Text = "Test Question" };
        exam.AddQuestion(sectionName, question);

        var section = exam.Sections.FirstOrDefault(s => s.Title == sectionName);
        section.ShouldNotBeNull();
        section.Questions.ShouldContain(question);
    }

    [Fact]
    public void RemoveQuestion_ExistingQuestion_RemovesQuestionFromSection()
    {
        var exam = new Exam();
        var sectionName = "Test Section";
        var question = new Question { Text = "To Be Removed" };
        exam.AddQuestion(sectionName, question);
        exam.RemoveQuestion(sectionName, question);

        var section = exam.Sections.FirstOrDefault(s => s.Title == sectionName);
        section.ShouldNotBeNull();
        section.Questions.ShouldNotContain(question);
    }

    [Fact]
    public void NumberOfQuestions_MultipleSections_ReturnsTotalCount()
    {
        var exam = new Exam();
        exam.AddQuestion("Section 1", new Question { Text = "Q1" });
        exam.AddQuestion("Section 1", new Question { Text = "Q2" });
        exam.AddQuestion("Section 2", new Question { Text = "Q3" });

        var total = exam.NumberOfQuestions;

        total.ShouldBe(3);
    }
}
