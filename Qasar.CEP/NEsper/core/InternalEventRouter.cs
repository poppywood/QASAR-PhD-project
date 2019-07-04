using System;

using EventBean = com.espertech.esper.events.EventBean;

namespace com.espertech.esper.core
{
	/// <summary>
    /// Interface for a service that routes events within the engine for further processing.
    /// </summary>

    public interface InternalEventRouter
	{
        /// <summary>Route the event such that the event is processed as required.</summary>
        /// <param name="event">event to route</param>
        /// <param name="statementHandle">provides statement resources</param>
        void Route(EventBean @event, EPStatementHandle statementHandle);
	}
}
