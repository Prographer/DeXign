using System;
using System.Windows.Media;

using DeXign.Resources;

namespace DeXign.Converter
{
    class ToolboxIconConverter : BaseValueConverter<object, ImageSource>
    {
        public override ImageSource Convert(object value, object parameter)
        {
            if (value == null)
                return null;
            
            return ResourceManager.GetToolboxIcon(value.GetType());
        }

        public override object ConvertBack(ImageSource value, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
