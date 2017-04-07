using System;
using System.Linq;
using System.Windows.Media;

using DeXign.Converter;
using DeXign.Extension;
using System.Collections.Generic;
using System.Windows.Data;

namespace DeXign.Controls
{
    public class MultiPropertyConverter : BaseMultiValueConverter<object, object>
    {
        public bool IsStable { get; private set; }

        public Type PropertyType { get; }

        public object[] Targets { get; }

        public IValueConverter this[object obj]
        {
            get { return converters[obj]; }
            set { converters[obj] = value; }
        }

        private Dictionary<object, IValueConverter> converters =
            new Dictionary<object, IValueConverter>();

        public MultiPropertyConverter(Type propertyType, object[] targets)
        {
            this.Targets = targets;
            this.PropertyType = propertyType;
        }

        public override object Convert(object[] values, object parameter)
        {
            object result = this.PropertyType.GetDefault();

            if (values.Length == 1)
            {
                this.IsStable = true;
                
                if (converters.TryGetValue(this.Targets[0], out IValueConverter converter))
                    result = converter.Convert(values[0], null, null, null);
                else
                    result = values[0];
            }
            else
            {
                object source = null;

                this.IsStable = false;

                for (int i = 0; i < values.Length; i++)
                {
                    object v = values[i];

                    if (converters.TryGetValue(this.Targets[i], out IValueConverter converter))
                        v = converter.Convert(v, null, null, null);

                    if (i == 0)
                    {
                        source = v;
                        continue;
                    }

                    this.IsStable |= ValueEquals(source, v);

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
                object v = value;

                if (converters.TryGetValue(this.Targets[i], out IValueConverter converter))
                    v = converter.ConvertBack(value, null, null, null);

                values[i] = v;
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
