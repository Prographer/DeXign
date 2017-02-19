using System;
using System.Windows;
using System.Windows.Controls;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Core.Designer;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Input;
using DeXign.Extension;
using DeXign.Converter;
using WPFExtension;

[assembly: ExportRenderer(typeof(PStackLayout), typeof(StackPanel), typeof(StackLayoutRenderer))]

namespace DeXign.Editor.Renderer
{
    class StackLayoutRenderer : LayerRenderer<PStackLayout, StackPanel>, IStackLayout
    {
        static EnumToEnumConverter<Orientation, POrientation> orientationConverter;

        static StackLayoutRenderer()
        {
            orientationConverter = new EnumToEnumConverter<Orientation, POrientation>();
        }

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

            BindingEx.SetBinding(
                element, StackPanel.OrientationProperty,
                Model, PStackLayout.OrientationProperty,
                converter: orientationConverter);

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
