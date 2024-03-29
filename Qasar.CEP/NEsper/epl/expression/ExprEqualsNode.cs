using System;
using System.Text;

using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents an equals (=) comparator in a filter expressiun tree.
    /// </summary>

    public class ExprEqualsNode : ExprNode
    {
        /// <summary>
        /// Returns true if this is a NOT EQUALS node, false if this is a EQUALS node.
        /// </summary>
        /// <value><c>true</c> if [not equals]; otherwise, <c>false</c>.</value>
        /// <returns> true for !=, false for =
        /// </returns>
        virtual public bool IsNotEquals
        {
            get { return isNotEquals; }
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

	    public override bool IsConstantResult
	    {
	        get { return false; }
	    }
	
        private readonly bool isNotEquals;

        private bool mustCoerce;
        private Type coercionType;

        /// <summary> Ctor.</summary>
        /// <param name="isNotEquals">true if this is a (!=) not equals rather then equals, false if its a '=' equals
        /// </param>
        public ExprEqualsNode(bool isNotEquals)
        {
            this.isNotEquals = isNotEquals;
        }

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate">The view resource delegate.</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
            // Must have 2 child nodes
            if (this.ChildNodes.Count != 2)
            {
                throw new IllegalStateException("Equals node does not have exactly 2 child nodes");
            }

            // Must be the same boxed type returned by expressions under this
            Type typeOne = TypeHelper.GetBoxedType(this.ChildNodes[0].ReturnType);
            Type typeTwo = TypeHelper.GetBoxedType(this.ChildNodes[1].ReturnType);

            // Null constants can be compared for any type
            if ((typeOne == null) || (typeTwo == null))
            {
                return;
            }

            if (typeOne == typeTwo)
            {
                mustCoerce = false;
                return;
            }

            // Get the common type such as Bool, String or Double and Long
            try
            {
                coercionType = TypeHelper.GetCompareToCoercionType(typeOne, typeTwo);
            }
            catch (ArgumentException)
            {
                throw new ExprValidationException(
                    "Implicit conversion from datatype '" +
                    typeTwo.FullName + "' to '" + 
                    typeOne.FullName + "' is not allowed");
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

        /// <summary>
        /// Evaluate event tuple and return result.
        /// </summary>
        /// <param name="eventsPerStream">event tuple</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns>
        /// evaluation result, a bool value for OR/AND-type evalution nodes.
        /// </returns>
        public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
        {
            Object leftResult = this.ChildNodes[0].Evaluate(eventsPerStream, isNewData);
            Object rightResult = this.ChildNodes[1].Evaluate(eventsPerStream, isNewData);

            if (leftResult == null)
            {
                return (rightResult == null) ^ isNotEquals;
            }
            if (rightResult == null)
            {
                return isNotEquals;
            }

            if (!mustCoerce)
            {
                return leftResult.Equals(rightResult) ^ isNotEquals;
            }
            else
            {
                Object left = TypeHelper.CoerceBoxed(leftResult, coercionType);
                Object right = TypeHelper.CoerceBoxed(rightResult, coercionType);
                return left.Equals(right) ^ isNotEquals;
            }
        }

        /// <summary>
        /// Returns the expression node rendered as a string.
        /// </summary>
        /// <value></value>
        /// <returns> string rendering of expression
        /// </returns>
        public override String ExpressionString
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                buffer.Append(this.ChildNodes[0].ExpressionString);
                buffer.Append(" = ");
                buffer.Append(this.ChildNodes[1].ExpressionString);

                return buffer.ToString();
            }
        }

        /// <summary>
        /// Return true if a expression node semantically equals the current node, or false if not.
        /// Concrete implementations should compare the type and any additional information
        /// that impact the evaluation of a node.
        /// </summary>
        /// <param name="node">to compare to</param>
        /// <returns>
        /// true if semantically equal, or false if not equals
        /// </returns>
        public override bool EqualsNode(ExprNode node)
        {
            if (!(node is ExprEqualsNode))
            {
                return false;
            }

            ExprEqualsNode other = (ExprEqualsNode)node;

            if (other.isNotEquals != this.isNotEquals)
            {
                return false;
            }

            return true;
        }
    }
}
