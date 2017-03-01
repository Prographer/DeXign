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
using DeXign.OS;

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
        
        public override void OnAddedChild(IRenderer childRenderer)
        {
            var position = this.Element.PointFromScreen(SystemMouse.Position);
            int index = -1;

            // Inserting
            for (int i = -1; i < this.Element.Children.Count - 2; i++)
            {
                double c1 = 0;
                double c2 = 0;
                double value = (this.Element.Orientation == Orientation.Vertical ? position.Y : position.X); ;

                if (i >= 0)
                {
                    var child = this.Element.Children[i] as FrameworkElement;
                    Rect bound = this.Element.GetArrangedBound(child);

                    c1 = (this.Element.Orientation == Orientation.Vertical ? bound.CenterY() : bound.CenterX());
                }
                
                var nextChild = this.Element.Children[i + 1] as FrameworkElement;
                Rect nextBound = this.Element.GetArrangedBound(nextChild);

                c2 = (this.Element.Orientation == Orientation.Vertical ? nextBound.CenterY() : nextBound.CenterX());

                if (c1 <= value && value <= c2)
                {
                    index = i + 1;
                }
            }

            if (index != -1)
            {
                this.Element.Children.Remove(childRenderer.Element);
                this.Element.Children.Insert(index, childRenderer.Element);
            }

            if (Element.IsVertical)
            {
                childRenderer.Element.Width = double.NaN;
                childRenderer.Element.HorizontalAlignment = HorizontalAlignment.Stretch;
                childRenderer.Element.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                childRenderer.Element.Height = double.NaN;
                childRenderer.Element.HorizontalAlignment = HorizontalAlignment.Left;
                childRenderer.Element.VerticalAlignment = VerticalAlignment.Stretch;
            }
        }
    }
}
