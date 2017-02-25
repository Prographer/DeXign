using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Controls
{
    public interface ISetter
    {
        object GetTargetValue();
        void SetTargetValue(object value);
    }

    public class BaseSetter : Control, ISetter, IDisposable
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

        public DependencyObject Target { get; private set; }

        public PropertyInfo TargetProperty { get; private set; }

        public DependencyProperty TargetDependencyProperty { get; private set; }

        bool isDisposed = false;

        public BaseSetter(DependencyObject target, PropertyInfo pi)
        {
            this.Focusable = false;

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

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                OnDispose();

                Target = null;
                TargetProperty = null;
                TargetDependencyProperty = null;
            }
        }

        protected virtual void OnDispose()
        {   
        }
    }
}