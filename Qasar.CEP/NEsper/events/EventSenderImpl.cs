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
using com.espertech.esper.core;

using log4net;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Event sender for use with plug-in event representations.
    /// <para/>
    /// The implementation asks a list of event bean factories originating from plug-in event
    /// representations to each reflect on the event and generate an event bean. The first one
    /// to return an event bean wins.
    /// </summary>
    public class EventSenderImpl
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Creates the specified runtime event sender.
        /// </summary>
        /// <param name="handlingFactories">The handling factories.</param>
        /// <param name="epRuntime">The ep runtime.</param>
        /// <returns></returns>
        public static EventSender Create(IEnumerable<EventSenderURIDesc> handlingFactories, EPRuntimeEventSender epRuntime)
        {
            return delegate(Object _event) {
                       // Ask each factory in turn to take care of it
                       foreach (EventSenderURIDesc entry in handlingFactories) {
                           EventBean eventBean = null;

                           try {
                               eventBean = entry.BeanFactory.Invoke(_event, entry.ResolutionURI);
                           } catch (Exception ex) {
                               log.Warn(
                                   "Unexpected exception thrown by plug-in event bean factory '" + entry.BeanFactory +
                                   "' processing event " + _event,
                                   ex);
                           }

                           if (eventBean != null) {
                               epRuntime.ProcessWrappedEvent(eventBean);
                               return;
                           }
                       }
                   };
        }
    }
}
