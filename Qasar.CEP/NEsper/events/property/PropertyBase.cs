using System;
using System.IO;
using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
	/// <summary>
    /// All properties have a property name and this is the abstract base class
    /// that serves up the property name.
    /// </summary>
	
    public abstract class PropertyBase : Property
	{
        /// <summary>
        /// Property name.
        /// </summary>
        internal String propertyNameAtomic;

        /// <summary> Returns the property name.</summary>
		/// <returns> name of property
		/// </returns>
        public virtual String PropertyNameAtomic
		{
			get { return propertyNameAtomic; }
		}
		
		/// <summary> Ctor.</summary>
		/// <param name="propertyName">is the name of the property
		/// </param>
		
        protected PropertyBase(String propertyName)
		{
            this.propertyNameAtomic = propertyName;
		}

        /// <summary>
        /// Gets the getter.
        /// </summary>
        /// <param name="param1">The param1.</param>
        /// <returns></returns>
        public abstract EventPropertyGetter GetGetter(BeanEventType param1);

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="param1">The param1.</param>
        /// <returns></returns>
		public abstract Type GetPropertyType(BeanEventType param1);

	    /// <summary>
	    /// Returns the property type for use with Map event representations.
	    /// </summary>
	    public abstract Type GetPropertyTypeMap(Map<String, Object> optionalMapPropTypes);

	    /// <summary>
	    /// Returns the getter-method for use with Map event representations.
	    /// </summary>
	    public abstract EventPropertyGetter GetGetterMap(Map<String, Object> optionalMapPropTypes);

        /// <summary>Write the EPL-representation of the property.</summary>
        /// <param name="writer">to write to</param>
        public virtual void ToPropertyEPL(StringWriter writer)
        {
        }
	}
}
