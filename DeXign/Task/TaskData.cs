using System;

namespace DeXign.Task
{
    public class TaskData : IDisposable
    {
        public object Source { get; set; }
        public Action DoAction { get; set; }
        public Action UndoAction { get; set; }

        public bool IsStable { get { return moved % 2 == 0; } }

        int moved = 0;

        public TaskData(object source, Action doAction, Action undoAction)
        {
            this.Source = source;
            this.DoAction = doAction;
            this.UndoAction = undoAction;
        }

        public virtual void Do()
        {
            moved++;
            DoAction?.Invoke();
        }

        public virtual void Undo()
        {
            moved++;
            UndoAction?.Invoke();
        }

        public virtual void Dispose()
        {
            DoAction = null;
            UndoAction = null;
        }
    }
}
