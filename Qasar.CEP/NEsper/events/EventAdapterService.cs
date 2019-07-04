using System;
using System.Collections.Generic;
using System.Xml;

using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.plugin;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Interface for a service to resolve event names to event type.
    /// </summary>

    public interface EventAdapterService
    {
        /// <summary>Adds an event type to the registery available for use, and originating outside as a non-adapter.</summary>
        /// <param name="alias">to add an event type under</param>
        /// <param name="eventType">the type to add</param>
        /// <throws>EventAdapterException if the alias is already in used by another type</throws>
        void AddTypeByAlias(String alias, EventType eventType);

        /// <summary>
        /// Return the event type for a given event name, or null if none is registered for that name.
        /// </summary>
        /// <param name="eventTypeAlias">is the event type alias name to return type for</param>
        /// <returns>
        /// event type for named event, or null if unknown/unnamed type
        /// </returns>
        EventType GetEventTypeByAlias(String eventTypeAlias);

        /// <summary>
        /// Add an event type with the given alias and a given set of properties.
        /// <para>
        /// If the alias already exists with the same event property information, returns the
        /// existing EventType instance.
        /// </para>
        /// <para>
        /// If the alias already exists with different event property information, throws an exception.
        /// </para>
        /// <para>
        /// If the alias does not already exists, adds the alias and constructs a new <see cref="com.espertech.esper.event.MapEventType"/>.
        /// </para>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias name for the event type</param>
        /// <param name="propertyTypes">is the names and types of event properties</param>
        /// <returns>event type is the type added</returns>
        /// <throws>
        /// EventAdapterException if alias already exists and doesn't match property type info
        /// </throws>
        EventType AddMapType(String eventTypeAlias, Map<String, Type> propertyTypes);

        /// <summary>
        /// Add an event type with the given alias and a given set of properties,where
        /// in properties may itself be Maps, nested and strongly-typed.
        /// <para>
        /// If the alias already exists with the same event property information, returns the
        /// existing EventType instance.
        /// </para>
        /// <para>
        /// If the alias already exists with different event property information, throws an exception.
        /// </para>
        /// <para>
        /// If the alias does not already exists, adds the alias and constructs a new
        /// <see cref="com.espertech.esper.event.MapEventType"/>.
        /// </para>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias name for the event type</param>
        /// <param name="propertyTypes">is the names and types of event properties</param>
        /// <returns>event type is the type added</returns>
        /// <throws>EventAdapterException if alias already exists and doesn't match property type info</throws>
        EventType AddNestableMapType(string eventTypeAlias, Map<string, object> propertyTypes);

        /// <summary>
        /// Add an event type with the given alias and the given underlying event type,
        /// as well as the additional given properties.
        /// </summary>
        /// <param name="eventTypeAlias">is the alias name for the event type</param>
        /// <param name="underlyingEventType">is the event type for the event type that this wrapper wraps</param>
        /// <param name="propertyTypes">is the names and types of any additional properties</param>
        /// <returns>eventType is the type added</returns>
        /// <throws>
        /// EventAdapterException if alias already exists and doesn't match this type's info
        /// </throws>

        EventType AddWrapperType(string eventTypeAlias, EventType underlyingEventType, Map<string, object> propertyTypes);

        /// <summary>
        /// Creates a new anonymous EventType instance for an event type that contains a map of name value pairs.
        /// The method accepts a Map that contains the property names as keys and Type objects as the values.
        /// The Type instances represent the property types.
        /// <para>
        /// New instances are createdStatement by this method on every invocation. Clients to this method need to
        /// cache the returned EventType instance to reuse EventType's for same-typed events.
        /// </para>
        /// </summary>
        /// <param name="propertyTypes">is a map of String to Type objects</param>
        /// <returns>
        /// EventType implementation for map field names and value types
        /// </returns>
        EventType CreateAnonymousMapType(Map<string, object> propertyTypes);

        /// <summary>
        /// Create an event wrapper bean from a set of event properties (name and value objectes) stored in a EDictionary.
        /// </summary>
        /// <param name="properties">is key-value pairs for the event properties</param>
        /// <param name="eventType">is the type metadata for any maps of that type</param>
        /// <returns>EventBean instance</returns>
        EventBean CreateMapFromValues(Map<string, object> properties, EventType eventType);

        /// <summary>
        /// Creata a wrapper around an event and some additional properties
        /// </summary>
        /// <param name="_event">is the wrapped event</param>
        /// <param name="properties">are the additional properties</param>
        /// <param name="eventType">os the type metadata for any wrappers of this type</param>
        /// <returns>wrapper event bean</returns>
        EventBean CreateWrapper(EventBean _event, Map<String, Object> properties, EventType eventType);

        /// <summary>
        /// Add an event type with the given alias and fully-qualified type name.
        /// <para>
        /// If the alias already exists with the same class name, returns the existing EventType instance.
        /// </para>
        /// 	<para>
        /// If the alias already exists with different class name, throws an exception.
        /// </para>
        /// 	<para>
        /// If the alias does not already exists, adds the alias and constructs a new <see cref="com.espertech.esper.event.BeanEventType"/>.
        /// </para>
        /// 	<para>
        /// Takes into account all event-type-auto-alias-package names supplied and
        /// attempts to resolve the class name via the packages if the direct resolution failed.
        /// </para>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias name for the event type</param>
        /// <param name="fullyQualTypeName">is the fully qualified class name</param>
        /// <param name="considerAutoAlias">whether auto-alias by namespaces should be considered</param>
        /// <returns>event type is the type added</returns>
        /// <throws>
        /// EventAdapterException if alias already exists and doesn't match class names
        /// </throws>
        EventType AddBeanType(String eventTypeAlias, String fullyQualTypeName, bool considerAutoAlias);

        /// <summary>
        /// Add an event type with the given alias and type.
        /// <para>
        /// If the alias already exists with the same Type, returns the existing EventType instance.
        /// </para>
        /// 	<para>
        /// If the alias already exists with different Type name, throws an exception.
        /// </para>
        /// 	<para>
        /// If the alias does not already exists, adds the alias and constructs a new <see cref="com.espertech.esper.event.BeanEventType"/>.
        /// </para>
        /// </summary>
        /// <param name="eventTypeAlias">is the alias name for the event type</param>
        /// <param name="clazz">is the fully qualified type</param>
        /// <returns>event type is the type added</returns>
        /// <throws>
        /// EventAdapterException if alias already exists and doesn't match class names
        /// </throws>
        EventType AddBeanType(String eventTypeAlias, Type clazz);


        /// <summary>
        /// Wrap the native event returning an <see cref="EventBean"/>.
        /// </summary>
        /// <param name="_event">to be wrapped</param>
        /// <returns>
        /// event bean wrapping native underlying event
        /// </returns>
        EventBean AdapterForBean(Object _event);

        /// <summary>
        /// Wrap the EDictionary-type event returning an <see cref="EventBean"/> using the event type alias name
        /// to identify the EventType that the event should carry.
        /// </summary>
        /// <param name="_event">to be wrapped</param>
        /// <param name="eventTypeAlias">alias for the event type of the event</param>
        /// <returns>
        /// event bean wrapping native underlying event
        /// </returns>
        /// <throws>
        /// EventAdapterException if the alias has not been declared, or the event cannot be wrapped using that
        /// alias's event type
        /// </throws>
        EventBean AdapterForMap(Map<String, Object> _event, String eventTypeAlias);

        /// <summary>
        /// Create an event type based on the original type passed in adding one or more properties.
        /// </summary>
        /// <param name="originalType">event type to add property to</param>
        /// <param name="fieldNames">names of properties</param>
        /// <param name="fieldTypes">types of properties</param>
        /// <returns>new event type</returns>
        EventType CreateAddToEventType(EventType originalType, IList<String> fieldNames, IList<Type> fieldTypes);

        /// <summary>
        /// Returns an adapter for the XML DOM document that exposes it's data as event properties for use in statements.
        /// </summary>
        /// <param name="node">is the node to wrap</param>
        /// <returns>event wrapper for document</returns>
        EventBean AdapterForDOM(XmlNode node);

        /// <summary>
        /// Creates an unnamed composite event type with event properties that are name-value pairs
        /// with values being other event types. Pattern statement generate events of such type.
        /// </summary>
        /// <param name="taggedEventTypes">is a map of name keys and event type values</param>
        /// <returns>
        /// event type representing a composite event
        /// </returns>
        EventType CreateAnonymousCompositeType(Map<String, Pair<EventType, String>> taggedEventTypes);

        /// <summary>
        /// Creates a wrapper for a composite event type. The wrapper wraps an event that
        /// consists of name-value pairs in which the values are other already-wrapped events.
        /// </summary>
        /// <param name="eventType">is the composite event type</param>
        /// <param name="taggedEvents">is the name-event map</param>
        /// <returns>wrapper for composite event</returns>
        EventBean AdapterForCompositeEvent(EventType eventType, Map<String, EventBean> taggedEvents);

        /// <summary>
        /// Create a new anonymous event type with the given underlying event type,
        /// as well as the additional given properties.
        /// </summary>
        /// <param name="underlyingEventType">is the event type for the event type that this wrapper wraps</param>
        /// <param name="propertyTypes">is the names and types of any additional properties</param>
        /// <returns>eventType is the type createdStatement</returns>
        /// <throws>
        /// EventAdapterException if alias already exists and doesn't match this type's info
        /// </throws>
        EventType CreateAnonymousWrapperType(EventType underlyingEventType, Map<string, object> propertyTypes);

        /// <summary>
        /// Adds an XML DOM event type.
        /// </summary>
        /// <param name="eventTypeAlias">is the alias to add the type for</param>
        /// <param name="configurationEventTypeXMLDOM">is the XML DOM config info</param>
        /// <returns>event type</returns>
        EventType AddXMLDOMType(String eventTypeAlias, ConfigurationEventTypeXMLDOM configurationEventTypeXMLDOM);

        /// <summary>
        /// Sets the configured legacy type information.
        /// </summary>
        Map<String, ConfigurationEventTypeLegacy> TypeLegacyConfigs { set; }

        /// <summary>
        /// Gets or sets the default resolution style for case-sentitivity.
        /// </summary>
        /// <value>The default property resolution style.</value>
        PropertyResolutionStyle DefaultPropertyResolutionStyle { get; set; }

        /// <summary>
        /// Gets or sets the alias resolver.
        /// </summary>
        /// <value>The alias resolver.</value>
        EPAliasResolver AliasResolver { get; set; }

        /// <summary>
        /// Adds a namespace where types reside in.
        /// </summary>
        /// <param name="namespace">A namespace where type reside.</param>
        void AddAutoAliasPackage(String @namespace);

        /// <summary>
        /// Returns a subset of the functionality of the service specific to creating object event types.
        /// </summary>
        /// <value>The bean event type factory.</value>
        /// <returns>bean event type factory</returns>
        BeanEventTypeFactory BeanEventTypeFactory { get; }

        /// <summary>
        /// Add a plug-in event representation.
        /// </summary>
        /// <param name="eventRepURI">URI is the unique identifier for the event representation</param>
        /// <param name="pluginEventRep">is the instance</param>
        void AddEventRepresentation(Uri eventRepURI, PlugInEventRepresentation pluginEventRep);

        /// <summary>
        /// Adds a plug-in event type.
        /// </summary>
        /// <param name="alias">is the name of the event type</param>
        /// <param name="resolutionURIs">is the URIs of plug-in event representations, or child URIs of such</param>
        /// <param name="initializer">is configs for the type</param>
        /// <returns>type</returns>
        EventType AddPlugInEventType(String alias, IList<Uri> resolutionURIs, Object initializer);

        /// <summary>
        /// Returns an event sender for a specific type, only generating events of that type.
        /// </summary>
        /// <param name="runtimeEventSender">the runtime handle for sending the wrapped type</param>
        /// <param name="eventTypeAlias">is the name of the event type to return the sender for</param>
        /// <returns>event sender that is static, single-type</returns>
        EventSender GetStaticTypeEventSender(EPRuntimeEventSender runtimeEventSender, String eventTypeAlias);

        /// <summary>
        /// Returns an event sender that dynamically decides what the event type for a given object is.
        /// </summary>
        /// <param name="runtimeEventSender">the runtime handle for sending the wrapped type</param>
        /// <param name="uri">is for plug-in event representations to provide implementations, if accepted, to make a wrapped event</param>
        /// <returns>
        /// event sender that is dynamic, multi-type based on multiple event bean factories provided byplug-in event representations
        /// </returns>
        EventSender GetDynamicTypeEventSender(EPRuntimeEventSender runtimeEventSender, IEnumerable<Uri> uri);
    }
}
