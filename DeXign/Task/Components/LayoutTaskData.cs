using DeXign.Editor;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using System;
using System.Windows.Documents;

namespace DeXign.Task
{
    public class LayoutTaskData : ElementTaskData
    {
        public LayoutTaskData(
            RendererTaskType taskType,
            IRendererLayout source, 
            Action doAction, 
            Action undoAction,
            Action destroyAction) :
            base(taskType, source, doAction, undoAction, destroyAction)
        {
        }

        public override void Do()
        {
            base.Do();

            if (TaskType == RendererTaskType.Remove)
            {
                foreach (IRenderer child in RendererTreeHelper.FindChildrens<IRenderer>(Source))
                    child.Element.RemoveAdorner((Adorner)child);
            }
        }

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