using Phlet.Extension;

using System.Windows;

namespace Phlet.Core.Controls
{
    public class PContentControl : PControl
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyHelper.Register();

        public PControl Content
        {
            get { return (PControl)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public bool HasContent
        {
            get { return Content != null; }
        }
    }
}