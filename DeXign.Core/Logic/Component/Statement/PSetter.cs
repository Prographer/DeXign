using System.Linq;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "설정하기", Visible = true)]
    public class PSetter : PTargetable
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register();
        
        [ComponentParameter("값", typeof(PObject), DisplayIndex = 1)]
        public object Value
        {
            get { return GetValue<object>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public PParameterBinder ValueParamBinder { get; }
        
        public PSetter() : base()
        {
            this.ClearReturnBinder();
            this.ValueParamBinder = this[BindOptions.Parameter].Skip(1).First() as PParameterBinder;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == TargetTypeProperty)
            {
                ValueParamBinder.ParameterType = this.TargetType;
            }
        }
    }
}
