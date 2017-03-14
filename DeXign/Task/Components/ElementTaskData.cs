using DeXign.Core.Controls;
using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using System;
using System.Collections.Generic;

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
        private List<IRenderer> inputs;
        private List<IRenderer> outputs;

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
            inputs = new List<IRenderer>();
            outputs = new List<IRenderer>();

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

                // Binder
                var binder = RendererManager.ResolveBinder(Source);

                outputs.Clear();
                inputs.Clear();

                // 나가는 연결
                foreach (BinderExpression expression in binder.GetOutputExpression())
                {
                    IRenderer inputRenderer = expression.Input.GetRenderer();

                    outputs.Add(inputRenderer);
                }

                // 들어오는 연결
                //  * 들어오는 연결(Input)은 Component만 가질 수 있음
                if (Source is IRendererComponent)
                {
                    foreach (BinderExpression expression in binder.GetInputExpression())
                    {
                        IRenderer outputRenderer = expression.Output.GetRenderer();

                        inputs.Add(outputRenderer);
                    }
                }

                binder.ReleaseAll();
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

                if (Source.RendererParent.Model != null)
                {
                    // On PObject
                    ObjectContentHelper.GetContent(
                        Source.RendererParent.Model,
                        null,
                        list =>
                        {
                            list.Remove(Source.Model);
                            list.Insert(index, Source.Model);
                        });
                }

                // On WPF
                ObjectContentHelper.GetContent(
                    Source.RendererParent.Element,
                    null,
                    list =>
                    {
                        list.Remove(Source.Element);
                        list.Insert(index, Source.Element);
                    });

                // Binder
                var layer = Source as StoryboardLayer;

                foreach (IRenderer inputRenerer in inputs)
                {
                    layer.Storyboard.ConnectComponent(inputRenerer, Source, BinderOptions.Trigger);
                }

                foreach (IRenderer outputRenderer in outputs)
                {
                    layer.Storyboard.ConnectComponent(Source, outputRenderer, BinderOptions.Trigger);
                }
            }
        }
    }
}
