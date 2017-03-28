using System.Windows;
using System.Windows.Controls;

using DeXign.Core.Logic;
using DeXign.Extension;
using DeXign.Resources;
using DeXign.Core;
using System;
using WPFExtension;
using System.Collections.Generic;

namespace DeXign.Editor.Logic
{
    [TemplatePart(Name = "PART_valueCheckBox", Type = typeof(CheckBox))]
    public class Branch : ComponentElement
    {
        public static readonly DependencyProperty OperatorProperty =
            DependencyHelper.Register();

        public ComparisonPredicate Operator
        {
            get { return (ComparisonPredicate)GetValue(OperatorProperty); }
            set { SetValue(OperatorProperty, value); }
        }

        public new PBranch Model => (PBranch)base.Model;

        private Dictionary<ComparisonPredicate, ComboBoxItem> dictItems;
        private CheckBox valueCheckBox;
        private ComboBox operatorBox;

        public Branch()
        {
            dictItems = new Dictionary<ComparisonPredicate, ComboBoxItem>();
        }

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            BindingEx.SetBinding(
                this.Model, PBranch.OperatorProperty,
                this, OperatorProperty);
                
            BindingEx.SetBinding(
                this.Model.Value2Binder, PBinder.IsDirectValueProperty,
                valueCheckBox, CheckBox.IsCheckedProperty);

            operatorBox.SelectedItem = dictItems[this.Model.Operator];
        }

        public override void OnApplyContentTemplate()
        {
            base.OnApplyContentTemplate();
            
            valueCheckBox = GetContentTemplateChild<CheckBox>("PART_valueCheckBox");
            operatorBox = GetContentTemplateChild<ComboBox>("PART_operatorBox");

            foreach (ComparisonPredicate value in Enum.GetValues(typeof(ComparisonPredicate)))
            {
                if (value.HasAttribute<DesignElementAttribute>())
                {
                    var attr = value.GetAttribute<DesignElementAttribute>();
                    
                    dictItems[value] = new ComboBoxItem()
                    {
                        Content = attr.DisplayName,
                        Tag = value
                    };

                    operatorBox.Items.Add(dictItems[value]);

                    operatorBox.SelectionChanged += OperatorBox_SelectionChanged;
                }
            }
        }

        private void OperatorBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (operatorBox.SelectedItem is ComboBoxItem item)
            {
                if (item.Tag is ComparisonPredicate op)
                {
                    this.Operator = op;
                }
            }
        }
    }
}
