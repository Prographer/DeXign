using DeXign.Core.Controls;
using DeXign.Core.Logic;

using System.Text;

namespace DeXign.Core.Compiler
{
    internal class WPFCodeBuilder : CSBuilder
    {
        public WPFCodeBuilder(DXCompileOption option, PContentPage[] screens, PBinderHost[] components) : base(option, screens, components)
        {
        }

        public string AppWindow()
        {
            return "";
        }

        public string PageInitialize()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < this.Screens.Length; i++)
            {
                sb.AppendLine($"var p{i} = GenResourceManager.LoadXaml(\"{this.Screens[i].GetPageName()}.xaml\") as Page;");
                sb.AppendLine($"dw.Add(p{i});");
            }

            sb.AppendLine($"dw.SetPage(p0);");

            return sb.ToString();
        }
    }
}
