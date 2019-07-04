using System;
using System.Reflection;

using CGLib;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Property getter using CGLib's FastMethod instance.
    /// </summary>

    public class CGLibPropertyGetter : EventPropertyGetter
    {
        private readonly FastMethod fastMethod;
        private readonly FastProperty fastProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CGLibPropertyGetter"/> class.
        /// </summary>
        /// <param name="fastProperty">The fast property.</param>
        public CGLibPropertyGetter(FastProperty fastProperty)
        {
            this.fastProperty = fastProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CGLibPropertyGetter"/> class.
        /// </summary>
        /// <param name="fastMethod">The fast method.</param>
        public CGLibPropertyGetter(FastMethod fastMethod)
        {
            this.fastMethod = fastMethod;
        }

        /// <summary>
        /// Return the value for the property in the event object specified when the instance was obtained.
        /// Useful for fast access to event properties. Throws a PropertyAccessException if the getter instance
        /// doesn't match the EventType it was obtained from, and to indicate other property access problems.
        /// </summary>
        /// <param name="eventBean">is the event to get the value of a property from</param>
        /// <returns>value of property in event</returns>
        /// <throws>  PropertyAccessException to indicate that property access failed </throws>
        public Object GetValue(EventBean eventBean)
        {
            Object underlying = eventBean.Underlying;

            try
            {
                return
                    fastProperty != null
                        ? fastProperty.Get(underlying)
                        : fastMethod.Invoke(underlying);
            }
            catch (InvalidCastException)
            {
                throw new PropertyAccessException("Mismatched getter instance to event bean type");
            }
            catch (TargetInvocationException e)
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
            return "CGLibPropertyGetter " +
                   "fastMethod=" + fastMethod + "; " +
                   "fastProperty=" + fastProperty
                ;
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
