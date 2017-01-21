﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Phlet.Core
{
    public class CodeGeneratorUnit<TElement> 
        where TElement : class, new()
    {
        public bool NodeIterating { get; set; } = false;
        public List<TElement> Items { get; }

        public CodeGeneratorUnit()
        {
            Items = new List<TElement>();
        }

        public IEnumerable<CodeComponent<TAttribute>> GetComponents<TAttribute>()
            where TAttribute : Attribute
        {
            if (Items != null)
            {
                foreach (TElement item in Items)
                {
                    if (!item.HasAttribute<TAttribute>())
                        continue;

                    var attr = item.GetAttribute<TAttribute>();
                    var component = new CodeComponent<TAttribute>(item, attr);
                    var stack = new Stack<CodeComponent<TAttribute>>(new[] { component });

                    while (stack.Count > 0)
                    {
                        var cc = stack.Pop();

                        if (NodeIterating || (!NodeIterating && cc.Depth == 0))
                            yield return cc;

                        if (cc.ElementType == ComponentType.Instance)
                        {
                            // 클래스 인경우
                            Type eType = cc.Element.GetType();

                            foreach (PropertyInfo pi in eType
                                .GetProperties()
                                .Where(pi => pi.HasAttribute<TAttribute>()))
                            {
                                var pAttr = pi.GetAttribute<TAttribute>();
                                var pComponet = new CodeComponent<TAttribute>(pi, pAttr)
                                {
                                    Depth = cc.Depth + 1
                                };

                                cc.Add(pComponet);
                                stack.Push(pComponet);
                            }
                        }
                        else
                        {
                            var pi = cc.Element as PropertyInfo;
                            
                            // 배열인 경우
                            if (pi.CanCastingTo<IEnumerable<TElement>>())
                            {
                                var list = (IEnumerable)pi.GetValue(cc.Parent.Element);

                                if (list == null)
                                    continue;

                                foreach (var obj in list)
                                {
                                    var oAttr = obj.GetAttribute<TAttribute>();
                                    var oComponent = new CodeComponent<TAttribute>(obj, oAttr)
                                    {
                                        Depth = cc.Depth + 1
                                    };

                                    cc.Add(oComponent);
                                    stack.Push(oComponent);
                                }
                            }
                            else if (pi.CanCastingTo<TElement>())
                            {
                                var obj = pi.GetValue(cc.Parent.Element);

                                if (obj == null)
                                    continue;

                                var oAttr = obj.GetAttribute<TAttribute>();
                                var oComponent = new CodeComponent<TAttribute>(obj, oAttr)
                                {
                                    Depth = cc.Depth + 1
                                };

                                cc.Add(oComponent);
                                stack.Push(oComponent);
                            }
                            
                        }
                    }
                }
            }
        }
    }
}