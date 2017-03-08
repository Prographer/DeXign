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
using System.Windows.Input;
using DeXign.Editor.Layer;

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

        public override bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item, Point mouse)
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
        
        protected override void OnAddedChild(IRenderer child, Point position)
        {
            base.OnAddedChild(child, position);

            if (IsLoaded)
            {
                // position은 RootParent 기준이기 때문에 StackPanel 기준으로 다시 계산
                position = RootParent.TranslatePoint(position, this);

                MoveToPosition(child.Element, position);
            }
            
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

        public void MoveToPosition(FrameworkElement element, Point position)
        {
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
                this.Element.Children.Remove(element);
                this.Element.Children.Insert(index, element);
            }
        }
    }
}
