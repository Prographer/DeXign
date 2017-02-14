using System;
using System.Windows.Markup;

namespace DeXign.Models
{
    class ResolutionItemModel : MarkupExtension
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Inch { get; set; }

        public string Title => $"{Width} x {Height}";

        public string SubTitle { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
