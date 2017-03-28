using DeXign.Extension;
using DeXign.Core.Logic;

using System.Collections.Generic;
using System.Linq;

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
                foreach (PObject item in Items
                    .SelectMany(host => host.Items.GetExpressions())
                    .Select(e => e.Output.Host)
                    .Distinct())
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

                        //if (NodeIterating || (!NodeIterating && cc.Depth == 0))
                        //    yield return cc;

                        if (cc.ElementType == CodeComponentType.Node)
                        {
                            var host = cc.Element as PBinderHost;
                            var zeroNodes = new List<PBinderHost>();

                            var childStack = new Stack<PBinderHost>(new[] { host });

                            // 호스트와 연결된 모든 노드를 BFS알고리즘을 사용하여 가져오고
                            // 생성된 Output Binder의 갯수가 1개인 것만 노드로 인정함
                            // 갯수가 2개이상인 노드에서 멈춤 (2개이상은 스코프가 분기된다고 가정)
                            while (childStack.Count > 0)
                            {
                                PBinderHost child = childStack.Pop();

                                foreach (PBinderHost childNode in BinderHelper.FindHostNodes(child, BindOptions.Output, 0))
                                {
                                    if (childNode[BindOptions.Output].Count() == 1 && childNode is PTrigger == false)
                                    {
                                        childStack.Push(childNode);
                                    }

                                    zeroNodes.Add(childNode);
                                }
                            }

                            foreach (PBinderHost zeroNode in zeroNodes)
                            {
                                var childComponent = new CodeComponent<TAttribute>(
                                        zeroNode,
                                        zeroNode.GetAttribute<TAttribute>())
                                {
                                    Depth = cc.Depth + 1,
                                    ElementType = CodeComponentType.Node
                                };

                                cc.Add(childComponent);

                                if (zeroNode[BindOptions.Output].Count() > 1 || zeroNode is PTrigger)
                                {
                                    stack.Push(childComponent);
                                }
                            }
                        }
                    }

                    yield return component;
                }
            }
        }
    }
}