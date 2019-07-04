///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.expression
{
	/// <summary>
    /// Represents a subselect in an expression tree.
    /// </summary>
    public class ExprSubselectInNode : ExprSubselectNode
    {
	    private bool isNotIn;
        private bool mustCoerce;
        private Type coercionType;

        /// <summary>Ctor.</summary>
        /// <param name="statementSpec">
        /// is the lookup statement spec from the parser, unvalidated
        /// </param>
        public ExprSubselectInNode(StatementSpecRaw statementSpec)
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
        /// Gets or sets a value indicating whether this instance is not in.
        /// </summary>
        /// <value><c>true</c> if this instance is not in; otherwise, <c>false</c>.</value>
        public bool IsNotIn
        {
            get { return isNotIn; }
            set { isNotIn = value; }
        }
        
        /// <summary>Indicate that this is a not-in lookup.</summary>
        /// <param name="notIn">is true for not-in, or false for regular 'in'</param>
        public void SetNotIn(bool notIn)
        {
            isNotIn = notIn;
        }

        /// <summary>
        /// Validates the specified stream type service.
        /// </summary>
        /// <param name="streamTypeService">The stream type service.</param>
        /// <param name="methodResolutionService">The method resolution service.</param>
        /// <param name="viewResourceDelegate">The view resource delegate.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
            if (this.ChildNodes.Count != 1)
            {
                throw new ExprValidationException("The Subselect-IN requires 1 child expression");
            }

            // Must be the same boxed type returned by expressions under this
            Type typeOne = TypeHelper.GetBoxedType(this.ChildNodes[0].ReturnType);
            Type typeTwo;
            if (selectClause != null)
            {
                typeTwo = selectClause.ReturnType;
            }
            else
            {
                typeTwo = this.rawEventType.UnderlyingType;
            }

            // Null constants can be compared for any type
            if ((typeOne == null) || (typeTwo == null))
            {
                return;
            }

            // Get the common type such as Bool, String or Double and Long
            try
            {
                coercionType = TypeHelper.GetCompareToCoercionType(typeOne, typeTwo);
            }
            catch (ArgumentException ex)
            {
                throw new ExprValidationException("Implicit conversion from datatype '" +
                        typeTwo.FullName +
                        "' to '" +
                        typeOne.FullName +
                        "' is not allowed", ex);
            }

            // Check if we need to coerce
            if ((coercionType == TypeHelper.GetBoxedType(typeOne)) &&
                (coercionType == TypeHelper.GetBoxedType(typeTwo)))
            {
                mustCoerce = false;
            }
            else
            {
                if (!TypeHelper.IsNumeric(coercionType))
                {
                    throw new IllegalStateException("Coercion type " + coercionType + " not numeric");
                }
                mustCoerce = true;
            }
        }

        public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData, Set<EventBean> matchingEvents)
        {
            if (matchingEvents == null)
            {
                return isNotIn;
            }
            if (matchingEvents.Count == 0)
            {
                return isNotIn;
            }

            // Filter according to the filter expression
            // Evaluate the select expression for each remaining row
            // Check if any of the results match the child expression, using coercion
            Set<EventBean> matchedFilteredEvents = matchingEvents;

            // Evaluate filter
            EventBean[] events = new EventBean[eventsPerStream.Length + 1];
            Array.Copy(eventsPerStream, 0, events, 1, eventsPerStream.Length);

            if (filterExpr != null)
            {
                matchedFilteredEvents = new HashSet<EventBean>();
                foreach (EventBean subselectEvent in matchingEvents)
                {
                    // Prepare filter expression event list
                    events[0] = subselectEvent;

                    // Eval filter expression
                    bool? pass = (bool?)filterExpr.Evaluate(events, true);
                    if (pass ?? false)
                    {
                        matchedFilteredEvents.Add(subselectEvent);
                    }
                }
            }
            if (matchedFilteredEvents.Count == 0)
            {
                return isNotIn;
            }

            // Evaluate the child expression
            Object leftResult = this.ChildNodes[0].Evaluate(eventsPerStream, isNewData);

            // Evaluate each select until we have a match
            foreach (EventBean _event in matchedFilteredEvents)
            {
                events[0] = _event;
                Object rightResult = selectClause.Evaluate(events, true);

                if (leftResult == null)
                {
                    if (rightResult == null)
                    {
                        return !isNotIn;
                    }
                    continue;
                }
                if (rightResult == null)
                {
                    continue;
                }

                if (!mustCoerce)
                {
                    if (leftResult.Equals(rightResult))
                    {
                        return !isNotIn;
                    }
                }
                else
                {
                    Object left = TypeHelper.CoerceBoxed(leftResult, coercionType);
                    Object right = TypeHelper.CoerceBoxed(rightResult, coercionType);
                    if (Object.Equals(left, right))
                    {
                        return !isNotIn;
                    }
                }
            }

            return isNotIn;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
