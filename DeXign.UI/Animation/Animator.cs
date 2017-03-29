using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DeXign.UI.Animation
{
    public static class Animator
    {
        /// <summary>
        /// Double 애니메이션을 실행합니다.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        /// <param name="toValue"></param>
        /// <param name="duration"></param>
        /// <param name="easing"></param>
        /// <param name="completedEvent"></param>
        public static void BeginDoubleAnimation(
            this UIElement element, 
            DependencyProperty property, 
            double toValue, 
            double duration = 400, 
            IEasingFunction easing = null,
            EventHandler completedEvent = null)
        {
            double fromValue = (double)element.GetValue(property);

            var animation = new DoubleAnimation()
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = easing,
                FillBehavior = FillBehavior.HoldEnd
            };

            if (completedEvent != null)
                animation.Completed += completedEvent;

            element.BeginAnimation(property, animation);
        }

        /// <summary>
        /// Double 애니메이션을 실행합니다.
        /// </summary>
        /// <param name="animatable"></param>
        /// <param name="property"></param>
        /// <param name="toValue"></param>
        /// <param name="duration"></param>
        /// <param name="easing"></param>
        /// <param name="completedEvent"></param>
        public static void BeginDoubleAnimation(
            this Animatable animatable,
            DependencyProperty property,
            double toValue,
            double duration = 400,
            IEasingFunction easing = null,
            EventHandler completedEvent = null)
        {
            double fromValue = (double)animatable.GetValue(property);

            var animation = new DoubleAnimation()
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = easing,
                FillBehavior = FillBehavior.HoldEnd
            };

            if (completedEvent != null)
                animation.Completed += completedEvent;

            animatable.BeginAnimation(property, animation);
        }

        /// <summary>
        /// 진행중인 애니메이션을 중단합니다.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        public static void StopAnimation(this UIElement element, DependencyProperty property)
        {
            element.SetValue(property, element.GetValue(property));
            element.BeginAnimation(property, null);
        }

        /// <summary>
        /// 진행중인 애니메이션을 중단합니다.
        /// </summary>
        /// <param name="animatable"></param>
        /// <param name="property"></param>
        public static void StopAnimation(this Animatable animatable, DependencyProperty property)
        {
            animatable.SetValue(property, animatable.GetValue(property));
            animatable.BeginAnimation(property, null);
        }
    }
}
