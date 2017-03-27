namespace DeXign.Core.Logic
{
    [CSharpCodeMap("{Target}")]
    [JavaCodeMap("{Target}")]
    public class PTargetBinder : PParameterBinder
    {
        public PTargetBinder()
        {
        }

        public PTargetBinder(IBinderHost host) : base(host)
        {
        }
    }
}