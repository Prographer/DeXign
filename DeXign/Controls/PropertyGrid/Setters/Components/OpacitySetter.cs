using DeXign.Extension;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_slider", Type = typeof(Slider))]
    [Setter(Key = "Opacity", Type = typeof(double))]
    class OpacitySetter : BaseSetter
    {
        Slider slider;

        public OpacitySetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            slider = GetTemplateChild<Slider>("PART_slider");

            BindingEx.SetBinding(
                this, ValueProperty,
                slider, Slider.ValueProperty);
        }

        protected override void OnDispose()
        {
            slider = null;
        }
    }
}
