using System.Windows;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "진행바")]
    [DesignElementIgnore("Background")]
    [XForms("Xamarin.Forms", "ProgressBar")]
    public class PProgressBar : PControl
    {
        public static readonly DependencyProperty ProgressProperty =
            DependencyHelper.Register(new PropertyMetadata(0.5d));

        [DesignElement(Key = "Percentage", Category = Constants.Property.Design, DisplayName = "진행도")]
        [XForms("Progress")]
        public double Progress
        {
            get { return GetValue<double>(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
    }
}
