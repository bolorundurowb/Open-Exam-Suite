using OpenExamSuite.Creator.Session.Models;
using OpenExamSuite.Creator.Session.Services;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Utilities;
using Xunit;

namespace OpenExamSuite.Creator.Session.Tests;

public class CreatorDocumentTests
{
    private static CreatorDocument CreateDocument() => new(new Reader(), new Writer());

    [Fact]
    public void NewDocument_HasDefaultExamAndSection()
    {
        var doc = CreateDocument();
        var result = doc.NewDocument();

        Assert.True(result.Success);
        Assert.Equal("Untitled Exam", doc.Exam.Properties.Title);
        Assert.False(doc.IsDirty);
        Assert.NotNull(doc.SelectedNodeId);
    }

    [Fact]
    public void AddSection_CreatesSection()
    {
        var doc = CreateDocument();
        doc.NewDocument();

        var section = doc.AddSection("History");

        Assert.Equal(NodeType.Section, section.Type);
        Assert.Contains(section, doc.Nodes.Values);
        Assert.True(doc.IsDirty);
    }

    [Fact]
    public void AddQuestion_CreatesQuestionWithTwoOptions()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();

        var question = doc.AddQuestion(section.Id);

        Assert.Equal(NodeType.Question, question.Type);
        Assert.Single(doc.Exam.Sections[0].Questions);
        Assert.Equal(2, question.Question!.Options.Count);
    }

    [Fact]
    public void Validation_ReportsNoCorrectAnswer()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);

        doc.Revalidate();

        Assert.Contains(doc.Problems, p => p.Kind == ProblemKind.NoCorrectAnswer && p.NodeId == question.Id);
    }

    [Fact]
    public void UpdateQuestionText_DoesNotChangeImageBytes()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);
        var imageBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        doc.UpdateQuestionImage(question.Id, imageBytes);

        doc.UpdateQuestionText(question.Id, "Updated text");

        Assert.Same(imageBytes, doc.Nodes[question.Id].Question!.ImageData);
    }

    [Fact]
    public void SetOptionCorrect_UpdatesAnswer()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);
        var optionA = question.Question!.Options[0].Alphabet.ToString();

        doc.SetOptionCorrect(question.Id, optionA, true);

        Assert.Contains('A', doc.Nodes[question.Id].Question!.Answers);
    }

    [Fact]
    public void Undo_RestoresPreviousState()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection("Before");
        var originalId = section.Id;

        doc.UpdateSectionName(section.Id, "After");
        Assert.Equal("After", doc.Nodes[originalId].Section!.Title);

        doc.Undo();
        Assert.Equal("Before", doc.Nodes[originalId].Section!.Title);
    }

    [Fact]
    public void DuplicateNode_DuplicatesQuestion()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);
        doc.UpdateQuestionText(question.Id, "Original");

        doc.DuplicateNode(question.Id);

        Assert.Equal(2, doc.Exam.Sections[0].Questions.Count);
        Assert.Contains(doc.Exam.Sections[0].Questions, q => q.Text == "Original");
    }

    [Fact]
    public void DeleteNode_RemovesQuestion()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);

        doc.DeleteNode(question.Id);

        Assert.Empty(doc.Exam.Sections[0].Questions);
    }

    [Fact]
    public void ReorderQuestion_ChangesOrder()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var q1 = doc.AddQuestion(section.Id);
        var q2 = doc.AddQuestion(section.Id);
        doc.UpdateQuestionText(q1.Id, "First");
        doc.UpdateQuestionText(q2.Id, "Second");

        doc.ReorderQuestion(q2.Id, 0);

        Assert.Equal("Second", doc.Exam.Sections[0].Questions[0].Text);
        Assert.Equal(1, doc.Exam.Sections[0].Questions[0].No);
    }

    [Fact]
    public void MoveQuestion_MovesBetweenSections()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section1 = doc.AddSection("A");
        var section2 = doc.AddSection("B");
        var question = doc.AddQuestion(section1.Id);

        doc.MoveQuestion(question.Id, section2.Id, 0);

        Assert.Empty(section1.Section!.Questions);
        Assert.Single(section2.Section!.Questions);
    }

    [Fact]
    public void SaveAndLoad_RoundTripsImageBytes()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);
        var imageBytes = new byte[] { 1, 2, 3, 4, 5 };
        doc.UpdateQuestionImage(question.Id, imageBytes);
        doc.SetOptionCorrect(question.Id, "A", true);
        doc.UpdateQuestionText(question.Id, "Round trip");

        var path = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.oef");
        try
        {
            var save = doc.Save(path);
            Assert.True(save.Success);

            var loaded = CreateDocument();
            var load = loaded.Load(path);
            Assert.True(load.Success);
            Assert.Equal("Round trip", loaded.Exam.Sections[0].Questions[0].Text);
            Assert.Equal(imageBytes, loaded.Exam.Sections[0].Questions[0].ImageData);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void UndoStack_IsCapped()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);

        for (var i = 0; i < 60; i++)
            doc.UpdateQuestionText(question.Id, $"Edit {i}");

        // Stack should be capped at 50; undo 50 times should still work.
        for (var i = 0; i < 50 && doc.CanUndo; i++)
            doc.Undo();

        Assert.False(doc.CanUndo);
    }
}
