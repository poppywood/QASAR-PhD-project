using System;
using System.IO;
using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
	/// <summary>
    /// Interface for a property of an event of type BeanEventType. Properties are designed to
	/// handle the different types of properties for such events: indexed, mapped, simple, nested,
    /// or a combination of those.
	/// </summary>

    public interface Property
	{
        /// <summary>
        /// Returns the property type.
        /// </summary>
        /// <param name="eventType">is the event type representing the bean</param>
        /// <returns>property type class</returns>

        Type GetPropertyType(BeanEventType eventType);

        /// <summary>
        /// Returns value getter for the property of an event of the given event type.
        /// </summary>
        /// <param name="eventType">is the type of event to make a getter for</param>
        /// <returns>fast property value getter for property</returns>

        EventPropertyGetter GetGetter(BeanEventType eventType);

        /// <summary>
        /// Returns the property type for use with Map event representations.
        /// </summary>
        Type GetPropertyTypeMap(Map<String, Object> optionalMapPropTypes);

        /// <summary>
        /// Returns the getter-method for use with Map event representations.
        /// </summary>
        EventPropertyGetter GetGetterMap(Map<String, Object> optionalMapPropTypes);

        /// <summary>Write the EPL-representation of the property.</summary>
        /// <param name="writer">to write to</param>
        void ToPropertyEPL(StringWriter writer);
	}
}
