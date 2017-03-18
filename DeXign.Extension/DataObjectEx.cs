using System.Windows;

namespace DeXign.Extension
{
    public static class DataObjectEx
    {
        public static bool HasData<T>(this IDataObject dataObject)
        {
            return dataObject.GetDataPresent(typeof(T));
        }

        public static T GetData<T>(this IDataObject dataObject)
        {
            return (T)dataObject.GetData(typeof(T));
        }
    }
}
