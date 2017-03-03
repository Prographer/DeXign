using System;
using System.Windows;
using System.Windows.Threading;

namespace DeXign.Task
{
    public class DispatcherTaskData : TaskData
    {
        public Action DestroyAction { get; set; }

        Dispatcher dispatcher;

        public DispatcherTaskData(object source, Action doAction, Action undoAction, Action destroyAction) : base(source, doAction, undoAction)
        {
            dispatcher = Application.Current.Dispatcher;

            this.DestroyAction = destroyAction;
        }

        public override void Do()
        {
            Invoke(DoAction);
        }

        public override void Undo()
        {
            Invoke(UndoAction);
        }

        public override void Dispose()
        {
            base.Dispose();

            Invoke(DestroyAction);
            DestroyAction = null;
        }

        private void Invoke(Action action)
        {
            if (action == null)
                return;

            dispatcher.Invoke(
                action,
                DispatcherPriority.ContextIdle);
        }
    }
}
