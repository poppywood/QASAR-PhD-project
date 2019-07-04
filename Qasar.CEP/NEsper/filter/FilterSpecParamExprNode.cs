///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.pattern;

namespace com.espertech.esper.filter
{
	/// <summary>
	/// This class represents an arbitrary expression node returning a bool value as a filter parameter in an <see cref="FilterSpecCompiled"/> filter specification.
	/// </summary>
	public sealed class FilterSpecParamExprNode : FilterSpecParam
	{
        private readonly ExprNode exprNode;
        private readonly Map<String, Pair<EventType, String>> taggedEventTypes;
        private readonly VariableService variableService;
        private readonly bool hasVariable;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="propertyName">is the event property name</param>
        /// <param name="filterOperator">is expected to be the BOOLEAN_EXPR operator</param>
        /// <param name="exprNode">represents the boolean expression</param>
        /// <param name="taggedEventTypes">is null if the expression doesn't need other streams, or is filled with a ordered list of stream names and types</param>
        /// <param name="variableService">provides access to variables</param>
        public FilterSpecParamExprNode(String propertyName,
                                      FilterOperator filterOperator,
                                      ExprNode exprNode,
                                      Map<String, Pair<EventType, String>> taggedEventTypes,
                                      VariableService variableService)
            : base(propertyName, filterOperator)
        {
            if (filterOperator != FilterOperator.BOOLEAN_EXPRESSION)
            {
                throw new ArgumentException("Invalid filter operator for filter expression node");
            }
            this.exprNode = exprNode;
            this.taggedEventTypes = taggedEventTypes;
            this.variableService = variableService;

            ExprNodeVariableVisitor visitor = new ExprNodeVariableVisitor();
            exprNode.Accept(visitor);
            this.hasVariable = visitor.HasVariables;
        }

	    /// <summary>
	    /// Returns the expression node of the bool expression this filter parameter represents.
	    /// </summary>
	    /// <returns>expression node</returns>
	    public ExprNode ExprNode
	    {
	    	get { return exprNode; }
	    }

	    /// <summary>
	    /// Returns the map of tag/stream names to event types that the filter expressions map use (for patterns)
	    /// </summary>
	    /// <returns>IDictionary</returns>
	    public Map<String, Pair<EventType, String>> TaggedEventTypes
	    {
            get { return taggedEventTypes; }
	    }

        /// <summary>
        /// Return the filter parameter constant to filter for.
        /// </summary>
        /// <param name="matchedEvents">is the prior results that can be used to determine filter parameters</param>
        /// <returns>filter parameter constant's value</returns>
	    public override Object GetFilterValue(MatchedEventMap matchedEvents)
	    {
            EventBean[] events = null;

	        if (taggedEventTypes != null)
	        {
                events = new EventBean[taggedEventTypes.Count + 1];
	            int count = 1;
	            foreach (String tag in taggedEventTypes.Keys)
	            {
	                events[count] = matchedEvents.GetMatchingEvent(tag);
	                count++;
	            }
	        }

            if (hasVariable)
            {
                return new ExprNodeAdapter(exprNode, events, variableService);
            }
            else
            {
                return new ExprNodeAdapter(exprNode, events, null);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
	    public override String ToString()
	    {
	        return base.ToString() + "  exprNode=" + exprNode;
	    }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
	    public override bool Equals(Object obj)
	    {
	        if (this == obj)
	        {
	            return true;
	        }

	        if (!(obj is FilterSpecParamExprNode))
	        {
	            return false;
	        }

	        FilterSpecParamExprNode other = (FilterSpecParamExprNode) obj;
	        if (!base.Equals(other))
	        {
	            return false;
	        }

	        if (exprNode != other.exprNode)
	        {
	            return false;
	        }

	        return true;
	    }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override int GetHashCode()
		{
            int result = base.GetHashCode();
            result = 31 * result + exprNode.GetHashCode();
            return result;
        }
	}
} // End of namespace
