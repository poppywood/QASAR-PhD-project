using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Getter for one or more levels deep nested properties.
    /// </summary>

    public class NestedPropertyGetter : EventPropertyGetter
    {
        private readonly EventPropertyGetter[] getterChain;
        private readonly BeanEventTypeFactory beanEventTypeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NestedPropertyGetter"/> class.
        /// </summary>
        /// <param name="getterChain">the chain of getters to retrieve each nested property.</param>
        /// <param name="beanEventTypeFactory">the cache and factory for event bean types and event wrappers.</param>
 
        public NestedPropertyGetter(IList<EventPropertyGetter> getterChain, BeanEventTypeFactory beanEventTypeFactory)
        {
            this.getterChain = CollectionHelper.ToArray(getterChain);
            this.beanEventTypeFactory = beanEventTypeFactory;
        }

        /// <summary>
        /// Return the value for the property in the event object specified when the instance was obtained.
        /// Useful for fast access to event properties. Throws a PropertyAccessException if the getter instance
        /// doesn't match the EventType it was obtained from, and to indicate other property access problems.
        /// </summary>
        /// <param name="eventBean">is the event to get the value of a property from</param>
        /// <returns>value of property in event</returns>
        /// <throws>  PropertyAccessException to indicate that property access failed </throws>
        public virtual Object GetValue(EventBean eventBean)
        {
            Object value = null;

            for (int i = 0; i < getterChain.Length; i++)
            {
                value = getterChain[i].GetValue(eventBean);

                if (value == null)
                {
                    return null;
                }

                if (i < (getterChain.Length - 1))
                {
                    EventType type = beanEventTypeFactory.CreateBeanType(value.GetType().FullName, value.GetType());
                    eventBean = new BeanEventBean(value, type);
                }
            }
            return value;
        }

        /// <summary>
        /// Returns true if the property exists, or false if the type does not have such a property.
        /// <para>
        /// Useful for dynamic properties of the syntax "property?" and the dynamic nested/indexed/mapped versions.
        /// Dynamic nested properties follow the syntax "property?.nested" which is equivalent to "property?.nested?".
        /// If any of the properties in the path of a dynamic nested property return null, the dynamic nested property
        /// does not exists and the method returns false.
        /// </para>
        /// 	<para>
        /// For non-dynamic properties, this method always returns true since a getter would not be available
        /// unless
        /// </para>
        /// </summary>
        /// <param name="eventBean">the event to check if the dynamic property exists</param>
        /// <returns>
        /// indictor whether the property exists, always true for non-dynamic (default) properties
        /// </returns>
        public bool IsExistsProperty(EventBean eventBean)
        {
            int lastElementIndex = getterChain.Length - 1;

            // walk the getter chain up to the previous-to-last element, returning its object value.
            // any null values in between mean the property does not exists
            for (int i = 0; i < getterChain.Length - 1; i++)
            {
                Object value = getterChain[i].GetValue(eventBean);

                if (value == null)
                {
                    return false;
                }
                else
                {
                    EventType type = beanEventTypeFactory.CreateBeanType(value.GetType().FullName, value.GetType());
                    eventBean = new BeanEventBean(value, type);
                }
            }

            return getterChain[lastElementIndex].IsExistsProperty(eventBean);
        }
    }
}