///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using CGLib;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Provides method information for dynamic (unchecked) properties of each class
    /// for use in obtaining property values.
    /// </summary>
	public class DynamicPropertyDescriptor
	{
        private readonly Type clazz;
        private readonly ValueGetter getter;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="clazz">the class to match when looking for a method</param>
        /// <param name="getter">Gets values out of the underlying object.</param>
        public DynamicPropertyDescriptor(Type clazz, ValueGetter getter)
	    {
	        this.clazz = clazz;
            this.getter = getter;
	    }

	    /// <summary>Returns the class for the method.</summary>
	    /// <returns>class to match on</returns>
	    public Type Clazz
	    {
            get { return clazz; }
	    }

        /// <summary>
        /// Returns a delegate that can get the value out of the underlying object.
        /// </summary>
        /// <value>The getter.</value>
	    public ValueGetter Getter
	    {
            get { return getter; }
	    }
	}
} // End of namespace
