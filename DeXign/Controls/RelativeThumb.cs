using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

using DeXign.Input;
using System.Windows.Media;

namespace DeXign.Controls
{
    class RelativeThumb : Thumb
    {
        public Visual RelativeTarget { get; set; }
        public Point PreviousDelta => (Point)previousDelta;

        Point beginMousePoint;
        Vector previousDelta;

        protected virtual void OnDragDelta(double horizontalChange, double verticalChange)
        {
            RaiseEvent(new DragDeltaEventArgs(horizontalChange, verticalChange));
        }

        protected virtual void OnDragStarted(double horizontalOffset, double verticalOffset)
        {
            RaiseEvent(new DragStartedEventArgs(horizontalOffset, verticalOffset));
        }

        protected virtual void OnDragCompleted(double horizontalChange, double verticalChange)
        {
            RaiseEvent(new DragCompletedEventArgs(horizontalChange, verticalChange, false));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsDragging)
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    Vector delta = ScreenToRelativePoint(SystemMouse.Position) - beginMousePoint;

                    if (delta != previousDelta)
                    {
                        previousDelta = delta;

                        e.Handled = true;
                        OnDragDelta(delta.X, delta.Y);
                    }
                }
                else
                {
                    if (e.MouseDevice.Captured == this)
                        ReleaseMouseCapture();

                    IsDragging = false;
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsDragging)
            {
                e.Handled = true;

                Focus();
                CaptureMouse();

                IsDragging = true;

                beginMousePoint = ScreenToRelativePoint(SystemMouse.Position);

                bool exceptionThrown = true;

                try
                {
                    OnDragStarted(beginMousePoint.X, beginMousePoint.Y);
                    exceptionThrown = false;
                }
                finally
                {
                    if (exceptionThrown)
                        CancelDrag();
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured && IsDragging)
            {
                e.Handled = true;
                IsDragging = false;

                ReleaseMouseCapture();

                Vector delta = ScreenToRelativePoint(SystemMouse.Position) - beginMousePoint;

                OnDragCompleted(delta.X, delta.Y);
            }
        }

        private Point ScreenToRelativePoint(Point point)
        {
            Visual target = RelativeTarget ?? this;

            return target.PointFromScreen(point);
        }

    }
}
