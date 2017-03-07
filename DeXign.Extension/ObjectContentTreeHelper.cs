using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeXign.Extension
{
    public static class ObjectContentTreeHelper
    {
        public static IEnumerable<object> GetChildren(this object obj)
        {
            object content = obj.GetContent();

            // Object
            if (content is PropertyInfo)
                yield return (content as PropertyInfo).GetValue(obj);

            // List
            if (content is IList)
                foreach (object item in content as IList)
                    yield return item;
        }

        public static IEnumerable<T> FindContentChildrens<T>(this object obj, bool findAll = true)
        {
            return Finds<T>(obj, ChildrenSetter, findAll);
        }

        private static void ChildrenSetter(object visual, Queue<object> visualQueue)
        {
            foreach (object child in ObjectContentTreeHelper.GetChildren(visual))
                visualQueue.Enqueue(child);
        }

        private static IEnumerable<T> Finds<T>(
            this object element,
            Action<object, Queue<object>> elementSetter,
            bool findAll = true)
        {
            var visualQueue = new Queue<object>();
            visualQueue.Enqueue(element);

            while (visualQueue.Count > 0)
            {
                object content = visualQueue.Dequeue();
                
                if (content is T && !ReferenceEquals(element, content))
                {
                    yield return (T)content;

                    if (!findAll)
                        break;
                }

                elementSetter(content, visualQueue);
            }
        }
    }
}
