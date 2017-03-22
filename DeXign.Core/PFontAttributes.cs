using System;

namespace DeXign.Core
{
    [Flags]
    [WPF("System.Windows", "FontWeights")]
    public enum PFontAttributes
    {
        [WPF("Normal")]
        None = 0,

        [WPF("Bold")]
        Bold = 1,

        [WPF("System.Windows", "FontStyles.Italic")]
        Italic = 2
    }
}