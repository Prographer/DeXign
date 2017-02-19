using System;

namespace DeXign.Converter
{
    class EnumToEnumConverter<TEnum1, TEnum2> : BaseValueConverter<TEnum1, TEnum2>
        where TEnum1 : struct, IConvertible
        where TEnum2 : struct, IConvertible
    {
        public override TEnum2 Convert(TEnum1 value, object parameter)
        {
            int intValue = (int)Enum.ToObject(typeof(TEnum1), value);

            return (TEnum2)Enum.ToObject(typeof(TEnum2), intValue);
        }

        public override TEnum1 ConvertBack(TEnum2 value, object parameter)
        {
            int intValue = (int)Enum.ToObject(typeof(TEnum2), value);

            return (TEnum1)Enum.ToObject(typeof(TEnum1), intValue);
        }
    }
}
