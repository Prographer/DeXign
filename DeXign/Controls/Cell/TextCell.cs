using System;
using System.Windows;
using WPFExtension;

namespace DeXign.Controls
{
    public class TextCell : ContentCell
    {
        public event EventHandler TextChanged;

        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register(
                new PropertyMetadata(TextPropertyChanged));

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
    }
}
