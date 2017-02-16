using System.Windows;
using System.Windows.Data;

namespace DeXign.Extension
{
    public static class BindingEx
    {
        public static void SetBinding(
            DependencyObject source, DependencyProperty sourceProperty, 
            DependencyObject target, DependencyProperty targetProperty,
            BindingMode mode = BindingMode.TwoWay,
            IValueConverter converter = null)
        {
            BindingOperations.SetBinding(
                target, targetProperty,
                new Binding(sourceProperty.Name)
                {
                    Source = source,
                    Mode = mode,
                    Converter = converter
                });
        }

        public static void SetBinding(
            DependencyObject source, string path,
            DependencyObject target, DependencyProperty targetProperty,
            BindingMode mode = BindingMode.TwoWay,
            IValueConverter converter = null)
        {
            BindingOperations.SetBinding(
                target, targetProperty,
                new Binding(path)
                {
                    Source = source,
                    Mode = mode,
                    Converter = converter
                });
        }
    }
}
