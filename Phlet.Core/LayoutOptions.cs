using System;
using System.ComponentModel;

namespace Phlet.Core
{
    [TypeConverter(typeof(LayoutOptionsConverter))]
    [XForms("LayoutOptions")]
    public struct LayoutOptions
    {
        int flags;

        public static readonly LayoutOptions Start = new LayoutOptions(LayoutAlignment.Start, false);
        public static readonly LayoutOptions Center = new LayoutOptions(LayoutAlignment.Center, false);
        public static readonly LayoutOptions End = new LayoutOptions(LayoutAlignment.End, false);
        public static readonly LayoutOptions Fill = new LayoutOptions(LayoutAlignment.Fill, false);
        public static readonly LayoutOptions StartAndExpand = new LayoutOptions(LayoutAlignment.Start, true);
        public static readonly LayoutOptions CenterAndExpand = new LayoutOptions(LayoutAlignment.Center, true);
        public static readonly LayoutOptions EndAndExpand = new LayoutOptions(LayoutAlignment.End, true);
        public static readonly LayoutOptions FillAndExpand = new LayoutOptions(LayoutAlignment.Fill, true);

        public LayoutOptions(LayoutAlignment alignment, bool expands)
        {
            var a = (int)alignment;

            if (a < 0 || a > 3)
                throw new ArgumentOutOfRangeException();

            flags = (int)alignment | (expands ? (int)LayoutExpandFlag.Expand : 0);
        }

        public LayoutAlignment Alignment
        {
            get { return (LayoutAlignment)(flags & 3); }
            set { flags = (flags & ~3) | (int)value; }
        }

        public bool Expands
        {
            get { return (flags & (int)LayoutExpandFlag.Expand) != 0; }
            set { flags = (flags & 3) | (value ? (int)LayoutExpandFlag.Expand : 0); }
        }

        public override bool Equals(object obj)
        {
            if (obj is LayoutOptions)
                return (LayoutOptions)obj == this;

            return false;
        }

        public static bool operator ==(LayoutOptions l1, LayoutOptions l2)
        {
            return l1.Alignment == l2.Alignment && l1.Expands == l2.Expands;
        }

        public static bool operator !=(LayoutOptions l1, LayoutOptions l2)
        {
            return !(l1 == l2);
        }
    }
}
