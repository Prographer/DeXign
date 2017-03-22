using System.Windows;
using System.Windows.Markup;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Visible = false, Category = Constants.Designer.Layout, DisplayName = "화면")]
    [ContentProperty("Content")]
    [XForms("Xamarin.Forms", "ContentPage")]
    [WPF("System.Windows.Controls", "Page")]
    public class PContentPage : PPage
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyHelper.Register();

        [XForms("Content")]
        [WPF("Content")]
        public PControl Content
        {
            get { return GetValue<PControl>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}