using System;
using System.Collections.Generic;

namespace Phlet.Core
{ 
    public abstract class Generator<TAttribute, TElement> : IGenerator<TAttribute>
        where TAttribute : Attribute
        where TElement : class, new()
    {
        public CodeGeneratorUnit<TElement> Unit { get; }
        public CodeGeneratorManifest Manifest { get; }
        public CodeGeneratorAssemblyInfo AssemblyInfo { get; }
        
        public Generator(
            CodeGeneratorUnit<TElement> cgUnit,
            CodeGeneratorManifest cgManifest,
            CodeGeneratorAssemblyInfo cgAssmInfo)
        {
            this.Unit = cgUnit;
            this.Manifest = cgManifest;
            this.AssemblyInfo = cgAssmInfo;
        }

        public IEnumerable<string> Generate()
        {
            return OnGenerate(
                Unit.GetComponents<TAttribute>());
        }

        protected abstract IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<TAttribute>> components);
    }
}
