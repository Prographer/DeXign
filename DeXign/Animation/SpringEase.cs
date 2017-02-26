using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DeXign.Animation
{
    public class SpringEase : EasingFunctionBase
    {
        const double S = 1.70158;

        protected override Freezable CreateInstanceCore()
        {
            return new SpringEase();
        }

        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime * normalizedTime * ((S + 1) * normalizedTime - S);
        }
    }
}
