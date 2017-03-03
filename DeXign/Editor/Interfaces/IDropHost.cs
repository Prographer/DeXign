using System.Windows;

namespace DeXign.Editor
{
    interface IDropHost<T>
    {
        bool CanDrop(T item, Point mouse);
        void OnDrop(T item, Point mouse);
    }
}
