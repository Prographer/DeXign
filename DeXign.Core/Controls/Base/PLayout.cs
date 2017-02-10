using DeXign.Core.Collections;
using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFExtension;

namespace DeXign.Core.Controls
{
    public class PLayout : PControl
    {
        public static readonly DependencyProperty PaddingProperty =
            DependencyHelper.Register();

        [DesignElement(Category = Constants.Property.Blank, DisplayName = "안쪽 여백")]
        [XForms("Padding")]
        public Thickness Padding
        {
            get { return GetValue<Thickness>(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
    }

    [XForms("Blank", "Xamarin.Forms", ContentProperty = "Children")]
    public class PLayout<T> : PLayout 
        where T : PControl
    {
        [XForms("Children")]
        public PControlCollection<T> Children { get; } = new PControlCollection<T>();
    }
}
