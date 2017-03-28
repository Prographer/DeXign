using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.UI
{
    public class DeXignPage : ContentControl
    {
        public static readonly DependencyProperty WindowTitleProperty =
            DependencyHelper.Register(
                new PropertyMetadata(""));

        public string WindowTitle
        {
            get { return (string)GetValue(WindowTitleProperty); }
            set { SetValue(WindowTitleProperty, value); }
        }
    }
}
