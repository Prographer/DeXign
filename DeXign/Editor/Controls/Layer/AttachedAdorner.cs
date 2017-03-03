using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Editor.Layer
{
    public static class AttachedAdorner
    {
        public static readonly DependencyProperty AdornersProperty =
            DependencyHelper.RegisterAttached<List<Adorner>>();

        public static readonly DependencyProperty AdornerIndexProperty =
            DependencyHelper.RegisterAttached<int>();
        
        #region [ Static Local Variable ]
        private static MethodInfo setZOrderMethodInfo;
        private static List<FrameworkElement> pendingElements;
        #endregion

        #region [ Static Constructor ]
        static AttachedAdorner()
        {
            pendingElements = new List<FrameworkElement>();
            setZOrderMethodInfo = typeof(AdornerLayer).GetMethod("SetAdornerZOrder", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        #endregion

        #region [ AdornerLayer ]
        private static void SetLayerZOrder(Adorner adorner, int index)
        {
            var adornerLayer = GetAdornerLayer((FrameworkElement)adorner.AdornedElement);

            if (adornerLayer != null && adorner != null && adornerLayer.Equals(adorner.Parent))
                setZOrderMethodInfo.Invoke(adornerLayer, new object[] { adorner, index });
        }

        public static AdornerLayer GetAdornerLayer(this FrameworkElement element)
        {
            var decorator = element?
                .FindLogicalParents<AdornerDecorator>()
                .FirstOrDefault();

            return decorator?.AdornerLayer;
        }
        #endregion

        #region [ Extension ]
        private static List<Adorner> GetAdorners(this FrameworkElement element)
        {
            return (List<Adorner>)element.GetValue(AdornersProperty);
        }

        public static void SetAdornerIndex(this Adorner adorner, int index)
        {
            var parentLayer = GetAdornerLayer((FrameworkElement)adorner.AdornedElement);

            if (parentLayer != null)
                SetLayerZOrder(adorner, index);

            adorner.SetValue(AdornerIndexProperty, index);
        }

        public static int GetAdornerIndex(this Adorner adorner)
        {
            return (int)adorner.GetValue(AdornerIndexProperty);
        }

        public static void AddAdorner(this FrameworkElement element, Adorner adorner)
        {
            var items = element.GetAdorners();

            if (items == null)
            {
                items = new List<Adorner>();
                element.SetValue(AdornersProperty, items);
            }

            if (!items.Contains(adorner))
            {
                items.Add(adorner);

                ApplyAdorner(element, adorner);
            }
        }

        public static void RemoveAdorner(this FrameworkElement element, Adorner adorner)
        {
            var items = element.GetAdorners();
            
            items?.Remove(adorner);
            element.GetAdornerLayer()?.Remove(adorner);
        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var items = element.GetAdorners();

            if (pendingElements.Contains(element))
            {
                element.Loaded -= Element_Loaded;

                pendingElements.Remove(element);
            }

            if (items == null)
                return;

            var layer = GetAdornerLayer(element);

            foreach (Adorner item in items)
                ApplyAdornerCore(element, item);
        }

        private static void ApplyAdorner(FrameworkElement element, Adorner adorner)
        {
            if (element.IsLoaded)
            {
                ApplyAdornerCore(element, adorner);
            }
            else
            {
                if (!pendingElements.Contains(element))
                {
                    pendingElements.Add(element);
                    element.Loaded += Element_Loaded;
                }
            }
        }

        private static void ApplyAdornerCore(FrameworkElement element, Adorner adorner)
        {
            var layer = GetAdornerLayer(element);

            layer.Add(adorner);
            SetLayerZOrder(adorner, adorner.GetAdornerIndex());
        }
        #endregion
    }
}