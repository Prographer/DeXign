using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("LayoutOptions", "Xamarin.Forms")]
    [WPF("VerticalAlignment", "System.Windows")]
    public enum PVerticalAlignment
    {
        [Description("위쪽 정렬")]
        [XForms("Begin")]
        [WPF("Top")]
        Top = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        [WPF("Center")]
        Center = 1,

        [Description("아래쪽 정렬")]
        [XForms("End")]
        [WPF("Bottom")]
        Bottom = 2,

        [Description("채우기")]
        [XForms("Fill")]
        [WPF("Stretch")]
        Stretch = 3
    }
}
