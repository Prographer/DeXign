using System.Windows.Media;

namespace Phlet.Extension
{
    enum HardwareAcceleration
    {
        NotSupport = 0,
        PartialSupport = 1,
        Support = 2
    }

    static class Hardware
    {
        public static HardwareAcceleration Acceleration
        {
            get
            {
                return (HardwareAcceleration)(RenderCapability.Tier >> 16);
            }
        }
    }
}