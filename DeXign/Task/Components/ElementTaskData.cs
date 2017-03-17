using System;
using System.Collections.Generic;

using DeXign.Editor;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Core.Logic;
using DeXign.Extension;
using DeXign.Editor.Logic;

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
        private Dictionary<IRenderer, List<(BindThumb Output, BindThumb Input)>> expressions;

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
            expressions = new Dictionary<IRenderer, List<(BindThumb Output, BindThumb Input)>>();

            this.TaskType = taskType;
        }

        /// <summary>
        /// 현재 작업을 실행합니다.
        /// </summary>
        public override void Do()
        {
            if (TaskType == RendererTaskType.Remove)
            {
                OnRemove();
            }
            else
            {
                OnAdd();
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
                OnRemoveRestore();
            }
            else
            {
                OnAddRestore();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            expressions.Clear();

            expressions = null;
        }

        private void OnAdd()
        {
            RestoreBinders();
        }

        private void OnAddRestore()
        {
            ReleaseBinders();
        }

        private void OnRemove()
        {
            // On WPF
            ObjectContentHelper.GetContent(
                Source.RendererParent.Element,
                null,
                list =>
                {
                    index = list.IndexOf(Source.Element);
                });

            ReleaseBinders();
        }

        private void OnRemoveRestore()
        {
            if (index != -1)
            {
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
            }
            
            RestoreBinders();
        }

        // 소스 및 하위의 모든 렌더러의 연결되었던 바인더를 복구합니다.
        private void RestoreBinders()
        {
            // Binder
            var layer = Source as StoryboardLayer;

            foreach (IRenderer renderer in RendererTreeHelper.FindChildrens<IRenderer>(Source, true))
            {
                if (expressions.ContainsKey(renderer))
                {
                    foreach (var expression in expressions[renderer])
                    {
                        layer.Storyboard.ConnectComponent(expression.Output, expression.Input);
                    }
                }
            }
        }

        // 소스 및 하위의 모든 렌더러와 연결된 바인더를 해제합니다.
        private void ReleaseBinders()
        {
            foreach (IRenderer renderer in RendererTreeHelper.FindChildrens<IRenderer>(Source, true))
            {
                var binderHost = renderer.ProvideValue() as PBinderHost;

                if (!expressions.ContainsKey(renderer))
                    expressions[renderer] = new List<(BindThumb Output, BindThumb Input)>();

                var list = expressions[renderer];
                list.Clear();

                // 모든 연결
                foreach (var expression in binderHost.Items.GetExpressions())
                {
                    var outputBinder = expression.Output as PBinder;
                    var inputBinder = expression.Input as PBinder;

                    var outputThumb = outputBinder.GetView<BindThumb>();
                    var inputThumb = inputBinder.GetView<BindThumb>();

                    list.Add((outputThumb, inputThumb));
                }

                binderHost.ReleaseAll();
            }
        }
    }
}
