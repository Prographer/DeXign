using System;
using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
    public class TextCell : ContentCell
    {
        public event EventHandler TextChanged;

        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register(
                new PropertyMetadata(TextPropertyChanged));

        public static readonly DependencyProperty TextBoxProperty =
            DependencyHelper.Register();

        public TextBox TextBox
        {
            get { return (TextBox)GetValue(TextBoxProperty); }
            set { SetValue(TextBoxProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TextCell).OnTextChanged();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected virtual void OnTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            TextBox = GetTemplateChild("PART_textBox") as TextBox;
        }
    }
}