using DeXign.Core;
using DeXign.Extension;
using System;

namespace DeXign.Converter
{
    class DisplayNameConverter : BaseValueConverter<object, string>
    {
        public override string Convert(object value, object parameter)
        {
            var attr = value?.GetAttribute<DesignElementAttribute>();

            if (attr == null)
                return null;

            return attr.DisplayName;
        }

        public override object ConvertBack(string value, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
