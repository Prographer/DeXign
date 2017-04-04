using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public class PNamedBinder : PBinder
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyHelper.Register();

        public string Title
        {
            get { return this.GetValue<string>(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public PNamedBinder()
        {
        }

        public PNamedBinder(IBinderHost host, BindOptions bindOption) : base(host, bindOption)
        {
        }
    }
}
