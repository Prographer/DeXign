using System.Windows;

using DeXign.Controls;
using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Converter;

using WPFExtension;

[assembly: ExportRenderer(typeof(PLabel), typeof(LabelEx), typeof(LabelRenderer))]

namespace DeXign.Editor.Renderer
{
    class LabelRenderer : LayerRenderer<PLabel, LabelEx>
    {
        static EnumToEnumConverter<PHorizontalTextAlignment, HorizontalAlignment> hConverter;
        static EnumToEnumConverter<PVerticalTextAlignment, VerticalAlignment> vConverter;

        static LabelRenderer()
        {
            hConverter = new EnumToEnumConverter<PHorizontalTextAlignment, HorizontalAlignment>();
            vConverter = new EnumToEnumConverter<PVerticalTextAlignment, VerticalAlignment>();
        }

        public LabelRenderer(LabelEx adornedElement, PLabel model) : base(adornedElement, model)
        {
        }

        protected override void OnElementAttached(LabelEx element)
        {
            base.OnElementAttached(element);

            SetSize(double.NaN, double.NaN);

            BindingHelper.SetBinding(
                Model, PLabel.ForegroundProperty,
                element, LabelEx.ForegroundProperty);

            BindingHelper.SetBinding(
                Model, PLabel.TextProperty,
                element, LabelEx.ContentProperty);

            BindingHelper.SetBinding(
                Model, PLabel.HorizontalTextAlignmentProperty,
                element, LabelEx.HorizontalContentAlignmentProperty,
                converter: hConverter);

            BindingHelper.SetBinding(
                Model, PLabel.VerticalTextAlignmentProperty,
                element, LabelEx.VerticalContentAlignmentProperty,
                converter: vConverter);

            BindingHelper.SetBinding(
                element, LabelEx.FontSizeProperty,
                Model, PLabel.FontSizeProperty);

            Model.Text = "텍스트";
        }
    }
}
