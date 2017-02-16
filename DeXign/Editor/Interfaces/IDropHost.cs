namespace DeXign.Editor.Interfaces
{
    interface IDropHost<T>
    {
        bool CanDrop(T item);
        void OnDrop(T item);
    }
}
