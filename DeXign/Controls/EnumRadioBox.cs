using DeXign.Converter;
using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using WPFExtension;

namespace DeXign.Controls
{
    [ContentProperty("Content")]
    class EnumContent : MarkupExtension
    {
        public string Value { get; set; }

        public object Content { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ContentProperty("Contents")]
    class EnumRadioBox : StackPanel
    {
        public static readonly DependencyProperty EnumTypeProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register();

        private static readonly DependencyPropertyKey ContentsPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public Type EnumType
        {
            get { return this.GetValue<Type>(EnumTypeProperty); }
            set
            {
                if (!value.IsEnum)
                    throw new ArgumentException();

                SetValue(EnumTypeProperty, value);
            }
        }

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public ObservableCollection<EnumContent> Contents
        {
            get { return this.GetValue<ObservableCollection<EnumContent>>(ContentsPropertyKey.DependencyProperty); }
        }

        public EnumRadioBox()
        {
            SetValue(ContentsPropertyKey, new ObservableCollection<EnumContent>());
            Contents.CollectionChanged += Contents_CollectionChanged;

            this.Orientation = Orientation.Horizontal;
            
            EnumTypeProperty.AddValueChanged(this, EnumTypeChanged);
            ValueProperty.AddValueChanged(this, ValueChanged);
        }
        
        private void Contents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvalidateContents();
        }

        private void InvalidateContents()
        {
            foreach (RadioButton radio in Children)
            {
                var ec = Contents
                    .FirstOrDefault(c =>
                    {
                        return c.Value == radio.Tag.ToString();
                    });

                if (ec != null)
                    radio.Content = ec.Content;
            }
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            var radio = Children
                .Cast<RadioButton>()
                .FirstOrDefault(r => r.Tag.Equals(this.Value));

            if (radio == null)
                return;

            radio.IsChecked = true;
        }

        private void EnumTypeChanged(object sender, EventArgs e)
        {
            Children.Clear();

            this.Value = EnumType.GetDefault();

            foreach (Enum value in Enum.GetValues(EnumType))
            {
                var radio = new RadioButton()
                {
                    Style = (Style)FindResource("EnumRadioButtonStyle"),
                    Tag = value,
                    IsChecked = value.Equals(this.Value)
                };

                radio.Checked += (ss, ee) =>
                {
                    if (radio.IsChecked.Value && this.Value != value)
                    {
                        this.Value = value;
                    }
                };
                
                Children.Add(radio);
            }

            InvalidateContents();
        }
    }
}