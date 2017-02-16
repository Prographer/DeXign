using DeXign.Theme;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Core.Controls;

using System.Windows;
using System.Windows.Shapes;

[assembly: ExportRenderer(typeof(PBoxView), typeof(Rectangle), typeof(BoxViewRenderer))]

namespace DeXign.Editor.Renderer
{
    class BoxViewRenderer : LayerRenderer<PBoxView, Rectangle>
    {
        public BoxViewRenderer(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override string OnLoadPlatformStyleName()
        {
            return ThemeKeyStore.BoxView;
        }

        protected override void OnElementAttached(Rectangle element)
        {
            base.OnElementAttached(element);

            if (!IsContentParent())
            {
                element.Width = 50;
                element.Height = 50;
            }

            BindingEx.SetBinding(
                element, Rectangle.FillProperty,
                Model, PBoxView.BackgroundProperty);
        }
    }
}