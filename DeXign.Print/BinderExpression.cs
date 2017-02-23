namespace DeXign.Logic
{
    public class BinderExpression
    {
        public IBinder Input { get; }
        public IBinder Output { get; }

        public BinderOptions BindOptions { get; }

        internal BinderExpression(IBinder input, IBinder output, BinderOptions bindOptions)
        {
            this.Input = input;
            this.Output = output;
        }
    }
}