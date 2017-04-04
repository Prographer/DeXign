using System.Windows.Shapes;

using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Theme;

using WPFExtension;

[assembly: ExportRenderer(typeof(PBoxView), typeof(Rectangle), typeof(BoxViewRenderer))]

namespace DeXign.Editor.Renderer
{
    class BoxViewRenderer : LayerRenderer<PBoxView, Rectangle>
    {
        public BoxViewRenderer(Rectangle adornedElement, PBoxView model) : base(adornedElement, model)
        {
        }

        protected override string OnLoadPlatformStyleName()
        {
            return ThemeKeyStore.BoxView;
        }

        protected override void OnElementAttached(Rectangle element)
        {
            base.OnElementAttached(element);

            BindingHelper.SetBinding(
                element, Rectangle.FillProperty,
                Model, PBoxView.FillProperty);

            this.SetSize(40, 40);
        }
    }
}