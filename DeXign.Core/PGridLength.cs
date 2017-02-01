using System;

namespace DeXign.Core
{
    [XForms("GridUnitType")]
    public enum GridUnitType
    {
        Absolute,
        Star,
        Auto
    }

    [XForms("GridLength")]
    public struct PGridLength
    {
        public static PGridLength Auto
        {
            get { return new PGridLength(1, GridUnitType.Auto); }
        }

        public static PGridLength Star
        {
            get { return new PGridLength(1, GridUnitType.Star); }
        }

        public double Value { get; }

        public GridUnitType GridUnitType { get; }

        public bool IsAbsolute
        {
            get { return GridUnitType == GridUnitType.Absolute; }
        }

        public bool IsAuto
        {
            get { return GridUnitType == GridUnitType.Auto; }
        }

        public bool IsStar
        {
            get { return GridUnitType == GridUnitType.Star; }
        }

        public PGridLength(double value) : this(value, GridUnitType.Absolute)
        {
        }

        public PGridLength(double value, GridUnitType type)
        {
            if (value < 0 || double.IsNaN(value))
                throw new ArgumentException("value is less than 0 or is not a number", "value");
            if ((int)type < (int)GridUnitType.Absolute || (int)type > (int)GridUnitType.Auto)
                throw new ArgumentException("type is not a valid GridUnitType", "type");

            Value = value;
            GridUnitType = type;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is PGridLength && Equals((PGridLength)obj);
        }

        bool Equals(PGridLength other)
        {
            return GridUnitType == other.GridUnitType && Math.Abs(Value - other.Value) < double.Epsilon;
        }

        public override int GetHashCode()
        {
            return GridUnitType.GetHashCode() * 397 ^ Value.GetHashCode();
        }

        public static implicit operator PGridLength(double absoluteValue)
        {
            return new PGridLength(absoluteValue);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Value, GridUnitType);
        }
    }
}
