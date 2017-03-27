using System;
using System.Windows;

namespace DeXign.Core.Logic
{
    [CodeMap("{Target}.{Property:Property} = {Property:ValueBinder};")]
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "설정하기", Visible = true)]
    public class PSetter : PTargetable
    {
        public bool IsDirectValue => ValueBinder.IsDirectValue;

        public object Value => ValueBinder.DirectValue;

        public PParameterBinder ValueBinder { get; set; }

        public PObject DummyObject { get; set; }
        
        public PSetter() : base()
        {
            this.ClearReturnBinder();

            this.ValueBinder = this.AddParamterBinder("값", typeof(object));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == TargetTypeProperty)
            {
                if (this.TargetType == null)
                    return;

                // 더미가 이미 생성됬고 설정된 타입과 같은경우
                if (this.DummyObject != null && this.DummyObject.GetType() == this.TargetType)
                    return;
                
                this.DummyObject = Activator.CreateInstance(this.TargetType) as PObject;
            }
        }
    }
}
