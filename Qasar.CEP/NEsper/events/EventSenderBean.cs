///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.util;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Event sender for object events.
    /// <para/>
    /// Allows sending only event objects of the underlying type matching the event type,
    /// or implementing the interface or extending the type. Any other event object generates
    /// an error.
    /// </summary>
    public class EventSenderBean
    {
        /// <summary>
        /// Creates the specified runtime event sender.
        /// </summary>
        /// <param name="runtimeEventSender">The runtime event sender.</param>
        /// <param name="beanEventType">Type of the bean event.</param>
        /// <returns></returns>
        public static EventSender Create(EPRuntimeEventSender runtimeEventSender, BeanEventType beanEventType)
        {
            MonitorLock myLock = new MonitorLock();

            return delegate(Object _event) {
                       HashSet<Type> compatibleClasses = new HashSet<Type>();

                       Type eventType = _event.GetType();

                       // type check
                       if (eventType != beanEventType.UnderlyingType) {
                           using( myLock.Acquire()) {
                               if (!compatibleClasses.Contains(eventType)) {
                                   if (
                                       TypeHelper.IsSubclassOrImplementsInterface(eventType,
                                                                                  beanEventType.UnderlyingType)) {
                                       compatibleClasses.Add(eventType);
                                   } else {
                                       throw new EPException("Event object of type " + eventType.FullName +
                                                             " does not equal, extend or implement the type " +
                                                             beanEventType.UnderlyingType.FullName +
                                                             " of event type '" + beanEventType.Alias + "'");
                                   }
                               }
                           }
                       }

                       runtimeEventSender.ProcessWrappedEvent(new BeanEventBean(_event, beanEventType));
                   };
        }
    }
}
