using System;
using System.ComponentModel;

namespace DeXign.Core
{
    [TypeConverter(typeof(LayoutAlignmentConverter))]
    [XForms("LayoutOptions")]
    public struct LayoutAlignment
    {
        int flags;

        public static readonly LayoutAlignment Start = new LayoutAlignment(LayoutOptions.Start, false);
        public static readonly LayoutAlignment Center = new LayoutAlignment(LayoutOptions.Center, false);
        public static readonly LayoutAlignment End = new LayoutAlignment(LayoutOptions.End, false);
        public static readonly LayoutAlignment Fill = new LayoutAlignment(LayoutOptions.Fill, false);
        public static readonly LayoutAlignment StartAndExpand = new LayoutAlignment(LayoutOptions.Start, true);
        public static readonly LayoutAlignment CenterAndExpand = new LayoutAlignment(LayoutOptions.Center, true);
        public static readonly LayoutAlignment EndAndExpand = new LayoutAlignment(LayoutOptions.End, true);
        public static readonly LayoutAlignment FillAndExpand = new LayoutAlignment(LayoutOptions.Fill, true);

        public LayoutAlignment(LayoutOptions alignment, bool expands)
        {
            var a = (int)alignment;

            if (a < 0 || a > 3)
                throw new ArgumentOutOfRangeException();

            flags = (int)alignment | (expands ? (int)LayoutExpandFlag.Expand : 0);
        }

        public LayoutOptions Alignment
        {
            get { return (LayoutOptions)(flags & 3); }
            set { flags = (flags & ~3) | (int)value; }
        }

        public bool Expands
        {
            get { return (flags & (int)LayoutExpandFlag.Expand) != 0; }
            set { flags = (flags & 3) | (value ? (int)LayoutExpandFlag.Expand : 0); }
        }

        public override bool Equals(object obj)
        {
            if (obj is LayoutAlignment)
                return (LayoutAlignment)obj == this;

            return false;
        }

        public static bool operator ==(LayoutAlignment l1, LayoutAlignment l2)
        {
            return l1.Alignment == l2.Alignment && l1.Expands == l2.Expands;
        }

        public static bool operator !=(LayoutAlignment l1, LayoutAlignment l2)
        {
            return !(l1 == l2);
        }
    }
}
