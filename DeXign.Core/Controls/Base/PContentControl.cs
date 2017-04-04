using System.Windows;
using System.Windows.Markup;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [ContentProperty("Content")]
    [XForms("Xamarin.Forms", "Frame")]
    public class PContentControl : PControl
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyHelper.Register();

        [XForms("Content")]
        public PControl Content
        {
            get { return this.GetValue<PControl>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}