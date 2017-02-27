namespace DeXign.Core.Logic
{
    public class BinderExpression
    {
        public BaseBinder Input { get; }
        public BaseBinder Output { get; }

        public BinderOptions BindOptions { get; }

        internal BinderExpression(BaseBinder input, BaseBinder output, BinderOptions bindOptions)
        {
            this.Input = input;
            this.Output = output;
            this.BindOptions = bindOptions;
        }

        public void Release()
        {
            Input.ReleaseInput(Output);
        }
    }
}