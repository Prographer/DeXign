namespace DeXign.Core.Logic
{
    public interface IBinderProvider
    {
        bool CanBind(BaseBinder outputBinder, BinderOptions options);
        void Bind(BaseBinder outputBinder, BinderOptions options);

        void ReleaseInput(BaseBinder outputBinder); // 들어오는 바인더 제거
        void ReleaseOutput(BaseBinder inputBinder); // 나가는 바인더 제거

        void ReleaseAll(); // 연결된 모든 바인더 제거
    }
}
