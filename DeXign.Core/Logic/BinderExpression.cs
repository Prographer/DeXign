namespace DeXign.Core.Logic
{
    public class BinderExpression
    {
        public IBinder Output { get; }
        public IBinder Input { get; }

        internal BinderExpression(IBinder output, IBinder input)
        {
            this.Output = output;
            this.Input = input;
        }

        public void Release()
        {
            this.Output.Release(this.Input);
        }
    }
}