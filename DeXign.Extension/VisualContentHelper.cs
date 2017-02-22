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
    public static class VisualContentHelper
    {
        public static void GetContent(
            this DependencyObject obj,
            Action<PropertyInfo> singleContent,
            Action<IList> listContent,
            Action failed = null)
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
                    singleContent?.Invoke(
                        contentPropertyInfo);

                    return;
                }
                else if (contentPropertyInfo.CanCastingTo<IList>())
                {
                    listContent?.Invoke(
                        (IList)contentPropertyInfo.GetValue(obj));

                    return;
                }
            }

            failed?.Invoke();
        }
    }
}
