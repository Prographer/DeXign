using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using WPFExtension;

namespace DeXign.Core.Designer
{
    [DictionaryKeyProperty("TargetType")]
    [ContentProperty("Content")]
    public class DesignerResource : DependencyObject
    {
        public static readonly DependencyProperty ToolTipProperty =
            DependencyHelper.Register();

        public object Content { get; set; }

        public object ToolTip
        {
            get { return GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }
    }
}
