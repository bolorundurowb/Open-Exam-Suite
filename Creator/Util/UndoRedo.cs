using Shared;
using System.Collections.Generic;
using Shared.Interfaces;
using Shared.Models;

namespace Creator.Util
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
            UndoCollection.Push(redoObject);
            return redoObject;
        }

        public ChangeRepresentationObject Undo()
        {
            if (UndoCollection.Count == 0) return null;
            //
            ChangeRepresentationObject undoObject = UndoCollection.Pop();
            RedoCollection.Push(undoObject);
            return undoObject;
        }
    }
}
