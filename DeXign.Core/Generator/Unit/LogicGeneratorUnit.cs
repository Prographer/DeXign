using DeXign.Extension;
using DeXign.Core.Logic;

using System.Collections.Generic;

namespace DeXign.Core
{
    public class LogicGeneratorUnit : CodeGeneratorUnit<PBinderHost>
    {
        public LogicGeneratorUnit()
        {
        }

        public LogicGeneratorUnit(IEnumerable<PBinderHost> items) : base(items)
        {
        }

        public override IEnumerable<CodeComponent<TAttribute>> GetComponents<TAttribute>()
        {
            if (Items != null)
            {
                foreach (PObject item in Items)
                {
                    if (!item.HasAttribute<TAttribute>())
                        continue;

                    var attr = item.GetAttribute<TAttribute>();
                    var component = new CodeComponent<TAttribute>(item, attr);
                    var stack = new Stack<CodeComponent<TAttribute>>(new[] { component });

                    component.ElementType = CodeComponentType.Node;

                    while (stack.Count > 0)
                    {
                        var cc = stack.Pop();

                        if (NodeIterating || (!NodeIterating && cc.Depth == 0))
                            yield return cc;

                        if (cc.ElementType == CodeComponentType.Node)
                        {
                            var host = cc.Element as PBinderHost;

                            foreach (PBinderHost childNode in BinderHelper.FindHostNodes(host, BindOptions.Output, 0))
                            {
                                
                            }
                        }
                    }
                }
            }
        }
    }
}