using System;
using System.Windows;

namespace DeXign.Task
{
    public class TaskData : IDisposable
    {
        public object Source { get; set; }
        public Action DoAction { get; set; }
        public Action UndoAction { get; set; }

        public TaskData(object source, Action doAction, Action undoAction)
        {
            this.Source = source;
            this.DoAction = doAction;
            this.UndoAction = undoAction;
        }

        public virtual void Do()
        {
            DoAction?.Invoke();
        }

        public virtual void Undo()
        {
            UndoAction?.Invoke();
        }

        public virtual void Dispose()
        {
            DoAction = null;
            UndoAction = null;
        }
    }
}
