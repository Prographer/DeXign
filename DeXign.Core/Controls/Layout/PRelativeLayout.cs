using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DeXign.Core.Controls
{
    [ContentProperty("Children")]
    [XForms("Xamarin.Forms", "RelativeLayout")]
    public class PRelativeLayout : PLayout<PControl>
    {
    }
}
