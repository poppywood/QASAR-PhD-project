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

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Represents a dynamic mapped property of a given name.
    /// <para>
    /// Dynamic properties always exist, have an Object type and are resolved to a method during runtime.
    /// </para>
    /// </summary>
	public class DynamicMappedProperty : PropertyBase, DynamicProperty
	{
	    private readonly String key;

	    /// <summary>Ctor.</summary>
	    /// <param name="propertyName">is the property name</param>
	    /// <param name="key">is the mapped access key</param>
	    public DynamicMappedProperty(String propertyName, String key)
            : base(propertyName)
	    {
	        this.key = key;
	    }

	    public override EventPropertyGetter GetGetter(BeanEventType eventType)
	    {
	        return new DynamicMappedPropertyGetter(propertyNameAtomic, key);
	    }

        /// <summary>
        /// Returns the property type.
        /// </summary>
        /// <param name="eventType">is the event type representing the object</param>
        /// <returns>property type class</returns>
	    public override Type GetPropertyType(BeanEventType eventType)
	    {
	        return typeof (Object);
	    }

        /// <summary>
        /// Returns the property type for use with Map event representations.
        /// </summary>
        /// <value></value>
        public override Type GetPropertyTypeMap(Map<string, object> optionalMapPropTypes)
	    {
            return typeof(Object);
        }

        /// <summary>
        /// Returns the getter-method for use with Map event representations.
        /// </summary>
        /// <value></value>
        public override EventPropertyGetter GetGetterMap(Map<string, object> optionalMapPropTypes)
	    {
	        return new MapMappedPropertyGetter(this.PropertyNameAtomic, key);
	    }

        public override void ToPropertyEPL(StringWriter writer)
        {
            writer.Write(propertyNameAtomic);
            writer.Write("('");
            writer.Write(key);
            writer.Write("')");
            writer.Write('?');
        }
	}
} // End of namespace
