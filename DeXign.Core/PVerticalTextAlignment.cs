using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("VerticalTextAlignment", "Xamarin.Forms")]
    [WPF("VerticalAlignment", "System.Windows")]
    public enum PVerticalTextAlignment
    {
        [Description("위쪽 정렬")]
        [XForms("Start")]
        [WPF("Top")]
        Top = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        [WPF("Center")]
        Center = 1,

        [Description("아래쪽 정렬")]
        [XForms("End")]
        [WPF("Bottom")]
        Bottom = 2
    }
}
