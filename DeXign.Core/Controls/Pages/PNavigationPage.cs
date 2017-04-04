using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [XForms("Xamarin.Forms", "NavigationPage")]
    public class PNavigationPage : PPage
    {
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyHelper.Register();

        private static readonly DependencyPropertyKey AccentColorPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty AccentColorProperty =
            AccentColorPropertyKey.DependencyProperty;

        public static readonly DependencyProperty BarBackgroundProperty =
            DependencyHelper.Register();
        
        public PPage CurrentPage
        {
            get { return this.GetValue<PPage>(CurrentPageProperty); }
        }

        [XForms("Tint")]
        public SolidColorBrush AccentColor
        {
            get { return this.GetValue<SolidColorBrush>(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }

        [XForms("BarBackgroundColor")]
        public SolidColorBrush BarBackground
        {
            get { return this.GetValue<SolidColorBrush>(BarBackgroundProperty); }
            set { SetValue(BarBackgroundProperty, value); }
        }

        public PNavigationPage()
        {   
        }

        public PNavigationPage(PPage root)
        {
        }
    }
}