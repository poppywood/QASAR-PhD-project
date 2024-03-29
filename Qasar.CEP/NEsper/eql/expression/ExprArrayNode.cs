///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using net.esper.eql.core;
using net.esper.events;
using net.esper.util;

namespace net.esper.eql.expression
{
	/// <summary>
    /// Represents an array in a filter expressiun tree.
    /// </summary>
	
    public class ExprArrayNode : ExprNode
	{
	    private Type coercionType;
	    private bool mustCoerce;
	    private int length;
	    private Object constantResult;

	    /// <summary>Ctor.</summary>
	    public ExprArrayNode()
	    {
	    }

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate"></param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
	    public override void Validate(StreamTypeService streamTypeService, MethodResolutionService methodResolutionService, ViewResourceDelegate viewResourceDelegate)
	    {
	        length = this.ChildNodes.Count;

	        // Can be an empty array with no content
	        if (this.ChildNodes.Count == 0)
	        {
	            coercionType = typeof(Object);
	            constantResult = new Object[0];
	            return;
	        }

	        List<Type> comparedTypes = new List<Type>();
	        for (int i = 0; i < length; i++)
	        {
	        	comparedTypes.Add(this.ChildNodes[i].ReturnType);
	        }

	        // Determine common denominator type
	        try {
	            coercionType = TypeHelper.GetCommonCoercionType(comparedTypes.ToArray());

	            // Determine if we need to coerce numbers when one type doesn't match any other type
	            if (TypeHelper.IsNumeric(coercionType))
	            {
	                mustCoerce = false;
	                foreach (Type comparedType in comparedTypes)
	                {
	                    if (comparedType != coercionType)
	                    {
	                        mustCoerce = true;
	                    }
	                }
	            }
	        }
	        catch (CoercionException ex)
	        {
	            // expected, such as mixing String and int values, or Java classes (not boxed) and primitives
	            // use Object[] in such cases
	        }
	        if (coercionType == null)
	        {
	            coercionType = typeof(Object);
	        }

	        // Determine if we are dealing with constants only
	        Object[] results = new Object[length];
	        int index = 0;
	        foreach (ExprNode child in this.ChildNodes)
	        {
	            if (!child.IsConstantResult)
	            {
	                results = null;  // not using a constant result
	                break;
	            }
	            results[index++] = child.Evaluate(null, false);
	        }

	        // Copy constants into array and coerce, if required
	        if (results != null)
	        {
                Array tempArray = Array.CreateInstance(coercionType, length);
	            constantResult = tempArray;
	            for (int i = 0; i < length; i++)
	            {
	                if (mustCoerce)
	                {
	                    Object boxed = results[i];
	                    if (boxed != null)
	                    {
	                        Object coercedResult = TypeHelper.CoerceBoxed(boxed, coercionType);
	                        tempArray.SetValue(coercedResult, i);
	                    }
	                }
	                else
	                {
	                    tempArray.SetValue(results[i], i);
	                }
	            }
	        }
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
	    	get { return constantResult != null; }
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
	    	get
	    	{
                //return Array.CreateInstance(coercionType, 0).GetType();
                return Type.GetType(coercionType.FullName + "[]");
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
	        if (constantResult != null)
	        {
	            return constantResult;
	        }

	        Array array = Array.CreateInstance(coercionType, length);

	        if (length == 0)
	        {
	            return array;
	        }

	        int index = 0;
	        foreach (ExprNode child in this.ChildNodes)
	        {
	            Object result = child.Evaluate(eventsPerStream, isNewData);
	            if (result != null)
	            {
	                if (mustCoerce)
	                {
	                    Object coercedResult = TypeHelper.CoerceBoxed(result, coercionType);
	                    array.SetValue(coercedResult, index);
	                }
	                else
	                {
                        array.SetValue(result, index);
	                }
	            }
	            index++;
	        }

	        return array;
	    }

        /// <summary>
        /// Returns the expression node rendered as a string.
        /// </summary>
        /// <value></value>
        /// <returns> string rendering of expression
        /// </returns>
	    public override string ExpressionString
	    {
	    	get
	    	{
		        StringBuilder buffer = new StringBuilder();
		        String delimiter = "";
	
		        buffer.Append("{");
		        foreach (ExprNode expr in this.ChildNodes)
		        {
		            buffer.Append(delimiter);
		            buffer.Append(expr.ExpressionString);
		            delimiter = ",";
		        }
	
		        buffer.Append('}');
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
	        if (!(node is ExprArrayNode))
	        {
	            return false;
	        }
	        return true;
	    }
	}
} // End of namespace
