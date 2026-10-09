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
        Assert.True(doc.Nodes.ContainsKey(section.Id));
        Assert.Equal(section.Id, doc.SelectedNodeId);
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
        Assert.Equal(q2.Id, doc.SelectedNodeId);
        Assert.Equal("Second", doc.Nodes[q2.Id].Question!.Text);
        Assert.Equal("First", doc.Nodes[q1.Id].Question!.Text);
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
        Assert.Equal(question.Id, doc.SelectedNodeId);
        Assert.Equal(section2.Id, doc.Nodes[question.Id].ParentId);
        Assert.True(doc.Nodes.ContainsKey(section1.Id));
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

    [Fact]
    public void ReorderQuestion_UndoRestoresSessionId()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var first = doc.AddQuestion(section.Id);
        var second = doc.AddQuestion(section.Id);
        doc.UpdateQuestionText(first.Id, "First");
        doc.UpdateQuestionText(second.Id, "Second");

        doc.ReorderQuestion(second.Id, 0);
        doc.Undo();

        Assert.Equal("First", doc.Exam.Sections[0].Questions[0].Text);
        Assert.Equal(second.Id, doc.Nodes.Values.Single(n => n.Question?.Text == "Second").Id);
        Assert.Equal(first.Id, doc.Nodes.Values.Single(n => n.Question?.Text == "First").Id);
    }

    [Fact]
    public void WriteCopy_LeavesOpenPathAndDirtyFlag()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);
        doc.UpdateQuestionText(question.Id, "Preview");
        var saved = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.oef");
        var copy = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.oef");
        try
        {
            Assert.True(doc.Save(saved).Success);
            doc.UpdateQuestionText(question.Id, "Edited");

            var written = doc.WriteCopy(copy);

            Assert.True(written.Success);
            Assert.Equal(saved, doc.FilePath);
            Assert.True(doc.IsDirty);
            Assert.Equal(question.Id, doc.SelectedNodeId);
            Assert.True(File.Exists(copy));
        }
        finally
        {
            File.Delete(saved);
            File.Delete(copy);
        }
    }

    [Fact]
    public void UpdateQuestionText_UnchangedText_DoesNotFireChanged()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection();
        var question = doc.AddQuestion(section.Id);
        doc.UpdateQuestionText(question.Id, "Initial");

        var changedFired = false;
        doc.Changed += () => changedFired = true;

        doc.UpdateQuestionText(question.Id, "Initial");

        Assert.False(changedFired);
    }

    [Fact]
    public void Save_FailureIncludesIoDetail()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        doc.AddSection();
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "exam.oef");

        var result = doc.Save(missing);

        Assert.False(result.Success);
        Assert.Equal(OpenExamSuite.Shared.Enums.ExamIoError.WriteFailed, result.Error);
        Assert.False(string.IsNullOrWhiteSpace(result.Detail));
        Assert.Equal(string.Empty, doc.FilePath ?? string.Empty);
    }

    [Fact]
    public void LoadXml_ImportsWithoutWritingOef()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        doc.AddSection("Imported");
        var xml = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.xml");
        try
        {
            Assert.True(doc.SaveXml(xml).Success);
            var written = File.GetLastWriteTimeUtc(xml);

            var loaded = CreateDocument();
            var result = loaded.LoadXml(xml);

            Assert.True(result.Success);
            Assert.True(loaded.IsDirty);
            Assert.Equal("Imported", loaded.Exam.Sections[0].Title);
            Assert.EndsWith(".oef", loaded.FilePath, StringComparison.OrdinalIgnoreCase);
            Assert.False(File.Exists(loaded.FilePath));
            Assert.Equal(written, File.GetLastWriteTimeUtc(xml));
        }
        finally
        {
            File.Delete(xml);
        }
    }

    [Fact]
    public void Load_DoesNotModifySourceTimestamp()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        doc.AddSection("Kept");
        var path = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.oef");
        try
        {
            Assert.True(doc.Save(path).Success);
            var written = File.GetLastWriteTimeUtc(path);

            var loaded = CreateDocument();
            var result = loaded.Load(path);

            Assert.True(result.Success);
            Assert.False(loaded.IsLegacy);
            Assert.Equal(written, File.GetLastWriteTimeUtc(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void IsRecoveryNewerThan_ComparesSourceTimestamp()
    {
        var doc = CreateDocument();
        var source = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.oef");
        var recovery = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.recovery.oef");
        try
        {
            File.WriteAllText(source, "source");
            File.WriteAllText(recovery, "recovery");
            File.SetLastWriteTimeUtc(source, DateTime.UtcNow.AddMinutes(-5));
            File.SetLastWriteTimeUtc(recovery, DateTime.UtcNow);

            Assert.True(doc.IsRecoveryNewerThan(recovery, source));

            File.SetLastWriteTimeUtc(recovery, DateTime.UtcNow.AddMinutes(-10));
            Assert.False(doc.IsRecoveryNewerThan(recovery, source));
            Assert.True(doc.IsRecoveryNewerThan(recovery, null));
        }
        finally
        {
            File.Delete(source);
            File.Delete(recovery);
        }
    }

    [Fact]
    public void FreshDocument_HasNoDocument()
    {
        var doc = CreateDocument();

        Assert.False(doc.HasDocument);
    }

    [Fact]
    public void NewDocument_SetsHasDocumentAndFiresOpened()
    {
        var doc = CreateDocument();
        var fired = 0;
        doc.Opened += () => fired++;

        doc.NewDocument();

        Assert.True(doc.HasDocument);
        Assert.Equal(1, fired);
    }

    [Fact]
    public void CloseDocument_ClearsTheOpenExam()
    {
        var doc = CreateDocument();
        var opened = 0;
        doc.Opened += () => opened++;
        doc.NewDocument();
        doc.AddSection("Section");

        doc.CloseDocument();

        Assert.False(doc.HasDocument);
        Assert.False(doc.IsDirty);
        Assert.Null(doc.FilePath);
        Assert.Empty(doc.Nodes);
        Assert.Equal(1, opened);
    }

    [Fact]
    public void Load_SetsHasDocumentAndFiresOpened()
    {
        var doc = CreateDocument();
        doc.NewDocument();
        var section = doc.AddSection("S");
        doc.AddQuestion(section.Id);
        var path = Path.Combine(Path.GetTempPath(), $"oes-test-{Guid.NewGuid():N}.oef");
        try
        {
            Assert.True(doc.Save(path).Success);

            var loaded = CreateDocument();
            var fired = 0;
            loaded.Opened += () => fired++;

            Assert.True(loaded.Load(path).Success);
            Assert.True(loaded.HasDocument);
            Assert.Equal(1, fired);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
