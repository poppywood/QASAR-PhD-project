using System;
using System.Collections;
using System.ComponentModel;

namespace com.espertech.esper.events.property
{
    internal class WrappedPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor m_baseProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="baseProperty">The base property.</param>
        public WrappedPropertyDescriptor( String name, PropertyDescriptor baseProperty )
            : base( name, null )
        {
            m_baseProperty = baseProperty;
        }

        /// <summary>
        /// Indicates whether the value of this property should be
        /// persisted.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Indicates whether or not the descriptor is readonly
        /// </summary>

        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the type of component this property is bound to.
        /// </summary>

        public override Type ComponentType
        {
            get { return m_baseProperty.ComponentType; }
        }

        /// <summary>
        /// Gets the return type of the property
        /// </summary>

        public override Type PropertyType
        {
            get { return m_baseProperty.PropertyType; }
        }

        /// <summary>
        /// Call the accessor method
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>

        public override Object GetValue(object component)
        {
            return m_baseProperty.GetValue(component);
        }

        /// <summary>
        /// Sets the value of the property
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>

        public override void SetValue(object component, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Can not override values with the simple accessor model
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>

        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Resets the value of the property
        /// </summary>
        /// <param name="component"></param>

        public override void ResetValue(object component)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns true if the objects are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public override bool Equals(object obj)
        {
            WrappedPropertyDescriptor temp = obj as WrappedPropertyDescriptor;
            if (temp != null)
            {
                return Equals(m_baseProperty, temp.m_baseProperty);
            }

            return false;
        }

        /// <summary>
        /// Returns a hahscode for the object.
        /// </summary>
        /// <returns></returns>

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
