using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using DeXign.Extension;

using WPFExtension;
using System.Linq;
using System.Windows.Media;

namespace DeXign.Controls
{
    public interface ISetter : IDisposable
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

        public DependencyObject Target { get; private set; }

        public PropertyInfo TargetProperty { get; private set; }

        public DependencyProperty TargetDependencyProperty { get; private set; }

        ListViewItem listViewItem;
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

            this.Loaded += BaseSetter_Loaded;
            this.Unloaded += BaseSetter_Unloaded;
        }

        private void BaseSetter_Unloaded(object sender, RoutedEventArgs e)
        {
            if (listViewItem != null)
            {
                listViewItem.PreviewMouseLeftButtonDown -= ListViewItem_MouseLeftButtonDown;
                listViewItem = null;
            }
        }

        private void BaseSetter_Loaded(object sender, RoutedEventArgs e)
        {
            listViewItem = this.FindVisualParents<ListViewItem>().FirstOrDefault();

            if (listViewItem != null)
                listViewItem.PreviewMouseLeftButtonDown += ListViewItem_MouseLeftButtonDown;
        }

        private void ListViewItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnSelected();
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

        protected virtual void OnSelected()
        {
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