using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using DeXign.Editor;
using DeXign.Editor.Interfaces;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Core.Controls;

[assembly: ExportRenderer(typeof(PContentPage), typeof(ContentControl), typeof(ScreenRenderer))]

namespace DeXign.Editor.Renderer
{
    class ScreenRenderer : LayerRenderer<PContentPage, ContentControl>
    {
        public ScreenRenderer(UIElement adornedElement) : base(adornedElement)
        {
            BindingEx.SetBinding(
                Model, PContentPage.ContentProperty,
                this, ContentControl.ContentProperty);
        }

        protected override void OnElementAttached(ContentControl element)
        {
        }

        protected override void OnDispatchRender(DrawingContext dc)
        {
            if (DesignMode == DesignMode.Trigger)
                return;

            var pen = CreatePen(SelectionBrush, 1);
            var dashedPen = CreatePen(SelectionBrush, 1);

            dashedPen.DashStyle = DashStyles.Dot;

            // Width
            dc.DrawLine(pen,
                new Point(0, -1 / ScaleX),
                new Point(0, -12 / ScaleX));

            dc.DrawLine(pen,
                new Point(RenderSize.Width - 1 / ScaleX, -1 / ScaleX),
                new Point(RenderSize.Width - 1 / ScaleX, -12 / ScaleX));

            dc.DrawLine(dashedPen,
                new Point(0, -7 / ScaleX),
                new Point(RenderSize.Width - 1 / ScaleX, -7 / ScaleX));

            // Height
            dc.DrawLine(pen,
                new Point(-1 / ScaleX, 0),
                new Point(-12 / ScaleX, 0));

            dc.DrawLine(pen,
                new Point(-1 / ScaleX, RenderSize.Height - 1 / ScaleX),
                new Point(-12 / ScaleX, RenderSize.Height - 1 / ScaleX));

            dc.DrawLine(dashedPen,
                new Point(-7 / ScaleX, 0),
                new Point(-7 / ScaleX, RenderSize.Height - 1 / ScaleX));
        }

        public override bool CanDrop(object item)
        {
            return true;
        }
    }
}