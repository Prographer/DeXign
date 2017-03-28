using System;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public class PComponent : PBinderHost
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty DescriptionProperty =
            DependencyHelper.Register();

        public string Title
        {
            get { return GetValue<string>(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public new string Description
        {
            get { return GetValue<string>(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public PComponent() : base()
        {
        }

        public PTargetBinder AddTargetBinder(string name, Type assignableType)
        {
            PTargetBinder binder;

            AddBinder(binder = new PTargetBinder(this)
            {
                Title = name,
                ParameterType = assignableType
            });

            return binder;
        }

        public PParameterBinder AddParamterBinder(string name, Type assignableType)
        {
            PParameterBinder binder;

            AddBinder(binder = new PParameterBinder(this)
            {
                Title = name,
                ParameterType = assignableType
            });

            return binder;
        }

        public PReturnBinder AddReturnBinder(string name, Type returnType)
        {
            PReturnBinder binder;

            AddBinder(binder = new PReturnBinder(this, returnType)
            {
                Title = name
            });

            return binder;
        }

        public void ClearParameterBinder()
        {
            this.ClearBinder(BindOptions.Parameter);
        }

        public void ClearReturnBinder()
        {
            this.ClearBinder(BindOptions.Return);
        }
    }
}