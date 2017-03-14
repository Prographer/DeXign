using DeXign.Editor.Logic;
using System;
using System.Collections.Generic;

namespace DeXign.Editor.Renderer
{
    public static class RendererTreeHelper
    {
        /// <summary>
        /// 바인더와 연결된 모든 렌더러의 표현을 가져옵니다.
        /// </summary>
        /// <param name="sourceBinder"></param>
        /// <returns></returns>
        public static IEnumerable<(IRenderer Output, IRenderer Input)> FindAllBinderExpressions(this IRenderer sourceBinder, int depth = -1)
        {
            foreach (var expression in RendererManager.ResolveBinder(sourceBinder).FindAllNodeExpressions(depth))
            {
                yield return (expression.Output.GetRenderer(), expression.Input.GetRenderer());
            }
        }

        /// <summary>
        /// 바인더와 연결된 모든 렌더러를 가져옵니다.
        /// </summary>
        /// <param name="sourceBinder"></param>
        /// <returns></returns>
        public static IEnumerable<IRenderer> FindAllBinders(this IRenderer sourceBinder, int depth = -1)
        {
            foreach (var expression in RendererManager.ResolveBinder(sourceBinder).FindAllNodes(depth))
            {
                yield return expression.GetRenderer();
            }
        }

        /// <summary>
        /// 렌더러의 부모를 찾습니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="renderer"></param>
        /// <param name="withSource"></param>
        /// <param name="findAll"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindParents<T>(this IRenderer renderer, bool withSource = false, bool findAll = true)
            where T : IRenderer
        {
            return Finds<T>(renderer, ParentSetter, withSource, findAll);
        }

        private static void ParentSetter(IRenderer renderer, Queue<IRenderer> rendererQueue)
        {
            if (renderer.RendererParent != null)
                rendererQueue.Enqueue(renderer.RendererParent);
        }

        /// <summary>
        /// 렌더러의 자식을 재귀적으로 찾습니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="renderer"></param>
        /// <param name="withSource"></param>
        /// <param name="findAll"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindChildrens<T>(this IRenderer renderer, bool withSource = false, bool findAll = true)
            where T : IRenderer
        {
            return Finds<T>(renderer, ChildrenSetter, withSource, findAll);
        }

        private static void ChildrenSetter(IRenderer renderer, Queue<IRenderer> rendererQueue)
        {
            foreach (IRenderer child in renderer.RendererChildren)
                rendererQueue.Enqueue(child);
        }

        private static IEnumerable<T> Finds<T>(
            this IRenderer renderer,
            Action<IRenderer, Queue<IRenderer>> rendererSetter, 
            bool withSource = false,
            bool findAll = true)
            where T : IRenderer
        {
            var rendererQueue = new Queue<IRenderer>();
            rendererQueue.Enqueue(renderer);

            while (rendererQueue.Count > 0)
            {
                IRenderer item = rendererQueue.Dequeue();
                
                if (item is T result && (withSource || !renderer.Equals(item)))
                {
                    yield return result;

                    if (!findAll)
                        break;
                }

                rendererSetter(item, rendererQueue);
            }
        }
    }
}
