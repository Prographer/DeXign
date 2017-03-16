using System;
using System.Linq;
using System.Windows;
using System.Reflection;

using DeXign.Extension;

using WPFExtension;
using System.Collections.Generic;

namespace DeXign.Core.Logic
{
    public class EventParameterInfo
    {
        public string Name { get; }
        public Type ParameterType { get; }

        public EventParameterInfo(string name, Type parameterType)
        {
            this.Name = name;
            this.ParameterType = parameterType;
        }

        public EventParameterInfo(ParameterInfo info)
        {
            this.Name = info.Name;
            this.ParameterType = info.ParameterType;
        }
    }

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

        public EventInfo EventInfo
        {
            get
            {
                return GetValue<EventInfo>(EventInfoProperty);
            }
            set
            {
                SetValue(EventInfoProperty, value);
                Invalidate();
            }
        }

        public EventParameterInfo[] ParameterInfos
        {
            get { return GetValue<EventParameterInfo[]>(ParameterInfosProperty); }
            private set { SetValue(ParameterInfosPropertyKey, value); }
        }

        public PTrigger()
        {
            this.AddNewBinder(BindOptions.Input);
            this.AddNewBinder(BindOptions.Output);
        }

        public PTrigger(EventInfo ei) : this()
        {
            this.EventInfo = ei;
        }

        private void Invalidate()
        {
            // Get Event Parameteres
            ParameterInfo[] infos = this.EventInfo
                .EventHandlerType
                .GetMethod("Invoke")
                .GetParameters();

            var eInfos = new List<EventParameterInfo>();
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == "sender")
                    eInfos.Add(new EventParameterInfo(infos[i].Name, this.EventInfo.DeclaringType));
                else
                    eInfos.Add(new EventParameterInfo(infos[i]));
            }

            this.ParameterInfos = eInfos.ToArray();


            // Display Name
            this.EventName = this.EventInfo.Name;

            if (this.EventInfo.HasAttribute<DesignElementAttribute>())
            {
                this.EventName = this.EventInfo.GetAttribute<DesignElementAttribute>().DisplayName;
            }

            // Binder Generate
            this.ClearReturnBinder();

            (string Name, bool Visible)[] paramDatas = null;

            if (this.EventInfo.HasAttribute<DesignDescriptionAttribute>())
            {
                var attr = this.EventInfo.GetAttribute<DesignDescriptionAttribute>();

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
