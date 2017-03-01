using System;
using System.Windows;
using System.Windows.Controls;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Core.Designer;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Converter;
using DeXign.Controls;

[assembly: ExportRenderer(typeof(PStackLayout), typeof(SpacingStackPanel), typeof(StackLayoutRenderer))]

namespace DeXign.Editor.Renderer
{
    class StackLayoutRenderer : LayerRenderer<PStackLayout, SpacingStackPanel>, IStackLayout
    {
        static EnumToEnumConverter<Orientation, POrientation> orientationConverter;

        static StackLayoutRenderer()
        {
            orientationConverter = new EnumToEnumConverter<Orientation, POrientation>();
        }

        public StackLayoutRenderer(SpacingStackPanel adornedElement, PStackLayout model) : base(adornedElement, model)
        {
        }

        public override bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item)
        {
            return item != null;
        }

        protected override void OnElementAttached(SpacingStackPanel element)
        {
            base.OnElementAttached(element);

            BindingEx.SetBinding(
                element, StackPanel.OrientationProperty,
                Model, PStackLayout.OrientationProperty,
                converter: orientationConverter);

            BindingEx.SetBinding(
                element, SpacingStackPanel.SpacingProperty,
                Model, PStackLayout.SpacingProperty);

            SetSize(100, 100);
        }
        
        public override void OnAddedChild(IRenderer child)
        {
            if (Element.IsVertical)
            {
                child.Element.Width = double.NaN;
                child.Element.HorizontalAlignment = HorizontalAlignment.Stretch;
                child.Element.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                child.Element.Height = double.NaN;
                child.Element.HorizontalAlignment = HorizontalAlignment.Left;
                child.Element.VerticalAlignment = VerticalAlignment.Stretch;
            }
        }
    }
}
