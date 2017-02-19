namespace DeXign.Core.Controls
{
    interface IFontControl
    {
        PFontAttributes FontAttributes { get; }
        string FontFamily { get; }
        double FontSize { get; }
    }
}
