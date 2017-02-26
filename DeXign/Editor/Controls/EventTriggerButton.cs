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

            // Thumb���� LostMouseCapture �ڵ��� ���� ���� ó���ع����� ���� �Ұ�����
            // ���������� MouseUp�� �Ұ�� ���� �Լ��� �Ҹ��� RoutedEvent�� �Ҹ�����
            // MouseUp�� ����ϰ� ���콺 ĸ�İ� ������°�� �����Լ� ȣ���� �Ұ���
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