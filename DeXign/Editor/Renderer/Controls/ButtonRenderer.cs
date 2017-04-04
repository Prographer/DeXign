using System.Windows;
using System.Windows.Controls;

using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Theme;

using WPFExtension;
using System;

[assembly: ExportRenderer(typeof(PButton), typeof(RadiusButton), typeof(ButtonRenderer))]

namespace DeXign.Editor.Renderer
{
    public class RadiusButton : Button
    {
        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyHelper.Register(
                new PropertyMetadata(5d));

        public double BorderRadius
        {
            get { return this.GetValue<double>(BorderRadiusProperty); }
            set { SetValue(BorderRadiusProperty, value); }
        }
    }
    
    public class ButtonRenderer : LayerRenderer<PButton, RadiusButton>
    {
        public ButtonRenderer(RadiusButton adornedElement, PButton model) : base(adornedElement, model)
        {
        }

        protected override string OnLoadPlatformStyleName()
        {
            return ThemeKeyStore.Button;
        }

        protected override void OnElementAttached(RadiusButton element)
        {
            base.OnElementAttached(element);

            SetSize(double.NaN, double.NaN);
            
            // Binding
            BindingHelper.SetBinding(
                Model, PButton.TextProperty,
                element, Button.ContentProperty);

            BindingHelper.SetBinding(
                element, Button.ForegroundProperty,
                Model, PButton.ForegroundProperty);

            BindingHelper.SetBinding(
                element, Button.BorderThicknessProperty,
                Model, PButton.BorderThicknessProperty);

            BindingHelper.SetBinding(
                element, Button.BorderBrushProperty,
                Model, PButton.BorderBrushProperty);

            BindingHelper.SetBinding(
                element, RadiusButton.BorderRadiusProperty,
                Model, PButton.BorderRadiusProperty);

            BindingHelper.SetBinding(
                element, Button.FontSizeProperty,
                Model, PButton.FontSizeProperty);

            // Setting
            Model.Text = "버튼";
        }
    }
}
