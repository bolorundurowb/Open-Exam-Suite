using OmniAssert;
using Xunit;

namespace OpenExamSuite.Core.Tests;

public class UndoRedoTests
{
    private readonly UndoRedo _undoRedo = new();
    private readonly ExamEditor _editor = new();

    [Fact]
    public void UndoRedo_QuestionAddDelete()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        var change = _editor.AddQuestion(exam, "Section A", new Question { Text = "Q1" });
        _undoRedo.Push(change);

        var undo = _undoRedo.Undo();
        _editor.RevertChange(exam, undo!);
        exam.Sections[0].Questions.Count.Must().Be(0);

        var redo = _undoRedo.Redo();
        _editor.ApplyChange(exam, redo!);
        exam.Sections[0].Questions.Count.Must().Be(1);
    }

    [Fact]
    public void UndoRedo_QuestionModify()
    {
        var exam = new Exam();
        _editor.AddSection(exam, "Section A");
        _editor.AddQuestion(exam, "Section A", new Question { Text = "Original" });
        var change = _editor.UpdateQuestion(exam, "Section A", 1, new Question { Text = "Updated" });
        _undoRedo.Push(change);

        var undo = _undoRedo.Undo();
        _editor.RevertChange(exam, undo!);
        exam.Sections[0].Questions[0].Text.Must().Be("Original");

        var redo = _undoRedo.Redo();
        _editor.ApplyChange(exam, redo!);
        exam.Sections[0].Questions[0].Text.Must().Be("Updated");
    }

    [Fact]
    public void Push_ClearsRedoStack()
    {
        _undoRedo.Push(new ChangeRepresentationObject { Action = ActionType.Add, SectionTitle = "S" });
        _undoRedo.Undo();

        _undoRedo.Push(new ChangeRepresentationObject { Action = ActionType.Add, SectionTitle = "S2" });

        _undoRedo.Redo().Must().BeNull();
    }

    [Fact]
    public void EmptyStack_ReturnsNull()
    {
        _undoRedo.Undo().Must().BeNull();
        _undoRedo.Redo().Must().BeNull();
    }

    [Fact]
    public void Clear_EmptiesStacks()
    {
        _undoRedo.Push(new ChangeRepresentationObject { Action = ActionType.Add, SectionTitle = "S" });
        _undoRedo.Undo();

        _undoRedo.Clear();

        _undoRedo.Redo().Must().BeNull();
        _undoRedo.Undo().Must().BeNull();
    }
}
