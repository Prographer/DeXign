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
            get { return this.GetValue<bool>(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public string Text
        {
            get { return this.GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
