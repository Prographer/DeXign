namespace DeXign.Core.Logic
{
    public enum BindDirection
    {
        Input = BindOptions.Input | BindOptions.Parameter,
        Output = BindOptions.Output | BindOptions.Return
    }
}
