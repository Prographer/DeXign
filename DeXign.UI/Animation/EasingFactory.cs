using System;
using System.Windows.Media.Animation;

namespace DeXign.UI.Animation
{
    public static class EasingFactory
    {
        #region [ Static Local Variable ]
        // * Back Ease
        private static BackEase _backEaseOut;
        private static BackEase _backEaseIn;
        private static BackEase _backEaseInOut;

        // * Bounce Ease
        private static BounceEase _bounceEaseOut;
        private static BounceEase _bounceEaseIn;
        private static BounceEase _bounceEaseInOut;

        // * Circle Ease
        private static CircleEase _circleEaseOut;
        private static CircleEase _circleEaseIn;
        private static CircleEase _circleEaseInOut;

        // * Cubic Ease
        private static CubicEase _cubicEaseOut;
        private static CubicEase _cubicEaseIn;
        private static CubicEase _cubicEaseInOut;

        // * Elastic Ease
        private static ElasticEase _elasticEaseOut;
        private static ElasticEase _elasticEaseIn;
        private static ElasticEase _elasticEaseInOut;

        // * Exponential Ease
        private static ExponentialEase _exponentialEaseOut;
        private static ExponentialEase _exponentialEaseIn;
        private static ExponentialEase _exponentialEaseInOut;

        // * Power Ease
        private static PowerEase _powerEaseOut;
        private static PowerEase _powerEaseIn;
        private static PowerEase _powerEaseInOut;

        // * Quadratic Ease
        private static QuadraticEase _quadraticEaseOut;
        private static QuadraticEase _quadraticEaseIn;
        private static QuadraticEase _quadraticEaseInOut;

        // * Quartic Ease
        private static QuarticEase _quarticEaseOut;
        private static QuarticEase _quarticEaseIn;
        private static QuarticEase _quarticEaseInOut;

        // * Quintic Ease
        private static QuinticEase _quinticEaseOut;
        private static QuinticEase _quinticEaseIn;
        private static QuinticEase _quinticEaseInOut;

        // * Sine Ease
        private static SineEase _sineEaseOut;
        private static SineEase _sineEaseIn;
        private static SineEase _sineEaseInOut;

        // * Spring Ease
        private static SpringEase _springEaseOut;
        private static SpringEase _springEaseIn;
        private static SpringEase _springEaseInOut;

        #endregion

        #region [ Static Property ]
        // * Back Ease
        public static BackEase BackOut => CreateEase(ref _backEaseOut, EasingMode.EaseOut);
        public static BackEase BackIn => CreateEase(ref _backEaseIn, EasingMode.EaseIn);
        public static BackEase BackInOut => CreateEase(ref _backEaseInOut, EasingMode.EaseInOut);

        // * Bounce Ease
        public static BounceEase BounceOut => CreateEase(ref _bounceEaseOut, EasingMode.EaseOut);
        public static BounceEase BounceIn => CreateEase(ref _bounceEaseIn, EasingMode.EaseIn);
        public static BounceEase BounceInOut => CreateEase(ref _bounceEaseInOut, EasingMode.EaseInOut);

        // * Circle Ease
        public static CircleEase CircleOut => CreateEase(ref _circleEaseOut, EasingMode.EaseOut);
        public static CircleEase CircleIn => CreateEase(ref _circleEaseIn, EasingMode.EaseIn);
        public static CircleEase CircleInOut => CreateEase(ref _circleEaseInOut, EasingMode.EaseInOut);

        // * Cubic Ease
        public static CubicEase CubicOut => CreateEase(ref _cubicEaseOut, EasingMode.EaseOut);
        public static CubicEase CubicIn => CreateEase(ref _cubicEaseIn, EasingMode.EaseIn);
        public static CubicEase CubicInOut => CreateEase(ref _cubicEaseInOut, EasingMode.EaseInOut);

        // * Elastic Ease
        public static ElasticEase ElasticOut => CreateEase(ref _elasticEaseOut, EasingMode.EaseOut);
        public static ElasticEase ElasticIn => CreateEase(ref _elasticEaseIn, EasingMode.EaseIn);
        public static ElasticEase ElasticInOut => CreateEase(ref _elasticEaseInOut, EasingMode.EaseInOut);

        // * Exponential Ease
        public static ExponentialEase ExponentialOut => CreateEase(ref _exponentialEaseOut, EasingMode.EaseOut);
        public static ExponentialEase ExponentialIn => CreateEase(ref _exponentialEaseIn, EasingMode.EaseIn);
        public static ExponentialEase ExponentialInOut => CreateEase(ref _exponentialEaseInOut, EasingMode.EaseInOut);

        // * Power Ease
        public static PowerEase PowerOut => CreateEase(ref _powerEaseOut, EasingMode.EaseOut);
        public static PowerEase PowerIn => CreateEase(ref _powerEaseIn, EasingMode.EaseIn);
        public static PowerEase PowerInOut => CreateEase(ref _powerEaseInOut, EasingMode.EaseInOut);

        // * Quadratic Ease
        public static QuadraticEase QuadraticOut => CreateEase(ref _quadraticEaseOut, EasingMode.EaseOut);
        public static QuadraticEase QuadraticIn => CreateEase(ref _quadraticEaseIn, EasingMode.EaseIn);
        public static QuadraticEase QuadraticInOut => CreateEase(ref _quadraticEaseInOut, EasingMode.EaseInOut);

        // * Quartic Ease
        public static QuarticEase QuarticOut => CreateEase(ref _quarticEaseOut, EasingMode.EaseOut);
        public static QuarticEase QuarticIn => CreateEase(ref _quarticEaseIn, EasingMode.EaseIn);
        public static QuarticEase QuarticInOut => CreateEase(ref _quarticEaseInOut, EasingMode.EaseInOut);

        // * Quintic Ease
        public static QuinticEase QuinticOut => CreateEase(ref _quinticEaseOut, EasingMode.EaseOut);
        public static QuinticEase QuinticIn => CreateEase(ref _quinticEaseIn, EasingMode.EaseIn);
        public static QuinticEase QuinticInOut => CreateEase(ref _quinticEaseInOut, EasingMode.EaseInOut);

        // * Sine Ease
        public static SineEase SineOut => CreateEase(ref _sineEaseOut, EasingMode.EaseOut);
        public static SineEase SineIn => CreateEase(ref _sineEaseIn, EasingMode.EaseIn);
        public static SineEase SineInOut => CreateEase(ref _sineEaseInOut, EasingMode.EaseInOut);

        // * Spring Ease
        public static SpringEase SpringOut => CreateEase(ref _springEaseOut, EasingMode.EaseOut);
        public static SpringEase SpringIn => CreateEase(ref _springEaseIn, EasingMode.EaseIn);
        public static SpringEase SpringInOut => CreateEase(ref _springEaseInOut, EasingMode.EaseInOut);
        #endregion

        static T CreateEase<T>(ref T easing, EasingMode mode)
            where T : EasingFunctionBase
        {
            if (easing == null)
            {
                easing = Activator.CreateInstance<T>();
                easing.EasingMode = mode;
            }

            return easing;
        }
    }
}
