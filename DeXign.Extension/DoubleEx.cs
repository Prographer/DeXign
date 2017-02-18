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
    }
}
