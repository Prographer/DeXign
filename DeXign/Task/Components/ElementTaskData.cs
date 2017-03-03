using DeXign.Editor;
using DeXign.Extension;
using System;

namespace DeXign.Task
{
    public enum RendererTaskType
    {
        Add,
        Remove
    }

    public class ElementTaskData : DispatcherTaskData
    {
        public new IRenderer Source => (IRenderer)base.Source;

        public RendererTaskType TaskType { get; }

        private int index = -1;

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
