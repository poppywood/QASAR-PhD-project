using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.filter
{
    /// <summary>
    /// Mapping of event type to a tree-like structure
    /// containing filter parameter constants in indexes <see cref="FilterParamIndexBase" /> and filter callbacks in <see cref="FilterHandleSetNode"/>.
    /// <para>
    /// This class evaluates events for the purpose of filtering by (1) looking up the event's <see cref="EventType" />
    /// and (2) asking the subtree for this event type to evaluate the event.
    /// </para>
    /// <para>
    /// The class performs all the locking required for multithreaded access.
    /// </para>
    /// </summary>

    public class EventTypeIndex : EventEvaluator
    {
        private readonly IDictionary<EventType, FilterHandleSetNode> eventTypes;
        private readonly FastReaderWriterLock eventTypesRWLock;

	    /// <summary>Returns the current size of the known event types.</summary>
	    /// <returns>collection size</returns>
	    protected int Count
	    {
	        get { return eventTypes.Count ; }
	    }

		/// <summary>
        /// Constructor
        /// </summary>

        public EventTypeIndex()
        {
            eventTypes = new Dictionary<EventType, FilterHandleSetNode>();
            eventTypesRWLock = new FastReaderWriterLock();
        }

        /// <summary>
        /// Destroy the service.
        /// </summary>
        public void Destroy()
        {
            eventTypes.Clear();
        }

        /// <summary>
        /// Add a new event type to the index and use the specified node for the root node of its subtree.
        /// If the event type already existed, the method will throw an IllegalStateException.
        /// </summary>
        /// <param name="eventType">the event type to be added to the index</param>
        /// <param name="rootNode">the root node of the subtree for filter constant indizes and callbacks</param>

        public void Add(EventType eventType, FilterHandleSetNode rootNode)
        {
            using (new WriterLock(eventTypesRWLock))
            {
                if (eventTypes.ContainsKey(eventType))
                {
                    throw new IllegalStateException("Event type already in index, add not performed, type=" + eventType);
                }
                eventTypes[eventType] = rootNode;
            }
        }

        /// <summary>Returns the root node for the given event type, or null if this event type has not been seen before.</summary>
        /// <param name="eventType">is an event type</param>
        /// <returns>the subtree's root node</returns>

        public FilterHandleSetNode this[EventType eventType]
        {
        	get
        	{
                using( new ReaderLock( eventTypesRWLock ))
                {
                    FilterHandleSetNode result;
                    eventTypes.TryGetValue(eventType, out result);
                    return result;
                }
        	}
        }

        /// <summary>
        /// Matches the event.
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <param name="matches">The matches.</param>
        public void MatchEvent(EventBean ev, IList<FilterHandle> matches)
        {
            if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
            {
                log.Debug(".MatchEvent Event received for matching, event=" + ev);
            }

            EventType eventType = ev.EventType;

            // Attempt to match exact type
            MatchType(eventType, ev, matches);

            // No supertype means we are done
            if (eventType.SuperTypes == null)
            {
                return;
            }

			foreach( EventType superType in eventType.DeepSuperTypes )
            {
                MatchType(superType, ev, matches);
            }
        }

        private void MatchType(EventType eventType, EventBean eventBean, IList<FilterHandle> matches)
        {
            FilterHandleSetNode rootNode;

            using( new ReaderLock(eventTypesRWLock))
            {
                eventTypes.TryGetValue(eventType, out rootNode);
            }

            // If the top class node is null, no filters have yet been registered for this event type.
            // In this case, log a message and done.
            if (rootNode == null)
            {
                if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
                {
                    String message = "Event type is not known to the filter service, eventType=" + eventType;
                    log.Debug(".MatchEvent " + message);
                }
                return;
            }

            rootNode.MatchEvent(eventBean, matches);
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
