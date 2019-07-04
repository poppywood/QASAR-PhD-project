///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
	/// <summary>
	/// Getter for a key property identified by a given key value, using vanilla reflection.
	/// </summary>
	
	public class KeyedPropertyGetter : EventPropertyGetter
	{
		private readonly IndexedPropertyDescriptor descriptor;
		private readonly Object key;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="key">is the key to supply as parameter to the mapped property getter</param>
		
		public KeyedPropertyGetter(IndexedPropertyDescriptor descriptor, Object key)
		{
			this.key = key;
			this.descriptor = descriptor;
		}

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
		public Object GetValue(EventBean obj)
		{
			Object underlying = obj.Underlying;
			
			try
			{
				return descriptor.GetValue( underlying, key ) ;
			}
			catch (InvalidCastException)
			{
				throw new PropertyAccessException("Mismatched getter instance to event bean type");
			}
			catch (TargetInvocationException e)
			{
				throw new PropertyAccessException(e);
			}
			catch (TargetException e)
			{
				throw new PropertyAccessException(e);
			}
			catch (UnauthorizedAccessException e)
			{
				throw new PropertyAccessException(e);
			}
			catch (ArgumentException e)
			{
				throw new PropertyAccessException(e);
			}
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return
				"KeyedMethodPropertyGetter " +
				" descriptor=" + descriptor +
				" key=" + key;
		}

        /// <summary>
        /// Returns true if the property exists, or false if the type does not have such a property.
        /// <para>
        /// Useful for dynamic properties of the syntax "property?" and the dynamic nested/indexed/mapped versions.
        /// Dynamic nested properties follow the syntax "property?.nested" which is equivalent to "property?.nested?".
        /// If any of the properties in the path of a dynamic nested property return null, the dynamic nested property
        /// does not exists and the method returns false.
        /// </para>
        /// 	<para>
        /// For non-dynamic properties, this method always returns true since a getter would not be available
        /// unless
        /// </para>
        /// </summary>
        /// <param name="eventBean">the event to check if the dynamic property exists</param>
        /// <returns>
        /// indictor whether the property exists, always true for non-dynamic (default) properties
        /// </returns>
        public bool IsExistsProperty(EventBean eventBean)
        {
            return true; // Property exists as the property is not dynamic (unchecked)
        } 
	}
}
