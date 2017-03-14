using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using DeXign.Controls;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using System.Windows.Input;

namespace DeXign.Editor.Controls
{
    class LayerMoveThumb : RelativeThumb
    {
        public event EventHandler Moved;

        public FrameworkElement Target { get; set; }
        public SelectionLayer ParentLayer { get; set; }

        Point beginPosition;
        Thickness beginMargin;
        
        public LayerMoveThumb(SelectionLayer layer)
        {
            this.ParentLayer = layer;
            this.Target = layer.AdornedElement;

            this.RelativeTarget = layer.Storyboard;
        }

        private IEnumerable<Guideline> GetSizeGuidableLines()
        {
            if (ParentLayer.Parent is IStoryboard)
            {
                
            }
            else
            {
                Point vPosition = Point.Add(beginPosition, (Vector)PreviousDelta);
                Rect vBound = new Rect(
                    ParentLayer.Parent.Element.TranslatePoint(vPosition, ParentLayer.Storyboard),
                    Target.RenderSize);

                // top
                yield return new Guideline(
                    new Point(vBound.X, vBound.Y),
                    new Point(vBound.Right, vBound.Y))
                {
                    Direction = GuidelineDirection.Top
                };

                // left
                yield return new Guideline(
                    new Point(vBound.X, vBound.Y), 
                    new Point(vBound.X, vBound.Bottom))
                {
                    Direction = GuidelineDirection.Left
                };

                // bottom
                yield return new Guideline(
                    new Point(vBound.X, vBound.Bottom),
                    new Point(vBound.Right, vBound.Bottom))
                {
                    Direction = GuidelineDirection.Bottom
                };

                // right
                yield return new Guideline(
                    new Point(vBound.Right, vBound.Top),
                    new Point(vBound.Right, vBound.Bottom))
                {
                    Direction = GuidelineDirection.Right
                };
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (ParentLayer.DesignMode == DesignMode.Trigger)
            {
                e.Handled = true;
            }
        }

        protected override void OnDragStarted(double horizontalOffset, double verticalOffset)
        {
            // Filter Push
            ParentLayer.GuidelineFilter.Push(GetSizeGuidableLines);

            beginPosition = Target.TranslatePoint(new Point(), ParentLayer.Parent.Element);

            if (ParentLayer.Parent is IStoryboard == false)
                beginMargin = Target.Margin;
        }

        protected override void OnDragCompleted(double horizontalChange, double verticalChange)
        {
            // Filter Pop
            ParentLayer.GuidelineFilter.Pop();

            base.OnDragCompleted(horizontalChange, verticalChange);
        }

        protected override void OnDragDelta(double horizontalChange, double verticalChange)
        {
            ParentLayer.CancelNextSelect();

            if (ParentLayer.Parent is IStoryboard)
            {
                Point canvasPosition = ApplyPositionDelta(
                    new Point(horizontalChange, verticalChange));

                Canvas.SetLeft(Target, canvasPosition.X);
                Canvas.SetTop(Target, canvasPosition.Y);
            }
            else
            {
                Thickness margin = ApplyMarginDelta(
                    new Point(horizontalChange, verticalChange));
                
                // Snap with Set
                ParentLayer.SetMargin(margin);
            }

            Moved?.Invoke(this, null);
        }

        private Point ApplyPositionDelta(Point delta)
        {
            return Point.Add(beginPosition, (Vector)delta);
        }

        private Thickness ApplyMarginDelta(Point delta)
        {
            Thickness margin = beginMargin;
            
            bool allowVertical = true;
            bool allowHorizontal = true;

            if (ParentLayer.Parent is StackLayoutRenderer stackRenderer)
            {
                allowVertical =
                    (stackRenderer.Element.Orientation == Orientation.Horizontal);

                allowHorizontal =
                    (stackRenderer.Element.Orientation == Orientation.Vertical);
            }

            if (allowVertical)
            {
                switch (ParentLayer.ClipData.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        margin.Top = beginMargin.Top + delta.Y;
                        break;

                    case VerticalAlignment.Bottom:
                        margin.Bottom = beginMargin.Bottom - delta.Y;
                        break;

                    case VerticalAlignment.Stretch:
                    case VerticalAlignment.Center:
                        margin.Top = beginMargin.Top + delta.Y;
                        margin.Bottom = beginMargin.Bottom - delta.Y;
                        break;
                }
            }

            if (allowHorizontal)
            {
                switch (ParentLayer.ClipData.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        margin.Left = beginMargin.Left + delta.X;
                        break;

                    case HorizontalAlignment.Right:
                        margin.Right = beginMargin.Right - delta.X;
                        break;

                    case HorizontalAlignment.Stretch:
                    case HorizontalAlignment.Center:
                        margin.Left = beginMargin.Left + delta.X;
                        margin.Right = beginMargin.Right - delta.X;
                        break;
                }
            }
            
            return margin;
        }
    }
}