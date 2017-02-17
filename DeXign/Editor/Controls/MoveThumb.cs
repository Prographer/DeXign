using DeXign.Editor.Layer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DeXign.Editor.Controls
{
    class MoveThumb : Thumb
    {
        private Vector beginSize;
        private Vector beginPosition;
        private Thickness beginThickness;
        private Rect beginBound;
        private Vector positionLimit;

        public FrameworkElement Target { get; set; }
        public SelectionLayer Layer { get; set; }

        public MoveThumb(SelectionLayer layer)
        {
            this.Layer = layer;
            this.Target = layer.AdornedElement;

            this.DragDelta += OnDragDelta;
            this.DragStarted += OnDragStated;
        }

        private void OnDragStated(object sender, DragStartedEventArgs e)
        {
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Layer.CancelNextInvert = true;

            if (Target.Parent is Canvas)
            {
                Canvas.SetTop(Target, Canvas.GetTop(Target) + e.VerticalChange);
                Canvas.SetLeft(Target, Canvas.GetLeft(Target) + e.HorizontalChange);
            }
            else
            {
                Thickness parentMargin = Layer.GetParentRenderMargin();
                Thickness margin = Target.Margin;

                parentMargin.Left *= -1;
                parentMargin.Top *= -1;
                
                switch (Layer.ClipData.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        margin.Top = parentMargin.Top + e.VerticalChange;
                        break;

                    case VerticalAlignment.Bottom:
                        margin.Bottom = parentMargin.Bottom - e.VerticalChange;
                        break;

                    case VerticalAlignment.Stretch:
                    case VerticalAlignment.Center:
                        margin.Top = parentMargin.Top + e.VerticalChange;
                        margin.Bottom = parentMargin.Bottom - e.VerticalChange;
                        break;
                }

                switch (Layer.ClipData.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        margin.Left = parentMargin.Left + e.HorizontalChange;
                        break;

                    case HorizontalAlignment.Right:
                        margin.Right = parentMargin.Right - e.HorizontalChange;
                        break;

                    case HorizontalAlignment.Stretch:
                    case HorizontalAlignment.Center:
                        margin.Left = parentMargin.Left + e.HorizontalChange;
                        margin.Right = parentMargin.Right - e.HorizontalChange;
                        break;
                }

                Target.Margin = margin;
            }
        }

        private void SetMarginTop()
        {

        }
    }
}