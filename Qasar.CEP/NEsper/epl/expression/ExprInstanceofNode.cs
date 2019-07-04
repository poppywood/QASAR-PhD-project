///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.expression
{
	/// <summary>
	/// Represents the INSTANCEOF(a,b,...) function is an expression tree.
	/// </summary>
	public class ExprInstanceofNode : ExprNode
	{
	    private readonly IList<String> typeIdentifiers;

	    private Type[] types;
	    private readonly CopyOnWriteList<Pair<Type, bool>> resultCache = new CopyOnWriteList<Pair<Type, bool>>();

	    /// <summary>Ctor.</summary>
	    /// <param name="typeIdentifiers">is a list of type names to check type for</param>
	    public ExprInstanceofNode(IList<String> typeIdentifiers)
	    {
	        this.typeIdentifiers = typeIdentifiers;
	    }

        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
	        if (ChildNodes.Count != 1)
	        {
	            throw new ExprValidationException("Instanceof node must have 1 child expression node supplying the expression to test");
	        }
	        if ((typeIdentifiers == null) || (typeIdentifiers.Count == 0))
	        {
	            throw new ExprValidationException("Instanceof node must have 1 or more class identifiers to verify type against");
	        }

	        Set<Type> typeList = GetTypeSet(typeIdentifiers);
	        types = typeList.ToArray();
	    }

	    public override bool IsConstantResult
	    {
	    	get { return false; }
	    }

	    public override Type ReturnType
	    {
	    	get { return typeof(bool); }
	    }

	    public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
	    {
	    	Object result = ChildNodes[0].Evaluate(eventsPerStream, isNewData);
	        if (result == null)
	        {
	            return false;
	        }

	        Type resultType = result.GetType() ;
	        
	        // return cached value
	        foreach (Pair<Type, bool> pair in resultCache)
	        {
	        	if (pair.First == resultType)
	            {
	                return pair.Second;
	            }
	        }

	        return CheckAddType(resultType);
	    }

	    // Checks type and adds to cache
	    private bool CheckAddType(Type type)
	    {
	    	lock( this )
	    	{
		        // check again in synchronized block
		        foreach (Pair<Type, bool> pair in resultCache)
		        {
		            if (pair.First == type)
		            {
		                return pair.Second;
		            }
		        }
	
		        // get the types superclasses and interfaces, and their superclasses and interfaces
		        Set<Type> typesToCheck = new HashSet<Type>();
		        TypeHelper.GetBase(type, typesToCheck);
		        typesToCheck.Add(type);
	
		        // check type against each class
		        bool fits = false;
		        foreach (Type clazz in types)
		        {
		            if (typesToCheck.Contains(clazz))
		            {
		                fits = true;
		                break;
		            }
		        }
	
		        resultCache.Add(new Pair<Type, bool>(type, fits));
		        return fits;
	    	}
	    }

	    public override String ExpressionString
	    {
	    	get
	    	{
		        StringBuilder buffer = new StringBuilder();
		        buffer.Append("instanceof(");
		        buffer.Append(ChildNodes[0].ExpressionString);
		        buffer.Append(", ");
	
		        String delimiter = "";
		        for (int i = 0; i < typeIdentifiers.Count; i++)
		        {
		            buffer.Append(delimiter);
                    buffer.Append(typeIdentifiers[i]);
		            delimiter = ", ";
		        }
		        buffer.Append(')');
		        return buffer.ToString();
	    	}
	    }

	    public override bool EqualsNode(ExprNode node)
	    {
	        ExprInstanceofNode other = node as ExprInstanceofNode;
	        if ( other == null )
	        {
	        	return false ;
	        }

	        return CollectionHelper.AreEqual(typeIdentifiers, other.typeIdentifiers);
	    }

	    /// <summary>Returns the list of class names or types to check instance of.</summary>
	    /// <returns>class names</returns>
	    public IList<String> TypeIdentifiers
	    {
            get { return typeIdentifiers; }
	    }

	    private Set<Type> GetTypeSet(IEnumerable<String> _typeIdentifiers)
	    {
            Set<Type> typeList = new HashSet<Type>();
            foreach (String typeName in _typeIdentifiers)
	        {
	            Type type;

	            // try the primitive names including "string"
	            type = TypeHelper.GetPrimitiveTypeForName(typeName.Trim());
	            if (type != null)
	            {
	                typeList.Add(type);
	                typeList.Add(TypeHelper.GetBoxedType(type));
	                continue;
	            }

	            // try to look up the class, not a primitive type name
	            try
	            {
	                type = TypeHelper.ResolveType(typeName.Trim());
	            }
	            catch (TypeLoadException e)
	            {
	                throw new ExprValidationException("Type as listed in instanceof function by name '" + typeName + "' cannot be loaded", e);
	            }

	            // Add primitive and boxed types, or type itself if not built-in
	            typeList.Add(TypeHelper.GetPrimitiveType(type));
	            typeList.Add(TypeHelper.GetBoxedType(type));
	        }
	        return typeList;
	    }
	}
} // End of namespace
