using System;

namespace DeXign.Task
{
    /// <summary>
    /// 작업 데이터를 관리하는 클래스입니다.
    /// </summary>
    public class TaskData : IDisposable
    {
        /// <summary>
        /// 작업을 생성한 주최를 가져옵니다.
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 현재 작업을 가져오거나 설정합니다.
        /// </summary>
        public Action DoAction { get; set; }

        /// <summary>
        /// 이전 작업을 가져오거나 설정합니다.
        /// </summary>
        public Action UndoAction { get; set; }

        /// <summary>
        /// 작업 상태의 유효성을 가져옵니다.
        /// </summary>
        public bool IsStable { get { return moved % 2 == 0; } }

        int moved = 0;

        /// <summary>
        /// 작업 데이터를 생성합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="doAction"></param>
        /// <param name="undoAction"></param>
        public TaskData(object source, Action doAction, Action undoAction)
        {
            this.Source = source;
            this.DoAction = doAction;
            this.UndoAction = undoAction;
        }

        /// <summary>
        /// 현재 작업을 실행합니다.
        /// </summary>
        public virtual void Do()
        {
            moved++;
            DoAction?.Invoke();
        }

        /// <summary>
        /// 이전 작업으로 돌아갑니다.
        /// </summary>
        public virtual void Undo()
        {
            moved++;
            UndoAction?.Invoke();
        }

        /// <summary>
        /// 작업 데이터의 자원을 해제합니다.
        /// </summary>
        public virtual void Dispose()
        {
            DoAction = null;
            UndoAction = null;
        }
    }
}
