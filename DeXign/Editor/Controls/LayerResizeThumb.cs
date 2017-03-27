using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shell;

using DeXign.Editor.Layer;
using DeXign.Extension;

using WPFExtension;
using System.Collections.Generic;
using DeXign.Controls;
using System.Windows.Input;

namespace DeXign.Editor.Controls
{
    class LayerResizeThumb : RelativeThumb
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

        public LayerResizeThumb(SelectionLayer layer)
        {
            this.Layer = layer;
            this.Target = layer.AdornedElement;

            this.RelativeTarget = layer.Storyboard;
        }
       
        protected override void OnDragDelta(double horizontalChange, double verticalChange)
        {
            // Cancel Design Mode
            Layer.CancelNextSelect();
            
            if (Layer.Parent is IStoryboard)
            {
                OnCanvasDragDelta(horizontalChange, verticalChange);
            }
            else
            {
                OnPanelDragDelta(horizontalChange, verticalChange);
            }
        }
        
        protected override void OnDragStarted(double horizontalOffset, double verticalOffset)
        {
            Layer.GuidelineFilter.Push(GetSizeGuidableLines);

            beginSize = (Vector)Target.RenderSize;

            if (Layer.Parent is IStoryboard)
            {
                beginPosition = new Vector(
                    Canvas.GetLeft(Target),
                    Canvas.GetTop(Target));
            }
            else
            {
                var position = Target.TranslatePoint(new Point(0, 0), (UIElement)Target.Parent);

                beginThickness = Layer.GetParentLayoutInfo().Margin;

                beginThickness.Left *= -1;
                beginThickness.Top *= -1;

                beginBound = new Rect(position, (Size)beginSize);
            }

            positionLimit = new Vector(
                beginPosition.X + beginSize.X - Target.GetDesignMinWidth(),
                beginPosition.Y + beginSize.Y - Target.GetDesignMinHeight());

            base.OnDragStarted(horizontalOffset, verticalOffset);
        }


        protected override void OnDragCompleted(double horizontalChange, double verticalChange)
        {
            Layer.GuidelineFilter.Pop();
            base.OnDragCompleted(horizontalChange, verticalChange);
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

            Thickness margin = beginThickness;
            var parentLayout = Layer.GetParentLayoutInfo();

            if (hasSizingTop)
            {
                switch (Layer.ClipData.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        SizingHeight(-deltaY * 2);
                        break;

                    case VerticalAlignment.Bottom:
                        {
                            double sizedHeight = SizingHeight(-deltaY);
                            margin.Top = Math.Min(parentLayout.Bound.Height - sizedHeight - margin.Bottom, 0);
                        }
                        break;

                    case VerticalAlignment.Stretch:
                        margin.Bottom = parentLayout.Bound.Height - parentLayout.Bound.Y - Target.RenderSize.Height;
                        margin.Top = Math.Min(
                            beginThickness.Top + deltaY, 
                            beginBound.Bottom - Target.GetDesignMinHeight());
                        break;

                    case VerticalAlignment.Top:
                        {
                            double sizedHeight = SizingHeight(-deltaY);
                            margin.Top = parentLayout.Bound.Height - sizedHeight - margin.Bottom;
                        }
                        break;
                }
            }

            if (hasSizingBottom)
            {
                switch (Layer.ClipData.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        SizingHeight(deltaY * 2);
                        break;
                        
                    case VerticalAlignment.Stretch:
                        margin.Bottom = beginThickness.Bottom - deltaY;
                        break;

                    case VerticalAlignment.Top:
                    case VerticalAlignment.Bottom:
                        {
                            double sizedHeight = SizingHeight(deltaY);
                            margin.Bottom = parentLayout.Bound.Height - parentLayout.Bound.Y - sizedHeight;

                            if (Layer.ClipData.VerticalAlignment == VerticalAlignment.Top)
                                margin.Bottom = Math.Min(margin.Bottom, 0);

                            if (Layer.Parent is IStackLayout)
                                margin.Bottom = beginThickness.Bottom;
                        }
                        break;
                }
            }

            if (hasSizingLeft)
            {
                switch (Layer.ClipData.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        SizingWidth(-deltaX * 2);
                        break;

                    case HorizontalAlignment.Right:
                        {
                            double sizedWidth = SizingWidth(-deltaX);
                            margin.Left = Math.Min(parentLayout.Bound.Width - sizedWidth - margin.Right, 0);
                        }
                        break;

                    case HorizontalAlignment.Stretch:
                        margin.Right = parentLayout.Margin.Right;
                        margin.Left = Math.Min(
                            beginThickness.Left + deltaX,
                            parentLayout.Bound.Width - parentLayout.Margin.Right - Target.GetDesignMinWidth());
                        break;

                    case HorizontalAlignment.Left:
                        {
                            double sizedWidth = SizingWidth(-deltaX);
                            margin.Left = beginBound.Right - sizedWidth;
                        }
                        break;
                }
            }

            if (hasSizingRight)
            {
                switch (Layer.ClipData.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        SizingWidth(deltaX * 2);
                        break;

                    case HorizontalAlignment.Stretch:
                        margin.Right = beginThickness.Right - deltaX;
                        break;

                    case HorizontalAlignment.Left:
                    case HorizontalAlignment.Right:
                        {
                            double sizedWidth = SizingWidth(deltaX);
                            margin.Right = parentLayout.Bound.Width - parentLayout.Bound.X - sizedWidth;

                            if (Layer.ClipData.HorizontalAlignment == HorizontalAlignment.Left)
                                margin.Right = Math.Min(margin.Right, 0);

                            if (Layer.Parent is IStackLayout)
                                margin.Right = beginThickness.Right;
                        }
                        break;
                }
            }

            Layer.SetMargin(margin);
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
            double width = Math.Max(
                Target.GetDesignMinWidth(),
                beginSize.X + deltaX);

            Layer.SetWidth(width);

            return width;
        }

        private double SizingHeight(double deltaY)
        {
            double height = Math.Max(
                Target.GetDesignMinHeight(),
                beginSize.Y + deltaY);

            Layer.SetHeight(height);

            return height;
        }

        private double SizingX(double deltaX, bool isCanvas = true)
        {
            double x;

            Canvas.SetLeft(
                Target,
                x = Math.Min(beginPosition.X - deltaX, positionLimit.X));

            return x;
        }

        private double SizingY(double deltaY, bool isCanvas = true)
        {
            double x;

            Canvas.SetTop(
                Target,
                x = Math.Min(beginPosition.Y - deltaY, positionLimit.Y));

            return x;
        }

        #region [ Guideline ]
        private IEnumerable<Guideline> GetSizeGuidableLines()
        {
            if (false)
            {
                yield return null;
            }
        }
        #endregion
    }
}