using System;
using System.ComponentModel;
using System.Reflection;

namespace com.espertech.esper.events.property
{
    abstract public class AbstractPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public AbstractPropertyDescriptor(string name) : base(name, null) { }

        /// <summary>
        /// Proxies the attributes.
        /// </summary>
        /// <param name="member">The member.</param>
        
        protected void InitializeAttributes( MemberInfo member )
        {
            Object[] objectArray = member.GetCustomAttributes(false);
            Attribute[] attribArray = new Attribute[objectArray.Length];
            Array.Copy(objectArray, attribArray, objectArray.Length);

            base.AttributeArray = attribArray;
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the property is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
            throw new NotSupportedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            throw new NotSupportedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
