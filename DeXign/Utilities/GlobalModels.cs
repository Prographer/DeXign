using DeXign.Core;

using System;
using System.Collections.Generic;

namespace DeXign.Utilities
{
    internal static class GlobalModels
    {
        static Dictionary<Guid, PObject> dict = new Dictionary<Guid, PObject>();

        public static void Register(Guid guid, PObject model)
        {
            if (!guid.Equals(model.Guid))
                return;

            dict[guid] = model;
        }

        public static void UnRegister(Guid guid)
        {
            if (HasModel(guid))
                dict.Remove(guid);
        }

        public static void UnRegister(PObject model)
        {
            UnRegister(model.Guid);
        }

        public static bool HasModel(Guid guid)
        {
            return dict.ContainsKey(guid);
        }

        public static bool HasModel(PObject model)
        {
            return HasModel(model.Guid);
        }

        public static T GetModel<T>(Guid guid)
            where T : PObject
        {
            if (HasModel(guid))
            {
                PObject obj = dict[guid];

                if (obj is T model)
                    return model;
            }

            return null;
        }

        public static IEnumerable<PObject> Items
        {
            get { return dict.Values; }
        }
    }
}
