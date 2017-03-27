using DeXign.Core.Logic;
using DeXign.Core.Controls;

namespace DeXign.Core.Compiler
{
    public class DXCompileParameter
    {
        public DXCompileOption Option { get; set; }
        public PContentPage[] Screens { get; set; }
        public PBinderHost[] Components { get; set; }

        public DXCompileParameter()
        {
        }

        public DXCompileParameter(DXCompileOption option, PContentPage[] screens, PBinderHost[] components)
        {
            this.Option = option;
            this.Screens = screens;
            this.Components = components;
        }
    }
}
