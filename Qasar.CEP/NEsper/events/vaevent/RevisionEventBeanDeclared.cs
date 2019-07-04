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

namespace com.espertech.esper.events.vaevent
{
    /// <summary>Revision event bean for the overlayed scheme. </summary>
    public class RevisionEventBeanDeclared : EventBean
    {
        private readonly RevisionEventType revisionEventType;
        private readonly EventBean underlyingFullOrDelta;
    
        private MultiKeyUntyped key;
        private EventBean lastBaseEvent;
        private RevisionBeanHolder[] holders;
        private bool isLatest;
    
        /// <summary>Ctor. </summary>
        /// <param name="eventType">revision event type</param>
        /// <param name="underlying">event wrapped</param>
        public RevisionEventBeanDeclared(RevisionEventType eventType, EventBean underlying)
        {
            this.revisionEventType = eventType;
            this.underlyingFullOrDelta = underlying;
        }
    
        /// <summary>Is true if latest event, or false if not. </summary>
        /// <returns>indicator if latest</returns>
        public bool IsLatest
        {
            get { return isLatest; }
            set { isLatest = value; }
        }

        /// <summary>
        /// Sets versions.
        /// </summary>
        /// <value>The holders.</value>
        public RevisionBeanHolder[] Holders
        {
            set { this.holders = value; }
        }

        /// <summary>Gets or sets the last base event. </summary>
        /// <returns>base event</returns>
        public EventBean LastBaseEvent
        {
            get { return lastBaseEvent; }
            set { this.lastBaseEvent = value; }
        }

        /// <summary>Returns wrapped event. </summary>
        /// <returns>wrapped event</returns>
        public EventBean UnderlyingFullOrDelta
        {
            get { return underlyingFullOrDelta; }
        }

        /// <summary>Returns the key. </summary>
        /// <returns>key</returns>
        public MultiKeyUntyped Key
        {
            get { return key; }
            set { this.key = value; }
        }

        /// <summary>Returns the revision event type. </summary>
        /// <returns>type</returns>
        public RevisionEventType RevisionEventType
        {
            get { return revisionEventType; }
        }

        public EventType EventType
        {
            get { return revisionEventType; }
        }

        public Object this[String property]
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
        public object Get(string property)
        {
            return this[property];
        }

        public object Underlying
        {
            get { return typeof (RevisionEventBeanDeclared); }
        }

        /// <summary>Returns a versioned value. </summary>
        /// <param name="paramList">getter parameters</param>
        /// <returns>value</returns>
        public Object GetVersionedValue(RevisionGetterParameters paramList)
        {
            RevisionBeanHolder holderMostRecent = null;
            
            if (holders != null)
            {
                foreach (int numSet in paramList.PropertyGroups)
                {
                    RevisionBeanHolder holder = holders[numSet];
                    if (holder != null)
                    {
                        if (holderMostRecent == null)
                        {
                            holderMostRecent = holder;
                        }
                        else
                        {
                            if (holder.Version > holderMostRecent.Version)
                            {
                                holderMostRecent = holder;
                            }
                        }
                    }
                }
            }
    
            // none found, use last full event
            if (holderMostRecent == null)
            {
                return paramList.BaseGetter.GetValue(lastBaseEvent);
            }

            return holderMostRecent.GetValueForProperty(paramList.PropertyNumber);
        }
    }
}
