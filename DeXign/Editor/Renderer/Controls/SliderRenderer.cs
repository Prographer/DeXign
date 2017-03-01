using System.Windows.Controls;

using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Theme;

[assembly: ExportRenderer(typeof(PSlider), typeof(Slider), typeof(SliderRenderer))]

namespace DeXign.Editor.Renderer
{
    class SliderRenderer : LayerRenderer<PSlider, Slider>
    {
        public SliderRenderer(Slider adornedElement, PSlider model) : base(adornedElement, model)
        {
        }

        protected override string OnLoadPlatformStyleName()
        {
            return ThemeKeyStore.Slider;
        }

        protected override void OnElementAttached(Slider element)
        {
            base.OnElementAttached(element);

            SetHeight(double.NaN);

            BindingEx.SetBinding(
                Model, PSlider.MinimumProperty,
                element, Slider.MinimumProperty);

            BindingEx.SetBinding(
                Model, PSlider.MaximumProperty,
                element, Slider.MaximumProperty);

            BindingEx.SetBinding(
                Model, PSlider.ValueProperty,
                element, Slider.ValueProperty);
        }
    }
}
