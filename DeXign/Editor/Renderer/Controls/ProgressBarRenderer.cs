using System.Windows.Controls;

using DeXign.Theme;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Core.Controls;

using WPFExtension;

[assembly: ExportRenderer(typeof(PProgressBar), typeof(ProgressBar), typeof(ProgressBarRenderer))]

namespace DeXign.Editor.Renderer
{
    class ProgressBarRenderer : LayerRenderer<PProgressBar, ProgressBar>
    {
        public ProgressBarRenderer(ProgressBar adornedElement, PProgressBar model) : base(adornedElement, model)
        {
        }

        protected override string OnLoadPlatformStyleName()
        {
            return ThemeKeyStore.ProgressBar;
        }

        protected override void OnElementAttached(ProgressBar element)
        {
            base.OnElementAttached(element);

            SetHeight(double.NaN);

            BindingHelper.SetBinding(
                Model, PProgressBar.ProgressProperty,
                element, ProgressBar.ValueProperty);
        }
    }
}
