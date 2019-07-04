///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.client;
using com.espertech.esper.core;
using com.espertech.esper.events;

namespace com.espertech.esper.plugin
{
    /// <summary>
    /// Provided once by an <see cref="PlugInEventRepresentation"/> for any event type it creates.
    /// </summary>
    public interface PlugInEventTypeHandler
    {
        /// <summary>Returns the event type. </summary>
        /// <returns>event type.</returns>
        EventType EventType { get; }

        /// <summary>Returns a facility responsible for converting or wrapping event objects. </summary>
        /// <param name="runtimeEventSender">for sending events into the engine</param>
        /// <returns>sender</returns>
        EventSender GetSender(EPRuntimeEventSender runtimeEventSender);
    }

    public delegate EventSender GetSenderDelegate(EPRuntimeEventSender runtimeEventSender);

    public class SimplePlugInEventTypeHandler : PlugInEventTypeHandler
    {
        private readonly GetSenderDelegate getSenderDelegate;
        private readonly EventType eventType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePlugInEventTypeHandler"/> class.
        /// </summary>
        /// <param name="getSenderDelegate">The get sender delegate.</param>
        /// <param name="eventType">Type of the event.</param>
        public SimplePlugInEventTypeHandler(GetSenderDelegate getSenderDelegate, EventType eventType)
        {
            this.getSenderDelegate = getSenderDelegate;
            this.eventType = eventType;
        }

        /// <summary>Returns a facility responsible for converting or wrapping event objects. </summary>
        /// <param name="runtimeEventSender">for sending events into the engine</param>
        /// <returns>sender</returns>
        public EventSender GetSender(EPRuntimeEventSender runtimeEventSender)
        {
            return getSenderDelegate.Invoke(runtimeEventSender);
        }

        /// <summary>
        /// Returns the event type.
        /// </summary>
        /// <value></value>
        /// <returns>event type.</returns>
        public EventType EventType
        {
            get { return eventType; }
        }
    }
}
