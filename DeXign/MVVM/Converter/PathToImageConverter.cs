using DeXign.Resources;

using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DeXign.Converter
{
    public class PathToImageConverter : BaseValueConverter<string, ImageSource>
    {
        public override ImageSource Convert(string value, object parameter)
        {
            var img = ResourceManager.GetImageSourceFromPath(value);
        
            return img;
        }

        public override string ConvertBack(ImageSource value, object parameter)
        {
            return (value as BitmapImage).UriSource.AbsolutePath;
        }
    }
}
