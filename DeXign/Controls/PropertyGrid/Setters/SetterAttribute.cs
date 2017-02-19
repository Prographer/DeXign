using System;

namespace DeXign.Controls
{

    class SetterAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}
