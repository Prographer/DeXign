using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace DeXign.Core.Designer
{
    [DictionaryKeyProperty("TargetType")]
    [ContentProperty("Content")]
    public class DesignerResource : DispatcherObject
    {
        public object Content { get; set; }

        public object ToolTip { get; set; }
    }
}
