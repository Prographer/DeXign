using System.Windows;
using System.Reflection;

using DeXign.Extension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_valueRadioBox", Type = typeof(EnumRadioBox))]
    class EnumRadioSetter : BaseSetter
    {
        internal EnumRadioBox ValueRadioBox;

        public EnumRadioSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ValueRadioBox = GetTemplateChild<EnumRadioBox>("PART_valueRadioBox");

            ValueRadioBox.Value = this.Value;

            BindingEx.SetBinding(
                this, ValueProperty,
                ValueRadioBox, EnumRadioBox.ValueProperty);
        }
    }
}
