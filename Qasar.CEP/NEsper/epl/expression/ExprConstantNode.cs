using System;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents a constant in an expression tree.
    /// </summary>

    public class ExprConstantNode : ExprNode
    {
        /// <summary>
        /// Returns the type that the node's evaluate method returns an instance of.
        /// </summary>
        /// <value>The type.</value>
        /// <returns> type returned when evaluated
        /// </returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override Type ReturnType
        {
            get
            {
                if (value == null)
                {
                    return null;
                }
                return TypeHelper.GetBoxedType(value.GetType());
            }
        }
		
		public override bool IsConstantResult
	    {
	        get { return true; }
	    }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private Object value;

        /// <summary> Ctor.</summary>
        /// <param name="value">is the constant's value.
        /// </param>
        public ExprConstantNode(Object value)
        {
            this.value = value;
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
            return value;
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
                if (value is String)
                {
                    return "\"" + value + "\"";
                }

                if (value == null)
                {
                    return "null";
                }

                return value.ToString();
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
            if (!(node is ExprConstantNode))
            {
                return false;
            }

            ExprConstantNode other = (ExprConstantNode)node;

            if ((other.value == null) && (this.value != null))
            {
                return false;
            }
            if ((other.value != null) && (this.value == null))
            {
                return false;
            }
            if ((other.value == null) && (this.value == null))
            {
                return true;
            }
            return other.value.Equals(this.value);
        }
    }
}
