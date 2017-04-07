using System.Windows;
using System.Windows.Controls;
using System.Reflection;

using DeXign.Converter;
using DeXign.Core.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_slider", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_valueBox", Type = typeof(TextBox))]
    [Setter(Key = "Percentage", Type = typeof(double))]
    class PercentageSetter : BaseSetter
    {
        Slider slider;
        TextBox valueBox;

        public PercentageSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        protected override void OnCreateMultiConverter()
        {
            foreach (var target in this.Targets)
            {
                if (target is PSlider slider)
                {
                    this.MultiConverter[target] = new SliderValueConverter(slider);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            slider = GetTemplateChild<Slider>("PART_slider");
            valueBox = GetTemplateChild<TextBox>("PART_valueBox");

            BindingHelper.SetBinding(
                this, ValueProperty,
                slider, Slider.ValueProperty);
        }
        
        protected override void OnDispose()
        {
            

            slider = null;
        }
    }
}