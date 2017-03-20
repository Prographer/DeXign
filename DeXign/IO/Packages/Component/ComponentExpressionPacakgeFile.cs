using DeXign.Core.Logic;
using System.Collections.Generic;
using System.Linq;

namespace DeXign.IO
{
    internal class ComponentExpressionPackageFile : ModelPackageFile<ObjectContainer<BindExpression>>
    {
        public const string Path = "Component";
        public const string FileName = "Component\\Expressions.xml";

        public ComponentExpressionPackageFile(List<PComponent> model)
        {
            this.Name = FileName;

            this.Model = new ObjectContainer<BindExpression>();
            this.Model.Items = new List<BindExpression>();

            this.Model.Items.AddRange(
                model.SelectMany(c => c.Items
                     .GetExpressions()
                     .Select(e => new BindExpression()
                     {
                         Output = (e.Output as PBinder).Guid,
                         Input = (e.Input as PBinder).Guid
                     }))
                     .Distinct());

            base.CreateStream();
        }
    }
}
