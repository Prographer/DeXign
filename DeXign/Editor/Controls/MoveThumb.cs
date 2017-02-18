using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DeXign.Editor.Controls
{
    class MoveThumb : Thumb
    {
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

            if (Layer.Parent is IStoryboard)
            {
                Canvas.SetTop(
                    Target, 
                    Canvas.GetTop(Target) + e.VerticalChange);

                Canvas.SetLeft(
                    Target,
                    Canvas.GetLeft(Target) + e.HorizontalChange);
            }
            else
            {
                Thickness parentMargin = Layer.GetParentRenderMargin();
                Thickness margin = Target.Margin;

                parentMargin.Left *= -1;
                parentMargin.Top *= -1;

                bool allowVertical = true;
                bool allowHorizontal = true;

                if (Layer.Parent is IStackLayout)
                {
                    var stackRenderer = Layer.Parent as StackLayoutRenderer;

                    allowVertical = 
                        (stackRenderer.Element.Orientation == Orientation.Horizontal);

                    allowHorizontal = 
                        (stackRenderer.Element.Orientation == Orientation.Vertical);
                }
                
                if (allowVertical)
                {
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
                }

                if (allowHorizontal)
                {
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
                }

                Target.Margin = margin.Clean();
            }
        }

        private void SetMarginTop()
        {

        }
    }
}