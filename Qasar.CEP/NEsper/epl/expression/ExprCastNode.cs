///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
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
	/// Represents the CAST(expression, type) function is an expression tree.
	/// </summary>
	public class ExprCastNode : ExprNode
	{
	    private readonly String typeIdentifier;
        private CastConverter castConverter;
        private Type targetType;

	    /// <summary>Ctor.</summary>
	    /// <param name="typeIdentifier">the the name of the type to cast to</param>
	    public ExprCastNode(String typeIdentifier)
	    {
	        this.typeIdentifier = typeIdentifier.Trim();
	    }

	    /// <summary>Returns the name of the type of cast to.</summary>
	    /// <returns>type name</returns>
	    public String ClassIdentifier
	    {
	    	get { return typeIdentifier; }
	    }

        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
	    {
	        if (ChildNodes.Count != 1)
	        {
	            throw new ExprValidationException("Cast function node must have exactly 1 child node");
	        }

	        // try the primitive names including "string"
	        targetType = TypeHelper.GetPrimitiveTypeForName(typeIdentifier);
            if (targetType == null) {
                try {
                    targetType = TypeHelper.ResolveType(typeIdentifier, true);
                } catch (Exception e) {
                    throw new ExprValidationException(
                        "Type as listed in cast function by name '" + typeIdentifier + "' cannot be loaded", e);
                }
            }

            targetType = TypeHelper.GetBoxedType(targetType);
            castConverter = CastHelper.GetCastConverter(targetType);
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
        /// Returns the type that the node's evaluate method returns an instance of.
        /// </summary>
        /// <value>The type.</value>
        /// <returns> type returned when evaluated
        /// </returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>
	    public override Type ReturnType
	    {
	    	get { return targetType; }
	    }

	    private delegate Object QuickCastDelegate(Object sourceObj);

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
	    	Object result = ChildNodes[0].Evaluate(eventsPerStream, isNewData);
	        if (result == null)
	        {
	            return null;
	        }

            if ( castConverter != null )
            {
                Object castResult = castConverter.Invoke(result);
                if ( castResult != null )
                {
                    return castResult;
                }
            }

            // Simple dynamic-cast
            if (targetType.IsAssignableFrom(result.GetType()))
            {
                if (!targetType.IsValueType)
                {
                    return result;
                }
            }

            try
            {
                result = Convert.ChangeType(result, targetType);
                return result;
            }
            catch (FormatException) // Occurs when numeric types parse strings
            {
                return null;
            }
            catch (InvalidCastException)
            {
                return null;
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
		        buffer.Append("cast(");
		        buffer.Append(ChildNodes[0].ExpressionString);
		        buffer.Append(", ");
		        buffer.Append(typeIdentifier);
		        buffer.Append(')');
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
	        ExprCastNode other = node as ExprCastNode;
	        if ( other == null )
	        {
	        	return false;
	        }
	        return Equals(other.typeIdentifier, typeIdentifier);
	    }
    }
} // End of namespace
