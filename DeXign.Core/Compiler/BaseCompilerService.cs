namespace DeXign.Core.Compiler
{
    public abstract class BaseCompilerService
    {
        public Platform Platform { get; set; }

        public abstract DXCompileResult Compile(DXCompileParameter parameter);
    }
}