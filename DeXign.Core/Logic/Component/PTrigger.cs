using System;
using System.Linq;
using System.Windows;
using System.Reflection;

using DeXign.Extension;

using WPFExtension;
using System.Collections.Generic;
using System.ComponentModel;

namespace DeXign.Core.Logic
{
    public class DXEventInfo
    {
        public Type DeclaringType { get; set; }

        public string Name { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventInfo RuntimeEventInfo => this.DeclaringType?.GetEvent(this.Name);
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DesignElementAttribute Attribute => this.RuntimeEventInfo?.GetAttribute<DesignElementAttribute>();

        public DXEventInfo()
        {
        }

        public DXEventInfo(EventInfo ei)
        {
            this.DeclaringType = ei.DeclaringType;
            this.Name = ei.Name;
        }

        public ParameterInfo[] GetParameters()
        {
            return this.RuntimeEventInfo
                .EventHandlerType
                .GetMethod("Invoke")
                .GetParameters();
        }
    }

    [CodeMap("{Target}.{EventName} += {EventCallback};")]
    [DesignElement(DisplayName = "이벤트", Visible = false)]
    public class PTrigger : PComponent
    {
        public static readonly DependencyProperty EventNameProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty EventInfoProperty =
            DependencyHelper.Register();

        private static readonly DependencyPropertyKey ParameterInfosPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty ParameterInfosProperty =
            ParameterInfosPropertyKey.DependencyProperty;

        public string EventName
        {
            get { return GetValue<string>(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }
        
        public DXEventInfo EventInfo
        {
            get
            {
                return GetValue<DXEventInfo>(EventInfoProperty);
            }
            set
            {
                SetValue(EventInfoProperty, value);
            }
        }

        public NamedParameterInfo[] ParameterInfos
        {
            get { return GetValue<NamedParameterInfo[]>(ParameterInfosProperty); }
            private set { SetValue(ParameterInfosPropertyKey, value); }
        }
        
        public PTrigger()
        {
            this.AddNewBinder(BindOptions.Input);
            this.AddNewBinder(BindOptions.Output);
        }

        public PTrigger(EventInfo eventInfo) : this()
        {
            SetRuntimeEvent(eventInfo);
        }

        public void SetRuntimeEvent(EventInfo eventInfo)
        {
            this.EventInfo = new DXEventInfo(eventInfo);
            Invalidate();
        }

        private void Invalidate()
        {
            // Get Event Parameteres
            ParameterInfo[] infos = this.EventInfo.GetParameters();
            
            var eInfos = new List<NamedParameterInfo>();
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == "sender")
                    eInfos.Add(new NamedParameterInfo(infos[i].Name, this.EventInfo.DeclaringType));
                else
                    eInfos.Add(new NamedParameterInfo(infos[i]));
            }

            this.ParameterInfos = eInfos.ToArray();
            
            // Display Name
            this.EventName = this.EventInfo.Name;

            this.EventName = this.EventInfo.Attribute.DisplayName;

            // Binder Generate
            this.ClearReturnBinder();

            (string Name, bool Visible)[] paramDatas = null;

            if (this.EventInfo.RuntimeEventInfo.HasAttribute<DesignDescriptionAttribute>())
            {
                var attr = this.EventInfo.RuntimeEventInfo.GetAttribute<DesignDescriptionAttribute>();

                paramDatas = DesignDescriptionDescriptor.GetParameterNames(attr).ToArray();
            }

            for (int i = 0; i < this.ParameterInfos.Length; i++)
            {
                if (paramDatas != null && !paramDatas[i].Visible)
                    continue;

                string name = this.ParameterInfos[i].Name;

                if (paramDatas != null)
                    name = paramDatas[i].Name;

                // Return 바인더 생성
                this.AddReturnBinder(name, this.ParameterInfos[i].ParameterType);
            }
        }
    }
}
