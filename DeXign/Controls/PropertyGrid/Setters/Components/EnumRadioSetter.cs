using System.Windows;
using System.Reflection;

using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_valueRadioBox", Type = typeof(EnumRadioBox))]
    class EnumRadioSetter : BaseSetter
    {
        internal EnumRadioBox ValueRadioBox;

        public EnumRadioSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ValueRadioBox = GetTemplateChild<EnumRadioBox>("PART_valueRadioBox");

            if (this.IsStable)
                ValueRadioBox.Value = this.Value;
            else
                ValueRadioBox.IsEmpty = true;

            BindingHelper.SetBinding(
                this, ValueProperty,
                ValueRadioBox, EnumRadioBox.ValueProperty);
        }

        protected override void OnDispose()
        {
            ValueRadioBox = null;
        }
    }
}
