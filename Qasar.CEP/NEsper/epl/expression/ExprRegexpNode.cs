using System;
using System.Data.Common;
using System.Globalization;
using System.Text.RegularExpressions;

using com.espertech.esper.client;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents the regexp-clause in an expression tree.
    /// </summary>

    public class ExprRegexpNode : ExprNode
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
            get { return typeof(bool?); }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        /// Returns true if the expression node's evaluation value doesn't depend on any events data,
        /// as must be determined at validation time, which is bottom-up and therefore
        /// reliably allows each node to determine constant value.
        /// @return true for constant evaluation value, false for non-constant evaluation value
	    public override bool IsConstantResult
	    {
	        get { return false; }
	    } 
	
        private readonly bool isNot;

        private Regex pattern;
        private bool isNumericValue;
        private bool isConstantPattern;

        /// <summary> Ctor.</summary>
        /// <param name="not">is true if the it's a "not regexp" expression, of false for regular regexp
        /// </param>
        public ExprRegexpNode(bool not)
        {
            this.isNot = not;
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
            if (this.ChildNodes.Count != 2)
            {
                throw new ExprValidationException("The regexp operator requires 2 child expressions");
            }

            // check pattern child node
            ExprNode patternChildNode = this.ChildNodes[1];
            Type patternChildType = patternChildNode.ReturnType;
            if (patternChildType != typeof(String))
            {
                throw new ExprValidationException("The regexp operator requires a String-type pattern expression");
            }
            if (this.ChildNodes[1].IsConstantResult)
            {
                isConstantPattern = true;
            }

            // check eval child node - can be String or numeric
            Type evalChildType = this.ChildNodes[0].ReturnType;
            isNumericValue = TypeHelper.IsNumeric(evalChildType);
            if ((evalChildType != typeof(String)) && (!isNumericValue))
            {
                throw new ExprValidationException("The regexp operator requires a String or numeric type left-hand expression");
            }
        }

        /// <summary>
        /// Evaluate event tuple and return result.
        /// </summary>
        /// <param name="eventsPerStream">event tuple</param>
        /// <param name="isNewData"></param>
        /// <returns>
        /// evaluation result, a bool value for OR/AND-type evalution nodes.
        /// </returns>
        public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
        {
            if (pattern == null)
            {
                String patternText = (String)this.ChildNodes[1].Evaluate(eventsPerStream, isNewData);
                if (patternText == null)
                {
                    return null;
                }
                try
                {
                   	pattern = new Regex(String.Format( "^{0}$", patternText ));
                }
                catch (ArgumentException ex)
                {
                    throw new EPException("Error compiling regex pattern '" + patternText + "'", ex);
                }
            }
            else
            {
                if (!isConstantPattern)
                {
                    String patternText = (String)this.ChildNodes[1].Evaluate(eventsPerStream, isNewData);
                    if (patternText == null)
                    {
                        return null;
                    }
                    try
                    {
                    	pattern = new Regex(String.Format( "^{0}$", patternText ));
                    }
                    catch (ArgumentException ex)
                    {
                        throw new EPException("Error compiling regex pattern '" + patternText + "'", ex);
                    }
                }
            }

            Object evalValue = this.ChildNodes[0].Evaluate(eventsPerStream, isNewData);
            if (evalValue == null)
            {
                return null;
            }

            if (isNumericValue)
            {
                if (evalValue is double)
                {
                    double tempValue = (double)evalValue;
                    evalValue = tempValue.ToString("F");
                }
                else if (evalValue is float)
                {
                    float tempValue = (float)evalValue;
                    evalValue = tempValue.ToString("F");
                }
                else if (evalValue is decimal)
                {
                    decimal tempValue = (decimal)evalValue;
                    evalValue = tempValue.ToString("F");
                }
                else
                {
                    evalValue = evalValue.ToString();
                }
            }

            Boolean result = pattern.IsMatch((String) evalValue);

            if (isNot)
            {
                return !result;
            }

            return result;
        }

        /// <summary>
        /// Returns true if the objects are equal.
        /// </summary>
        /// <param name="node_">the node to compare against</param>
        /// <returns></returns>
        public override bool EqualsNode(ExprNode node_)
        {
            if (!(node_ is ExprRegexpNode))
            {
                return false;
            }

            ExprRegexpNode other = (ExprRegexpNode)node_;
            if (this.isNot != other.isNot)
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
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();

                buffer.Append(this.ChildNodes[0].ExpressionString);

                if (isNot)
                {
                    buffer.Append(" not");
                }
                buffer.Append(" regexp ");
                buffer.Append(this.ChildNodes[1].ExpressionString);

                return buffer.ToString();
            }
        }
    }
}
