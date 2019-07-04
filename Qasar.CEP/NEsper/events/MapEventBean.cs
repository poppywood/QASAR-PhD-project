using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using com.espertech.esper.compat;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Wrapper for events represented by a Map of key-value pairs that are the event properties.
    /// MapEventBean instances are equal if they have the same <see cref="EventType" /> and all property names
    /// and values are reference-equal. 
    /// </summary>

    public class MapEventBean : EventBean
    {
        private readonly EventType eventType;
        private readonly DataMap properties;

        /// <summary>
        /// Return the <see cref="EventType" /> instance that describes the set of properties available for this event.
        /// </summary>
        /// <value></value>
        /// <returns> event type
        /// </returns>
        virtual public EventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Get the underlying data object to this event wrapper.
        /// </summary>
        /// <value></value>
        /// <returns> underlying data object.
        /// </returns>
        virtual public Object Underlying
        {
            get { return properties; }
        }

        /// <summary>
        /// Constructor for initialization with existing values.
        /// Makes a shallow copy of the supplied values to not be surprised by changing property values.
        /// </summary>
        /// <param name="properties">are the event property values</param>
        /// <param name="eventType">is the type of the event, i.e. describes the map entries</param>

        public MapEventBean(IDictionary<string,object> properties, EventType eventType)
        {
        	this.properties = new HashMap<string,object>() ;
            this.properties.PutAll(properties);
            this.eventType = eventType;
        }

        /// <summary>
        /// Constructor for initialization with existing values.
        /// Makes a shallow copy of the supplied values to not be surprised by changing property values.
        /// </summary>
        /// <param name="eventType">is the type of the event, i.e. describes the map entries</param>
        /// <param name="events">are the event property constisting of events</param>

        public MapEventBean(EventType eventType, IEnumerable<KeyValuePair<string, EventBean>> events)
        {
            this.properties = new HashMap<string,object>();

            foreach (KeyValuePair<String, EventBean> entry in events)
            {
                properties[entry.Key] = entry.Value.Underlying;
            }

            this.eventType = eventType;
        }

        /// <summary>
        /// Constructor for the mutable functions, e.g. only the type of values is known but not the actual values.
        /// </summary>
        /// <param name="eventType">is the type of the event, i.e. describes the map entries</param>

        public MapEventBean(EventType eventType)
        {
            this.properties = new HashMap<string,object>();
            this.eventType = eventType;
        }

        /// <summary>
        /// Returns the value of an event property.
        /// </summary>
        /// <value></value>
        /// <returns> the value of a simple property with the specified name.
        /// </returns>
        /// <throws>  PropertyAccessException - if there is no property of the specified name, or the property cannot be accessed </throws>
        public virtual Object this[String property]
        {
            get
            {
                EventPropertyGetter getter = eventType.GetGetter(property);
                if (getter == null)
                {
                    throw new ArgumentException("Property named '" + property + "' is not a valid property name for this type");
                }
             
                return getter.GetValue(this);
            }
        }

        /// <summary>
        /// Returns the value of an event property.  This method is a proxy of the indexer.
        /// </summary>
        /// <param name="property">name of the property whose value is to be retrieved</param>
        /// <returns>
        /// the value of a simple property with the specified name.
        /// </returns>
        /// <throws>  PropertyAccessException - if there is no property of the specified name, or the property cannot be accessed </throws>
        public virtual Object Get(String property)
        {
            return this[property];
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "MapEventBean " + "eventType=" + eventType;
        }
    }
}
