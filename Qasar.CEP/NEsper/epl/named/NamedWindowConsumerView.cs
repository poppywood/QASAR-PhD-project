///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.epl.expression;
using com.espertech.esper.events;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// Represents a consumer of a named window that selects from a named window via a from-clause.
    /// <para>
    /// The view simply dispatches directly to child views, and keeps the last new event for iteration.
    /// </para>
    /// </summary>
	public class NamedWindowConsumerView : ViewSupport, StatementStopCallback
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    private readonly IList<ExprNode> filterList;
	    private readonly EventType eventType;
	    private readonly NamedWindowTailView tailView;
	    private readonly EventBean[] eventPerStream = new EventBean[1];

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="filterList">is a list of filter expressions</param>
        /// <param name="eventType">the event type of the named window</param>
        /// <param name="statementStopService">for registering a callback when the view stopped, to unregister the statement as a consumer</param>
        /// <param name="tailView">to indicate when the consumer stopped to remove the consumer</param>
	    public NamedWindowConsumerView(IList<ExprNode> filterList,
	                                   EventType eventType,
	                                   StatementStopService statementStopService,
	                                   NamedWindowTailView tailView)
	    {
	        this.filterList = filterList;
	        this.eventType = eventType;
	        this.tailView = tailView;
	        statementStopService.AddSubscriber(this);
	    }

	    public override void Update(EventBean[] newData, EventBean[] oldData)
	    {
	        if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
	        {
	            log.Debug(".update Received update, " +
	                    "  newData.Length==" + ((newData == null) ? 0 : newData.Length) +
	                    "  oldData.Length==" + ((oldData == null) ? 0 : oldData.Length));
	        }

	        // if we have a filter for the named window,
	        if (filterList.Count != 0)
	        {
	            newData = PassFilter(newData, true);
	            oldData = PassFilter(oldData, false);
	        }

	        if ((newData != null) || (oldData != null))
	        {
	            UpdateChildren(newData, oldData);
	        }
	    }

	    private EventBean[] PassFilter(EventBean[] eventData, bool isNewData)
	    {
	        if ((eventData == null) || (eventData.Length == 0))
	        {
	            return null;
	        }

	        OneEventCollection filtered = null;
	        foreach (EventBean @event in eventData)
	        {
	            eventPerStream[0] = @event;
	            bool pass = true;
	            foreach (ExprNode filter in filterList)
	            {
	                bool? result = (bool?) filter.Evaluate(eventPerStream, isNewData);
                    if (!result ?? true)
	                {
	                    pass = false;
	                    break;
	                }
	            }

	            if (pass)
	            {
	                if (filtered == null)
	                {
                        filtered = new OneEventCollection();
	                }
	                filtered.Add(@event);
	            }
	        }

	        if (filtered == null)
	        {
	            return null;
	        }
	        return filtered.ToArray();
	    }

	    public override EventType EventType
	    {
            get { return eventType; }
	    }

	    public override IEnumerator<EventBean> GetEnumerator()
	    {
            if (( filterList == null ) || ( filterList.Count == 0 )) {
                foreach (EventBean eventBean in tailView) {
                    yield return eventBean;
                }
            } else {
                EventBean[] eventArray = new EventBean[1];
                foreach( EventBean eventBean in tailView ) {
                    bool isFiltered = true;
                    eventArray[0] = eventBean;
                    foreach( ExprNode filter in filterList ) {
                        bool? result = (bool?) filter.Evaluate(eventArray, true);
                        if ( !result ?? false ) {
                            // Event was filtered; end processing so that we can proceed
                            // to the next eventBean.
                            isFiltered = false;
                            break;
                        }
                    }
                    // Event was not filtered
                    if (isFiltered) {
                        yield return eventBean;
                    }
                }
            }
        }

	    public void StatementStopped()
	    {
	        tailView.RemoveConsumer(this);
	    }
	}
} // End of namespace
