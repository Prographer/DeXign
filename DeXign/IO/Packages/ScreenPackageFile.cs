using System.IO;

using DeXign.Core;
using DeXign.Core.Controls;

namespace DeXign.IO
{
    internal class ScreenPackageFile : ModelPackageFile<PContentPage>
    {
        public const string Path = "Screens";

        public ScreenPackageFile(PContentPage model) : base(model)
        {
        }

        protected override void CreateStream()
        {
            base.CreateStream();

            this.Name = $"{Path}\\{LayoutExtension.GetPageName(this.Model)}.xml";
        }
    }
}
