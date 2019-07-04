///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using log4net;

namespace com.espertech.esper.epl.expression
{
	/// <summary>
    /// Represents an exists-subselect in an expression tree.
    /// </summary>
	
    public class ExprSubselectExistsNode : ExprSubselectNode
	{
	    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    /// <summary>Ctor.</summary>
	    /// <param name="statementSpec">
	    /// is the lookup statement spec from the parser, unvalidated
	    /// </param>
	    public ExprSubselectExistsNode(StatementSpecRaw statementSpec)
			: base(statementSpec)
	    {
	    }

        /// <summary>
        /// Returns the type that the node's evaluate method returns an instance of.
        /// </summary>
        /// <value>The type.</value>
        /// <returns> type returned when evaluated
        /// </returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override Type ReturnType
	    {
            get { return typeof(bool?); }
	    }

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate">delegates for view resources to expression nodes</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
	    }

        /// <summary>
        /// Evaluate the subquery expression returning an evaluation result object.
        /// </summary>
        /// <param name="eventsPerStream">is the events for each stream in a join</param>
        /// <param name="isNewData">is true for new data, or false for old data</param>
        /// <param name="matchingEvents">is filtered results from the table of stored subquery events</param>
        /// <returns>evaluation result</returns>
	    public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData, Set<EventBean> matchingEvents)
	    {
	        if (matchingEvents == null)
	        {
	            return false;
	        }
	        if (matchingEvents.Count == 0)
	        {
	            return false;
	        }

	        if (filterExpr == null)
	        {
	            return true;
	        }

	        // Evaluate filter
	        EventBean[] events = new EventBean[eventsPerStream.Length + 1];
            Array.Copy(eventsPerStream, 0, events, 1, eventsPerStream.Length);

	        foreach (EventBean subselectEvent in matchingEvents)
	        {
	            // Prepare filter expression event list
	            events[0] = subselectEvent;

	            bool? pass = (bool?) filterExpr.Evaluate(events, true);
                if (pass ?? false)
	            {
	                return true;
	            }
	        }

	        return false;
	    }
	}
} // End of namespace
