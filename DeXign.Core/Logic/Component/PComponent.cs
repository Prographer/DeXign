using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public class PComponent : BaseBinder
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyHelper.Register();

        public string Title
        {
            get { return GetValue<string>(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}