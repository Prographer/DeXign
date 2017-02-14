using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DeXign.Editor.Layer
{
    partial class SelectionLayer : StoryboardLayer
    {
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (DesignMode == DesignMode.None)
                AnimateFrameThickness(4);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            AnimateFrameThickness(0, 250);
        }

        protected void AnimateFrameThickness(double thickness, double duration = 300)
        {
            var thicknessAnim = new DoubleAnimation(
                thickness,
                new Duration(TimeSpan.FromMilliseconds(duration)))
            {
                EasingFunction = new CircleEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            this.BeginAnimation(SelectionLayer.FrameThicknessProperty, thicknessAnim);
        }
    }
}
