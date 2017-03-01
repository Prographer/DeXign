using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("Aspect", "Xamarin.Forms")]
    public enum PStretch
    {
        [Description("채우기")]
        [XForms("Fill")]
        Fill = 1,

        [Description("비율")]
        [XForms("AspectFit")]
        Uniform = 2,

        [Description("비율 채우기")]
        [XForms("AspectFill")]
        UniformToFill = 3
    }
}
