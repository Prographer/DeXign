using System;
using System.Windows;
using System.Windows.Threading;

namespace DeXign.Task
{
    public class DispatcherTaskData : TaskData
    {
        Dispatcher dispatcher;

        public DispatcherTaskData(object source, Action doAction, Action undoAction) : base(source, doAction, undoAction)
        {
            dispatcher = Application.Current.Dispatcher;
        }

        public override void Do()
        {
            if (DoAction == null)
                return;

            dispatcher.BeginInvoke(
                DoAction,
                DispatcherPriority.ContextIdle);
        }

        public override void Undo()
        {
            if (UndoAction == null)
                return;

            dispatcher.BeginInvoke(
                UndoAction,
                DispatcherPriority.ContextIdle);
        }
    }
}
