using System;
using System.Linq;
using System.Windows.Media;

using DeXign.Converter;
using DeXign.Extension;

namespace DeXign.Controls
{
    public class MultiPropertyConverter : BaseMultiValueConverter<object, object>
    {
        public bool IsStable { get; private set; }

        public Type PropertyType { get; }

        public MultiPropertyConverter(Type propertyType)
        {
            this.PropertyType = propertyType;
        }

        public override object Convert(object[] values, object parameter)
        {
            object result = this.PropertyType.GetDefault();

            if (values.Length == 1)
            {
                this.IsStable = true;

                result = values[0];
            }
            else
            {
                object source = values[0];

                this.IsStable = false;

                foreach (object value in values.Skip(1))
                {
                    this.IsStable |= ValueEquals(source, value);

                    if (!this.IsStable)
                        break;
                }

                if (this.IsStable)
                    result = source;
            }

            return result;
        }

        public override object[] ConvertBack(object value, int length, object parameter)
        {
            this.IsStable = true;

            object[] values = new object[length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = value;
            }

            return values;
        }

        private bool ValueEquals(object a, object b)
        {
            if (object.Equals(a, b))
                return true;

            if (a is SolidColorBrush aBrush && b is SolidColorBrush bBrush)
                return ValueEquals(aBrush.Color, bBrush.Color);

            return false;
        }
    }
}
