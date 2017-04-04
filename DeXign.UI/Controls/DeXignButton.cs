using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.UI
{
    public class DeXignButton : Button
    {
        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register();

        public string Text
        {
            get { return this.GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
