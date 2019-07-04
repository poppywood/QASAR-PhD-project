using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using com.espertech.esper.compat;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Resolves properties using the TypeDescriptor in System.ComponentModel.
    /// </summary>

    public class ClassicPropertyResolver : PropertyResolver
    {
        /// <summary>
        /// Gets the properties for a given type.
        /// </summary>
        /// <param name="type">Type on which properties are to be resolved</param>
        /// <returns></returns>

        public override IEnumerable<PropertyDescriptor> GetProperties(Type type)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(type))
            {
                yield return CheckOverride(descriptor);
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
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(property.DeclaringType);
            PropertyDescriptor descriptor = properties[property.Name];
            return CheckOverride(descriptor);
        }

        /// <summary>
        /// Gets the property for the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override PropertyDescriptor GetPropertyFor(FieldInfo field, String name)
        {
            return new SimpleFieldPropertyDescriptor(name, field);
        }
    }
}
