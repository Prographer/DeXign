using System;
using System.Linq;
using System.Windows;
using WPFExtension;

namespace DeXign.Core.Logic
{
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "설정하기", Visible = true)]
    public class PSetter : PComponent
    {
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register(nameof(Property), typeof(DependencyProperty), typeof(PSetter));

        public static readonly DependencyProperty TargetTypeProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register();

        public DependencyProperty Property
        {
            get { return GetValue<DependencyProperty>(PropertyProperty); }
            set { SetValue(Property, value); }
        }

        [ComponentParameter("대상", typeof(PObject), DisplayIndex = 0)]
        public Type TargetType
        {
            get { return GetValue<Type>(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

        [ComponentParameter("값", typeof(PObject), DisplayIndex = 1)]
        public object Value
        {
            get { return GetValue<object>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public PParameterBinder ValueParamBinder { get; }

        public PParameterBinder TargetBinder { get; }

        public PSetter() : base()
        {
            this.AddNewBinder(BindOptions.Input);
            this.AddNewBinder(BindOptions.Output);
            
            this.TargetBinder = this[BindOptions.Parameter].First() as PParameterBinder;
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
