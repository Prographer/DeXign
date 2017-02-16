using DeXign.Core.Controls;
using System.Windows;
using DeXign.Extension;
using WPFExtension;
using System.Windows.Controls;
using System;

namespace DeXign.Core
{
    [XForms("RowDefinition")]
    public class PRowDefinition : PObject, IDefinition
    {
        public static DependencyProperty HeightProperty
            = DependencyHelper.Register();

        [XForms("Height")]
        public PGridLength Height
        {
            get { return GetValue<PGridLength>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static implicit operator RowDefinition(PRowDefinition definition)
        {
            return new RowDefinition()
            {
                Height = definition.Height
            };
        }

        #region [ IConvertiable ]
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(RowDefinition))
                return (RowDefinition)this;

            return null;
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
