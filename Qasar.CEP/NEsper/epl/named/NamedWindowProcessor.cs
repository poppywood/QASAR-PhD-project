///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// An instance of this class is associated with a specific named window. The processor provides 
    /// the views to create-window, on-delete statements and statements selecting from a named window.
    /// </summary>
    public class NamedWindowProcessor
    {
        private readonly NamedWindowTailView tailView;
        private readonly NamedWindowRootView rootView;
        private readonly EventType eventType;

        /// <summary>Ctor.</summary>
        /// <param name="namedWindowService">service for dispatching results</param>
        /// <param name="windowName">the window name</param>
        /// <param name="eventType">the type of event held by the named window</param>
        /// <param name="createWindowStmtHandle">the statement handle of the statement that created the named window</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        /// <param name="revisionProcessor">for revision processing</param>
        public NamedWindowProcessor(NamedWindowService namedWindowService, String windowName, EventType eventType, EPStatementHandle createWindowStmtHandle, StatementResultService statementResultService, ValueAddEventProcessor revisionProcessor)
        {
            this.eventType = eventType;

            rootView = new NamedWindowRootView(revisionProcessor);
            tailView = new NamedWindowTailView(eventType, namedWindowService, rootView, createWindowStmtHandle, statementResultService, revisionProcessor);
            rootView.DataWindowContents = tailView;   // for iteration used for delete without index
        }

        /// <summary>Returns the tail view of the named window, hooked into the view chain after the named window's data window views, as the last view. </summary>
        /// <returns>tail view</returns>
        public NamedWindowTailView TailView
        {
            get { return tailView; }  // hooked as the tail sview before any data windows
        }

        /// <summary>Returns the root view of the named window, hooked into the view chain before the named window's data window views, right after the filter stream that filters for insert-into events. </summary>
        /// <returns>tail view</returns>
        public NamedWindowRootView RootView
        {
            get { return rootView; } // hooked as the top view before any data windows
        }

        /// <summary>Returns a new view for a new on-delete or on-select statement. </summary>
        /// <param name="onTriggerDesc">descriptor describing the on-trigger specification</param>
        /// <param name="filterEventType">event type to trigger on</param>
        /// <param name="statementStopService">to indicate a on-delete was stopped</param>
        /// <param name="internalEventRouter">for insert-into handling</param>
        /// <param name="resultSetProcessor">for select-clause processing</param>
        /// <param name="statementHandle">is the handle to the statement, used for routing/insert-into</param>
        /// <param name="joinExpr">is the join expression or null if there is none</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        /// <returns>on trigger handling view</returns>
        public NamedWindowOnExprBaseView AddOnExpr(OnTriggerDesc onTriggerDesc,
                                                   ExprNode joinExpr,
                                                   EventType filterEventType,
                                                   StatementStopService statementStopService,
                                                   InternalEventRouter internalEventRouter,
                                                   ResultSetProcessor resultSetProcessor,
                                                   EPStatementHandle statementHandle,
                                                   StatementResultService statementResultService)
        {
            return rootView.AddOnExpr(onTriggerDesc, joinExpr, filterEventType, statementStopService, internalEventRouter, resultSetProcessor, statementHandle, statementResultService);
        }

        /// <summary>Returns the event type of the named window. </summary>
        /// <returns>event type</returns>
        public EventType NamedWindowType
        {
            get { return eventType; }
        }

        /// <summary>Adds a consuming (selecting) statement to the named window. </summary>
        /// <param name="statementHandle">is the statement's handle for locking</param>
        /// <param name="statementStopService">for indicating the consuming statement is stopped or destroyed</param>
        /// <param name="filterList">is a list of filter expressions</param>
        /// <returns>consumer view</returns>
        public NamedWindowConsumerView AddConsumer(IList<ExprNode> filterList,
                                                   EPStatementHandle statementHandle,
                                                   StatementStopService statementStopService)
        {
            return tailView.AddConsumer(filterList, statementHandle, statementStopService);
        }

        /// <summary>Deletes a named window and removes any associated resources. </summary>
        public void Destroy()
        {
            tailView.Destroy();
            rootView.Destroy();
        }
    }
}
