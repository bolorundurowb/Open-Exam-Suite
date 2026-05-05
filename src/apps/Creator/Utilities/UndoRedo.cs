using OpenExamSuite.Shared.Interfaces;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Creator.Utilities;

public class UndoRedo : IUndoRedo
{
    private readonly Stack<ChangeRepresentationObject> _undoStack = new();
    private readonly Stack<ChangeRepresentationObject> _redoStack = new();

    public void Push(ChangeRepresentationObject change)
    {
        _undoStack.Push(change);
        _redoStack.Clear();
    }

    public ChangeRepresentationObject? Redo()
    {
        if (_redoStack.Count == 0) return null;
        var item = _redoStack.Pop();
        _undoStack.Push(item);
        return item;
    }

    public ChangeRepresentationObject? Undo()
    {
        if (_undoStack.Count == 0) return null;
        var item = _undoStack.Pop();
        _redoStack.Push(item);
        return item;
    }
}