using System.Windows;
using System.Windows.Markup;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Visible = false, Category = Constants.Designer.Layout, DisplayName = "스크린")]
    [ContentProperty("Content")]
    [XForms("Xamarin.Forms", "ContentPage")]
    public class PContentPage : PPage
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyHelper.Register();

        [XForms("Content")]
        public PControl Content
        {
            get { return GetValue<PControl>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}