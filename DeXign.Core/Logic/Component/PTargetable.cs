using System;
using System.Linq;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public abstract class PTargetable : PComponent
    {
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register(nameof(Property), typeof(DependencyProperty), typeof(PGetter));

        public static readonly DependencyProperty TargetTypeProperty =
            DependencyHelper.Register();

        public DependencyProperty Property
        {
            get { return GetValue<DependencyProperty>(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }
        
        public Type TargetType
        {
            get { return GetValue<Type>(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

        public PParameterBinder TargetBinder { get; set; }

        public PTargetable() : base()
        {
            this.AddNewBinder(BindOptions.Input);
            this.AddNewBinder(BindOptions.Output);

            this.AddReturnBinder("값", typeof(object));
            this.TargetBinder = this.AddParamterBinder("대상", typeof(PObject));

            this.TargetBinder.IsSingle = true;

            this.Released += PTargetable_Released;
        }

        private void PTargetable_Released(object sender, BinderBindedEventArgs e)
        {
            // 대상과 연결 끊어짐
            if (e.Expression.Input.Equals(this.TargetBinder))
            {
                this.TargetType = null;
            }
        }
    }
}
