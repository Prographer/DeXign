using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using DeXign.Controls;
using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Extension;

using WPFExtension;

namespace DeXign.Editor.Logic
{
    [TemplatePart(Name = "PART_valueCheckBox", Type = typeof(CheckBox))]
    public class Setter : BaseProperty
    {
        public static readonly DependencyProperty ValueSetterProperty =
            DependencyHelper.Register();

        public ISetter ValueSetter
        {
            get { return (ISetter)GetValue(ValueSetterProperty); }
            set { SetValue(ValueSetterProperty, value); }
        }

        public new PSetter Model => (PSetter)base.Model;

        private CheckBox valueCheckBox;
        
        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();
            
            BindingEx.SetBinding(
                this.Model.ValueBinder, PBinder.IsDirectValueProperty,
                valueCheckBox, CheckBox.IsCheckedProperty);
        }

        public override void OnApplyContentTemplate()
        {
            base.OnApplyContentTemplate();

            valueCheckBox = GetContentTemplateChild<CheckBox>("PART_valueCheckBox");
        }

        protected override void OnTargetTypeChanged()
        {
            base.OnTargetTypeChanged();
            
            OnSelectedPropertyChanged();
        }

        protected override void OnSelectedPropertyChanged()
        {
            base.OnSelectedPropertyChanged();

            if (this.Model.DummyObject == null || this.TargetType == null)
            {
                this.ValueSetter?.Dispose();
                this.ValueSetter = null;
                
                return;
            }

            var pi = TargetType.GetProperty(SelectedProperty.Name);
            var attr = pi.GetAttribute<DesignElementAttribute>();

            this.ValueSetter?.Dispose();

            if (string.IsNullOrEmpty(attr.Key))
                this.ValueSetter = SetterManager.CreateSetter(this.Model.DummyObject, pi);
            else
                this.ValueSetter = SetterManager.CreateSetter(this.Model.DummyObject, pi, attr.Key);

            if (this.ValueSetter != null)
            {
                (this.ValueSetter as FrameworkElement).Width = 150;

                if (this.ValueSetter is ValueBoxSetter vBoxSetter)
                {
                    vBoxSetter.Foreground = Brushes.Black;
                    vBoxSetter.Background = Brushes.Transparent;
                }

                BindingEx.SetBinding(
                    this.ValueSetter as BaseSetter, BaseSetter.ValueProperty,
                    this.Model.ValueBinder, PBinder.DirectValueProperty);
            }
        }
    }
}
