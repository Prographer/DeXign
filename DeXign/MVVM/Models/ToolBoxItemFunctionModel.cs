using System;
using DeXign.Core;
using DeXign.Core.Designer;
using System.Reflection;

namespace DeXign.Models
{
    internal class ToolBoxItemFunctionModel : ToolBoxItemModel
    {
        public MethodInfo MethodInfo { get; }

        public ToolBoxItemFunctionModel(
            MethodInfo mi, 
            AttributeTuple<DesignElementAttribute, Type> data, 
            DesignerResource resource) 
            : base(data, resource)
        {
            this.MethodInfo = mi;
        }
    }
}
