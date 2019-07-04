///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.events
{
    /// <summary>An event that is carries multiple representations of event properties: A synthetic representation that is designed for delivery as <see cref="EventBean"/> to client <see cref="com.espertech.esper.client.UpdateListener"/> code, and a natural representation as a bunch of Object-type properties for fast delivery to client subscriber objects via method call. </summary>
    public class NaturalEventBean : EventBean
    {
        private readonly EventType eventBeanType;
        private readonly Object[] natural;
        private readonly EventBean optionalSynthetic;
    
        /// <summary>Ctor. </summary>
        /// <param name="eventBeanType">the event type of the synthetic event</param>
        /// <param name="natural">the properties of the event</param>
        /// <param name="optionalSynthetic">the event bean that is the synthetic event, or null if no synthetic is packed in</param>
        public NaturalEventBean(EventType eventBeanType, Object[] natural, EventBean optionalSynthetic) {
            this.eventBeanType = eventBeanType;
            this.natural = natural;
            this.optionalSynthetic = optionalSynthetic;
        }

        /// <summary> Return the <see cref="EventType" /> instance that describes the set of properties available for this event.</summary>
        /// <returns> event type
        /// </returns>
        public EventType EventType
        {
            get { return eventBeanType; }
        }

        /// <summary>
        /// Returns the value of an event property.
        /// </summary>
        /// <value></value>
        /// <returns> the value of a simple property with the specified name.
        /// </returns>
        /// <throws>  PropertyAccessException - if there is no property of the specified name, or the property cannot be accessed </throws>
        public Object this[String property]
        {
            get
            {
                if (optionalSynthetic != null) {
                    return optionalSynthetic.Get(property);
                }
                throw new PropertyAccessException(
                    "Property access not allowed for natural events without the synthetic event present");
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
        public object Get(string property)
        {
            return this[property];
        }

        /// <summary> Get the underlying data object to this event wrapper.</summary>
        /// <returns> underlying data object, usually either a Map or a bean instance.
        /// </returns>
        public object Underlying
        {
            get { return typeof (Object[]); }
        }

        /// <summary>Returns the column object result representation. </summary>
        /// <returns>select column values</returns>
        public Object[] Natural
        {
            get { return natural; }
        }

        /// <summary>Returns the synthetic event that can be attached. </summary>
        /// <returns>synthetic if attached, or null if none attached</returns>
        public EventBean OptionalSynthetic
        {
            get { return optionalSynthetic; }
        }
    }
}
