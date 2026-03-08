using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Shared.Interfaces;

public interface IUndoRedo
{
    ChangeRepresentationObject Undo();
    ChangeRepresentationObject Redo();
    void InsertObjectforUndoRedo(ChangeRepresentationObject dataobject);
}