using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.type;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents a simple Math (+/-/divide/*) in a filter expression tree.
    /// </summary>

    public class ExprMathNode : ExprNode
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
            get { return resultType; }
        }

 	    public override bool IsConstantResult
	    {
	        get { return false; }
	    }

        /// <summary>
        /// Gets the math arithetic type enum.
        /// </summary>
        /// <value>The math arith type enum.</value>
        public MathArithTypeEnum MathArithTypeEnum
        {
            get { return mathArithTypeEnum; }
        }
		
		private readonly MathArithTypeEnum mathArithTypeEnum;

        private MathArithTypeEnum.Computer arithTypeEnumComputer;
        private Type resultType;

        /// <summary> Ctor.</summary>
        /// <param name="mathArithTypeEnum">type of math
        /// </param>
        public ExprMathNode(MathArithTypeEnum mathArithTypeEnum)
        {
            this.mathArithTypeEnum = mathArithTypeEnum;
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
            if (this.ChildNodes.Count != 2)
            {
                throw new ExprValidationException("Arithmatic node must have 2 child nodes");
            }

            foreach (ExprNode child in this.ChildNodes)
            {
                Type childType = child.ReturnType;
                if (!TypeHelper.IsNumeric(childType))
                {
                    throw new ExprValidationException("Implicit conversion from datatype '" + childType.FullName + "' to numeric is not allowed");
                }
            }

            // Determine result type, set up compute function
            Type childTypeOne = this.ChildNodes[0].ReturnType;
            Type childTypeTwo = this.ChildNodes[1].ReturnType;

            if (childTypeOne.Equals(childTypeTwo))
            {
                resultType = TypeHelper.GetBoxedType(childTypeTwo);
            }
            else
            {
                resultType = TypeHelper.GetArithmaticCoercionType(childTypeOne, childTypeTwo);
            }

            arithTypeEnumComputer = mathArithTypeEnum.GetComputer(resultType);
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
            IList<ExprNode> children = this.ChildNodes;
            Object valueChildOne = children[0].Evaluate(eventsPerStream, isNewData);
            Object valueChildTwo = children[1].Evaluate(eventsPerStream, isNewData);

            if ((valueChildOne == null) || (valueChildTwo == null))
            {
                return null;
            }

            // arithTypeEnumComputer is initialized by validation
            return arithTypeEnumComputer(valueChildOne, valueChildTwo);
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
                buffer.Append("(");

                buffer.Append(this.ChildNodes[0].ExpressionString);
                buffer.Append(mathArithTypeEnum.ExpressionText);
                buffer.Append(this.ChildNodes[1].ExpressionString);

                buffer.Append(")");
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
            if (!(node is ExprMathNode))
            {
                return false;
            }

            ExprMathNode other = (ExprMathNode)node;

            if (other.mathArithTypeEnum != this.mathArithTypeEnum)
            {
                return false;
            }

            return true;
        }
    }
}
