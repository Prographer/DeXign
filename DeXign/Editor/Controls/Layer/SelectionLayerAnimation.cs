using System;
using System.Windows.Input;

using DeXign.Animation;

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

        protected void AnimateFrameThickness(double thickness, double duration = 300, EventHandler completed = null)
        {
            this.StopAnimation(
                SelectionLayer.FrameThicknessProperty);

            this.BeginDoubleAnimation(
                SelectionLayer.FrameThicknessProperty, 
                thickness, duration, EasingFactory.CircleOut, completed);
        }
    }
}
