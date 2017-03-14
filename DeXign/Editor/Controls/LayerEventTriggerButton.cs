using System;
using System.Windows;

using DeXign.Editor.Layer;
using DeXign.Editor.Logic;
using DeXign.Extension;

namespace DeXign.Editor.Controls
{
    class LayerEventTriggerButton : BindThumb, IUISupport
    {
        SelectionLayer parentLayer;

        public LayerEventTriggerButton(SelectionLayer parentLayer)
        {
            if (parentLayer is IRenderer renderer)
            {
                this.parentLayer = parentLayer;
                this.Renderer = renderer;
            }
        }

        protected override void OnDragStarting()
        {
            base.OnDragStarting();

            parentLayer.DesignModeChanged += Layer_DesignModeChanged;
        }

        private void Layer_DesignModeChanged(object sender, EventArgs e)
        {
            if (parentLayer.DesignMode != DesignMode.Trigger)
            {
                parentLayer.Storyboard.CloseComponentBox();
            }
        }

        protected override void OnDragEnd()
        {
            Storyboard storyboard = parentLayer.Storyboard;

            storyboard.OpenComponentBox(this.Renderer.Model);
        }

        protected override void OnDragLineReleased()
        {
            base.OnDragLineReleased();

            parentLayer.DesignModeChanged -= Layer_DesignModeChanged;
        }

        public Point GetLocation()
        {
            UIElement element = this;

            if (parentLayer.DesignMode != DesignMode.Trigger)
                element = parentLayer.AdornedElement;

            var position = new Point(element.RenderSize.Width, element.RenderSize.Height / 2);

            return element.TranslatePoint(
                position,
                parentLayer.Storyboard);
        }

        public Rect GetBound()
        {
            throw new NotImplementedException();
        }
    }
}