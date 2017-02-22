namespace DeXign.Logic
{
    public interface IBinderProvider
    {
        bool CanBind(IBinder binder);
        void Bind(IBinder binder);

        void Release(); // release from Input.Outputs
        void Release(IBinder binder); // release from this.Outputs
    }
}
