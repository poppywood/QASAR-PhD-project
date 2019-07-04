using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using CGLib;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Resolves properties using the Reflection to identify the properties and then
    /// using dynamic property generation.
    /// </summary>
    
    public class FastPropertyResolver : PropertyResolver
    {
        /// <summary>
        /// Gets the properties for a given type.
        /// </summary>
        /// <param name="type">Type on which properties are to be resolved</param>
        /// <returns></returns>

        public override IEnumerable<PropertyDescriptor> GetProperties(Type type)
        {
            foreach (PropertyInfo baseProperty in type.GetProperties())
            {
                FastClass fastClass = FastClass.Create(baseProperty.DeclaringType);
                FastProperty fastProp = fastClass.GetProperty(baseProperty);
                yield return new FastPropertyDescriptor(fastProp);
            }
        }

        /// <summary>
        /// Gets a property descriptor for the given property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override PropertyDescriptor GetPropertyFor(PropertyInfo property, String name)
        {
            FastClass fastClass = FastClass.Create(property.DeclaringType);
            FastProperty fastProp = fastClass.GetProperty(property);
            return new FastPropertyDescriptor(fastProp);
        }

        /// <summary>
        /// Gets the property for the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override PropertyDescriptor GetPropertyFor(FieldInfo field, String name)
        {
            FastClass fastClass = FastClass.Create(field.DeclaringType);
            FastField fastField = fastClass.GetField(field);
            return new FastFieldPropertyDescriptor(fastField);
        }
    }
}
