using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;

namespace DeXign.Converter
{
    public abstract class BaseMultiValueConverter<TFrom, TTo> : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(values.Cast<TFrom>().ToArray(), parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return ConvertBack((TTo)value, targetTypes.Length, parameter).Cast<object>().ToArray();
        }

        public abstract TTo Convert(TFrom[] value, object parameter);
        public abstract TFrom[] ConvertBack(TTo value, int length, object parameter);
    }
}
