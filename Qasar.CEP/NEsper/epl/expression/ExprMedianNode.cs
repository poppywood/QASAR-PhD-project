using System;

using com.espertech.esper.collection;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;

namespace com.espertech.esper.epl.expression
{
	/// <summary>
	/// Represents the median(...) aggregate function is an expression tree.
	/// </summary>
	
	public class ExprMedianNode : ExprAggregateNode
	{
        /// <summary>
        /// Returns the aggregation function name for representation in a generate expression string.
        /// </summary>
        /// <value></value>
        /// <returns> aggregation function name
        /// </returns>
		public override string AggregationFunctionName
		{
			get { return "median"; }
		}

		/// <summary> Ctor.</summary>
		/// <param name="distinct">flag indicating unique or non-unique value aggregation
		/// </param>
		public ExprMedianNode( bool distinct )
			: base( distinct )
		{
		}

        /// <summary>
        /// Gives the aggregation node a chance to validate the sub-expression types.
        /// </summary>
        /// <param name="streamTypeService">is the types per stream</param>
        /// <param name="methodResolutionService">used for resolving method and function names</param>
        /// <returns>aggregation function use</returns>
        /// <throws>ExprValidationException when expression validation failed</throws>
	    protected override AggregationMethod ValidateAggregationChild(StreamTypeService streamTypeService, MethodResolutionService methodResolutionService)
		{
			base.ValidateSingleNumericChild( streamTypeService );
			return methodResolutionService.MakeMedianAggregator();
		}

        /// <summary>
        /// Return true if a expression aggregate node semantically equals the current node, or false if not.
        /// For use by the EqualsNode implementation which compares the distinct flag.
        /// </summary>
        /// <param name="node">to compare to</param>
        /// <returns>
        /// true if semantically equal, or false if not equals
        /// </returns>
		public override bool EqualsNodeAggregate( ExprAggregateNode node )
		{
			if ( !( node is ExprMedianNode ) )
			{
				return false;
			}

			return true;
		}
	}
}
