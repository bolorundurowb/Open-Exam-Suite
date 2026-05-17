using System;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Creator.Services;

public interface IUndoRedoService
{
    bool CanUndo { get; }
    bool CanRedo { get; }

    /// <summary>Raised when the stacks change so the UI can refresh CanExecute.</summary>
    event EventHandler? Changed;

    void Push(ChangeRepresentationObject change);
    ChangeRepresentationObject? Undo();
    ChangeRepresentationObject? Redo();
    void Reset();
}
