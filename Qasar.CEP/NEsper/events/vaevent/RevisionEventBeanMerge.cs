///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.collection;
using com.espertech.esper.events;
using com.espertech.esper.util;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>Merge-event for event revisions. </summary>
    public class RevisionEventBeanMerge : EventBean
    {
        private readonly RevisionEventType revisionEventType;
        private readonly EventBean underlyingFullOrDelta;
    
        private NullableObject<Object>[] overlay;
        private EventBean lastBaseEvent;
        private MultiKeyUntyped key;
        private bool latest;
    
        /// <summary>Ctor. </summary>
        /// <param name="revisionEventType">type</param>
        /// <param name="underlyingFull">event wrapped</param>
        public RevisionEventBeanMerge(RevisionEventType revisionEventType, EventBean underlyingFull)
        {
            this.revisionEventType = revisionEventType;
            this.underlyingFullOrDelta = underlyingFull;
        }

        /// <summary>Gets or sets the flag that indicates latest or not. </summary>
        /// <returns>latest flag</returns>
        public bool IsLatest
        {
            get { return latest; }
            set { this.latest = value; }
        }

        /// <summary>Gets or sets the key. </summary>
        /// <returns>key</returns>
        public MultiKeyUntyped Key
        {
            get { return key; }
            set { this.key = value; }
        }

        /// <summary>Gets or sets overlay values. </summary>
        /// <returns>overlay</returns>
        public NullableObject<Object>[] Overlay
        {
            get { return overlay; }
            set { this.overlay = value; }
        }

        /// <summary>Gets or sets last base event. </summary>
        /// <returns>base event</returns>
        public EventBean LastBaseEvent
        {
            get { return lastBaseEvent; }
            set { this.lastBaseEvent = value; }
        }

        /// <summary>
        /// Return the <see cref="EventType"/> instance that describes the set of properties
        /// available for this event.
        /// </summary>
        /// <value></value>
        /// <returns> event type
        /// </returns>
        public EventType EventType
        {
            get { return revisionEventType; }
        }

        /// <summary>
        /// Returns the value of an event property.
        /// </summary>
        /// <value></value>
        /// <returns> the value of a simple property with the specified name.
        /// </returns>
        /// <throws>  PropertyAccessException - if there is no property of the specified name, or the property cannot be accessed </throws>
        public object this[string property]
        {
            get
            {
                EventPropertyGetter getter = revisionEventType.GetGetter(property);
                if (getter == null) {
                    return null;
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
        public Object Get(String property)
        {
            return this[property];
        }

        public object Underlying
        {
            get { return typeof (RevisionEventBeanMerge); }
        }

        /// <summary>Returns wrapped event </summary>
        /// <returns>event</returns>
        public EventBean UnderlyingFullOrDelta
        {
            get { return underlyingFullOrDelta; }
        }

        /// <summary>Returns a value from the key. </summary>
        /// <param name="index">within key</param>
        /// <returns>value</returns>
        public Object GetKeyValue(int index)
        {
            return key.Keys[index];
        }
    
        /// <summary>Returns base event value. </summary>
        /// <param name="paramList">supplies getter</param>
        /// <returns>value</returns>
        public Object GetBaseEventValue(RevisionGetterParameters paramList)
        {
            return paramList.BaseGetter.GetValue(lastBaseEvent);
        }
    
        /// <summary>Returns a versioned value. </summary>
        /// <param name="paramList">getter and indexes</param>
        /// <returns>value</returns>
        public Object GetVersionedValue(RevisionGetterParameters paramList)
        {
            int propertyNumber = paramList.PropertyNumber;
    
            if (overlay != null)
            {
                NullableObject<Object> value = overlay[propertyNumber];
                if (value != null)
                {
                    return value.Value;
                }
            }

            EventPropertyGetter getter = paramList.BaseGetter;
            if (getter == null)
            {
                return null;  // The property was added by a delta event and only exists on a delta
            }
            return getter.GetValue(lastBaseEvent);
        }
    }
}
