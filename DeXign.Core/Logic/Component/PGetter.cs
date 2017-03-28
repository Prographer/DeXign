namespace DeXign.Core.Logic
{
    [CSharpCodeMap("{Target}.{Property:Property}")]
    [JavaCodeMap("{Target}.get{Property:Property}()")]
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "가져오기")]
    public class PGetter : PTargetable
    {
        public PGetter() : base()
        {    
        }
    }
}