using DeXign.Extension;
using System.Windows;

namespace DeXign.Core.Controls
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