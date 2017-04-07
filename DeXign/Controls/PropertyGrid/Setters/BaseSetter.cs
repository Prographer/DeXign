﻿using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using DeXign.Extension;

using WPFExtension;

namespace DeXign.Controls
{
    public class BaseSetter : Control, ISetter
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register();

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public DependencyObject[] Targets { get; private set; }

        public PropertyInfo[] TargetProperties { get; private set; }

        public DependencyProperty[] TargetDependencyProperties { get; private set; }

        public bool IsStable => multiConverter.IsStable;

        MultiPropertyConverter multiConverter;
        ListViewItem listViewItem;
        bool isDisposed = false;

        public BaseSetter(DependencyObject[] target, PropertyInfo[] pi)
        {
            this.Focusable = false;

            this.Targets = target;
            this.TargetProperties = pi;

            if (this.TargetProperties != null)
            {
                this.TargetDependencyProperties = this.TargetProperties
                    .Select(tp => ReflectionEx.GetDependencyProperty(tp))
                    .Where(dp => dp != null)
                    .ToArray();
            }

            if (this.TargetProperties == null || 
                this.TargetDependencyProperties == null || 
                this.TargetProperties.Length != this.TargetDependencyProperties.Length)
            {
                throw new ArgumentException("속성을 찾을 수 없습니다.");
            }
            
            InitializeMultiBinding();

            this.Loaded += BaseSetter_Loaded;
            this.Unloaded += BaseSetter_Unloaded;
        }

        private void InitializeMultiBinding()
        {
            multiConverter = new MultiPropertyConverter(this.TargetProperties[0].PropertyType);

            var multiBinding = new MultiBinding()
            {
                Converter = multiConverter,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Default
            };
            
            for (int i = 0; i < this.Targets.Length; i++)
            {
                multiBinding.Bindings.Add(
                    new Binding(this.TargetDependencyProperties[i].Name)
                    {
                        Source = this.Targets[i],
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Default
                    });
            }

            this.SetBinding(ValueProperty, multiBinding);
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

                BindingOperations.ClearBinding(this, ValueProperty);

                Targets = null;
                TargetProperties = null;
                TargetDependencyProperties = null;
            }
        }

        protected virtual void OnDispose()
        {
        }
    }
}