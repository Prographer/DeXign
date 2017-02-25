using DeXign.Extension;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_valueBox", Type = typeof(TextBox))]
    class ValueBoxSetter : BaseSetter
    {
        TextBox valueBox;

        public ValueBoxSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            valueBox = GetTemplateChild<TextBox>("PART_valueBox");
            
            BindingEx.SetBinding(
                this, ValueProperty,
                valueBox, TextBox.TextProperty);
        }

        protected override void OnDispose()
        {
            valueBox = null;
        }
    }
}
