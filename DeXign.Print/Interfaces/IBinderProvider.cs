namespace DeXign.Logic
{
    public interface IBinderProvider
    {
        bool CanBind(IBinder outputBinder, BinderOptions options);
        void Bind(IBinder outputBinder, BinderOptions options);

        void ReleaseInput(IBinder outputBinder); // 들어오는 바인더 제거
        void ReleaseOutput(IBinder inputBinder); // 나가는 바인더 제거

        void ReleaseAll(); // 연결된 모든 바인더 제거
    }
}
