using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Layout, DisplayName = "스택")]
    [XForms("Xamarin.Forms", "StackLayout", ContentProperty = "Children")]
    public class PStackLayout : PLayout<PControl>
    {
    }
}
