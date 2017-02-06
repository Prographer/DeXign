using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [XForms("Xamarin.Forms", "Page")]
    public class PPage : PVisual
    {
        private static readonly DependencyProperty TitleProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty PaddingProperty =
            DependencyHelper.Register();

        [XForms("Title")]
        public string Title { get; set; }

        [XForms("Padding")]
        public Thickness Padding
        {
            get { return GetValue<Thickness>(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
    }
}
