using System;
using System.Windows.Media;

using DeXign.Core;
using DeXign.Resources;

namespace DeXign.Converter
{
    class ToolboxIconConverter : BaseValueConverter<object, ImageSource>
    {
        public override ImageSource Convert(object value, object parameter)
        {
            Type type = null;

            if (value is Type t)
                type = t;

            if (value is PObject pObj)
                type = pObj.GetType();

            if (type == null)
                return null;

            return ResourceManager.GetToolboxIcon(type);
        }

        public override object ConvertBack(ImageSource value, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
