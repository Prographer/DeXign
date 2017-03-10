using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using DeXign.Commands;

using WPFExtension;
using System.Windows.Controls;

namespace DeXign.Task
{
    /// <summary>
    /// 작업관련 메서드를 제공하고 관리하는 클래스입니다.
    /// </summary>
    public class TaskManager : DependencyObjectEx
    {
        private static readonly DependencyPropertyKey CanRedoPropertyKey =
            DependencyHelper.RegisterReadOnly();

        private static readonly DependencyPropertyKey CanUndoPropertyKey =
            DependencyHelper.RegisterReadOnly();

        /// <summary>
        /// DeXign.Task.TaskManager.CanRedo 종속성입니다.
        /// </summary>
        public static readonly DependencyProperty CanRedoProperty = CanRedoPropertyKey.DependencyProperty;

        /// <summary>
        /// DeXign.Task.TaskManager.CanUndo 종속성입니다.
        /// </summary>
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

        /// <summary>
        /// 지난 작업을 기록하고 있는 스택을 가져옵니다.
        /// </summary>
        protected Stack<TaskData> DoStack { get; }

        /// <summary>
        /// 이전 작업으로 되돌린 작업을 기록하고 있는 스택을 가져옵니다.
        /// </summary>
        protected Stack<TaskData> UndoStack { get; }
        
        /// <summary>
        /// 작업 관리자를 생성합니다.
        /// </summary>
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

        /// <summary>
        /// 작업을 추가합니다.
        /// </summary>
        /// <param name="task"></param>
        public virtual void Push(TaskData task)
        {
            ClearUndoStack();

            DoStack.Push(task);
            task.Do();

            Update();
        }

        /// <summary>
        /// 작업을 추가합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="doAction"></param>
        /// <param name="undoAction"></param>
        public virtual void Push(object source, Action doAction, Action undoAction)
        {
            this.Push(new TaskData(source, doAction, undoAction));
        }

        /// <summary>
        /// 이전 상태로 되돌린 작업을 취소합니다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 이전 상태로 되돌아갑니다.
        /// </summary>
        /// <returns></returns>
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
                if (!task.IsStable && DoStack.Count(t => t.Source.Equals(task.Source)) == 0)
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