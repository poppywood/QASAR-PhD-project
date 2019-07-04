using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using com.espertech.esper.compat;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Resolves properties for a given type.
    /// </summary>

    abstract public class PropertyResolver
    {
        [ThreadStatic] private static PropertyResolver threadResolver;

        /// <summary>
        /// Gets the properties for a given type.
        /// </summary>
        /// <param name="type">Type on which properties are to be resolved</param>
        /// <returns></returns>

        public abstract IEnumerable<PropertyDescriptor> GetProperties(Type type);

        /// <summary>
        /// Gets a property descriptor for the given property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public virtual PropertyDescriptor GetPropertyFor(PropertyInfo property)
        {
            String name = property.Name;

            // Check to see if the name has been override
            Object[] attribs = property.GetCustomAttributes(typeof (OverrideNameAttribute), true);
            if ( attribs != null )
            {
                name = ((OverrideNameAttribute) attribs[0]).Name;
            }

            return GetPropertyFor(property, name);
        }

        /// <summary>
        /// Gets a property descriptor for the given property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="name">The property name.</param>
        /// <returns></returns>
        public abstract PropertyDescriptor GetPropertyFor(PropertyInfo property, String name);

        /// <summary>
        /// Gets the property for the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public virtual PropertyDescriptor GetPropertyFor(FieldInfo field)
        {
            String name = field.Name;

            // Check to see if the name has been override
            Object[] attribs = field.GetCustomAttributes(typeof(OverrideNameAttribute), true);
            if ((attribs != null) && (attribs.Length > 0))
            {
                name = ((OverrideNameAttribute)attribs[0]).Name;
            }

            return GetPropertyFor(field, name);
        }

        /// <summary>
        /// Gets the property for the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract PropertyDescriptor GetPropertyFor(FieldInfo field, String name);

        /// <summary>
        /// Gets the resolver currently in use on this thread.
        /// </summary>
        public static PropertyResolver Current
        {
            get
            {
                if ( threadResolver == null )
                {
                    // If no resolver hsa been allocated then we allocate our
                    // default.  This provides a fallback, but should not interfere
                    // with people who use it properly within a using block.
                    threadResolver = new FastPropertyResolver();
                }
                
                return threadResolver;
            }
        }

        /// <summary>
        /// Uses the specified resolver for the given scope.  The resolver becomes
        /// bound to the property resolver for the duration of the scope on the
        /// thread that it is being used on.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public static IDisposable Use( PropertyResolver resolver )
        {
            return new StackDisposable(resolver);   
        }

        /// <summary>
        /// Checks the override attribute and ensures that a proper descriptor is returned.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public static PropertyDescriptor CheckOverride( PropertyDescriptor descriptor )
        {
            if (descriptor != null)
            {
                OverrideNameAttribute attribute =
                    descriptor.Attributes[typeof(OverrideNameAttribute)] as OverrideNameAttribute;
                if (attribute != null)
                {
                    descriptor = new WrappedPropertyDescriptor(attribute.Name, descriptor);
                }
            }

            return descriptor;
        }

        /// <summary>
        /// Used internally to manage pseudo-allocations.
        /// </summary>

        private class StackDisposable : IDisposable
        {
            private readonly PropertyResolver parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="StackDisposable"/> class.
            /// </summary>
            /// <param name="resolver">The resolver.</param>
            public StackDisposable(PropertyResolver resolver)
            {
                parent = threadResolver;
                threadResolver = resolver;
            }

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                threadResolver = parent;
            }

            #endregion
        }
    }
}
