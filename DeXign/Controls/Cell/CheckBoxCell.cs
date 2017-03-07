using System.Windows;

using WPFExtension;

namespace DeXign.Controls
{
    public class CheckBoxCell : ContentCell
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register();

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
