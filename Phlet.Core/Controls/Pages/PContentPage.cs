using Phlet.Extension;
using System.Windows;

namespace Phlet.Core.Controls
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