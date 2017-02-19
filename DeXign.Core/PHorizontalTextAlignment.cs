using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("HorizontalTextAlignment", "Xamarin.Forms")]
    public enum PHorizontalTextAlignment
    {
        [Description("왼쪽 정렬")]
        [XForms("Start")]
        Left = 0,

        [Description("가운데 정렬")]
        [XForms("Center")]
        Center = 1,

        [Description("오른쪽 정렬")]
        [XForms("End")]
        Right = 2
    }
}
