using System.Collections;

namespace DeXign.Extension
{
    public static class ListEx
    {
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
