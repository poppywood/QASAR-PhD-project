using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace net.esper.events.property
{
    /// <summary>
    /// Resolves properties using the Reflection to identify the properties and then
    /// using dynamic property generation.
    /// </summary>
    
    public class DynamicPropertyResolver : PropertyResolver
    {
        static readonly PropertyDescriptorGenerator dynamicPropertyGenerator = new PropertyDescriptorGenerator();

        /// <summary>
        /// Gets the properties for a given type.
        /// </summary>
        /// <param name="type">Type on which properties are to be resolved</param>
        /// <returns></returns>

        public override IEnumerable<PropertyDescriptor> GetProperties(Type type)
        {
            foreach (PropertyInfo baseProperty in type.GetProperties())
            {
                PropertyDescriptor descriptor = dynamicPropertyGenerator.CreateDynamicPropertyDescriptor(baseProperty);
                yield return descriptor;
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
            return dynamicPropertyGenerator.CreateDynamicPropertyDescriptor(property, name);
        }

        /// <summary>
        /// Gets the property for the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override PropertyDescriptor GetPropertyFor(FieldInfo field, String name)
        {
            return dynamicPropertyGenerator.CreateDynamicPropertyDescriptor(field, name);
        }
    }
}
