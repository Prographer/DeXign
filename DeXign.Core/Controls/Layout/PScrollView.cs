using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DeXign.Core.Controls
{
    [ContentProperty("Children")]
    [DesignElement(Category = Constants.Designer.Layout, DisplayName = "스크롤")]
    [XForms("Xamarin.Forms", "ScrollView")]
    public class PScrollView : PLayout
    {
    }
}
