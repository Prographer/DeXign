using System.Windows;

namespace DeXign.Extension
{
    public static class BoolEx
    {
        public static Visibility ToVisibility(this bool value, Visibility invisible = Visibility.Collapsed)
        {
            return value ? Visibility.Visible : invisible;
        }

        public static bool Nomalize(this bool? value)
        {
            return value != null && value.Value;
        }
    }
}
