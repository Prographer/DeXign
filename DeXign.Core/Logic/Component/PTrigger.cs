using System.Windows;
using System.Reflection;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Logic.Component
{
    [DesignElement(DisplayName = "이벤트", Visible = false)]
    public class PTrigger : PComponent
    {
        public static readonly DependencyProperty EventInfoProperty =
            DependencyHelper.Register();

        private static readonly DependencyPropertyKey ParameterInfosPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty ParameterInfosProperty =
            ParameterInfosPropertyKey.DependencyProperty;

        public EventInfo EventInfo
        {
            get
            {
                return GetValue<EventInfo>(EventInfoProperty);
            }
            set
            {
                // Get Event Parameteres
                this.ParameterInfos = value
                    .EventHandlerType
                    .GetMethod("Invoke")
                    .GetParameters();

                SetValue(EventInfoProperty, value);
                InvalidateTitle();
            }
        }
        
        public ParameterInfo[] ParameterInfos
        {
            get { return GetValue< ParameterInfo[]>(ParameterInfosProperty); }
            private set { SetValue(ParameterInfosPropertyKey, value); }
        }

        public PTrigger()
        {
        }

        public PTrigger(EventInfo ei)
        {
            this.EventInfo = ei;
        }

        private void InvalidateTitle()
        {
            this.Title = this.EventInfo.Name;
            
            if (this.EventInfo.HasAttribute<DesignElementAttribute>())
            {
                this.Title = this.EventInfo.GetAttribute<DesignElementAttribute>().DisplayName;
            }
        }
    }
}
