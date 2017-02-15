namespace DeXign.Editor.Interfaces
{
    interface IDropHost
    {
        bool CanDrop(object item);
        void OnDrop(object item);
    }
}
