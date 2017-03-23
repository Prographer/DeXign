using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("HorizontalTextAlignment", "Xamarin.Forms")]
    [WPF("HorizontalAlignment", "System.Windows")]
    public enum PHorizontalTextAlignment
    {
        [Description("왼쪽 정렬")]
        [XForms("Start")]
        [WPF("Left")]
        Left = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        [WPF("Center")]
        Center = 1,

        [Description("오른쪽 정렬")]
        [XForms("End")]
        [WPF("Right")]
        Right = 2
    }
}
