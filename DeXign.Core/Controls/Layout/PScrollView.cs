using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [ContentProperty("Content")]
    [DesignElement(Category = Constants.Designer.Layout, DisplayName = "스크롤")]
    [XForms("Xamarin.Forms", "ScrollView")]
    public class PScrollView : PLayout
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyHelper.Register();

        [XForms("Content")]
        public PObject Content
        {
            get { return GetValue<PObject>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}
