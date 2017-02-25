using DeXign.Core.Controls;

namespace DeXign.Core
{
    public interface INameScope
    {
        void Register(PObject obj, string name);
        void Unregister(PObject obj);

        string GetName(PObject obj);
        bool HasName(string name);
    }
}