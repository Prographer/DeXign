using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Extension
{
    public static class VisualTreeHelperEx
    {
        public static IEnumerable<T> FindVisualParents<T>(this DependencyObject element, bool findAll = true) 
            where T : DependencyObject
        {
            return Finds<T>(element, ParentSetter, findAll);
        }

        private static void ParentSetter(DependencyObject visual, Queue<DependencyObject> visualQueue)
        {
            var parent = VisualTreeHelper.GetParent(visual);

            if (parent != null)
                visualQueue.Enqueue(parent);
        }

        public static IEnumerable<T> FindVisualChildrens<T>(this DependencyObject element, bool findAll = true) 
            where T : DependencyObject
        {
            return Finds<T>(element, ChildrenSetter, findAll);
        }

        private static void ChildrenSetter(DependencyObject visual, Queue<DependencyObject> visualQueue)
        {
            int count = VisualTreeHelper.GetChildrenCount(visual);

            for (int i = 0; i < count; i++)
            {
                visualQueue.Enqueue(
                    VisualTreeHelper.GetChild(visual, i));
            }
        }

        private static IEnumerable<T> Finds<T>(
            this DependencyObject element, 
            Action<DependencyObject, Queue<DependencyObject>> elementSetter,
            bool findAll = true) 
            where T : DependencyObject
        {
            var visualQueue = new Queue<DependencyObject>();
            visualQueue.Enqueue(element);

            while (visualQueue.Count > 0)
            {
                DependencyObject visual = visualQueue.Dequeue();

                if (visual is FrameworkElement frameworkElement)
                    frameworkElement.ApplyTemplate();

                if (visual is T result)
                {
                    yield return result;

                    if (!findAll)
                        break;
                }

                elementSetter(visual, visualQueue);
            }
        }
    }
}
