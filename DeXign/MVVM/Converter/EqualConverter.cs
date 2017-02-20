using DeXign.Extension;
using System;

namespace DeXign.Converter
{
    class EqualConverter : BaseValueConverter<object, bool>
    {
        public object TargetValue { get; set; }

        public override bool Convert(object value, object parameter)
        {
            return value.Equals(TargetValue);
        }

        public override object ConvertBack(bool value, object parameter)
        {
            return value ? TargetValue : TargetValue.GetType().GetDefault();
        }
    }
}
