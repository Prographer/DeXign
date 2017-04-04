using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Reflection;

using DeXign.Converter;
using DeXign.Core.Controls;
using DeXign.Extension;

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

            BindingHelper.SetBinding(
                this, ValueProperty,
                slider, Slider.ValueProperty);

            if (this.Target is PSlider)
            {
                BindingHelper.SetBinding(
                    this.Target, PSlider.MinimumProperty,
                    slider, Slider.MinimumProperty);

                BindingHelper.SetBinding(
                    this.Target, PSlider.MaximumProperty,
                    slider, Slider.MaximumProperty);
                
                PSlider.MaximumProperty.AddValueChanged(this.Target, MaximumChanged);
                PSlider.MinimumProperty.AddValueChanged(this.Target, MinimumChanged);
            }
        }

        private void MinimumChanged(object sender, EventArgs e)
        {
            var data = GetTextBindingData();

            data.Converter.Minimum = (double)this.Target.GetValue(PSlider.MinimumProperty);
            data.Expression.UpdateSource();
        }

        private void MaximumChanged(object sender, EventArgs e)
        {
            var data = GetTextBindingData();

            data.Converter.Maximum = (double)this.Target.GetValue(PSlider.MaximumProperty);
            data.Expression.UpdateSource();
        }

        private (BindingExpression Expression, PercentageConverter Converter) GetTextBindingData()
        {
            var expression = valueBox.GetBindingExpression(TextBox.TextProperty);
            var binding = expression.ParentBinding;
            var converter = binding.Converter as PercentageConverter;

            return (expression, converter);
        }

        protected override void OnDispose()
        {
            if (this.Target is PSlider)
            {
                PSlider.MaximumProperty.RemoveValueChanged(this.Target, MaximumChanged);
                PSlider.MinimumProperty.RemoveValueChanged(this.Target, MinimumChanged);
            }

            slider = null;
        }
    }
}