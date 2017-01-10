using Phlet.Core.Collections;
using Phlet.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Phlet.Core.Controls
{
    public class PLayout : PControl
    {
        public static readonly DependencyProperty PaddingProperty =
            DependencyHelper.Register();

        [XForms("Padding")]
        public Thickness Padding
        {
            get { return GetValue<Thickness>(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
    }

    public class PLayout<T> : PLayout where T : PControl
    {
        [XForms("Children")]
        public PControlCollection<T> Children { get; } = new PControlCollection<T>();
    }
}
