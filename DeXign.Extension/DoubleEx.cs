using System;

namespace DeXign.Extension
{
    public static class DoubleEx
    {
        public static double Clean(this double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
                return 0;

            if (Math.Abs(value) < 0.001)
                value = 0;

            return value;
        }

        public static bool EpsilonEqauls(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= double.Epsilon;
        }

        public static double ToDouble(this string value)
        {
            if (TryToDouble(value, out double result))
                return result;

            throw new Exception();
        }

        public static bool TryToDouble(this string value, out double result)
        {
            value = value.ToLower();

            if (value == "auto" || value == "nan" || value.Contains("ÀÚµ¿"))
            {
                result = double.NaN;
                return true;
            }

            if (double.TryParse(value, out double dValue))
            {
                result = dValue;
                return true;
            }

            result = -1;

            return false;
        }
    }
}