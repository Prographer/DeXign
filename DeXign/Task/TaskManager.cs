using System.Collections.Generic;
using System.Windows;

using WPFExtension;

namespace DeXign.Task
{
    public class TaskManager : DependencyObjectEx
    {
        private static readonly DependencyPropertyKey CanRedoPropertyKey =
            DependencyHelper.RegisterReadOnly();

        private static readonly DependencyPropertyKey CanUndoPropertyKey =
            DependencyHelper.RegisterReadOnly();

        /// <summary>
        /// 이전작업으로 돌아갈 수 있는지를 나타내는 값을 가져옵니다.
        /// </summary>
        public bool CanRedo
        {
            get { return GetValue<bool>(CanRedoPropertyKey.DependencyProperty); }
            private set { SetValue(CanRedoPropertyKey, value); }
        }

        /// <summary>
        /// 이전작업으로 돌아갈 수 있는지를 나타내는 값을 가져옵니다.
        /// </summary>
        public bool CanUndo
        {
            get { return GetValue<bool>(CanUndoPropertyKey.DependencyProperty); }
            private set { SetValue(CanUndoPropertyKey, value); }
        }

        protected Stack<TaskData> DoStack { get; }
        protected Stack<TaskData> UndoStack { get; }

        public TaskManager()
        {
            DoStack = new Stack<TaskData>();
            UndoStack = new Stack<TaskData>();
        }

        public void Push(TaskData task)
        {
            UndoStack.Clear();

            DoStack.Push(task);
            task.Do();

            Update();
        }

        public bool Redo()
        {
            if (CanRedo)
            {
                TaskData task = UndoStack.Pop();

                DoStack.Push(task);
                task.Do();

                Update();

                return true;
            }

            return false;
        }

        public bool Undo()
        {
            if (DoStack.Count > 0)
            {
                TaskData task = DoStack.Pop();

                UndoStack.Push(task);
                task.Undo();

                Update();

                return true;
            }

            return false;
        }

        private void Update()
        {
            CanRedo = (UndoStack.Count > 0);
            CanUndo = (DoStack.Count > 0);
        }
    }
}
