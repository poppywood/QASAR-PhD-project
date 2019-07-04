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
using com.espertech.esper.epl.join.exec;
using com.espertech.esper.epl.join.plan;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.type;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// Factory for building a <see cref="JoinSetComposer"/> from analyzing filter nodes, for
    /// fast join tuple result set composition.
    /// </summary>
    public class JoinSetComposerFactoryImpl : JoinSetComposerFactory
    {
        /// <summary>
        /// Builds join tuple composer.
        /// </summary>
        /// <param name="outerJoinDescList">list of descriptors for outer join criteria</param>
        /// <param name="optionalFilterNode">filter tree for analysis to build indexes for fast access</param>
        /// <param name="streamTypes">types of streams</param>
        /// <param name="streamNames">names of streams</param>
        /// <param name="streamViews">leaf view per stream</param>
        /// <param name="selectStreamSelectorEnum">indicator for rstream or istream-only, for optimization</param>
        /// <param name="isUnidirectional">an array of indicators for each stream set to true for a unidirectional stream in a join</param>
        /// <param name="hasChildViews">indicates if child views are declared for a stream</param>
        /// <param name="isNamedWindow">indicates whether the join is against named windows</param>
        /// <returns>composer implementation</returns>
        /// <throws>
        /// ExprValidationException is thrown to indicate that
        /// validation of view use in joins failed.
        /// </throws>
        public JoinSetComposer MakeComposer(IList<OuterJoinDesc> outerJoinDescList,
                                                       ExprNode optionalFilterNode,
                                                       EventType[] streamTypes,
                                                       String[] streamNames,
                                                       Viewable[] streamViews,
                                                       SelectClauseStreamSelectorEnum selectStreamSelectorEnum,
                                                       bool[] isUnidirectional,
                                                       bool[] hasChildViews,
                                                       bool[] isNamedWindow)
        {
            // Determine if there is a historical
            bool hasHistorical = false;
            for (int i = 0; i < streamViews.Length; i++)
            {
                if (streamViews[i] is HistoricalEventViewable)
                {
                    if (hasHistorical)
                    {
                        throw new ExprValidationException("Joins between historical data streams are not supported");
                    }
                    hasHistorical = true;
                    if (streamTypes.Length > 2)
                    {
                        throw new ExprValidationException("Joins between historical data require a only one event stream in the join");
                    }
                }
            }

            EventTable[][] indexes;
            QueryStrategy[] queryStrategies;

            // Handle a join with a database or other historical data source
            if (hasHistorical)
            {
                Pair<EventTable[][], QueryStrategy[]> indexAndStrategies =
                        MakeComposerHistorical(outerJoinDescList, optionalFilterNode, streamTypes, streamViews);
                indexes = indexAndStrategies.First;
                queryStrategies = indexAndStrategies.Second;
                
                return new JoinSetComposerImpl(indexes, queryStrategies, selectStreamSelectorEnum);
            }


            // Determine if any stream has a unidirectional keyword
            int unidirectionalStreamNumber = -1;
            for (int i = 0; i < isUnidirectional.Length; i++)
            {
                if (isUnidirectional[i])
                {
                    if (unidirectionalStreamNumber != -1)
                    {
                        throw new ExprValidationException("The unidirectional keyword can only apply to one stream in a join");
                    }
                    unidirectionalStreamNumber = i;
                }
            }
            if ((unidirectionalStreamNumber != -1) && (hasChildViews[unidirectionalStreamNumber]))
            {
                throw new ExprValidationException("The unidirectional keyword requires that no views are declared onto the stream");
            }

            QueryPlan queryPlan = QueryPlanBuilder.GetPlan(streamTypes, outerJoinDescList, optionalFilterNode, streamNames);

            // Build indexes
            QueryPlanIndex[] indexSpecs = queryPlan.IndexSpecs;
            indexes = new EventTable[indexSpecs.Length][];
            for (int streamNo = 0; streamNo < indexSpecs.Length; streamNo++)
            {
                String[][] indexProps = indexSpecs[streamNo].IndexProps;
                Type[][] coercionTypes = indexSpecs[streamNo].CoercionTypesPerIndex;
                indexes[streamNo] = new EventTable[indexProps.Length];
                for (int indexNo = 0; indexNo < indexProps.Length; indexNo++)
                {
                    indexes[streamNo][indexNo] = BuildIndex(streamNo, indexProps[indexNo], coercionTypes[indexNo], streamTypes[streamNo]);
                }
            }

            // Build strategies
            QueryPlanNode[] queryExecSpecs = queryPlan.ExecNodeSpecs;
            queryStrategies = new QueryStrategy[queryExecSpecs.Length];
            for (int i = 0; i < queryExecSpecs.Length; i++)
            {
                QueryPlanNode planNode = queryExecSpecs[i];
                ExecNode executionNode = planNode.MakeExec(indexes, streamTypes);

                if (log.IsDebugEnabled)
                {
                    log.Debug(".makeComposer Execution nodes for stream " + i + " '" + streamNames[i] +
                        "' : \n" + ExecNode.Print(executionNode));
                }

                queryStrategies[i] = new ExecNodeQueryStrategy(i, streamTypes.Length, executionNode);
            }

            // If all streams have views, normal business is a query plan for each stream
            if (unidirectionalStreamNumber == -1)
            {
                return new JoinSetComposerImpl(indexes, queryStrategies, selectStreamSelectorEnum);
            }
            else
            {
                return new JoinSetComposerStreamToWinImpl(indexes, unidirectionalStreamNumber, queryStrategies[unidirectionalStreamNumber]);
            }
        }

        private Pair<EventTable[][], QueryStrategy[]> MakeComposerHistorical(
            IList<OuterJoinDesc> outerJoinDescList,
            ExprNode optionalFilterNode,
            EventType[] streamTypes,
            Viewable[] streamViews)
        {
            EventTable[][] indexes;
            QueryStrategy[] queryStrategies;

            // No tables for any streams
            indexes = new EventTable[streamTypes.Length][];
            queryStrategies = new QueryStrategy[streamTypes.Length];
            for (int streamNo = 0; streamNo < streamTypes.Length; streamNo++)
            {
                indexes[streamNo] = new EventTable[0];
            }

            int polledView = 0;
            int streamView = 1;
            if (streamViews[1] is HistoricalEventViewable)
            {
                streamView = 0;
                polledView = 1;
            }

            // Build an outer join expression node
            bool isOuterJoin = false;
            ExprEqualsNode outerJoinEqualsNode = null;
            if (outerJoinDescList.Count != 0)
            {
                OuterJoinDesc outerJoinDesc = outerJoinDescList[0];
                switch (outerJoinDesc.OuterJoinType)
                {
                    case OuterJoinType.FULL:
                        isOuterJoin = true;
                        break;
                    case OuterJoinType.LEFT:
                        isOuterJoin = (streamView == 0);
                        break;
                    case OuterJoinType.RIGHT:
                        isOuterJoin = (streamView == 1);
                        break;
                }

                outerJoinEqualsNode = new ExprEqualsNode(false);
                outerJoinEqualsNode.AddChildNode(outerJoinDesc.LeftNode);
                outerJoinEqualsNode.AddChildNode(outerJoinDesc.RightNode);
                outerJoinEqualsNode.Validate(null, null, null, null, null);
            }

            // Determine filter for indexing purposes
            ExprNode filterForIndexing = null;
            if ((outerJoinEqualsNode != null) && (optionalFilterNode != null)) // both filter and outer join, add
            {
                filterForIndexing = new ExprAndNode();
                filterForIndexing.AddChildNode(optionalFilterNode);
                filterForIndexing.AddChildNode(outerJoinEqualsNode);
            }
            else if ((outerJoinEqualsNode == null) && (optionalFilterNode != null))
            {
                filterForIndexing = optionalFilterNode;
            }
            else if (outerJoinEqualsNode != null)
            {
                filterForIndexing = outerJoinEqualsNode;
            }

            Pair<HistoricalIndexLookupStrategy, PollResultIndexingStrategy> indexStrategies =
                DetermineIndexing(filterForIndexing, streamTypes[polledView], streamTypes[streamView], polledView,
                                  streamView);

            HistoricalEventViewable viewable = (HistoricalEventViewable) streamViews[polledView];
            queryStrategies[streamView] =
                new HistoricalDataQueryStrategy(streamView, polledView, viewable, isOuterJoin, outerJoinEqualsNode,
                                                indexStrategies.First, indexStrategies.Second);

            return new Pair<EventTable[][], QueryStrategy[]>(indexes, queryStrategies);
        }

        private Pair<HistoricalIndexLookupStrategy, PollResultIndexingStrategy> DetermineIndexing(
                ExprNode filterForIndexing,
                EventType polledViewType,
                EventType streamViewType,
                int polledViewStreamNum,
                int streamViewStreamNum)
        {
            // No filter means unindexed event tables
            if (filterForIndexing == null)
            {
                return new Pair<HistoricalIndexLookupStrategy, PollResultIndexingStrategy>(
                    new HistoricalIndexLookupStrategyNoIndex(), new PollResultIndexingStrategyNoIndex());
            }

            // analyze query graph; Whereas stream0=named window, stream1=delete-expr filter
            QueryGraph queryGraph = new QueryGraph(2);
            FilterExprAnalyzer.Analyze(filterForIndexing, queryGraph);

            // index and key property names
            String[] keyPropertiesJoin = queryGraph.GetKeyProperties(streamViewStreamNum, polledViewStreamNum);
            String[] indexPropertiesJoin = queryGraph.GetIndexProperties(streamViewStreamNum, polledViewStreamNum);

            // If the analysis revealed no join columns, must use the brute-force full table scan
            if ((keyPropertiesJoin == null) || (keyPropertiesJoin.Length == 0))
            {
                return new Pair<HistoricalIndexLookupStrategy, PollResultIndexingStrategy>(
                    new HistoricalIndexLookupStrategyNoIndex(), new PollResultIndexingStrategyNoIndex());
            }

            // Build a set of index descriptors with property name and coercion type
            bool mustCoerce = false;
            Type[] coercionTypes = new Type[indexPropertiesJoin.Length];
            for (int i = 0; i < keyPropertiesJoin.Length; i++)
            {
                Type keyPropType = TypeHelper.GetBoxedType(streamViewType.GetPropertyType(keyPropertiesJoin[i]));
                Type indexedPropType = TypeHelper.GetBoxedType(polledViewType.GetPropertyType(indexPropertiesJoin[i]));
                Type coercionType = indexedPropType;
                if (keyPropType != indexedPropType)
                {
                    coercionType = TypeHelper.GetCompareToCoercionType(keyPropType, keyPropType);
                    mustCoerce = true;
                }

                coercionTypes[i] = coercionType;
            }

            // No coercion
            if (!mustCoerce)
            {
                PollResultIndexingStrategyIndex indexing =
                    new PollResultIndexingStrategyIndex(polledViewStreamNum, polledViewType, indexPropertiesJoin);
                HistoricalIndexLookupStrategy strategy =
                    new HistoricalIndexLookupStrategyIndex(streamViewType, keyPropertiesJoin);
                return new Pair<HistoricalIndexLookupStrategy, PollResultIndexingStrategy>(strategy, indexing);
            }
            else
            {
                // With coercion, same lookup strategy as the index coerces
                PollResultIndexingStrategy indexing =
                    new PollResultIndexingStrategyIndexCoerce(polledViewStreamNum, polledViewType, indexPropertiesJoin,
                                                              coercionTypes);
                HistoricalIndexLookupStrategy strategy =
                    new HistoricalIndexLookupStrategyIndex(streamViewType, keyPropertiesJoin);
                return new Pair<HistoricalIndexLookupStrategy, PollResultIndexingStrategy>(strategy, indexing);
            }
        }


        /// <summary>
        /// Build an index/table instance using the event properties for the event type.
        /// </summary>
        /// <param name="indexedStreamNum">number of stream indexed</param>
        /// <param name="indexProps">properties to index</param>
        /// <param name="optCoercionTypes">optional array of coercion types, or null if no coercion is required</param>
        /// <param name="eventType">type of event to expect</param>
        /// <returns>table build</returns>
        public static EventTable BuildIndex(int indexedStreamNum, String[] indexProps, Type[] optCoercionTypes, EventType eventType)
        {
            EventTable table = null;
            if (indexProps.Length == 0)
            {
                table = new UnindexedEventTable(indexedStreamNum);
            }
            else
            {
                if (optCoercionTypes == null)
                {
                    table = new PropertyIndexedEventTable(indexedStreamNum, eventType, indexProps);
                }
                else
                {
                    table = new PropertyIndTableCoerceAll(indexedStreamNum, eventType, indexProps, optCoercionTypes);
                }
            }
            return table;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
