using System;
using System.Reflection;

namespace DeXign.Core.Logic.Component
{
    // Event
    public class PTrigger : PComponent
    {
        public Type DeclareType { get; }
        public EventInfo EventInfo { get; }
        public ParameterInfo[] ParameterInfos { get; }

        public PTrigger(Type declareType, EventInfo ei)
        {
            this.DeclareType = declareType;
            this.EventInfo = ei;
            this.ParameterInfos = EventInfo.EventHandlerType.GetMethod("Invoke").GetParameters();
        }
    }
}
