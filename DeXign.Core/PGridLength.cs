using System;
using System.Windows;

namespace DeXign.Core
{
    [XForms("GridUnitType")]
    public enum PGridUnitType
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
            get { return new PGridLength(1, PGridUnitType.Auto); }
        }

        public static PGridLength Star
        {
            get { return new PGridLength(1, PGridUnitType.Star); }
        }

        public double Value { get; }

        public PGridUnitType GridUnitType { get; }

        public bool IsAbsolute
        {
            get { return GridUnitType == PGridUnitType.Absolute; }
        }

        public bool IsAuto
        {
            get { return GridUnitType == PGridUnitType.Auto; }
        }

        public bool IsStar
        {
            get { return GridUnitType == PGridUnitType.Star; }
        }

        public PGridLength(double value) : this(value, PGridUnitType.Absolute)
        {
        }

        public PGridLength(double value, PGridUnitType type)
        {
            if (value < 0 || double.IsNaN(value))
                throw new ArgumentException("value is less than 0 or is not a number", "value");
            if ((int)type < (int)PGridUnitType.Absolute || (int)type > (int)PGridUnitType.Auto)
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
            switch (GridUnitType)
            {
                case PGridUnitType.Absolute:
                    return Value.ToString();

                case PGridUnitType.Star:
                    return $"{Value}*";

                case PGridUnitType.Auto:
                    return "Auto";
            }

            return null;
        }

        public static implicit operator GridLength(PGridLength gridLength)
        {
            return new GridLength(
                gridLength.Value, 
                (GridUnitType)gridLength.GridUnitType);
        }
    }
}
