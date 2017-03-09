using DeXign.Editor;
using DeXign.Extension;
using System;

namespace DeXign.Task
{
    /// <summary>
    /// 렌더러의 작업을 지정합니다.
    /// </summary>
    public enum RendererTaskType
    {
        /// <summary>
        /// 렌더러 추가
        /// </summary>
        Add,

        /// <summary>
        /// 렌더러 삭제
        /// </summary>
        Remove
    }

    /// <summary>
    /// 렌더러의 작업 관련 데이터를 관리하는 클래스입니다.
    /// </summary>
    public class ElementTaskData : DispatcherTaskData
    {
        /// <summary>
        /// 작업을 생성한 주최를 가져옵니다.
        /// </summary>
        public new IRenderer Source => (IRenderer)base.Source;

        /// <summary>
        /// 작업 유형을 가져옵니다.
        /// </summary>
        public RendererTaskType TaskType { get; }

        private int index = -1;

        /// <summary>
        /// Element 렌더러 작업 데이터를 생성합니다.
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="source"></param>
        /// <param name="doAction"></param>
        /// <param name="undoAction"></param>
        /// <param name="destroyAction"></param>
        public ElementTaskData(
            RendererTaskType taskType,
            IRendererElement source,
            Action doAction,
            Action undoAction,
            Action destroyAction) :
            base(source, doAction, undoAction, destroyAction)
        {
            this.TaskType = taskType;
        }

        /// <summary>
        /// 현재 작업을 실행합니다.
        /// </summary>
        public override void Do()
        {
            if (TaskType == RendererTaskType.Remove)
            {
                // On WPF
                ObjectContentHelper.GetContent(
                    Source.RendererParent.Element,
                    null,
                    list =>
                    {
                        index = list.IndexOf(Source.Element);
                    });
            }

            base.Do();
        }

        /// <summary>
        /// 이전 작업으로 돌아갑니다.
        /// </summary>
        public override void Undo()
        {
            base.Undo();

            if (TaskType == RendererTaskType.Remove)
            {
                if (index == -1)
                    return;

                // On PObject
                ObjectContentHelper.GetContent(
                    Source.RendererParent.Model,
                    null,
                    list =>
                    {
                        list.Remove(Source.Model);
                        list.Insert(index, Source.Model);
                    });

                // On WPF
                ObjectContentHelper.GetContent(
                    Source.RendererParent.Element,
                    null,
                    list =>
                    {
                        list.Remove(Source.Element);
                        list.Insert(index, Source.Element);
                    });
            }
        }
    }
}
