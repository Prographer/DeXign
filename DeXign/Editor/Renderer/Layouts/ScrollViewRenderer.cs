using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using System.Windows.Controls;
using DeXign.Core;
using DeXign.Core.Designer;
using System;
using DeXign.Controls;

[assembly: ExportRenderer(typeof(PScrollView), typeof(ProtrudedScrollViewer), typeof(ScrollViewRenderer))]

namespace DeXign.Editor.Renderer
{
    class ScrollViewRenderer : LayerRenderer<PScrollView, ProtrudedScrollViewer>
    {
        public ScrollViewRenderer(ProtrudedScrollViewer adornedElement, PScrollView model) : base(adornedElement, model)
        {
        }

        protected override void OnElementAttached(ProtrudedScrollViewer element)
        {
            base.OnElementAttached(element);
        }

        public override bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item)
        {
            return true;
        }

        public override void OnAddedChild(IRenderer child)
        {
            base.OnAddedChild(child);
        }
    }
}
