using System;
using System.Windows;
using System.Windows.Controls;

using DeXign.Core.Controls;

using WPFExtension;

namespace DeXign.Core
{
    [XForms("ColumnDefinition")]
    public class PColumnDefinition : PObject, IDefinition
    {
        public static readonly DependencyProperty WidthProperty = 
            DependencyHelper.Register();

        [XForms("Width")]
        public PGridLength Width
        {
            get { return this.GetValue<PGridLength>(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        
        public static implicit operator ColumnDefinition(PColumnDefinition definition)
        {
            return new ColumnDefinition()
            {
                Width = definition.Width
            };
        }

        #region [ IConvertiable ]
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(ColumnDefinition))
                return (ColumnDefinition)this;

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
