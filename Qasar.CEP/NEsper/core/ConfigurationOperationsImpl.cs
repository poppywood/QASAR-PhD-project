///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.util;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Provides runtime engine configuration operations.
	/// </summary>
	public class ConfigurationOperationsImpl : ConfigurationOperations
	{
	    private readonly EventAdapterService eventAdapterService;
	    private readonly EngineImportService engineImportService;
        private readonly VariableService variableService;
        private readonly EngineSettingsService engineSettingsService;
        private readonly ValueAddEventService valueAddEventService;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="eventAdapterService">is the event wrapper and type service</param>
        /// <param name="engineImportService">for imported aggregation functions and static functions</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <param name="engineSettingsService">some engine settings are writable</param>
        /// <param name="valueAddEventService">update event handling</param>
        public ConfigurationOperationsImpl(EventAdapterService eventAdapterService,
	                                       EngineImportService engineImportService,
	                                       VariableService variableService,
	                                       EngineSettingsService engineSettingsService,
	                                       ValueAddEventService valueAddEventService)
        {
	        this.eventAdapterService = eventAdapterService;
	        this.engineImportService = engineImportService;
            this.variableService = variableService;
            this.engineSettingsService = engineSettingsService;
            this.valueAddEventService = valueAddEventService;
	    }

        /// <summary>
        /// Adds a namespace where event types reside in.
        /// <para>
        /// This setting allows an application to place all it's events into one or more namespaces
        /// and then declare these packages via this method. The engine attempts to resolve an event
        /// type alias to a class residing in each declared package.
        /// </para>
        /// 	<para>
        /// For example, in the statement "select * from MyEvent" the engine attempts to load class "namespace.MyEvent"
        /// and if successful, uses that class as the event type.
        /// </para>
        /// </summary>
        /// <param name="namespace">the namespace</param>
        public void AddEventTypeAutoAlias(String @namespace)
        {
            eventAdapterService.AddAutoAliasPackage(@namespace);
        }

        /// <summary>
        /// Adds a plug-in aggregation function given a function name and an aggregation class name.
        /// <p>
        /// The aggregation class must : the base class <see cref="com.espertech.esper.epl.agg.AggregationSupport"/>.
        /// </p>
        /// 	<p>
        /// The same function name cannot be added twice.
        /// </p>
        /// </summary>
        /// <param name="functionName">is the new aggregation function name</param>
        /// <param name="aggregationClassName">is the fully-qualified class name of the class implementing the aggregation function</param>
        /// <throws>
        /// ConfigurationException is thrown to indicate a problem adding aggregation function
        /// </throws>
	    public void AddPlugInAggregationFunction(String functionName, String aggregationClassName)
	    {
	        try
	        {
	            engineImportService.AddAggregation(functionName, aggregationClassName);
	        }
	        catch (EngineImportException e)
	        {
	            throw new ConfigurationException(e.Message, e);
	        }
	    }

        /// <summary>
        /// Adds a package or class to the list of automatically-imported classes and packages.
        /// <p>
        /// To import a single class offering a static method, simply supply the fully-qualified name of the class
        /// and use the syntax <code>classname.Methodname(...)</code>
        /// 	</p>
        /// 	<p>
        /// To import a whole package and use the <code>classname.Methodname(...)</code> syntax, specifiy a package
        /// with wildcard, such as <code>com.mycompany.staticlib.*</code>.
        /// </p>
        /// </summary>
        /// <param name="importName">is a fully-qualified class name or a package name with wildcard</param>
        /// <throws>
        /// ConfigurationException if incorrect package or class names are encountered
        /// </throws>
	    public void AddImport(String importName)
	    {
	        try
	        {
	            engineImportService.AddImport(importName);
	        }
	        catch (EngineImportException e)
	        {
	            throw new ConfigurationException(e.Message, e);
	        }
	    }

        /// <summary>
        /// Checks if an eventTypeAlias has already been registered for that alias name.
        /// </summary>
        /// <param name="eventTypeAlias">the alias name</param>
        /// <returns>true if already registered</returns>
        public bool IsEventTypeAliasExists(String eventTypeAlias)
        {
            return eventAdapterService.GetEventTypeByAlias(eventTypeAlias) != null;
        }

        /// <summary>
        /// Adds the event type alias.
        /// </summary>
        /// <param name="eventTypeAlias">The event type alias.</param>
        /// <param name="eventTypeName">Name of the event type.</param>
	    public void AddEventTypeAlias(String eventTypeAlias, String eventTypeName)
	    {
	        try
	        {
                eventAdapterService.AddBeanType(eventTypeAlias, eventTypeName, false);
	        }
	        catch (EventAdapterException t)
	        {
	            throw new ConfigurationException(t.Message, t);
	        }
	    }

        /// <summary>
        /// Add an alias for an event type represented by plain-old object events.
        /// <p>
        /// Allows a second alias to be added for the same type.
        /// Does not allow the same alias to be used for different types.
        /// </p>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias for the event type</param>
        /// <param name="eventType">is the event type for which to create the alias</param>
        /// <throws>
        /// ConfigurationException if the alias is already in used for a different type
        /// </throws>
	    public void AddEventTypeAlias(String eventTypeAlias, Type eventType)
	    {
	        try
	        {
	            eventAdapterService.AddBeanType(eventTypeAlias, eventType);
	        }
	        catch (EventAdapterException t)
	        {
	            throw new ConfigurationException(t.Message, t);
	        }
	    }

        /// <summary>
        /// Add an alias for an event type represented by plain-old object events,
        /// using the simple name of the type as the alias.
        /// <para>
        /// For example, if your class is "com.mycompany.MyEvent", then this method
        /// adds the alias "MyEvent" for the class.
        /// </para>
        /// 	<para>
        /// Allows a second alias to be added for the same type.
        /// Does not allow the same alias to be used for different types.
        /// </para>
        /// </summary>
        /// <param name="eventType">the event type for which to create the alias from the class simple name</param>
        /// <throws>ConfigurationException if the alias is already in used for a different type</throws>
        public void AddEventTypeAliasSimpleName(Type eventType)
        {
            try
            {
                eventAdapterService.AddBeanType(eventType.Name, eventType);
            }
            catch (EventAdapterException t)
            {
                throw new ConfigurationException(t.Message, t);
            }
        }

        /// <summary>
        /// Add an alias for an event type that represents DataMap events.
        /// <p>
        /// Allows a second alias to be added for the same type.
        /// Does not allow the same alias to be used for different types.
        /// </p>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias for the event type</param>
        /// <param name="typeMap">maps the name of each property in the Map event to the type
        /// (fully qualified classname) of its value in Map event instances.</param>
        /// <throws>
        /// ConfigurationException if the alias is already in used for a different type
        /// </throws>
	    public void AddEventTypeAlias(String eventTypeAlias, Properties typeMap)
	    {
	        Map<String, Type> types = CreatePropertyTypes(typeMap);
	        try
	        {
	            eventAdapterService.AddMapType(eventTypeAlias, types);
	        }
	        catch (EventAdapterException t)
	        {
	            throw new ConfigurationException(t.Message, t);
	        }
	    }

        /// <summary>
        /// Add an alias for an event type that represents DataMap events, taking a Map of
        /// event property and class name as a parameter.
        /// <p>
        /// This method is provided for convenience and is same in function to method
        /// taking a Properties object that contain fully qualified class name as values.
        /// </p>
        /// 	<p>
        /// Allows a second alias to be added for the same type.
        /// Does not allow the same alias to be used for different types.
        /// </p>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias for the event type</param>
        /// <param name="typeMap">maps the name of each property in the Map event to the type of its value in the Map object</param>
        /// <throws>
        /// ConfigurationException if the alias is already in used for a different type
        /// </throws>
	    public void AddEventTypeAlias(String eventTypeAlias, IDictionary<String, Type> typeMap)
	    {
	        try
	        {
                eventAdapterService.AddMapType(eventTypeAlias, new BaseMap<String, Type>(typeMap));
	        }
	        catch (EventAdapterException t)
	        {
	            throw new ConfigurationException(t.Message, t);
	        }
	    }

        /// <summary>
        /// Add an alias for an event type that represents DataMap events, and for which each property may
        /// itself be a Map of further properties,with unlimited nesting levels.
        /// <p>
        /// Each entry in the type mapping must contain the String property nameand either a Class or further DataMap value.
        /// </p>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias for the event type</param>
        /// <param name="typeMap">maps the name of each property in the Map event to the type(fully qualified classname) of its value in Map event instances.</param>
        /// <throws>ConfigurationException if the alias is already in used for a different type</throws>
        public void AddEventTypeAliasNestable(String eventTypeAlias, Map<String, Object> typeMap)
        {
            try {
                eventAdapterService.AddNestableMapType(eventTypeAlias, typeMap);
            } catch (EventAdapterException t) {
                throw new ConfigurationException(t.Message, t);
            }
        }

        /// <summary>
        /// Add an alias for an event type that represents nestable strong-typed Map events,
        /// taking a Map of event property and class name as a parameter.
        /// <para/>
        /// This method takes a Map of String property names and Object property type. Each
        /// Object property type can either be a Type to denote a built-in type or application object,
        /// or can itself also be a Map&lt;String, Object&gt; to describe a property that itself is a
        /// map of further properties.
        /// <para/>
        /// This method is provided for convenience and is same in function to method taking a Properties
        /// object that contain fully qualified class name as values.
        /// <para/>
        /// Allows a second alias to be added for the same type.Does not allow the same alias to be used
        /// for different types.
        /// </summary>
        /// <param name="eventTypeAlias">is the alias for the event type</param>
        /// <param name="typeMap">maps the name of each property in the Map event to the type of its value in the Map object</param>
        /// <throws>ConfigurationException if the alias is already in used for a different type</throws>
	    public void AddNestableEventTypeAlias(String eventTypeAlias, DataMap typeMap)
        {
            try
            {
                eventAdapterService.AddNestableMapType(eventTypeAlias, typeMap);
            }
            catch (EventAdapterException t)
            {
                throw new ConfigurationException(t.Message, t);
            }
        }

        /// <summary>
        /// Add an alias for an event type that represents org.w3c.dom.Node events.
        /// <p>
        /// Allows a second alias to be added for the same type.
        /// Does not allow the same alias to be used for different types.
        /// </p>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias for the event type</param>
        /// <param name="xmlDOMEventTypeDesc">descriptor containing property and mapping information for XML-DOM events</param>
        /// <throws>
        /// ConfigurationException if the alias is already in used for a different type
        /// </throws>
	    public void AddEventTypeAlias(String eventTypeAlias, ConfigurationEventTypeXMLDOM xmlDOMEventTypeDesc)
	    {
	        try
	        {
	            eventAdapterService.AddXMLDOMType(eventTypeAlias, xmlDOMEventTypeDesc);
	        }
	        catch (EventAdapterException t)
	        {
	            throw new ConfigurationException(t.Message, t);
	        }
	    }

        /// <summary>
        /// Creates the property types.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
	    private static Map<String, Type> CreatePropertyTypes(Properties properties)
	    {
	        Map<String, Type> propertyTypes = new HashMap<String, Type>();
            foreach( KeyValuePair<String,String> entry in properties)
	        {
	            string typename = entry.Value;
	            if (typename == "string")
	            {
	                typename = typeof(string).FullName;
	            }

	            // use the boxed type for primitives
	            string boxedTypeName = TypeHelper.GetBoxedTypeName(typename);

	            Type type;
	            try
	            {
	                type = Type.GetType(boxedTypeName, true);
	            }
	            catch (TypeLoadException ex)
	            {
	                throw new ConfigurationException("Unable to load class '" + boxedTypeName + "', class not found", ex);
	            }

	            propertyTypes[entry.Key] = type;
	        }
	        return propertyTypes;
	    }

        public void AddVariable(String variableName, Type type, Object initializationValue)
        {
            try
            {
                variableService.CreateNewVariable(variableName, type, initializationValue, null);
            }
            catch (VariableExistsException e)
            {
                throw new ConfigurationException("Error creating variable: " + e.Message, e);
            }
            catch (VariableTypeException e)
            {
                throw new ConfigurationException("Error creating variable: " + e.Message, e);
            }
        }

        public void AddPlugInEventType(String eventTypeAlias, IList<Uri> resolutionURIs, Object initializer)
        {
            try
            {
                eventAdapterService.AddPlugInEventType(eventTypeAlias, resolutionURIs, initializer);
            }
            catch (EventAdapterException e)
            {
                throw new ConfigurationException("Error adding plug-in event type: " + e.Message, e);
            }
        }

	    public IList<Uri> PlugInEventTypeAliasResolutionURIs
	    {
	        set { engineSettingsService.PlugInEventTypeResolutionURIs = value; }
	    }

	    public void AddRevisionEventType(String revisionEventTypeAlias, ConfigurationRevisionEventType revisionEventTypeConfig)
        {
            valueAddEventService.AddRevisionEventType(revisionEventTypeAlias, revisionEventTypeConfig, eventAdapterService);
        }

        public void AddVariantStream(String variantEventTypeAlias, ConfigurationVariantStream variantStreamConfig)
        {
            valueAddEventService.AddVariantStream(variantEventTypeAlias, variantStreamConfig, eventAdapterService);
        }
    }
} // End of namespace
