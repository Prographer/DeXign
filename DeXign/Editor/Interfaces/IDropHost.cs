namespace DeXign.Editor
{
    interface IDropHost<T>
    {
        bool CanDrop(T item);
        void OnDrop(T item);
    }
}
