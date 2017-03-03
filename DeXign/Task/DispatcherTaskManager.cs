using System;

namespace DeXign.Task
{
    public class DispatcherTaskManager : TaskManager
    {
        public override void Push(object source, Action doAction, Action undoAction)
        {
            base.Push(new DispatcherTaskData(source, doAction, undoAction, null));
        }

        public void Push(object source, Action doAction, Action undoAction, Action destroyAction = null)
        {
            base.Push(new DispatcherTaskData(source, doAction, undoAction, destroyAction));
        }
    }
}
