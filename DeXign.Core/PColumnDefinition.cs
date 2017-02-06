using DeXign.Core.Controls;
using System.Windows;
using DeXign.Extension;
using WPFExtension;

namespace DeXign.Core
{
    [XForms("ColumnDefinition")]
    public class PColumnDefinition : PObject, IDefinition
    {
        public static DependencyProperty WidthProperty
            = DependencyHelper.Register();

        [XForms("Width")]
        public PGridLength Width
        {
            get { return GetValue<PGridLength>(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
    }
}
