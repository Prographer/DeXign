using System.ComponentModel;

namespace DeXign.Core
{
    [XForms("Aspect", "Xamarin.Forms")]
    [WPF("Stretch", "System.Windows.Media")]
    public enum PStretch
    {
        [Description("채우기")]
        [XForms("Fill")]
        [WPF("Fill")]
        Fill = 1,

        [Description("비율")]
        [XForms("AspectFit")]
        [WPF("Uniform")]
        Uniform = 2,

        [Description("비율 채우기")]
        [XForms("AspectFill")]
        [WPF("UniformToFill")]
        UniformToFill = 3
    }
}
