using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Markup.Primitives;

namespace DeXign.Utilities
{
    public class DumpDependencyObject
    {
        public DependencyObject Object { get; }
        
        private Dictionary<DependencyProperty, object> values;
        private Dictionary<DependencyPropertyKey, object> readonlyValues;

        public DumpDependencyObject(DependencyObject obj)
        {
            values = new Dictionary<DependencyProperty, object>();
            readonlyValues = new Dictionary<DependencyPropertyKey, object>();

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
            foreach (var kv in values)
            {
                destination.SetValue(kv.Key, kv.Value);
            }
        }
    }
}
