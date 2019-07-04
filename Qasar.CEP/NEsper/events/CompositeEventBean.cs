using System;
using System.Collections.Generic;

namespace com.espertech.esper.events
{
	/// <summary>
	/// Event bean wrapper for events that consists of a Map of name tags as key values and
	/// event bean wrappers as value objects, for use by pattern expressions.
	/// </summary>
    public class CompositeEventBean : EventBean, TaggedCompositeEventBean
	{
		private readonly IDictionary<String, EventBean> wrappedEvents;
		private readonly EventType eventType;

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
        /// <returns> underlying data object, usually either a Map or a bean instance.
        /// </returns>
		virtual public Object Underlying
		{
			get { return wrappedEvents; }
		}

        /// <summary>
        /// Gets the <see cref="T:System.Object"/> with the specified property.
        /// </summary>
        /// <value></value>
        /// <returns> the value of a simple property with the specified name.
        /// </returns>
        /// <throws>  PropertyAccessException - if there is no property of the specified name, or the property cannot be accessed </throws>

		virtual public Object this[String property]
		{
			get
			{
				EventPropertyGetter getter = eventType.GetGetter( property );
				if ( getter == null )
				{
					throw new ArgumentException( "Property named '" + property + "' is not a valid property name for this type" );
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

		/// <summary> Ctor.</summary>
		/// <param name="wrappedEvents">is the event properties map with keys being the property name tages
		/// and values the wrapped event
		/// </param>
		/// <param name="eventType">is the event type instance for the wrapper
		/// </param>
		
		public CompositeEventBean( IDictionary<String, EventBean> wrappedEvents, EventType eventType )
		{
			this.wrappedEvents = wrappedEvents;
			this.eventType = eventType;
		}

        public virtual EventBean GetEventBean(String property)
        {
            EventBean eventBean;
            return wrappedEvents.TryGetValue(property, out eventBean) ? eventBean : null;
        }
	}
}
