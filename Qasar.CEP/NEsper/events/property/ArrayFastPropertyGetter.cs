///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using CGLib;

using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Getter for an array property identified by a given index, using the CGLIB fast method.
    /// </summary>
	public class ArrayFastPropertyGetter : EventPropertyGetter
	{
	    private readonly FastMethod fastMethod;
	    private readonly int index;

	    /// <summary>Constructor.</summary>
	    /// <param name="fastMethod">
	    /// is the method to use to retrieve a value from the object
	    /// </param>
	    /// <param name="index">is tge index within the array to get the property from</param>
	    public ArrayFastPropertyGetter(FastMethod fastMethod, int index)
	    {
	        this.index = index;
	        this.fastMethod = fastMethod;

	        if (index < 0)
	        {
	            throw new ArgumentException("Invalid negative index value");
	        }
	    }

	    public Object GetValue(EventBean obj) 
	    {
	        Object underlying = obj.Underlying;

	        try
	        {
	            Array value = (Array) fastMethod.Invoke(underlying, null);
	            if (value.Length <= index)
	            {
	                return null;
	            }
	            return value.GetValue(index);
	        }
	        catch (InvalidCastException)
	        {
	            throw new PropertyAccessException("Mismatched getter instance to event bean type");
	        }
	        catch (TargetInvocationException e)
	        {
	            throw new PropertyAccessException(e);
	        }
	    }

	    public override String ToString()
	    {
	        return "ArrayFastPropertyGetter " +
	                " fastMethod=" + fastMethod +
	                " index=" + index;
	    }

	    public bool IsExistsProperty(EventBean eventBean)
	    {
	        return true; // Property exists as the property is not dynamic (unchecked)
	    }
	}
} // End of namespace
