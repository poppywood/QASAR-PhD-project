using System;
using System.Reflection;

using CGLib;

using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
	/// <summary>
	/// Getter for an array property identified by a given index, using vanilla reflection.
	/// </summary>
	
	public class ArrayMethodPropertyGetter : EventPropertyGetter
	{
		private readonly FastMethod method;
		private readonly int index;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="method">is the method to use to retrieve a value from the object</param>
		/// <param name="index">is tge index within the array to get the property from</param>
		
		public ArrayMethodPropertyGetter(MethodInfo method, int index)
		{
			this.index = index;
		    this.method = FastClass.CreateMethod(method);
			
			if (index < 0)
			{
				throw new ArgumentException("Invalid negative index value");
			}
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
				Object value = method.Invoke(underlying, null);
				if (((Array) value).Length <= index)
				{
					return null;
				}

				return ((Array) value).GetValue(index);
			}
			catch (InvalidCastException)
			{
				throw new PropertyAccessException("Mismatched getter instance to event bean type");
			}
			catch (TargetInvocationException e)
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

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return
                "ArrayMethodPropertyGetter " +
                " method=" + method +
                " index=" + index;
		}
	}
}