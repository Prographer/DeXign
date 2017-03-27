using System;

namespace DeXign.UI
{
    public static class ObjectEx
    {
        public static T Cast<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}
