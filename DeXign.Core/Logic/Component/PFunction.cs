using System.Linq;
using System.Reflection;
using System.Windows;

using DeXign.SDK;
using DeXign.Extension;

using WPFExtension;
using System;
using System.ComponentModel;

namespace DeXign.Core.Logic
{
    public class DXMethodInfo
    {
        public Type DeclaringType { get; set; }
        
        public string Name { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MethodInfo RuntimeMethodInfo => this.DeclaringType?.GetMethod(this.Name);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type ReturnType => this.RuntimeMethodInfo?.ReturnType;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DXAttribute Attribute => this.RuntimeMethodInfo?.GetAttribute<DXAttribute>();

        public DXMethodInfo()
        {
        }

        public DXMethodInfo(MethodInfo mi)
        {
            this.DeclaringType = mi.DeclaringType;
            this.Name = mi.Name;
        }

        public ParameterInfo[] GetParameters()
        {
            return this.RuntimeMethodInfo.GetParameters();
        }
    }


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

        public DXMethodInfo FunctionInfo
        {
            get
            {
                return GetValue<DXMethodInfo>(FunctionInfoProperty);
            }
            set
            {
                SetValue(FunctionInfoProperty, value);
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
            SetRuntimeFunction(mi);
        }

        public void SetRuntimeFunction(MethodInfo mi)
        {
            this.FunctionInfo = new DXMethodInfo(mi);
            Invalidate();
        }

        private void Invalidate()
        {
            this.ParameterInfos = this.FunctionInfo
                .GetParameters()
                .Select(pi => new NamedParameterInfo(pi))
                .ToArray();
            
            // Display Name
            this.FunctionName = this.FunctionInfo.Attribute.DisplayName;

            // Binder Generate
            this.ClearReturnBinder();
            this.ClearParameterBinder();

            this.ReturnBinder = this.AddReturnBinder("결과", this.FunctionInfo.ReturnType);

            foreach (ParameterInfo pi in this.FunctionInfo.GetParameters())
            {
                string name = pi.Name;

                if (pi.HasAttribute<DXAttribute>())
                    name = pi.GetAttribute<DXAttribute>().DisplayName;

                // 파라미터 바인더 생성
                this.AddParamterBinder(name, pi.ParameterType);
            }
        }
    }
}