using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using DeXign.Rules;
using DeXign.Resources;

using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_valueBox", Type = typeof(TextBox))]
    class ValueBoxSetter : BaseSetter
    {
        TextBox valueBox;

        public ValueBoxSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        protected override void OnSelected()
        {
            Keyboard.Focus(valueBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            valueBox = GetTemplateChild<TextBox>("PART_valueBox");

            if (TargetProperty.PropertyType == typeof(double))
            {
                Binding b = BindingHelper.SetBinding(
                    this, ValueProperty,
                    valueBox, TextBox.TextProperty,
                    converter: ResourceManager.GetConverter("DoubleToString"));

                b.ValidationRules.Add(new DoubleRule());
            }
            else
            {
                BindingHelper.SetBinding(
                    this, ValueProperty,
                    valueBox, TextBox.TextProperty);
            }
        }

        protected override void OnDispose()
        {
            valueBox = null;
        }
    }
}
