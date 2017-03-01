using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using DeXign.OS;
using DeXign.Editor.Layer;

namespace DeXign.Editor.Controls
{
    class EventTriggerButton : Control
    {
        public SelectionLayer ParentLayer { get; set; }

        private LineConnectorBase dragLine;

        public EventTriggerButton(SelectionLayer parentLayer)
        {
            this.ParentLayer = parentLayer;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (ParentLayer is IRenderer == false)
                return;

            var renderer = ParentLayer as IRenderer;
            var storyboard = ParentLayer.RootParent;

            storyboard.CloseComponentBox();

            dragLine = storyboard
                .CreatePendingConnectedLine(
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
            
            ParentLayer.DesignModeChanged += ParentLayer_DesignModeChanged;
            dragLine.Released += DragLine_Released;

            // 드래그
            DragDrop.DoDragDrop(this, renderer.Model, DragDropEffects.None);
            
            // 컴포넌트박스 Open
            storyboard.OpenComponentBox(renderer.Model);
        }

        private void ParentLayer_DesignModeChanged(object sender, System.EventArgs e)
        {
            if (ParentLayer.DesignMode != DesignMode.Trigger && dragLine != null)
            {
                ParentLayer.RootParent.CloseComponentBox();
            }
        }

        private void DragLine_Released(object sender, System.EventArgs e)
        {
            // Released Connector on Renderer

            if (dragLine == null)
                return;
            
            ParentLayer.DesignModeChanged -= ParentLayer_DesignModeChanged;
            dragLine.Released -= DragLine_Released;
            dragLine = null;
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            dragLine.Update();
            e.Handled = true;
        }
    }
}