using DeXign.SDK;
using System.Windows;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "진행바")]
    [DesignElementIgnore("Background")]
    [DXIgnore("Background")]
    [XForms("Xamarin.Forms", "ProgressBar")]
    [WPF("System.Windows.Controls", "ProgressBar")]
    public class PProgressBar : PControl
    {
        public static readonly DependencyProperty ProgressProperty =
            DependencyHelper.Register(new PropertyMetadata(0.5d));

        [DesignElement(Key = "Percentage", Category = Constants.Property.Design, DisplayName = "진행도")]
        [XForms("Progress")]
        [WPF("Value")]
        public double Progress
        {
            get { return GetValue<double>(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
    }
}
