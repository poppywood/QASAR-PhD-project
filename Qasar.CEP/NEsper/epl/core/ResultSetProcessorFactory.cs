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
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;

using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Factory for output processors. Output processors process the result set of a join or of a view
    /// and apply aggregation/grouping, having and some output limiting logic.
    /// <para/>
    /// The instance produced by the factory depends on the presence of aggregation functions in the select list,
    /// the presence and nature of the group-by clause.
    /// <para/>
    /// In case (1) and (2) there are no aggregation functions in the select clause.
    /// <para/>
    /// Case (3) is without group-by and with aggregation functions and without non-aggregated properties
    /// in the select list: <pre>select Sum(volume) </pre>.
    /// Always produces one row for new and old data, aggregates without grouping.
    /// <para/>
    /// Case (4) is without group-by and with aggregation functions but with non-aggregated properties
    /// in the select list: <pre>select price, Sum(volume) </pre>.
    /// Produces a row for each event, aggregates without grouping.
    /// <para/>
    /// Case (5) is with group-by and with aggregation functions and all selected properties are grouped-by.
    /// in the select list: <pre>select customerId, Sum(volume) group by customerId</pre>.
    /// Produces a old and new data row for each group changed, aggregates with grouping, see
    /// <see cref="ResultSetProcessorRowPerGroup"/>
    /// <para/>
    /// Case (6) is with group-by and with aggregation functions and only some selected properties are grouped-by.
    /// in the select list: <pre>select customerId, supplierId, Sum(volume) group by customerId</pre>.
    /// Produces row for each event, aggregates with grouping.
    /// </summary>
	public class ResultSetProcessorFactory
	{
        /// <summary>
        /// Returns the result set process for the given select expression, group-by clause and
        /// having clause given a set of types describing each stream in the from-clause.
        /// </summary>
        /// <param name="statementSpecCompiled">the statement specification</param>
        /// <param name="stmtContext">engine and statement level services</param>
        /// <param name="typeService">for information about the streams in the from clause</param>
        /// <param name="viewResourceDelegate">delegates views resource factory to expression resources requirements</param>
        /// <returns>result set processor instance</returns>
        /// <throws>ExprValidationException when any of the expressions is invalid</throws>
	    public static ResultSetProcessor GetProcessor(StatementSpecCompiled statementSpecCompiled,
	                                                  StatementContext stmtContext,
	                                                  StreamTypeService typeService,
	                                                  ViewResourceDelegate viewResourceDelegate)
	    {
	        SelectClauseSpecCompiled selectClauseSpec = statementSpecCompiled.SelectClauseSpec;
	        InsertIntoDesc insertIntoDesc = statementSpecCompiled.InsertIntoDesc;
	        IList<ExprNode> groupByNodes = statementSpecCompiled.GroupByExpressions;
	        ExprNode optionalHavingNode = statementSpecCompiled.HavingExprRootNode;
	        OutputLimitSpec outputLimitSpec = statementSpecCompiled.OutputLimitSpec;
	        IList<OrderByItem> orderByList = statementSpecCompiled.OrderByList;

	        if (log.IsDebugEnabled)
	        {
	            log.Debug(".getProcessor Getting processor for " +
	                    " selectionList=" + selectClauseSpec.SelectExprList +
	                    " groupByNodes=" + CollectionHelper.Render(groupByNodes) +
	                    " optionalHavingNode=" + optionalHavingNode);
	        }

	        // Expand any instances of select-clause aliases in the
	        // order-by clause with the full expression
	        ExpandAliases(selectClauseSpec.SelectExprList, orderByList);

	        // Validate selection expressions, if any (could be wildcard i.e. empty list)
	        List<SelectClauseExprCompiledSpec> namedSelectionList = new List<SelectClauseExprCompiledSpec>();
	        for (int i = 0; i < selectClauseSpec.SelectExprList.Count; i++)
	        {
	            // validate element
	            SelectClauseElementCompiled element = selectClauseSpec.SelectExprList[i];
	            if (element is SelectClauseExprCompiledSpec)
	            {
	                SelectClauseExprCompiledSpec expr = (SelectClauseExprCompiledSpec) element;
	                ExprNode validatedExpression =
	                    expr.SelectExpression.GetValidatedSubtree(typeService,
	                                                              stmtContext.MethodResolutionService,
	                                                              viewResourceDelegate,
	                                                              stmtContext.SchedulingService,
	                                                              stmtContext.VariableService);

	                // determine an element name if none assigned
	                String asName = expr.AssignedName;
	                if (asName == null)
	                {
	                    asName = validatedExpression.ExpressionString;
	                }

	                expr.AssignedName = asName;
	                expr.SelectExpression = validatedExpression;
	                namedSelectionList.Add(expr);
	            }
	        }
	        bool isUsingWildcard = selectClauseSpec.IsUsingWildcard;

	        // Validate stream selections, if any (such as stream.*)
	        foreach (SelectClauseElementCompiled compiled in selectClauseSpec.SelectExprList)
	        {
	            if (!(compiled is SelectClauseStreamCompiledSpec))
	            {
	                continue;
	            }
	            SelectClauseStreamCompiledSpec streamSelectSpec = (SelectClauseStreamCompiledSpec) compiled;
	            int streamNum = Int32.MinValue;
	            bool isTaggedEvent = false;
                bool isProperty = false;
                Type propertyType = null;
                for (int i = 0; i < typeService.StreamNames.Length; i++)
	            {
	                String streamAlias = streamSelectSpec.StreamAliasName;
	                if (typeService.StreamNames[i].Equals(streamAlias))
	                {
	                    streamNum = i;
	                    break;
	                }

	                if (typeService.EventTypes[i] is TaggedCompositeEventType)
	                {
	                    TaggedCompositeEventType compositeType = (TaggedCompositeEventType) typeService.EventTypes[i];
	                    if (compositeType.TaggedEventTypes.Get(streamAlias) != null)
	                    {
	                        streamNum = i;
	                        isTaggedEvent = true;
	                        break;
	                    }
	                }
	            }

                // stream alias not found
                if (streamNum == Int32.MinValue)
	            {
                    // see if the stream name specified resolves as a property
                    PropertyResolutionDescriptor desc = null;
                    try
                    {
                        desc = typeService.ResolveByPropertyName(streamSelectSpec.StreamAliasName);
                    }
                    catch (StreamTypesException)
                    {
                        // not handled
                    }

                    if (desc == null)
                    {
                        throw new ExprValidationException("Stream selector '" + streamSelectSpec.StreamAliasName + ".*' does not match any stream alias name in the from clause");
                    }
                    isProperty = true;
                    propertyType = desc.PropertyType;
                    streamNum = desc.StreamNum;
                }

	            streamSelectSpec.StreamNumber = streamNum;
	            streamSelectSpec.IsTaggedEvent = isTaggedEvent;
	            streamSelectSpec.IsProperty = isProperty;
                streamSelectSpec.PropertyType = propertyType;
            }

	        // Validate group-by expressions, if any (could be empty list for no group-by)
	        for (int i = 0; i < groupByNodes.Count; i++)
	        {
	            // Ensure there is no subselects
	            ExprNodeSubselectVisitor visitor = new ExprNodeSubselectVisitor();
	            groupByNodes[i].Accept(visitor);
	            if (visitor.Subselects.Count > 0)
	            {
	                throw new ExprValidationException("Subselects not allowed within group-by");
	            }

	            groupByNodes[i] = groupByNodes[i].GetValidatedSubtree(typeService,
	                                                                  stmtContext.MethodResolutionService,
	                                                                  viewResourceDelegate,
	                                                                  stmtContext.SchedulingService,
	                                                                  stmtContext.VariableService);
	        }

	        // Validate having clause, if present
	        if (optionalHavingNode != null)
	        {
	            // Ensure there is no subselects
	            ExprNodeSubselectVisitor visitor = new ExprNodeSubselectVisitor();
	            optionalHavingNode.Accept(visitor);
	            if (visitor.Subselects.Count > 0)
	            {
	                throw new ExprValidationException("Subselects not allowed within having-clause");
	            }

	            optionalHavingNode =
	                optionalHavingNode.GetValidatedSubtree(typeService,
	                                                       stmtContext.MethodResolutionService,
	                                                       viewResourceDelegate,
	                                                       stmtContext.SchedulingService,
	                                                       stmtContext.VariableService);
	        }

	        // Validate order-by expressions, if any (could be empty list for no order-by)
	        for (int i = 0; i < orderByList.Count; i++)
	        {
	        	ExprNode orderByNode = orderByList[i].ExprNode;

	            // Ensure there is no subselects
	            ExprNodeSubselectVisitor visitor = new ExprNodeSubselectVisitor();
	            orderByNode.Accept(visitor);
	            if (visitor.Subselects.Count > 0)
	            {
	                throw new ExprValidationException("Subselects not allowed within order-by clause");
	            }

	            Boolean isDescending = orderByList[i].IsDescending;
	            OrderByItem validatedOrderBy =
	                new OrderByItem(
	                    orderByNode.GetValidatedSubtree(typeService,
	                                                    stmtContext.MethodResolutionService,
	                                                    viewResourceDelegate,
	                                                    stmtContext.SchedulingService,
	                                                    stmtContext.VariableService),
	                    isDescending);
	            orderByList[i] = validatedOrderBy;
	        }

	        // Get the select expression nodes
	        List<ExprNode> selectNodes = new List<ExprNode>();
	        foreach (SelectClauseExprCompiledSpec element in namedSelectionList)
	        {
	        	selectNodes.Add(element.SelectExpression);
	        }

	        // Get the order-by expression nodes
	        List<ExprNode> orderByNodes = new List<ExprNode>();
	        foreach (OrderByItem element in orderByList)
	        {
	        	orderByNodes.Add(element.ExprNode);
	        }

	        // Determine aggregate functions used in select, if any
	        List<ExprAggregateNode> selectAggregateExprNodes = new List<ExprAggregateNode>();
	        foreach (SelectClauseExprCompiledSpec element in namedSelectionList)
	        {
	            ExprAggregateNode.GetAggregatesBottomUp(element.SelectExpression, selectAggregateExprNodes);
	        }

	        // Determine if we have a having clause with aggregation
	        List<ExprAggregateNode> havingAggregateExprNodes = new List<ExprAggregateNode>();
	        Set<Pair<int, String>> propertiesAggregatedHaving = new HashSet<Pair<int, String>>();
	        if (optionalHavingNode != null)
	        {
	            ExprAggregateNode.GetAggregatesBottomUp(optionalHavingNode, havingAggregateExprNodes);
	            propertiesAggregatedHaving = GetAggregatedProperties(havingAggregateExprNodes);
	        }

	        // Determine if we have a order-by clause with aggregation
	        List<ExprAggregateNode> orderByAggregateExprNodes = new List<ExprAggregateNode>();
	        if (orderByNodes != null)
	        {
	            foreach (ExprNode orderByNode in orderByNodes)
	            {
	                ExprAggregateNode.GetAggregatesBottomUp(orderByNode, orderByAggregateExprNodes);
	            }
	        }

	        // Construct the appropriate aggregation service
	        bool hasGroupBy = CollectionHelper.IsNotEmpty(groupByNodes);
            AggregationService aggregationService =
                AggregationServiceFactory.GetService(selectAggregateExprNodes,
                                                     havingAggregateExprNodes,
                                                     orderByAggregateExprNodes,
                                                     hasGroupBy,
                                                     stmtContext.MethodResolutionService);

	        // Construct the processor for sorting output events
	        OrderByProcessor orderByProcessor = OrderByProcessorFactory.GetProcessor(namedSelectionList,
	                groupByNodes, orderByList, aggregationService, stmtContext.EventAdapterService);

	        // Construct the processor for evaluating the select clause
            SelectExprProcessor selectExprProcessor =
                SelectExprProcessorFactory.GetProcessor(selectClauseSpec.SelectExprList,
                                                        isUsingWildcard,
                                                        insertIntoDesc,
                                                        typeService,
                                                        stmtContext.EventAdapterService,
                                                        stmtContext.StatementResultService,
                                                        stmtContext.ValueAddEventService);

	        // Get a list of event properties being aggregated in the select clause, if any
	        Set<Pair<int, String>> propertiesGroupBy = GetGroupByProperties(groupByNodes);
	        // Figure out all non-aggregated event properties in the select clause (props not under a sum/avg/max aggregation node)
	        Set<Pair<int, String>> nonAggregatedProps = GetNonAggregatedProps(selectNodes);

	        // Validate that group-by is filled with sensible nodes (identifiers, and not part of aggregates selected, no aggregates)
	        ValidateGroupBy(groupByNodes);

	        // Validate the having-clause (selected aggregate nodes and all in group-by are allowed)
	        if (optionalHavingNode != null)
	        {
	            ValidateHaving(propertiesGroupBy, optionalHavingNode);
	        }

	        // We only generate Remove-Stream events if they are explicitly selected, or the insert-into requires them
	        bool isSelectRStream = (statementSpecCompiled.SelectStreamSelectorEnum == SelectClauseStreamSelectorEnum.RSTREAM_ISTREAM_BOTH
	                || statementSpecCompiled.SelectStreamSelectorEnum == SelectClauseStreamSelectorEnum.RSTREAM_ONLY);
	        if ((statementSpecCompiled.InsertIntoDesc != null) && (!statementSpecCompiled.InsertIntoDesc.IsIStream))
	        {
	            isSelectRStream = true;
	        }

	        // Determine if any output rate limiting must be performed early while processing results
	        bool isOutputLimiting = outputLimitSpec != null;
	        if ((outputLimitSpec != null) && outputLimitSpec.DisplayLimit == OutputLimitLimitType.SNAPSHOT)
	        {
	            isOutputLimiting = false;   // Snapshot output does not count in terms of limiting output for grouping/aggregation purposes
	        }

	        // (1)
	        // There is no group-by clause and no aggregate functions with event properties in the select clause and having clause (simplest case)
	        if (CollectionHelper.IsEmpty(groupByNodes) &&
                CollectionHelper.IsEmpty(selectAggregateExprNodes) &&
                CollectionHelper.IsEmpty(havingAggregateExprNodes))
	        {
	            // (1a)
	            // There is no need to perform select expression processing, the single view itself (no join) generates
	            // events in the desired format, therefore there is no output processor. There are no order-by expressions.
	            if (CollectionHelper.IsEmpty(orderByNodes) && optionalHavingNode == null && !isOutputLimiting)
	            {
	                log.Debug(".getProcessor Using no result processor");
	                return new ResultSetProcessorHandThrough(selectExprProcessor, isSelectRStream);
	            }

	            // (1b)
	            // We need to process the select expression in a simple fashion, with each event (old and new)
	            // directly generating one row, and no need to update aggregate state since there is no aggregate function.
	            // There might be some order-by expressions.
	            log.Debug(".getProcessor Using ResultSetProcessorSimple");
	            return new ResultSetProcessorSimple(selectExprProcessor, orderByProcessor, optionalHavingNode, isSelectRStream);
	        }

	        // (2)
	        // A wildcard select-clause has been specified and the group-by is ignored since no aggregation functions are used, and no having clause
            if (CollectionHelper.IsEmpty(namedSelectionList) &&
                CollectionHelper.IsEmpty(propertiesAggregatedHaving))
	        {
	            log.Debug(".getProcessor Using ResultSetProcessorSimple");
	            return new ResultSetProcessorSimple(selectExprProcessor, orderByProcessor, optionalHavingNode, isSelectRStream);
	        }

            bool hasAggregation = CollectionHelper.IsNotEmpty(selectAggregateExprNodes) ||
                                  CollectionHelper.IsNotEmpty(propertiesAggregatedHaving);
	        if (CollectionHelper.IsEmpty(groupByNodes) && hasAggregation)
	        {
	            // (3)
	            // There is no group-by clause and there are aggregate functions with event properties in the select clause (aggregation case)
	            // or having class, and all event properties are aggregated (all properties are under aggregation functions).
	            if (CollectionHelper.IsEmpty(nonAggregatedProps) && (!isUsingWildcard))
	            {
	                log.Debug(".getProcessor Using ResultSetProcessorRowForAll");
	                return new ResultSetProcessorRowForAll(selectExprProcessor, aggregationService, orderByProcessor, optionalHavingNode, isSelectRStream);
	            }

	            // (4)
	            // There is no group-by clause but there are aggregate functions with event properties in the select clause (aggregation case)
	            // or having clause and not all event properties are aggregated (some properties are not under aggregation functions).
	            log.Debug(".getProcessor Using ResultSetProcessorAggregateAll");
	            return new ResultSetProcessorAggregateAll(selectExprProcessor, orderByProcessor, aggregationService, optionalHavingNode, isSelectRStream);
	        }

	        // Handle group-by cases
	        if (CollectionHelper.IsEmpty(groupByNodes))
	        {
	            throw new IllegalStateException("Unexpected empty group-by expression list");
	        }

	        // Figure out if all non-aggregated event properties in the select clause are listed in the group by
	        Set<Pair<int, String>> nonAggregatedPropsSelect = GetNonAggregatedProps(selectNodes);
	        bool allInGroupBy = true;
	        foreach (Pair<int, String> nonAggregatedProp in nonAggregatedPropsSelect)
	        {
	            if (!propertiesGroupBy.Contains(nonAggregatedProp))
	            {
	                allInGroupBy = false;
	            }
	        }

	        // Wildcard select-clause means we do not have all selected properties in the group
	        if (CollectionHelper.IsEmpty(namedSelectionList))
	        {
	            allInGroupBy = false;
	        }

	        // Figure out if all non-aggregated event properties in the order-by clause are listed in the select expression
	        Set<Pair<int, String>> nonAggregatedPropsOrderBy = GetNonAggregatedProps(orderByNodes);

	        bool allInSelect = true;
	        foreach (Pair<int, String> nonAggregatedProp in nonAggregatedPropsOrderBy)
	        {
	            if (!nonAggregatedPropsSelect.Contains(nonAggregatedProp))
	            {
	                allInSelect = false;
	            }
	        }

	        // Wildcard select-clause means that all order-by props in the select expression
	        if (CollectionHelper.IsEmpty(namedSelectionList))
	        {
	            allInSelect = true;
	        }

	        // (4)
	        // There is a group-by clause, and all event properties in the select clause that are not under an aggregation
	        // function are listed in the group-by clause, and if there is an order-by clause, all non-aggregated properties
	        // referred to in the order-by clause also appear in the select (output one row per group, not one row per event)
	        if (allInGroupBy && allInSelect)
	        {
	            log.Debug(".getProcessor Using ResultSetProcessorRowPerGroup");
	            return new ResultSetProcessorRowPerGroup(selectExprProcessor, orderByProcessor, aggregationService, groupByNodes, optionalHavingNode, isSelectRStream);
	        }

	        // (6)
	        // There is a group-by clause, and one or more event properties in the select clause that are not under an aggregation
	        // function are not listed in the group-by clause (output one row per event, not one row per group)
	        log.Debug(".getProcessor Using ResultSetProcessorAggregateGrouped");
	        return new ResultSetProcessorAggregateGrouped(selectExprProcessor, orderByProcessor, aggregationService, groupByNodes, optionalHavingNode, isSelectRStream);
	    }

	    private static void ValidateHaving(ICollection<Pair<int, string>> propertiesGroupedBy,
	                                       ExprNode havingNode)
	    {
	        List<ExprAggregateNode> aggregateNodesHaving = new List<ExprAggregateNode>();
	        if (aggregateNodesHaving != null)
	        {
	            ExprAggregateNode.GetAggregatesBottomUp(havingNode, aggregateNodesHaving);
	        }

	        // Any non-aggregated properties must occur in the group-by clause (if there is one)
	        if (CollectionHelper.IsNotEmpty(propertiesGroupedBy))
	        {
	            ExprNodeIdentifierVisitor visitor = new ExprNodeIdentifierVisitor(true);
	            havingNode.Accept(visitor);
	            IList<Pair<int, String>> allPropertiesHaving = visitor.ExprProperties;
	            Set<Pair<int, String>> aggPropertiesHaving = GetAggregatedProperties(aggregateNodesHaving);
	            CollectionHelper.RemoveAll(allPropertiesHaving, aggPropertiesHaving);
	            CollectionHelper.RemoveAll(allPropertiesHaving, propertiesGroupedBy);

	            if (CollectionHelper.IsNotEmpty(allPropertiesHaving))
	            {
	                String name = CollectionHelper.First(allPropertiesHaving).Second;
	                throw new ExprValidationException("Non-aggregated property '" + name + "' in the HAVING clause must occur in the group-by clause");
	            }
	        }
	    }

	    private static void ValidateGroupBy(IEnumerable<ExprNode> groupByNodes)
	    {
	        // Make sure there is no aggregate function in group-by
	        List<ExprAggregateNode> aggNodes = new List<ExprAggregateNode>();
	        foreach (ExprNode groupByNode in groupByNodes)
	        {
	            ExprAggregateNode.GetAggregatesBottomUp(groupByNode, aggNodes);
	            if (CollectionHelper.IsNotEmpty(aggNodes))
	            {
	                throw new ExprValidationException("Group-by expressions cannot contain aggregate functions");
	            }
	        }
	    }

	    private static Set<Pair<int, String>> GetNonAggregatedProps(IEnumerable<ExprNode> exprNodes)
	    {
	        // Determine all event properties in the clause
	        Set<Pair<int, String>> nonAggProps = new HashSet<Pair<int, String>>();
	        foreach (ExprNode node in exprNodes)
	        {
	            ExprNodeIdentifierVisitor visitor = new ExprNodeIdentifierVisitor(false);
	            node.Accept(visitor);
	            IList<Pair<int, String>> propertiesNode = visitor.ExprProperties;
	            nonAggProps.AddAll(propertiesNode);
	        }

	        return nonAggProps;
	    }

	    private static Set<Pair<int, String>> GetAggregatedProperties(IEnumerable<ExprAggregateNode> aggregateNodes)
	    {
	        // Get a list of properties being aggregated in the clause.
	        Set<Pair<int, String>> propertiesAggregated = new HashSet<Pair<int, String>>();
	        foreach (ExprNode selectAggExprNode in aggregateNodes)
	        {
	            ExprNodeIdentifierVisitor visitor = new ExprNodeIdentifierVisitor(true);
	            selectAggExprNode.Accept(visitor);
	            IList<Pair<int, String>> properties = visitor.ExprProperties;
	            propertiesAggregated.AddAll(properties);
	        }

	        return propertiesAggregated;
	    }

	    private static Set<Pair<int, String>> GetGroupByProperties(IEnumerable<ExprNode> groupByNodes)
	    {
	        // Get the set of properties refered to by all group-by expression nodes.
	        Set<Pair<int, String>> propertiesGroupBy = new HashSet<Pair<int, String>>();

	        foreach (ExprNode groupByNode in groupByNodes)
	        {
	            ExprNodeIdentifierVisitor visitor = new ExprNodeIdentifierVisitor(true);
	            groupByNode.Accept(visitor);
	            IList<Pair<int, String>> propertiesNode = visitor.ExprProperties;
	            propertiesGroupBy.AddAll(propertiesNode);

	            // For each group-by expression node, require at least one property.
	            if (CollectionHelper.IsEmpty(propertiesNode))
	            {
	                throw new ExprValidationException("Group-by expressions must refer to property names");
	            }
	        }

	        return propertiesGroupBy;
	    }

    private static void ExpandAliases(IList<SelectClauseElementCompiled> selectionList, IList<OrderByItem> orderByList)
	    {
	    	foreach (SelectClauseElementCompiled selectElement in selectionList)
	    	{
	            // process only expressions
	            if (!(selectElement is SelectClauseExprCompiledSpec))
	            {
	                continue;
	            }
	            SelectClauseExprCompiledSpec selectExpr = (SelectClauseExprCompiledSpec) selectElement;

	            String alias = selectExpr.AssignedName;
	    		if(alias != null)
	    		{
	    			ExprNode fullExpr = selectExpr.SelectExpression;
                    for (int ii = 0; ii < orderByList.Count; ii++) {
                        OrderByItem orderByElement = orderByList[ii];
                        ExprNode swapped = AliasNodeSwapper.Swap(orderByElement.ExprNode, alias, fullExpr);
                        OrderByItem newOrderByElement = new OrderByItem(swapped, orderByElement.IsDescending);
                        orderByList[ii] = newOrderByElement;
                    }
	    		}
	    	}
	    }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
