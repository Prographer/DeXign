using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using DeXign.Core.Designer;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DeXign.Resources
{
    public static class ResourceManager
    {
        private static Dictionary<string, BitmapImage> imageCache =
            new Dictionary<string, BitmapImage>();

        public static FontFamily GetFont(string fontName, ResourceDictionary resources = null)
        {
            return GetResource<FontFamily>($"Font.{fontName}", resources);
        }

        public static StreamGeometry GetPath(string pathName, ResourceDictionary resources = null)
        {
            return GetResource<StreamGeometry>($"Path.{pathName}", resources);
        }

        public static Color GetColor(string colorName, ResourceDictionary resources = null)
        {
            return GetResource<Color>($"Color.{colorName}", resources);
        }

        public static Brush GetBrush(string brushName, ResourceDictionary resources = null)
        {
            return GetResource<Brush>($"Brush.{brushName}", resources);
        }

        public static IValueConverter GetConverter(string name, ResourceDictionary resources = null)
        {
            return GetResource<IValueConverter>($"Converter.{name}", resources);
        }

        public static Style GetStyle(string styleName, ResourceDictionary resources = null)
        {
            return GetResource<Style>(styleName, resources);
        }

        public static DesignerResource GetDesignerResource(Type type, ResourceDictionary resources = null)
        {
            return GetResource<DesignerResource>(type, resources);
        }

        public static DesignerResource GetDesignerResource<T>(ResourceDictionary resources = null)
        {
            return GetResource<DesignerResource>(typeof(T), resources);
        }

        public static DesignerResource GetDesignerResource(string name, ResourceDictionary resources = null)
        {
            return GetResource<DesignerResource>(name, resources);
        }

        public static ImageSource GetToolboxIcon(Type type)
        {
            if (App.Current.Resources.Contains(type))
            {
                var res = App.Current.Resources[type] as DesignerResource;
                var img = res.Content as Image;

                return img.Source;
            }

            return null;
        }

        public static BitmapImage GetImageSource(string name)
        {
            string path = $"pack://application:,,,/DeXign;component/Resources/{name}";

            return GetImageSourceFromPath(path);
        }

        public static BitmapImage GetImageSourceFromPath(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (!imageCache.ContainsKey(name))
                imageCache[name] = new BitmapImage(new Uri(name));

            return imageCache[name];
        }

        public static T GetResource<T>(object name, ResourceDictionary resources = null)
        {
            var res = resources ?? App.Current.Resources;

            if (res.Contains(name))
                return (T)res[name];

            return default(T);
        }
    }
}