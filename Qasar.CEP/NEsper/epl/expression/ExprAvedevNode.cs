using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents the avedev(...) aggregate function is an expression tree.
    /// </summary>

    public class ExprAvedevNode : ExprAggregateNode
    {
        /// <summary>
        /// Returns the aggregation function name for representation in a generate expression string.
        /// </summary>
        /// <value></value>
        /// <returns> aggregation function name
        /// </returns>
        public override String AggregationFunctionName
        {
            get { return "avedev"; }
        }

        /// <summary> Ctor.</summary>
        /// <param name="distinct">flag indicating unique or non-unique value aggregation
        /// </param>
        public ExprAvedevNode(bool distinct)
            : base(distinct)
        {
        }

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">used for resolving method and function names</param>
        /// <returns>aggregation function use</returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>

	    protected override AggregationMethod ValidateAggregationChild(StreamTypeService streamTypeService, MethodResolutionService methodResolutionService)
	    {
	        base.ValidateSingleNumericChild(streamTypeService);
	        return methodResolutionService.MakeAvedevAggregator();
	    }

        /// <summary>
        /// Return true if a expression aggregate node semantically equals the current node, or false if not.
        /// For use by the EqualsNode implementation which compares the distinct flag.
        /// </summary>
        /// <param name="node">to compare to</param>
        /// <returns>
        /// true if semantically equal, or false if not equals
        /// </returns>
        public override bool EqualsNodeAggregate(ExprAggregateNode node)
        {
            if (!(node is ExprAvedevNode))
            {
                return false;
            }

            return true;
        }
    }
}
