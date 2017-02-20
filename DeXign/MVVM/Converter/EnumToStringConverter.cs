using DeXign.Extension;
using System;

namespace DeXign.Converter
{
    class EnumToStringConverter<T> : BaseValueConverter<T, string>
        where T : struct, IConvertible
    {
        public override string Convert(T value, object parameter)
        {
            return value.ToString();
        }

        public override T ConvertBack(string value, object parameter)
        {
            return value.ToEnum<T>().Value;
        }
    }
}
