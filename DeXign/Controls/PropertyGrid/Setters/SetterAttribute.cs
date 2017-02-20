using System;

namespace DeXign.Controls
{

    class SetterAttribute : Attribute
    {
        public string Key { get; set; }
        public Type Type { get; set; }
    }
}
