using System;
using System.Collections.Generic;
using System.Xml;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.client;
using com.espertech.esper.core;
using com.espertech.esper.events;
using com.espertech.esper.events.xml;
using com.espertech.esper.plugin;
using com.espertech.esper.util;

using log4net;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Implementation for resolving event name to event type.
    /// <para>
    /// The implementation assigned a unique identifier to each event type.
    /// For Class-based event types, only one EventType instance and one event type id exists for the same class.
    /// </para>
    /// <para>
    /// Alias names must be unique, that is an alias name must resolve to a single event type.
    /// </para>
    /// <para>
    /// Each event type can have multiple aliases defined for it. For example, expressions such as
    /// "select * from A" and "select * from B"
    /// in which A and B are aliases for the same class X the select clauses each fireStatementStopped for events of type X.
    /// In summary, aliases A and B point to the same underlying event type and therefore event type id.
    /// </para>
    /// </summary>

    public class EventAdapterServiceImpl : EventAdapterService
    {
        private readonly Map<Type, BeanEventType> typesPerBean;
        private readonly Map<String, EventType> aliasToTypeMap;
        private readonly Map<String, PlugInEventTypeHandler> aliasToHandlerMap;
        private readonly BeanEventAdapter beanEventAdapter;
        private readonly Map<String, EventType> xmldomRootElementNames;
        private readonly MonitorLock syncLock;
        private readonly LinkedHashSet<String> namespaces;
        private EPAliasResolver aliasResolver;
        private readonly Map<Uri, PlugInEventRepresentation> plugInRepresentations;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAdapterServiceImpl"/> class.
        /// </summary>
        public EventAdapterServiceImpl()
        {
            syncLock = new MonitorLock();
            aliasToTypeMap = new HashMap<String, EventType>();
            xmldomRootElementNames = new HashMap<String, EventType>();
            aliasToHandlerMap = new HashMap<String, PlugInEventTypeHandler>();

            // Share the mapping of class to type with the type creation for thread safety
            //typesPerBean = new ConcurrentHashDictionary<Class, BeanEventType>();
            typesPerBean = new HashMap<Type, BeanEventType>();
            beanEventAdapter = new BeanEventAdapter(typesPerBean);

            namespaces = new LinkedHashSet<String>();

            aliasResolver = null;

            plugInRepresentations = new HashMap<Uri, PlugInEventRepresentation>();
        }

        public void AddTypeByAlias(String alias, EventType eventType)
        {
            lock (this) {
                if (aliasToTypeMap.ContainsKey(alias)) {
                    throw new EventAdapterException("Alias by name '" + alias + "' already exists");
                }
                aliasToTypeMap.Put(alias, eventType);
            }
        }

        public void AddEventRepresentation(Uri eventRepURI, PlugInEventRepresentation pluginEventRep)
        {
            if (plugInRepresentations.ContainsKey(eventRepURI)) {
                throw new EventAdapterException("Plug-in event representation URI by name " + eventRepURI +
                                                " already exists");
            }
            plugInRepresentations.Put(eventRepURI, pluginEventRep);
        }

        public EventType AddPlugInEventType(String alias, IList<Uri> resolutionURIs, Object initializer)
        {
            if (aliasToTypeMap.ContainsKey(alias)) {
                throw new EventAdapterException("Event type named '" + alias +
                                                "' has already been declared");
            }

            PlugInEventRepresentation handlingFactory = null;
            Uri handledEventTypeURI = null;

            if ((resolutionURIs == null) ||
                (resolutionURIs.Count == 0)) {
                throw new EventAdapterException("Event type named '" + alias + "' could not be created as" +
                                                " no resolution URIs for dynamic resolution of event type aliases through a plug-in event representation have been defined");
            }

            PlugInEventTypeHandlerContext context;
            foreach (Uri eventTypeURI in resolutionURIs) {
                // Determine a list of event representations that may handle this type
                Map<Uri, Object> allFactories = new HashMap<Uri, Object>();
                foreach (KeyValuePair<Uri, PlugInEventRepresentation> item in plugInRepresentations) {
                    allFactories[item.Key] = item.Value;
                }

                ICollection<KeyValuePair<Uri, Object>> factories = URIUtil.FilterSort(eventTypeURI, allFactories);

                if (CollectionHelper.IsEmpty(factories)) {
                    continue;
                }

                // Ask each in turn to accept the type (the process of resolving the type)
                foreach (KeyValuePair<Uri, Object> entry in factories) {
                    PlugInEventRepresentation factory = (PlugInEventRepresentation) entry.Value;
                    context = new PlugInEventTypeHandlerContext(eventTypeURI, initializer, alias);
                    if (factory.AcceptsType(context)) {
                        handlingFactory = factory;
                        handledEventTypeURI = eventTypeURI;
                        break;
                    }
                }

                if (handlingFactory != null) {
                    break;
                }
            }

            if (handlingFactory == null) {
                throw new EventAdapterException("Event type named '" + alias + "' could not be created as none of the " +
                                                "registered plug-in event representations accepts any of the resolution URIs '" +
                                                CollectionHelper.Render(resolutionURIs)
                                                + "' and initializer");
            }

            context = new PlugInEventTypeHandlerContext(handledEventTypeURI, initializer, alias);
            PlugInEventTypeHandler handler = handlingFactory.GetTypeHandler(context);
            if (handler == null) {
                throw new EventAdapterException("Event type named '" + alias +
                                                "' could not be created as no handler was returned");
            }

            EventType eventType = handler.EventType;
            aliasToTypeMap.Put(alias, eventType);
            aliasToHandlerMap.Put(alias, handler);

            return eventType;
        }

        public EventSender GetStaticTypeEventSender(EPRuntimeEventSender runtimeEventSender, String eventTypeAlias)
        {
            EventType eventType = aliasToTypeMap.Get(eventTypeAlias);
            if (eventType == null) {
                throw new EventTypeException("Event type named '" + eventTypeAlias + "' could not be found");
            }

            // handle built-in types
            if (eventType is BeanEventType) {
                return EventSenderBean.Create(runtimeEventSender, (BeanEventType) eventType);
            }
            if (eventType is MapEventType) {
                return EventSenderMap.Create(runtimeEventSender, (MapEventType) eventType);
            }
            if (eventType is BaseXMLEventType) {
                return EventSenderXMLDOM.Create(runtimeEventSender, (BaseXMLEventType) eventType);
            }

            PlugInEventTypeHandler handlers = aliasToHandlerMap.Get(eventTypeAlias);
            if (handlers != null) {
                return handlers.GetSender(runtimeEventSender);
            }
            throw new EventTypeException("An event sender for event type named '" + eventTypeAlias +
                                         "' could not be created as the type is internal");
        }

        /// <summary>
        /// Gets the dynamic type event sender.
        /// </summary>
        /// <param name="epRuntime">The ep runtime.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public EventSender GetDynamicTypeEventSender(EPRuntimeEventSender epRuntime, IEnumerable<Uri> uri)
        {
            List<EventSenderURIDesc> handlingFactories = new List<EventSenderURIDesc>();
            foreach (Uri resolutionURI in uri) {
                // Determine a list of event representations that may handle this type
                Map<Uri, Object> allFactories = new HashMap<Uri, Object>();
                foreach( KeyValuePair<Uri, PlugInEventRepresentation> item in plugInRepresentations ) {
                    allFactories[item.Key] = item.Value;
                }
                
                ICollection<KeyValuePair<Uri, Object>> factories = URIUtil.FilterSort(resolutionURI, allFactories);

                if (CollectionHelper.IsEmpty(factories)) {
                    continue;
                }

                // Ask each in turn to accept the type (the process of resolving the type)
                foreach (KeyValuePair<Uri, Object> entry in factories) {
                    PlugInEventRepresentation factory = (PlugInEventRepresentation) entry.Value;
                    PlugInEventBeanReflectorContext context = new PlugInEventBeanReflectorContext(resolutionURI);
                    if (factory.AcceptsEventBeanResolution(context)) {
                        PlugInEventBeanFactory beanFactory = factory.GetEventBeanFactory(context);
                        if (beanFactory == null) {
                            log.Warn("Plug-in event representation returned a null bean factory, ignoring entry");
                            continue;
                        }
                        EventSenderURIDesc desc = new EventSenderURIDesc(beanFactory, resolutionURI, entry.Key);
                        handlingFactories.Add(desc);
                    }
                }
            }

            if (CollectionHelper.IsEmpty(handlingFactories)) {
                throw new EventTypeException("Event sender for resolution URIs '" + CollectionHelper.Render(uri)
                                             + "' did not return at least one event representation's event factory");
            }

            return EventSenderImpl.Create(handlingFactories, epRuntime);
        }

        /// <summary>
        /// Gets the bean event type factory.
        /// </summary>

        public BeanEventTypeFactory BeanEventTypeFactory
        {
            get { return beanEventAdapter; }
        }

        /// <summary>
        /// Set the legacy type information.
        /// </summary>
        public Map<String, ConfigurationEventTypeLegacy> TypeLegacyConfigs
        {
            set { beanEventAdapter.TypeToLegacyConfigs = value; }
        }

        /// <summary>
        /// Sets the default property resolution style.
        /// </summary>
        /// <value>The default property resolution style.</value>
        public PropertyResolutionStyle DefaultPropertyResolutionStyle
        {
            get { return beanEventAdapter.DefaultPropertyResolutionStyle; }
            set { beanEventAdapter.DefaultPropertyResolutionStyle = value; }
        }

        /// <summary>
        /// Gets or sets the alias resolver.
        /// </summary>
        /// <value>The alias resolver.</value>
        public EPAliasResolver AliasResolver
        {
            get { return aliasResolver; }
            set { aliasResolver = value; }
        }

        /// <summary>
        /// Gets the exists type by alias.
        /// </summary>
        /// <param name="eventTypeAlias">The event type alias.</param>
        /// <returns></returns>
        public EventType GetEventTypeByAlias(String eventTypeAlias)
        {
            if (eventTypeAlias == null)
            {
                throw new IllegalStateException("Null event type alias parameter");
            }

            EventType eventType = aliasToTypeMap.Get(eventTypeAlias);
            if ( eventType == null )
            {
                // This is an extension made to Nesper to support dynamic
                //   resolution of aliases.  We need to talk to the esper team
                //   about incorporating this into esper.

                if (aliasResolver != null)
                {
                    Type tempType = aliasResolver(eventTypeAlias);
                    if ( tempType != null ) {
                        eventType = AddBeanType(eventTypeAlias, tempType);
                    }
                }
            }

            return eventType;
        }

        /// <summary>
        /// Add an alias and class as an event type.
        /// </summary>
        /// <param name="eventTypeAlias">is the alias</param>
        /// <param name="clazz">is the type to add</param>
        /// <returns>event type</returns>
        /// <throws>EventAdapterException to indicate an error constructing the type</throws>
        public EventType AddBeanType(String eventTypeAlias, Type clazz)
        {
            using (syncLock.Acquire())
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(".addBeanType Adding " + eventTypeAlias + " for type " + clazz.FullName);
                }

                EventType existingType = aliasToTypeMap.Get(eventTypeAlias);
                if (existingType != null)
                {
                    if (existingType.UnderlyingType == clazz)
                    {
                        return existingType;
                    }

                    throw new EventAdapterException("Event type named '" + eventTypeAlias +
                                                    "' has already been declared with differing column name or type information");
                }

                EventType eventType = beanEventAdapter.CreateBeanType(eventTypeAlias, clazz);
                aliasToTypeMap.Put(eventTypeAlias, eventType);

                return eventType;
            }
        }

        /// <summary>
        /// Create an event bean given an event of object id.
        /// </summary>
        /// <param name="_event">is the event class</param>
        /// <returns>event</returns>
        public EventBean AdapterForBean(Object _event)
        {
            EventType eventType = typesPerBean.Get(_event.GetType());
            if (eventType == null)
            {
                // This will update the typesPerBean mapping
                eventType = beanEventAdapter.CreateBeanType(_event.GetType().FullName, _event.GetType());
            }
            return new BeanEventBean(_event, eventType);
        }

        /// <summary>
        /// Add an event type for the given type name.
        /// </summary>
        /// <param name="eventTypeAlias">is the alias</param>
        /// <param name="fullyQualClassName">is the type name</param>
        /// <param name="considerAutoAlias">if set to <c>true</c> [consider auto alias].</param>
        /// <returns>event type</returns>
        /// <throws>
        /// EventAdapterException if the Class name cannot resolve or other error occured
        /// </throws>
        public EventType AddBeanType(String eventTypeAlias, String fullyQualClassName, bool considerAutoAlias)
        {
            using (syncLock.Acquire())
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(".addBeanType Adding " + eventTypeAlias + " for type " + fullyQualClassName);
                }

                EventType existingType = aliasToTypeMap.Get(eventTypeAlias);
                if (existingType != null)
                {
                    if (existingType.UnderlyingType.FullName == fullyQualClassName)
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(".addBeanType Returning existing type for " + eventTypeAlias);
                        }
                        return existingType;
                    }

                    throw new EventAdapterException("Event type named '" + eventTypeAlias +
                                                    "' has already been declared with differing column name or type information");
                }

                // Try to resolve as a fully-qualified class name first
                Type type = null;
                try
                {
                    type = TypeHelper.ResolveType(fullyQualClassName);
                }
                catch (TypeLoadException ex)
                {
                    if (!considerAutoAlias)
                    {
                        throw new EventAdapterException("Failed to load class " + fullyQualClassName, ex);
                    }

                    // Attempt to resolve from auto-alias packages
                    foreach (String @namespace in namespaces)
                    {
                        String generatedClassName = @namespace + "." + fullyQualClassName;

                        Type resolvedType = TypeHelper.ResolveType(generatedClassName, false);
                        if (resolvedType == null)
                        {
                            continue; // expected, class may not exists in all packages
                        }

                        if (type != null)
                        {
                            throw new EventAdapterException(
                                "Failed to resolve alias '" + eventTypeAlias +
                                "', the class was ambigously found both in " +
                                "package '" + type.Namespace + "' and in " +
                                "package '" + resolvedType.Namespace + "'", ex);
                        }

                        type = resolvedType;
                    }

                    if (type == null)
                    {
                        throw new EventAdapterException("Failed to load class " + fullyQualClassName, ex);
                    }
                }
                EventType eventType = beanEventAdapter.CreateBeanType(eventTypeAlias, type);
                aliasToTypeMap[eventTypeAlias] = eventType;

                return eventType;
            }
        }

        /// <summary>
        /// Add an event type with the given alias and a given set of properties.
        /// If the alias already exists with the same event property information, returns the
        /// existing EventType instance.
        /// If the alias already exists with different event property information, throws an exception.
        /// If the alias does not already exists, adds the alias and constructs a new <see cref="com.espertech.esper.events.MapEventType"/>.
        /// </summary>
        /// <param name="eventTypeAlias">is the alias name for the event type</param>
        /// <param name="propertyTypes">is the names and types of event properties</param>
        /// <returns>event type is the type added</returns>
        /// <throws>  EventAdapterException if alias already exists and doesn't match property type info </throws>
        public EventType AddMapType(String eventTypeAlias, Map<String, Type> propertyTypes)
        {
            using (syncLock.Acquire())
            {
                MapEventType newEventType = new MapEventType(eventTypeAlias, propertyTypes, this);

                EventType existingType = aliasToTypeMap.Get(eventTypeAlias);
                if (existingType != null)
                {
                    // The existing type must be the same as the type createdStatement
                    if (!Equals(newEventType,existingType))
                    {
                        throw new EventAdapterException("Event type named '" + eventTypeAlias +
                                                        "' has already been declared with differing column name or type information");
                    }

                    // Since it's the same, return the existing type
                    return existingType;
                }

                aliasToTypeMap.Put(eventTypeAlias, newEventType);

                return newEventType;
            }
        }

        public EventType AddNestableMapType(String eventTypeAlias, Map<String, Object> propertyTypes)
        {
            using (syncLock.Acquire()) {
                MapEventType newEventType = new MapEventType(eventTypeAlias, this, propertyTypes);

                EventType existingType = aliasToTypeMap.Get(eventTypeAlias);
                if (existingType != null) {
                    // The existing type must be the same as the type createdStatement
                    if (!newEventType.Equals(existingType)) {
                        throw new EventAdapterException("Event type named '" + eventTypeAlias +
                                                        "' has already been declared with differing column name or type information");
                    }

                    // Since it's the same, return the existing type
                    return existingType;
                }

                aliasToTypeMap.Put(eventTypeAlias, newEventType);

                return newEventType;
            }
        }

        /// <summary>
        /// Adapters for map.
        /// </summary>
        /// <param name="_event">The _event.</param>
        /// <param name="eventTypeAlias">The event type alias.</param>
        /// <returns></returns>
        public EventBean AdapterForMap(Map<String, Object> _event, String eventTypeAlias)
        {
            EventType existingType = aliasToTypeMap.Get(eventTypeAlias);
            if (existingType == null)
            {
                throw new EventAdapterException("Event type alias '" + eventTypeAlias + "' has not been defined");
            }

            //Map<String, Object> eventMap = (EDictionary<String, Object>)_event;
            return CreateMapFromValues(_event, existingType);
        }

        /// <summary>
        /// Returns an adapter for the XML DOM document that exposes it's data as event properties for use in statements.
        /// </summary>
        /// <param name="node">is the node to wrap</param>
        /// <returns>event wrapper for document</returns>
        public EventBean AdapterForDOM(XmlNode node)
        {
            XmlNode namedNode;

            if (node is XmlDocument)
            {
                namedNode = ((XmlDocument) node).DocumentElement;
            }
            else if (node is XmlElement)
            {
                namedNode = node;
            }
            else
            {
                throw new EPException("Unexpected DOM node of type '" + node.GetType() + "' encountered, please supply a Document or Element node");
            }

            String rootElementName = namedNode.LocalName;
            if (rootElementName == null)
            {
                rootElementName = namedNode.Name;
            }

            EventType eventType = xmldomRootElementNames.Get(rootElementName);
            if (eventType == null)
            {
                throw new EventAdapterException("DOM event root element name '" + rootElementName +
                        "' has not been configured");
            }

            return new XMLEventBean(node, eventType);
        }

        /// <summary>Add a configured XML DOM event type.</summary>
        /// <param name="eventTypeAlias">is the alias name of the event type</param>
        /// <param name="configurationEventTypeXMLDOM">
        /// configures the event type schema and namespace and XPath
        /// property information.
        /// </param>
        public EventType AddXMLDOMType(String eventTypeAlias, ConfigurationEventTypeXMLDOM configurationEventTypeXMLDOM)
        {
            using (syncLock.Acquire())
            {
                if (configurationEventTypeXMLDOM.RootElementName == null)
                {
                    throw new EventAdapterException("Required root element name has not been supplied");
                }

                EventType existingType = aliasToTypeMap.Get(eventTypeAlias);
                if (existingType != null)
                {
                    String message = "Event type named '" + eventTypeAlias +
                                     "' has already been declared with differing column name or type information";
                    if (!(existingType is BaseXMLEventType))
                    {
                        throw new EventAdapterException(message);
                    }
                    ConfigurationEventTypeXMLDOM config =
                        ((BaseXMLEventType) existingType).ConfigurationEventTypeXMLDOM;
                    if (!Equals(config, configurationEventTypeXMLDOM))
                    {
                        throw new EventAdapterException(message);
                    }

                    return existingType;
                }

                EventType type;
                if (configurationEventTypeXMLDOM.SchemaResource == null)
                {
                    type = new SimpleXMLEventType(configurationEventTypeXMLDOM);
                }
                else
                {
                    type = new SchemaXMLEventType(configurationEventTypeXMLDOM);
                }

                aliasToTypeMap.Put(eventTypeAlias, type);
                xmldomRootElementNames.Put(configurationEventTypeXMLDOM.RootElementName, type);

                return type;
            }
        }

        /// <summary>
        /// Create an event wrapper bean from a set of event properties (name and value objectes) stored in a Map.
        /// </summary>
        /// <param name="properties">is key-value pairs for the event properties</param>
        /// <param name="eventType">is the type metadata for any maps of that type</param>
        /// <returns>EventBean instance</returns>
        public EventBean CreateMapFromValues(Map<String, Object> properties, EventType eventType)
        {
            return new MapEventBean(properties, eventType);
        }

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
        public EventType AddWrapperType(String eventTypeAlias, EventType underlyingEventType, DataMap propertyTypes)
        {
            using (syncLock.Acquire())
            {
                // If we are wrapping an underlying type that is itself a wrapper, then this is a special case
                if (underlyingEventType is WrapperEventType)
                {
                    WrapperEventType underlyingWrapperType = (WrapperEventType) underlyingEventType;

                    // the underlying type becomes the type already wrapped
                    // properties are a superset of the wrapped properties and the additional properties
                    underlyingEventType = underlyingWrapperType.UnderlyingEventType;
                    DataMap propertiesSuperset = new HashMap<string,object>();
                    propertiesSuperset.PutAll(underlyingWrapperType.UnderlyingMapType.Types);
                    propertiesSuperset.PutAll(propertyTypes);
                    propertyTypes = propertiesSuperset;
                }

                WrapperEventType newEventType = new WrapperEventType(eventTypeAlias, underlyingEventType, propertyTypes, this);

                EventType existingType = aliasToTypeMap.Get(eventTypeAlias);
                if (existingType != null)
                {
                    // The existing type must be the same as the type created
                    if (!Equals(newEventType, existingType))
                    {
                        // It is possible that the wrapped event type is compatible: a child type of the desired type
                        if (IsCompatibleWrapper(existingType, underlyingEventType, propertyTypes))
                        {
                            return existingType;
                        }

                        throw new EventAdapterException("Event type named '" + eventTypeAlias +
                                "' has already been declared with differing column name or type information");
                    }

                    // Since it's the same, return the existing type
                    return existingType;
                }

                aliasToTypeMap.Put(eventTypeAlias, newEventType);

                return newEventType;
            }
        }

        /// <summary>Returns true if the wrapper type is compatible with an existing wrapper type, for the reason thatthe underlying event is a subtype of the existing underlying wrapper's type.</summary>
        /// <param name="existingType">is the existing wrapper type</param>
        /// <param name="underlyingType">is the proposed new wrapper type's underlying type</param>
        /// <param name="propertyTypes">is the additional properties</param>
        /// <returns>true for compatible, or false if not</returns>
        public static bool IsCompatibleWrapper(EventType existingType, EventType underlyingType, DataMap propertyTypes)
        {
            if (!(existingType is WrapperEventType))
            {
                return false;
            }
            WrapperEventType existingWrapper = (WrapperEventType) existingType;

            if (!(MapEventType.IsDeepEqualsProperties(existingWrapper.UnderlyingMapType.Types, propertyTypes)))
            {
                return false;
            }
            EventType existingUnderlyingType = existingWrapper.UnderlyingEventType;

            // If one of the supertypes of the underlying type is the existing underlying type, we are compatible
            if (underlyingType.SuperTypes == null)
            {
                return false;
            }
            foreach (EventType superUnderlying in underlyingType.SuperTypes)
            {
                if (superUnderlying == existingUnderlyingType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a new anonymous EventType instance for an event type that contains a map of name value pairs.
        /// The method accepts a Map that contains the property names as keys and Class objects as the values.
        /// The Class instances represent the property types.
        /// <para>
        /// New instances are created by this method on every invocation. Clients to this method need to
        /// cache the returned EventType instance to reuse EventType's for same-typed events.
        /// </para>
        /// </summary>
        /// <param name="propertyTypes">is a map of String to Class objects</param>
        /// <returns>
        /// EventType implementation for map field names and value types
        /// </returns>
        public EventType CreateAnonymousMapType(DataMap propertyTypes)
        {
            String alias = UuidGenerator.Generate(propertyTypes);
            return new MapEventType(alias, this, propertyTypes);
        }

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
        public EventType CreateAnonymousWrapperType(EventType underlyingEventType, DataMap propertyTypes)
        {
            String alias = UuidGenerator.Generate(propertyTypes);
            return new WrapperEventType(alias, underlyingEventType, propertyTypes, null);
        }

        /// <summary>
        /// Creates the type of the add to event.
        /// </summary>
        /// <param name="originalType">Type of the original.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <param name="fieldTypes">The field types.</param>
        /// <returns></returns>
        public EventType CreateAddToEventType(EventType originalType, IList<String> fieldNames, IList<Type> fieldTypes)
        {
            DataMap types = new HashMap<string,object>();

            // Copy properties of original event, add property value
            foreach (String property in originalType.PropertyNames)
            {
                types.Put(property, originalType.GetPropertyType(property));
            }

            // Copy new properties
            for (int i = 0; i < fieldNames.Count; i++)
            {
                types.Put(fieldNames[i], fieldTypes[i]);
            }

            return CreateAnonymousMapType(types);
        }

        /// <summary>
        /// Creates an unnamed composite event type with event properties that are name-value pairs
        /// with values being other event types. Pattern statement generate events of such type.
        /// </summary>
        /// <param name="taggedEventTypes">is a map of name keys and event type values</param>
        /// <returns>
        /// event type representing a composite event
        /// </returns>
        public EventType CreateAnonymousCompositeType(Map<String, Pair<EventType, String>> taggedEventTypes)
        {
            String alias = UuidGenerator.Generate(taggedEventTypes);
            return new CompositeEventType(alias, taggedEventTypes);
        }

        /// <summary>
        /// Create a wrapper around an event and some additional properties
        /// </summary>
        /// <param name="_event">is the wrapped event</param>
        /// <param name="properties">are the additional properties</param>
        /// <param name="eventType">os the type metadata for any wrappers of this type</param>
        /// <returns>wrapper event bean</returns>
        public EventBean CreateWrapper(EventBean _event, DataMap properties, EventType eventType)
        {
            if (_event is WrapperEventBean)
            {
                WrapperEventBean wrapper = (WrapperEventBean) _event;
                properties.PutAll(wrapper.DecoratingProperties);
                return new WrapperEventBean(wrapper.UnderlyingEvent, properties, eventType);
            }
            else
            {
                return new WrapperEventBean(_event, properties, eventType);
            }
        }

        /// <summary>
        /// Creates a wrapper for a composite event type. The wrapper wraps an event that
        /// consists of name-value pairs in which the values are other already-wrapped events.
        /// </summary>
        /// <param name="eventType">is the composite event type</param>
        /// <param name="taggedEvents">is the name-event map</param>
        /// <returns>wrapper for composite event</returns>
        public EventBean AdapterForCompositeEvent(EventType eventType, Map<String, EventBean> taggedEvents)
        {
            return new CompositeEventBean(taggedEvents, eventType);
        }

        /// <summary>
        /// Adds a namespace where types reside in.
        /// </summary>
        /// <param name="namespace">A namespace where type reside.</param>
        public void AddAutoAliasPackage(String @namespace)
        {
            namespaces.Add(@namespace);
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
