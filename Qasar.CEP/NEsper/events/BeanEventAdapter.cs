using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.util;

namespace com.espertech.esper.events
{
    /// <summary>
    /// A factory for <see cref="BeanEventType"/> instances based on type information
    /// and using configured settings for 
    /// </summary>
	public class BeanEventAdapter : BeanEventTypeFactory
	{
		private readonly Map<Type, BeanEventType> typesPerBean;
		private readonly Map<String, ConfigurationEventTypeLegacy> typeToLegacyConfigs;
		private readonly MonitorLock typesPerBeanLock ;

        /// <summary>Default property resolution style.</summary>
        protected PropertyResolutionStyle defaultPropertyResolutionStyle;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeanEventAdapter"/> class.
        /// </summary>
        /// <param name="typesPerBean">The types per bean.</param>
        public BeanEventAdapter(Map<Type, BeanEventType> typesPerBean)
        {
            this.typesPerBean = typesPerBean;
            this.typesPerBeanLock = new MonitorLock();
            this.typeToLegacyConfigs = new HashMap<String, ConfigurationEventTypeLegacy>();
            this.defaultPropertyResolutionStyle = PropertyResolutionStyleHelper.DefaultPropertyResolutionStyle;
        }

        /// <summary>
        /// Gets or sets the property resolution style.
        /// </summary>
        /// <value>The property resolution style.</value>

        public virtual PropertyResolutionStyle DefaultPropertyResolutionStyle
        {
            get { return defaultPropertyResolutionStyle; }
            set { defaultPropertyResolutionStyle = value; }
        }

		/// <summary>
		/// Sets the additional mappings for legacy types.
		/// </summary>
		
		public Map<String, ConfigurationEventTypeLegacy> TypeToLegacyConfigs
		{
            get
            {
                throw new NotSupportedException();
            }
			set
			{
				typeToLegacyConfigs.PutAll( value ) ;
			}
		}

        /// <summary>
        /// Creates a new EventType object for a specified type if this is the first time
        /// the type has been seen. Else uses a cached EventType instance, i.e. client types
        /// do not need to cache.
        /// </summary>
        /// <param name="alias">is the alias</param>
        /// <param name="type">the type of the object.</param>
        /// <returns>EventType implementation for bean class</returns>
		
        public BeanEventType CreateBeanType(String alias, Type type)
		{
		    if (type == null)
		    {
		        throw new ArgumentException("Null value passed as type");
		    }

            BeanEventType eventType = null;

		    // not created yet, thread-safe create
		    using(typesPerBeanLock.Acquire())
		    {
		        eventType = typesPerBean.Get(type);
		        if (eventType != null)
		        {
		            return eventType;
		        }

		        // Check if we have a legacy type definition for this class
		        ConfigurationEventTypeLegacy legacyDef = typeToLegacyConfigs.Get(type.FullName);

                eventType = new BeanEventType(type, this, legacyDef, alias);
                typesPerBean[type] = eventType;
		    }

		    return eventType;
		}
	}
}
