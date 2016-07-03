using Shared;
using System.Collections.Generic;

namespace Creator
{
    public class UndoRedo : IUndoRedo
    {
        private Stack<ChangeRepresentationObject> UndoCollection = new Stack<ChangeRepresentationObject>();
        private Stack<ChangeRepresentationObject> RedoCollection = new Stack<ChangeRepresentationObject>();
        //
        public void InsertObjectforUndoRedo(ChangeRepresentationObject dataobject)
        {
            UndoCollection.Push(dataobject);
            RedoCollection.Clear();
        }

        public ChangeRepresentationObject Redo()
        {
            if (RedoCollection.Count == 0) return null;
            //
            ChangeRepresentationObject redoObject = RedoCollection.Pop();
            return redoObject;
        }

        public ChangeRepresentationObject Undo()
        {
            if (UndoCollection.Count == 0) return null;
            //
            ChangeRepresentationObject undoObject = UndoCollection.Pop();
            return undoObject;
        }
    }
}
