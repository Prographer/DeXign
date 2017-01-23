using System.Linq;
using System.Reflection;

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
            string contentProperty = component.Attribute.ContentProperty;

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