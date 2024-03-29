using System;
using System.Text;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.type;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents the bit-wise operators in an expression tree.
    /// </summary>

    public class ExprBitWiseNode : ExprNode
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
            get { return _resultType; }
        }

        /// <summary>
        /// Returns true if the expression node's evaluation value doesn't depend on any events data,
        /// as must be determined at validation time, which is bottom-up and therefore
        /// reliably allows each node to determine constant value.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true for constant evaluation value, false for non-constant evaluation value
        /// </returns>
		public override bool IsConstantResult
	    {
	        get { return false; }
	    }

        /// <summary>
        /// Gets the bitwise operator.
        /// </summary>
        /// <value>The bitwise operator.</value>
        public BitWiseOpEnum BitWiseOpEnum
        {
            get {return _bitWiseOpEnum;}
        }

		private readonly BitWiseOpEnum _bitWiseOpEnum;
        private BitWiseOpEnum.Computer _bitWiseOpEnumComputer;
        private Type _resultType;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="bitWiseOpEnum_">type of math</param>
        public ExprBitWiseNode(BitWiseOpEnum bitWiseOpEnum_)
        {
            _bitWiseOpEnum = bitWiseOpEnum_;
        }

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
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
            if (this.ChildNodes.Count != 2)
            {
                throw new ExprValidationException("BitWise node must have 2 child nodes");
            }

            foreach (ExprNode child in ChildNodes)
            {
                Type childType = child.ReturnType;
                if ((!TypeHelper.IsBoolean(childType)) && (!TypeHelper.IsNumeric(childType)))
                {
                    throw new ExprValidationException("Invalid datatype for bitwise " + childType.FullName + " is not allowed");
                }
            }

            // Determine result type, set up compute function
            Type childTypeOne = this.ChildNodes[0].ReturnType;
            Type childTypeTwo = this.ChildNodes[1].ReturnType;
            if ((TypeHelper.IsFloatingPointClass(childTypeOne)) || (TypeHelper.IsFloatingPointClass(childTypeTwo)))
            {
                throw new ExprValidationException("Invalid type for bitwise " + _bitWiseOpEnum.ComputeDescription + " operator");
            }
            else
            {
                Type childBoxedTypeOne = TypeHelper.GetBoxedType(childTypeOne);
                Type childBoxedTypeTwo = TypeHelper.GetBoxedType(childTypeTwo);

                if (childBoxedTypeOne == childBoxedTypeTwo)
                {
                    _resultType = childBoxedTypeOne;
                    _bitWiseOpEnumComputer = _bitWiseOpEnum.GetComputer(_resultType);
                }
                else
                {
                    throw new ExprValidationException("Both nodes muts be of the same type for bitwise " + _bitWiseOpEnum.ComputeDescription + " operator");
                }
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
            Object valueChildOne = this.ChildNodes[0].Evaluate(eventsPerStream, isNewData);
            Object valueChildTwo = this.ChildNodes[1].Evaluate(eventsPerStream, isNewData);

            if ((valueChildOne == null) || (valueChildTwo == null))
            {
                return null;
            }

            // bitWiseOpEnumComputer is initialized by validation
            return _bitWiseOpEnumComputer(valueChildOne, valueChildTwo);
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
            if (!(node is ExprBitWiseNode))
            {
                return false;
            }

            ExprBitWiseNode other = (ExprBitWiseNode)node;

            if (other._bitWiseOpEnum != _bitWiseOpEnum)
            {
                return false;
            }

            return true;
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

                buffer.Append(ChildNodes[0].ExpressionString);
                buffer.Append(_bitWiseOpEnum.ComputeDescription);
                buffer.Append(ChildNodes[1].ExpressionString);

                buffer.Append(")");
                return buffer.ToString();
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
