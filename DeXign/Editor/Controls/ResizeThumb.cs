using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DeXign.Extension;
using WPFExtension;
using DeXign.Editor.Layer;

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
        public SelectionLayer Layer { get; set; }

        private Vector beginSize;
        private Vector beginPosition;
        private Thickness beginThickness;
        private Rect beginBound;
        private Vector positionLimit;

        public ResizeThumb(SelectionLayer layer)
        {
            this.Layer = layer;
            this.Target = layer.AdornedElement;
            
            this.DragDelta += OnDragDelta;
            this.DragStarted += OnDragStated;
        }
        
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector scale = GetScale();

            double deltaX = e.HorizontalChange * scale.X;
            double deltaY = e.VerticalChange * scale.Y;

            // Cancel Design Mode
            Layer.CancelNextInvert = true;

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
                switch (Layer.ClipData.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                    case VerticalAlignment.Bottom:
                        SizingHeight(-deltaY);
                        break;

                    case VerticalAlignment.Stretch:
                        margin.Bottom = targetParent.RenderSize.Height - position.Y - Target.RenderSize.Height;
                        margin.Top = Math.Min(
                            margin.Top + deltaY, 
                            beginBound.Bottom - Target.GetDesignMinHeight());
                        break;

                    case VerticalAlignment.Top:
                        double sizedHeight = SizingHeight(-deltaY);
                        margin.Top = beginBound.Bottom - sizedHeight;
                        break;
                }
            }

            if (hasSizingBottom)
            {
                switch (Layer.ClipData.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                    case VerticalAlignment.Top:
                        SizingHeight(deltaY);
                        break;

                    case VerticalAlignment.Stretch:
                        margin.Top = position.Y;
                        margin.Bottom -= deltaY;
                        break;

                    case VerticalAlignment.Bottom:
                        double sizedHeight = SizingHeight(deltaY);
                        margin.Bottom = targetParent.RenderSize.Height - position.Y - sizedHeight;
                        break;
                }
            }

            if (hasSizingLeft)
            {
                switch (Layer.ClipData.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                    case HorizontalAlignment.Right:
                        SizingWidth(-deltaX);
                        break;

                    case HorizontalAlignment.Stretch:
                        margin.Right = targetParent.RenderSize.Width - position.X - Target.RenderSize.Width;
                        margin.Left = Math.Min(
                            margin.Left + deltaX,
                            beginBound.Right - Target.GetDesignMinWidth());
                        break;

                    case HorizontalAlignment.Left:
                        double sizedWidth = SizingWidth(-deltaX);
                        margin.Left = beginBound.Right - sizedWidth;
                        break;
                }
            }

            if (hasSizingRight)
            {
                switch (Layer.ClipData.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                    case HorizontalAlignment.Left:
                        SizingWidth(deltaX);
                        break;

                    case HorizontalAlignment.Stretch:
                        margin.Left = position.X;
                        margin.Right -= deltaX;
                        break;

                    case HorizontalAlignment.Right:
                        double sizedWidth = SizingWidth(deltaX);
                        margin.Right = targetParent.RenderSize.Width - position.X - sizedWidth;
                        break;
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