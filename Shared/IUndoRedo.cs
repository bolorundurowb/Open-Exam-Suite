namespace Shared
{
    public interface IUndoRedo
    {
        ChangeRepresentationObject Undo();
        ChangeRepresentationObject Redo();
        void InsertObjectforUndoRedo(ChangeRepresentationObject dataobject);
    }

    public enum ActionType
    {
        Delete,
        Modify,
        Add
    }
}
