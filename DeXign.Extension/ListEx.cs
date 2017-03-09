using System.Collections;
using System.Collections.Generic;

namespace DeXign.Extension
{
    public static class ListEx
    {
        public static void SafeAdd<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void SafeRemove<T>(this IList<T> list, T item)
        {
            if (list.Contains(item))
                list.Remove(item);
        }

        public static void SafeAdd<T>(this IList list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void SafeRemove<T>(this IList list, T item)
        {
            if (list.Contains(item))
                list.Remove(item);
        }
    }
}
