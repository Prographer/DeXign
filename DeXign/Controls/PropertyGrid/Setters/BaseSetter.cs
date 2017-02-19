using DeXign.Extension;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WPFExtension;

namespace DeXign.Controls
{
    public interface ISetter
    {
        object GetTargetValue();
        void SetTargetValue(object value);
    }

    public class BaseSetter : Control, ISetter
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register();

        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetTargetValue(value);
                SetValue(ValueProperty, value);
            }
        }

        public DependencyObject Target { get; }

        public PropertyInfo TargetProperty { get; }

        public DependencyProperty TargetDependencyProperty { get; }

        public BaseSetter(DependencyObject target, PropertyInfo pi)
        {
            this.Target = target;

            TargetProperty = pi;
            TargetDependencyProperty = TargetProperty?.GetDependencyProperty();

            if (TargetProperty == null || TargetDependencyProperty == null)
                throw new ArgumentException("속성을 찾을 수 없습니다.");

            // Target Binding
            OnTargetPropertyBinding();
        }

        protected virtual void OnTargetPropertyBinding()
        {
            SetValue(ValueProperty, GetTargetValue());

            BindingEx.SetBinding(
                Target, TargetDependencyProperty,
                this, ValueProperty);
        }

        public virtual object GetTargetValue()
        {
            return TargetProperty.GetValue(Target);
        }

        public virtual void SetTargetValue(object value)
        {
            TargetProperty.SetValue(Target, value);
        }

        protected T GetTemplateChild<T>(string name)
            where T : DependencyObject
        {
            return (T)GetTemplateChild(name);
        }
    }
}