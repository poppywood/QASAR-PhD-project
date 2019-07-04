///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using com.espertech.esper.compat;
using com.espertech.esper.events;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Represents a dynamic simple property of a given name.
    /// <para>
    /// Dynamic properties always exist, have an Object type and are resolved to a method during runtime.
    /// </para>
    /// </summary>
	public class DynamicSimpleProperty : PropertyBase, DynamicProperty
	{
	    /// <summary>Ctor.</summary>
	    /// <param name="propertyName">is the property name</param>
	    public DynamicSimpleProperty(String propertyName)
            : base(propertyName)
	    {
	    }

	    public override EventPropertyGetter GetGetter(BeanEventType eventType)
	    {
            return new DynamicSimplePropertyGetter(propertyNameAtomic);
	    }

	    public override Type GetPropertyType(BeanEventType eventType)
	    {
	        return typeof(Object);
	    }

        /// <summary>
        /// Returns the property type for use with Map event representations.
        /// </summary>
        /// <value></value>
        public override Type GetPropertyTypeMap(Map<string,object> optionalMapPropTypes)
	    {
	        return typeof(Object);
	    }

        /// <summary>
        /// Returns the getter-method for use with Map event representations.
        /// </summary>
        /// <value></value>
        public override EventPropertyGetter GetGetterMap(Map<string, object> optionalMapPropTypes)
        {
            String lPropertyName = PropertyNameAtomic;
            return new ProxyEventPropertyGetter(
                delegate(EventBean eventBean) {
                    DataMap map = (DataMap) eventBean.Underlying;
                    return map.Get(lPropertyName);
                },
                delegate(EventBean eventBean) {
                    DataMap map = (DataMap) eventBean.Underlying;
                    return map.ContainsKey(lPropertyName);
                });
        }

        /// <summary>Write the EPL-representation of the property.</summary>
        /// <param name="writer">to write to</param>
        public override void ToPropertyEPL(StringWriter writer)
        {
            writer.Write(propertyNameAtomic);
        }
	}
} // End of namespace
