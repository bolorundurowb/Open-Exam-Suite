using System;
using System.Collections.Generic;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Creator.Services;

/// <summary>
/// Port of <c>Creator/Utilities/UndoRedo.cs</c>. Push clears the redo stack;
/// undo moves the top of undo onto redo and returns the item; redo does the
/// inverse. Identical semantics to the WinForms version.
/// </summary>
public sealed class UndoRedoService : IUndoRedoService
{
    private readonly Stack<ChangeRepresentationObject> _undo = new();
    private readonly Stack<ChangeRepresentationObject> _redo = new();

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public event EventHandler? Changed;

    public void Push(ChangeRepresentationObject change)
    {
        _undo.Push(change);
        _redo.Clear();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public ChangeRepresentationObject? Undo()
    {
        if (_undo.Count == 0) return null;
        var item = _undo.Pop();
        _redo.Push(item);
        Changed?.Invoke(this, EventArgs.Empty);
        return item;
    }

    public ChangeRepresentationObject? Redo()
    {
        if (_redo.Count == 0) return null;
        var item = _redo.Pop();
        _undo.Push(item);
        Changed?.Invoke(this, EventArgs.Empty);
        return item;
    }

    public void Reset()
    {
        _undo.Clear();
        _redo.Clear();
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
