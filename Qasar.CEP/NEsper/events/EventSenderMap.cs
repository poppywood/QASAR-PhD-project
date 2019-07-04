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

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Event sender for map-backed events.
    /// <para/>
    /// Allows sending only event objects of type map, does not check map contents.
    /// Any other event object generates an error.
    /// </summary>
    public class EventSenderMap
    {
        /// <summary>
        /// Creates the specified runtime event sender.
        /// </summary>
        /// <param name="runtimeEventSender">The runtime event sender.</param>
        /// <param name="mapEventType">Type of the map event.</param>
        /// <returns></returns>
        public static EventSender Create(EPRuntimeEventSender runtimeEventSender, MapEventType mapEventType)
        {
            return delegate(Object _event) {
                       DataMap map = _event as DataMap;
                       if (map == null) {
                           throw new EPException("Unexpected event object of type " + _event.GetType().FullName +
                                                 ", expected " + typeof (DataMap).FullName);
                       }
                       MapEventBean mapEvent = new MapEventBean(map, mapEventType);
                       runtimeEventSender.ProcessWrappedEvent(mapEvent);
                   };
        }
    }
}
