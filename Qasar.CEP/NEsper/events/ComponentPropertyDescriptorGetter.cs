using System;
using System.ComponentModel;

namespace com.espertech.esper.events
{
    /// <summary>
    /// An EventPropertyGetter that uses the internal PropertyDescriptor
    /// provided by the ComponentModel.
    /// </summary>

	public class ComponentPropertyDescriptorGetter : EventPropertyGetter
	{
		private readonly PropertyDescriptor propDesc;
		
		/// <summary>Constructor.</summary>
		
        public ComponentPropertyDescriptorGetter(PropertyDescriptor propDesc)
		{
        	this.propDesc = propDesc;
		}

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified bean as the key.
        /// </summary>
        /// <value></value>

        public Object GetValue(EventBean eventBean)
        {
            Object underlying = eventBean.Underlying;

            try
            {
                return propDesc.GetValue(underlying);
            }
            catch (ArgumentException)
            {
                throw new PropertyAccessException("Mismatched getter instance to event bean type");
            }
            catch (UnauthorizedAccessException e)
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
        /// <para>
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
				"ComponentPropertyDescriptorGetter " +
				"propDesc=" + propDesc.ToString();
		}
	}
}
