using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Core.Controls;
using System.Windows.Controls;
using System.Windows;

[assembly: ExportRenderer(typeof(PButton), typeof(Button), typeof(ButtonRenderer))]

namespace DeXign.Editor.Renderer
{
    public class ButtonRenderer : LayerRenderer<PButton, Button>
    {
        public ButtonRenderer(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnElementAttached(Button element)
        {
            base.OnElementAttached(element);

            // Binding
            BindingEx.SetBinding(
                element, Button.ContentProperty,
                Model, PButton.TextProperty);

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
