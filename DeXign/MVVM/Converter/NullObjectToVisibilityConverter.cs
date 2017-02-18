using System;
using System.Windows;

namespace DeXign.Converter
{
    class NullObjectToVisibilityConverter : BaseValueConverter<object, Visibility>
    {
        public Visibility NullValue { get; set; } = Visibility.Collapsed;

        public Visibility NotNullValue { get; set; } = Visibility.Visible;

        public override Visibility Convert(object value, object parameter)
        {
            return (value == null ? NullValue : NotNullValue);
        }

        public override object ConvertBack(Visibility value, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
