using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("Stretch", "Xamarin.Forms")]
    public enum PStretch
    {
        [Description("원본")]
        None = 0,

        [Description("채우기")]
        Fill = 1,

        [Description("비율")]
        Uniform = 2,

        [Description("비율 채우기")]
        UniformToFill = 3
    }
}
