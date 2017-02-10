namespace DeXign.Designer.Interfaces
{
    interface IDropHost
    {
        bool CanDrop(object item);
        void Drop(object item);
    }
}
