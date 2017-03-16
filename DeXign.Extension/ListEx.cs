using System.Collections;
using System.Collections.Generic;

namespace DeXign.Extension
{
    public static class ListEx
    {
        public static void SafeRemoveRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.SafeRemove(item);
            }
        }

        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.Remove(item);
            }
        }

        public static void SafeAddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.SafeAdd(item);
            }
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.Add(item);
            }
        }

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

        public static void SafeAdd(this IList list, object item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void SafeRemove(this IList list, object item)
        {
            if (list.Contains(item))
                list.Remove(item);
        }
    }
}
