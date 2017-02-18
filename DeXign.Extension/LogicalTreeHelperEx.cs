using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Extension
{
    public static class LogicalTreeHelperEx
    {
        public static IEnumerable<T> FindLogicalParents<T>(this Visual element, bool findAll = true) where T : Visual
        {
            return Finds<T>(element, ParentSetter, findAll);
        }

        private static void ParentSetter(DependencyObject visual, Queue<DependencyObject> visualQueue)
        {
            var parent = LogicalTreeHelper.GetParent(visual);

            if (parent != null)
                visualQueue.Enqueue(parent);
        }

        public static IEnumerable<T> FindLogicalChildrens<T>(this Visual element, bool findAll = true) where T : Visual
        {
            return Finds<T>(element, ChildrenSetter, findAll);
        }

        private static void ChildrenSetter(DependencyObject visual, Queue<DependencyObject> visualQueue)
        {
            foreach (DependencyObject child in LogicalTreeHelper.GetChildren(visual))
                visualQueue.Enqueue(child);
        }

        private static IEnumerable<T> Finds<T>(
            this Visual element,
            Action<DependencyObject, Queue<DependencyObject>> elementSetter,
            bool findAll = true) where T : Visual
        {
            var visualQueue = new Queue<DependencyObject>();
            visualQueue.Enqueue(element);

            while (visualQueue.Count > 0)
            {
                DependencyObject visual = visualQueue.Dequeue();

                if (visual is FrameworkElement)
                    (visual as FrameworkElement).ApplyTemplate();

                if (visual is T)
                {
                    yield return visual as T;

                    if (!findAll)
                        break;
                }

                elementSetter(visual, visualQueue);
            }
        }
    }
}
