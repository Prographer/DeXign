using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace DeXign.Core
{
    public static class CodeComponentEx
    {
        public static bool HasContentComponent<T>(this CodeComponent<T> component)
            where T : GenerateAttribute
        {
            return component.GetContentComponent() != null;
        }

        public static CodeComponent<T> GetContentComponent<T>(this CodeComponent<T> component)
            where T : GenerateAttribute
        {
            string contentProperty = "";
            var contentAttr = component.Element
                .GetType()
                .GetCustomAttribute<ContentPropertyAttribute>();

            if (contentAttr != null)
                contentProperty = contentAttr.Name;

            if (component.Children != null && !string.IsNullOrEmpty(contentProperty))
            {
                return component.Children
                    .Where(c => c.ElementType == CodeComponentType.Property)
                    .Where(c => (c.Element as PropertyInfo).Name == contentProperty)
                    .FirstOrDefault();
            }

            return null;
        }
    }
}