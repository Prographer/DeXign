using System;
using System.Collections.Generic;

using DeXign.Core.Controls;

namespace DeXign.Core
{
    public static class LayoutExtension
    {
        static Dictionary<PPage, string> names = 
            new Dictionary<PPage, string>();

        public static void SetPageName(this PPage page, string name)
        {
            if (!StringRule.CheckNamingRule(name))
                throw new Exception("이름 명명규칙에 어긋납니다.");

            names[page] = name;
        }

        public static string GetPageName(this PPage page)
        {
            if (names.ContainsKey(page))
                return names[page];

            return null;
        }
    }
}