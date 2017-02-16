using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DeXign.Extension;
using WPFExtension;

namespace DeXign.Editor.Controls
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
        private Rect beginBound;
        private Vector positionLimit;

        public ResizeThumb(FrameworkElement target)
        {
            this.Target = target;
            
            this.DragDelta += OnDragDelta;
            this.DragStarted += OnDragStated;
        }
        
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector scale = GetScale();

            double deltaX = e.HorizontalChange * scale.X;
            double deltaY = e.VerticalChange * scale.Y;

            if (Target.Parent is Canvas)
            {
                OnCanvasDragDelta(deltaX, deltaY);
            }
            else 
            {
                OnPanelDragDelta(deltaX, deltaY);
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
            {
                var position = Target.TranslatePoint(new Point(0, 0), (UIElement)Target.Parent);

                beginThickness = Target.Margin;
                beginBound = new Rect(position, (Size)beginSize);
            }

            positionLimit = new Vector(
                beginPosition.X + beginSize.X - Target.GetDesignMinWidth(),
                beginPosition.Y + beginSize.Y - Target.GetDesignMinHeight());
        }

        protected virtual Vector GetScale()
        {
            if (RenderTransform is ScaleTransform)
                return new Vector(
                    (RenderTransform as ScaleTransform).ScaleX,
                    (RenderTransform as ScaleTransform).ScaleY);

            return new Vector(1, 1);
        }

        private void OnPanelDragDelta(double deltaX, double deltaY)
        {
            bool marginLeft = 
                Target.HorizontalAlignment == HorizontalAlignment.Left ||
                Target.HorizontalAlignment == HorizontalAlignment.Stretch;

            bool marginRight =
                Target.HorizontalAlignment == HorizontalAlignment.Right ||
                Target.HorizontalAlignment == HorizontalAlignment.Stretch;

            bool marginTop =
                Target.VerticalAlignment == VerticalAlignment.Top ||
                Target.VerticalAlignment == VerticalAlignment.Stretch;

            bool marginBottom =
                Target.VerticalAlignment == VerticalAlignment.Bottom ||
                Target.VerticalAlignment == VerticalAlignment.Stretch;

            bool hasSizingLeft =
                ResizeDirection == ResizeGripDirection.Left ||
                ResizeDirection == ResizeGripDirection.BottomLeft ||
                ResizeDirection == ResizeGripDirection.TopLeft;

            bool hasSizingRight =
                ResizeDirection == ResizeGripDirection.Right ||
                ResizeDirection == ResizeGripDirection.BottomRight ||
                ResizeDirection == ResizeGripDirection.TopRight;

            bool hasSizingTop =
                ResizeDirection == ResizeGripDirection.Top ||
                ResizeDirection == ResizeGripDirection.TopLeft ||
                ResizeDirection == ResizeGripDirection.TopRight;

            bool hasSizingBottom =
                ResizeDirection == ResizeGripDirection.Bottom ||
                ResizeDirection == ResizeGripDirection.BottomLeft ||
                ResizeDirection == ResizeGripDirection.BottomRight;

            Thickness margin = Target.Margin;
            var targetParent = (FrameworkElement)Target.Parent;
            var position = Target.TranslatePoint(new Point(0, 0), targetParent);

            if (hasSizingTop)
            {
                // Center, Bottom
                if (!marginTop)
                    SizingHeight(-deltaY);

                // Stretch
                if (marginTop & marginBottom)
                {
                    margin.Bottom = targetParent.RenderSize.Height - position.Y - Target.RenderSize.Height;
                    margin.Top += deltaY;
                }

                // Top
                if (marginTop & !marginBottom)
                {
                    double sizedHeight = SizingHeight(-deltaY);

                    margin.Top = beginBound.Bottom - sizedHeight;
                }
            }

            if (hasSizingBottom)
            {
                // Center, Top
                if (!marginBottom)
                    SizingHeight(deltaY);

                // Stretch
                if (marginTop & marginBottom)
                {
                    margin.Top = position.Y;
                    margin.Bottom -= deltaY;
                }

                // Bottom
                if (marginBottom & !marginTop)
                {
                    double sizedHeight = SizingHeight(deltaY);

                    margin.Bottom = targetParent.RenderSize.Height - position.Y - sizedHeight;
                }
            }

            if (hasSizingLeft)
            {
                // Center, Right
                if (!marginLeft)
                    SizingWidth(-deltaX);

                // Stretch
                if (marginLeft & marginRight)
                {
                    margin.Right = targetParent.RenderSize.Width - position.X - Target.RenderSize.Width;
                    margin.Left += deltaX;
                }

                // Left
                if (marginLeft & !marginRight)
                {
                    double sizedWidth = SizingWidth(-deltaX);

                    margin.Left = beginBound.Right - sizedWidth;
                }
            }

            if (hasSizingRight)
            {
                // Center, Left
                if (!marginRight)
                    SizingWidth(deltaX);

                // Stretch
                if (marginLeft & marginRight)
                {
                    margin.Left = position.X;
                    margin.Right -= deltaX;
                }

                // Right
                if (marginRight & !marginLeft)
                {
                    double sizedWidth = SizingWidth(deltaX);

                    margin.Right = targetParent.RenderSize.Width - position.X - sizedWidth;
                }
            }
            
            Target.Margin = margin;
        }

        protected virtual void OnCanvasDragDelta(double deltaX, double deltaY)
        {
            switch (ResizeDirection)
            {
                #region < Resize Horizontal >
                case ResizeGripDirection.Left:
                    SizingWidth(-deltaX);
                    SizingX(-deltaX);
                    break;

                case ResizeGripDirection.Right:
                    SizingWidth(deltaX);
                    break;
                #endregion

                #region < Resize Vertical >
                case ResizeGripDirection.Top:
                    SizingHeight(-deltaY);
                    SizingY(-deltaY);
                    break;

                case ResizeGripDirection.Bottom:
                    SizingHeight(deltaY);
                    break;
                #endregion

                #region < Resize NEWS >
                case ResizeGripDirection.BottomRight:
                    SizingWidth(deltaX);
                    SizingHeight(deltaY);
                    break;

                case ResizeGripDirection.TopLeft:
                    SizingWidth(-deltaX);
                    SizingHeight(-deltaY);
                    SizingX(-deltaX);
                    SizingY(-deltaY);
                    break;
                #endregion

                #region < Resize WNSE >
                case ResizeGripDirection.BottomLeft:
                    SizingWidth(-deltaX);
                    SizingHeight(deltaY);
                    SizingX(-deltaX);
                    break;

                case ResizeGripDirection.TopRight:
                    SizingWidth(deltaX);
                    SizingHeight(-deltaY);
                    SizingY(-deltaY);
                    break;
                #endregion
            }
        }

        private double SizingWidth(double deltaX)
        {
            return Target.Width = Math.Max(
                Target.GetDesignMinWidth(), 
                Target.ActualWidth + deltaX);
        }

        private double SizingHeight(double deltaY)
        {
            return Target.Height = Math.Max(
                Target.GetDesignMinHeight(), 
                Target.ActualHeight + deltaY);
        }

        private double SizingX(double deltaX, bool isCanvas = true)
        {
            if (isCanvas)
            {
                double x;

                Canvas.SetLeft(
                    Target,
                    x = Math.Min(Canvas.GetLeft(Target) - deltaX, positionLimit.X));

                return x;
            }
            else
            {
                // TODO: Grid, StackaPanel, ScrollViewer
            }

            return -1;
        }

        private double SizingY(double deltaY, bool isCanvas = true)
        {
            if (isCanvas)
            {
                double x;

                Canvas.SetTop(
                    Target,
                    x = Math.Min(Canvas.GetTop(Target) - deltaY, positionLimit.Y));

                return x;
            }
            else
            {
                // TODO: Grid, StackaPanel, ScrollViewer
            }

            return -1;
        }
    }
}