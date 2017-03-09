using DeXign.Editor;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using System;
using System.Windows.Documents;

namespace DeXign.Task
{
    /// <summary>
    /// 렌더러의 작업 관련 데이터를 관리하는 클래스입니다.
    /// </summary>
    public class LayoutTaskData : ElementTaskData
    {
        /// <summary>
        /// Layout 렌더러 작업 데이터를 생성합니다.
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="source"></param>
        /// <param name="doAction"></param>
        /// <param name="undoAction"></param>
        /// <param name="destroyAction"></param>
        public LayoutTaskData(
            RendererTaskType taskType,
            IRendererLayout source, 
            Action doAction, 
            Action undoAction,
            Action destroyAction) :
            base(taskType, source, doAction, undoAction, destroyAction)
        {
        }

        /// <summary>
        /// 현재 작업을 실행합니다.
        /// </summary>
        public override void Do()
        {
            base.Do();

            if (TaskType == RendererTaskType.Remove)
            {
                foreach (IRenderer child in RendererTreeHelper.FindChildrens<IRenderer>(Source))
                    child.Element.RemoveAdorner((Adorner)child);
            }
        }

        /// <summary>
        /// 이전 작업으로 돌아갑니다.
        /// </summary>
        public override void Undo()
        {
            base.Undo();

            if (TaskType == RendererTaskType.Remove)
            {
                foreach (IRenderer child in RendererTreeHelper.FindChildrens<IRenderer>(Source))
                    child.Element.AddAdorner((Adorner)child);
            }
        }
    }
}