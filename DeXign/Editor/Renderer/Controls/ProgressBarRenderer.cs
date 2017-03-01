using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

            BindingEx.SetBinding(
                Model, PProgressBar.ProgressProperty,
                element, ProgressBar.ValueProperty);
        }
    }
}
