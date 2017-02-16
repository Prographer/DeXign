using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace DeXign.Core
{
    public static class CodeComponentEx
    {
        public static bool HasContentComponent(this CodeComponent<XFormsAttribute> component)
        {
            return component.GetContentComponent() != null;
        }

        public static CodeComponent<XFormsAttribute> GetContentComponent(this CodeComponent<XFormsAttribute> component)
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
                    .Where(c => c.ElementType == ComponentType.Property)
                    .Where(c => (c.Element as PropertyInfo).Name == contentProperty)
                    .FirstOrDefault();
            }

            return null;
        }
    }
}