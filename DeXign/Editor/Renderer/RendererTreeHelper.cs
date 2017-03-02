using System;
using System.Collections.Generic;

namespace DeXign.Editor.Renderer
{
    public static class RendererTreeHelper
    {
        public static IEnumerable<T> FindParents<T>(this IRenderer element, bool findAll = true)
        {
            return Finds<T>(element, ParentSetter, findAll);
        }

        private static void ParentSetter(IRenderer renderer, Queue<IRenderer> rendererQueue)
        {
            if (renderer.RendererParent != null)
                rendererQueue.Enqueue(renderer.RendererParent);
        }

        public static IEnumerable<T> FindChildrens<T>(this IRenderer renderer, bool findAll = true)
        {
            return Finds<T>(renderer, ChildrenSetter, findAll);
        }

        private static void ChildrenSetter(IRenderer renderer, Queue<IRenderer> rendererQueue)
        {
            foreach (IRenderer child in renderer.RendererChildren)
                rendererQueue.Enqueue(child);
        }

        private static IEnumerable<T> Finds<T>(
            this IRenderer renderer,
            Action<IRenderer, Queue<IRenderer>> rendererSetter,
            bool findAll = true)
        {
            var rendererQueue = new Queue<IRenderer>();
            rendererQueue.Enqueue(renderer);

            while (rendererQueue.Count > 0)
            {
                IRenderer item = rendererQueue.Dequeue();
                
                if (item is T)
                {
                    yield return (T)item;

                    if (!findAll)
                        break;
                }

                rendererSetter(item, rendererQueue);
            }
        }
    }
}
