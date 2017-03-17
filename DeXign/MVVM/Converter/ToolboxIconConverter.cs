using System;
using System.Windows.Media;

using DeXign.Resources;
using DeXign.Core.Controls;

namespace DeXign.Converter
{
    class ToolboxIconConverter : BaseValueConverter<object, ImageSource>
    {
        public override ImageSource Convert(object value, object parameter)
        {
            if (value is PVisual == false)
                return null;

            if (value == null)
                return null;
            
            // PObject Type -> Icon
            return ResourceManager.GetToolboxIcon(value.GetType());
        }

        public override object ConvertBack(ImageSource value, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
