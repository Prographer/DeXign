using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("LayoutOptions", "Xamarin.Forms")]
    public enum PHorizontalAlignment
    {
        [Description("왼쪽 정렬")]
        [XForms("Begin")]
        Left = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        Center = 1,

        [Description("오른쪽 정렬")]
        [XForms("End")]
        Right = 2,

        [Description("채우기")]
        [XForms("Fill")]
        Stretch = 3
    }
}
