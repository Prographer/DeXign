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
            base.OnDispatchRender(dc);

            if (DesignMode == DesignMode.Trigger)
                return;

            var pen = CreatePen(SelectionBrush, 1);
            var dashedPen = CreatePen(SelectionBrush, 1);

            dashedPen.DashStyle = DashStyles.Dot;

            // Width
            dc.DrawLine(pen,
                new Point(0, this.Fit(-1)),
                new Point(0, this.Fit(-12)));

            dc.DrawLine(pen,
                new Point(RenderSize.Width - this.Fit(1), this.Fit(-1)),
                new Point(RenderSize.Width - this.Fit(1), this.Fit(-12)));

            dc.DrawLine(dashedPen,
                new Point(0, this.Fit(-7)),
                new Point(RenderSize.Width - this.Fit(1), this.Fit(-7)));

            // Height
            dc.DrawLine(pen,
                new Point(this.Fit(-1), 0),
                new Point(this.Fit(-12), 0));

            dc.DrawLine(pen,
                new Point(this.Fit(-1), RenderSize.Height - this.Fit(1)),
                new Point(this.Fit(-12), RenderSize.Height - this.Fit(1)));

            dc.DrawLine(dashedPen,
                new Point(this.Fit(-7), 0),
                new Point(this.Fit(-7), RenderSize.Height - this.Fit(1)));
        }

        public override bool CanDrop(ItemDropRequest request, Point mouse)
        {
            return request != null;
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