using DeXign.Converter;
using DeXign.Core.Controls;
using DeXign.Extension;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
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

        public PercentageSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            slider = GetTemplateChild<Slider>("PART_slider");
            valueBox = GetTemplateChild<TextBox>("PART_valueBox");

            BindingEx.SetBinding(
                this, ValueProperty,
                slider, Slider.ValueProperty);

            if (this.Target is PSlider)
            {
                BindingEx.SetBinding(
                    this.Target, PSlider.MinimumProperty,
                    slider, Slider.MinimumProperty);

                BindingEx.SetBinding(
                    this.Target, PSlider.MaximumProperty,
                    slider, Slider.MaximumProperty);

                var expression = valueBox.GetBindingExpression(TextBox.TextProperty);
                var binding = expression.ParentBinding;
                var converter = binding.Converter as PercentageConverter;

                PSlider.MaximumProperty.AddValueChanged(this.Target,
                    (s, e) =>
                    {
                        converter.Maximum = (double)this.Target.GetValue(PSlider.MaximumProperty);
                        expression.UpdateSource();
                    });

                PSlider.MinimumProperty.AddValueChanged(this.Target,
                    (s, e) =>
                    {
                        converter.Minimum = (double)this.Target.GetValue(PSlider.MinimumProperty);
                        expression.UpdateSource();
                    });
            }
        }

        protected override void OnDispose()
        {
            slider = null;
        }
    }
}
