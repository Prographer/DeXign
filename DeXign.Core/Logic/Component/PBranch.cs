using System.Windows;
using WPFExtension;

namespace DeXign.Core.Logic
{
    [CSharpCodeMap("if ({Line:1}{Property:Operator}{Line:2}){Scope:1}else{Scope:2}")]
    [JavaCodeMap("if ({Line:1}{Property:Operator}{Line:2}){Scope:1}else{Scope:2}")]
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "분기")]
    public class PBranch : PComponent
    {
        public static readonly DependencyProperty OperatorProperty =
            DependencyHelper.Register();
        
        public ComparisonPredicate Operator
        {
            get { return GetValue<ComparisonPredicate>(OperatorProperty); }
            set { SetValue(OperatorProperty, value); }
        }
        
        public PBinder TrueBinder { get; set; }
        public PBinder FalseBinder { get; set; }

        public PParameterBinder Value1Binder { get; set; }
        public PParameterBinder Value2Binder { get; set; }

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
