using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WPFExtension;

namespace DeXign.Designer.Controls
{
    class ResizeThumb : Thumb
    {
        public static readonly DependencyProperty StrokeProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(Brushes.Black));

        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public ResizeGripDirection ResizeDirection { get; set; }

        public FrameworkElement Target { get; set; }

        private Vector beginSize;
        private Vector beginPosition;
        private Thickness beginThickness;
        private Vector positionLimit;

        public ResizeThumb(FrameworkElement target)
        {
            this.Target = target;
            
            this.DragDelta += OnDragDelta;
            this.DragStarted += OnDragStated;
        }
        
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Target.Parent is Canvas)
            {
                OnCanvasDragDelta(e);
            }
            else
            {
                // TODO
            }
        }

        private void OnDragStated(object sender, DragStartedEventArgs e)
        {
            beginSize = new Vector(
                Target.ActualWidth,
                Target.ActualHeight);

            if (Target.Parent is Canvas)
                beginPosition = new Vector(
                    Canvas.GetLeft(Target),
                    Canvas.GetTop(Target));
            else
                beginThickness = Target.Margin;

            positionLimit = new Vector(
                beginPosition.X + beginSize.X - Target.MinWidth,
                beginPosition.Y + beginSize.Y - Target.MinHeight);
        }

        protected virtual Vector GetScale()
        {
            if (RenderTransform is ScaleTransform)
                return new Vector(
                    (RenderTransform as ScaleTransform).ScaleX,
                    (RenderTransform as ScaleTransform).ScaleY);

            return new Vector(1, 1);
        }

        protected virtual void OnCanvasDragDelta(DragDeltaEventArgs e)
        {
            Vector scale = GetScale();

            double deltaX = e.HorizontalChange * scale.X;
            double deltaY = e.VerticalChange * scale.Y;

            bool sizingWidth = false;
            bool sizingHeight = false;
            bool sizingX = false;
            bool sizingY = false;

            switch (ResizeDirection)
            {
                case ResizeGripDirection.BottomRight:
                    sizingWidth = true;
                    sizingHeight = true;
                    break;

                case ResizeGripDirection.Bottom:
                    sizingHeight = true;
                    break;

                case ResizeGripDirection.BottomLeft:
                    deltaX *= -1;

                    sizingWidth = true;
                    sizingHeight = true;
                    sizingX = true;
                    break;

                case ResizeGripDirection.Left:
                    deltaX *= -1;

                    sizingWidth = true;
                    sizingX = true;
                    break;

                case ResizeGripDirection.Right:
                    sizingWidth = true;
                    break;

                case ResizeGripDirection.TopRight:
                    deltaY *= -1;

                    sizingWidth = true;
                    sizingHeight = true;
                    sizingY = true;
                    break;

                case ResizeGripDirection.Top:
                    deltaY *= -1;

                    sizingHeight = true;
                    sizingY = true;
                    break;

                case ResizeGripDirection.TopLeft:
                    deltaX *= -1;
                    deltaY *= -1;

                    sizingWidth = true;
                    sizingHeight = true;
                    sizingX = true;
                    sizingY = true;
                    break;
            }

            if (sizingWidth)
                Target.Width = Math.Max(Target.MinWidth, Target.ActualWidth + deltaX);

            if (sizingHeight)
                Target.Height = Math.Max(Target.MinHeight, Target.ActualHeight + deltaY);

            if (sizingX)
                Canvas.SetLeft(
                    Target,
                    Math.Min(Canvas.GetLeft(Target) - deltaX, positionLimit.X));

            if (sizingY)
                Canvas.SetTop(
                    Target,
                    Math.Min(Canvas.GetTop(Target) - deltaY, positionLimit.Y));
        }
    }
}