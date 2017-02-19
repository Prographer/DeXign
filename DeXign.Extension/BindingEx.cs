using System.Windows;
using System.Windows.Data;

namespace DeXign.Extension
{
    public static class BindingEx
    {
        public static Binding SetBinding(
            DependencyObject source, DependencyProperty sourceProperty, 
            DependencyObject target, DependencyProperty targetProperty,
            BindingMode mode = BindingMode.TwoWay,
            IValueConverter converter = null)
        {
            Binding result;

            BindingOperations.SetBinding(
                target, targetProperty,
                result = new Binding(sourceProperty.Name)
                {
                    Source = source,
                    Mode = mode,
                    Converter = converter
                });

            return result;
        }

        public static Binding SetBinding(
            DependencyObject source, string path,
            DependencyObject target, DependencyProperty targetProperty,
            BindingMode mode = BindingMode.TwoWay,
            IValueConverter converter = null)
        {
            Binding result;

            BindingOperations.SetBinding(
                target, targetProperty,
                result = new Binding(path)
                {
                    Source = source,
                    Mode = mode,
                    Converter = converter
                });

            return result;
        }
    }
}
