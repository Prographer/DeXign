using System.Reflection;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic.Component
{
    [DesignElement(DisplayName = "이벤트", Visible = false)]
    public class PTrigger : PComponent
    {
        public static readonly DependencyProperty EventInfoProperty =
            DependencyHelper.Register();

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
            }
        }
        
        public ParameterInfo[] ParameterInfos { get; private set; }

        public PTrigger()
        {
        }

        public PTrigger(EventInfo ei)
        {
            this.EventInfo = ei;
        }
    }
}
