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
    /// Getter for a key property identified by a given key value, using vanilla reflection.
    /// </summary>
    public class KeyedMethodPropertyGetter : EventPropertyGetter
	{
	    private readonly FastMethod method;
	    private readonly Object key;

	    /// <summary>Constructor.</summary>
	    /// <param name="method">
	    /// is the method to use to retrieve a value from the object.
	    /// </param>
	    /// <param name="key">
	    /// is the key to supply as parameter to the mapped property getter
	    /// </param>
	    public KeyedMethodPropertyGetter(MethodInfo method, Object key)
	    {
	        this.key = key;
	        this.method = FastClass.CreateMethod(method);
	    }

	    public Object GetValue(EventBean obj)
	    {
	        Object underlying = obj.Underlying;

	        try
	        {
	            return method.Invoke(underlying, new Object[] {key});
	        }
	        catch (InvalidCastException)
	        {
	            throw new PropertyAccessException("Mismatched getter instance to event bean type");
	        }
	        catch (TargetInvocationException e)
	        {
	            throw new PropertyAccessException(e);
	        }
	        catch (ArgumentException e)
	        {
	            throw new PropertyAccessException(e);
	        }
	    }

        public override String ToString()
	    {
	        return "KeyedMethodPropertyGetter " +
	                " method=" + method.ToString() +
	                " key=" + key;
	    }

	    public bool IsExistsProperty(EventBean eventBean)
	    {
	        return true; // Property exists as the property is not dynamic (unchecked)
	    }
	}
} // End of namespace
