using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WPFExtension;

namespace DeXign
{
    public static class AttachedAdorner
    {
        #region [ Dependency Property ]
        private static readonly DependencyProperty AdornerProperty =
           DependencyHelper.RegisterAttached<Adorner>();

        public static readonly DependencyProperty AdornerTypeProperty =
           DependencyHelper.RegisterAttached<Type>(
               new FrameworkPropertyMetadata(default(Type), PropertyChangedCallback));

        public static readonly DependencyProperty AdornerIndexProperty =
           DependencyHelper.RegisterAttached<int>(
               new FrameworkPropertyMetadata(0, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.Loaded += Loaded;
            }
            
            if (dependencyPropertyChangedEventArgs.Property == AdornerIndexProperty)
                SetLayerZOrder(frameworkElement, (int)dependencyPropertyChangedEventArgs.NewValue);
        }
        #endregion

        #region [ Static Local Variable ]
        private static MethodInfo setZOrderMethodInfo;
        #endregion

        #region [ Static Constructor ]
        static AttachedAdorner()
        {
            setZOrderMethodInfo = typeof(AdornerLayer).GetMethod("SetAdornerZOrder", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        #endregion

        #region [ AdornerLayer ]
        private static void SetLayerZOrder(FrameworkElement element, int index)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            Adorner adorner = GetAdorner(element);

            if (adornerLayer != null && adorner != null)
                setZOrderMethodInfo.Invoke(adornerLayer, new object[] { adorner, index });
        }

        private static void Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                var adorner = GetAdorner(frameworkElement) ?? Activator.CreateInstance(GetAdornerType(frameworkElement), frameworkElement) as Adorner;

                if (adorner != null && adorner.Parent == null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(frameworkElement);
                    adornerLayer.Add(adorner);

                    SetAdorner(frameworkElement, adorner);
                    SetLayerZOrder(frameworkElement, GetAdornerIndex(frameworkElement));
                }
            }
        }
        #endregion

        #region [ Get/Set Extended Method ]
        public static void SetAdornerType(DependencyObject element, Type value)
        {
            element.SetValue(AdornerTypeProperty, value);
        }

        public static Type GetAdornerType(DependencyObject element)
        {
            return (Type)element.GetValue(AdornerTypeProperty);
        }

        public static void SetAdornerIndex(DependencyObject element, int value)
        {
            element.SetValue(AdornerIndexProperty, value);
        }

        public static int GetAdornerIndex(DependencyObject element)
        {
            return (int)element.GetValue(AdornerIndexProperty);
        }

        public static Adorner GetAdorner(DependencyObject element)
        {
            return (Adorner)element.GetValue(AdornerProperty);
        }

        public static void SetAdorner(DependencyObject element, Adorner adorner)
        {
            element.SetValue(AdornerProperty, adorner);
        }
        #endregion
    }
}