using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Reflection;

using WPFExtension;
using DeXign.Resources;
using DeXign.Rules;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_valueBox", Type = typeof(SubmitTextBox))]
    class ValueBoxSetter : BaseSetter
    {
        SubmitTextBox valueBox;

        public ValueBoxSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        protected override void OnSelected()
        {
            Keyboard.Focus(valueBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            valueBox = GetTemplateChild<SubmitTextBox>("PART_valueBox");

            if (this.Value is double)
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

            if (!this.IsStable)
                valueBox.Text = "";
        }

        protected override void OnDispose()
        {
            if (valueBox != null)
                BindingOperations.ClearAllBindings(valueBox);
            
            valueBox = null;
        }
    }
}