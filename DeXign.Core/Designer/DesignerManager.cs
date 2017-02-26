using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DeXign.Extension;

namespace DeXign.Core.Designer
{
    public static class DesignerManager
    {
        static AttributeTuple<DesignElementAttribute, Type>[] types;
        static Dictionary<Type, AttributeTuple<DesignElementAttribute, PropertyInfo>[]> props;
        static Dictionary<Type, AttributeTuple<DesignElementAttribute, EventInfo>[]> events;

        static DesignerManager()
        {
            // caching
            
            types = GetElementTypesCore().ToArray();

            props = types
                .Select(at => at.Element)
                .Where(t => t.HasAttribute<DesignElementAttribute>())
                .ToDictionary(
                    t => t,
                    t => GetPropertiesCore(t)
                        .Where(p => p.Attribute.Visible).ToArray());

            events = types
                .Select(at => at.Element)
                .Where(t => t.HasAttribute<DesignElementAttribute>())
                .ToDictionary(
                    t => t,
                    t => GetEventsCore(t)
                        .Where(e => e.Attribute.Visible).ToArray());
        }

        /// <summary>
        /// <see cref="DesignElementAttribute"/> Ư���� ���ǵ� ��� �� Ÿ���� �����ɴϴ�. 
        /// </summary>
        /// <param name="declareType">�� Ÿ��</param>
        /// <returns></returns>
        public static IEnumerable<AttributeTuple<DesignElementAttribute, Type>> GetElementTypes()
        {
            return types;
        }

        /// <summary>
        /// <see cref="DesignElementAttribute"/> Ư���� ���ǵ� ��� �Ӽ��� �����ɴϴ�. 
        /// </summary>
        /// <param name="declareType">�� Ÿ��</param>
        /// <returns></returns>
        public static IEnumerable<AttributeTuple<DesignElementAttribute, PropertyInfo>> GetProperties(Type declareType)
        {
            if (props.ContainsKey(declareType))
                return props[declareType];

            return GetPropertiesCore(declareType);
        }

        /// <summary>
        /// <see cref="DesignElementAttribute"/> Ư���� ���ǵ� ��� �̺�Ʈ�� �����ɴϴ�. 
        /// </summary>
        /// <param name="declareType">�� Ÿ��</param>
        /// <returns></returns>
        public static IEnumerable<AttributeTuple<DesignElementAttribute, EventInfo>> GetEvents(Type declareType)
        {
            if (events.ContainsKey(declareType))
                return events[declareType];

            return GetEventsCore(declareType);
        }

        private static IEnumerable<AttributeTuple<DesignElementAttribute, Type>> GetElementTypesCore()
        {
            return Assembly.GetAssembly(typeof(PObject))
                .GetTypes()
                .Where(t => t.HasAttribute<DesignElementAttribute>())
                .Select(t => new AttributeTuple<DesignElementAttribute, Type>(
                    t.GetAttribute<DesignElementAttribute>(), t));
        }

        private static IEnumerable<AttributeTuple<DesignElementAttribute, PropertyInfo>> GetPropertiesCore(Type declareType)
        {
            return declareType.GetProperties()
                .Where(pi => pi.HasAttribute<DesignElementAttribute>())
                .Select(pi => new AttributeTuple<DesignElementAttribute, PropertyInfo>(
                    pi.GetAttribute<DesignElementAttribute>(), pi));
        }

        private static IEnumerable<AttributeTuple<DesignElementAttribute, EventInfo>> GetEventsCore(Type declareType)
        {
            return declareType.GetEvents()
                .Where(ei => ei.HasAttribute<DesignElementAttribute>())
                .Select(ei => new AttributeTuple<DesignElementAttribute, EventInfo>(
                    ei.GetAttribute<DesignElementAttribute>(), ei));
        }
    }
}
