using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using DeXign.Commands;

using WPFExtension;

namespace DeXign.Task
{
    public class TaskManager : DependencyObjectEx
    {
        private static readonly DependencyPropertyKey CanRedoPropertyKey =
            DependencyHelper.RegisterReadOnly();

        private static readonly DependencyPropertyKey CanUndoPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty CanRedoProperty = CanRedoPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanUndoProperty = CanUndoPropertyKey.DependencyProperty;

        /// <summary>
        /// 이전작업으로 돌아갈 수 있는지를 나타내는 값을 가져옵니다.
        /// </summary>
        public bool CanRedo
        {
            get { return GetValue<bool>(CanRedoProperty); }
            private set { SetValue(CanRedoPropertyKey, value); }
        }

        /// <summary>
        /// 이전작업으로 돌아갈 수 있는지를 나타내는 값을 가져옵니다.
        /// </summary>
        public bool CanUndo
        {
            get { return GetValue<bool>(CanUndoProperty); }
            private set { SetValue(CanUndoPropertyKey, value); }
        }

        /// <summary>
        /// 이전작업으로 되돌리는 명령어를 가져옵니다.
        /// </summary>
        public ActionCommand RedoCommand { get; }

        /// <summary>
        /// 이전작업으로 되돌리는 명령어를 가져옵니다.
        /// </summary>
        public ActionCommand UndoCommand { get; }

        protected Stack<TaskData> DoStack { get; }
        protected Stack<TaskData> UndoStack { get; }
        
        public TaskManager()
        {
            DoStack = new Stack<TaskData>();
            UndoStack = new Stack<TaskData>();
            
            RedoCommand = new ActionCommand();
            UndoCommand = new ActionCommand();

            RedoCommand.OnExecute += RedoCommand_OnExecute;
            UndoCommand.OnExecute += UndoCommand_OnExecute;
        }

        private void UndoCommand_OnExecute(object sender, object e)
        {
            Undo();
        }

        private void RedoCommand_OnExecute(object sender, object e)
        {
            Redo();
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }

        public virtual void Push(TaskData task)
        {
            ClearUndoStack();

            DoStack.Push(task);
            task.Do();

            Update();
        }

        public virtual void Push(object source, Action doAction, Action undoAction)
        {
            this.Push(new TaskData(source, doAction, undoAction));
        }

        public bool Redo()
        {
            if (CanRedo)
            {
                TaskData task = UndoStack.Pop();
                Update();

                DoStack.Push(task);
                task.Do();

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

        protected virtual void ClearUndoStack()
        {
            foreach (TaskData task in UndoStack.ToArray().Reverse())
            {
                // 삭제될 Task의 Source를 참조하고 있는지 확인함
                if (DoStack.Count(t => t.Source.Equals(task.Source)) == 0)
                    task.Dispose();
            }

            UndoStack.Clear();
        }

        private void Update()
        {
            CanRedo = (UndoStack.Count > 0);
            CanUndo = (DoStack.Count > 0);
        }
    }
}