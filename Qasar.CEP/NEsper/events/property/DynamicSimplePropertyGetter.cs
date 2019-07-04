///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Reflection;

using CGLib;

using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Getter for a dynamic property (syntax field.inner?), using vanilla reflection.
    /// </summary>
	public class DynamicSimplePropertyGetter : DynamicPropertyGetterBase, EventPropertyGetter
	{
        private readonly String propertyName;
	    private readonly String getterMethodName;
	    private readonly String isMethodName;

	    /// <summary>Ctor.</summary>
	    /// <param name="fieldName">the property name</param>
	    public DynamicSimplePropertyGetter(String fieldName)
	    {
	        this.propertyName = GetPropertyName(fieldName);
	        this.getterMethodName = GetGetterMethodName(fieldName);
	        this.isMethodName = GetIsMethodName(fieldName);
	    }

        protected override ValueGetter DetermineGetter(Type clazz)
        {
            MethodInfo method = FastClassUtil.GetGetMethodForProperty(clazz, propertyName);
            if (method == null)
            {
                method = clazz.GetMethod(getterMethodName);
                if (method == null)
                {
                    method = clazz.GetMethod(isMethodName);
                }
            }

            // Create the valueGetter; if there is a method, then invoke
            // the method.
            ValueGetter valueGetter = null;
            if (method != null)
            {
                FastMethod fastMethod = FastClass.CreateMethod(method);
                valueGetter = delegate(Object underlying)
                                  {
                                      return fastMethod.Invoke(underlying);
                                  };
            }

            return valueGetter;
        }

	    private static String GetIsMethodName(String propertyName)
	    {
	        StringWriter writer = new StringWriter();
	        writer.Write("Is");
	        writer.Write(Char.ToUpper(propertyName[0]));
	        writer.Write(propertyName.Substring(1));
	        return writer.ToString();
	    }
	}
} // End of namespace
