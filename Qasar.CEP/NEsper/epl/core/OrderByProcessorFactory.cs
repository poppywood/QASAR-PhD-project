using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.epl;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.expression;

using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Factory for <see cref="com.espertech.esper.epl.core.OrderByProcessor"/> processors.
    /// </summary>

    public class OrderByProcessorFactory
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Returns processor for order-by clauses.
        /// </summary>
        /// <param name="selectionList">is a list of select expressions</param>
        /// <param name="groupByNodes">is a list of group-by expressions</param>
        /// <param name="orderByList">is a list of order-by expressions</param>
        /// <param name="aggregationService">is the service for aggregation, ie. building sums and averages per group or overall</param>
        /// <param name="eventAdapterService">provides event adapters</param>
        /// <returns>ordering processor instance</returns>
        /// <throws>com.espertech.esper.epl.expression.ExprValidationException when validation of expressions fails</throws>

        public static OrderByProcessor GetProcessor(IList<SelectClauseExprCompiledSpec> selectionList,
                                                    IList<ExprNode> groupByNodes,
                                                    IList<OrderByItem> orderByList,
                                                    AggregationService aggregationService,
                                                    EventAdapterService eventAdapterService)
        {
            // Get the order by expression nodes
            IList<ExprNode> orderByNodes = new List<ExprNode>();
		    foreach(OrderByItem element in orderByList)
		    {
			    orderByNodes.Add(element.ExprNode);
		    }


            // No order-by clause
            if (orderByList.Count == 0)
            {
                log.Debug(".GetProcessor Using no OrderByProcessor");
                return null;
            }

            // Determine aggregate functions used in select, if any
            IList<ExprAggregateNode> selectAggNodes = new List<ExprAggregateNode>();
            foreach (SelectClauseExprCompiledSpec element in selectionList)
            {
                ExprAggregateNode.GetAggregatesBottomUp(element.SelectExpression, selectAggNodes);
            }

            // Get all the aggregate functions occuring in the order-by clause
            IList<ExprAggregateNode> orderAggNodes = new List<ExprAggregateNode>();
            foreach (ExprNode orderByNode in orderByNodes)
            {
                ExprAggregateNode.GetAggregatesBottomUp(orderByNode, orderAggNodes);
            }

            ValidateOrderByAggregates(selectAggNodes, orderAggNodes);

            // Create the type of the order-by event
            Map<String, Type> propertyNamesAndTypes = new HashMap<String, Type>();
            foreach (ExprNode orderByNode in orderByNodes)
            {
                propertyNamesAndTypes[orderByNode.ExpressionString] = orderByNode.ReturnType;
            }

            // Tell the order-by processor whether to compute group-by
            // keys if they are not present
            Boolean needsGroupByKeys = (selectionList.Count != 0) && (orderAggNodes.Count != 0);

            log.Debug(".GetProcessor Using OrderByProcessorSimple");
            return new OrderByProcessorSimple(orderByList, groupByNodes, needsGroupByKeys, aggregationService);
        }

        private static void ValidateOrderByAggregates(
            IList<ExprAggregateNode> selectAggNodes,
            IList<ExprAggregateNode> orderAggNodes)
        {
            // Check that the order-by clause doesn't contain 
            // any aggregate functions not in the select expression
            foreach (ExprAggregateNode orderAgg in orderAggNodes)
            {
                Boolean inSelect = false;
                foreach (ExprAggregateNode selectAgg in selectAggNodes)
                {
                    if (ExprNode.DeepEquals(selectAgg, orderAgg))
                    {
                        inSelect = true;
                        break;
                    }
                }
                if (!inSelect)
                {
                    throw new ExprValidationException("Aggregate functions in the order-by clause must also occur in the select expression");
                }
            }
        }
    }
}
