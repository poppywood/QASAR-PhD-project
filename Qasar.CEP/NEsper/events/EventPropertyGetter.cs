using System;

namespace com.espertech.esper.events
{
	/// <summary> Get property values from an event instance for a given event property.
	/// Instances that implement this interface are usually bound to a particular <see cref="EventType" /> and cannot
	/// be used to access <see cref="EventBean" /> instances of a different type.
	/// </summary>

    public interface EventPropertyGetter
    {
        /// <summary> Return the value for the property in the event object specified when the instance was obtained.
        /// Useful for fast access to event properties. Throws a PropertyAccessException if the getter instance
        /// doesn't match the EventType it was obtained from, and to indicate other property access problems.
        /// </summary>
        /// <param name="eventBean">is the event to get the value of a property from
        /// </param>
        /// <returns> value of property in event
        /// </returns>
        /// <throws>  PropertyAccessException to indicate that property access failed </throws>

        Object GetValue(EventBean eventBean) ;

        /// <summary>
        /// Returns true if the property exists, or false if the type does not have such a property.
        /// <para>
        /// Useful for dynamic properties of the syntax "property?" and the dynamic nested/indexed/mapped versions.
        /// Dynamic nested properties follow the syntax "property?.nested" which is equivalent to "property?.nested?".
        /// If any of the properties in the path of a dynamic nested property return null, the dynamic nested property
        /// does not exists and the method returns false.
        /// </para>
        /// <para>
        /// For non-dynamic properties, this method always returns true since a getter would not be available
        /// unless
        /// </para>
        /// </summary>
        /// <param name="eventBean">the event to check if the dynamic property exists</param>
        /// <returns>
        /// indictor whether the property exists, always true for non-dynamic (default) properties
        /// </returns>
        ///
        bool IsExistsProperty(EventBean eventBean);

    }

    /// <summary>
    /// A delegate wrapper for the event property getter
    /// </summary>
    /// <param name="eventBean"></param>
    /// <returns></returns>
    public delegate Object EventPropertyGetterDelegate(EventBean eventBean);

    /// <summary>
    /// A delegate wrapper for the event property tester
    /// </summary>
    /// <param name="eventBean"></param>
    /// <returns></returns>

    public delegate bool EventPropertyTesterDelegate(EventBean eventBean);

    /// <summary>
    /// An interface that wraps the the event property getter with a delegate
    /// </summary>

    public sealed class ProxyEventPropertyGetter : EventPropertyGetter
    {
        private readonly EventPropertyGetterDelegate getterDelegate;
        private readonly EventPropertyTesterDelegate testerDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyEventPropertyGetter"/> class.
        /// </summary>
        /// <param name="getterDelegate">The getter delegate.</param>
        /// <param name="testerDelegate">The tester delegate.</param>
        public ProxyEventPropertyGetter(
            EventPropertyGetterDelegate getterDelegate,
            EventPropertyTesterDelegate testerDelegate )
        {
            this.getterDelegate = getterDelegate;
            this.testerDelegate = testerDelegate;
        }

        /// <summary>
        /// Return the value for the property in the event object specified when the instance was obtained.
        /// Useful for fast access to event properties. Throws a PropertyAccessException if the getter instance
        /// doesn't match the EventType it was obtained from, and to indicate other property access problems.
        /// </summary>
        /// <param name="eventBean">is the event to get the value of a property from</param>
        /// <returns>value of property in event</returns>
        /// <throws>  PropertyAccessException to indicate that property access failed </throws>
        public Object GetValue(EventBean eventBean)
        {
            return this.getterDelegate(eventBean);
        }

        /// <summary>
        /// Returns true if the property exists, or false if the type does not have such a property.
        /// <para>
        /// Useful for dynamic properties of the syntax "property?" and the dynamic nested/indexed/mapped versions.
        /// Dynamic nested properties follow the syntax "property?.nested" which is equivalent to "property?.nested?".
        /// If any of the properties in the path of a dynamic nested property return null, the dynamic nested property
        /// does not exists and the method returns false.
        /// </para>
        /// <para>
        /// For non-dynamic properties, this method always returns true since a getter would not be available
        /// unless
        /// </para>
        /// </summary>
        /// <param name="eventBean">the event to check if the dynamic property exists</param>
        /// <returns>
        /// indictor whether the property exists, always true for non-dynamic (default) properties
        /// </returns>
        ///
        public bool IsExistsProperty(EventBean eventBean)
        {
            return this.testerDelegate(eventBean);
        }
    }
}