using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.events.property;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Implementation of the EventType interface for handling type classes.
    /// </summary>

    public class BeanEventType : EventType
    {
        private readonly Type type;
        private readonly BeanEventTypeFactory beanEventTypeFactory;
        private readonly ConfigurationEventTypeLegacy optionalLegacyDef;
		private readonly String alias;

        private String[] propertyNames;
        private Map<string, SimplePropertyInfo> simpleProperties;

        private Map<String, EventPropertyDescriptor> mappedPropertyDescriptors;
        private Map<String, EventPropertyDescriptor> indexedPropertyDescriptors;
        private IList<EventType> superTypes;
        private ICollection<EventType> deepSuperTypes;

        private readonly PropertyResolutionStyle propertyResolutionStyle;

        private Map<String, IList<SimplePropertyInfo>> simpleSmartPropertyTable;
        private Map<String, IList<SimplePropertyInfo>> indexedSmartPropertyTable;
        private Map<String, IList<SimplePropertyInfo>> mappedSmartPropertyTable;

        internal class SimplePropertyInfo
        {
            public Type type;
            public EventPropertyGetter getter;
            public EventPropertyDescriptor descriptor;

            internal SimplePropertyInfo()
            {
            }
            
            internal SimplePropertyInfo(
                Type type,
                EventPropertyGetter getter, 
                EventPropertyDescriptor descriptor )
            {
                this.type = TypeHelper.GetBoxedType(type);
                this.getter = getter;
                this.descriptor = descriptor;
            }
        }

        /// <summary>Constructor takes a object type as an argument.</summary>
        /// <param name="type">the type of an object</param>
        /// <param name="beanEventTypeFactory">the cache and factory for event bean types and event wrappers</param>
        /// <param name="optionalLegacyDef">optional configuration supplying legacy event type information</param>
        /// <param name="alias">the event type alias for the class</param>

        public BeanEventType(Type type,
                             BeanEventTypeFactory beanEventTypeFactory,
                             ConfigurationEventTypeLegacy optionalLegacyDef,
                             String alias)
        {
            this.type = type;
            this.beanEventTypeFactory = beanEventTypeFactory;
            this.optionalLegacyDef = optionalLegacyDef;
            this.alias = alias;
            this.propertyResolutionStyle =
                this.optionalLegacyDef != null
                    ? optionalLegacyDef.PropertyResolutionStyle
                    : beanEventTypeFactory.DefaultPropertyResolutionStyle;

            Initialize();
        }
		
		/// <summary>
		/// Gets the event alias
		/// </summary>
		
		public String Alias
		{
			get { return alias; }
		}

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>

        public Type GetPropertyType(String propertyName)
        {
            SimplePropertyInfo propertyInfo = GetSimplePropertyInfo(propertyName);
            if (( propertyInfo != null ) && ( propertyInfo.type != null ))
            {
                return propertyInfo.type;
            }

            Property prop = PropertyParser.Parse(propertyName, beanEventTypeFactory, false);
            if (prop is SimpleProperty)
            {
                // there is no such property since it wasn't in simplePropertyTypes
                return null;
            }
            return prop.GetPropertyType(this);
        }

        /// <summary>
        /// Determines whether the specified property name is property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property name is property; otherwise, <c>false</c>.
        /// </returns>

        public Boolean IsProperty(String propertyName)
        {
            if (GetPropertyType(propertyName) == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the type of the underlying.
        /// </summary>
        /// <returns></returns>

        public Type UnderlyingType
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the getter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>

        public EventPropertyGetter GetGetter(String propertyName)
        {
            SimplePropertyInfo propertyInfo = GetSimplePropertyInfo(propertyName);
            if (( propertyInfo != null ) && ( propertyInfo.getter != null ))
            {
                return propertyInfo.getter;
            }

            Property prop = PropertyParser.Parse(propertyName, beanEventTypeFactory, false);
            if (prop is SimpleProperty)
            {
                // there is no such property since it wasn't in simplePropertyGetters
                return null;
            }
            return prop.GetGetter(this);
        }

        /// <summary>
        /// Looks up and returns a cached simple property's descriptor.
        /// </summary>
        /// <param name="propertyName">propertyName to look up</param>
        /// <returns>property descriptor</returns>

        public EventPropertyDescriptor GetSimpleProperty(String propertyName)
        {
            SimplePropertyInfo propertyInfo = GetSimplePropertyInfo(propertyName);
            return
                propertyInfo != null
                    ? propertyInfo.descriptor
                    : null;
        }

        /// <summary>
        /// Gets the resolution style.
        /// </summary>
        /// <value>The resolution style.</value>
        private PropertyResolutionStyle ResolutionStyle
        {
            get
            {
                return propertyResolutionStyle;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the bean type uses a smart resolution style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [uses smart resolution style]; otherwise, <c>false</c>.
        /// </value>
        private bool UsesSmartResolutionStyle
        {
            get
            {
                switch (ResolutionStyle)
                {
                    case PropertyResolutionStyle.CASE_SENSITIVE:
                        return false;
                    case PropertyResolutionStyle.CASE_INSENSITIVE:
                    case PropertyResolutionStyle.DISTINCT_CASE_INSENSITIVE:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets the simple property info.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private SimplePropertyInfo GetSimplePropertyInfo(String propertyName)
        {
            SimplePropertyInfo propertyInfo;
            IList<SimplePropertyInfo> simplePropertyInfoList;

            switch( ResolutionStyle )
            {
                case PropertyResolutionStyle.CASE_SENSITIVE:
                    return simpleProperties.Get(propertyName);
                case PropertyResolutionStyle.CASE_INSENSITIVE:
                    propertyInfo = simpleProperties.Get(propertyName);
                    if (propertyInfo != null)
                    {
                        return propertyInfo;
                    }

                    simplePropertyInfoList = simpleSmartPropertyTable.Get(propertyName.ToLowerInvariant());
                    return
                        simplePropertyInfoList != null
                            ? simplePropertyInfoList[0]
                            : null;

                case PropertyResolutionStyle.DISTINCT_CASE_INSENSITIVE:
                    propertyInfo = simpleProperties.Get(propertyName);
                    if (propertyInfo != null)
                    {
                        return propertyInfo;
                    }

                    simplePropertyInfoList = simpleSmartPropertyTable.Get(propertyName.ToLowerInvariant());
                    if (simplePropertyInfoList != null)
                    {
                        if (simplePropertyInfoList.Count != 1)
                        {
                            throw new EPException("Unable to determine which property to use for \"" + propertyName + "\" because more than one property matched");
                        }

                        return simplePropertyInfoList[0];
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// Looks up and returns a cached mapped property's descriptor.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>

        public EventPropertyDescriptor GetMappedProperty(String propertyName)
        {
            switch( ResolutionStyle )
            {
                case PropertyResolutionStyle.CASE_SENSITIVE:
                    return mappedPropertyDescriptors.Get(propertyName);
                case PropertyResolutionStyle.CASE_INSENSITIVE:
                    {
                        IList<SimplePropertyInfo> propertyInfos =
                            mappedSmartPropertyTable.Get(propertyName.ToLowerInvariant());
                        return propertyInfos != null
                                   ? propertyInfos[0].descriptor
                                   : null;
                    }
                case PropertyResolutionStyle.DISTINCT_CASE_INSENSITIVE:
                    {
                        IList<SimplePropertyInfo> propertyInfos =
                            mappedSmartPropertyTable.Get(propertyName.ToLowerInvariant());
                        if (propertyInfos != null)
                        {
                            if (propertyInfos.Count != 1)
                            {
                                throw new EPException("Unable to determine which property to use for \"" + propertyName +
                                                      "\" because more than one property matched");
                            }

                            return propertyInfos[0].descriptor;
                        }

                        break;
                    }
            }

            return null;
        }

        /// <summary>
        /// Looks up and returns a cached indexed property's descriptor.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>

        public EventPropertyDescriptor GetIndexedProperty(String propertyName)
        {
            switch (ResolutionStyle)
            {
                case PropertyResolutionStyle.CASE_SENSITIVE:
                    return indexedPropertyDescriptors.Get(propertyName);
                case PropertyResolutionStyle.CASE_INSENSITIVE:
                    {
                        IList<SimplePropertyInfo> propertyInfos =
                            indexedSmartPropertyTable.Get(propertyName.ToLowerInvariant());
                        return propertyInfos != null
                                   ? propertyInfos[0].descriptor
                                   : null;
                    }
                case PropertyResolutionStyle.DISTINCT_CASE_INSENSITIVE:
                    {
                        IList<SimplePropertyInfo> propertyInfos =
                            indexedSmartPropertyTable.Get(propertyName.ToLowerInvariant());
                        if (propertyInfos != null)
                        {
                            if (propertyInfos.Count != 1)
                            {
                                throw new EPException("Unable to determine which property to use for \"" + propertyName +
                                                      "\" because more than one property matched");
                            }

                            return propertyInfos[0].descriptor;
                        }

                        break;
                    }
            }
            
            return null;
        }

        /// <summary>
        /// Get all valid property names for the event type.
        /// </summary>
        /// <value>The property names.</value>
        /// <returns> A string array containing the property names of this typed event data object.
        /// </returns>
        public ICollection<String> PropertyNames
        {
            get { return propertyNames; }
        }

        /// <summary>
        /// Returns an array of event types that are super to this event type, from which this event type
        /// inherited event properties.  For object instances underlying the event this method returns the
        /// event types for all superclasses extended by the object and all interfaces implemented by the
        /// object.
        /// </summary>
        /// <value></value>
        /// <returns>an array of event types</returns>
        public IEnumerable<EventType> SuperTypes
        {
            get { return superTypes; }
        }

        /// <summary>
        /// Returns enumerable over all super types to event type, going up the hierarchy and including
        /// all interfaces (and their extended interfaces) and superclasses as EventType instances.
        /// </summary>
        /// <value></value>
        public IEnumerable<EventType> DeepSuperTypes
        {
            get { return deepSuperTypes; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return String.Format("BeanEventType type={0}", type.FullName);
        }

        private void Initialize()
        {
            PropertyListBuilder propertyListBuilder = PropertyListBuilderFactory.CreateBuilder(optionalLegacyDef);
            IList<EventPropertyDescriptor> properties = propertyListBuilder.AssessProperties(type);

            propertyNames = new String[properties.Count];
            simpleProperties = new HashMap<String, SimplePropertyInfo>();
            mappedPropertyDescriptors = new HashMap<String, EventPropertyDescriptor>();
            indexedPropertyDescriptors = new HashMap<String, EventPropertyDescriptor>();

            if (UsesSmartResolutionStyle)
            {
                simpleSmartPropertyTable = new HashMap<String, IList<SimplePropertyInfo>>();
                mappedSmartPropertyTable = new HashMap<String, IList<SimplePropertyInfo>>();
                indexedSmartPropertyTable = new HashMap<String, IList<SimplePropertyInfo>>();
            }

            int count = 0;
            foreach (EventPropertyDescriptor desc in properties)
            {
                String propertyName = desc.PropertyName;
                propertyNames[count++] = desc.ListedName;

                switch (desc.PropertyType)
                {
                    case EventPropertyType.SIMPLE:
                        {
                            EventPropertyGetter getter = PropertyHelper.GetGetter(desc.Descriptor);
                            // Create the property info
                            SimplePropertyInfo propertyInfo = new SimplePropertyInfo(desc.ReturnType, getter, desc);
                            // Enter the property into the simple property table
                            simpleProperties[propertyName] = propertyInfo;

                            // Only map into the smart property table if the resolution style
                            // would require it.  Otherwise its just a waste of time and memory.
                            if (UsesSmartResolutionStyle)
                            {
                                // Find the property in the smart property table
                                string smartPropertyName = propertyName.ToLowerInvariant();
                                IList<SimplePropertyInfo> propertyInfoList = simpleSmartPropertyTable.Get(smartPropertyName);
                                if (propertyInfoList == null)
                                {
                                    propertyInfoList = new List<SimplePropertyInfo>();
                                    simpleSmartPropertyTable[smartPropertyName] = propertyInfoList;
                                }
                                // Enter the property into the smart property list
                                propertyInfoList.Add(propertyInfo);
                            }
                        }
                        break;
                    case EventPropertyType.MAPPED:
                        mappedPropertyDescriptors[propertyName] = desc;
                        // Recognize that there may be properties with overlapping case-insentitive names
                        if (UsesSmartResolutionStyle)
                        {
                            // Find the property in the smart property table
                            String smartPropertyName = propertyName.ToLowerInvariant();
                            IList<SimplePropertyInfo> propertyInfoList = mappedSmartPropertyTable.Get(smartPropertyName);
                            if (propertyInfoList == null)
                            {
                                propertyInfoList = new List<SimplePropertyInfo>();
                                mappedSmartPropertyTable[smartPropertyName] = propertyInfoList;
                            }

                            // Enter the property into the smart property list
                            SimplePropertyInfo propertyInfo = new SimplePropertyInfo(desc.ReturnType, null, desc);
                            propertyInfoList.Add(propertyInfo);
                        }
                        break;
                    case EventPropertyType.INDEXED:
                        indexedPropertyDescriptors[propertyName] = desc;
                        // Recognize that there may be properties with overlapping case-insentitive names
                        if (UsesSmartResolutionStyle)
                        {
                            // Find the property in the smart property table
                            String smartPropertyName = propertyName.ToLowerInvariant();
                            IList<SimplePropertyInfo> propertyInfoList = indexedSmartPropertyTable.Get(smartPropertyName);
                            if (propertyInfoList == null)
                            {
                                propertyInfoList = new List<SimplePropertyInfo>();
                                indexedSmartPropertyTable[smartPropertyName] = propertyInfoList;
                            }

                            // Enter the property into the smart property list
                            SimplePropertyInfo propertyInfo = new SimplePropertyInfo(desc.ReturnType, null, desc);
                            propertyInfoList.Add(propertyInfo);
                        }
                        break;
                }
            }

            // Determine event type super types
            superTypes = GetSuperTypes(type, beanEventTypeFactory);

            // Determine deep supertypes
            // Get super types (superclasses and interfaces), deep get of all in the tree
            Set<Type> supers = new HashSet<Type>();
            GetSuper(type, supers);
            RemoveCoreLibInterfaces(supers);    // Remove core library super types

            // Cache the supertypes of this event type for later use
            deepSuperTypes = new HashSet<EventType>();
            foreach (Type superClass in supers)
            {
                EventType superType = beanEventTypeFactory.CreateBeanType(superClass.FullName, superClass);
                deepSuperTypes.Add(superType);
            }
        }

        /// <summary>
        /// Gets the super types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="beanEventTypeFactory">The bean event adapter.</param>
        /// <returns></returns>
        public static IList<EventType> GetSuperTypes(Type type, BeanEventTypeFactory beanEventTypeFactory)
        {
            IList<Type> superclasses = new List<Type>();

            // add superclass
            Type superClass = type.BaseType;
            if (superClass != null)
            {
                superclasses.Add(superClass);
            }

            // Add interfaces.  Under the CLR, implemented interfaces are flattened
            // directly under the type so the hierarchy that exists in Java is broken
            // by this method.  This is something to remember should there be issues.

            Type[] interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                superclasses.Add(interfaces[i]);
            }

            // Build event types, ignoring language types
            IList<EventType> superTypes = new List<EventType>();
            foreach (Type superclass in superclasses)
            {
                if (superclass.Namespace != "System")
                {
                    EventType superType = beanEventTypeFactory.CreateBeanType(superclass.FullName, superclass);
                    superTypes.Add(superType);
                }
            }

            return superTypes;
        }

        /// <summary>
        /// Add the given class's implemented interfaces and superclasses to the
        /// result set of classes.
        /// </summary>
        /// <param name="type">The type to introspect.</param>
        /// <param name="result">The result.</param>

        public static void GetSuper(Type type, Set<Type> result)
        {
            GetSuperInterfaces(type, result);
            GetSuperClasses(type, result);
        }

        private static void GetSuperInterfaces(Type type, Set<Type> result)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                result.Add(interfaceType);
                GetSuperInterfaces(interfaceType, result);
            }
        }

        private static void GetSuperClasses(Type type, Set<Type> result)
        {
            Type superClass = type.BaseType;
            if (superClass == null)
            {
                return;
            }

            result.Add(superClass);
            GetSuper(superClass, result);
        }

        private static void RemoveCoreLibInterfaces(ICollection<Type> classes)
        {
            IList<Type> deadList = new List<Type>();

            foreach (Type type in classes)
            {
                if (type.FullName.StartsWith("System"))
                {
                    deadList.Add(type);
                }
            }

            foreach (Type type in deadList)
            {
                classes.Remove(type);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
