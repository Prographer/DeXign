﻿using System;
using System.Collections.Generic;

namespace DeXign.Editor.Renderer
{
    public static class RendererTreeHelper
    {
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
