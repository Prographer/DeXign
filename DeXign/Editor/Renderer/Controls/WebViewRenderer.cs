using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.UI.Controls;

[assembly: ExportRenderer(typeof(PWebView), typeof(DeXignWebView), typeof(WebViewRenderer))]

namespace DeXign.Editor.Renderer
{
    class WebViewRenderer : LayerRenderer<PWebView, DeXignWebView>
    {
        public WebViewRenderer(DeXignWebView adornedElement, PWebView model) : base(adornedElement, model)
        {
        }

        protected override void OnElementAttached(DeXignWebView element)
        {
            base.OnElementAttached(element);

            BindingEx.SetBinding(
                this.Model, PWebView.SourceProperty,
                element, DeXignWebView.AddressProperty);
        }
    }
}
