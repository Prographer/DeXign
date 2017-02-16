using System;
using System.Collections.Generic;
using System.Windows;

namespace DeXign.Theme
{
    public static class ThemeManager
    {
        private static Dictionary<Platform, ResourceDictionary> themes;

        static ThemeManager()
        {
            themes = new Dictionary<Platform, ResourceDictionary>()
            {
                {
                    Platform.Android, new ResourceDictionary()
                    {
                        Source = new Uri("/DeXign;component/Themes/Platforms/AndroidStyle.xaml", UriKind.RelativeOrAbsolute)
                    }
                },
                {
                    Platform.iOS, new ResourceDictionary()
                    {
                        Source = new Uri("/DeXign;component/Themes/Platforms/iOSStyle.xaml", UriKind.RelativeOrAbsolute)
                    }
                },
                {
                    Platform.Window, new ResourceDictionary()
                    {
                        Source = new Uri("/DeXign;component/Themes/Platforms/WindowStyle.xaml", UriKind.RelativeOrAbsolute)
                    }
                }
            };
        }

        public static ResourceDictionary GetTheme(Platform platform)
        {
            if (themes.ContainsKey(platform))
                return themes[platform];

            return null;
        }
    }
}
