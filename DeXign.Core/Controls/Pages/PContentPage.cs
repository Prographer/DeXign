using DeXign.Extension;
using System.Windows;

namespace DeXign.Core.Controls
{
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