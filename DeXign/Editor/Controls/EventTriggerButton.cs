using System.Windows;
using System.Windows.Controls;

using WPFExtension;

using DeXign.Editor.Layer;
using DeXign.Controls;
using System;
using DeXign.Input;
using System.Windows.Input;
using DeXign.Editor.Renderer;

namespace DeXign.Editor.Controls
{
    class EventTriggerButton : RelativeThumb
    {
        public SelectionLayer ParentLayer { get; set; }

        private LineConnectorBase dragLine;

        public EventTriggerButton(SelectionLayer parentLayer)
        {
            this.ParentLayer = parentLayer;

            // Thumb에서 LostMouseCapture 핸들을 제일 먼저 처리해버려서 래핑 불가능함
            // 정상적으로 MouseUp을 할경우 가상 함수가 불린후 RoutedEvent가 불리지만
            // MouseUp을 통과하고 마우스 캡쳐가 사라지는경우 가상함수 호출이 불가능
            this.DragCompleted += EventTriggerButton_DragCompleted;
        }

        private void EventTriggerButton_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (dragLine != null)
            {
                ParentLayer.RootParent.RemoveConnectedLine(dragLine);
                dragLine = null;
            }
        }

        protected override void OnDragStarted(double horizontalOffset, double verticalOffset)
        {
            dragLine = ParentLayer.RootParent
                .CreateConnectedLine(
                    c =>
                    {
                        return this.TranslatePoint(
                            new Point(this.RenderSize.Width, this.RenderSize.Height / 2),
                            c.Parent);
                    },
                    c =>
                    {
                        return c.Parent.PointFromScreen(SystemMouse.Position);
                    });
                
            base.OnDragStarted(horizontalOffset, verticalOffset);
        }

        protected override void OnDragDelta(double horizontalChange, double verticalChange)
        {
            // Cancel Design Mode
            ParentLayer.CancelNextInvert = true;

            dragLine.Update();
        }
    }
}