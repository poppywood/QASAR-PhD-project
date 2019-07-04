///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.join.plan;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.lookup;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// The root window in a named window plays multiple roles: It holds the indexes for deleting rows,
    /// if any on-delete statement requires such indexes. Such indexes are updated when events arrive, or
    /// remove from when a data window or on-delete statement expires events. The view keeps track of
    /// on-delete statements their indexes used.
    /// </summary>
    public class NamedWindowRootView : ViewSupport
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private EventType namedWindowEventType;
        private readonly NamedWindowIndexRepository indexRepository;
        private IEnumerable<EventBean> dataWindowContents;
        private readonly Map<LookupStrategy, PropertyIndexedEventTable> tablePerStrategy;
        private readonly ValueAddEventProcessor revisionProcessor;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="revisionProcessor">handle update events if supplied, or null if not handling revisions</param>
        public NamedWindowRootView(ValueAddEventProcessor revisionProcessor)
        {
            this.indexRepository = new NamedWindowIndexRepository();
            this.tablePerStrategy = new HashMap<LookupStrategy, PropertyIndexedEventTable>();
            this.revisionProcessor = revisionProcessor;
        }

        /// <summary>
        /// Gets or sets the enumerable to use to obtain current named window data window contents.
        /// </summary>
        public IEnumerable<EventBean> DataWindowContents
        {
            get { return this.dataWindowContents; }
            set { this.dataWindowContents = value; }
        }

        /// <summary>Called by tail view to indicate that the data window view exired events that must be removed from index tables. </summary>
        /// <param name="oldData">removed stream of the data window</param>
        //
        public void RemoveOldData(EventBean[] oldData)
        {
            if (revisionProcessor != null) {
                revisionProcessor.RemoveOldData(oldData, indexRepository);
            } else {
                foreach (EventTable table in indexRepository.Tables) {
                    table.Remove(oldData);
                }
            }
        }

        // Called by deletion strategy and also the insert-into for new events only
        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".update Received update, " +
                        "  newData.Length==" + ((newData == null) ? 0 : newData.Length) +
                        "  oldData.Length==" + ((oldData == null) ? 0 : oldData.Length));
            }

            if (revisionProcessor != null) {
                revisionProcessor.OnUpdate(newData, oldData, this, indexRepository);
            } else {
                // Update indexes for fast deletion, if there are any
                foreach (EventTable table in indexRepository.Tables) {
                    table.Add(newData);
                    table.Remove(oldData);
                }

                // Update child views
                UpdateChildren(newData, oldData);
            }
        }

        /// <summary>Add an on-trigger view that, using a lookup strategy, looks up from the named window and may select or delete rows. </summary>
        /// <param name="onTriggerDesc">the specification for the on-delete</param>
        /// <param name="filterEventType">the event type for the on-clause in the on-delete</param>
        /// <param name="statementStopService">for stopping the statement</param>
        /// <param name="internalEventRouter">for insert-into behavior</param>
        /// <param name="resultSetProcessor">@return view representing the on-delete view chain, posting delete events to it's listeners</param>
        /// <param name="statementHandle">is the handle to the statement, used for routing/insert-into</param>
        /// <param name="joinExpr">is the join expression or null if there is none</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        /// <returns>base view for on-trigger expression</returns>
        public NamedWindowOnExprBaseView AddOnExpr(OnTriggerDesc onTriggerDesc, ExprNode joinExpr, EventType filterEventType, StatementStopService statementStopService, InternalEventRouter internalEventRouter, ResultSetProcessor resultSetProcessor, EPStatementHandle statementHandle, StatementResultService statementResultService)
        {
            // Determine strategy for deletion and index table to use (if any)
            Pair<LookupStrategy,PropertyIndexedEventTable> strategy = GetStrategyPair(onTriggerDesc, joinExpr, filterEventType);

            // If a new table is required, add that table to be updated
            if (strategy.Second != null)
            {
                tablePerStrategy.Put(strategy.First, strategy.Second);
            }

            if (onTriggerDesc.OnTriggerType == OnTriggerType.ON_DELETE)
            {
                return new NamedWindowOnDeleteView(statementStopService, strategy.First, this, statementResultService);
            }
            else
            {
                return new NamedWindowOnSelectView(statementStopService, strategy.First, this, internalEventRouter, resultSetProcessor, statementHandle, statementResultService);
            }
        }

        /// <summary>Unregister an on-delete statement view, using the strategy as a key to remove a reference to the index table used by the strategy. </summary>
        /// <param name="strategy">to use for deleting events</param>
        public void RemoveOnExpr(LookupStrategy strategy)
        {
            PropertyIndexedEventTable table = tablePerStrategy.RemoveAndReturn(strategy);
            if (table != null)
            {
                indexRepository.RemoveTableReference(table);
            }
        }

        private Pair<LookupStrategy,PropertyIndexedEventTable> GetStrategyPair(OnTriggerDesc onTriggerDesc, ExprNode joinExpr, EventType filterEventType)
        {
            // No join expression means delete all
            if (joinExpr == null)
            {
                return new Pair<LookupStrategy,PropertyIndexedEventTable>(new LookupStrategyAllRows(dataWindowContents), null);
            }

            // analyze query graph; Whereas stream0=named window, stream1=delete-expr filter
            QueryGraph queryGraph = new QueryGraph(2);
            FilterExprAnalyzer.Analyze(joinExpr, queryGraph);

            // index and key property names
            String[] keyPropertiesJoin = queryGraph.GetKeyProperties(1, 0);
            String[] indexPropertiesJoin = queryGraph.GetIndexProperties(1, 0);

            // If the analysis revealed no join columns, must use the brute-force full table scan
            if ((keyPropertiesJoin == null) || (keyPropertiesJoin.Length == 0))
            {
                return new Pair<LookupStrategy,PropertyIndexedEventTable>(new LookupStrategyTableScan(joinExpr, dataWindowContents), null);
            }

            // Build a set of index descriptors with property name and coercion type
            bool mustCoerce = false;
            Type[] coercionTypes = new Type[indexPropertiesJoin.Length];
            for (int i = 0; i < keyPropertiesJoin.Length; i++)
            {
                Type keyPropType = TypeHelper.GetBoxedType(filterEventType.GetPropertyType(keyPropertiesJoin[i]));
                Type indexedPropType = TypeHelper.GetBoxedType(namedWindowEventType.GetPropertyType(indexPropertiesJoin[i]));
                Type coercionType = indexedPropType;
                if (keyPropType != indexedPropType)
                {
                    coercionType = TypeHelper.GetCompareToCoercionType(keyPropType, keyPropType);
                    mustCoerce = true;
                }

                coercionTypes[i] = coercionType;
            }

            // Add all joined fields to an array for sorting
            JoinedPropDesc[] joinedPropDesc = new JoinedPropDesc[keyPropertiesJoin.Length];
            for (int i = 0; i < joinedPropDesc.Length; i++)
            {
                joinedPropDesc[i] = new JoinedPropDesc(indexPropertiesJoin[i], coercionTypes[i], keyPropertiesJoin[i], 1);
            }

            Array.Sort(joinedPropDesc);
            keyPropertiesJoin = JoinedPropDesc.GetKeyProperties(joinedPropDesc);
            indexPropertiesJoin = JoinedPropDesc.GetIndexProperties(joinedPropDesc);

            // Get the table for this index
            PropertyIndexedEventTable table = indexRepository.AddTable(joinedPropDesc, dataWindowContents, namedWindowEventType, mustCoerce);

            // assign types and stream numbers
            EventType[] eventTypePerStream = new EventType[] {null, filterEventType};
            int[] streamNumbersPerProperty = new int[joinedPropDesc.Length];
            for (int i = 0; i < streamNumbersPerProperty.Length; i++)
            {
                streamNumbersPerProperty[i] = 1;
            }

            // create the strategy
            TableLookupStrategy lookupStrategy;
            if (!mustCoerce)
            {
                lookupStrategy = new IndexedTableLookupStrategy(eventTypePerStream, streamNumbersPerProperty, keyPropertiesJoin, table);
            }
            else
            {
                lookupStrategy = new IndexedTableLookupStrategyCoercing(eventTypePerStream, streamNumbersPerProperty, keyPropertiesJoin, table, coercionTypes);
            }

            return new Pair<LookupStrategy,PropertyIndexedEventTable>(new LookupStrategyIndexed(joinExpr, lookupStrategy), table);
        }

        public override Viewable Parent
        {
            set
            {
                base.Parent = value;
                namedWindowEventType = value.EventType;
            }
        }

        /// <summary>
        /// Provides metadata information about the type of object the event collection contains.
        /// </summary>
        /// <returns>
        /// metadata for the objects in the collection
        /// </returns>
        public override EventType EventType
        {
            get { return namedWindowEventType; }
        }

        public override IEnumerator<EventBean> GetEnumerator()
        {
            return null;
        }

        /// <summary>Destroy and clear resources. </summary>
        public void Destroy()
        {
            indexRepository.Destroy();
            tablePerStrategy.Clear();
        }
    }
}
