﻿using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_slider", Type = typeof(Slider))]
    [Setter(Key = "Slider", Type = typeof(double))]
    class SliderSetter : BaseSetter
    {
        Slider slider;

        public SliderSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            slider = GetTemplateChild<Slider>("PART_slider");

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
