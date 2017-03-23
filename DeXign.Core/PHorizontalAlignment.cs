using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("LayoutOptions", "Xamarin.Forms")]
    [WPF("HorizontalAlignment", "System.Windows")]
    public enum PHorizontalAlignment
    {
        [Description("왼쪽 정렬")]
        [XForms("Begin")]
        [WPF("Left")]
        Left = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        [WPF("Center")]
        Center = 1,

        [Description("오른쪽 정렬")]
        [XForms("End")]
        [WPF("Right")]
        Right = 2,

        [Description("채우기")]
        [XForms("Fill")]
        [WPF("Stretch")]
        Stretch = 3
    }
}
