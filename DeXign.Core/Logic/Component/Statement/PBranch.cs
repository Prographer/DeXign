namespace DeXign.Core.Logic
{
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "분기")]
    public class PBranch : PComponent
    {
        public PBinder TrueBinder { get; }
        public PBinder FalseBinder { get; }

        public PParameterBinder Value1Binder { get; }
        public PParameterBinder Value2Binder { get; }

        public PBranch() : base()
        {
            this.AddNewBinder(BindOptions.Input);

            this.Value1Binder = this.AddParamterBinder("값1", typeof(object));
            this.Value2Binder = this.AddParamterBinder("값2", typeof(object));

            this.TrueBinder = new PNamedBinder(this, BindOptions.Output)
            {
                Title = "참"
            };

            this.FalseBinder = new PNamedBinder(this, BindOptions.Output)
            {
                Title = "거짓"
            };

            this.AddBinder(this.TrueBinder);
            this.AddBinder(this.FalseBinder);
        }
    }
}
