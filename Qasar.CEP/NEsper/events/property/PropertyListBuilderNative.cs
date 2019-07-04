using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
	/// <summary>
    /// Implementation for a property list builder that considers native properties
    /// plus any explicitly configured props.
	/// </summary>

    public class PropertyListBuilderNative : PropertyListBuilder
	{
		private readonly ConfigurationEventTypeLegacy optionalLegacyConfig;

	    /// <summary>
	    /// Creates properties for us.
	    /// </summary>
	    private readonly PropertyResolver propertyResolver;
		
		/// <summary> Ctor.</summary>
		/// <param name="optionalLegacyConfig">configures legacy type, or null information
		/// has been supplied.
		/// </param>
		
        public PropertyListBuilderNative(ConfigurationEventTypeLegacy optionalLegacyConfig)
		{
		    this.optionalLegacyConfig = optionalLegacyConfig;

		    if (this.optionalLegacyConfig != null)
		    {
		        switch (optionalLegacyConfig.CodeGeneration)
		        {
		            case ConfigurationEventTypeLegacy.CodeGenerationEnum.DISABLED:
		                this.propertyResolver = new ClassicPropertyResolver();
		                break;
		            case ConfigurationEventTypeLegacy.CodeGenerationEnum.ENABLED:
		                this.propertyResolver = new FastPropertyResolver();
		                break;
		        }
		    }
		    else
		    {
                this.propertyResolver = new FastPropertyResolver();
		    }
		}

	    /// <summary>
        /// Introspect the type and deterime exposed event properties.
        /// </summary>
        /// <param name="type">type to introspect</param>
        /// <returns>list of event property descriptors</returns>
        public IList<EventPropertyDescriptor> AssessProperties(Type type)
        {
            using (PropertyResolver.Use(propertyResolver))
            {
                IList<EventPropertyDescriptor> result = PropertyHelper.GetProperties(type);
                if (optionalLegacyConfig != null)
                {
                    PropertyListBuilderExplicit.GetExplicitProperties(result, type, optionalLegacyConfig);
                }
                return result;
            }
        }
	}
}
