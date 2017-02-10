using DeXign.Core.Controls;

namespace DeXign.Designer.Interfaces
{
    interface IRenderer<T> 
        where T : PObject
    {
        T Model { get; }

        void AttachModel(T model);
        void DettachModel();
    }
}
