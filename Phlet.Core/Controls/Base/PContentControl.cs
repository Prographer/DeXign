using Phlet.Extension;
using System.Windows;

namespace Phlet.Core.Controls
{
    [XForms("Xamarin.Forms", "Frame")]
    public class PContentControl : PControl
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