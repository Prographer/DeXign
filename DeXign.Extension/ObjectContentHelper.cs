using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace DeXign.Extension
{
    public static class ObjectContentHelper
    {
        public static object GetContent(this object obj)
        {
            var attr = obj.GetAttribute<ContentPropertyAttribute>();

            if (attr != null)
            {
                var contentPropertyInfo = obj
                    .GetType()
                    .GetProperty(attr.Name);

                if (contentPropertyInfo.CanCastingTo<DependencyObject>() ||
                    contentPropertyInfo.PropertyType == typeof(object))
                {
                    return contentPropertyInfo;
                }
                else if (contentPropertyInfo.CanCastingTo<IList>())
                {
                    return (IList)contentPropertyInfo.GetValue(obj);
                }
            }

            return null;
        }


        public static void GetContent(
            this object obj,
            Action<PropertyInfo> singleContent,
            Action<IList> listContent,
            Action failed = null)
        {
            object content = obj.GetContent();

            if (content == null)
            {
                failed?.Invoke();
                return;
            }

            if (content is PropertyInfo)
                singleContent?.Invoke(
                    content as PropertyInfo);

            if (content is IList)
                listContent?.Invoke(
                    (IList)content);
        }
    }
}
