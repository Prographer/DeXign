using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Core.Designer;
using DeXign.Editor;
using DeXign.Editor.Renderer;

[assembly: ExportRenderer(typeof(PContentPage), typeof(ContentControl), typeof(ScreenRenderer))]

namespace DeXign.Editor.Renderer
{
    class ScreenRenderer : LayerRenderer<PContentPage, ContentControl>, IContentLayout
    {
        public ScreenRenderer(ContentControl adornedElement, PContentPage model) : base(adornedElement, model)
        {
            adornedElement.PreviewMouseLeftButtonDown += AdornedElement_PreviewMouseLeftButtonDown;
        }

        private void AdornedElement_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
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
                new Point(0, -1 / Scale),
                new Point(0, -12 / Scale));

            dc.DrawLine(pen,
                new Point(RenderSize.Width - 1 / Scale, -1 / Scale),
                new Point(RenderSize.Width - 1 / Scale, -12 / Scale));

            dc.DrawLine(dashedPen,
                new Point(0, -7 / Scale),
                new Point(RenderSize.Width - 1 / Scale, -7 / Scale));

            // Height
            dc.DrawLine(pen,
                new Point(-1 / Scale, 0),
                new Point(-12 / Scale, 0));

            dc.DrawLine(pen,
                new Point(-1 / Scale, RenderSize.Height - 1 / Scale),
                new Point(-12 / Scale, RenderSize.Height - 1 / Scale));

            dc.DrawLine(dashedPen,
                new Point(-7 / Scale, 0),
                new Point(-7 / Scale, RenderSize.Height - 1 / Scale));
        }

        public override bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item, Point mouse)
        {
            return item != null;
        }

        protected override string OnLoadPlatformStyleName()
        {
            return Theme.ThemeKeyStore.Screen;
        }

        protected override void OnAddedChild(IRenderer child, Point position)
        {
            base.OnAddedChild(child, position);

            child.Element.VerticalAlignment = VerticalAlignment.Stretch;
            child.Element.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}