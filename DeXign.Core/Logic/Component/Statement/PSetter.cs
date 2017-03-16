using System.Windows;

namespace DeXign.Core.Logic
{
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "설정하기", Visible = true)]
    public class PSetter : PComponent
    {
        public static readonly DependencyProperty TargetPropertyProperty =
            DependencyProperty.Register(nameof(TargetProperty), typeof(DependencyProperty), typeof(PSetter));

        public static readonly DependencyProperty TaretProperty =
            DependencyProperty.Register(nameof(Target), typeof(PObject), typeof(PSetter));

        public DependencyProperty TargetProperty
        {
            get { return GetValue<DependencyProperty>(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        [ComponentParameter("대상", typeof(PObject))]
        public PObject Target
        {
            get { return GetValue<PObject>(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        private PParameterBinder paramValueBinder;

        public PSetter() : base()
        {
            this.AddNewBinder(BindOptions.Input);
            this.AddNewBinder(BindOptions.Output);

            paramValueBinder = this.AddParamterBinder("값", typeof(object));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == TargetPropertyProperty)
            {
                paramValueBinder.ParameterType = this.TargetProperty.PropertyType;
            }
        }
    }
}
