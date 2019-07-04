using System;
using System.Reflection;

namespace com.espertech.esper.events
{
	/// <summary>
    /// Property getter for methods using vanilla reflection.
    /// </summary>
	
    public sealed class ReflectionPropMethodGetter : EventPropertyGetter
	{
		private readonly MethodInfo method;
		
		/// <summary> Constructor.</summary>
		/// <param name="method">is the regular reflection method to use to obtain values for a field.
		/// </param>
		public ReflectionPropMethodGetter(MethodInfo method)
		{
			this.method = method;
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
                return method.Invoke(underlying, (Object[])null);
            }
            catch (ArgumentException)
            {
                throw new PropertyAccessException("Mismatched getter instance to event bean type");
            }
            catch (UnauthorizedAccessException e)
            {
                throw new PropertyAccessException(e);
            }
            catch (TargetInvocationException e)
            {
                throw new PropertyAccessException(e);
            }
            catch (TargetException e)
            {
                throw new PropertyAccessException(e);
            }
            catch (Exception e)
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
			return "ReflectionPropMethodGetter " + "method=" + method.ToString();
		}
	}
}
