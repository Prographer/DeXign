using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("VerticalTextAlignment", "Xamarin.Forms")]
    public enum PVerticalTextAlignment
    {
        [Description("위쪽 정렬")]
        [XForms("Start")]
        Top = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        Center = 1,

        [Description("아래쪽 정렬")]
        [XForms("End")]
        Bottom = 2
    }
}
