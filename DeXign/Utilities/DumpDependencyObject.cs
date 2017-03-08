using System.Windows;
using System.Collections.Generic;
using System.Windows.Markup.Primitives;
using DeXign.Core.Controls;
using System;
using System.Linq;

namespace DeXign.Utilities
{
    public class DumpDependencyObject
    {
        public DependencyObject Object { get; }

        private Dictionary<DependencyProperty, object> values;

        public DumpDependencyObject(DependencyObject obj)
        {
            values = new Dictionary<DependencyProperty, object>();

            this.Object = obj;

            this.Dump();
        }

        public void Dump()
        {
            values.Clear();

            var markupObject = MarkupWriter.GetMarkupObjectFor(this.Object);

            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.DependencyProperty != null)
                    {
                        values[mp.DependencyProperty] = this.Object.GetValue(mp.DependencyProperty);
                    }
                }
            }
        }

        public void RollBack()
        {
            CopyTo(this.Object);
        }

        public void CopyTo(DependencyObject destination)
        {
            foreach (var kv in values
                .OrderBy(item => item.Key.Name.Contains("Alignment") ? 1 : 2))
            {
                destination.SetValue(kv.Key, kv.Value);
            }
        }
    }
}
