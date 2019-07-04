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
	/// Represents an indexed property or array property, ie. an 'value' property with read method getValue(int index)
	/// or a 'array' property via read method Array returning an array.
	/// </summary>

	public class IndexedProperty : PropertyBase
	{
		private readonly int index;

		/// <summary> Returns index for indexed access.</summary>
		/// <returns> index value
		/// </returns>

		virtual public int Index
		{
			get { return index; }
		}

		/// <summary> Ctor.</summary>
		/// <param name="propertyName">is the property name
		/// </param>
		/// <param name="index">is the index to use to access the property value
		/// </param>

		public IndexedProperty( String propertyName, int index )
			: base( propertyName )
		{
			this.index = index;
		}

		/// <summary>
		/// Returns value getter for the property of an event of the given event type.
		/// </summary>
		/// <param name="eventType">is the type of event to make a getter for</param>
		/// <returns>fast property value getter for property</returns>
		
		public override EventPropertyGetter GetGetter( BeanEventType eventType )
		{
			EventPropertyDescriptor propertyDesc = eventType.GetIndexedProperty( propertyNameAtomic );
			if ( propertyDesc != null )
			{
				return new KeyedPropertyGetter( propertyDesc.Descriptor as IndexedPropertyDescriptor, index );
			}

			// Try the array as a simple property
            propertyDesc = eventType.GetSimpleProperty(propertyNameAtomic);
			if ( propertyDesc == null )
			{
				return null;
			}

			Type returnType = propertyDesc.ReturnType;
			if ( returnType.IsArray )
			{
				return new ArrayPropertyGetter( propertyDesc.Descriptor, index ) ;
			}

			return null;
		}

        /// <summary>
        /// Returns the property type.
        /// </summary>
        /// <param name="eventType">is the event type representing the Object</param>
        /// <returns>property type class</returns>
		public override Type GetPropertyType( BeanEventType eventType )
		{
            EventPropertyDescriptor descriptor = eventType.GetIndexedProperty(propertyNameAtomic);
			if ( descriptor != null )
			{
				return descriptor.ReturnType;
			}

			// Check if this is an method returning array which is a type of simple property
            descriptor = eventType.GetSimpleProperty(propertyNameAtomic);
			if ( descriptor == null )
			{
				return null;
			}

			Type returnType = descriptor.ReturnType;
			if ( returnType.IsArray )
			{
			    return TypeHelper.GetBoxedType(returnType.GetElementType());
			}
			return null;
		}

        public override Type GetPropertyTypeMap(Map<string,object> optionalMapPropTypes)
        {
            return null;  // indexed properties are not allowed in non-dynamic form in a map
        }

        public override EventPropertyGetter GetGetterMap(Map<string, object> optionalMapPropTypes)
        {
            return null;  // indexed properties are not allowed in non-dynamic form in a map
        }

        public override void ToPropertyEPL(StringWriter writer)
        {
            writer.Write(propertyNameAtomic);
            writer.Write("[");
            writer.Write(index.ToString());
            writer.Write("]");
        }

	}
}
