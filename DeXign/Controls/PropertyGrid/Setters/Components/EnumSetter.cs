using DeXign.Extension;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_valueComboBox", Type = typeof(ComboBox))]
    [Setter(Type = typeof(Enum))]
    class EnumSetter : BaseSetter
    {
        ComboBox valueComboBox;

        public EnumSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            valueComboBox = GetTemplateChild<ComboBox>("PART_valueComboBox");

            foreach (Enum value in Enum.GetValues(this.PropertyType))
                valueComboBox.Items.Add(value.GetDescription());

            if (this.IsStable)
                valueComboBox.SelectedItem = ((Enum)this.Value).GetDescription();

            valueComboBox.SelectionChanged += ValueComboBox_SelectionChanged;
            
            ValueProperty.AddValueChanged(this, ValueChanged);
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            string v = ((Enum)Value).GetDescription();

            if (valueComboBox.SelectedItem.ToString() != v)
                valueComboBox.SelectedItem = v;
        }

        private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Enum v = valueComboBox.SelectedItem.ToString()
                .FromDescription(this.PropertyType); ;

            if (Value != v)
                Value = v;
        }

        protected override void OnDispose()
        {
            if (valueComboBox != null)
            {
                valueComboBox.SelectionChanged -= ValueComboBox_SelectionChanged;
                ValueProperty.RemoveValueChanged(this, ValueChanged);

                valueComboBox = null;
            }
        }
    }
}
