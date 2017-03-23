using System.ComponentModel;

namespace DeXign.Core
{
    [WPF("System.Windows.Controls", "Orientation")]
    public enum POrientation : int
    {
        [Description("가로")]
        [WPF("Horizontal")]
        Horizontal = 0,

        [Description("세로")]
        [WPF("Vertical")]
        Vertical = 1
    }
}
