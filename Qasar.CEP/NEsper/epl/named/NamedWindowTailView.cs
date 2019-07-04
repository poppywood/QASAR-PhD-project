///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// This view is hooked into a named window's view chain as the last view and handles dispatching of
    /// named window insert and remove stream results via <see cref="NamedWindowService"/> to consuming statements.
    /// </summary>
    public class NamedWindowTailView
        : ViewSupport
        , IEnumerable<EventBean>
    {
        private readonly EventType eventType;
        private readonly NamedWindowRootView namedWindowRootView;
        private readonly NamedWindowService namedWindowService;
        [NonSerialized]
        private Map<EPStatementHandle, IList<NamedWindowConsumerView>> consumers;  // handles as copy-on-write
        private readonly EPStatementHandle createWindowStmtHandle;
        private readonly StatementResultService statementResultService;
        private readonly ValueAddEventProcessor revisionProcessor;

        /// <summary>Ctor.</summary>
        /// <param name="eventType">the event type of the named window</param>
        /// <param name="namedWindowService">the service for dispatches to consumers for hooking into the dispatch loop</param>
        /// <param name="namedWindowRootView">the root data window view for indicating remove stream events to be removed from possible on-delete indexes</param>
        /// <param name="createWindowStmtHandle">statement handle for the statement that created the named window, for safe iteration</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        /// <param name="revisionProcessor">handles update events</param>
        public NamedWindowTailView(EventType eventType, NamedWindowService namedWindowService, NamedWindowRootView namedWindowRootView, EPStatementHandle createWindowStmtHandle, StatementResultService statementResultService, ValueAddEventProcessor revisionProcessor)
        {
            this.eventType = eventType;
            this.namedWindowService = namedWindowService;
            consumers = new HashMap<EPStatementHandle, IList<NamedWindowConsumerView>>();
            this.namedWindowRootView = namedWindowRootView;
            this.createWindowStmtHandle = createWindowStmtHandle;
            this.statementResultService = statementResultService;
            this.revisionProcessor = revisionProcessor;
        }

        /// <summary>Returns true to indicate that the data window view is a batch view.</summary>
        /// <returns>true if batch view</returns>
        public bool IsParentBatchWindow
        {
            get { return this.Parent is BatchingDataWindowView; }
        }

        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            // Only old data (remove stream) needs to be removed from indexes (kept by root view), if any
            if (oldData != null)
            {
                namedWindowRootView.RemoveOldData(oldData);
            }

            // Post to child views, only if there are listeners or subscribers
            if (statementResultService.IsMakeNatural || statementResultService.IsMakeSynthetic)
            {
                UpdateChildren(newData, oldData);
            }

            // Add to dispatch list for later result dispatch by runtime
            NamedWindowDeltaData delta = new NamedWindowDeltaData(newData, oldData);
            namedWindowService.AddDispatch(delta, consumers);
        }

        /// <summary>Adds a consumer view keeping the consuming statement's handle and lock to coordinate dispatches. </summary>
        /// <param name="statementHandle">the statement handle</param>
        /// <param name="statementStopService">for when the consumer stops, to unregister the consumer</param>
        /// <param name="filterList">is a list of filter expressions</param>
        /// <returns>consumer representative view</returns>
        public NamedWindowConsumerView AddConsumer(IList<ExprNode> filterList, EPStatementHandle statementHandle, StatementStopService statementStopService)
        {
            // Construct consumer view, allow a callback to this view to remove the consumer
            NamedWindowConsumerView consumerView = new NamedWindowConsumerView(filterList, eventType, statementStopService, this);

            // Keep a list of consumer views per statement to accomodate joins and subqueries
            IList<NamedWindowConsumerView> viewsPerStatements = consumers.Get(statementHandle);
            if (viewsPerStatements == null)
            {
                viewsPerStatements = new CopyOnWriteList<NamedWindowConsumerView>();

                // avoid concurrent modification as a thread may currently iterate over consumers as its dispatching
                // without the engine lock
                Map<EPStatementHandle, IList<NamedWindowConsumerView>> newConsumers = new HashMap<EPStatementHandle, IList<NamedWindowConsumerView>>();
                newConsumers.PutAll(consumers);
                newConsumers.Put(statementHandle, viewsPerStatements);
                consumers = newConsumers;
            }
            viewsPerStatements.Add(consumerView);

            return consumerView;
        }

        /// <summary>Called by the consumer view to indicate it was stopped or destroyed, such that the consumer can be deregistered and further dispatches disregard this consumer. </summary>
        /// <param name="namedWindowConsumerView">is the consumer representative view</param>
        public void RemoveConsumer(NamedWindowConsumerView namedWindowConsumerView)
        {
            EPStatementHandle handleRemoved = null;
            // Find the consumer view
            foreach (KeyValuePair<EPStatementHandle, IList<NamedWindowConsumerView>> entry in consumers)
            {
                bool foundAndRemoved = entry.Value.Remove(namedWindowConsumerView);
                // Remove the consumer view
                if ((foundAndRemoved) && (entry.Value.Count == 0))
                {
                    // Remove the handle if this list is now empty
                    handleRemoved = entry.Key;
                    break;
                }
            }
            if (handleRemoved != null)
            {
                Map<EPStatementHandle, IList<NamedWindowConsumerView>> newConsumers = new HashMap<EPStatementHandle, IList<NamedWindowConsumerView>>();
                newConsumers.PutAll(consumers);
                newConsumers.Remove(handleRemoved);
                consumers = newConsumers;
            }
        }

        public override EventType EventType
        {
            get { return eventType; }
        }

        public override IEnumerator<EventBean> GetEnumerator()
        {
            if (revisionProcessor != null) {
                ICollection<EventBean> coll = revisionProcessor.GetSnapshot(createWindowStmtHandle, parent);
                return coll.GetEnumerator();
            }

            using (createWindowStmtHandle.StatementLock.AcquireLock(null)) {
                IEnumerator<EventBean> ee = parent.GetEnumerator();
                List<EventBean> list = new List<EventBean>();
                while(ee.MoveNext()) {
                    list.Add(ee.Current);
                }

                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns a snapshot of window contents, thread-safely
        /// </summary>
        /// <returns>window contents</returns>
        public ICollection<EventBean> Snapshot()
        {
            if (revisionProcessor != null)
            {
                return revisionProcessor.GetSnapshot(createWindowStmtHandle, parent);
            }

            using(createWindowStmtHandle.StatementLock.AcquireLock(null))
            {
                IEnumerator<EventBean> pEnum = parent.GetEnumerator();
                
                LinkedList<EventBean> list = new LinkedList<EventBean>();
                while( pEnum.MoveNext() ) {
                    list.AddLast(pEnum.Current);
                }

                return list;
            }
        }

        /// <summary>Destroy the view. </summary>
        public void Destroy()
        {
            consumers.Clear();
        }
    }
}
