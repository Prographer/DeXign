using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core
{
    public static class DataTreeHelper
    {
        public static IEnumerable<T> FindParents<T>(this PObject model, bool findAll = true)
            where T : PObject
        {
            return Finds<T>(model, ParentSetter, findAll);
        }

        private static void ParentSetter(PObject model, Queue<PObject> modelQueue)
        {
            //if (model.RendererParent != null)
            //    modelQueue.Enqueue(model.RendererParent);
        }

        public static IEnumerable<T> FindChildrens<T>(this PObject model, bool findAll = true)
            where T : PObject
        {
            return Finds<T>(model, ChildrenSetter, findAll);
        }

        private static void ChildrenSetter(PObject model, Queue<PObject> modelQueue)
        {
            //foreach (PObject child in renderer.RendererChildren)
            //    rendererQueue.Enqueue(child);
        }

        private static IEnumerable<T> Finds<T>(
            this PObject model,
            Action<PObject, Queue<PObject>> modelQueue,
            bool findAll = true)
            where T : PObject
        {
            var rendererQueue = new Queue<PObject>();
            rendererQueue.Enqueue(model);

            while (rendererQueue.Count > 0)
            {
                PObject item = rendererQueue.Dequeue();

                if (item is T && !model.Equals(item))
                {
                    yield return (T)item;

                    if (!findAll)
                        break;
                }

                modelQueue(item, rendererQueue);
            }
        }
    }
}
