using System.Linq;
using System.Reflection;
using System.Windows;

using DeXign.SDK;
using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Logic
{

    [DesignElement(DisplayName = "함수", Visible = false)]
    public class PFunction : PComponent
    {
        public static readonly DependencyProperty FunctionNameProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty FunctionInfoProperty =
            DependencyHelper.Register();

        private static readonly DependencyPropertyKey ParameterInfosPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty ParameterInfosProperty =
            ParameterInfosPropertyKey.DependencyProperty;

        public string FunctionName
        {
            get { return GetValue<string>(FunctionNameProperty); }
            set { SetValue(FunctionNameProperty, value); }
        }

        public MethodInfo FunctionInfo
        {
            get
            {
                return GetValue<MethodInfo>(FunctionInfoProperty);
            }
            set
            {
                SetValue(FunctionInfoProperty, value);
                Invalidate();
            }
        }
        
        public PReturnBinder ReturnBinder { get; private set; }

        public NamedParameterInfo[] ParameterInfos
        {
            get { return GetValue<NamedParameterInfo[]>(ParameterInfosProperty); }
            private set { SetValue(ParameterInfosPropertyKey, value); }
        }

        public PFunction()
        {
            this.AddNewBinder(BindOptions.Input);
            this.AddNewBinder(BindOptions.Output);
        }

        public PFunction(MethodInfo mi) : this()
        {
            this.FunctionInfo = mi;
        }

        private void Invalidate()
        {
            this.ParameterInfos = this.FunctionInfo
                .GetParameters()
                .Select(pi => new NamedParameterInfo(pi))
                .ToArray();

            // Display Name
            if (this.FunctionInfo.HasAttribute<DXFunctionAttribute>())
            {
                this.FunctionName = this.FunctionInfo.GetAttribute<DXFunctionAttribute>().DisplayName;
            }

            // Binder Generate
            this.ClearReturnBinder();
            this.ClearParameterBinder();

            this.ReturnBinder = this.AddReturnBinder("결과", this.FunctionInfo.ReturnType);

            foreach (ParameterInfo pi in this.FunctionInfo.GetParameters())
            {
                string name = pi.Name;

                if (pi.HasAttribute<DXAttribute>())
                    name = pi.GetAttribute<DXAttribute>().DisplayName;

                // Return 바인더 생성
                this.AddParamterBinder(name, pi.ParameterType);
            }
        }
    }
}