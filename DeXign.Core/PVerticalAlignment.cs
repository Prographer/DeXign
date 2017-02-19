using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("LayoutOptions", "Xamarin.Forms")]
    public enum PVerticalAlignment
    {
        [Description("위쪽 정렬")]
        [XForms("Begin")]
        Top = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        Center = 1,

        [Description("아래쪽 정렬")]
        [XForms("End")]
        Bottom = 2,

        [Description("채우기")]
        [XForms("Fill")]
        Stretch = 3
    }
}
