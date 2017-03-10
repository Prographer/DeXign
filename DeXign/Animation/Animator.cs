using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DeXign.Animation
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
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = easing
            };

            animation.Completed += (s, e) =>
            {
                element.SetValue(property, element.GetValue(property));
                element.StopAnimation(property);

                completedEvent?.Invoke(s, e);
            };

            element.BeginAnimation(property, animation);
        }

        /// <summary>
        /// 진행중인 애니메이션을 중단합니다.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        public static void StopAnimation(this UIElement element, DependencyProperty property)
        {
            element.BeginAnimation(property, null);
        }
    }
}
