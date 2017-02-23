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
            UpdateSourceTrigger sourceTrigger = UpdateSourceTrigger.Default,
            IValueConverter converter = null)
        {
            Binding result;

            BindingOperations.SetBinding(
                target, targetProperty,
                result = new Binding(sourceProperty.Name)
                {
                    Source = source,
                    Mode = mode,
                    Converter = converter,
                    UpdateSourceTrigger = sourceTrigger
                });

            return result;
        }

        public static Binding SetBinding(
            DependencyObject source, string path,
            DependencyObject target, DependencyProperty targetProperty,
            BindingMode mode = BindingMode.TwoWay,
            UpdateSourceTrigger sourceTrigger = UpdateSourceTrigger.Default,
            IValueConverter converter = null)
        {
            Binding result;

            BindingOperations.SetBinding(
                target, targetProperty,
                result = new Binding(path)
                {
                    Source = source,
                    Mode = mode,
                    Converter = converter,
                    UpdateSourceTrigger = sourceTrigger
                });

            return result;
        }

        public static Binding TryBinding(
            DependencyObject source, string path,
            DependencyObject target, DependencyProperty targetProperty,
            BindingMode mode = BindingMode.TwoWay,
            UpdateSourceTrigger sourceTrigger = UpdateSourceTrigger.Default,
            IValueConverter converter = null)
        {
            var sourceProperty = source.FindDependencyProperty(path);

            if (sourceProperty != null)
            {
                return BindingEx.SetBinding(
                    source, sourceProperty,
                    target, targetProperty,
                    mode, sourceTrigger, converter);
            }

            return null;
        }

        public static Binding TryBinding(
            DependencyObject source, DependencyProperty sourceProperty,
            DependencyObject target, string path,
            BindingMode mode = BindingMode.TwoWay,
            UpdateSourceTrigger sourceTrigger = UpdateSourceTrigger.Default,
            IValueConverter converter = null)
        {
            var targetProperty = target.FindDependencyProperty(path);

            if (targetProperty != null)
            {
                return BindingEx.SetBinding(
                    source, sourceProperty,
                    target, targetProperty,
                    mode, sourceTrigger, converter);
            }

            return null;
        }
    }
}
