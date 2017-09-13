using Shared.Models;

namespace Shared.Interfaces
{
    public interface IUndoRedo
    {
        ChangeRepresentationObject Undo();
        ChangeRepresentationObject Redo();
        void InsertObjectforUndoRedo(ChangeRepresentationObject dataobject);
    }
}
