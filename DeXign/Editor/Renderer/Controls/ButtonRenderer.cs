using System.Windows;
using System.Windows.Controls;

using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Theme;

using WPFExtension;

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
            get { return (double)GetValue(BorderRadiusProperty); }
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
            BindingEx.SetBinding(
                Model, PButton.TextProperty,
                element, Button.ContentProperty);

            BindingEx.SetBinding(
                element, Button.ForegroundProperty,
                Model, PButton.ForegroundProperty);

            BindingEx.SetBinding(
                element, Button.BorderThicknessProperty,
                Model, PButton.BorderThicknessProperty);

            BindingEx.SetBinding(
                element, Button.BorderBrushProperty,
                Model, PButton.BorderBrushProperty);

            // TODO: BorderRadius

            // Setting
            Model.Text = "버튼";
        }
    }
}
