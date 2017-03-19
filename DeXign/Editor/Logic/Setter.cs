using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using DeXign.Controls;
using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Extension;
using DeXign.Resources;

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

        protected PParameterBinder ValueBinder { get; private set; }

        private PObject dummyObject;
        private CheckBox valueCheckBox;
        
        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            this.ValueBinder = this.GetBinderModel<PParameterBinder>(BindOptions.Parameter, 1);

            BindingEx.SetBinding(
                valueCheckBox, CheckBox.IsCheckedProperty,
                this.GetBindThumb(BindOptions.Parameter, 1), BindThumb.IsEnabledProperty,
                converter: ResourceManager.GetConverter("Not"));
        }

        public override void OnApplyContentTemplate()
        {
            base.OnApplyContentTemplate();

            valueCheckBox = GetContentTemplateChild<CheckBox>("PART_valueCheckBox");
        }

        protected override void OnTargetTypeChanged()
        {
            base.OnTargetTypeChanged();

            dummyObject = Activator.CreateInstance(TargetType) as PObject;

            InvalidateProperty(SelectedPropertyProperty);
        }

        protected override void OnSelectedPropertyChanged()
        {
            base.OnTargetTypeChanged();

            if (dummyObject == null)
                return;

            var pi = TargetType.GetProperty(SelectedProperty.Name);
            var attr = pi.GetAttribute<DesignElementAttribute>();

            this.ValueSetter?.Dispose();

            if (string.IsNullOrEmpty(attr.Key))
                this.ValueSetter = SetterManager.CreateSetter(dummyObject, pi);
            else
                this.ValueSetter = SetterManager.CreateSetter(dummyObject, pi, attr.Key);

            if (this.ValueSetter != null)
            {
                (this.ValueSetter as FrameworkElement).Width = 150;

                if (this.ValueSetter is ValueBoxSetter vBoxSetter)
                {
                    vBoxSetter.Foreground = Brushes.Black;
                    vBoxSetter.Background = Brushes.Transparent;
                }
            }
        }
    }
}
