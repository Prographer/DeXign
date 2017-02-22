using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Controls;
using DeXign.Core;
using DeXign.Converter;

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

            BindingEx.SetBinding(
                Model, PLabel.ForegroundProperty,
                element, LabelEx.ForegroundProperty);

            BindingEx.SetBinding(
                Model, PLabel.TextProperty,
                element, LabelEx.ContentProperty);

            BindingEx.SetBinding(
                Model, PLabel.HorizontalTextAlignmentProperty,
                element, LabelEx.HorizontalContentAlignmentProperty,
                converter: hConverter);

            BindingEx.SetBinding(
                Model, PLabel.VerticalTextAlignmentProperty,
                element, LabelEx.VerticalContentAlignmentProperty,
                converter: vConverter);

            Model.Text = "텍스트";
        }
    }
}
