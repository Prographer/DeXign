using System.Windows;
using System.Windows.Controls;
using WPFExtension;

namespace DeXign.Controls
{
    public class ContentCell : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyHelper.Register();

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
