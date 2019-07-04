using System;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Wrapper for regular objects the represent events.
    /// Allows access to event properties, which is done through the getter supplied by the event type.
    /// <see cref="EventType" /> instances containing type information are obtained from
    /// <see cref="BeanEventTypeFactory"/>.  Two BeanEventBean instances are equal if they have the
    /// same event type and refer to the same instance of event object.  Clients that need to compute
    /// equality between objects wrapped by this class need to obtain the underlying object.
    /// </summary>

    public class BeanEventBean : EventBean
    {
        /// <summary>
        /// Get the underlying data object to this event wrapper.
        /// </summary>
        /// <value></value>
        /// <returns> underlying data object, usually either a Map or a bean instance.
        /// </returns>
        virtual public Object Underlying
        {
            get { return _event; }
        }

        /// <summary>
        /// Return the <see cref="EventType"/> instance that describes the set of properties available for this event.
        /// </summary>
        /// <value></value>
        /// <returns> event type
        /// </returns>
        virtual public EventType EventType
        {
            get { return eventType; }
        }
	
        private readonly EventType eventType;
        private readonly Object _event;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_event">is the event object</param>
        /// <param name="eventType">is the schema information for the event object.</param>
		
        public BeanEventBean(Object _event, EventType eventType)
        {
            this.eventType = eventType;
            this._event = _event;
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
                    throw new PropertyAccessException("Property named '" + property + "' is not a valid property name for this type");
                }
             
                return getter.GetValue(this);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "BeanEventBean" + " eventType=" + eventType + " bean=" + _event;
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
    }
}