using System;
using System.IO;

using com.espertech.esper.events;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Represents a simple property of a given name.
    /// </summary>
    public class SimpleProperty : PropertyBase
    {
        /// <summary> Ctor.</summary>
        /// <param name="propertyName">is the property name
        /// </param>
        public SimpleProperty(String propertyName)
            : base(propertyName)
        {
        }

        /// <summary>
        /// Returns value getter for the property of an event of the given event type.
        /// </summary>
        /// <param name="eventType">is the type of event to make a getter for</param>
        /// <returns>fast property value getter for property</returns>
        public override EventPropertyGetter GetGetter(BeanEventType eventType)
        {
            EventPropertyDescriptor propertyDesc = eventType.GetSimpleProperty(propertyNameAtomic);
            if (propertyDesc == null)
            {
                return null;
            }
            if (!propertyDesc.PropertyType.Equals(EventPropertyType.SIMPLE))
            {
                return null;
            }
            return eventType.GetGetter(propertyNameAtomic);
        }

        /// <summary>
        /// Returns the property type.
        /// </summary>
        /// <param name="eventType">the event type representing the bean</param>
        /// <returns>property type class</returns>
        public override Type GetPropertyType(BeanEventType eventType)
        {
            EventPropertyDescriptor propertyDesc = eventType.GetSimpleProperty(propertyNameAtomic);
            if (propertyDesc == null)
            {
                return null;
            }
            return eventType.GetPropertyType(propertyNameAtomic);
        }

        /// <summary>
        /// Gets the property type map.
        /// </summary>
        /// <returns></returns>
        public override Type GetPropertyTypeMap(DataMap optionalMapPropTypes)
        {
            // The simple, none-dynamic property needs a definition of the map contents else no property
            if (optionalMapPropTypes == null) {
                return null;
            }
            Object def = optionalMapPropTypes.Get(propertyNameAtomic);
            if (def == null) {
                return null;
            }
            if (def is Type) {
                return (Type) def;
            }
            if (def is DataMap) {
                return typeof (DataMap);
            }
            String message = "Nestable map type configuration encountered an unexpected value type of '"
                             + def.GetType() + " for property '" + propertyNameAtomic + "', expected Map or Class";
            throw new PropertyAccessException(message);
        }

        /// <summary>
        /// Gets the getter map.
        /// </summary>
        /// <returns></returns>
        public override EventPropertyGetter GetGetterMap(DataMap optionalMapPropTypes)
        {
            // The simple, none-dynamic property needs a definition of the map contents else no property
            if (optionalMapPropTypes == null) {
                return null;
            }
            Object def = optionalMapPropTypes.Get(propertyNameAtomic);
            if (def == null) {
                return null;
            }

            String propertyName = this.PropertyNameAtomic;
            return new ProxyEventPropertyGetter(
                delegate(EventBean eventBean) {
                    DataMap map = (DataMap) eventBean.Underlying;
                    return map.Get(propertyName);
                },
                delegate(EventBean eventBean) {
                    DataMap map = (DataMap) eventBean.Underlying;
                    return map.ContainsKey(propertyName);
                });
        }

        public override void ToPropertyEPL(StringWriter writer)
        {
            writer.Write(propertyNameAtomic);
        }
    }
}