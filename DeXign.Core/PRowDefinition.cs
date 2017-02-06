using DeXign.Core.Controls;
using System.Windows;
using DeXign.Extension;
using WPFExtension;

namespace DeXign.Core
{
    [XForms("RowDefinition")]
    public class PRowDefinition : PObject, IDefinition
    {
        public static DependencyProperty HeightProperty
            = DependencyHelper.Register();

        [XForms("Height")]
        public PGridLength Height
        {
            get { return GetValue<PGridLength>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
    }
}
