using System;
using DeXign.Core;
using System.Collections.Generic;
using System.Linq;

namespace DeXign.Editor.Controls
{
    public partial class Storyboard : INameScope
    {
        Dictionary<PObject, string> nameDict =
            new Dictionary<PObject, string>();

        public void Register(PObject obj, string name)
        {
            nameDict[obj] = name;
        }

        public void Unregister(PObject obj)
        {
            if (nameDict.ContainsKey(obj))
                nameDict.Remove(obj);
        }

        public string GetName(PObject obj)
        {
            if (nameDict.ContainsKey(obj))
                return nameDict[obj];

            return null;
        }

        public bool HasName(string name)
        {
            return nameDict.ContainsValue(name);
        }

        public PObject GetOwner(string name)
        {
            return nameDict
                .Where(kv => kv.Value == name)
                .Select(kv => kv.Key)
                .FirstOrDefault();
        }
    }
}
