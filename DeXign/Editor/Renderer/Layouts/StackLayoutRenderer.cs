using System;
using System.Windows;
using System.Windows.Controls;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Core.Designer;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Input;

[assembly: ExportRenderer(typeof(PStackLayout), typeof(StackPanel), typeof(StackLayoutRenderer))]

namespace DeXign.Editor.Renderer
{
    class StackLayoutRenderer : LayerRenderer<PStackLayout, StackPanel>, IStackLayout
    {
        public StackLayoutRenderer(StackPanel adornedElement, PStackLayout model) : base(adornedElement, model)
        {
        }

        public override bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item)
        {
            return item != null;
        }

        protected override void OnElementAttached(StackPanel element)
        {
            base.OnElementAttached(element);

            SetSize(100, 100);
        }

        public override void OnAddedChild(IRenderer child)
        {
            if (Element.Orientation == Orientation.Vertical)
            {
                child.Element.Width = double.NaN;
                child.Element.HorizontalAlignment = HorizontalAlignment.Stretch;
                child.Element.VerticalAlignment = VerticalAlignment.Center;
            }
            else
            {
                child.Element.Height = double.NaN;
                child.Element.HorizontalAlignment = HorizontalAlignment.Center;
                child.Element.VerticalAlignment = VerticalAlignment.Stretch;
            }
        }
    }
}
