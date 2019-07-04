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
using com.espertech.esper.util;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Represents a mapped property or array property, ie. a 'value' property
    /// with read method getValue(int index) or a 'array' property via read
    /// method Array returning an array.
    /// </summary>

    public class MappedProperty : PropertyBase
    {
        private readonly String key;

        /// <summary> Returns the key value for mapped access.</summary>
        /// <returns> key value
        /// </returns>

		virtual public String Key
        {
            get { return key; }
        }

        /// <summary> Ctor.</summary>
        /// <param name="propertyName">is the property name of the mapped property
        /// </param>
        /// <param name="key">is the key value to access the mapped property
        /// </param>
        
		public MappedProperty( String propertyName, String key )
            : base(propertyName)
        {
            this.key = key;
        }

        /// <summary>
        /// Returns value getter for the property of an event of the given event type.
        /// </summary>
        /// <param name="eventType">is the type of event to make a getter for</param>
        /// <returns>fast property value getter for property</returns>
        public override EventPropertyGetter GetGetter(BeanEventType eventType)
        {
            EventPropertyDescriptor propertyDesc = eventType.GetMappedProperty(propertyNameAtomic);
            if (propertyDesc == null)
            {
                // property not found, is not a property
                return null;
            }

            return new KeyedPropertyGetter(propertyDesc.Descriptor as IndexedPropertyDescriptor, key);
        }

        /// <summary>
        /// Returns the property type.
        /// </summary>
        /// <param name="eventType">is the event type representing the object</param>
        /// <returns>property type class</returns>
        public override Type GetPropertyType(BeanEventType eventType)
        {
            EventPropertyDescriptor propertyDesc = eventType.GetMappedProperty(propertyNameAtomic);
            if (propertyDesc == null)
            {
                // property not found, is not a property
                return null;
            }
            return TypeHelper.GetBoxedType(propertyDesc.ReturnType);
        }
        

        /// <summary>
        /// Returns the property type for use with Map event representations.
        /// </summary>
        public override Type GetPropertyTypeMap(Map<String, Object> optionalMapPropTypes)
        {
            return null; // Mapped properties are not allowed in non-dynamic form in a map
        }

        /// <summary>
        /// Returns the getter-method for use with Map event representations.
        /// </summary>
        public override EventPropertyGetter GetGetterMap(Map<String, Object> optionalMapPropTypes)
        {
            return null; // Mapped properties are not allowed in non-dynamic form in a map
        }

        public override void ToPropertyEPL(StringWriter writer)
        {
            writer.Write(propertyNameAtomic);
            writer.Write("('");
            writer.Write(key);
            writer.Write("')");
        }
    }
}
